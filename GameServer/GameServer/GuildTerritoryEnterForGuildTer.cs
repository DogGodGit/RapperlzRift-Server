using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class GuildTerritoryEnterForGuildTerritoryRevivalParam : PlaceEntranceParam
{
}
public class GuildTerritoryEnterForGuildTerritoryRevivalCommandHandler : InGameCommandHandler<GuildTerritoryEnterForGuildTerritoryRevivalCommandBody, GuildTerritoryEnterForGuildTerritoryRevivalResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is GuildTerritoryEnterForGuildTerritoryRevivalParam))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		GuildMember myGuildMember = m_myHero.guildMember;
		if (myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되지 않았습니다.");
		}
		m_myHero.Revive(bSendEvent: false);
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		m_myHero.ClearPaidImmediateRevivalDailyCount(currentDate);
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SaveToDB(currentTime, nPaidImmediateRevivalDailyCount);
		SaveToDB_Log(currentTime);
		GuildTerritoryInstance targetPlace = myGuildMember.guild.territoryInst;
		lock (targetPlace.syncObject)
		{
			m_myHero.SetPositionAndRotation(targetPlace.territory.SelectStartPosition(), targetPlace.territory.SelectStartRotationY());
			targetPlace.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			GuildTerritoryEnterForGuildTerritoryRevivalResponseBody resBody = new GuildTerritoryEnterForGuildTerritoryRevivalResponseBody();
			resBody.placeInstanceId = (Guid)targetPlace.instanceId;
			List<Sector> interestSectors = targetPlace.GetInterestSectors(m_myHero.sector);
			resBody.heroes = Sector.GetPDHeroes(interestSectors, m_myHero.id, currentTime).ToArray();
			resBody.monsters = Sector.GetPDMonsterInstances<PDMonsterInstance>(interestSectors, currentTime).ToArray();
			resBody.hp = m_myHero.hp;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
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
	}

	private void SaveToDB_Log(DateTimeOffset currentTime)
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
