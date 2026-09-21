using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.Net;
using HslCommunication.Profinet.AllenBradley;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.OrientalMotor;

/// <summary>
/// 东方马达的EIP通信协议，适用控制器
/// </summary>
public class OrientalMotorEipNet : NetworkConnectedCip
{
	private byte[] inputBuffer = null;

	private byte[] outputBuffer = new byte[172];

	private Socket socketCore = null;

	private Thread threadReceive = null;

	private int bufferLength = 2048;

	private bool IsStarted = false;

	private EndPoint udpEndPoint = null;

	private int returnId = 0;

	private DateTime bufferReceiveTime = DateTime.Now;

	/// <summary>
	/// 获取或设置CIP I/O 模式下的端口信息
	/// </summary>
	public int CipIoPort { get; private set; }

	/// <summary>
	/// 回发消息的时候RunIdle
	/// </summary>
	public uint RunIdleHeader { get; set; } = 1u;

	/// <summary>
	/// RPI的时间，单位是毫秒，默认100ms
	/// </summary>
	public uint RPITime { get; set; } = 100u;

	/// <summary>
	/// EIP协议超时时间，单位是秒，只能设置2得指数倍，例如1,2,4,8,16,32,64,128,256等，默认2秒
	/// </summary>
	public int ActualTimeout { get; set; } = 2;

	/// <summary>
	/// 收到数据的事件，如果是172字节长度，则是Input数据，其他长度的数据则不处理
	/// </summary>
	public event EventHandler<OrientalMotorEventArgs> OnReceiveData;

	/// <inheritdoc cref="M:HslCommunication.Core.Device.DeviceTcpNet.#ctor" />
	public OrientalMotorEipNet()
	{
		base.WordLength = 2;
		bufferReceiveTime = DateTime.Now;
	}

	/// <inheritdoc cref="M:HslCommunication.Core.Device.DeviceTcpNet.#ctor(System.String,System.Int32)" />
	public OrientalMotorEipNet(string ip, int port)
		: this()
	{
		IpAddress = ip;
		Port = port;
	}

	private int GetTickTimes(int actualTimeout)
	{
		int num = 0;
		for (int num2 = 1; num2 < actualTimeout; num2 *= 2)
		{
			num++;
		}
		return num;
	}

	private byte[] GetHandleData()
	{
		byte[] array = "000000000000020000000000b2003a0054022006240103fa0000000001004757010000000000000000000000a0860100b248a0860100ae4801083404bb002b00fb13010020042c652c64".ToHexBytes();
		base.TOConnectionId = (uint)HslHelper.HslRandom.Next();
		BitConverter.GetBytes(base.TOConnectionId).CopyTo(array, 28);
		BitConverter.GetBytes(RPITime * 1000).CopyTo(array, 44);
		BitConverter.GetBytes(RPITime * 1000).CopyTo(array, 50);
		array[22] = (byte)GetTickTimes(ActualTimeout * 4);
		return array;
	}

	/// <inheritdoc />
	protected override OperateResult InitializationOnConnect()
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(CommunicationPipe, AllenBradleyHelper.RegisterSessionHandle(), hasResponseData: true, usePackAndUnpack: false);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult operateResult2 = AllenBradleyHelper.CheckResponse(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		base.SessionHandle = base.ByteTransform.TransUInt32(operateResult.Content, 4);
		OperateResult<byte[]> operateResult3 = ReadFromCoreServer(CommunicationPipe, AllenBradleyHelper.PackRequestHeader(111, base.SessionHandle, GetHandleData()), hasResponseData: true, usePackAndUnpack: false);
		if (!operateResult3.IsSuccess)
		{
			return operateResult3;
		}
		base.OTConnectionId = BitConverter.ToUInt32(operateResult3.Content, 44);
		CipIoPort = operateResult3.Content[96] * 256 + operateResult3.Content[97];
		returnId = 0;
		OperateResult operateResult4 = StartUdpServer();
		if (!operateResult4.IsSuccess)
		{
			return operateResult4;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <inheritdoc />
	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		OperateResult<byte[]> read1 = await ReadFromCoreServerAsync(CommunicationPipe, AllenBradleyHelper.RegisterSessionHandle(), hasResponseData: true, usePackAndUnpack: false).ConfigureAwait(continueOnCapturedContext: false);
		if (!read1.IsSuccess)
		{
			return read1;
		}
		OperateResult check = AllenBradleyHelper.CheckResponse(read1.Content);
		if (!check.IsSuccess)
		{
			return check;
		}
		base.SessionHandle = base.ByteTransform.TransUInt32(read1.Content, 4);
		OperateResult<byte[]> read2 = await ReadFromCoreServerAsync(CommunicationPipe, AllenBradleyHelper.PackRequestHeader(111, base.SessionHandle, GetHandleData()), hasResponseData: true, usePackAndUnpack: false).ConfigureAwait(continueOnCapturedContext: false);
		if (!read2.IsSuccess)
		{
			return read2;
		}
		base.OTConnectionId = BitConverter.ToUInt32(read2.Content, 44);
		CipIoPort = read2.Content[96] * 256 + read2.Content[97];
		returnId = 0;
		OperateResult udpStart = StartUdpServer();
		if (!udpStart.IsSuccess)
		{
			return udpStart;
		}
		return OperateResult.CreateSuccessResult();
	}

	/// <inheritdoc />
	protected override OperateResult ExtraOnDisconnect()
	{
		IsStarted = false;
		if (socketCore != null)
		{
			NetSupport.CloseSocket(socketCore);
		}
		return base.ExtraOnDisconnect();
	}

	/// <inheritdoc />
	protected override async Task<OperateResult> ExtraOnDisconnectAsync()
	{
		IsStarted = false;
		if (socketCore != null)
		{
			NetSupport.CloseSocket(socketCore);
		}
		return await base.ExtraOnDisconnectAsync();
	}

	/// <inheritdoc cref="M:HslCommunication.Core.IReadWriteNet.Read(System.String,System.UInt16)" />
	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		if (base.ReceiveTimeOut > 0 && (DateTime.Now - bufferReceiveTime).TotalMilliseconds > (double)base.ReceiveTimeOut)
		{
			return new OperateResult<byte[]>("No input data received within the timeout period");
		}
		if (address.StartsWith("input", StringComparison.OrdinalIgnoreCase))
		{
			if (inputBuffer != null)
			{
				int num = HslHelper.ExtractStartIndex(ref address);
				if (num < 0)
				{
					num = 0;
				}
				if (num + length > inputBuffer.Length)
				{
					return new OperateResult<byte[]>("Read length exceeds the buffer size");
				}
				return OperateResult.CreateSuccessResult(inputBuffer.SelectMiddle(num, length));
			}
			return new OperateResult<byte[]>("No input data received yet");
		}
		if (address.StartsWith("output", StringComparison.OrdinalIgnoreCase))
		{
			int num2 = HslHelper.ExtractStartIndex(ref address);
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 + length > outputBuffer.Length)
			{
				return new OperateResult<byte[]>("Read length exceeds the buffer size");
			}
			return OperateResult.CreateSuccessResult(outputBuffer.SelectMiddle(num2, length));
		}
		return new OperateResult<byte[]>("Only support read input data, address should start with 'input'");
	}

	/// <inheritdoc cref="M:HslCommunication.Core.IReadWriteNet.ReadAsync(System.String,System.UInt16)" />
	public override async Task<OperateResult<byte[]>> ReadAsync(string address, ushort length)
	{
		return await Task.FromResult(Read(address, length));
	}

	/// <inheritdoc cref="M:HslCommunication.Core.IReadWriteNet.Write(System.String,System.Byte[])" />
	[HslMqttApi("WriteByteArray", "")]
	public override OperateResult Write(string address, byte[] value)
	{
		if (base.ReceiveTimeOut > 0 && (DateTime.Now - bufferReceiveTime).TotalMilliseconds > (double)base.ReceiveTimeOut)
		{
			return new OperateResult("No input data received within the timeout period");
		}
		if (address.StartsWith("output", StringComparison.OrdinalIgnoreCase))
		{
			int num = HslHelper.ExtractStartIndex(ref address);
			if (num < 0)
			{
				num = 0;
			}
			if (num + value.Length > outputBuffer.Length)
			{
				return new OperateResult<byte[]>("Write length exceeds the buffer size");
			}
			Array.Copy(value, 0, outputBuffer, num, value.Length);
			return OperateResult.CreateSuccessResult();
		}
		return new OperateResult<byte[]>("Only support write output data, address should start with 'output'");
	}

	/// <inheritdoc cref="M:HslCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Byte[])" />
	public override async Task<OperateResult> WriteAsync(string address, byte[] value)
	{
		return await Task.FromResult(Write(address, value));
	}

	/// <inheritdoc cref="M:HslCommunication.Profinet.OrientalMotor.OrientalMotorEipNet.Read(System.String,System.UInt16)" />
	[HslMqttApi("ReadByte", "")]
	public OperateResult<byte> ReadByte(string address)
	{
		return ByteTransformHelper.GetResultFromArray(Read(address, 1));
	}

	/// <inheritdoc cref="M:HslCommunication.Profinet.OrientalMotor.OrientalMotorEipNet.Write(System.String,System.Byte[])" />
	[HslMqttApi("WriteByte", "")]
	public OperateResult Write(string address, byte value)
	{
		return Write(address, new byte[1] { value });
	}

	private OperateResult StartUdpServer()
	{
		if (socketCore != null)
		{
			NetSupport.CloseSocket(socketCore);
		}
		try
		{
			socketCore = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socketCore.Bind(new IPEndPoint(IPAddress.Any, CipIoPort));
			IsStarted = true;
			threadReceive = new Thread(ThreadReceiveUdpCycle)
			{
				IsBackground = true
			};
			threadReceive.Start();
			return OperateResult.CreateSuccessResult();
		}
		catch (Exception ex)
		{
			return new OperateResult("Start Udp failed: " + ex.Message);
		}
	}

	private void ThreadReceiveUdpCycle()
	{
		byte[] array = new byte[bufferLength];
		while (IsStarted)
		{
			int num = 0;
			try
			{
				EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
				num = socketCore.ReceiveFrom(array, 0, array.Length, SocketFlags.None, ref remoteEP);
				udpEndPoint = remoteEP;
				if (num < 18)
				{
					continue;
				}
				LogRevcMessage(array.SelectBegin(num));
				int num2 = BitConverter.ToUInt16(array, 0);
				int num3 = 2;
				for (int i = 0; i < num2; i++)
				{
					ushort num4 = BitConverter.ToUInt16(array, num3);
					num3 += 2;
					ushort num5 = BitConverter.ToUInt16(array, num3);
					num3 += 2;
					if (num4 == 177 && num5 == 174)
					{
						bufferReceiveTime = DateTime.Now;
						if (inputBuffer == null)
						{
							inputBuffer = new byte[172];
						}
						Array.Copy(array, num3 + 2, inputBuffer, 0, inputBuffer.Length);
						if (OnReceiveData != null)
						{
							OrientalMotorEventArgs e = new OrientalMotorEventArgs();
							e.Content = array.SelectMiddle(num3 + 2, inputBuffer.Length);
							OnReceiveData?.Invoke(this, e);
						}
						byte[] array2 = "0200028008000200db1d01000000b100b20001000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000".ToHexBytes();
						BitConverter.GetBytes(base.OTConnectionId).CopyTo(array2, 6);
						Array.Copy(array, 10, array2, 10, 4);
						BitConverter.GetBytes((short)returnId).CopyTo(array2, 18);
						BitConverter.GetBytes(RunIdleHeader).CopyTo(array2, 20);
						Array.Copy(outputBuffer, 0, array2, 24, outputBuffer.Length);
						returnId++;
						if (returnId > 65535)
						{
							returnId = 1;
						}
						LogSendMessage(array2);
						try
						{
							socketCore.SendTo(array2, 0, array2.Length, SocketFlags.None, udpEndPoint);
						}
						catch (Exception ex)
						{
							base.LogNet?.WriteException("UDP send back failed: ", ex);
						}
					}
					num3 += num5;
				}
			}
			catch (Exception)
			{
				break;
			}
		}
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"OrientalMotorEipNet[{IpAddress}:{Port}]";
	}
}
