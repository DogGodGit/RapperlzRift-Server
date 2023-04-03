using System;
using System.Data;
using ClientCommon;

namespace GameServer;

public class GuildRanking
{
	private int m_nRanking;

	private int m_nNationId;

	private Guid m_guildId = Guid.Empty;

	private string m_sGuildName;

	private long m_lnMight;

	private Guid m_guildMasterId = Guid.Empty;

	private string m_sGuildMasterName;

	public int ranking => m_nRanking;

	public int nationId => m_nNationId;

	public Guid guildId => m_guildId;

	public string guildName => m_sGuildName;

	public long might => m_lnMight;

	public string guildMasterName => m_sGuildMasterName;

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRanking = Convert.ToInt32(dr["ranking"]);
		m_nNationId = Convert.ToInt32(dr["nationId"]);
		m_guildId = (Guid)dr["guildId"];
		m_sGuildName = Convert.ToString(dr["guildName"]);
		m_lnMight = Convert.ToInt64(dr["might"]);
		m_guildMasterId = (Guid)dr["guildMasterId"];
		m_sGuildMasterName = Convert.ToString(dr["guildMasterName"]);
	}

	public PDGuildRanking ToPDGuildRanking()
	{
		PDGuildRanking inst = new PDGuildRanking();
		inst.ranking = m_nRanking;
		inst.nationId = m_nNationId;
		inst.guildId = (Guid)m_guildId;
		inst.guildName = m_sGuildName;
		inst.might = m_lnMight;
		inst.guildMasterId = (Guid)m_guildMasterId;
		inst.guildMasterName = m_sGuildMasterName;
		return inst;
	}
}
