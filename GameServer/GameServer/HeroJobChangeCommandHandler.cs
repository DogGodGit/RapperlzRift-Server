using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroJobChangeCommandHandler : InGameCommandHandler<HeroJobChangeCommandBody, HeroJobChangeResponseBody>
{
	public const short kResult_NotEnoughHeroLevel = 101;

	public const short kResult_NotEnoughItem = 102;

	private int m_nUsedItemId;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

	private InventorySlot m_changedInventorySlot;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nTargetJobId = m_body.targetJobId;
		if (nTargetJobId <= 0)
		{
			throw new CommandHandleException(1, "목표직업ID가 유효하지 않습니다. nTargetJobId = " + nTargetJobId);
		}
		if (m_myHero.level < Resource.instance.jobChangeRequiredHeroLevel)
		{
			throw new CommandHandleException(101, "영웅레벨이 부족합니다.");
		}
		Job currentJob = m_myHero.job;
		if (currentJob.parentJobId > 0)
		{
			throw new CommandHandleException(1, "이미 전직을 했습니다.");
		}
		Job targetJob = Resource.instance.GetJob(nTargetJobId);
		if (targetJob == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 직업입니다. nTargetJobId = " + nTargetJobId);
		}
		if (targetJob.parentJobId != currentJob.id)
		{
			throw new CommandHandleException(1, "하위 직업군이 맞지 않습니다.");
		}
		m_nUsedItemId = Resource.instance.jobChangeRequiredItemId;
		if (m_myHero.GetItemCount(m_nUsedItemId) < 1)
		{
			throw new CommandHandleException(102, "아이템이 부족합니다.");
		}
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		m_myHero.UseItem(m_nUsedItemId, bFisetUseOwn: true, 1, changedInventorySlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
		m_changedInventorySlot = changedInventorySlots[0];
		m_myHero.ChangeJob(targetJob);
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB();
		SaveToLogDB(currentJob.id);
		HeroJobChangeResponseBody resBody = new HeroJobChangeResponseBody();
		resBody.changedInventorySlot = m_changedInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Job(m_myHero.id, m_myHero.job.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_changedInventorySlot));
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nOldJobId)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroJobChangeLog(Guid.NewGuid(), m_myHero.id, nOldJobId, m_myHero.job.id, m_nUsedItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
