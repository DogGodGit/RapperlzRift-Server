using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildMemberGrade
{
	public const int kType_Master = 1;

	public const int kType_ViceMaster = 2;

	public const int kType_Lord = 3;

	public const int kType_Normal = 4;

	public const int kCount = 4;

	private int m_nId;

	private bool m_bInvitationEnabled;

	private bool m_bApplicationAccecptanceEnabled;

	private bool m_bFoodWarehouseRewardCollectionEnabled;

	private bool m_bSuppleySupportQuestEnabled;

	private bool m_bBuildingLevelUpEnabled;

	private bool m_bGuildCallEnabled;

	private bool m_bWeeklyObjectiveSettingEnabled;

	private bool m_bGuildBlessingBuffEnabled;

	public int id => m_nId;

	public bool invitationEnabled => m_bInvitationEnabled;

	public bool applicationAccecptanceEnabled => m_bApplicationAccecptanceEnabled;

	public bool foodWarehouseRewardCollectionEnabled => m_bFoodWarehouseRewardCollectionEnabled;

	public bool suppleySupportQuestEnabled => m_bSuppleySupportQuestEnabled;

	public bool buildingLevelUpEnabled => m_bBuildingLevelUpEnabled;

	public bool guildCallEnabled => m_bGuildCallEnabled;

	public bool weeklyObjectiveSettingEnabled => m_bWeeklyObjectiveSettingEnabled;

	public bool guildBlessingBuffEnabled => m_bGuildBlessingBuffEnabled;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["memberGrade"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "멤버등급이 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_bInvitationEnabled = Convert.ToBoolean(dr["invitationEnabled"]);
		m_bApplicationAccecptanceEnabled = Convert.ToBoolean(dr["applicationAcceptanceEnabled"]);
		m_bFoodWarehouseRewardCollectionEnabled = Convert.ToBoolean(dr["guildFoodWarehouseRewardCollectionEnabled"]);
		m_bSuppleySupportQuestEnabled = Convert.ToBoolean(dr["guildSupplySupportQuestEnabled"]);
		m_bBuildingLevelUpEnabled = Convert.ToBoolean(dr["guildBuildingLevelUpEnabled"]);
		m_bGuildCallEnabled = Convert.ToBoolean(dr["guildCallEnabled"]);
		m_bWeeklyObjectiveSettingEnabled = Convert.ToBoolean(dr["weeklyObjectiveSettingEnabled"]);
		m_bGuildBlessingBuffEnabled = Convert.ToBoolean(dr["guildBlessingBuffEnabled"]);
	}
}
