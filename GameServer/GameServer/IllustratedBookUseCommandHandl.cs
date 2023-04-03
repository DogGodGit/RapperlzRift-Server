using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class IllustratedBookUseCommandHandler : InGameCommandHandler<IllustratedBookUseCommandBody, IllustratedBookUseResponseBody>
{
	public const short kResult_NotEnoughItem = 101;

	private InventorySlot m_targetInventorySlot;

	private IllustratedBook m_illustratedBook;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nSlotIndex = m_body.slotIndex;
		if (nSlotIndex < 0)
		{
			throw new CommandHandleException(1, "슬롯 인덱스가 유효하지 않습니다. nSlotIndex = " + nSlotIndex);
		}
		m_targetInventorySlot = m_myHero.GetInventorySlot(nSlotIndex);
		if (m_targetInventorySlot == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
		}
		if (m_targetInventorySlot.isEmpty)
		{
			throw new CommandHandleException(1, "빈 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
		}
		if (!(m_targetInventorySlot.obj is ItemInventoryObject obj))
		{
			throw new CommandHandleException(1, "해당 인벤토리슬롯의 오브젝트가 아이템타입이 아닙니다.");
		}
		Item item = obj.item;
		_ = obj.owned;
		if (item.type.id != 26)
		{
			throw new CommandHandleException(1, "해당 인벤토리의 아이템이 도감이 아닙니다.");
		}
		if (!item.IsUseableHeroLevel(m_myHero.level))
		{
			throw new CommandHandleException(1, "사용할 수 있는 영웅레벨이 아닙니다.");
		}
		m_myHero.UseItem(nSlotIndex, 1);
		int nIllustratedBookId = item.value1;
		int nActivationIllustratedBookId = 0;
		m_illustratedBook = Resource.instance.GetIllustratedBook(nIllustratedBookId);
		if (m_illustratedBook == null)
		{
			throw new CommandHandleException(1, "도감이 존재하지 않습니다.");
		}
		if (!m_myHero.ContainsIllustratedBook(nIllustratedBookId))
		{
			HeroIllustratedBook heroIllustratedBook = new HeroIllustratedBook(m_myHero);
			heroIllustratedBook.Init(m_illustratedBook);
			m_myHero.AddIllustratedBook(heroIllustratedBook);
			m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
			nActivationIllustratedBookId = nIllustratedBookId;
		}
		m_myHero.AddExplorationPoint(m_illustratedBook.explorationPoint);
		SaveToDB(nActivationIllustratedBookId);
		SaveToDB_Log();
		IllustratedBookUseResponseBody resBody = new IllustratedBookUseResponseBody();
		resBody.activationIllustratedBookId = nActivationIllustratedBookId;
		resBody.explorationPoint = m_myHero.explorationPoint;
		resBody.changedInventorySlot = m_targetInventorySlot.ToPDInventorySlot();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	public void SaveToDB(int nActivationIllustratedBookId)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		if (nActivationIllustratedBookId > 0)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroIllustratedBook(m_myHero.id, nActivationIllustratedBookId));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ExplorationPoint(m_myHero.id, m_myHero.explorationPoint));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_targetInventorySlot));
		dbWork.Schedule();
	}

	public void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroIllustratedBookActivationLog(Guid.NewGuid(), m_myHero.id, m_illustratedBook.id, m_myHero.explorationPoint, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
