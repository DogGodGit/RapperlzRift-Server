using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class SkillLevelUpTotallyCommandHandler : InGameCommandHandler<SkillLevelUpTotallyCommandBody, SkillLevelUpTotallyResponseBody>
{
	public class HeroSkillLevelUpLog
	{
		public int oldLevel;

		public int level;

		public long usedGold;

		public int materialItemId;

		public int materialItemOwnCount;

		public int materialItemUnOwnCount;

		public HeroSkillLevelUpLog(int nOldLevel, int nLevel, long lnUsedGold, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount)
		{
			oldLevel = nOldLevel;
			level = nLevel;
			usedGold = lnUsedGold;
			materialItemId = nMaterialItemId;
			materialItemOwnCount = nMaterialItemOwnCount;
			materialItemUnOwnCount = nMaterialItemUnOwnCount;
		}
	}

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private List<HeroSkillLevelUpLog> m_heroSkillLevelUpLogs = new List<HeroSkillLevelUpLog>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nSkillId = m_body.skillId;
		if (nSkillId <= 0)
		{
			throw new CommandHandleException(1, "스킬 ID가 유효하지 않습니다. nSkillId = " + nSkillId);
		}
		HeroSkill heroSkill = m_myHero.GetSkill(nSkillId);
		if (heroSkill == null)
		{
			throw new CommandHandleException(1, "영웅 스킬이 존재하지 않습니다. nSkillId = " + nSkillId);
		}
		if (!heroSkill.isOpened)
		{
			throw new CommandHandleException(1, "사용할 수 없는 스킬입니다.");
		}
		int nOldLevel = heroSkill.level;
		while (!heroSkill.isMaxLevel)
		{
			JobSkillMaster skillMaster = heroSkill.skill.skillMaster;
			JobSkillLevelMaster currentSkillLevelMaster = skillMaster.GetLevel(heroSkill.level);
			if (m_myHero.level < currentSkillLevelMaster.nextLevelUpRequiredHeroLevel)
			{
				break;
			}
			long lnNextLevelUpGold = currentSkillLevelMaster.nextLevelUpGold;
			int nNextLevelUpItemId = currentSkillLevelMaster.nextLevelUpItemId;
			int nNextLevelUpItemCount = currentSkillLevelMaster.nextLevelUpItemCount;
			if (m_myHero.gold < lnNextLevelUpGold || (nNextLevelUpItemId > 0 && m_myHero.GetItemCount(nNextLevelUpItemId) < nNextLevelUpItemCount))
			{
				break;
			}
			int nUsedOwnCount = 0;
			int nUsedUnOwnCount = 0;
			if (nNextLevelUpItemId > 0)
			{
				m_myHero.UseItem(nNextLevelUpItemId, bFisetUseOwn: true, nNextLevelUpItemCount, m_changedInventorySlots, out nUsedOwnCount, out nUsedUnOwnCount);
			}
			m_myHero.UseGold(lnNextLevelUpGold);
			heroSkill.level++;
			m_heroSkillLevelUpLogs.Add(new HeroSkillLevelUpLog(currentSkillLevelMaster.level, heroSkill.level, lnNextLevelUpGold, nNextLevelUpItemId, nUsedOwnCount, nUsedUnOwnCount));
		}
		if (nOldLevel != heroSkill.level)
		{
			m_myHero.RefreshBattlePower();
			SaveToDB(heroSkill);
		}
		SkillLevelUpTotallyResponseBody resBody = new SkillLevelUpTotallyResponseBody();
		resBody.skillLevel = heroSkill.level;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.gold = m_myHero.gold;
		SendResponseOK(resBody);
	}

	private void SaveToDB(HeroSkill heroSkill)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Gold(m_myHero.id, m_myHero.gold));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroSkill_Level(heroSkill.hero.id, heroSkill.skillId, heroSkill.level));
		dbWork.Schedule();
		SaveToDB_AddSkillLevelUpLog(heroSkill);
	}

	private void SaveToDB_AddSkillLevelUpLog(HeroSkill heroSkill)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			foreach (HeroSkillLevelUpLog log in m_heroSkillLevelUpLogs)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroSkillLevelUpLog(Guid.NewGuid(), heroSkill.hero.id, heroSkill.skillId, log.oldLevel, log.level, log.usedGold, log.materialItemId, log.materialItemOwnCount, log.materialItemUnOwnCount, m_currentTime));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
