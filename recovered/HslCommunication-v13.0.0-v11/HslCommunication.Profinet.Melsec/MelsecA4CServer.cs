using System;
using System.Text;
using HslCommunication.BasicFramework;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 基于MC协议的A4C格式虚拟服务器，可以模拟A4C格式的PLC，支持格式1，2，3，4，5，具体可以使用<see cref="P:HslCommunication.Profinet.Melsec.MelsecA3CServer.Format" />来设置，支持设置是否校验，同时支持网口或是串口的访问<br />
/// </summary>
/// <remarks>
/// 可访问的地址支持M,X,Y,B,D,W,R,ZR地址，其中 M,X,Y,B 支持位访问
/// </remarks>
public class MelsecA4CServer : MelsecA3CServer
{
	private int format = 1;

	/// <inheritdoc cref="P:HslCommunication.Profinet.Melsec.Helper.IReadWriteA3C.Format" />
	public override int Format
	{
		get
		{
			return format;
		}
		set
		{
			LogMsgFormatBinary = value == 5;
			format = value;
		}
	}

	/// <inheritdoc />
	protected override INetMessage GetNewNetMessage()
	{
		return new MelsecA4CServerMessage
		{
			Format = Format,
			SumCheck = base.SumCheck
		};
	}

	/// <inheritdoc />
	protected override bool CheckSerialReceiveDataComplete(byte[] buffer, int receivedLength)
	{
		try
		{
			return MelsecA4CServerMessage.CheckServer4CCommandComplete(buffer, Format, receivedLength, base.SumCheck);
		}
		catch (Exception ex)
		{
			base.LogNet?.WriteException("CheckSerialReceiveDataComplete", SoftBasic.GetAsciiStringRender(buffer.SelectBegin(receivedLength)), ex);
			return true;
		}
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> ExtraMcCommand(byte[] command)
	{
		if (Format == 1)
		{
			if (base.SumCheck)
			{
				return OperateResult.CreateSuccessResult(command.SelectMiddle(17, command.Length - 19));
			}
			return OperateResult.CreateSuccessResult(command.SelectMiddle(17, command.Length - 17));
		}
		if (Format == 2)
		{
			if (base.SumCheck)
			{
				return OperateResult.CreateSuccessResult(command.SelectMiddle(19, command.Length - 21));
			}
			return OperateResult.CreateSuccessResult(command.SelectMiddle(19, command.Length - 19));
		}
		if (Format == 3)
		{
			if (base.SumCheck)
			{
				return OperateResult.CreateSuccessResult(command.SelectMiddle(17, command.Length - 20));
			}
			return OperateResult.CreateSuccessResult(command.SelectMiddle(17, command.Length - 18));
		}
		if (Format == 4)
		{
			if (base.SumCheck)
			{
				return OperateResult.CreateSuccessResult(command.SelectMiddle(17, command.Length - 21));
			}
			return OperateResult.CreateSuccessResult(command.SelectMiddle(17, command.Length - 19));
		}
		if (Format == 5)
		{
			if (base.SumCheck)
			{
				return OperateResult.CreateSuccessResult(command.SelectMiddle(12, command.Length - 16));
			}
			return OperateResult.CreateSuccessResult(command.SelectMiddle(12, command.Length - 14));
		}
		return new OperateResult<byte[]>("Not Support Format:" + Format);
	}

	/// <inheritdoc />
	protected override byte[] PackCommand(ushort status, byte[] data)
	{
		if (data == null)
		{
			data = new byte[0];
		}
		if (data.Length == 0)
		{
			if (Format == 1)
			{
				if (status == 0)
				{
					byte[] bytes = Encoding.ASCII.GetBytes("\u0006F80000FF03FF0000");
					SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(bytes, 3);
					return bytes;
				}
				byte[] bytes2 = Encoding.ASCII.GetBytes("\u0015F80000FF03FF00000000");
				SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(bytes2, 3);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes2, bytes2.Length - 4);
				return bytes2;
			}
			if (Format == 2)
			{
				if (status == 0)
				{
					byte[] bytes3 = Encoding.ASCII.GetBytes("\u000600F80000FF03FF0000");
					SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(bytes3, 5);
					return bytes3;
				}
				byte[] bytes4 = Encoding.ASCII.GetBytes("\u001500F80000FF03FF00000000");
				SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(bytes4, 5);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes4, bytes4.Length - 4);
				return bytes4;
			}
			if (Format == 3)
			{
				if (status == 0)
				{
					byte[] bytes5 = Encoding.ASCII.GetBytes("\u0002F80000FF03FF0000QACK\u0003");
					SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(bytes5, 3);
					return bytes5;
				}
				byte[] bytes6 = Encoding.ASCII.GetBytes("\u0002F80000FF03FF0000QNAK0000\u0003");
				SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(bytes6, 3);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes6, bytes6.Length - 5);
				return bytes6;
			}
			if (Format == 4)
			{
				if (status == 0)
				{
					byte[] bytes7 = Encoding.ASCII.GetBytes("\u0006F80000FF03FF0000\r\n");
					SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(bytes7, 3);
					return bytes7;
				}
				byte[] bytes8 = Encoding.ASCII.GetBytes("\u0015F80000FF03FF00000000\r\n");
				SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(bytes8, 3);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes8, bytes8.Length - 6);
				return bytes8;
			}
			if (Format == 5)
			{
				if (status == 0)
				{
					byte[] array = (base.SumCheck ? new byte[20] : new byte[18]);
					"10020C00F80000FFFF030000FFFF00001003".ToHexBytes().CopyTo(array, 0);
					array[5] = base.Station;
					if (base.SumCheck)
					{
						MelsecA3CServer.SetSumCheck(array, 2, 4, 2);
					}
					return array;
				}
				byte[] array2 = (base.SumCheck ? new byte[20] : new byte[18]);
				"10020C00F80000FFFF030000FFFF00001003".ToHexBytes().CopyTo(array2, 0);
				array2[5] = base.Station;
				if (base.SumCheck)
				{
					MelsecA3CServer.SetSumCheck(array2, 2, 4, 2);
				}
				BitConverter.GetBytes(status).CopyTo(array2, array2.Length - 6);
				return array2;
			}
			return null;
		}
		if (Format == 1)
		{
			if (status != 0)
			{
				byte[] bytes9 = Encoding.ASCII.GetBytes("\u0015F80000FF03FF00000000");
				SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(bytes9, 3);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes9, bytes9.Length - 4);
				return bytes9;
			}
			byte[] array3 = new byte[(base.SumCheck ? 20 : 18) + data.Length];
			Encoding.ASCII.GetBytes("\u0002F80000FF03FF0000").CopyTo(array3, 0);
			SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(array3, 3);
			data.CopyTo(array3, 17);
			array3[array3.Length - ((!base.SumCheck) ? 1 : 3)] = 3;
			if (base.SumCheck)
			{
				MelsecA3CServer.SetSumCheck(array3, 1, 2);
			}
			return array3;
		}
		if (Format == 2)
		{
			if (status != 0)
			{
				byte[] bytes10 = Encoding.ASCII.GetBytes("\u001500F80000FF03FF00000000");
				SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(bytes10, 5);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes10, bytes10.Length - 4);
				return bytes10;
			}
			byte[] array4 = new byte[(base.SumCheck ? 22 : 20) + data.Length];
			Encoding.ASCII.GetBytes("\u000200F80000FF03FF0000").CopyTo(array4, 0);
			SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(array4, 5);
			data.CopyTo(array4, 19);
			array4[array4.Length - ((!base.SumCheck) ? 1 : 3)] = 3;
			if (base.SumCheck)
			{
				MelsecA3CServer.SetSumCheck(array4, 1, 2);
			}
			return array4;
		}
		if (Format == 3)
		{
			if (status != 0)
			{
				byte[] bytes11 = Encoding.ASCII.GetBytes("\u0002F80000FF03FF0000QNAK0000\u0003");
				SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(bytes11, 3);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes11, bytes11.Length - 5);
				return bytes11;
			}
			byte[] array5 = new byte[(base.SumCheck ? 24 : 22) + data.Length];
			Encoding.ASCII.GetBytes("\u0002F80000FF03FF0000QACK").CopyTo(array5, 0);
			SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(array5, 3);
			array5[array5.Length - ((!base.SumCheck) ? 1 : 3)] = 3;
			data.CopyTo(array5, 21);
			if (base.SumCheck)
			{
				MelsecA3CServer.SetSumCheck(array5, 1, 2);
			}
			return array5;
		}
		if (Format == 4)
		{
			if (status != 0)
			{
				byte[] bytes12 = Encoding.ASCII.GetBytes("\u0015F80000FF03FF00000000\r\n");
				SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(bytes12, 3);
				SoftBasic.BuildAsciiBytesFrom(status).CopyTo(bytes12, bytes12.Length - 6);
				return bytes12;
			}
			byte[] array6 = new byte[(base.SumCheck ? 22 : 20) + data.Length];
			Encoding.ASCII.GetBytes("\u0002F80000FF03FF0000").CopyTo(array6, 0);
			SoftBasic.BuildAsciiBytesFrom(base.Station).CopyTo(array6, 3);
			array6[array6.Length - (base.SumCheck ? 5 : 3)] = 3;
			data.CopyTo(array6, 17);
			if (base.SumCheck)
			{
				MelsecA3CServer.SetSumCheck(array6, 1, 4);
			}
			array6[array6.Length - 2] = 13;
			array6[array6.Length - 1] = 10;
			return array6;
		}
		if (Format == 5)
		{
			if (status != 0)
			{
				byte[] array7 = (base.SumCheck ? new byte[20] : new byte[18]);
				"10020C00F80000FFFF030000FFFF00001003".ToHexBytes().CopyTo(array7, 0);
				array7[5] = base.Station;
				BitConverter.GetBytes(status).CopyTo(array7, array7.Length - 4);
				return array7;
			}
			byte[] array8 = new byte[(base.SumCheck ? 20 : 18) + data.Length];
			"10020000F80000FFFF030000FFFF0000".ToHexBytes().CopyTo(array8, 0);
			array8[5] = base.Station;
			array8[array8.Length - ((!base.SumCheck) ? 1 : 3)] = 3;
			array8[array8.Length - (base.SumCheck ? 4 : 2)] = 16;
			data.CopyTo(array8, 16);
			int value = 12 + data.Length;
			array8[2] = BitConverter.GetBytes(value)[0];
			array8[3] = BitConverter.GetBytes(value)[1];
			if (base.SumCheck)
			{
				MelsecA3CServer.SetSumCheck(array8, 2, 4, 2);
			}
			return array8;
		}
		return null;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"MelsecA4CServer[{base.Port}]";
	}
}
