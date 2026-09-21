using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HslCommunication.Core;
using HslCommunication.Core.Address;
using HslCommunication.Core.Device;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Net;
using HslCommunication.Profinet.Melsec.Helper;
using HslCommunication.Reflection;

namespace HslCommunication.Profinet.Melsec;

/// <summary>
/// 三菱计算机链接协议的网口版本，适用FX3U系列，FX3G，FX3S等等系列，通常在PLC侧连接的是485的接线口<br />
/// Network port version of Mitsubishi Computer Link Protocol, suitable for FX3U series, FX3G, FX3S, etc., usually the 485 connection port is connected on the PLC side
/// </summary>
/// <remarks>
/// 关于在PLC侧的配置信息，协议：专用协议  传送控制步骤：格式一  站号设置：0
/// </remarks>
public class MelsecFxLinksOverTcp : DeviceTcpNet, IReadWriteFxLinks, IReadWriteDevice, IReadWriteNet, IReadWriteDeviceStation
{
	private byte station = 0;

	private byte waittingTime = 0;

	private bool sumCheck = true;

	/// <inheritdoc cref="P:HslCommunication.Profinet.Melsec.Helper.IReadWriteFxLinks.Station" />
	public byte Station
	{
		get
		{
			return station;
		}
		set
		{
			station = value;
		}
	}

	/// <inheritdoc cref="P:HslCommunication.Profinet.Melsec.Helper.IReadWriteFxLinks.WaittingTime" />
	public byte WaittingTime
	{
		get
		{
			return waittingTime;
		}
		set
		{
			if (value > 15)
			{
				waittingTime = 15;
			}
			else
			{
				waittingTime = value;
			}
		}
	}

	/// <inheritdoc cref="P:HslCommunication.Profinet.Melsec.Helper.IReadWriteFxLinks.SumCheck" />
	public bool SumCheck
	{
		get
		{
			return sumCheck;
		}
		set
		{
			sumCheck = value;
		}
	}

	/// <inheritdoc cref="P:HslCommunication.Profinet.Melsec.Helper.IReadWriteFxLinks.Format" />
	public int Format { get; set; } = 1;

	/// <summary>
	/// 实例化默认的对象<br />
	/// Instantiate the default object
	/// </summary>
	public MelsecFxLinksOverTcp()
	{
		base.WordLength = 1;
		base.ByteTransform = new RegularByteTransform();
		LogMsgFormatBinary = false;
	}

	/// <summary>
	/// 指定ip地址和端口号来实例化默认的对象<br />
	/// Specify the IP address and port number to instantiate the default object
	/// </summary>
	/// <param name="ipAddress">Ip地址信息</param>
	/// <param name="port">端口号</param>
	public MelsecFxLinksOverTcp(string ipAddress, int port)
		: this()
	{
		IpAddress = ipAddress;
		Port = port;
	}

	/// <inheritdoc />
	protected override INetMessage GetNewNetMessage()
	{
		return new MelsecFxLinksMessage(Format, SumCheck);
	}

	/// <inheritdoc />
	public override byte[] PackCommandWithHeader(byte[] command)
	{
		return MelsecFxLinksHelper.PackCommandWithHeader(this, command);
	}

	/// <inheritdoc />
	public override OperateResult<byte[]> UnpackResponseContent(byte[] send, byte[] response)
	{
		return MelsecFxLinksHelper.CheckPlcResponse(response);
	}

	/// <inheritdoc />
	public virtual OperateResult<MelsecFxLinksAddress> AnalyzeAddress(string address)
	{
		return MelsecFxLinksAddress.ParseFrom(address);
	}

	/// <inheritdoc />
	protected override OperateResult<List<byte[]>> BuildDeviceReadCommand(string address, ushort length, bool isBit)
	{
		return MelsecFxLinksHelper.BuildReadCommand(this, address, length, isBit);
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, byte[] value)
	{
		return MelsecFxLinksHelper.BuildWriteByteCommand(this, address, value);
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, bool[] value)
	{
		return MelsecFxLinksHelper.BuildWriteBoolCommand(this, address, value);
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> ParseDeviceReadResult(string address, byte[] buildCommand, byte[] content)
	{
		return MelsecFxLinksHelper.ExtraResponse(content);
	}

	/// <inheritdoc />
	protected override OperateResult<bool[]> ParseDeviceReadBoolResult(string address, byte[] buildCommand, byte[] content, ushort length)
	{
		return OperateResult.CreateSuccessResult(content.Select((byte m) => m == 49).ToArray());
	}

	/// <inheritdoc />
	[HslMqttApi("ReadBoolArray", "")]
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (address.IndexOf('.') > 0)
		{
			return HslHelper.ReadBool(this, address, length);
		}
		return base.ReadBool(address, length);
	}

	/// <inheritdoc />
	public override async Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length)
	{
		if (address.IndexOf('.') > 0)
		{
			return await HslHelper.ReadBoolAsync(this, address, length);
		}
		return await base.ReadBoolAsync(address, length);
	}

	/// <inheritdoc cref="M:HslCommunication.Profinet.Melsec.Helper.MelsecFxLinksHelper.StartPLC(HslCommunication.Profinet.Melsec.Helper.IReadWriteFxLinks,System.String)" />
	[HslMqttApi(Description = "Start the PLC operation, you can carry additional parameter information and specify the station number. Example: s=2; Note: The semicolon is required.")]
	public OperateResult StartPLC(string parameter = "")
	{
		return MelsecFxLinksHelper.StartPLC(this, parameter);
	}

	/// <inheritdoc cref="M:HslCommunication.Profinet.Melsec.Helper.MelsecFxLinksHelper.StopPLC(HslCommunication.Profinet.Melsec.Helper.IReadWriteFxLinks,System.String)" />
	[HslMqttApi(Description = "Stop PLC operation, you can carry additional parameter information and specify the station number. Example: s=2; Note: The semicolon is required.")]
	public OperateResult StopPLC(string parameter = "")
	{
		return MelsecFxLinksHelper.StopPLC(this, parameter);
	}

	/// <inheritdoc cref="M:HslCommunication.Profinet.Melsec.Helper.MelsecFxLinksHelper.ReadPlcType(HslCommunication.Profinet.Melsec.Helper.IReadWriteFxLinks,System.String)" />
	[HslMqttApi(Description = "Read the PLC model information, you can carry additional parameter information, and specify the station number. Example: s=2; Note: The semicolon is required.")]
	public OperateResult<string> ReadPlcType(string parameter = "")
	{
		return MelsecFxLinksHelper.ReadPlcType(this, parameter);
	}

	/// <inheritdoc cref="M:HslCommunication.Profinet.Melsec.MelsecFxLinksOverTcp.StartPLC(System.String)" />
	public async Task<OperateResult> StartPLCAsync(string parameter = "")
	{
		return await MelsecFxLinksHelper.StartPLCAsync(this, parameter);
	}

	/// <inheritdoc cref="M:HslCommunication.Profinet.Melsec.MelsecFxLinksOverTcp.StopPLC(System.String)" />
	public async Task<OperateResult> StopPLCAsync(string parameter = "")
	{
		return await MelsecFxLinksHelper.StopPLCAsync(this, parameter);
	}

	/// <inheritdoc cref="M:HslCommunication.Profinet.Melsec.MelsecFxLinksOverTcp.ReadPlcType(System.String)" />
	public async Task<OperateResult<string>> ReadPlcTypeAsync(string parameter = "")
	{
		return await MelsecFxLinksHelper.ReadPlcTypeAsync(this, parameter);
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"MelsecFxLinksOverTcp[{IpAddress}:{Port}]";
	}
}
