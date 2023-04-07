using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class ProofOfValorEnterCommandHandler : InGameCommandHandler<ProofOfValorEnterCommandBody, ProofOfValorEnterResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is ProofOfValorEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		DateTimeOffset enterTime = param.enterTime;
		DateTime enterDate = enterTime.Date;
		ProofOfValor proofOfValor = Resource.instance.proofOfValor;
		m_myHero.UseStamina(proofOfValor.requiredStamina, enterTime);
		m_myHero.ClearPaidImmediateRevivalDailyCount(enterDate);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SaveToDB(enterDate, nPaidImmediateRevivalDailyCount);
		m_myHero.heroProofOfValorInst.level = m_myHero.level;
		ProofOfValorInstance proofOfValorInst = new ProofOfValorInstance();
		lock (proofOfValorInst.syncObject)
		{
			proofOfValorInst.Init(m_myHero.heroProofOfValorInst, enterTime);
			Cache.instance.AddPlace(proofOfValorInst);
			m_myHero.SetPositionAndRotation(proofOfValor.startPosition, proofOfValor.startYRotation);
			proofOfValorInst.Enter(m_myHero, enterTime, bIsRevivalEnter: false);
			proofOfValorInst.Start(enterTime);
			ProofOfValorEnterResponseBody resBody = new ProofOfValorEnterResponseBody();
			resBody.date = (DateTime)enterTime.Date;
			resBody.placeInstanceId = (Guid)proofOfValorInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.monsters = proofOfValorInst.GetPDMonsterInstances<PDMonsterInstance>(enterTime).ToArray();
			resBody.hp = m_myHero.hp;
			resBody.stamina = m_myHero.stamina;
			resBody.playCount = m_myHero.dailyProofOfValorPlayCount.value;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
			m_myHero.ProcessTodayTask(9, enterDate);
			m_myHero.IncreaseOpen7DayEventProgressCount(8);
			m_myHero.ProcessMainQuestForContent(16);
			m_myHero.ProcessSubQuestForContent(16);
		}
	}

	private void SaveToDB(DateTime date, int nPaidImmediateRevivalCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, date, nPaidImmediateRevivalCount));
		dbWork.Schedule();
	}
}
