using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using Photon.SocketServer;

namespace ServerFramework;

public static class SFUtil
{
	public static OperationRequest CreateOperationRequest(short snCommandName, long lnPacketId, byte[] body)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[0] = snCommandName;
		dictionary[1] = lnPacketId;
		dictionary[2] = body;
		return new OperationRequest(0, (Dictionary<byte, object>)(object)dictionary);
	}

	public static OperationResponse CreateOperationResponse(short snResult, string sErrorMessage, short snCommandName, long lnPacketId, byte[] body)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[0] = snCommandName;
		dictionary[1] = lnPacketId;
		dictionary[2] = body;
		OperationResponse operationResponse = new OperationResponse();
		operationResponse.OperationCode = 0;
		operationResponse.ReturnCode = snResult;
		operationResponse.DebugMessage = sErrorMessage;
		operationResponse.Parameters = (Dictionary<byte, object>)(object)dictionary;
		return operationResponse;
	}

	public static EventData CreateEventData(short snEventName, byte[] body)
	{
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary[0] = snEventName;
		dictionary[1] = body;
		return new EventData(0, (Dictionary<byte, object>)(object)dictionary);
	}

	public static string ToString(Exception ex)
	{
		if (ex == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("{0}: {1}", ex.GetType().FullName, ex.Message);
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.AppendFormat("# StackTrace");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append(ex.StackTrace);
		if (ex.InnerException != null)
		{
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(ToString(ex.InnerException));
		}
		return stringBuilder.ToString();
	}

	public static string TraceSqlCommand(SqlCommand sc)
	{
		if (sc == null)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("# CommandText : {0}", sc.CommandText);
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.AppendFormat("# Parameters : {0}", sc.Parameters.Count);
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("{");
		foreach (SqlParameter parameter in sc.Parameters)
		{
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.AppendFormat("  {0} : {1}", parameter.ParameterName, parameter.Value);
		}
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}
	public static void SetLanguage(CultureInfo ciLang)
	{
		Resource.Exception.Culture = ciLang;
	}
}
