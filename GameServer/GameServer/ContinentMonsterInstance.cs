using System;
using ClientCommon;

namespace GameServer;

public class ContinentMonsterInstance : MonsterInstance
{
	private ContinentMonsterArrange m_arrange;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.ContinentMonster;

	public override Monster monster => m_arrange.monster;

	public ContinentMonsterArrange arrange => m_arrange;

	public void Init(ContinentInstance continentInst, ContinentMonsterArrange arrange)
	{
		if (continentInst == null)
		{
			throw new ArgumentException("continentInst");
		}
		if (arrange == null)
		{
			throw new ArgumentException("arrange");
		}
		m_arrange = arrange;
		InitMonsterInstance(continentInst, arrange.SelectPosition(), arrange.SelectRotationY());
	}

	protected override void OnDeadAsync(DateTimeOffset time)
	{
		base.OnDeadAsync(time);
		foreach (MonsterReceivedDamage damage in m_receivedDamages.Values)
		{
			Guid attackerId = damage.attackerId;
			Hero attacker = m_currentPlace.GetHero(attackerId);
			if (attacker == null)
			{
				continue;
			}
			lock (attacker.syncObject)
			{
				if (IsQuestAreaPosition(attacker.position))
				{
					attacker.ProcessGuildMissionForHunt(this, time);
				}
			}
		}
	}

	protected override void ProcessQuest(Hero attacker, DateTimeOffset currentTime)
	{
		base.ProcessQuest(attacker, currentTime);
		attacker.ProcessBountyHunterQuest(this, currentTime);
		attacker.ProcessGuildHuntingQuestForHunt(this);
		attacker.ProcessJobChangeQuestForHunt(this);
	}

	protected override void ProcessQuestForParty_Member(Hero member, DateTimeOffset currentTime)
	{
		base.ProcessQuestForParty_Member(member, currentTime);
		member.ProcessBountyHunterQuest(this, currentTime);
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDContinentMonsterInstance();
	}
}
