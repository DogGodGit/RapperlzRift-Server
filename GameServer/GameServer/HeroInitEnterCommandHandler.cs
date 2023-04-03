using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroInitEnterCommandHandler : InGameCommandHandler<HeroInitEnterCommandBody, HeroInitEnterResponseBody>
{
	public const short kResult_ProgressingNationWarByTargetNation = 101;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private Location m_targetLocation;

	private int m_nTargetLocationParam;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetRotationY;

	private CartInstance m_ridingCartInst;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is HeroInitEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		m_currentTime = DateTimeUtil.currentTime;
		m_targetLocation = param.location;
		m_nTargetLocationParam = param.locationParam;
		m_targetPosition = param.position;
		m_fTargetRotationY = param.rotationY;
		switch (m_targetLocation.locationType)
		{
		case LocationType.Continent:
			ProcessForContinent();
			break;
		case LocationType.UndergroundMaze:
			ProcessForUndergroundMaze();
			break;
		case LocationType.GuildTerritory:
			ProcessForGuildTerritory();
			break;
		default:
			throw new CommandHandleException(1, "대상위치의 타입이 유효하지 않습니다.");
		}
	}

	private void ProcessForContinent()
	{
		ContinentInstance targetPlace = null;
		Continent targetContinent = (Continent)m_targetLocation;
		if (targetContinent.isNationTerritory)
		{
			NationInstance nationInst = Cache.instance.GetNationInstance(m_nTargetLocationParam);
			if (nationInst == null)
			{
				throw new CommandHandleException(1, "해당 국가가 존재하지 않습니다. m_nTargetLocationParam = " + m_nTargetLocationParam);
			}
			NationWarInstance nationWarInst = nationInst.nationWarInst;
			if (nationWarInst != null && !nationWarInst.IsNationWarJoinEnabled(m_myHero))
			{
				Resource res = Resource.instance;
				m_myHero.placeEntranceParam = new ContinentSaftyAreaEnterParam(res.GetContinent(res.saftyRevivalContinentId), m_myHero.nationId, res.SelectSaftyRevivalPosition(), res.SelectSaftyRevivalYRotation(), m_currentTime);
				throw new CommandHandleException(101, "해당 국가가 국가전 진행중이므로 입장할 수 없습니다.");
			}
			targetPlace = nationInst.GetContinentInstance(targetContinent.id);
		}
		else
		{
			targetPlace = Cache.instance.GetDisputeContinentInstance(targetContinent.id);
		}
		if (targetPlace == null)
		{
			throw new CommandHandleException(1, "해당 장소가 존재하지 않습니다. m_targetLocationId = " + m_targetLocation.locationId);
		}
		lock (targetPlace.syncObject)
		{
			m_myHero.SetPositionAndRotation(m_targetPosition, m_fTargetRotationY);
			targetPlace.Enter(m_myHero, m_currentTime, bIsRevivalEnter: false);
			ProcessForContinent_Cart(targetPlace);
		}
	}

	private void ProcessForContinent_Cart(ContinentInstance targetPlace)
	{
		ProcessForContinent_MainQuestCart(targetPlace);
		ProcessForContinent_SupplySupportQuestCart(targetPlace);
		ProcessForContinent_GuildSupplySupportQuestCart(targetPlace);
		ProcessForContinent_Finish(targetPlace);
	}

	private void ProcessForContinent_MainQuestCart(ContinentInstance targetPlace)
	{
		HeroMainQuest heroMainQuest = m_myHero.currentHeroMainQuest;
		if (heroMainQuest == null)
		{
			return;
		}
		MainQuestCartInstance cartInst = heroMainQuest.cartInst;
		if (cartInst == null)
		{
			return;
		}
		lock (cartInst.syncObject)
		{
			if (cartInst.isRiding)
			{
				cartInst.SetPositionAndRotation(m_targetPosition, m_fTargetRotationY);
				targetPlace.EnterCart(cartInst, m_currentTime, bSendEvent: true);
				m_ridingCartInst = cartInst;
				return;
			}
			cartInst.SetPositionAndRotation(heroMainQuest.cartPosition, heroMainQuest.cartRotationY);
			NationContinentInstance cartPlace = m_myHero.nationInst.GetContinentInstance(heroMainQuest.cartContinentId);
			lock (cartPlace.syncObject)
			{
				cartPlace.EnterCart(cartInst, m_currentTime, bSendEvent: true);
			}
		}
	}

	private void ProcessForContinent_SupplySupportQuestCart(ContinentInstance targetPlace)
	{
		HeroSupplySupportQuest heroQuest = m_myHero.supplySupportQuest;
		if (heroQuest == null)
		{
			return;
		}
		SupplySupportQuestCartInstance cartInst = heroQuest.cartInst;
		if (cartInst == null)
		{
			throw new Exception("cartInst");
		}
		lock (cartInst.syncObject)
		{
			if (cartInst.isRiding)
			{
				cartInst.SetPositionAndRotation(m_targetPosition, m_fTargetRotationY);
				targetPlace.EnterCart(cartInst, m_currentTime, bSendEvent: true);
				m_ridingCartInst = cartInst;
				return;
			}
			cartInst.SetPositionAndRotation(heroQuest.cartPosition, heroQuest.cartYRotation);
			NationContinentInstance cartPlace = m_myHero.nationInst.GetContinentInstance(heroQuest.cartContinentId);
			lock (cartPlace.syncObject)
			{
				cartPlace.EnterCart(cartInst, m_currentTime, bSendEvent: true);
			}
		}
	}

	private void ProcessForContinent_GuildSupplySupportQuestCart(ContinentInstance targetPlace)
	{
		GuildSupplySupportQuestPlay guildQuest = m_myHero.guildSupplySupportQuestPlay;
		if (guildQuest == null)
		{
			return;
		}
		GuildSupplySupportQuestCartInstance cartInst = guildQuest.cartInst;
		if (cartInst == null)
		{
			throw new Exception("cartInst");
		}
		lock (cartInst.syncObject)
		{
			if (cartInst.isRiding)
			{
				cartInst.SetPositionAndRotation(m_targetPosition, m_fTargetRotationY);
				targetPlace.EnterCart(cartInst, m_currentTime, bSendEvent: true);
				m_ridingCartInst = cartInst;
				return;
			}
			cartInst.SetPositionAndRotation(guildQuest.cartPosition, guildQuest.cartYRotation);
			NationContinentInstance cartPlace = m_myHero.nationInst.GetContinentInstance(guildQuest.cartContinentId);
			lock (cartPlace.syncObject)
			{
				cartPlace.EnterCart(cartInst, m_currentTime, bSendEvent: true);
			}
		}
	}

	private void ProcessForContinent_Finish(ContinentInstance targetPlace)
	{
		m_myHero.isInitEntranceCopmleted = true;
		HeroInitEnterResponseBody resBody = new HeroInitEnterResponseBody();
		resBody.position = m_myHero.position;
		resBody.rotationY = m_myHero.rotationY;
		resBody.placeInstanceId = (Guid)targetPlace.instanceId;
		List<Sector> interestSectors = targetPlace.GetInterestSectors(m_myHero.sector);
		resBody.heroes = Sector.GetPDHeroes(interestSectors, m_myHero.id, m_currentTime).ToArray();
		resBody.monsterInsts = Sector.GetPDMonsterInstances<PDMonsterInstance>(interestSectors, m_currentTime).ToArray();
		resBody.continentObjectInsts = Sector.GetPDContinentObjectInstances(interestSectors).ToArray();
		resBody.cartInsts = Sector.GetPDCartInstances(interestSectors, (m_ridingCartInst != null) ? m_ridingCartInst.instanceId : 0, m_currentTime).ToArray();
		SendResponseOK(resBody);
	}

	private void ProcessForUndergroundMaze()
	{
		UndergroundMaze undergroundMaze = (UndergroundMaze)m_targetLocation;
		UndergroundMazeFloor floor = undergroundMaze.GetFloor(m_nTargetLocationParam);
		if (floor == null)
		{
			throw new CommandHandleException(1, "해당 지하미로층이 존재하지 않습니다. m_nTargetLocationParam = " + m_nTargetLocationParam);
		}
		UndergroundMazeInstance targetPlace = m_myHero.nationInst.GetUndergroundMazeInstance(m_nTargetLocationParam);
		if (targetPlace == null)
		{
			throw new CommandHandleException(1, "해당 장소가 존재하지 않습니다. m_targetLocationId = " + m_targetLocation.locationId);
		}
		lock (targetPlace.syncObject)
		{
			m_myHero.SetPositionAndRotation(m_targetPosition, m_fTargetRotationY);
			targetPlace.Enter(m_myHero, m_currentTime, bIsRevivalEnter: false);
			m_myHero.StartUndergroundMazePlay(m_currentTime);
			m_myHero.isInitEntranceCopmleted = true;
			try
			{
				SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
				logWork.AddSqlCommand(GameLogDac.CSC_AddUndergroundMazePlayLog(m_myHero.undergroundMazeLogId, targetPlace.instanceId, m_myHero.id, 0, m_currentTime));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				LogError(null, ex, bStackTrace: true);
			}
			HeroInitEnterResponseBody resBody = new HeroInitEnterResponseBody();
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.placeInstanceId = (Guid)targetPlace.instanceId;
			List<Sector> interestSectors = targetPlace.GetInterestSectors(m_myHero.sector);
			resBody.heroes = Sector.GetPDHeroes(interestSectors, m_myHero.id, m_currentTime).ToArray();
			resBody.monsterInsts = Sector.GetPDMonsterInstances<PDMonsterInstance>(interestSectors, m_currentTime).ToArray();
			SendResponseOK(resBody);
		}
	}

	private void ProcessForGuildTerritory()
	{
		if (m_myHero.guildMember == null)
		{
			throw new CommandHandleException(1, "길드에 가입되어있지 않습니다.");
		}
		GuildTerritoryInstance targetPlace = m_myHero.guildMember.guild.territoryInst;
		lock (targetPlace.syncObject)
		{
			m_myHero.SetPositionAndRotation(m_targetPosition, m_fTargetRotationY);
			targetPlace.Enter(m_myHero, m_currentTime, bIsRevivalEnter: false);
			m_myHero.isInitEntranceCopmleted = true;
			HeroInitEnterResponseBody resBody = new HeroInitEnterResponseBody();
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.placeInstanceId = (Guid)targetPlace.instanceId;
			List<Sector> interestSectors = targetPlace.GetInterestSectors(m_myHero.sector);
			resBody.heroes = Sector.GetPDHeroes(interestSectors, m_myHero.id, m_currentTime).ToArray();
			resBody.monsterInsts = Sector.GetPDMonsterInstances<PDMonsterInstance>(interestSectors, m_currentTime).ToArray();
			SendResponseOK(resBody);
		}
	}
}
