using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ArtifactRoomEnterCommandHandler : InGameCommandHandler<ArtifactRoomEnterCommandBody, ArtifactRoomEnterResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is ArtifactRoomEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		ArtifactRoom artifactRoom = Resource.instance.artifactRoom;
		ArtifactRoomFloor floor = artifactRoom.GetFloor(m_myHero.artifactRoomCurrentFloor);
		DateTimeOffset enterTime = param.enterTime;
		m_myHero.ClearPaidImmediateRevivalDailyCount(enterTime.Date);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SaveToDB(enterTime.Date, nPaidImmediateRevivalDailyCount);
		ArtifactRoomInstance artifactRoomInst = new ArtifactRoomInstance();
		lock (artifactRoomInst.syncObject)
		{
			artifactRoomInst.Init(floor);
			Cache.instance.AddPlace(artifactRoomInst);
			m_myHero.SetPositionAndRotation(artifactRoom.startPosition, artifactRoom.startYRotation);
			artifactRoomInst.Enter(m_myHero, enterTime, bIsRevivalEnter: false);
			artifactRoomInst.Start(enterTime);
			ArtifactRoomEnterResponseBody resBody = new ArtifactRoomEnterResponseBody();
			resBody.placeInstanceId = (Guid)artifactRoomInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.hp = m_myHero.hp;
			resBody.date = (DateTime)enterTime.Date;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
			m_myHero.ProcessMainQuestForContent(10);
			m_myHero.ProcessSubQuestForContent(10);
		}
	}

	private void SaveToDB(DateTime date, int nPaidImmediateRevivalCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, date, nPaidImmediateRevivalCount));
		dbWork.Schedule();
	}
}
