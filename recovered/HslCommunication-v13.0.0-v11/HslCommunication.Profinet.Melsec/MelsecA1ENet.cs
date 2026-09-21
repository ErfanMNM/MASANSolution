using System;
using System.Collections.Generic;
using System.Linq;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 三菱PLC通讯协议，采用A兼容1E帧协议实现，使用二进制码通讯，请根据实际型号来进行选取<br />
/// Mitsubishi PLC communication protocol, implemented using A compatible 1E frame protocol, using binary code communication, please choose according to the actual model
/// </summary>
/// <remarks>
/// 本类适用于的PLC列表
/// <list type="number">
/// <item>FX3U(C) PLC   测试人sandy_liao</item>
/// </list>
/// <note type="important">本通讯类由CKernal推送，感谢</note>
/// </remarks>
public class MelsecA1ENet : DeviceTcpNet
{
	/// <summary>
	/// PLC编号，默认为0xFF<br />
	/// PLC number, default is 0xFF
	/// </summary>
	public byte PLCNumber { get; set; } = byte.MaxValue;

	/// <summary>
	/// 实例化一个默认的对象<br />
	/// Instantiate a default object
	/// </summary>
	public MelsecA1ENet()
	{
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform();
	}

	/// <summary>
	/// 指定ip地址和端口来来实例化一个默认的对象<br />
	/// Specify the IP address and port to instantiate a default object
	/// </summary>
	/// <param name="ipAddress">PLC的Ip地址</param>
	/// <param name="port">PLC的端口</param>
	public MelsecA1ENet(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	/// <inheritdoc />
	protected override INetMessage GetNewNetMessage()
	{
		return new MelsecA1EBinaryMessage();
	}

	/// <inheritdoc />
	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		OperateResult operateResult = CheckResponseLegal(response);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		return base.UnpackResponseContent(send, response);
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, byte[] value)
	{
		return BuildWriteWordCommand(address, value, PLCNumber);
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, bool[] value)
	{
		return BuildWriteBoolCommand(address, value, PLCNumber);
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> ParseDeviceReadResult(string address, byte[] buildCommand, byte[] content)
	{
		return ExtractActualData(content, isBit: false);
	}

	/// <inheritdoc />
	protected override OperateResult<bool[]> ParseDeviceReadBoolResult(string address, byte[] buildCommand, byte[] content, ushort length)
	{
		OperateResult<byte[]> operateResult = ExtractActualData(content, isBit: true);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<bool[]>(operateResult);
		}
		return OperateResult.CreateSuccessResult(operateResult.Content.Select((byte m) => m == 1).ToArray());
	}

	/// <inheritdoc />
	protected override OperateResult<List<byte[]>> BuildDeviceReadCommand(string address, ushort length, bool isBit)
	{
		return BuildReadCommand(address, length, isBit, PLCNumber);
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"MelsecA1ENet[{IpAddress}:{Port}]";
	}

	/// <summary>
	/// 根据类型地址长度确认需要读取的指令头
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="length">长度</param>
	/// <param name="isBit">指示是否按照位成批的读出</param>
	/// <param name="plcNumber">PLC编号</param>
	/// <returns>带有成功标志的指令数据</returns>
	public static OperateResult<List<byte[]>> BuildReadCommand(string address, ushort length, bool isBit, byte plcNumber)
	{
		OperateResult<MelsecA1EDataType, int> operateResult = MelsecHelper.McA1EAnalysisAddress(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<List<byte[]>>(operateResult);
		}
		byte b = ((!isBit) ? ((byte)1) : ((byte)0));
		int[] array = SoftBasic.SplitIntegerToArray(length, isBit ? 256 : 64);
		List<byte[]> list = new List<byte[]>();
		for (int i = 0; i < array.Length; i++)
		{
			byte[] array2 = new byte[12]
			{
				b,
				plcNumber,
				10,
				0,
				BitConverter.GetBytes(operateResult.Content2)[0],
				BitConverter.GetBytes(operateResult.Content2)[1],
				BitConverter.GetBytes(operateResult.Content2)[2],
				BitConverter.GetBytes(operateResult.Content2)[3],
				BitConverter.GetBytes(operateResult.Content1.DataCode)[0],
				BitConverter.GetBytes(operateResult.Content1.DataCode)[1],
				0,
				0
			};
			int num = array[i];
			if (num == 256)
			{
				num = 0;
			}
			array2[10] = BitConverter.GetBytes(num)[0];
			array2[11] = BitConverter.GetBytes(num)[1];
			list.Add(array2);
			if (isBit)
			{
				operateResult.Content2 += array[i];
			}
			else if (operateResult.Content1.DataType == 1)
			{
				operateResult.Content2 += array[i] * 16;
			}
			else
			{
				operateResult.Content2 += array[i];
			}
		}
		return OperateResult.CreateSuccessResult(list);
	}

	/// <summary>
	/// 根据类型地址以及需要写入的数据来生成指令头
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">数据值</param>
	/// <param name="plcNumber">PLC编号</param>
	/// <returns>带有成功标志的指令数据</returns>
	public static OperateResult<byte[]> BuildWriteWordCommand(string address, byte[] value, byte plcNumber)
	{
		OperateResult<MelsecA1EDataType, int> operateResult = MelsecHelper.McA1EAnalysisAddress(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] array = new byte[12 + value.Length];
		array[0] = 3;
		array[1] = plcNumber;
		array[2] = 10;
		array[3] = 0;
		array[4] = BitConverter.GetBytes(operateResult.Content2)[0];
		array[5] = BitConverter.GetBytes(operateResult.Content2)[1];
		array[6] = BitConverter.GetBytes(operateResult.Content2)[2];
		array[7] = BitConverter.GetBytes(operateResult.Content2)[3];
		array[8] = BitConverter.GetBytes(operateResult.Content1.DataCode)[0];
		array[9] = BitConverter.GetBytes(operateResult.Content1.DataCode)[1];
		array[10] = BitConverter.GetBytes(value.Length / 2)[0];
		array[11] = BitConverter.GetBytes(value.Length / 2)[1];
		Array.Copy(value, 0, array, 12, value.Length);
		return OperateResult.CreateSuccessResult(array);
	}

	/// <summary>
	/// 根据类型地址以及需要写入的数据来生成指令头
	/// </summary>
	/// <param name="address">起始地址</param>
	/// <param name="value">数据值</param>
	/// <param name="plcNumber">PLC编号</param>
	/// <returns>带有成功标志的指令数据</returns>
	public static OperateResult<byte[]> BuildWriteBoolCommand(string address, bool[] value, byte plcNumber)
	{
		OperateResult<MelsecA1EDataType, int> operateResult = MelsecHelper.McA1EAnalysisAddress(address);
		if (!operateResult.IsSuccess)
		{
			return OperateResult.CreateFailedResult<byte[]>(operateResult);
		}
		byte[] array = MelsecHelper.TransBoolArrayToByteData(value);
		byte[] array2 = new byte[12 + array.Length];
		array2[0] = 2;
		array2[1] = plcNumber;
		array2[2] = 10;
		array2[3] = 0;
		array2[4] = BitConverter.GetBytes(operateResult.Content2)[0];
		array2[5] = BitConverter.GetBytes(operateResult.Content2)[1];
		array2[6] = BitConverter.GetBytes(operateResult.Content2)[2];
		array2[7] = BitConverter.GetBytes(operateResult.Content2)[3];
		array2[8] = BitConverter.GetBytes(operateResult.Content1.DataCode)[0];
		array2[9] = BitConverter.GetBytes(operateResult.Content1.DataCode)[1];
		array2[10] = BitConverter.GetBytes(value.Length)[0];
		array2[11] = BitConverter.GetBytes(value.Length)[1];
		Array.Copy(array, 0, array2, 12, array.Length);
		return OperateResult.CreateSuccessResult(array2);
	}

	/// <summary>
	/// 检测反馈的消息是否合法
	/// </summary>
	/// <param name="response">接收的报文</param>
	/// <returns>是否成功</returns>
	public static OperateResult CheckResponseLegal(byte[] response)
	{
		if (response.Length < 2)
		{
			return new OperateResult(StringResources.Language.ReceiveDataLengthTooShort);
		}
		if (response[1] == 0)
		{
			return OperateResult.CreateSuccessResult();
		}
		if (response[1] == 91 && response.Length > 2)
		{
			return new OperateResult(response[2], StringResources.Language.MelsecPleaseReferToManualDocument);
		}
		return new OperateResult(response[1], StringResources.Language.MelsecPleaseReferToManualDocument);
	}

	/// <summary>
	/// 从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取
	/// </summary>
	/// <param name="response">反馈的数据内容</param>
	/// <param name="isBit">是否位读取</param>
	/// <returns>解析后的结果对象</returns>
	public static OperateResult<byte[]> ExtractActualData(byte[] response, bool isBit)
	{
		if (isBit)
		{
			byte[] array = new byte[(response.Length - 2) * 2];
			for (int i = 2; i < response.Length; i++)
			{
				if ((response[i] & 0x10) == 16)
				{
					array[(i - 2) * 2] = 1;
				}
				if ((response[i] & 1) == 1)
				{
					array[(i - 2) * 2 + 1] = 1;
				}
			}
			return OperateResult.CreateSuccessResult(array);
		}
		return OperateResult.CreateSuccessResult(response.RemoveBegin(2));
	}
}
