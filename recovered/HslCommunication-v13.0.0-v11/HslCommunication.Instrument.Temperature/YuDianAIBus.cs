using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Reflection;

namespace HslCommunication.Instrument.Temperature;

/// <summary>
/// 宇电AIBus协议的温度采集模块，基于串口通讯实现。
/// </summary>
public class YuDianAIBus : DeviceSerialPort
{
	/// <inheritdoc cref="P:HslCommunication.ModBus.ModbusRtu.Station" />
	public byte Station { get; set; } = 0;

	/// <summary>
	/// 实例化一个默认的对象<br />
	/// Initializes a new instance of the <see cref="T:HslCommunication.Instrument.Temperature.YuDianAIBus" /> class.
	/// </summary>
	/// <remarks>This constructor sets up the default configuration for the <see cref="T:HslCommunication.Instrument.Temperature.YuDianAIBus" /> instance, 
	/// including initializing the byte transformation mechanism, setting the word length, and defining  the default sleep
	/// time between operations.</remarks>
	public YuDianAIBus()
	{
		base.ByteTransform = new RegularByteTransform();
		base.WordLength = 1;
		base.SleepTime = 20;
	}

	/// <inheritdoc />
	protected override INetMessage GetNewNetMessage()
	{
		return new YuDianMessage();
	}

	private OperateResult<byte[]> BuildCommandHelper(string address, byte code, byte data1, byte data2)
	{
		byte b = (byte)HslHelper.ExtractParameter(ref address, "s", Station);
		if (!byte.TryParse(address, out var result))
		{
			return new OperateResult<byte[]>($"Address [{result}] Wrong!");
		}
		byte b2 = ((b < 128) ? ((byte)(b + 128)) : b);
		byte[] array = new byte[8] { b2, b2, code, result, data1, data2, 0, 0 };
		int num = b;
		for (int i = 2; i < array.Length - 2; i += 2)
		{
			num += BitConverter.ToUInt16(array, i);
		}
		array[6] = (byte)(num % 256);
		array[7] = (byte)(num / 256);
		return OperateResult.CreateSuccessResult(array);
	}

	private OperateResult<byte[]> BuildReadCommand(string address, ushort length, bool isBit)
	{
		return BuildCommandHelper(address, 82, 0, 0);
	}

	private OperateResult<byte[]> BuildWriteCommand(string address, byte[] value)
	{
		if (value.Length != 2)
		{
			return new OperateResult<byte[]>("Value Length Must Be 2 Bytes");
		}
		return BuildCommandHelper(address, 67, value[0], value[1]);
	}

	/// <inheritdoc cref="M:HslCommunication.Core.IReadWriteNet.Read(System.String,System.UInt16)" />
	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<byte[]> operateResult = BuildReadCommand(address, length, isBit: false);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		OperateResult<byte[]> operateResult2 = ReadFromCoreServer(operateResult.Content);
		if (!operateResult2.IsSuccess)
		{
			return operateResult2;
		}
		if (length == 1)
		{
			return OperateResult.CreateSuccessResult(operateResult2.Content.SelectMiddle(6, 2));
		}
		return OperateResult.CreateSuccessResult(operateResult2.Content.SelectMiddle(0, 8));
	}

	/// <inheritdoc cref="M:HslCommunication.Core.IReadWriteNet.ReadDouble(System.String,System.UInt16)" />
	[HslMqttApi("ReadDoubleArray", "")]
	public override OperateResult<double[]> ReadDouble(string address, ushort length)
	{
		OperateResult<short[]> operateResult = ReadInt16(address, length);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<double[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(((IEnumerable<short>)operateResult.Content).Select((Func<short, double>)((short m) => m)).ToArray());
	}

	/// <inheritdoc cref="M:HslCommunication.Core.IReadWriteNet.Write(System.String,System.Byte[])" />
	[HslMqttApi("WriteByteArray", "")]
	public override OperateResult Write(string address, byte[] value)
	{
		OperateResult<byte[]> operateResult = BuildWriteCommand(address, value);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return ReadFromCoreServer(operateResult.Content);
	}

	/// <inheritdoc cref="M:HslCommunication.Core.IReadWriteNet.Write(System.String,System.Double[])" />
	[HslMqttApi("WriteDoubleArray", "")]
	public override OperateResult Write(string address, double[] values)
	{
		return Write(address, values.Select((double m) => (short)m).ToArray());
	}

	/// <inheritdoc cref="M:HslCommunication.Core.IReadWriteNet.ReadDoubleAsync(System.String,System.UInt16)" />
	public override async Task<OperateResult<double[]>> ReadDoubleAsync(string address, ushort length)
	{
		OperateResult<short[]> read = await ReadInt16Async(address, length).ConfigureAwait(continueOnCapturedContext: false);
		if (!read.IsSuccess)
		{
			return OperateResult.CreateFailedResult<double[]>(read);
		}
		return OperateResult.CreateSuccessResult(((IEnumerable<short>)read.Content).Select((Func<short, double>)((short m) => m)).ToArray());
	}

	/// <inheritdoc cref="M:HslCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Double[])" />
	public override async Task<OperateResult> WriteAsync(string address, double[] values)
	{
		return await WriteAsync(address, values.Select((double m) => (short)m).ToArray()).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"YuDianAIBus<{CommunicationPipe}>";
	}
}
