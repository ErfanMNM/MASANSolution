using System;
using System.IO;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Core.Device;

internal class DevicePluginMessage : NetMessageBase, INetMessage
{
	public Func<int> ProtocolHeadBytesLengthFunc = () => -1;

	public Func<byte[], byte[], int> GetContentLengthByHeadBytesFunc = null;

	public Func<byte[], byte[], bool> CheckReceiveDataCompleteFunc = null;

	/// <inheritdoc cref="P:HslCommunication.Core.IMessage.INetMessage.ProtocolHeadBytesLength" />
	public int ProtocolHeadBytesLength => ProtocolHeadBytesLengthFunc();

	/// <inheritdoc cref="M:HslCommunication.Core.IMessage.INetMessage.GetContentLengthByHeadBytes" />
	public int GetContentLengthByHeadBytes()
	{
		if (GetContentLengthByHeadBytesFunc != null)
		{
			return GetContentLengthByHeadBytesFunc(base.SendBytes, base.HeadBytes);
		}
		return 0;
	}

	/// <inheritdoc cref="M:HslCommunication.Core.IMessage.INetMessage.CheckReceiveDataComplete(System.Byte[],System.IO.MemoryStream)" />
	public override bool CheckReceiveDataComplete(byte[] send, MemoryStream ms)
	{
		byte[] arg = ms.ToArray();
		if (CheckReceiveDataCompleteFunc != null)
		{
			return CheckReceiveDataCompleteFunc(send, arg);
		}
		return true;
	}
}
