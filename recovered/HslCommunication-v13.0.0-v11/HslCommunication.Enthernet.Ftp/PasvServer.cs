using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using HslCommunication.Core;

namespace HslCommunication.Enthernet.Ftp;

internal class PasvServer
{
	/// <summary>
	/// 缓存数据
	/// </summary>
	public byte[] buffer;

	private Socket socket;

	private Socket client;

	private int port;

	/// <summary>
	/// 客户端套接字
	/// </summary>
	public Socket ClientSocket => client;

	/// <summary>
	/// 存储的文件流
	/// </summary>
	public FileStream SaveFileStream { get; set; }

	/// <summary>
	/// 操作是否完成
	/// </summary>
	public bool IsCompleted { get; set; } = false;

	public int Start(int portMin, int portMax)
	{
		for (int i = 0; i < 20; i++)
		{
			try
			{
				port = HslHelper.HslRandom.Next(portMin, portMax);
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Bind(new IPEndPoint(IPAddress.Any, port));
				socket.Listen(10);
				socket.BeginAccept(AsyncAcceptCallback, socket);
			}
			catch (Exception)
			{
				port = -1;
				continue;
			}
			break;
		}
		return port;
	}

	public void Close()
	{
		NetSupport.CloseSocket(socket);
		NetSupport.CloseSocket(client);
	}

	private void AsyncAcceptCallback(IAsyncResult iar)
	{
		if (iar.AsyncState is Socket socket)
		{
			try
			{
				client = socket.EndAccept(iar);
			}
			catch (ObjectDisposedException)
			{
				return;
			}
			catch (SocketException)
			{
			}
			catch (Exception)
			{
			}
			NetSupport.CloseSocket(socket);
		}
	}
}
