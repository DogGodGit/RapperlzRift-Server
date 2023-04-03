using System;
using System.Security.Cryptography;
using System.Text;
using LitJson;

namespace GameServer;

public class UserAccessToken
{
	public const string kPN_UserId = "userId";

	public const string kPN_AccessSecret = "accessSecret";

	public const string kPN_CheckCode = "checkCode";

	private Guid m_userId = Guid.Empty;

	private string m_sAccessSecret;

	private string m_sCheckCode;

	public Guid userId => m_userId;

	public string accessSecret => m_sAccessSecret;

	public string checkCode => m_sCheckCode;

	public bool isValid => m_sCheckCode == GetCheckCode(m_userId, m_sAccessSecret);

	public UserAccessToken(Guid userId, string sAccessSecret, string sCheckCode)
	{
		m_userId = userId;
		m_sAccessSecret = sAccessSecret;
		m_sCheckCode = sCheckCode;
	}

	public JsonData ToJson()
	{
		JsonData jo = LitJsonUtil.CreateObject();
		jo["userId"] = m_userId.ToString();
		jo["accessSecret"] = m_sAccessSecret;
		jo["checkCode"] = m_sCheckCode;
		return jo;
	}

	public override string ToString()
	{
		return ToJson().ToJson();
	}

	public static string GetCheckCode(Guid userId, string sAccessSecret)
	{
		string sHashKey = AppConfigUtil.hashKey;
		MD5 encoder = MD5.Create();
		string sText = $"{sHashKey}:{userId}:{sAccessSecret}";
		byte[] data = encoder.ComputeHash(Encoding.UTF8.GetBytes(sText));
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < data.Length; i++)
		{
			sb.Append(data[i].ToString("x2"));
		}
		return sb.ToString();
	}

	public static UserAccessToken Parse(string sToken)
	{
		JsonData joToken = JsonMapper.ToObject(sToken);
		Guid userId = Guid.Parse(LitJsonUtil.GetStringProperty(joToken, "userId"));
		string sAccessSecret = LitJsonUtil.GetStringProperty(joToken, "accessSecret");
		string sCheckCode = LitJsonUtil.GetStringProperty(joToken, "checkCode");
		return new UserAccessToken(userId, sAccessSecret, sCheckCode);
	}

	public static bool TryParse(string sToken, out UserAccessToken token)
	{
		token = null;
		if (string.IsNullOrEmpty(sToken))
		{
			return false;
		}
		try
		{
			token = Parse(sToken);
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}
}
