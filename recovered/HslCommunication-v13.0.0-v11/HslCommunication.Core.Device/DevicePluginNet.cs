using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HslCommunication.Core.IMessage;
using HslCommunication.Core.Plugin;

namespace HslCommunication.Core.Device;

/// <summary>
/// 插件设备对象，插件设备定义一些方法就可以使用HSL的全部方法，具体方法参考官网文档及skill说明<br />
/// Plugin device object. By defining some methods for the plugin device, all the methods of HSL can be used. For specific methods, please refer to the official website documentation and the skill instructions.
/// </summary>
public class DevicePluginNet : DeviceTcpNet
{
	private Func<string, ushort, bool, List<byte[]>> BuildDeviceReadCommandFunc = null;

	private Func<string, bool[], byte[]> BuildDeviceWriteBoolCommandFunc = null;

	private Func<string, byte[], byte[]> BuildDeviceWriteByteCommandFunc = null;

	private Func<string, byte[], byte[], string> CheckDeviceResponseFunc = null;

	private Func<string, byte[], byte[], byte[]> ParseDeviceReadResultFunc = null;

	private Func<string, byte[], byte[], ushort, bool[]> ParseDeviceReadBoolResultFunc = null;

	private Func<int> InitializationOnConnectFunc = null;

	private Func<int> ProtocolHeadBytesLengthFunc = null;

	private Func<byte[], byte[], int> GetContentLengthByHeadBytesFunc = null;

	private Func<byte[], byte[], bool> CheckReceiveDataCompleteFunc = null;

	private Func<string, ushort, byte[]> ReadBytesFunc = null;

	private Func<string, ushort, bool[]> ReadBoolFunc = null;

	private Func<string, byte[], string> WriteBytesFunc = null;

	private Func<string, bool[], string> WriteBoolFunc = null;

	private Func<string, ushort, Task<byte[]>> ReadBytesAsyncFunc = null;

	private Func<string, ushort, Task<bool[]>> ReadBoolAsyncFunc = null;

	private Func<string, byte[], Task<string>> WriteBytesAsyncFunc = null;

	private Func<string, bool[], Task<string>> WriteBoolAsyncFunc = null;

	private object pluginObject = null;

	private DevicePluginMessage pluginMessage = null;

	/// <summary>
	/// 指定一个自定义的用户对象来创建插件设备
	/// </summary>
	public DevicePluginNet(object deviceObject)
	{
		base.ByteTransform = new RegularByteTransform();
		pluginMessage = new DevicePluginMessage();
		SetDeviceObject(deviceObject);
	}

	private void SetDeviceObject(object deviceObject)
	{
		pluginObject = deviceObject;
		Type type = deviceObject.GetType();
		BuildDeviceReadCommandFunc = PluginsHelper.ExecuteFuncThree<string, ushort, bool, List<byte[]>>(type, deviceObject, "BuildDeviceReadCommand", new string[3] { "address", "length", "isBit" });
		BuildDeviceWriteBoolCommandFunc = PluginsHelper.ExecuteFuncTwo<string, bool[], byte[]>(type, deviceObject, "BuildDeviceWriteCommand", new string[2] { "address", "value" });
		BuildDeviceWriteByteCommandFunc = PluginsHelper.ExecuteFuncTwo<string, byte[], byte[]>(type, deviceObject, "BuildDeviceWriteCommand", new string[2] { "address", "value" });
		CheckDeviceResponseFunc = PluginsHelper.ExecuteFuncThree<string, byte[], byte[], string>(type, deviceObject, "CheckDeviceResponse", new string[3] { "address", "buildCommand", "content" });
		ParseDeviceReadResultFunc = PluginsHelper.ExecuteFuncThree<string, byte[], byte[], byte[]>(type, deviceObject, "ParseDeviceReadResult", new string[3] { "address", "buildCommand", "content" });
		ParseDeviceReadBoolResultFunc = PluginsHelper.ExecuteFuncFour<string, byte[], byte[], ushort, bool[]>(type, deviceObject, "ParseDeviceReadBoolResult", new string[4] { "address", "buildCommand", "content", "ushort" });
		ProtocolHeadBytesLengthFunc = PluginsHelper.ExecuteFuncZero<int>(type, deviceObject, "ProtocolHeadBytesLength", new string[0]);
		GetContentLengthByHeadBytesFunc = PluginsHelper.ExecuteFuncTwo<byte[], byte[], int>(type, deviceObject, "GetContentLengthByHeadBytes", new string[2] { "send", "head" });
		CheckReceiveDataCompleteFunc = PluginsHelper.ExecuteFuncTwo<byte[], byte[], bool>(type, deviceObject, "CheckReceiveDataComplete", new string[2] { "send", "content" });
		ReadBytesFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, byte[]>(type, deviceObject, "Read", new string[2] { "address", "length" });
		ReadBoolFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, bool[]>(type, deviceObject, "ReadBool", new string[2] { "address", "length" });
		WriteBytesFunc = PluginsHelper.ExecuteFuncTwo<string, byte[], string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		WriteBoolFunc = PluginsHelper.ExecuteFuncTwo<string, bool[], string>(type, deviceObject, "Write", new string[2] { "address", "value" });
		ReadBytesAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, Task<byte[]>>(type, deviceObject, "ReadAsync", new string[2] { "address", "length" });
		ReadBoolAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, ushort, Task<bool[]>>(type, deviceObject, "ReadBoolAsync", new string[2] { "address", "length" });
		WriteBytesAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, byte[], Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		WriteBoolAsyncFunc = PluginsHelper.ExecuteFuncTwo<string, bool[], Task<string>>(type, deviceObject, "WriteAsync", new string[2] { "address", "value" });
		if (ProtocolHeadBytesLengthFunc != null)
		{
			pluginMessage.ProtocolHeadBytesLengthFunc = ProtocolHeadBytesLengthFunc;
		}
		if (GetContentLengthByHeadBytesFunc != null)
		{
			pluginMessage.GetContentLengthByHeadBytesFunc = GetContentLengthByHeadBytesFunc;
		}
		if (CheckReceiveDataCompleteFunc != null)
		{
			pluginMessage.CheckReceiveDataCompleteFunc = CheckReceiveDataCompleteFunc;
		}
		InitializationOnConnectFunc = PluginsHelper.ExecuteFuncZero<int>(type, deviceObject, "InitializationOnConnect", new string[0]);
		PropertyInfo property = type.GetProperty("ReadFromCoreServerFunc");
		if (property != null && property.PropertyType == typeof(Func<byte[], bool, bool, byte[]>) && property.CanWrite)
		{
			property.SetValue(deviceObject, new Func<byte[], bool, bool, byte[]>(ReadFromCoreServerHelper));
		}
		PropertyInfo property2 = type.GetProperty("ReadFromCoreServer");
		if (property2 != null && property2.PropertyType == typeof(Func<byte[], byte[]>) && property2.CanWrite)
		{
			property2.SetValue(deviceObject, new Func<byte[], byte[]>(ReadFromCoreServerHelper2));
		}
		PropertyInfo property3 = type.GetProperty("DataFormat", new Type[0]);
		if (property3 != null && property3.PropertyType == typeof(string))
		{
			string text = property3.GetValue(deviceObject, null).ToString();
			if (!string.IsNullOrEmpty(text))
			{
				if (text.Equals("ABCD", StringComparison.OrdinalIgnoreCase))
				{
					base.ByteTransform.DataFormat = DataFormat.ABCD;
				}
				else if (text.Equals("BADC", StringComparison.OrdinalIgnoreCase))
				{
					base.ByteTransform.DataFormat = DataFormat.BADC;
				}
				else if (text.Equals("CDAB", StringComparison.OrdinalIgnoreCase))
				{
					base.ByteTransform.DataFormat = DataFormat.CDAB;
				}
				else if (text.Equals("DCBA", StringComparison.OrdinalIgnoreCase))
				{
					base.ByteTransform.DataFormat = DataFormat.DCBA;
				}
			}
		}
		PropertyInfo property4 = type.GetProperty("WordLength", new Type[0]);
		if (property4 != null && property4.PropertyType == typeof(ushort))
		{
			base.WordLength = (ushort)property4.GetValue(deviceObject, null);
		}
	}

	/// <inheritdoc />
	protected override INetMessage GetNewNetMessage()
	{
		return pluginMessage;
	}

	/// <inheritdoc />
	protected override OperateResult InitializationOnConnect()
	{
		if (InitializationOnConnectFunc != null)
		{
			try
			{
				InitializationOnConnectFunc();
			}
			catch (Exception ex)
			{
				return new OperateResult("InitializationOnConnect failed: " + ex.Message);
			}
		}
		return base.InitializationOnConnect();
	}

	/// <inheritdoc />
	protected override async Task<OperateResult> InitializationOnConnectAsync()
	{
		if (InitializationOnConnectFunc != null)
		{
			try
			{
				InitializationOnConnectFunc();
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				return new OperateResult("InitializationOnConnect failed: " + ex2.Message);
			}
		}
		return await base.InitializationOnConnectAsync().ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <inheritdoc />
	protected override OperateResult<List<byte[]>> BuildDeviceReadCommand(string address, ushort length, bool isBit)
	{
		if (BuildDeviceReadCommandFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(BuildDeviceReadCommandFunc(address, length, isBit));
			}
			catch (Exception ex)
			{
				return new OperateResult<List<byte[]>>(ex.Message);
			}
		}
		return base.BuildDeviceReadCommand(address, length, isBit);
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, bool[] value)
	{
		if (BuildDeviceWriteBoolCommandFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(BuildDeviceWriteBoolCommandFunc(address, value));
			}
			catch (Exception ex)
			{
				return new OperateResult<byte[]>(ex.Message);
			}
		}
		return base.BuildDeviceWriteCommand(address, value);
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> BuildDeviceWriteCommand(string address, byte[] value)
	{
		if (BuildDeviceWriteByteCommandFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(BuildDeviceWriteByteCommandFunc(address, value));
			}
			catch (Exception ex)
			{
				return new OperateResult<byte[]>(ex.Message);
			}
		}
		return base.BuildDeviceWriteCommand(address, value);
	}

	/// <inheritdoc />
	protected override OperateResult CheckDeviceResponse(string address, byte[] buildCommand, byte[] content)
	{
		if (CheckDeviceResponseFunc != null)
		{
			try
			{
				string text = CheckDeviceResponseFunc(address, buildCommand, content);
				if (string.IsNullOrEmpty(text))
				{
					return OperateResult.CreateSuccessResult();
				}
				return new OperateResult<byte[]>(text);
			}
			catch (Exception ex)
			{
				return new OperateResult<byte[]>("CheckDeviceResponse failed: " + ex.Message);
			}
		}
		return base.CheckDeviceResponse(address, buildCommand, content);
	}

	/// <inheritdoc />
	protected override OperateResult<byte[]> ParseDeviceReadResult(string address, byte[] buildCommand, byte[] content)
	{
		if (ParseDeviceReadResultFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(ParseDeviceReadResultFunc(address, buildCommand, content));
			}
			catch (Exception ex)
			{
				return new OperateResult<byte[]>("ParseDeviceReadResult failed: " + ex.Message);
			}
		}
		return base.ParseDeviceReadResult(address, buildCommand, content);
	}

	/// <inheritdoc />
	protected override OperateResult<bool[]> ParseDeviceReadBoolResult(string address, byte[] buildCommand, byte[] content, ushort length)
	{
		if (ParseDeviceReadBoolResultFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(ParseDeviceReadBoolResultFunc(address, buildCommand, content, length));
			}
			catch (Exception ex)
			{
				return new OperateResult<bool[]>("ParseDeviceReadBoolResult failed: " + ex.Message);
			}
		}
		return base.ParseDeviceReadBoolResult(address, buildCommand, content, length);
	}

	/// <inheritdoc />
	public override OperateResult<byte[]> Read(string address, ushort length)
	{
		if (ReadBytesFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(ReadBytesFunc(address, length));
			}
			catch (Exception ex)
			{
				return new OperateResult<byte[]>(ex.Message);
			}
		}
		return base.Read(address, length);
	}

	/// <inheritdoc />
	public override OperateResult<bool[]> ReadBool(string address, ushort length)
	{
		if (ReadBoolFunc != null)
		{
			try
			{
				return OperateResult.CreateSuccessResult(ReadBoolFunc(address, length));
			}
			catch (Exception ex)
			{
				return new OperateResult<bool[]>(ex.Message);
			}
		}
		return base.ReadBool(address, length);
	}

	/// <inheritdoc />
	public override OperateResult Write(string address, byte[] value)
	{
		if (WriteBytesFunc != null)
		{
			try
			{
				string value2 = WriteBytesFunc(address, value);
				if (string.IsNullOrEmpty(value2))
				{
					return OperateResult.CreateSuccessResult();
				}
			}
			catch (Exception ex)
			{
				return new OperateResult<bool[]>(ex.Message);
			}
		}
		return base.Write(address, value);
	}

	/// <inheritdoc />
	public override OperateResult Write(string address, bool[] value)
	{
		if (WriteBoolFunc != null)
		{
			try
			{
				string value2 = WriteBoolFunc(address, value);
				if (string.IsNullOrEmpty(value2))
				{
					return OperateResult.CreateSuccessResult();
				}
			}
			catch (Exception ex)
			{
				return new OperateResult<bool[]>(ex.Message);
			}
		}
		return base.Write(address, value);
	}

	private byte[] ReadFromCoreServerHelper(byte[] send, bool hasResponseData, bool usePackAndUnpack)
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(CommunicationPipe, send, hasResponseData, usePackAndUnpack);
		if (operateResult.IsSuccess)
		{
			return operateResult.Content;
		}
		throw new Exception(operateResult.Message);
	}

	private byte[] ReadFromCoreServerHelper2(byte[] send)
	{
		OperateResult<byte[]> operateResult = ReadFromCoreServer(send);
		if (operateResult.IsSuccess)
		{
			return operateResult.Content;
		}
		throw new Exception(operateResult.Message);
	}

	/// <summary>
	/// 获取当前的插件对象
	/// </summary>
	/// <returns></returns>
	public dynamic GetPluginObject()
	{
		return pluginObject;
	}

	/// <summary>
	/// 从一个文件里加载设备的插件信息，指定插件文件路径，设备名称，设置参数的方法
	/// </summary>
	/// <param name="file">插件文件的完整路径</param>
	/// <param name="deviceName">设备名称</param>
	/// <returns>创建的参数对象信息</returns>
	public static DevicePluginNet CreateFromFile(string file, string deviceName = null)
	{
		Assembly assembly = Assembly.LoadFrom(file);
		Type type = assembly.GetTypes().FirstOrDefault((Type m) => m.Name == "PluginsHelper");
		if (type != null)
		{
			object obj = type.GetMethod("DeviceDefinitions")?.Invoke(null, null);
			if (obj != null)
			{
				if (obj.GetType() == typeof(object[]))
				{
					return CreateDevicePluginNet(obj as object[], deviceName);
				}
				if (obj.GetType() == typeof(List<object>))
				{
					return CreateDevicePluginNet(obj as List<object>, deviceName);
				}
			}
		}
		return null;
	}

	private static DevicePluginNet CreateDevicePluginNet(IEnumerable<object> list, string deviceName = null)
	{
		foreach (object item in list)
		{
			bool flag = false;
			PluginsDeviceDefinition pluginsDeviceDefinition = new PluginsDeviceDefinition(item);
			if (!string.IsNullOrEmpty(deviceName) && deviceName == pluginsDeviceDefinition.DeviceName)
			{
				flag = true;
			}
			if (string.IsNullOrEmpty(deviceName))
			{
				flag = true;
			}
			if (flag)
			{
				object obj = null;
				object obj2 = null;
				obj2 = ((obj != null) ? pluginsDeviceDefinition.DeviceObject.Assembly.CreateInstance(pluginsDeviceDefinition.DeviceObject.FullName, ignoreCase: true, BindingFlags.Default, null, new object[1] { obj }, null, null) : pluginsDeviceDefinition.DeviceObject.Assembly.CreateInstance(pluginsDeviceDefinition.DeviceObject.FullName, ignoreCase: true, BindingFlags.Default, null, new object[0], null, null));
				return new DevicePluginNet(obj2);
			}
		}
		return null;
	}
}
