using System;

namespace HslCommunication.Enthernet.Tcp;

/// <summary>
/// 会话额外的属性
/// </summary>
public class SessionExtraInfo
{
	internal VpnPortServer Server { get; set; }

	/// <summary>
	/// 服务的启动时间
	/// </summary>
	public DateTime StartTime { get; set; } = DateTime.Now;

	/// <summary>
	/// 服务的运行时间，单位秒，默认15分钟，超过这个时间后，服务将自动关闭
	/// </summary>
	public int RunTime { get; set; } = 900;

	/// <summary>
	/// 是否可以自定义映射的端口号
	/// </summary>
	public bool CanDefinePort { get; set; } = false;
}
