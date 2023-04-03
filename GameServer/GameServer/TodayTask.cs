using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TodayTask
{
	public const int kId_ExpDungeon = 1;

	public const int kId_FearAltarDungeon = 2;

	public const int kId_DimensionRaidQuest = 3;

	public const int kId_MysteryBoxQuest = 4;

	public const int kId_SecretLetterQuest = 5;

	public const int kId_FishingQuest = 6;

	public const int kId_BountyHunterQuest = 7;

	public const int kId_ProofOfValor = 9;

	public const int kId_StoryDungeon = 10;

	public const int kId_AncientRelic = 11;

	public const int kId_FieldOfHonor = 12;

	public const int kId_TreatOfFarmQuest = 13;

	public const int kId_OsirisRoomDungeon = 16;

	public const int kId_HolyWarQuest = 18;

	public const int kId_SupplySupportQuest = 19;

	public const int kId_GuildMissionQuest = 20;

	public const int kId_GuildAltar = 21;

	public const int kId_GuildSupplySupportQuest = 22;

	public const int kId_GuildFarmQuest = 23;

	public const int kId_GuildFoodWareHouse = 24;

	public const int kId_DailyQuest = 26;

	public const int kId_WeeklyQuest = 27;

	public const int kId_WisdomTemple = 28;

	public const int kId_RuinsReclaim = 29;

	public const int kId_TrueHeroQuest = 30;

	public const int kId_FieldBoss = 31;

	public const int kId_InfiniteWar = 32;

	public const int kId_NationWar = 34;

	public const int kId_CreatureFarmQuest = 35;

	private int m_nId;

	private string m_sNameKey;

	private int m_nRank;

	private int m_nAchievementPoint;

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public int rank => m_nRank;

	public int achievementPoint => m_nAchievementPoint;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["taskId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "할일ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_nAchievementPoint = Convert.ToInt32(dr["achievementPoint"]);
		if (m_nAchievementPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "달성포인트가 유효하지 않습니다. m_nId = " + m_nId + ", m_nAchievementPoint = " + m_nAchievementPoint);
		}
	}
}
