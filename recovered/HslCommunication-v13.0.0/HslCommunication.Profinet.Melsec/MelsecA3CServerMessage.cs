using System;
using System.IO;
using System.Text;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Profinet.Melsec;

internal class MelsecA3CServerMessage : NetMessageBase, INetMessage
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

	protected bool CalculateMcLengthComplete(byte[] receive, int startIndex, int tail = 0)
	{
		return CheckServerMcCommandComplete(receive, receive.Length, startIndex, SumCheck, tail);
	}

	/// <inheritdoc cref="M:HslCommunication.Core.IMessage.INetMessage.CheckReceiveDataComplete(System.Byte[],System.IO.MemoryStream)" />
	public override bool CheckReceiveDataComplete(byte[] send, MemoryStream ms)
	{
		byte[] array = ms.ToArray();
		return CheckServer3CCommandComplete(array, Format, array.Length, SumCheck);
	}

	public static bool CheckServer3CCommandComplete(byte[] receive, int format, int actualLength, bool sumCheck)
	{
		if (receive.Length < 5)
		{
			return false;
		}
		switch (format)
		{
		case 1:
			if (receive[0] == 5)
			{
				return CheckServerMcCommandComplete(receive, actualLength, 11, sumCheck);
			}
			break;
		case 2:
			if (receive[0] == 5)
			{
				return CheckServerMcCommandComplete(receive, actualLength, 13, sumCheck);
			}
			break;
		case 3:
			if (receive[0] == 2)
			{
				return CheckServerMcCommandComplete(receive, actualLength, 11, sumCheck, 1);
			}
			break;
		case 4:
			if (receive[0] == 5)
			{
				return CheckServerMcCommandComplete(receive, actualLength, 11, sumCheck, 2);
			}
			break;
		}
		return true;
	}

	public static bool CheckServerMcCommandComplete(byte[] receive, int actualLength, int startIndex, bool sumCheck, int tail = 0)
	{
		int num = -1;
		if (actualLength < startIndex + 4)
		{
			return false;
		}
		if (Encoding.ASCII.GetString(receive, startIndex, 4) == "0401")
		{
			num = 20;
		}
		else if (Encoding.ASCII.GetString(receive, startIndex, 4) == "1401")
		{
			if (actualLength < startIndex + 20)
			{
				return false;
			}
			string text = Encoding.ASCII.GetString(receive, startIndex + 4, 4);
			if (text == "0001")
			{
				num = 20 + Convert.ToInt32(Encoding.ASCII.GetString(receive, startIndex + 16, 4), 16);
			}
			else if (text == "0000")
			{
				num = 20 + Convert.ToInt32(Encoding.ASCII.GetString(receive, startIndex + 16, 4), 16) * 4;
			}
		}
		else if (Encoding.ASCII.GetString(receive, startIndex, 4) == "0403")
		{
			if (actualLength < startIndex + 12)
			{
				return false;
			}
			num = 12 + (Convert.ToInt32(Encoding.ASCII.GetString(receive, startIndex + 8, 2), 16) + Convert.ToInt32(Encoding.ASCII.GetString(receive, startIndex + 10, 2), 16)) * 8;
		}
		else
		{
			if (!(Encoding.ASCII.GetString(receive, startIndex, 4) == "1402"))
			{
				return true;
			}
			if (actualLength < startIndex + 12)
			{
				return false;
			}
			string text2 = Encoding.ASCII.GetString(receive, startIndex + 4, 4);
			if (text2 == "0001")
			{
				num = 10 + Convert.ToInt32(Encoding.ASCII.GetString(receive, startIndex + 8, 2), 16) * 10;
			}
			if (text2 == "0000")
			{
				num = 12 + Convert.ToInt32(Encoding.ASCII.GetString(receive, startIndex + 8, 2), 16) * 12 + Convert.ToInt32(Encoding.ASCII.GetString(receive, startIndex + 10, 2), 16) * 16;
			}
		}
		if (num < 0)
		{
			return false;
		}
		return actualLength >= startIndex + num + tail + (sumCheck ? 2 : 0);
	}
}
