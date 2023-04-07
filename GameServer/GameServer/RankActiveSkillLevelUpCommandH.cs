using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class RankActiveSkillLevelUpCommandHandler : InGameCommandHandler<RankActiveSkillLevelUpCommandBody, RankActiveSkillLevelUpResponseBody>
{
	public const short kResult_MaxLevel = 101;

	public const short kResult_NotEnoughGold = 102;

	public const short kReqult_NotEnoughItem = 103;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private int m_nTargetSkillId;

	private int m_nOldLevel;

	private int m_nLevel;

	private long m_lnNextLevelUpRequiredGold;

	private int m_nNextLevelUpRequiredItemId;

	private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

	private int m_nUsedOwnCount;

	private int m_nUsedUnOwnCount;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_nTargetSkillId = m_body.targetSkillId;
		HeroRankActiveSkill heroSkill = m_myHero.GetRankActiveSkill(m_nTargetSkillId);
		if (heroSkill == null)
		{
			throw new CommandHandleException(1, "목표스킬ID가 유효하지 않습니다. m_nTargetSkillId = " + m_nTargetSkillId);
		}
		if (heroSkill.isMaxLevel)
		{
			throw new CommandHandleException(101, "이미 스킬레벨이 최대치입니다.");
		}
		RankActiveSkillLevel heroSkillLevel = heroSkill.skillLevel;
		m_nOldLevel = heroSkill.level;
		m_lnNextLevelUpRequiredGold = heroSkillLevel.nextLevelUpRequiredGold;
		if (m_lnNextLevelUpRequiredGold > m_myHero.gold)
		{
			throw new CommandHandleException(102, "골드가 부족합니다.");
		}
		m_nNextLevelUpRequiredItemId = heroSkillLevel.nextLevelUpRequiredItemId;
		int nNextLevelUpRequiredItemCount = heroSkillLevel.nextLevelUpRequiredItemCount;
		if (m_nNextLevelUpRequiredItemId > 0 && m_myHero.GetItemCount(m_nNextLevelUpRequiredItemId) < nNextLevelUpRequiredItemCount)
		{
			throw new CommandHandleException(103, "아이템이 부족합니다.");
		}
		m_myHero.UseGold(m_lnNextLevelUpRequiredGold);
		if (m_nNextLevelUpRequiredItemId > 0)
		{
			m_myHero.UseItem(m_nNextLevelUpRequiredItemId, bFisetUseOwn: true, nNextLevelUpRequiredItemCount, m_changedInventorySlots, out m_nUsedOwnCount, out m_nUsedUnOwnCount);
		}
		heroSkill.level++;
		m_nLevel = heroSkill.level;
		SaveToDB();
		SaveToDB_Log();
		RankActiveSkillLevelUpResponseBody resBody = new RankActiveSkillLevelUpResponseBody();
		resBody.gold = m_myHero.gold;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroRankActiveSkill(m_myHero.id, m_nTargetSkillId, m_nLevel));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroRankActiveSkillLevelUpLog(Guid.NewGuid(), m_myHero.id, m_nTargetSkillId, m_nOldLevel, m_nLevel, m_lnNextLevelUpRequiredGold, m_nNextLevelUpRequiredItemId, m_nUsedOwnCount, m_nUsedUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}
}
