using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Profinet.Cimon.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Cimon;

/// <summary>
/// 西蒙PLC的数据读取类，支持的地址请参考Demo测试工具上的示例<br />
/// Simon PLC data reading class, the supported address please refer to the Demo test tool on the example
/// </summary>
public class CimonHmiProtocol : DeviceTcpNet
{
	private byte frameNo = 1;

	/// <summary>
	/// 获取或设置当前的站号信息<br />
	/// Gets or sets the current station number information
	/// </summary>
	public byte FrameNo
	{
		get
		{
			return frameNo;
		}
		set
		{
			frameNo = value;
		}
	}

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public CimonHmiProtocol()
	{
		base.WordLength = 1;
		IpAddress = "127.0.0.1";
		Port = 10260;
		base.ByteTransform = new ReverseWordTransform();
	}

	/// <summary>
	/// 根据IP地址，端口号来实例化一个对象
	/// </summary>
	/// <param name="ip">IP地址</param>
	/// <param name="port">端口号</param>
	public CimonHmiProtocol(string ip, int port = 10260)
		: this()
	{
		IpAddress = ip;
		Port = port;
	}

	/// <inheritdoc />
	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return CimonHelper.ExtractActualData(response);
	}

	/// <inheritdoc />
	protected override OperateResult<List<byte[]>> BuildDeviceReadCommand(string address, ushort length, bool isBit)
	{
		if (isBit)
		{
			byte b = (byte)HslHelper.ExtractParameter(ref address, "f", frameNo);
			return CimonHelper.PackReadWriteCommand(b, 114, address, length, null);
		}
		byte b2 = (byte)HslHelper.ExtractParameter(ref address, "f", frameNo);
		return CimonHelper.PackReadWriteCommand(b2, 82, address, length, null);
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, byte[] value)
	{
		byte b = (byte)HslHelper.ExtractParameter(ref address, "f", frameNo);
		if (value == null)
		{
			value = new byte[0];
		}
		return CimonHelper.PackReadWriteCommand(b, 87, address, value.Length, value);
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, bool[] value)
	{
		byte b = (byte)HslHelper.ExtractParameter(ref address, "f", frameNo);
		if (value == null)
		{
			value = new bool[0];
		}
		byte[] array = new byte[value.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (value[i])
			{
				array[i] = 1;
			}
		}
		return CimonHelper.PackReadWriteCommand(b, 119, address, value.Length, array);
	}

	/// <inheritdoc />
	protected override OperateResult<bool[]> ParseDeviceReadBoolResult(string address, byte[] buildCommand, byte[] content, ushort length)
	{
		return OperateResult.CreateSuccessResult(content.Select((byte m) => m != 0).ToArray());
	}

	/// <inheritdoc />
	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (Regex.IsMatch(address, "D[0-9]+\\.[0-9a-fA-F]"))
		{
			return HslHelper.ReadBool(this, address, length, 16, reverseByWord: true);
		}
		return base.ReadBool(address, length);
	}

	/// <inheritdoc />
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		if (Regex.IsMatch(address, "D[0-9]+\\.[0-9a-fA-F]"))
		{
			return await HslHelper.ReadBoolAsync(this, address, length, 16, reverseByWord: true).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await base.ReadBoolAsync(address, length).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"CimonHmiProtocol<{CommunicationPipe}>";
	}
}
