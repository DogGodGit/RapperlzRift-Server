using System;
using System.Diagnostics;
using System.Text;
using ExitGames.Logging;
using Photon.SocketServer;

namespace ServerFramework;

public static class SFLogUtil
{
	private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

	public static void Debug(Type callerType, string sMessage)
	{
		Debug(callerType, sMessage, null);
	}

	public static void Debug(Type callerType, string sMessage, Exception ex)
	{
		Debug(callerType, sMessage, ex, bStackTrace: false);
	}

	public static void Debug(Type callerType, string sMessage, Exception ex, bool bStackTrace)
	{
		Debug(callerType, sMessage, ex, bStackTrace, bAsync: true);
	}

	public static void Debug(Type callerType, string sMessage, Exception ex, bool bStackTrace, bool bAsync)
	{
		string text = MakeMessage(callerType, sMessage, bStackTrace);
		if (bAsync)
		{
			((SFApplication)ApplicationBase.Instance).AddLogWork(new SFAction<object, Exception>((Action<object, Exception>)logger.Debug, (object)text, ex));
		}
		else
		{
			logger.Debug(text, (Exception)(object)ex);
		}
	}

	public static void Info(Type callerType, string sMessage)
	{
		Info(callerType, sMessage, null);
	}

	public static void Info(Type callerType, string sMessage, Exception ex)
	{
		Info(callerType, sMessage, ex, bStackTrace: false);
	}

	public static void Info(Type callerType, string sMessage, Exception ex, bool bStackTrace)
	{
		Info(callerType, sMessage, ex, bStackTrace, bAsync: true);
	}

	public static void Info(Type callerType, string sMessage, Exception ex, bool bStackTrace, bool bAsync)
	{
		string text = MakeMessage(callerType, sMessage, bStackTrace);
		if (bAsync)
		{
			((SFApplication)ApplicationBase.Instance).AddLogWork(new SFAction<object, Exception>((Action<object, Exception>)logger.Info, (object)text, ex));
		}
		else
		{
			logger.Info(text, (Exception)(object)ex);
		}
	}

	public static void Warn(Type callerType, string sMessage)
	{
		Warn(callerType, sMessage, null);
	}

	public static void Warn(Type callerType, string sMessage, Exception ex)
	{
		Warn(callerType, sMessage, ex, bStackTrace: false);
	}

	public static void Warn(Type callerType, string sMessage, Exception ex, bool bStackTrace)
	{
		Warn(callerType, sMessage, ex, bStackTrace, bAsync: true);
	}

	public static void Warn(Type callerType, string sMessage, Exception ex, bool bStackTrace, bool bAsync)
	{
		string text = MakeMessage(callerType, sMessage, bStackTrace);
		if (bAsync)
		{
			((SFApplication)ApplicationBase.Instance).AddLogWork(new SFAction<object, Exception>((Action<object, Exception>)logger.Warn, (object)text, ex));
		}
		else
		{
			logger.Warn(text, (Exception)(object)ex);
		}
	}

	public static void Error(Type callerType, string sMessage)
	{
		Error(callerType, sMessage, null);
	}

	public static void Error(Type callerType, string sMessage, Exception ex)
	{
		Error(callerType, sMessage, ex, bStackTrace: false);
	}

	public static void Error(Type callerType, string sMessage, Exception ex, bool bStackTrace)
	{
		Error(callerType, sMessage, ex, bStackTrace, bAsync: true);
	}

	public static void Error(Type callerType, string sMessage, Exception ex, bool bStackTrace, bool bAsync)
	{
		string text = MakeMessage(callerType, sMessage, bStackTrace);
		if (bAsync)
		{
			((SFApplication)ApplicationBase.Instance).AddLogWork(new SFAction<object, Exception>((Action<object, Exception>)logger.Error, (object)text, ex));
		}
		else
		{
			logger.Error(text, (Exception)(object)ex);
		}
	}

	public static void Fatal(Type callerType, string sMessage)
	{
		Fatal(callerType, sMessage, null);
	}

	public static void Fatal(Type callerType, string sMessage, Exception ex)
	{
		Fatal(callerType, sMessage, ex, bStackTrace: false);
	}

	public static void Fatal(Type callerType, string sMessage, Exception ex, bool bStackTrace)
	{
		Fatal(callerType, sMessage, ex, bStackTrace, bAsync: true);
	}

	public static void Fatal(Type callerType, string sMessage, Exception ex, bool bStackTrace, bool bAsync)
	{
		string text = MakeMessage(callerType, sMessage, bStackTrace);
		if (bAsync)
		{
			((SFApplication)ApplicationBase.Instance).AddLogWork(new SFAction<object, Exception>((Action<object, Exception>)logger.Fatal, (object)text, ex));
		}
		else
		{
			logger.Fatal(text, (Exception)(object)ex);
		}
	}

	private static string MakeMessage(Type callerType, string sMessage, bool bStackTrace)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(SFStringUtil.NullToNullLiteral(sMessage));
		if (bStackTrace)
		{
			StackTrace stackTrace = new StackTrace(fNeedFileInfo: true);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("# StackTrace");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(stackTrace.ToString());
		}
		return string.Format("[{0}] {1}", (callerType == null) ? "null" : callerType.FullName, stringBuilder.ToString());
	}
}
