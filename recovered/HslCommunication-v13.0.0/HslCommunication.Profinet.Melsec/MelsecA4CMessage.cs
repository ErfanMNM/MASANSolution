using System;
using System.IO;
using System.Text;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// Melsec A3C协议的消息类，主要用于数据接收的判断，具体的格式请参考三菱的A3C协议文档<br />
/// </summary>
public class MelsecA4CMessage : NetMessageBase, INetMessage
{
	/// <inheritdoc cref="P:HslCommunication.Profinet.Melsec.Helper.IReadWriteA3C.Format" />
	public int Format { get; set; } = 1;

	/// <inheritdoc cref="P:HslCommunication.Profinet.Melsec.Helper.IReadWriteA3C.SumCheck" />
	public bool SumCheck { get; set; } = true;

	/// <inheritdoc cref="P:HslCommunication.Core.IMessage.INetMessage.ProtocolHeadBytesLength" />
	public int ProtocolHeadBytesLength => -1;

	/// <inheritdoc cref="M:HslCommunication.Core.IMessage.INetMessage.GetContentLengthByHeadBytes" />
	public int GetContentLengthByHeadBytes()
	{
		return 0;
	}

	/// <inheritdoc cref="M:HslCommunication.Core.IMessage.INetMessage.CheckReceiveDataComplete(System.Byte[],System.IO.MemoryStream)" />
	public override bool CheckReceiveDataComplete(byte[] send, MemoryStream ms)
	{
		byte[] array = ms.ToArray();
		if (array.Length < 5)
		{
			return false;
		}
		if (Format == 1)
		{
			if (array[0] == 2)
			{
				if (SumCheck)
				{
					return array[array.Length - 3] == 3;
				}
				return array[array.Length - 1] == 3;
			}
			if (array[0] == 21)
			{
				return array.Length >= 21;
			}
			if (array[0] == 6)
			{
				return array.Length >= 17;
			}
			return false;
		}
		if (Format == 2)
		{
			if (array[0] == 2)
			{
				if (SumCheck)
				{
					return array[array.Length - 3] == 3;
				}
				return array[array.Length - 1] == 3;
			}
			if (array[0] == 21)
			{
				return array.Length >= 23;
			}
			if (array[0] == 6)
			{
				return array.Length >= 19;
			}
			return false;
		}
		if (Format == 3)
		{
			if (array[0] != 2)
			{
				return false;
			}
			if (array.Length < 21)
			{
				return false;
			}
			string text = Encoding.ASCII.GetString(array, 17, 4);
			if (text == "QNAK")
			{
				return array.Length >= 26;
			}
			if (text == "QACK")
			{
				if (!SumCheck)
				{
					return array[array.Length - 1] == 3;
				}
				if (array.Length == 22 && array[21] == 3)
				{
					return true;
				}
				if (array[array.Length - 3] == 3)
				{
					return true;
				}
			}
			return false;
		}
		if (Format == 4)
		{
			if (array[0] == 2)
			{
				return array[array.Length - 2] == 13 && array[array.Length - 1] == 10;
			}
			if (array[0] == 21)
			{
				return array.Length >= 23;
			}
			if (array[0] == 6)
			{
				return array.Length >= 19;
			}
			return false;
		}
		if (Format == 5)
		{
			if (array[0] != 16)
			{
				return false;
			}
			if (array[1] != 2)
			{
				return false;
			}
			int num = BitConverter.ToUInt16(array, 2);
			if (SumCheck)
			{
				return array.Length >= num + 8;
			}
			return array.Length >= num + 6;
		}
		return true;
	}
}
