using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class WisdomTempleSweepCommandHandler : InGameCommandHandler<WisdomTempleSweepCommandBody, WisdomTempleSweepResponseBody>
{
	public const short kResult_Dead = 101;

	public const short kResult_NotEnoughSweepItem = 102;

	public const short kResult_NotEnoughStamina = 103;

	public const short kResult_EnterCountOverflowed = 104;

	private WisdomTemple m_wisdomTemple;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private bool m_bIsFreeSweep;

	private DateValuePair<int> m_freeSweepDailyCount;

	private long m_lnAcquisitionExp;

	private PDItemBooty m_itemBooty;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		m_wisdomTemple = Resource.instance.wisdomTemple;
		if (!m_myHero.wisdomTempleCleared)
		{
			throw new CommandHandleException(1, "영웅이 소탕에 필요한 조건을 만족하지 못했습니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은상태 입니다.");
		}
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		m_myHero.RefreshFreeSweepDailyCount(currentDate);
		m_freeSweepDailyCount = m_myHero.freeSweepDailyCount;
		if (m_freeSweepDailyCount.value < Resource.instance.dungeonFreeSweepDailyCount)
		{
			m_bIsFreeSweep = true;
		}
		int nDungeonSweepItemId = Resource.instance.dungeonSweepItemId;
		if (!m_bIsFreeSweep && m_myHero.GetItemCount(nDungeonSweepItemId) == 0)
		{
			throw new CommandHandleException(102, "소탕령이 부족합니다.");
		}
		int nRequiredStamina = m_wisdomTemple.requiredStamina;
		if (m_myHero.stamina < nRequiredStamina)
		{
			throw new CommandHandleException(103, "스태미너가 부족합니다.");
		}
		if (m_myHero.GetWisdomTempleAvailableEnterCount(currentDate) <= 0)
		{
			throw new CommandHandleException(104, "입장횟수가 초과되었습니다.");
		}
		int nUsedOwnCount = 0;
		int nUsedUnOwnCount = 0;
		if (!m_bIsFreeSweep)
		{
			m_myHero.UseItem(nDungeonSweepItemId, bFisetUseOwn: true, 1, m_changedInventorySlots, out nUsedOwnCount, out nUsedUnOwnCount);
		}
		else
		{
			m_freeSweepDailyCount.value++;
		}
		m_myHero.UseStamina(nRequiredStamina, m_currentTime);
		m_myHero.dailyWisdomTemplePlayCount.value++;
		SweepReward();
		SaveToDB();
		SaveToDB_Log();
		WisdomTempleSweepResponseBody resBody = new WisdomTempleSweepResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.stamina = m_myHero.stamina;
		resBody.playCount = m_myHero.dailyWisdomTemplePlayCount.value;
		resBody.freeSweepDailyCount = m_freeSweepDailyCount.value;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.booty = m_itemBooty;
		resBody.acquiredExp = m_lnAcquisitionExp;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	private void SweepReward()
	{
		m_lnAcquisitionExp = m_wisdomTemple.GetSweepReward(m_myHero.level).expReward?.value ?? 0;
		if (m_lnAcquisitionExp > 0)
		{
			m_lnAcquisitionExp = (long)Math.Floor((float)m_lnAcquisitionExp * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
			m_myHero.AddExp(m_lnAcquisitionExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		}
		ItemReward itemReward = m_wisdomTemple.sweepItemReward;
		if (itemReward != null)
		{
			Item item = itemReward.item;
			int nCount = itemReward.count;
			bool bOwned = itemReward.owned;
			int nRewardItemRemainCount = m_myHero.AddItem(item, bOwned, nCount, m_changedInventorySlots);
			m_itemBooty = new PDItemBooty();
			m_itemBooty.id = item.id;
			m_itemBooty.count = nCount;
			m_itemBooty.owned = bOwned;
			if (nRewardItemRemainCount > 0)
			{
				m_mail = Mail.Create("MAIL_REWARD_N_17", "MAIL_REWARD_D_17", m_currentTime);
				m_mail.AddAttachmentWithNo(new MailAttachment(item, nRewardItemRemainCount, bOwned));
				m_myHero.AddMail(m_mail, bSendEvent: true);
			}
		}
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroWisdomTemplePlay(m_myHero.id, m_currentTime.Date, m_myHero.dailyWisdomTemplePlayCount.value));
		if (m_lnAcquisitionExp > 0)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		}
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWisdomTempleRewardLog(logId, m_myHero.id, Guid.Empty, m_currentTime));
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWisdomTempleRewardDetailLog(Guid.NewGuid(), logId, 0, m_lnAcquisitionExp, (m_itemBooty != null) ? m_itemBooty.id : 0, m_itemBooty != null && m_itemBooty.owned, (m_itemBooty != null) ? m_itemBooty.count : 0));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}
}
