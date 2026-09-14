using HslCommunication.Profinet.Melsec;

namespace HslCommunication.Profinet.Inovance;

/// <summary>
/// 汇川的计算机链接协议，使用PLC得COM1口，设置 D8126=06h, D8120.14=1 才能正常通信。<br />
/// Inovance's computer connection protocol, when using the COM1 port of the PLC, requires setting D8126=06h and D8120.14=1 for normal communication.
/// </summary>
public class InovanceComputerLink : MelsecFxLinks
{
	/// <summary>
	/// 实例化一个欧姆龙计算机链接协议的对象<br />
	/// Instantiate an object of the Omron Computer Link Protocol
	/// </summary>
	public InovanceComputerLink()
	{
		base.Format = 4;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"InovanceComputerLink<{CommunicationPipe}>";
	}
}
