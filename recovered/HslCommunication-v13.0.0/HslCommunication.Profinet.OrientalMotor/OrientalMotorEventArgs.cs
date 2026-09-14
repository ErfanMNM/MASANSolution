using System;

namespace HslCommunication.Profinet.OrientalMotor;

/// <summary>
/// 东方马达的消息类信息
/// </summary>
public class OrientalMotorEventArgs : EventArgs
{
	/// <summary>
	/// 原始字节内容
	/// </summary>
	public byte[] Content { get; set; }
}
