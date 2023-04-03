using System;
using System.Collections.Generic;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class UndergroundMazeInstance : Place
{
	public class MonsterRegenTimerState
	{
		public Timer timer;

		public UndergroundMazeMonsterArrange arrange;

		public DateTimeOffset regenTime;
	}

	private UndergroundMazeFloor m_floor;

	private Nation m_nation;

	private HashSet<MonsterRegenTimerState> m_monsterRegenTimerStates = new HashSet<MonsterRegenTimerState>();

	public override PlaceType placeType => PlaceType.UndergroundMaze;

	public override Location location => m_floor.undergroundMaze;

	public override int locationParam => m_floor.floor;

	public override Rect3D mapRect => undergroundMaze.mapRect;

	public override bool interestManaged => true;

	public override bool ownershipManaged => true;

	public override bool battleEnabled => true;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => true;

	public override bool isExpScrollBuffEnabled => true;

	public override bool isExpLevelPenaltyEnabled => true;

	public override bool isWorldLevelExpBuffEnabled => true;

	public UndergroundMaze undergroundMaze => m_floor.undergroundMaze;

	public UndergroundMazeFloor floor => m_floor;

	public Nation nation => m_nation;

	public void Init(UndergroundMazeFloor floor, Nation nation)
	{
		if (floor == null)
		{
			throw new ArgumentNullException("floor");
		}
		if (nation == null)
		{
			throw new ArgumentNullException("nation");
		}
		m_floor = floor;
		m_nation = nation;
		InitPlace();
		foreach (UndergroundMazeMonsterArrange arrange in m_floor.arranges)
		{
			InitMonster(arrange);
		}
	}

	private void InitMonster(UndergroundMazeMonsterArrange arrange)
	{
		for (int i = 0; i < arrange.monsterCount; i++)
		{
			AddMonster(CreateMonster(arrange));
		}
	}

	private UndergroundMazeMonsterInstance CreateMonster(UndergroundMazeMonsterArrange arrange)
	{
		UndergroundMazeMonsterInstance monsterInst = new UndergroundMazeMonsterInstance();
		monsterInst.Init(this, arrange);
		return monsterInst;
	}

	protected override void OnHeroExit(Hero hero, bool isLogOut)
	{
		base.OnHeroExit(hero, isLogOut);
		if (isLogOut)
		{
			try
			{
				SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
				int nPlayTime = (int)Math.Floor((DateTimeUtil.currentTime - hero.undergroundMazeStartTime).TotalSeconds);
				logWork.AddSqlCommand(GameLogDac.CSC_UpdateUndergroundMazePlayLog(hero.undergroundMazeLogId, nPlayTime));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
			}
		}
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		OnMonsterRemoved_UndergroundMazeMonster((UndergroundMazeMonsterInstance)monsterInst);
	}

	private void OnMonsterRemoved_UndergroundMazeMonster(UndergroundMazeMonsterInstance monsterInst)
	{
		DateTimeOffset monsterDeadTime = monsterInst.lastDamageTime;
		UndergroundMazeMonsterArrange arrange = monsterInst.arrange;
		int nRegenTime = arrange.regenTime * 1000;
		MonsterRegenTimerState state = new MonsterRegenTimerState();
		state.arrange = arrange;
		state.regenTime = monsterDeadTime.AddSeconds(arrange.regenTime);
		state.timer = new Timer(OnMonsterRegenTimerTick, state, -1, -1);
		state.timer.Change(nRegenTime, -1);
		m_monsterRegenTimerStates.Add(state);
	}

	private void OnMonsterRegenTimerTick(object state)
	{
		AddWork(new SFAction<MonsterRegenTimerState>(RegenMonster, (MonsterRegenTimerState)state), bGlobalLockRequired: false);
	}

	private void RegenMonster(MonsterRegenTimerState state)
	{
		Timer timer = state.timer;
		timer.Dispose();
		m_monsterRegenTimerStates.Remove(state);
		UndergroundMazeMonsterInstance monsterInst = CreateMonster(state.arrange);
		SpawnMonster(monsterInst, DateTimeUtil.currentTime);
	}

	public void OnHeroLimitTimeExpired(Hero hero)
	{
		AddWork(new SFAction<Hero>(DungeonBanish, hero), bGlobalLockRequired: true);
	}

	private void DungeonBanish(Hero hero)
	{
		hero.Revive(bSendEvent: false);
		Exit(hero, isLogOut: false, null);
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			int nPlayTime = (int)Math.Floor((DateTimeUtil.currentTime - hero.undergroundMazeStartTime).TotalSeconds);
			logWork.AddSqlCommand(GameLogDac.CSC_UpdateUndergroundMazePlayLog(hero.undergroundMazeLogId, nPlayTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
		ServerEvent.SendUndergroundMazeBanished(hero.account.peer, hero.previousContinentId, hero.previousNationId, hero.hp);
	}

	protected override void ReleaseInternal()
	{
		base.ReleaseInternal();
		foreach (MonsterRegenTimerState state in m_monsterRegenTimerStates)
		{
			state.timer.Dispose();
		}
		m_monsterRegenTimerStates.Clear();
	}
}
