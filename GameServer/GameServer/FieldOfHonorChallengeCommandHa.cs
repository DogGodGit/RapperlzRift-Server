using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class FieldOfHonorChallengeCommandHandler : InGameCommandHandler<FieldOfHonorChallengeCommandBody, FieldOfHonorChallengeResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is FieldOfHonorChallengeParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		DateTimeOffset challengeTime = param.challengeTime;
		DateTime challengeDate = challengeTime.Date;
		Hero targetRanker = param.targetRanker;
		int nTargetRanking = param.targetRanking;
		if (targetRanker == null)
		{
			throw new CommandHandleException(1, "목표랭커가 존재하지 않습니다.");
		}
		Biz.UpdateFieldOfHonorRanker(m_myHero);
		m_myHero.ClearPaidImmediateRevivalDailyCount(challengeDate);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SaveToDB(challengeTime.Date, nPaidImmediateRevivalDailyCount);
		FieldOfHonor fieldOfHonor = Resource.instance.fieldOfHonor;
		FieldOfHonorInstance fieldOfHonorInst = new FieldOfHonorInstance();
		lock (fieldOfHonorInst.syncObject)
		{
			fieldOfHonorInst.Init(m_myHero, targetRanker, nTargetRanking, challengeTime);
			Cache.instance.AddPlace(fieldOfHonorInst);
			m_myHero.SetPositionAndRotation(fieldOfHonor.startPosition, fieldOfHonor.startYRotation);
			fieldOfHonorInst.Enter(m_myHero, challengeTime, bIsRevivalEnter: false);
			fieldOfHonorInst.Start(challengeTime);
			FieldOfHonorChallengeResponseBody resBody = new FieldOfHonorChallengeResponseBody();
			resBody.date = (DateTime)challengeDate;
			resBody.placeInstanceId = (Guid)fieldOfHonorInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.hp = m_myHero.hp;
			resBody.playCount = m_myHero.dailyFieldOfHonorPlayCount.value;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			resBody.targetHero = fieldOfHonorInst.targetRanker.ToPDHero(challengeTime);
			SendResponseOK(resBody);
			m_myHero.ProcessSeriesMission(5);
			m_myHero.ProcessTodayMission(5, challengeTime);
			m_myHero.ProcessTodayTask(12, challengeDate);
			m_myHero.ProcessRetrievalProgressCount(2, challengeDate);
			m_myHero.ProcessMainQuestForContent(13);
			m_myHero.ProcessSubQuestForContent(13);
		}
	}

	private void SaveToDB(DateTime date, int nPaidImmediateRevivalCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, date, nPaidImmediateRevivalCount));
		dbWork.Schedule();
	}
}
