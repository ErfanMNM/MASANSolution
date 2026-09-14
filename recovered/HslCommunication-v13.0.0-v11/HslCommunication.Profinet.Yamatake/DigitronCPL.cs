using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Profinet.Yamatake.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Yamatake;

/// <summary>
/// 日本山武的数字指示调节器，目前适配SDC40B
/// </summary>
public class DigitronCPL : DeviceSerialPort
{
	/// <summary>
	/// 获取或设置当前的站号信息
	/// </summary>
	public byte Station { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public DigitronCPL()
	{
		Station = 1;
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform();
		LogMsgFormatBinary = false;
		base.ReceiveEmptyDataCount = 5;
	}

	/// <inheritdoc />
	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(13, 10);
	}

	/// <inheritdoc />
	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return DigitronCPLHelper.ExtraActualResponse(response);
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, byte[] value)
	{
		return DigitronCPLHelper.BuildWriteCommand(Station, address, value);
	}

	/// <inheritdoc />
	[HslMqttApi("ReadByteArray", "")]
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		OperateResult<byte[]> operateResult = DigitronCPLHelper.BuildReadCommand(Station, address, length);
		if (!operateResult.IsSuccess)
		{
			return operateResult;
		}
		return ReadFromCoreServer(operateResult.Content);
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"DigitronCPL[{base.PortName}:{base.BaudRate}]";
	}
}
