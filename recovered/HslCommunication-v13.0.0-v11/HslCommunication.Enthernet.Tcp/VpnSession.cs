using System;
using System.Net.Sockets;
using HslCommunication.Core;

namespace HslCommunication.Enthernet.Tcp;

internal class VpnSession
{
	public byte[] tcpBuffer = new byte[2048];

	public string Guid { get; set; }

	public Socket Socket { get; set; }

	/// <summary>
	/// 正在连接设备
	/// </summary>
	public bool ConnectingDevice { get; set; } = true;

	public VpnSession()
	{
		Guid = System.Guid.NewGuid().ToString("N");
	}

	public void Close()
	{
		NetSupport.CloseSocket(Socket);
		Socket = null;
	}
}
