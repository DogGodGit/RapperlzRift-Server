using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class FearAltarHalidomElementalRewardReceiveCommandHandler : InGameCommandHandler<FearAltarHalidomElementalRewardReceiveCommandBody, FearAltarHalidomElementalRewardReceiveResponseBody>
{
	public const short kResult_NotElementalLevelIsFull = 101;

	public const short kResult_NotEnoughInventorySlot = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private PDItemBooty m_booty;

	private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

	private HeroFearAltarHalidomElementalReward m_heroReward;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nElementalId = m_body.elementalId;
		FearAltar fearAltar = Resource.instance.fearAltar;
		FearAltarHalidomElemental halidomElemental = fearAltar.GetHalidomElemental(nElementalId);
		if (halidomElemental == null)
		{
			throw new CommandHandleException(1, "원소ID가 유효하지 않습니다. nElementalId = " + nElementalId);
		}
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		m_myHero.RefreshFearAltarHalidom(currentDate);
		if (!base.hero.CheckFearAltarHalidomElementalLevelIsFull(nElementalId))
		{
			throw new CommandHandleException(101, "현재원소의 레벨을 모두 획득하지 않았습니다.");
		}
		m_myHero.RefreshFearAltarHalidomElementalReward(currentDate);
		if (base.hero.ContainsFearAltarHalidomElementalReward(nElementalId))
		{
			throw new CommandHandleException(1, "이미 해당원소의 보상을 받았습니다.");
		}
		ItemReward itemReward = halidomElemental.collectionItemRewrad;
		if (itemReward != null)
		{
			Item item = itemReward.item;
			bool bOwned = itemReward.owned;
			int nCount = itemReward.count;
			if (nCount > m_myHero.GetInventoryAvailableSpace(item, bOwned))
			{
				throw new CommandHandleException(102, "인벤토리슬롯이 부족합니다.");
			}
			m_booty = new PDItemBooty();
			m_booty.id = item.id;
			m_booty.owned = bOwned;
			m_booty.count = nCount;
			m_myHero.AddItem(item, bOwned, nCount, m_changedInventorySlots);
		}
		m_heroReward = new HeroFearAltarHalidomElementalReward(m_myHero);
		m_heroReward.Init(halidomElemental, DateTimeUtil.GetWeekStartDate(currentDate));
		m_myHero.AddFearAltarHalidomElementalReward(m_heroReward);
		SaveToDB();
		SaveToDB_Log();
		FearAltarHalidomElementalRewardReceiveResponseBody resBody = new FearAltarHalidomElementalRewardReceiveResponseBody();
		resBody.weekStartDate = (DateTime)DateTimeUtil.GetWeekStartDate(currentDate);
		resBody.booty = m_booty;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroFearAltarHalidomElementalReward(m_myHero.id, m_heroReward.weekStartDate, m_heroReward.halidomElemental.id));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroFearAltarHalidomElementalRewardLog(Guid.NewGuid(), m_myHero.id, m_heroReward.halidomElemental.id, m_booty.id, m_booty.owned, m_booty.count, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex);
		}
	}
}
