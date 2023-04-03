using System;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class AbnormalStateEffect
{
	private long m_lnId;

	private Unit m_target;

	private Unit m_source;

	private AbnormalStateLevel m_abnormalStateLevel;

	private int m_nSourceAbnormalStateOffense;

	private int m_nHitCount;

	private int m_nAdditionalDuration;

	private int m_nUsedAdditionalDuration;

	private int m_nDamageAbsorbShieldRemainingAmount;

	private int m_nImmortalityCount;

	private int m_nHPRestoreAmount;

	private bool m_bRunning;

	private Timer m_timer;

	private int m_nCurrentHitCount;

	private float m_fDuration;

	private DateTimeOffset m_effectStartTime = DateTimeOffset.MinValue;

	public static readonly SFSynchronizedLongFactory idFactory = new SFSynchronizedLongFactory();

	public long id => m_lnId;

	public Unit target => m_target;

	public Unit source => m_source;

	public AbnormalStateLevel abnormalStateLevel => m_abnormalStateLevel;

	public AbnormalState abnormalState => abnormalStateLevel.abnormalState;

	public DateTimeOffset effectStartTime => m_effectStartTime;

	public int sourceJobId
	{
		get
		{
			if (abnormalState.sourceType != 1)
			{
				return 0;
			}
			return ((AbnormalStateJobSkillLevel)m_abnormalStateLevel).jobAbnormalState.job.id;
		}
	}

	public int sourceAbnormalStateOffense => m_nSourceAbnormalStateOffense;

	public int damageAbsorbShieldRemainingAmount => m_nDamageAbsorbShieldRemainingAmount;

	public int hpRestoreAmount => m_nHPRestoreAmount;

	public bool running => m_bRunning;

	public float duration => m_fDuration;

	public AbnormalStateEffect()
	{
		m_lnId = idFactory.NewValue();
	}

	public void Init(Unit source, Unit target, AbnormalStateLevel abnormalStateLevel, DateTimeOffset currentTime)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (abnormalStateLevel == null)
		{
			throw new ArgumentNullException("abnormalStateLevel");
		}
		m_source = source;
		m_target = target;
		m_abnormalStateLevel = abnormalStateLevel;
		int nAbnormalStateDuration = abnormalStateLevel.duration;
		m_fDuration = nAbnormalStateDuration;
		m_effectStartTime = currentTime;
		int nDuration = Math.Max(nAbnormalStateDuration * 1000, 0);
		if (nDuration == 0)
		{
			throw new Exception("지속시간이 유효하지 않습니다.");
		}
		if (abnormalState.id == 10)
		{
			m_nDamageAbsorbShieldRemainingAmount = abnormalStateLevel.value1;
		}
		else if (abnormalState.id == 106)
		{
			m_nImmortalityCount = 1;
		}
		else if (abnormalState.id == 102)
		{
			m_nDamageAbsorbShieldRemainingAmount = (int)Math.Floor((float)source.realMaxHP * ((float)abnormalStateLevel.value1 / 10000f));
		}
		else if (abnormalState.id == 109)
		{
			m_nDamageAbsorbShieldRemainingAmount = (int)Math.Floor((float)source.realMaxHP * ((float)abnormalStateLevel.value5 / 10000f));
		}
		if (!abnormalState.isTickPerSecondAbnormalState)
		{
			m_timer = new Timer(OnTimerTick);
			m_timer.Change(nDuration, -1);
			m_bRunning = true;
			return;
		}
		if (abnormalState.id == 3)
		{
			m_nSourceAbnormalStateOffense = (int)Math.Floor((float)m_source.realPhysicalOffense * ((float)m_abnormalStateLevel.value1 / 10000f));
		}
		else if (abnormalState.id == 4)
		{
			m_nSourceAbnormalStateOffense = (int)Math.Floor((float)m_source.realMagicalOffense * ((float)m_abnormalStateLevel.value1 / 10000f));
		}
		else
		{
			if (abnormalState.id != 11)
			{
				throw new Exception("상태이상이 유효하지 않습니다.");
			}
			m_nHPRestoreAmount = (int)Math.Floor((float)m_target.realMaxHP * ((float)m_abnormalStateLevel.value2 / 10000f));
		}
		int nAbnormalStateHitInterval = 1;
		int nHitInterval = nAbnormalStateHitInterval * 1000;
		m_nHitCount = nAbnormalStateDuration / nAbnormalStateHitInterval;
		if (m_nHitCount <= 0)
		{
			throw new Exception("상태이상 적중횟수가 유효하지 않습니다.");
		}
		m_timer = new Timer(OnTimerTick);
		m_timer.Change(nHitInterval, nHitInterval);
		m_bRunning = true;
	}

	private void OnTimerTick(object state)
	{
		m_target.AddWork(new SFAction(ProcessTimerTick), bGlobalLockRequired: false);
	}

	private void ProcessTimerTick()
	{
		if (!m_bRunning)
		{
			return;
		}
		if (abnormalState.id == 104 && m_nAdditionalDuration > 0)
		{
			m_timer.Change(m_nAdditionalDuration * 1000, -1);
			m_nUsedAdditionalDuration += m_nAdditionalDuration;
			m_nAdditionalDuration = 0;
			return;
		}
		if (!abnormalState.isTickPerSecondAbnormalState)
		{
			Finish();
			return;
		}
		m_nCurrentHitCount++;
		m_target.OnAbnormalStateEffectTick(this);
		if (m_nCurrentHitCount >= m_nHitCount)
		{
			Finish();
		}
	}

	private void Finish()
	{
		DisposeTimer();
		m_bRunning = false;
		m_target.ProcessAbnormalStateEffectFinished(this);
	}

	private void DisposeTimer()
	{
		if (m_timer != null)
		{
			m_timer.Dispose();
			m_timer = null;
		}
	}

	public void Stop()
	{
		if (m_bRunning)
		{
			DisposeTimer();
			m_bRunning = false;
		}
	}

	public void AddDuration()
	{
		if (abnormalState.id == 104 && m_nAdditionalDuration + m_nUsedAdditionalDuration < 2)
		{
			m_nAdditionalDuration++;
		}
	}

	public bool UseImmortalityEffect()
	{
		if (abnormalState.id != 106)
		{
			return false;
		}
		if (m_nImmortalityCount <= 0)
		{
			return false;
		}
		m_nImmortalityCount--;
		return true;
	}

	public int AbsorbDamage(int nDamage)
	{
		if (m_nDamageAbsorbShieldRemainingAmount <= 0)
		{
			return 0;
		}
		int nAbsorbDamage = Math.Min(m_nDamageAbsorbShieldRemainingAmount, nDamage);
		m_nDamageAbsorbShieldRemainingAmount -= nAbsorbDamage;
		return nDamage - nAbsorbDamage;
	}

	public PDAbnormalStateEffect ToPDAbnormalStateEffect(DateTimeOffset currentTime)
	{
		PDAbnormalStateEffect inst = new PDAbnormalStateEffect();
		inst.instanceId = m_lnId;
		inst.abnormalStateId = abnormalState.id;
		inst.sourceJobId = sourceJobId;
		inst.level = m_abnormalStateLevel.level;
		inst.remainingTime = Math.Max((float)((double)m_fDuration - (currentTime - m_effectStartTime).TotalSeconds), 0f);
		inst.damageAbsorbShieldRemainingAbsorbAmount = m_nDamageAbsorbShieldRemainingAmount;
		return inst;
	}

	public int CompareTo(AbnormalStateEffect other)
	{
		if (other == null)
		{
			return -1;
		}
		return m_effectStartTime.CompareTo(other.effectStartTime);
	}

	public static int Compare(AbnormalStateEffect x, AbnormalStateEffect y)
	{
		if (x == null)
		{
			if (y != null)
			{
				return 1;
			}
			return 0;
		}
		return x.CompareTo(y);
	}
}
