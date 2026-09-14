using System;
using System.Text;

namespace HslCommunication.Profinet.OpenProtocol;

/// <summary>
/// Open协议的消息对象信息
/// </summary>
public class OpenEventArgs : EventArgs
{
	/// <summary>
	/// Open协议的消息内容
	/// </summary>
	public string Content { get; set; }

	/// <summary>
	/// 接收到的原数据信息
	/// </summary>
	public byte[] RawData { get; set; }

	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public OpenEventArgs()
	{
	}

	/// <summary>
	/// 指定Open的消息来实例化对象
	/// </summary>
	/// <param name="content">Open内容</param>
	public OpenEventArgs(byte[] content)
	{
		RawData = content;
		Content = Encoding.ASCII.GetString(content).TrimEnd(default(char));
	}

	/// <summary>
	/// 指定Open的消息来实例化对象
	/// </summary>
	/// <param name="content">Open内容</param>
	public OpenEventArgs(string content)
	{
		Content = content;
	}
}
