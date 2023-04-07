using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class MainGearDisassembleCommandHandler : InGameCommandHandler<MainGearDisassembleCommandBody, MainGearDisassembleResponseBody>
{
	private List<HeroMainGear> m_removeHeroMainGears = new List<HeroMainGear>();

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private List<HeroMainGearDisassembleLog> m_disassembleLogs = new List<HeroMainGearDisassembleLog>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		if (m_body.targetHeroMainGearIds == null)
		{
			throw new CommandHandleException(1, "분해할 메인장비ID 항목이 존재하지 않습니다.");
		}
		if (m_body.targetHeroMainGearIds.Length > Resource.instance.mainGearDisassembleSlotCount)
		{
			throw new CommandHandleException(1, "분해할 메인장비 수가 최대 분해가능한 갯수를 넘어갑니다.");
		}
		Guid[] targetHeroMainGearIds = (Guid[])(object)m_body.targetHeroMainGearIds;
		foreach (Guid targetMainGearId in targetHeroMainGearIds)
		{
			HeroMainGear heroMainGear = m_myHero.GetMainGear(targetMainGearId);
			if (heroMainGear == null)
			{
				throw new CommandHandleException(1, "메인장비가 존재하지 않습니다. targetMainGearId = " + targetMainGearId);
			}
			if (heroMainGear.inventorySlot == null)
			{
				throw new CommandHandleException(1, "인벤토리에 존재하지 않습니다. targetMainGearId = " + targetMainGearId);
			}
			if (m_removeHeroMainGears.Contains(heroMainGear))
			{
				throw new CommandHandleException(1, "중복되는 메인장비입니다. targetMainGearId = " + targetMainGearId);
			}
			m_removeHeroMainGears.Add(heroMainGear);
		}
		ResultItemCollection resultItemCollection = new ResultItemCollection();
		foreach (HeroMainGear heroMainGear2 in m_removeHeroMainGears)
		{
			MainGearTier mainGearTier = heroMainGear2.gear.tier;
			int nMainGearGrade = heroMainGear2.gear.grade.id;
			MainGearDisassembleResultCountPool resultCountPool = mainGearTier.GetDisassembleResultCountPool(nMainGearGrade);
			MainGearDisassembleResultPool resultPool = mainGearTier.GetDisassembleResultPool(nMainGearGrade);
			int nPickCount = resultCountPool.SelectCount();
			HeroMainGearDisassembleLog detailLog = new HeroMainGearDisassembleLog(heroMainGear2);
			foreach (MainGearDisassembleResultPoolEntry disassembleResult in resultPool.SelectEntries(nPickCount))
			{
				detailLog.AddDetailLog(disassembleResult.item.id, disassembleResult.itemCount, disassembleResult.itemOwned);
				resultItemCollection.AddResultItemCount(disassembleResult.item, disassembleResult.itemOwned, disassembleResult.itemCount);
			}
			m_disassembleLogs.Add(detailLog);
		}
		foreach (HeroMainGear removeHeroMainGear in m_removeHeroMainGears)
		{
			InventorySlot inventorySlot = removeHeroMainGear.inventorySlot;
			m_myHero.RemoveMainGear(removeHeroMainGear.id);
			inventorySlot.Clear();
			m_changedInventorySlots.Add(inventorySlot);
		}
		foreach (ResultItem resultItem in resultItemCollection.resultItems)
		{
			int nRemainingCount = m_myHero.AddItem(resultItem.item, resultItem.owned, resultItem.count, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_REWARD_N_102", "MAIL_REWARD_D_102", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(resultItem.item, nRemainingCount, resultItem.owned));
			}
		}
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB();
		SaveToDB_AddHeroMainGearDisassembleLog();
		MainGearDisassembleResponseBody resBody = new MainGearDisassembleResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (HeroMainGear mainGear in m_removeHeroMainGears)
		{
			dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroMainGear(mainGear.id, m_currentTime));
		}
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_ApplyChangedInventorySlots(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToDB_AddHeroMainGearDisassembleLog()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			foreach (HeroMainGearDisassembleLog disassembleLog in m_disassembleLogs)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroMainGearDisassembleLog(disassembleLog.id, disassembleLog.heroMainGear.hero.id, disassembleLog.heroMainGear.id, m_currentTime));
				foreach (HeroMainGearDisassembleDetailLog detailLog in disassembleLog.detailLogs)
				{
					logWork.AddSqlCommand(GameLogDac.CSC_AddHeroMainGearDisassembleDetailLog(disassembleLog.id, detailLog.no, detailLog.itemId, detailLog.itemCount, detailLog.itemOwned));
				}
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
