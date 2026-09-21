namespace HslCommunication.Profinet.OpenProtocol;

/// <summary>
/// 用于额外的订阅的MID信息，包含当前的MID以及回复的MID信息，如果是订阅的消息，则回复的MID通常是当前MID+1<br />
/// </summary>
public class ExtraSubscribeMid
{
	/// <summary>
	/// 订阅的MID信息，通常是设备方发过来的数据的Mid<br />
	/// Gets or sets the middle identifier.
	/// </summary>
	public int Mid { get; set; }

	/// <summary>
	/// 用于订阅的消息，回复的MID通常是当前MID+1<br />
	/// Gets or sets the message ID to which this message is a reply.
	/// </summary>
	public int ReplyMid { get; set; } = -1;
}
