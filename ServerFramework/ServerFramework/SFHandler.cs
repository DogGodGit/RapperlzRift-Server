using System;
using System.Text;
using Photon.SocketServer;

namespace ServerFramework;

public abstract class SFHandler : ISFHandler, ISFRunnable
{
	public const string kLogFooterPrefix = "#F ";

	protected ISFPeer m_sender;

	protected short m_snPacketName;

	protected byte[] m_rawBody;

	protected SendParameters m_sendParameters = default(SendParameters);

	public ISFPeer sender => m_sender;

	public short packetName => m_snPacketName;

	public byte[] rawBody => m_rawBody;

	public SendParameters sendParameters => m_sendParameters;

	protected void InitHandler(ISFPeer sender, short snPacketName, byte[] rawBody, SendParameters sendParameters)
	{
		if (sender == null)
		{
			throw new ArgumentNullException("sender");
		}
		m_sender = sender;
		m_snPacketName = snPacketName;
		m_rawBody = rawBody;
		m_sendParameters = sendParameters;
		DeserializeBody();
	}

	protected abstract void DeserializeBody();

	public void Run()
	{
		try
		{
			Handle();
		}
		catch (Exception ex)
		{
			LogError(null, ex);
		}
	}

	protected abstract void Handle();

	public void LogDebug(string sMessage)
	{
		LogDebug(sMessage, null);
	}

	public void LogDebug(string sMessage, Exception ex)
	{
		LogDebug(sMessage, ex, bStackTrace: false);
	}

	public void LogDebug(string sMessage, Exception ex, bool bStackTrace)
	{
		LogDebug(sMessage, ex, bStackTrace, bAsync: true);
	}

	public void LogDebug(string sMessage, Exception ex, bool bStackTrace, bool bAsync)
	{
		SFLogUtil.Debug(GetType(), MakeLogMessage(sMessage), ex, bStackTrace, bAsync);
	}

	public void LogInfo(string sMessage)
	{
		LogInfo(sMessage, null);
	}

	public void LogInfo(string sMessage, Exception ex)
	{
		LogInfo(sMessage, ex, bStackTrace: false);
	}

	public void LogInfo(string sMessage, Exception ex, bool bStackTrace)
	{
		LogInfo(sMessage, ex, bStackTrace, bAsync: true);
	}

	public void LogInfo(string sMessage, Exception ex, bool bStackTrace, bool bAsync)
	{
		SFLogUtil.Info(GetType(), MakeLogMessage(sMessage), ex, bStackTrace, bAsync);
	}

	public void LogWarn(string sMessage)
	{
		LogWarn(sMessage, null);
	}

	public void LogWarn(string sMessage, Exception ex)
	{
		LogWarn(sMessage, ex, bStackTrace: false);
	}

	public void LogWarn(string sMessage, Exception ex, bool bStackTrace)
	{
		LogWarn(sMessage, ex, bStackTrace, bAsync: true);
	}

	public void LogWarn(string sMessage, Exception ex, bool bStackTrace, bool bAsync)
	{
		SFLogUtil.Warn(GetType(), MakeLogMessage(sMessage), ex, bStackTrace, bAsync);
	}

	public void LogError(string sMessage)
	{
		LogError(sMessage, null);
	}

	public void LogError(string sMessage, Exception ex)
	{
		LogError(sMessage, ex, bStackTrace: false);
	}

	public void LogError(string sMessage, Exception ex, bool bStackTrace)
	{
		LogError(sMessage, ex, bStackTrace, bAsync: true);
	}

	public void LogError(string sMessage, Exception ex, bool bStackTrace, bool bAsync)
	{
		SFLogUtil.Error(GetType(), MakeLogMessage(sMessage), ex, bStackTrace, bAsync);
	}

	public void LogFatal(string sMessage)
	{
		LogFatal(sMessage, null);
	}

	public void LogFatal(string sMessage, Exception ex)
	{
		LogFatal(sMessage, ex, bStackTrace: false);
	}

	public void LogFatal(string sMessage, Exception ex, bool bStackTrace)
	{
		LogFatal(sMessage, ex, bStackTrace, bAsync: true);
	}

	public void LogFatal(string sMessage, Exception ex, bool bStackTrace, bool bAsync)
	{
		SFLogUtil.Fatal(GetType(), MakeLogMessage(sMessage), ex, bStackTrace, bAsync);
	}

	public string GetLogFooter()
	{
		StringBuilder stringBuilder = new StringBuilder();
		WriteLogFooter(stringBuilder);
		return stringBuilder.ToString();
	}

	protected virtual void WriteLogFooter(StringBuilder sb)
	{
		if (sb.Length > 0)
		{
			sb.Append(Environment.NewLine);
		}
		sb.Append("#F ");
		sb.Append("PeerId : " + m_sender.id);
	}

	public string MakeLogMessage(string sMessage)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SFStringUtil.NullToNullLiteral(sMessage));
		string logFooter = GetLogFooter();
		if (!string.IsNullOrEmpty(logFooter))
		{
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(logFooter);
		}
		return stringBuilder.ToString();
	}
}
