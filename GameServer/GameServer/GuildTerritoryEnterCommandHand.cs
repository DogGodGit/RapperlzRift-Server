using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class GuildTerritoryEnterCommandHandler : InGameCommandHandler<GuildTerritoryEnterCommandBody, GuildTerritoryEnterResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is GuildTerritoryEnterParam))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		GuildMember myGuildMember = m_myHero.guildMember;
		if (myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되지 않았습니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		GuildTerritoryInstance targetPlace = myGuildMember.guild.territoryInst;
		lock (targetPlace.syncObject)
		{
			m_myHero.SetPositionAndRotation(targetPlace.territory.SelectStartPosition(), targetPlace.territory.SelectStartRotationY());
			targetPlace.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			GuildTerritoryEnterResponseBody resBody = new GuildTerritoryEnterResponseBody();
			resBody.placeInstanceId = (Guid)targetPlace.instanceId;
			List<Sector> interestSectors = targetPlace.GetInterestSectors(m_myHero.sector);
			resBody.heroes = Sector.GetPDHeroes(interestSectors, m_myHero.id, currentTime).ToArray();
			resBody.monsters = Sector.GetPDMonsterInstances<PDMonsterInstance>(interestSectors, currentTime).ToArray();
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			SendResponseOK(resBody);
		}
	}
}
