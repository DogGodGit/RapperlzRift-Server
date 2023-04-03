using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class FieldBossMonsterInstance : MonsterInstance
{
	private FieldBoss m_fieldBoss;

	private FieldBossEventSchedule m_schedule;

	private Guid m_logId = Guid.Empty;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.FieldBossMonster;

	public override Monster monster => m_fieldBoss.monsterArrange.monster;

	public FieldBoss fieldBoss => m_fieldBoss;

	public void Init(ContinentInstance continentInst, FieldBoss fieldBoss, FieldBossEventSchedule schedule)
	{
		if (continentInst == null)
		{
			throw new ArgumentNullException("continentInst");
		}
		if (fieldBoss == null)
		{
			throw new ArgumentNullException("fieldBoss");
		}
		if (schedule == null)
		{
			throw new ArgumentNullException("schedule");
		}
		m_fieldBoss = fieldBoss;
		m_schedule = schedule;
		InitMonsterInstance(continentInst, m_fieldBoss.position, m_fieldBoss.yRotation);
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		m_logId = Guid.NewGuid();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddFieldBossCreationLog(m_logId, m_lnInstanceId, m_schedule.id, m_fieldBoss.id, currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Warn(GetType(), null, ex);
		}
	}

	protected override void OnDeadAsync(DateTimeOffset time)
	{
		base.OnDeadAsync(time);
		ContinentInstance continentInst = (ContinentInstance)m_currentPlace;
		continentInst.RemoveFieldBoss(m_fieldBoss.id);
		if (continentInst.isNationTerritory)
		{
			NationContinentInstance nationContinentInst = (NationContinentInstance)continentInst;
			NationInstance nationInst = Cache.instance.GetNationInstance(nationContinentInst.nationId);
			ServerEvent.SendFieldBossDead(nationInst.GetClientPeers(Guid.Empty), m_fieldBoss.id);
		}
		else
		{
			ServerEvent.SendFieldBossDead(Cache.instance.GetClientPeers(Guid.Empty), m_fieldBoss.id);
		}
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddFieldBosskillLog(m_logId, m_lnInstanceId, m_lastDamageTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Warn(GetType(), null, ex);
		}
		if (m_fieldBoss.itemReward == null)
		{
			return;
		}
		FieldBossEvent fieldBossEvent = m_fieldBoss.fieldBossEvent;
		float fRewardRadius = fieldBossEvent.rewardRadius;
		HashSet<Hero> lootedHero = new HashSet<Hero>();
		foreach (MonsterReceivedDamage damage in m_receivedDamages.Values)
		{
			Hero attacker = m_currentPlace.GetHero(damage.attackerId);
			if (attacker == null || lootedHero.Contains(attacker))
			{
				continue;
			}
			lock (attacker.syncObject)
			{
				if (!MathUtil.CircleContains(m_position, fRewardRadius, attacker.position) || attacker.isDead)
				{
					continue;
				}
				LootRewardItem(attacker);
				lootedHero.Add(attacker);
				PartyMember partyMember = attacker.partyMember;
				if (partyMember == null)
				{
					continue;
				}
				foreach (PartyMember member in partyMember.party.members)
				{
					Hero heroMember = m_currentPlace.GetHero(member.id);
					if (heroMember == null || lootedHero.Contains(heroMember))
					{
						continue;
					}
					lock (heroMember.syncObject)
					{
						if (MathUtil.CircleContains(m_position, fRewardRadius, heroMember.position) && !heroMember.isDead)
						{
							LootRewardItem(heroMember);
							lootedHero.Add(heroMember);
						}
					}
				}
			}
		}
	}

	private void LootRewardItem(Hero looter)
	{
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		Mail mail = null;
		ItemReward itemReward = m_fieldBoss.itemReward;
		Item item = itemReward.item;
		bool bOwned = itemReward.owned;
		int nCount = itemReward.count;
		PDItemBooty booty = new PDItemBooty();
		booty.id = item.id;
		booty.owned = bOwned;
		booty.count = nCount;
		int nRemainingCount = looter.AddItem(item, bOwned, nCount, changedInventorySlots);
		if (nRemainingCount > 0)
		{
			mail = Mail.Create("MAIL_REWARD_N_36", "MAIL_REWARD_D_36", m_lastDamageTime);
			mail.AddAttachment(new MailAttachment(item, nRemainingCount, bOwned));
			looter.AddMail(mail, bSendEvent: true);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(looter.id);
		foreach (InventorySlot slot in changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(mail));
		}
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddFieldBossRewardLog(Guid.NewGuid(), m_logId, looter.id, item.id, bOwned, nCount));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
		ServerEvent.SendFieldBossRewardLooted(looter.account.peer, booty, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDFieldBossMonsterInstance();
	}
}
