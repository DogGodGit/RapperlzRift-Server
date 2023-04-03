using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ArtifactLevelUpCommandHandler : InGameCommandHandler<ArtifactLevelUpCommandBody, ArtifactLevelUpResponseBody>
{
	private class DetailLog
	{
		public Guid heroMainGearId;

		public int exp;

		public DetailLog(Guid heroMainGearId, int nExp)
		{
			this.heroMainGearId = heroMainGearId;
			exp = nExp;
		}
	}

	public const short kResult_ArtifactNotOpened = 101;

	public const short kResult_ArtifactMaxLevel = 102;

	public const short kResult_MainGearNotExist = 103;

	public const short kResult_MainGearNotExistInInventory = 104;

	public const short kResult_MainGearDuplicated = 105;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private HashSet<HeroMainGear> m_mainGears = new HashSet<HeroMainGear>();

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private int m_nOldArtifactNo;

	private int m_nOldArtifactLevel;

	private int m_nOldArtifactExp;

	private List<DetailLog> m_detailLogs = new List<DetailLog>();

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid[] mainGearIds = (Guid[])(object)m_body.mainGears;
		if (mainGearIds == null || mainGearIds.Length == 0)
		{
			throw new CommandHandleException(1, "메인장비를 선택하지 않았습니다.");
		}
		m_currentTime = DateTimeUtil.currentTime;
		if (!m_myHero.isArtifactOpened)
		{
			throw new CommandHandleException(101, "아티팩트가 개방되지 않았습니다.");
		}
		if (m_myHero.isArtifactLast && m_myHero.isArtifactLevelMax)
		{
			throw new CommandHandleException(102, "아티팩트가 최대레벨입니다.");
		}
		int nMaterialExp = 0;
		Guid[] array = mainGearIds;
		foreach (Guid mainGearId in array)
		{
			HeroMainGear mainGear = m_myHero.GetMainGear(mainGearId);
			if (mainGear == null)
			{
				throw new CommandHandleException(103, "메인장비가 존재하지 않습니다. mainGearId = " + mainGearId);
			}
			if (mainGear.inventorySlot == null)
			{
				throw new CommandHandleException(104, "메인장비가 인벤토리에 존재하지 않습니다. mainGearId = " + mainGearId);
			}
			if (!m_mainGears.Add(mainGear))
			{
				throw new CommandHandleException(105, "메인장비가 중복됩니다. mainGearId = " + mainGearId);
			}
			ArtifactLevelUpMaterial material = Resource.instance.GetArtifactLevelUpMaterial(mainGear.gear.tier.id, mainGear.gear.grade.id);
			if (material == null)
			{
				throw new CommandHandleException(1, "아티팩트레벨업재료가 존재하지 않습니다. tier = " + mainGear.gear.tier.id + ", grade = " + mainGear.gear.grade.id);
			}
			nMaterialExp += material.exp;
			m_detailLogs.Add(new DetailLog(mainGearId, material.exp));
		}
		m_nOldArtifactNo = m_myHero.artifactNo;
		m_nOldArtifactLevel = m_myHero.artifactLevel;
		m_nOldArtifactExp = m_myHero.artifactExp;
		m_myHero.AddArtifactExp(nMaterialExp);
		foreach (HeroMainGear mainGear2 in m_mainGears)
		{
			InventorySlot inventorySlot = mainGear2.inventorySlot;
			m_myHero.RemoveMainGear(mainGear2.id);
			inventorySlot.Clear();
			m_changedInventorySlots.Add(inventorySlot);
		}
		SaveToDB();
		SaveToGameLogDB();
		ArtifactLevelUpResponseBody resBody = new ArtifactLevelUpResponseBody();
		resBody.artifactNo = m_myHero.artifactNo;
		resBody.artifactLevel = m_myHero.artifactLevel;
		resBody.artifactExp = m_myHero.artifactExp;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_ArtifactLevelUp(m_myHero));
		foreach (HeroMainGear mainGear in m_mainGears)
		{
			dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroMainGear(mainGear.id, m_currentTime));
		}
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_ApplyChangedInventorySlots(slot));
		}
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroArtifactLevelUpLog(logId, m_myHero.id, m_nOldArtifactNo, m_nOldArtifactLevel, m_nOldArtifactExp, m_myHero.artifactNo, m_myHero.artifactLevel, m_myHero.artifactExp, m_currentTime));
			foreach (DetailLog log in m_detailLogs)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroArtifactLevelUpDetailLog(Guid.NewGuid(), logId, log.heroMainGearId, log.exp));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
