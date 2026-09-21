using System.Collections.Generic;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Profinet.Vigor.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Vigor;

/// <summary>
/// 丰炜通信协议的网口透传版本，支持VS系列，地址支持携带站号，例如 s=2;D100, 字地址支持 D,SD,R,T,C(C200-C255是32位寄存器), 位地址支持X,Y,M,SM,S,TS(定时器触点),TC（定时器线圈）,CS(计数器触点),CC（计数器线圈)<br />
/// The network port transparent transmission version of Fengwei communication protocol supports VS series, and the address supports carrying station number, 
/// such as s=2;D100, word address supports D, SD, R, T, C (C200-C255 are 32-bit registers), Bit address supports X, Y, M, SM, S, TS (timer contact), 
/// TC (timer coil), CS (counter contact), CC (counter coil)
/// </summary>
/// <remarks>
/// 暂时不支持对字寄存器(D,R)进行读写位操作，感谢随时关注库的更新日志
/// </remarks>
public class VigorSerialOverTcp : DeviceTcpNet
{
	/// <inheritdoc cref="P:HslCommunication.Profinet.Vigor.VigorSerial.Station" />
	public byte Station { get; set; }

	/// <summary>
	/// 实例化默认的构造方法<br />
	/// Instantiate the default constructor
	/// </summary>
	public VigorSerialOverTcp()
	{
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform();
	}

	/// <summary>
	/// 使用指定的ip地址和端口来实例化一个对象<br />
	/// Instantiate an object with the specified IP address and port
	/// </summary>
	/// <param name="ipAddress">设备的Ip地址</param>
	/// <param name="port">设备的端口号</param>
	public VigorSerialOverTcp(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	/// <inheritdoc />
	protected override INetMessage GetNewNetMessage()
	{
		return new VigorSerialMessage();
	}

	/// <inheritdoc />
	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return VigorVsHelper.CheckResponseContent(response);
	}

	/// <inheritdoc />
	protected override OperateResult<List<byte[]>> BuildDeviceReadCommand(string address, ushort length, bool isBit)
	{
		return VigorVsHelper.BuildReadCommand(Station, address, length, isBit);
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, byte[] value)
	{
		return VigorVsHelper.BuildWriteWordCommand(Station, address, value);
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, bool[] value)
	{
		return VigorVsHelper.BuildWriteBoolCommand(Station, address, value);
	}

	/// <inheritdoc />
	protected override OperateResult<bool[]> ParseDeviceReadBoolResult(string address, byte[] buildCommand, byte[] content, ushort length)
	{
		return OperateResult.CreateSuccessResult(content.ToBoolArray());
	}

	/// <inheritdoc />
	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (address.Contains("."))
		{
			return HslHelper.ReadBool(this, address, length);
		}
		return base.ReadBool(address, length);
	}

	/// <inheritdoc />
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		if (address.Contains("."))
		{
			return await HslHelper.ReadBoolAsync(this, address, length);
		}
		return await base.ReadBoolAsync(address, length);
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"VigorSerialOverTcp[{IpAddress}:{Port}]";
	}
}
