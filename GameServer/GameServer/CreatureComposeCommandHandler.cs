using System;
using System.Collections.Generic;
using System.Linq;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CreatureComposeCommandHandler : InGameCommandHandler<CreatureComposeCommandBody, CreatureComposeResponseBody>
{
	private class CompositionSkillLog
	{
		public int index;

		public int oldSkillId;

		public int oldSkillGrade;

		public int skillId;

		public int skillGrade;

		public bool isProtected;

		public CompositionSkillLog(int nIndex, int nOldSkillId, int nOldSkillGrade, int nSkillId, int nSkillGrade, bool bIsProtected)
		{
			index = nIndex;
			oldSkillId = nOldSkillId;
			oldSkillGrade = nOldSkillGrade;
			skillId = nSkillId;
			skillGrade = nSkillGrade;
			isProtected = bIsProtected;
		}
	}

	public const short kResult_NotOpendSkillSlot = 101;

	public const short kReuslt_EmptySkillSlot = 102;

	public const short kResult_NotEnoughItem = 103;

	public const short kResult_NotEnoughInventory = 104;

	private HeroCreature m_mainHeroCreature;

	private HeroCreature m_materialHeroCreature;

	private int m_nSkillProtectionItemId;

	private int m_nUsedSkillProtectionItemOwnCount;

	private int m_nUsedSkillProtectionItemUnOwnCount;

	private HashSet<HeroCreatureSkill> m_heroCreatureSkills = new HashSet<HeroCreatureSkill>();

	private ResultItemCollection m_resultItemCollection;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private List<CompositionSkillLog> m_skillLogs = new List<CompositionSkillLog>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid mainInstanceId = (Guid)m_body.mainInstanceId;
		Guid materialInstanceId = (Guid)m_body.materialInstanceId;
		int[] protectedIndices = m_body.protectedIndices;
		if (mainInstanceId == Guid.Empty)
		{
			throw new CommandHandleException(1, "메인영웅크리처ID가 유효하지 않습니다. mainInstanceId = " + mainInstanceId);
		}
		if (materialInstanceId == Guid.Empty)
		{
			throw new CommandHandleException(1, "재료영웅크리처ID가 유효하지 않습니다. materialInstanceId = " + materialInstanceId);
		}
		if (mainInstanceId == materialInstanceId)
		{
			throw new CommandHandleException(1, "메인영웅크리처와 재료영웅크리처가 동일합니다.");
		}
		m_mainHeroCreature = m_myHero.GetCreature(mainInstanceId);
		if (m_mainHeroCreature == null)
		{
			throw new CommandHandleException(1, "메인영웅크리처가 존재하지 않습니다. mainInstanceId = " + mainInstanceId);
		}
		m_materialHeroCreature = m_myHero.GetCreature(materialInstanceId);
		if (m_materialHeroCreature == null)
		{
			throw new CommandHandleException(1, "재료영웅크리처가 존재하지 않습니다. materialInstanceId = " + materialInstanceId);
		}
		if (m_materialHeroCreature.participated)
		{
			throw new CommandHandleException(1, "재료영웅크리처가 이미 출전중입니다. materialInstanceId = " + materialInstanceId);
		}
		if (m_materialHeroCreature.cheered)
		{
			throw new CommandHandleException(1, "재료영웅크리처가 이미 응원중입니다. materialInstanceId = " + materialInstanceId);
		}
		HashSet<int> protectedSkillIndices = new HashSet<int>();
		if (protectedIndices != null)
		{
			int[] array = protectedIndices;
			foreach (int protectedIndex in array)
			{
				if (protectedSkillIndices.Contains(protectedIndex))
				{
					throw new CommandHandleException(1, "중복된 슬롯인덱스입니다. protectedIndex = " + protectedIndex);
				}
				HeroCreatureSkill heroCreatureSkill = m_mainHeroCreature.GetSkill(protectedIndex);
				if (heroCreatureSkill == null)
				{
					throw new CommandHandleException(1, "존재하지 않는 스킬슬롯입니다. protectedIndex = " + protectedIndex);
				}
				if (!heroCreatureSkill.isOpened)
				{
					throw new CommandHandleException(101, "개방되지 않은 스킬슬롯입니다. protectedIndex = " + protectedIndex);
				}
				if (heroCreatureSkill.skillAttr == null)
				{
					throw new CommandHandleException(102, "빈 스킬슬롯입니다. protectedIndex = " + protectedIndex);
				}
				protectedSkillIndices.Add(protectedIndex);
			}
		}
		int nProtectedSkillIndicesCount = protectedSkillIndices.Count;
		CreatureSkillSlotProtection slotProtection = Resource.instance.GetCreatureSkillSlotProtection(nProtectedSkillIndicesCount);
		if (nProtectedSkillIndicesCount > 0)
		{
			if (slotProtection == null)
			{
				throw new CommandHandleException(1, "존재하지 않는 보호개수입니다.");
			}
			if (m_mainHeroCreature.skillCount < slotProtection.requiredSkillCount)
			{
				throw new CommandHandleException(1, "보호개수가 메인영웅크리처의 스킬 수에 유효하지 않습니다.");
			}
		}
		int nRequiredSkillCount = 0;
		int nRequiredItemCount = 0;
		if (slotProtection != null)
		{
			nRequiredSkillCount = slotProtection.requiredSkillCount;
			nRequiredItemCount = slotProtection.requiredItemCount;
			m_nSkillProtectionItemId = Resource.instance.creatureCompositionSkillProtectionItemId;
			if (m_myHero.GetItemCount(m_nSkillProtectionItemId) < nRequiredItemCount)
			{
				throw new CommandHandleException(103, "아이템이 부족합니다.");
			}
		}
		long lnTotalMaterialCreatueExp = m_materialHeroCreature.GetAccumulationExp();
		lnTotalMaterialCreatueExp = (long)((float)(lnTotalMaterialCreatueExp * Resource.instance.creatureReleaseExpRetrievalRate) / 10000f);
		m_resultItemCollection = new ResultItemCollection();
		Item[] creatureFeeds = Resource.instance.creatureFeeds;
		foreach (Item creatureFeed in creatureFeeds)
		{
			int nItemCount = (int)(lnTotalMaterialCreatueExp / creatureFeed.value1);
			lnTotalMaterialCreatueExp -= creatureFeed.value1 * nItemCount;
			if (nItemCount > 0)
			{
				m_resultItemCollection.AddResultItemCount(creatureFeed, bOwned: true, nItemCount);
			}
		}
		if (!m_myHero.IsAvailableInventory(m_resultItemCollection))
		{
			throw new CommandHandleException(104, "인벤토리가 부족합니다.");
		}
		int nOpendMainHeroCreatureSkillSlotCount = Resource.instance.creatureSkillSlotBaseOpenCount + m_mainHeroCreature.additionalOpenSkillSlotCount;
		HeroCreatureSkill[] skills = m_materialHeroCreature.skills;
		foreach (HeroCreatureSkill materialHeroCreatureSkill in skills)
		{
			if (materialHeroCreatureSkill.skillAttr != null)
			{
				int nMaxIndex = m_mainHeroCreature.skillCount;
				if (nMaxIndex < nOpendMainHeroCreatureSkillSlotCount)
				{
					nMaxIndex++;
				}
				int nSelectedIndex = SFRandom.Next(0, nMaxIndex);
				HeroCreatureSkill targetHeroCreatureSkill = m_mainHeroCreature.GetSkill(nSelectedIndex);
				CreatureSkillAttr creatureSkillAttr = targetHeroCreatureSkill.skillAttr;
				int nOldSkillId = 0;
				int nOldSkillGrade = 0;
				if (creatureSkillAttr != null)
				{
					nOldSkillId = creatureSkillAttr.skill.id;
					nOldSkillGrade = creatureSkillAttr.grade.grade;
				}
				bool bProtected = protectedIndices?.Contains(nSelectedIndex) ?? false;
				if (!bProtected)
				{
					targetHeroCreatureSkill.skillAttr = materialHeroCreatureSkill.skillAttr;
				}
				m_heroCreatureSkills.Add(targetHeroCreatureSkill);
				m_skillLogs.Add(new CompositionSkillLog(nSelectedIndex, nOldSkillId, nOldSkillGrade, targetHeroCreatureSkill.skillAttr.skill.id, targetHeroCreatureSkill.skillAttr.grade.grade, bProtected));
			}
		}
		m_myHero.RemoveCreature(m_materialHeroCreature.instanceId);
		if (m_nSkillProtectionItemId > 0)
		{
			m_myHero.UseItem(m_nSkillProtectionItemId, bFisetUseOwn: true, nRequiredSkillCount, m_changedInventorySlots, out m_nUsedSkillProtectionItemOwnCount, out m_nUsedSkillProtectionItemUnOwnCount);
		}
		foreach (ResultItem result in m_resultItemCollection.resultItems)
		{
			m_myHero.AddItem(result.item, result.owned, result.count, m_changedInventorySlots);
		}
		if (m_mainHeroCreature.participated || m_mainHeroCreature.cheered)
		{
			m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
		SaveToDB();
		SaveToLogDB();
		CreatureComposeResponseBody resBody = new CreatureComposeResponseBody();
		resBody.mainHeroCreatureSkills = m_mainHeroCreature.GetPDHeroCreatureSkills().ToArray();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_DeleteHeroCreature(m_materialHeroCreature));
		foreach (HeroCreatureSkill skill in m_heroCreatureSkills)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroCreatureSkill(skill.creature.instanceId, skill.slotIndex, skill.skillAttr.skill.id, skill.skillAttr.grade.grade));
		}
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_ApplyChangedInventorySlots(slot));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCompositionLog(logId, m_mainHeroCreature.instanceId, m_myHero.id, m_materialHeroCreature.instanceId, m_materialHeroCreature.level, m_materialHeroCreature.exp, m_materialHeroCreature.injectionLevel, m_materialHeroCreature.injectionExp, m_nSkillProtectionItemId, m_nUsedSkillProtectionItemOwnCount, m_nUsedSkillProtectionItemUnOwnCount, m_currentTime));
			foreach (CompositionSkillLog skillLog in m_skillLogs)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCompositionSkillLog(Guid.NewGuid(), logId, skillLog.index, skillLog.oldSkillId, skillLog.oldSkillGrade, skillLog.skillId, skillLog.skillGrade, skillLog.isProtected));
			}
			foreach (ResultItem result in m_resultItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCompositionDetailLog(Guid.NewGuid(), logId, result.item.id, result.owned, result.count));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
