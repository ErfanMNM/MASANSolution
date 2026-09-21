using System.Text;
using HslCommunication.Core;
using HslCommunication.Core.Pipe;

namespace HslCommunication.Enthernet.Ftp;

/// <summary>
/// Ftp的会话信息对象<br />
/// Ftp session information object
/// </summary>
public class FtpSession : ISessionContext
{
	/// <summary>
	/// 当前会话的编码信息<br />
	/// Current session encoding information
	/// </summary>
	internal Encoding Encoding { get; set; } = Encoding.Default;

	/// <summary>
	/// 当前的目录信息<br />
	/// Current directory information
	/// </summary>
	public string CurrentDirectory { get; set; } = "/";

	/// <summary>
	/// 获取或设置当前的会话对象<br />
	/// Get or set the current session object
	/// </summary>
	public PipeTcpNet Session { get; set; }

	/// <summary>
	/// 当前的用户名<br />
	/// Current username
	/// </summary>
	public string UserName { get; set; }

	/// <summary>
	/// 是否允许上传文件操作<br />
	/// can upload file operation
	/// </summary>
	public bool CanUpload { get; set; } = true;

	/// <summary>
	/// 是否允许删除文件，删除文件夹，创建文件夹，重命名文件，重命名文件夹操作<br />
	/// can delete file operation
	/// </summary>
	public bool CanEdit { get; set; } = true;

	/// <summary>
	/// 当前的会话信息关联的自定义信息<br />
	/// Custom information associated with the current session information
	/// </summary>
	public object Tag { get; set; }

	/// <summary>
	/// 当前接收的客户端ID信息<br />
	/// Client ID information currently received
	/// </summary>
	public string ClientId { get; set; }

	/// <summary>
	/// 示例化一个Ftp的会话对象<br />
	/// Instantiate an Ftp session object
	/// </summary>
	public FtpSession()
	{
		UserName = "anonymous";
		ClientId = string.Empty;
		Tag = null;
	}

	/// <summary>
	/// 获取实际的物理路径
	/// </summary>
	/// <param name="basePath">基础路径</param>
	/// <returns>实际的路径</returns>
	public string GetPhysicalPath(string basePath)
	{
		string text = CurrentDirectory;
		if (string.IsNullOrEmpty(text))
		{
			text = "/";
		}
		if (!text.StartsWith("/"))
		{
			text = "/" + text;
		}
		text = text.Replace("/", "\\");
		return basePath + text;
	}

	/// <summary>
	/// 获取文件路径信息
	/// </summary>
	/// <param name="basePath"></param>
	/// <param name="fileName"></param>
	/// <returns></returns>
	public string GetPhysicalFilePath(string basePath, string fileName)
	{
		string text = GetPhysicalPath(basePath);
		if (!text.EndsWith("\\"))
		{
			text += "\\";
		}
		if (fileName.StartsWith("/"))
		{
			fileName = fileName.Substring(1);
		}
		fileName = fileName.Replace("/", "\\");
		return text + fileName;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"Ftp Session[{UserName}@{Session}]";
	}
}
