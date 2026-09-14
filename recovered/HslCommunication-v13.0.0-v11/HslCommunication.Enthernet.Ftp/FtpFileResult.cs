using System;
using System.Collections.Generic;
using HslCommunication.Core;
using HslCommunication.Core.Pipe;

namespace HslCommunication.Enthernet.Ftp;

internal class FtpFileResult
{
	public PipeTcpNet PipeTcpNet { get; set; }

	public List<FtpFileItem> Files { get; set; }

	public bool IsCompleted { get; set; } = false;

	public string ErrorMessage { get; set; }

	public Action<long, long> ProcessReport { get; set; }

	public HslCancelToken CancelToken { get; set; }
}
