using System;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Profinet.Inovance;

/// <summary>
/// 汇川的Easy系列的报文类，使用专用的报文头，地址说明参考Demo界面
/// </summary>
public class InovanceEasyNetMessage : NetMessageBase, INetMessage
{
	/// <inheritdoc cref="P:HslCommunication.Core.IMessage.INetMessage.ProtocolHeadBytesLength" />
	public int ProtocolHeadBytesLength => 2;

	/// <inheritdoc cref="M:HslCommunication.Core.IMessage.INetMessage.GetContentLengthByHeadBytes" />
	public int GetContentLengthByHeadBytes()
	{
		int num = BitConverter.ToUInt16(base.HeadBytes, 0);
		return num - 2;
	}
}
