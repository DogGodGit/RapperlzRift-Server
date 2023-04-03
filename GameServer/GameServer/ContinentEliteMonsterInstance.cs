using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ContinentEliteMonsterInstance : EliteMonsterInstance
{
	private Guid m_logId = Guid.Empty;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.ContinentEliteMonster;

	public Guid logId => m_logId;

	public void Init(ContinentInstance continentInst, EliteMonsterMaster master, DateTimeOffset time)
	{
		if (continentInst == null)
		{
			throw new ArgumentNullException("continentInst");
		}
		if (master == null)
		{
			throw new ArgumentNullException("master");
		}
		m_eliteMonster = master.SelectMonster();
		InitMonsterInstance(continentInst, master.position, master.SelectRotationY());
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			m_logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddContinentEliteMonsterSpawnLog(m_logId, m_eliteMonster.id, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	protected override void OnDeadAsync(DateTimeOffset time)
	{
		base.OnDeadAsync(time);
		int nEliteMonsterKillApplicationRequiredHeroLevel = Resource.instance.eliteMonsterKillApplicationRequiredHeroLevel;
		PartyMember partyMember = null;
		MonsterReceivedDamage[] array = base.sortedReceivedDamages;
		foreach (MonsterReceivedDamage damage in array)
		{
			Hero attackHero = m_currentPlace.GetHero(damage.attackerId);
			if (attackHero == null)
			{
				continue;
			}
			lock (attackHero.syncObject)
			{
				if (!IsInterestSector(attackHero.sector) || attackHero.isDead)
				{
					continue;
				}
				if (attackHero.level >= nEliteMonsterKillApplicationRequiredHeroLevel)
				{
					attackHero.IncreaseEliteMonsterKillCount(m_eliteMonster);
					try
					{
						SFSqlStandaloneWork logWork2 = SqlStandaloneWorkUtil.CreateGameLogDBWork();
						logWork2.AddSqlCommand(GameLogDac.CSC_AddHeroContinentEliteMonsterKillLog(Guid.NewGuid(), m_logId, attackHero.id, m_lastDamageTime));
						logWork2.Schedule();
					}
					catch (Exception ex2)
					{
						SFLogUtil.Error(GetType(), null, ex2);
					}
				}
				partyMember = attackHero.partyMember;
				break;
			}
		}
		if (partyMember == null)
		{
			return;
		}
		foreach (PartyMember member in partyMember.party.members)
		{
			if (member.id == partyMember.id)
			{
				continue;
			}
			Hero heroMember = m_currentPlace.GetHero(member.id);
			if (heroMember == null)
			{
				continue;
			}
			lock (heroMember.syncObject)
			{
				if (IsInterestSector(heroMember.sector) && !heroMember.isDead && heroMember.level >= nEliteMonsterKillApplicationRequiredHeroLevel)
				{
					heroMember.IncreaseEliteMonsterKillCount(m_eliteMonster);
					try
					{
						SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
						logWork.AddSqlCommand(GameLogDac.CSC_AddHeroContinentEliteMonsterKillLog(Guid.NewGuid(), m_logId, heroMember.id, m_lastDamageTime));
						logWork.Schedule();
					}
					catch (Exception ex)
					{
						SFLogUtil.Error(GetType(), null, ex);
					}
				}
			}
		}
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		PDContinentEliteMonsterInstance inst = new PDContinentEliteMonsterInstance();
		inst.eliteMonsterId = m_eliteMonster.id;
		return inst;
	}
}
