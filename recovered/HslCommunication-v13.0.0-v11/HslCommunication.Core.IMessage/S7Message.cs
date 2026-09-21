namespace HslCommunication.Core.IMessage;

/// <summary>
/// 西门子S7协议的消息解析规则
/// </summary>
public class S7Message : NetMessageBase, INetMessage
{
	/// <inheritdoc cref="P:HslCommunication.Core.IMessage.INetMessage.ProtocolHeadBytesLength" />
	public int ProtocolHeadBytesLength => 4;

	/// <summary>
	/// 是否检查消息的合法性，默认为true，如果设置为false，则不检查消息的合法性，直接返回接收的数据内容，适用于一些特殊的协议解析需求
	/// </summary>
	public bool CheckMessage { get; set; } = true;

	/// <inheritdoc cref="M:HslCommunication.Core.IMessage.INetMessage.CheckHeadBytesLegal(System.Byte[])" />
	public override bool CheckHeadBytesLegal(byte[] token)
	{
		if (base.HeadBytes == null)
		{
			return false;
		}
		if (base.HeadBytes[0] == 3 && base.HeadBytes[1] == 0)
		{
			return true;
		}
		return false;
	}

	/// <inheritdoc cref="M:HslCommunication.Core.IMessage.INetMessage.GetContentLengthByHeadBytes" />
	public int GetContentLengthByHeadBytes()
	{
		byte[] headBytes = base.HeadBytes;
		if (headBytes != null && headBytes.Length >= 4)
		{
			int num = base.HeadBytes[2] * 256 + base.HeadBytes[3] - 4;
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}
		return 0;
	}

	/// <inheritdoc />
	public override int CheckMessageMatch(byte[] send, byte[] receive)
	{
		if (!CheckMessage)
		{
			return 1;
		}
		if (receive != null && receive.Length >= 14 && send != null && send.Length >= 14 && receive[5] == 240)
		{
			if (send[11] == receive[11] && send[12] == receive[12])
			{
				return 1;
			}
			return -1;
		}
		return base.CheckMessageMatch(send, receive);
	}
}
