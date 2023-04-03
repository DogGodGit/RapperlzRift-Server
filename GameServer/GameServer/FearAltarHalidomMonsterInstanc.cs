using System;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class FearAltarHalidomMonsterInstance : MonsterInstance
{
	private FearAltarHalidom m_halidom;

	private FearAltarStageWave m_wave;

	private FearAltarMonsterAttrFactor m_attrFactor;

	private Timer m_lifetimeTimer;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.FearAltarHalidomMonster;

	public override Monster monster => m_halidom.monsterArrange.monster;

	public FearAltarHalidom halidom => m_halidom;

	public FearAltarStageWave wave => m_wave;

	public void Init(FearAltarInstance fearAltarInst, FearAltarHalidom halidom, FearAltarStageWave wave, FearAltarMonsterAttrFactor attrFactor)
	{
		if (fearAltarInst == null)
		{
			throw new ArgumentNullException("fearAltarInst");
		}
		if (halidom == null)
		{
			throw new ArgumentNullException("halidom");
		}
		if (wave == null)
		{
			throw new ArgumentNullException("wave");
		}
		if (attrFactor == null)
		{
			throw new ArgumentNullException("attrFactor");
		}
		m_halidom = halidom;
		m_wave = wave;
		m_attrFactor = attrFactor;
		InitMonsterInstance(fearAltarInst, wave.halidomMonsterPosition, wave.SelectHalidomMonsterRotationY());
		int nLifeTime = fearAltarInst.fearAltar.halidomMonsterLifetime * 1000;
		m_lifetimeTimer = new Timer(OnLitetimeTimerTick);
		m_lifetimeTimer.Change(nLifeTime, -1);
	}

	private void OnLitetimeTimerTick(object state)
	{
		m_currentPlace.AddWork(new SFAction(ExpireLifetime), bGlobalLockRequired: false);
	}

	private void ExpireLifetime()
	{
		if (!m_bReleased)
		{
			((FearAltarInstance)m_currentPlace).OnExpireHalidomMonsterLifetime(this);
		}
	}

	protected override void RefreshRealValues_Multiplication()
	{
		base.RefreshRealValues_Multiplication();
		m_nRealMaxHP = (int)Math.Floor((float)m_nRealMaxHP * m_attrFactor.offenseFactor);
		m_nRealPhysicalOffense = (int)Math.Floor((float)m_nRealPhysicalOffense * m_attrFactor.offenseFactor);
	}

	protected override void OnDead()
	{
		base.OnDead();
		FearAltarInstance fearAltarInst = (FearAltarInstance)m_currentPlace;
		DateTime dungeonCreationWeekStartTime = DateTimeUtil.GetWeekStartDate(fearAltarInst.creationTime);
		if (dungeonCreationWeekStartTime != DateTimeUtil.GetWeekStartDate(m_lastDamageTime))
		{
			return;
		}
		int nHalidomAcquisitionRate = m_halidom.fearAltar.halidomAcquisitionRate;
		foreach (MonsterReceivedDamage damage in m_receivedDamages.Values)
		{
			Hero hero = m_currentPlace.GetHero(damage.attackerId);
			if (hero == null)
			{
				continue;
			}
			lock (hero.syncObject)
			{
				if (!hero.ContainsFearAltarHalidom(m_halidom.id) && Util.DrawLots(nHalidomAcquisitionRate))
				{
					hero.AcquireFearAltarHalidom(this, m_lastDamageTime);
				}
			}
		}
	}

	private void DisposeLifetimeTimer()
	{
		if (m_lifetimeTimer != null)
		{
			m_lifetimeTimer.Dispose();
			m_lifetimeTimer = null;
		}
	}

	protected override void ReleaseInternal()
	{
		DisposeLifetimeTimer();
		base.ReleaseInternal();
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		PDFearAltarHalidomMonsterInstance inst = new PDFearAltarHalidomMonsterInstance();
		inst.halidomId = m_halidom.id;
		return inst;
	}
}
