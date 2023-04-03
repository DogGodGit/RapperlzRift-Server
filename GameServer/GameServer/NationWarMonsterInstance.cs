using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class NationWarMonsterInstance : MonsterInstance
{
	public const float kMonsterEmergencyHPFactor = 0.2f;

	private NationWarInstance m_nationWarInst;

	private NationWarMonsterArrange m_arrange;

	private Nation m_nation;

	private bool m_bHitEnabled;

	private bool m_bAbnormalStateHitEnabled;

	private int m_nFinalDamagePenaltyRate;

	private bool m_bIsBattleMode;

	private DateTimeOffset m_battleModeStartTime = DateTimeOffset.MinValue;

	private bool m_bIsEmergency;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.NationWarMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public NationWarMonsterArrange arrange => m_arrange;

	public override int nationId => m_nation.id;

	public int arrangeType => m_arrange.type;

	public int arrangeId => m_arrange.id;

	public Nation nation => m_nation;

	public override bool hitEnabled
	{
		get
		{
			if (base.hitEnabled)
			{
				return m_bHitEnabled;
			}
			return false;
		}
	}

	public override bool abnormalStateHitEnabled
	{
		get
		{
			if (base.abnormalStateHitEnabled)
			{
				return m_bAbnormalStateHitEnabled;
			}
			return false;
		}
	}

	protected override int finalDamagePenaltyRate => m_nFinalDamagePenaltyRate;

	public bool isBattleMode => m_bIsBattleMode;

	public DateTimeOffset battleModeStartTime => m_battleModeStartTime;

	public bool isEmergency => m_bIsEmergency;

	public void Init(ContinentInstance continentInst, NationWarInstance nationWarInst, NationWarMonsterArrange arrange, Nation nation)
	{
		if (continentInst == null)
		{
			throw new ArgumentNullException("continentInst");
		}
		if (nationWarInst == null)
		{
			throw new ArgumentNullException("nationWarInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		if (nation == null)
		{
			throw new ArgumentNullException("nation");
		}
		m_nationWarInst = nationWarInst;
		m_arrange = arrange;
		m_nation = nation;
		if (arrange.type == 1)
		{
			m_nFinalDamagePenaltyRate = 10000;
		}
		if (arrange.type != 2)
		{
			m_bHitEnabled = true;
			m_bAbnormalStateHitEnabled = true;
		}
		InitMonsterInstance(continentInst, arrange.position, arrange.yRotation);
	}

	protected override void OnDamage()
	{
		base.OnDamage();
		StartBattleMode();
		CheckEmergency();
	}

	private void StartBattleMode()
	{
		m_battleModeStartTime = m_lastDamageTime;
		if (!m_bIsBattleMode)
		{
			m_bIsBattleMode = true;
			Global.instance.AddWork(new SFAction<NationWarMonsterInstance>(m_nationWarInst.OnMonsterBattleModeStart, this));
		}
	}

	protected override void OnUpdateInternal()
	{
		base.OnUpdateInternal();
		if (isBattleMode)
		{
			DateTimeOffset currentTime = DateTimeUtil.currentTime;
			if (!((currentTime - m_battleModeStartTime).TotalSeconds < (double)Resource.instance.nationWarMonsterBattleModeDuration))
			{
				m_bIsBattleMode = false;
				Global.instance.AddWork(new SFAction<NationWarMonsterInstance>(m_nationWarInst.OnMonsterBattleModeEnd, this));
			}
		}
	}

	private void CheckEmergency()
	{
		if (!m_bIsEmergency)
		{
			int nEmergencyHP = (int)Math.Floor((float)m_nRealMaxHP * 0.2f);
			if (m_nHP <= nEmergencyHP)
			{
				m_bIsEmergency = true;
				m_currentPlace.AddWork(new SFAction<NationWarMonsterInstance>(m_nationWarInst.OnMonsterEmergency, this), bGlobalLockRequired: true);
			}
		}
	}

	protected override void OnDeadAsync(DateTimeOffset time)
	{
		base.OnDeadAsync(time);
		int nNationId = ((NationContinentInstance)m_currentPlace).nationId;
		NationInstance nationInst = Cache.instance.GetNationInstance(nNationId);
		NationWarInstance nationWarInst = nationInst.nationWarInst;
		if (nationWarInst != null)
		{
			ProgressAccomplishment_NationWarCommanderKillCount(time);
			nationWarInst.OnMonsterDead(this);
		}
	}

	private void ProgressAccomplishment_NationWarCommanderKillCount(DateTimeOffset time)
	{
		if (arrange.type != 1 || !(m_lastAttacker is Hero lastAttacker))
		{
			return;
		}
		lock (lastAttacker.syncObject)
		{
			if (!(lastAttacker.currentPlace.instanceId != m_currentPlace.instanceId) && IsQuestAreaPosition(lastAttacker.position))
			{
				lastAttacker.accNationWarCommanderKillCount++;
				ServerEvent.SendAccNationWarCommanderKillCountUpdated(lastAttacker.account.peer, lastAttacker.accNationWarCommanderKillCount);
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(lastAttacker.id);
				dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_NationWarCommanderKillCount(lastAttacker.id, lastAttacker.accNationWarCommanderKillCount));
				dbWork.Schedule();
			}
		}
	}

	public void IncreaseFinalDamagePenaltyRate()
	{
		if (arrangeType == 1)
		{
			m_nFinalDamagePenaltyRate = Math.Min(m_nFinalDamagePenaltyRate + 5000, 10000);
		}
	}

	public void DecreaseFinalDamagePenaltyRate()
	{
		if (arrangeType == 1)
		{
			m_nFinalDamagePenaltyRate = Math.Max(m_nFinalDamagePenaltyRate - 5000, 0);
		}
	}

	public void EnableMonsterHit()
	{
		if (arrangeType == 2)
		{
			m_bHitEnabled = true;
			m_bAbnormalStateHitEnabled = true;
		}
	}

	public bool IsEnableNpc(int nNpcId)
	{
		if (nNpcId != m_arrange.nationWarNpcId)
		{
			return true;
		}
		return false;
	}

	public PDSimpleNationWarMonsterInstance ToPDSimpleNationWarMonsterInstance()
	{
		PDSimpleNationWarMonsterInstance inst = new PDSimpleNationWarMonsterInstance();
		inst.monsterArrangeId = m_arrange.id;
		inst.nationId = nationId;
		inst.monsterMaxHp = m_nRealMaxHP;
		inst.monsterHp = m_nHP;
		inst.isBattleMode = m_bIsBattleMode;
		return inst;
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		PDNationWarMonsterInstance inst = new PDNationWarMonsterInstance();
		inst.arrangeId = arrangeId;
		return inst;
	}
}
