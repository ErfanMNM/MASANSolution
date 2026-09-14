using System;
using Newtonsoft.Json;

namespace HslCommunication.Core.Plugin;

/// <summary>
/// 插件设备的定义内容
/// </summary>
public class PluginsDeviceDefinition
{
	/// <summary>
	/// 设备的基本名称信息
	/// </summary>
	public string DeviceName { get; set; }

	/// <summary>
	/// 当前设备的描述信息
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// 设备本身的图片信息，建议图片尺寸 128 * 128
	/// </summary>
	public byte[] Image { get; set; }

	/// <summary>
	/// 获取或设置小图标显示
	/// </summary>
	public string ImageKey { get; set; }

	/// <summary>
	/// 实际的设备类型对象信息
	/// </summary>
	[JsonIgnore]
	public Type DeviceObject { get; set; }

	/// <summary>
	/// 实际的设备类型的初始化配置参数信息
	/// </summary>
	[JsonIgnore]
	public Type DevicePara { get; set; }

	/// <summary>
	/// 插件的文件路径信息
	/// </summary>
	public string PluginFilePath { get; set; }

	/// <summary>
	/// 网站的地址
	/// </summary>
	public string Http { get; set; }

	/// <summary>
	/// 默认的实例化构造方法
	/// </summary>
	public PluginsDeviceDefinition()
	{
	}

	/// <summary>
	/// 根据传入的自定义对象，来初始化当前的设备插件对象
	/// </summary>
	/// <param name="plugin">设备对象</param>
	public PluginsDeviceDefinition(object plugin)
	{
		Type type = plugin.GetType();
		DeviceName = PluginsHelper.GetObjectProperty<string>(type, plugin, "DeviceName");
		Description = PluginsHelper.GetObjectProperty<string>(type, plugin, "Description");
		DeviceObject = PluginsHelper.GetObjectProperty<Type>(type, plugin, "DeviceObject");
		DevicePara = PluginsHelper.GetObjectProperty<Type>(type, plugin, "DevicePara");
	}
}
