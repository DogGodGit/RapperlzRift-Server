using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class UndergroundMazeEnterForUndergroundMazeTransmissionParam : PlaceEntranceParam
{
	private UndergroundMazeFloor m_floor;

	public UndergroundMazeFloor floor => m_floor;

	public UndergroundMazeEnterForUndergroundMazeTransmissionParam(UndergroundMazeFloor floor)
	{
		m_floor = floor;
	}
}
public class UndergroundMazeEnterForUndergroundMazeReviveParam : PlaceEntranceParam
{
	private UndergroundMazeInstance m_undergroundMazeInst;

	public UndergroundMazeInstance undergroundMazeInst => m_undergroundMazeInst;

	public UndergroundMazeEnterForUndergroundMazeReviveParam(UndergroundMazeInstance undergroundMazeInst)
	{
		m_undergroundMazeInst = undergroundMazeInst;
	}
}
public class UndergroundMazeEnterForUndergroundMazeReviveCommandHandler : InGameCommandHandler<UndergroundMazeEnterForUndergroundMazeReviveCommandBody, UndergroundMazeEnterForUndergroundMazeReviveResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is UndergroundMazeEnterForUndergroundMazeReviveParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		UndergroundMazeInstance targetUnergroundMazeInst = param.undergroundMazeInst;
		if (targetUnergroundMazeInst == null)
		{
			throw new CommandHandleException(1, "해당 지하미궁인스턴스가 존재하지 않습니다.");
		}
		UndergroundMaze undergroundMaze = targetUnergroundMazeInst.undergroundMaze;
		m_myHero.SetPositionAndRotation(undergroundMaze.SelectPosition(), undergroundMaze.SelectRotationY());
		m_myHero.Revive(bSendEvent: false);
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		m_myHero.ClearPaidImmediateRevivalDailyCount(currentDate);
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SaveToDB(currentTime, nPaidImmediateRevivalDailyCount);
		lock (targetUnergroundMazeInst.syncObject)
		{
			targetUnergroundMazeInst.Enter(m_myHero, currentTime, bIsRevivalEnter: true);
			UndergroundMazeEnterForUndergroundMazeReviveResponseBody resBody = new UndergroundMazeEnterForUndergroundMazeReviveResponseBody();
			resBody.hp = m_myHero.hp;
			resBody.placeInstanceId = (Guid)targetUnergroundMazeInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.heroes = targetUnergroundMazeInst.GetPDHeroes(currentTime).ToArray();
			resBody.monsterInsts = targetUnergroundMazeInst.GetPDMonsterInstances<PDUndergroundMazeMonsterInstance>(currentTime).ToArray();
			resBody.date = (DateTime)currentDate;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
		}
	}

	private void SaveToDB(DateTimeOffset currentTime, int nPaidImmediateRevivalDailyCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, currentTime.Date, nPaidImmediateRevivalDailyCount));
		dbWork.Schedule();
		SaveToDB_AddHeroRevivallog(currentTime);
	}

	private void SaveToDB_AddHeroRevivallog(DateTimeOffset currentTime)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroRevivalLog(Guid.NewGuid(), m_myHero.id, 1, 0, 0, currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
