using HslCommunication.Core.IMessage;

namespace HslCommunication.Instrument.Temperature;

/// <summary>
/// Represents a message structure used in the YuDian communication protocol.
/// </summary>
public class YuDianMessage : NetMessageBase, INetMessage
{
	/// <inheritdoc cref="P:HslCommunication.Core.IMessage.INetMessage.ProtocolHeadBytesLength" />
	public int ProtocolHeadBytesLength => 10;

	/// <inheritdoc cref="M:HslCommunication.Core.IMessage.INetMessage.GetContentLengthByHeadBytes" />
	public int GetContentLengthByHeadBytes()
	{
		return 0;
	}
}
