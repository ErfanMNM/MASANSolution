using System;
using System.IO;

namespace HslCommunication.Profinet.Melsec;

internal class MelsecA4CServerMessage : MelsecA3CServerMessage
{
	public override bool CheckReceiveDataComplete(byte[] send, MemoryStream ms)
	{
		byte[] array = ms.ToArray();
		return CheckServer4CCommandComplete(array, base.Format, array.Length, base.SumCheck);
	}

	public static bool CheckServer4CCommandComplete(byte[] receive, int format, int actualLength, bool sumCheck)
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
				return MelsecA3CServerMessage.CheckServerMcCommandComplete(receive, actualLength, 17, sumCheck);
			}
			break;
		case 2:
			if (receive[0] == 5)
			{
				return MelsecA3CServerMessage.CheckServerMcCommandComplete(receive, actualLength, 19, sumCheck);
			}
			break;
		case 3:
			if (receive[0] == 2)
			{
				return MelsecA3CServerMessage.CheckServerMcCommandComplete(receive, actualLength, 17, sumCheck, 1);
			}
			break;
		case 4:
			if (receive[0] == 5)
			{
				return MelsecA3CServerMessage.CheckServerMcCommandComplete(receive, actualLength, 17, sumCheck, 2);
			}
			break;
		case 5:
			if (actualLength < 4)
			{
				return false;
			}
			if (receive[0] == 16 && receive[1] == 2)
			{
				int num = BitConverter.ToUInt16(receive, 2);
				if (sumCheck)
				{
					return actualLength >= num + 8;
				}
				return actualLength >= num + 6;
			}
			break;
		}
		return true;
	}
}
