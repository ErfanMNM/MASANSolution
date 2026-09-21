using HslCommunication.Core.IMessage;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 基于Qna 兼容4C帧协议的通讯，支持格式1,2,3,4,5，支持设置开启和校验，具体的地址需要参照三菱的基本地址<br />
/// Communication based on the QnA‑compatible 4C‑frame protocol supports Formats 1, 2, 3, 4, and 5. 
/// Checksum verification can be enabled or disabled. Refer to Mitsubishi’s base address specifications for specific addresses.
/// </summary>
public class MelsecA4CNet : MelsecA3CNet
{
	/// <inheritdoc />
	public override int McMode => 1;

	/// <inheritdoc />
	protected override INetMessage GetNewNetMessage()
	{
		return new MelsecA4CMessage
		{
			Format = base.Format,
			SumCheck = base.SumCheck
		};
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"MelsecA4CNet[{CommunicationPipe}]";
	}
}
