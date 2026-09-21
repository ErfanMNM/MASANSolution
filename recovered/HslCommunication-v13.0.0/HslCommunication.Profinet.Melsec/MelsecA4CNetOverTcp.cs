using HslCommunication.Core.IMessage;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 基于Qna 兼容4C帧协议的通讯，支持格式1,2,3,4,5，支持设置开启和校验，具体的地址需要参照三菱的基本地址，本类是基于TCP的通信<br />
/// Communication based on the QnA‑compatible 4C‑frame protocol supports Formats 1, 2, 3, 4, and 5. 
/// Checksum verification can be enabled or disabled. Refer to Mitsubishi’s base address specifications for specific addresses. This class implements TCP‑based communication.
/// </summary>
public class MelsecA4CNetOverTcp : MelsecA3CNetOverTcp
{
	/// <inheritdoc />
	public override int McMode => 1;

	/// <summary>
	/// 实例化默认的对象<br />
	/// Instantiate the default object
	/// </summary>
	public MelsecA4CNetOverTcp()
	{
	}

	/// <summary>
	/// 指定ip地址和端口号来实例化对象<br />
	/// Specify the IP address and port number to instantiate the object
	/// </summary>
	/// <param name="ipAddress">Ip地址信息</param>
	/// <param name="port">端口号信息</param>
	public MelsecA4CNetOverTcp(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

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
		return $"MelsecA4CNetOverTcp[{CommunicationPipe}]";
	}
}
