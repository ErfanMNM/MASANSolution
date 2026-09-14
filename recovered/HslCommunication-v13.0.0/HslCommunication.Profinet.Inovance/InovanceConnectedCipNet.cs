using System;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Profinet.Omron;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Inovance;

/// <summary>
/// 汇川的CIP协议，基于连接的协议
/// </summary>
public class InovanceConnectedCipNet : OmronConnectedCipNet
{
	/// <summary>
	/// 实例化一个默认的对象
	/// </summary>
	public InovanceConnectedCipNet()
	{
		base.WordLength = 2;
		base.ByteTransform = new RegularByteTransform();
	}

	/// <summary>
	/// 根据指定的IP及端口来实例化这个连接对象
	/// </summary>
	/// <param name="ipAddress">PLC的Ip地址</param>
	/// <param name="port">PLC的端口号信息</param>
	public InovanceConnectedCipNet(string ipAddress, int port = 44818)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	/// <inheritdoc />
	protected override byte[] GetLargeForwardOpen(ushort connectionID)
	{
		byte[] array = "\r\n00 00 00 00 00 00 02 00 00 00 00 00 b2 00 34 00\r\n5b 02 20 06 24 01 0e 9c 02 00 00 80 01 00 fe 80\r\n02 00 1b 05 30 a7 2b 03 02 00 00 00 80 84 1e 00\r\ncc 07 00 42 80 84 1e 00 cc 07 00 42 a3 03 20 02\r\n24 01 2c 01".ToHexBytes();
		BitConverter.GetBytes(base.OTConnectionId).CopyTo(array, 24);
		BitConverter.GetBytes(base.TOConnectionId).CopyTo(array, 28);
		BitConverter.GetBytes((ushort)(2 + connectionID)).CopyTo(array, 32);
		BitConverter.GetBytes((ushort)4105).CopyTo(array, 34);
		HslHelper.HslRandom.GetBytes(4).CopyTo(array, 36);
		array[40] = base.ConnectionTimeoutMultiplier;
		return array;
	}

	/// <inheritdoc />
	[HslMqttApi("WriteBool", "")]
	public override OperateResult Write(string address, bool value)
	{
		return WriteTag(address, 193, (!value) ? new byte[1] : new byte[1] { 1 });
	}

	/// <inheritdoc />
	public override async Task<OperateResult> WriteAsync(string address, bool value)
	{
		return await WriteTagAsync(address, 193, (!value) ? new byte[1] : new byte[1] { 1 }).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"InovanceConnectedCipNet<{CommunicationPipe}>";
	}
}
