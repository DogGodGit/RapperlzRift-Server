using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildContent : IPickEntry
{
	public const int kId_GuildMission = 1;

	public const int kId_GuildHunting = 2;

	public const int kId_GuildAltar = 3;

	public const int kId_GuildFoodWarehouse = 4;

	public const int kId_GuildFarmQuest = 5;

	public const int kId_GuildHuntingDonation = 6;

	public const int kId_GuildSupplySupportQuest = 7;

	private int m_nId;

	private int m_nAchievementPoint;

	private bool m_bIsDailyObjective;

	public int id => m_nId;

	public int achivementPoint => m_nAchievementPoint;

	public bool isDailyObjective => m_bIsDailyObjective;

	public int point => 50;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["guildContentId"]);
		if (m_nId < 0)
		{
			SFLogUtil.Warn(GetType(), "길드컨텐츠ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nAchievementPoint = Convert.ToInt32(dr["achievementPoint"]);
		if (m_nAchievementPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "달성포인트가 유효하지 않습니다.");
		}
		m_bIsDailyObjective = Convert.ToBoolean(dr["isDailyObjective"]);
	}
}
