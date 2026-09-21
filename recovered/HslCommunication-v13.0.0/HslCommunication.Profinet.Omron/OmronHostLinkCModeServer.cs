using System;
using System.Linq;
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;

namespace HslCommunication.Profinet.Omron;

/// <summary>
/// 欧姆龙的HostLinkCMode协议的虚拟服务器
/// </summary>
public class OmronHostLinkCModeServer : OmronFinsServer
{
	private byte operationMode = 1;

	private SoftBuffer timerCounterBuffer = null;

	/// <inheritdoc cref="P:HslCommunication.Profinet.Omron.OmronHostLink.UnitNumber" />
	public byte UnitNumber { get; set; }

	/// <inheritdoc cref="M:HslCommunication.Profinet.Omron.OmronFinsServer.#ctor" />
	public OmronHostLinkCModeServer()
	{
		LogMsgFormatBinary = false;
		connectionInitialization = false;
		base.SerialReceiveAtleastTime = 2000;
		timerCounterBuffer = new SoftBuffer(131072);
	}

	/// <inheritdoc />
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		try
		{
			if (address.StartsWith("CNT"))
			{
				return OperateResult.CreateSuccessResult((from m in timerCounterBuffer.GetBytes(Convert.ToInt32(address.Substring(3)) + 10000 + 2048, length)
					select m != 0).ToArray());
			}
			if (address.StartsWith("TIM"))
			{
				return OperateResult.CreateSuccessResult((from m in timerCounterBuffer.GetBytes(Convert.ToInt32(address.Substring(3)) + 10000, length)
					select m != 0).ToArray());
			}
		}
		catch (Exception ex)
		{
			return new OperateResult<bool[]>("Address Error: " + ex.Message);
		}
		return base.ReadBool(address, length);
	}

	/// <inheritdoc />
	public override OperateResult Write(string address, bool[] value)
	{
		try
		{
			if (address.StartsWith("CNT"))
			{
				timerCounterBuffer.SetBytes(value.Select((bool m) => (byte)(m ? 1 : 0)).ToArray(), Convert.ToInt32(address.Substring(3)) + 10000 + 2048);
				return OperateResult.CreateSuccessResult();
			}
			if (address.StartsWith("TIM"))
			{
				timerCounterBuffer.SetBytes(value.Select((bool m) => (byte)(m ? 1 : 0)).ToArray(), Convert.ToInt32(address.Substring(3)) + 10000);
				return OperateResult.CreateSuccessResult();
			}
		}
		catch (Exception ex)
		{
			return new OperateResult<bool[]>("Address Error: " + ex.Message);
		}
		return base.Write(address, value);
	}

	/// <inheritdoc />
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		try
		{
			if (address.StartsWith("CNT"))
			{
				return OperateResult.CreateSuccessResult(timerCounterBuffer.GetBytes((Convert.ToInt32(address.Substring(3)) + 2048) * 2, length * 2));
			}
			if (address.StartsWith("TIM"))
			{
				return OperateResult.CreateSuccessResult(timerCounterBuffer.GetBytes(Convert.ToInt32(address.Substring(3)) * 2, length * 2));
			}
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("Address Error: " + ex.Message);
		}
		return base.Read(address, length);
	}

	/// <inheritdoc />
	public override OperateResult Write(string address, byte[] value)
	{
		try
		{
			if (address.StartsWith("CNT"))
			{
				timerCounterBuffer.SetBytes(value, (Convert.ToInt32(address.Substring(3)) + 2048) * 2);
				return OperateResult.CreateSuccessResult();
			}
			if (address.StartsWith("TIM"))
			{
				timerCounterBuffer.SetBytes(value, Convert.ToInt32(address.Substring(3)) * 2);
				return OperateResult.CreateSuccessResult();
			}
		}
		catch (Exception ex)
		{
			return new OperateResult<byte[]>("Address Error: " + ex.Message);
		}
		return base.Write(address, value);
	}

	/// <inheritdoc />
	protected override INetMessage GetNewNetMessage()
	{
		return new SpecifiedCharacterMessage(13);
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> ReadFromCoreServer(PipeSession session, byte[] receive)
	{
		if (receive.Length < 9)
		{
			return new OperateResult<byte[]>("Uknown Data：" + SoftBasic.GetAsciiStringRender(receive));
		}
		return OperateResult.CreateSuccessResult(ReadFromFinsCore(receive));
	}

	/// <inheritdoc />
	protected override byte[] ReadFromFinsCore(byte[] finsCore)
	{
		string text = Encoding.ASCII.GetString(finsCore, 3, 2);
		switch (text)
		{
		default:
			if (!(text == "RG"))
			{
				switch (text)
				{
				default:
					if (!(text == "WC"))
					{
						switch (text)
						{
						case "MM":
							return PackCommand(0, finsCore, new byte[1] { 48 });
						case "MS":
							return PackCommand(0, finsCore, new byte[2] { operationMode, 48 });
						case "SC":
						{
							byte b = Convert.ToByte(Encoding.ASCII.GetString(finsCore, 5, 2), 16);
							if (b >= 0 && b <= 2)
							{
								operationMode = b;
							}
							return PackCommand(0, finsCore, null);
						}
						default:
							return PackCommand(22, finsCore, null);
						}
					}
					goto case "WD";
				case "WD":
				case "WR":
				case "WL":
				case "WH":
				case "WJ":
				case "WE":
				{
					SoftBuffer softBuffer = null;
					switch (text)
					{
					case "WD":
						softBuffer = dBuffer;
						break;
					case "WR":
						softBuffer = cioBuffer;
						break;
					case "WL":
						softBuffer = cioBuffer;
						break;
					case "WH":
						softBuffer = hBuffer;
						break;
					case "WJ":
						softBuffer = arBuffer;
						break;
					case "WE":
						softBuffer = emBuffer;
						break;
					case "WC":
						softBuffer = timerCounterBuffer;
						break;
					default:
						return PackCommand(22, finsCore, null);
					}
					if (text == "WE")
					{
						int num = Convert.ToInt32(Encoding.ASCII.GetString(finsCore, 7, 4));
						byte[] data = Encoding.ASCII.GetString(finsCore, 11, finsCore.Length - 15).ToHexBytes();
						softBuffer.SetBytes(data, num * 2);
						return PackCommand(0, finsCore, null);
					}
					if (text == "WC")
					{
						try
						{
							int num2 = Convert.ToInt32(Encoding.ASCII.GetString(finsCore, 5, 4));
							string text2 = Encoding.ASCII.GetString(finsCore, 9, finsCore.Length - 13);
							byte[] array = new byte[text2.Length / 2];
							for (int i = 0; i < array.Length / 2; i++)
							{
								ushort num3 = Convert.ToUInt16(text2.Substring(i * 4, 4));
								array[i * 2] = (byte)(num3 / 256);
								array[i * 2 + 1] = (byte)(num3 % 256);
							}
							softBuffer.SetBytes(array, num2 * 2);
							return PackCommand(0, finsCore, null);
						}
						catch (Exception ex)
						{
							base.LogNet?.WriteException("OmronHostLinkCModeServer WC command error", ex);
							return PackCommand(22, finsCore, null);
						}
					}
					int num4 = Convert.ToInt32(Encoding.ASCII.GetString(finsCore, 5, 4));
					byte[] data2 = Encoding.ASCII.GetString(finsCore, 9, finsCore.Length - 13).ToHexBytes();
					softBuffer.SetBytes(data2, num4 * 2);
					return PackCommand(0, finsCore, null);
				}
				}
			}
			goto case "RD";
		case "RD":
		case "RR":
		case "RL":
		case "RH":
		case "RJ":
		case "RE":
		case "RC":
		{
			SoftBuffer softBuffer2 = null;
			switch (text)
			{
			case "RD":
				softBuffer2 = dBuffer;
				break;
			case "RR":
				softBuffer2 = cioBuffer;
				break;
			case "RL":
				softBuffer2 = cioBuffer;
				break;
			case "RH":
				softBuffer2 = hBuffer;
				break;
			case "RJ":
				softBuffer2 = arBuffer;
				break;
			case "RE":
				softBuffer2 = emBuffer;
				break;
			case "RC":
				softBuffer2 = timerCounterBuffer;
				break;
			case "RG":
				softBuffer2 = timerCounterBuffer;
				break;
			default:
				return PackCommand(22, finsCore, null);
			}
			if (text == "RE")
			{
				int num5 = Convert.ToInt32(Encoding.ASCII.GetString(finsCore, 7, 4));
				ushort num6 = Convert.ToUInt16(Encoding.ASCII.GetString(finsCore, 11, 4));
				byte[] bytes = softBuffer2.GetBytes(num5 * 2, num6 * 2);
				return PackCommand(0, finsCore, bytes);
			}
			if (text == "RG")
			{
				int num7 = Convert.ToInt32(Encoding.ASCII.GetString(finsCore, 5, 4));
				ushort length = Convert.ToUInt16(Encoding.ASCII.GetString(finsCore, 9, 4));
				byte[] bytes2 = softBuffer2.GetBytes(num7 + 10000, length);
				return PackCommand(0, finsCore, bytes2);
			}
			int num8 = Convert.ToInt32(Encoding.ASCII.GetString(finsCore, 5, 4));
			ushort num9 = Convert.ToUInt16(Encoding.ASCII.GetString(finsCore, 9, 4));
			byte[] bytes3 = softBuffer2.GetBytes(num8 * 2, num9 * 2);
			return PackCommand(0, finsCore, bytes3);
		}
		}
	}

	private byte[] GetReturnBackData(byte[] data)
	{
		byte[] array = new byte[data.Length * 2];
		for (int i = 0; i < data.Length / 2; i++)
		{
			ushort num = base.ByteTransform.TransUInt16(data, i * 2);
			if (num > 9999)
			{
				num = 9999;
			}
			Encoding.ASCII.GetBytes(num.ToString("D4")).CopyTo(array, i * 4);
		}
		return array;
	}

	/// <inheritdoc />
	protected override byte[] PackCommand(int status, byte[] finsCore, byte[] data)
	{
		string text = Encoding.ASCII.GetString(finsCore, 3, 2);
		if (data == null)
		{
			data = new byte[0];
		}
		if (text == "RC")
		{
			data = GetReturnBackData(data);
		}
		else if (text == "RG")
		{
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i] == 0)
				{
					data[i] = 48;
				}
				else if (data[i] == 1)
				{
					data[i] = 49;
				}
			}
		}
		else
		{
			data = SoftBasic.BytesToAsciiBytes(data);
		}
		byte[] array = new byte[11 + data.Length];
		Encoding.ASCII.GetBytes("@0000").CopyTo(array, 0);
		Encoding.ASCII.GetBytes(UnitNumber.ToString("X2")).CopyTo(array, 1);
		Array.Copy(finsCore, 3, array, 3, 2);
		Encoding.ASCII.GetBytes(status.ToString("X2")).CopyTo(array, 5);
		if (data.Length != 0)
		{
			data.CopyTo(array, 7);
		}
		int num = array[0];
		for (int j = 1; j < array.Length - 4; j++)
		{
			num ^= array[j];
		}
		SoftBasic.BuildAsciiBytesFrom((byte)num).CopyTo(array, array.Length - 4);
		array[array.Length - 2] = 42;
		array[array.Length - 1] = 13;
		return array;
	}

	/// <inheritdoc />
	protected override bool CheckSerialReceiveDataComplete(byte[] buffer, int receivedLength)
	{
		if (receivedLength > 1)
		{
			return buffer[receivedLength - 1] == 13;
		}
		return false;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"OmronHostLinkCModeServer[{base.Port}]";
	}
}
