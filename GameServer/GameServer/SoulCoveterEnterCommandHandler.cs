using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class SoulCoveterEnterCommandHandler : InGameCommandHandler<SoulCoveterEnterCommandBody, SoulCoveterEnterResponseBody>
{
	public const short kResult_EnterTimeout = 101;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	private SoulCoveterInstance m_soulCoveterInst;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is SoulCoveterEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		m_dungeonCreationTime = param.dungeonCreationTime;
		DateTime dungeonCreationDate = m_dungeonCreationTime.Date;
		Guid instanceId = param.soulCoveterInstanceId;
		m_soulCoveterInst = Cache.instance.GetPlace(instanceId) as SoulCoveterInstance;
		if (m_soulCoveterInst == null)
		{
			throw new CommandHandleException(1, "던전이 존재하지 않습니다.");
		}
		if (m_soulCoveterInst.isFinished)
		{
			throw new CommandHandleException(101, "현재 던전에 입장할 수 없는 상태입니다.");
		}
		SoulCoveter soulCoveter = Resource.instance.soulCoveter;
		m_myHero.UseStamina(soulCoveter.requiredStamina, m_dungeonCreationTime);
		m_myHero.ClearPaidImmediateRevivalDailyCount(dungeonCreationDate);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		lock (m_soulCoveterInst.syncObject)
		{
			m_myHero.SetPositionAndRotation(soulCoveter.SelectPosition(), soulCoveter.SelectRotationY());
			m_soulCoveterInst.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			m_myHero.RefreshWeeklySoulCoveterPlayCount(dungeonCreationDate);
			DateValuePair<int> weeklySoulCoveterPlayCount = m_myHero.weeklySoulCoveterPlayCount;
			weeklySoulCoveterPlayCount.value++;
			m_myHero.accSoulCoveterPlayCount++;
			int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
			SaveToDB(nPaidImmediateRevivalDailyCount);
			SoulCoveterEnterResponseBody resBody = new SoulCoveterEnterResponseBody();
			resBody.date = (DateTime)dungeonCreationDate;
			resBody.placeInstanceId = (Guid)m_soulCoveterInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.remainingStartTime = m_soulCoveterInst.GetRemainingStartTime(currentTime);
			resBody.remainingLimitTime = m_soulCoveterInst.GetRemainingLimitTime(currentTime);
			resBody.waveNo = m_soulCoveterInst.waveNo;
			resBody.heroes = m_soulCoveterInst.GetPDHeroes(m_myHero.id, currentTime).ToArray();
			resBody.monsterInsts = m_soulCoveterInst.GetPDMonsterInstances(currentTime).ToArray();
			resBody.hp = m_myHero.hp;
			resBody.stamina = m_myHero.stamina;
			resBody.playCount = weeklySoulCoveterPlayCount.value;
			resBody.accPlayCount = m_myHero.accSoulCoveterPlayCount;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
			m_myHero.ProcessMainQuestForContent(15);
			m_myHero.ProcessSubQuestForContent(15);
		}
	}

	private void SaveToDB(int nPaidImmediateRevivalCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, m_dungeonCreationTime.Date, nPaidImmediateRevivalCount));
		dbWork.AddSqlCommand(GameDac.CSC_AddSoulCoveterInstanceMember(m_soulCoveterInst.instanceId, m_myHero.id, 0, m_dungeonCreationTime));
		dbWork.Schedule();
	}
}
