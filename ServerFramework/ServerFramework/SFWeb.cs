using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ServerFramework;

public static class SFWeb
{
	public static string GetResponse(string sUrl, string sPostData)
	{
		Stream stream = null;
		StreamReader streamReader = null;
		string text = null;
		try
		{
			string text2 = (string.IsNullOrEmpty(sPostData) ? "GET" : "POST");
			WebRequest webRequest = WebRequest.Create(sUrl);
			webRequest.Method = text2;
			if (text2 == "POST")
			{
				byte[] bytes = Encoding.UTF8.GetBytes(sPostData);
				webRequest.ContentType = "application/x-www-form-urlencoded";
				webRequest.ContentLength = bytes.Length;
				stream = webRequest.GetRequestStream();
				stream.Write(bytes, 0, bytes.Length);
				stream.Close();
				stream = null;
			}
			streamReader = new StreamReader(webRequest.GetResponse().GetResponseStream(), Encoding.UTF8);
			text = streamReader.ReadToEnd();
			streamReader.Close();
			streamReader = null;
			return text;
		}
		finally
		{
			stream?.Close();
			streamReader?.Close();
		}
	}

	public static string GetResponse(string sUrl, IEnumerable<KeyValuePair<string, string>> postParams)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (postParams != null)
		{
			foreach (KeyValuePair<string, string> postParam in postParams)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("&");
				}
				stringBuilder.AppendFormat("{0}={1}", postParam.Key, postParam.Value);
			}
		}
		return GetResponse(sUrl, stringBuilder.ToString());
	}
}
