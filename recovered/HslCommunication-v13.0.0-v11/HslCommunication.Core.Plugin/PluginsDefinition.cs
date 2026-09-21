using System;

namespace HslCommunication.Core.Plugin;

internal class PluginsDefinition
{
	public string DllName { get; set; }

	public string Company { get; set; }

	public string Description { get; set; }

	public string Http { get; set; }

	public string Framework { get; set; }

	public string Version { get; set; }

	public byte[] Icon16 { get; set; }

	public DateTime ReleaseData { get; set; }

	/// <summary>
	/// 获取插件类型的字符串信息，如果没有定义的话，就使用dll本身的名字作为插件类型信息
	/// </summary>
	/// <returns>字符串内容</returns>
	public string GetPluginTypeString()
	{
		if (DllName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
		{
			return DllName.Substring(0, DllName.Length - 4);
		}
		return DllName;
	}
}
