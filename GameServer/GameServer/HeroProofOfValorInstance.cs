using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroProofOfValorInstance
{
	public const int kStatus_Init = -1;

	public const int kStatus_Creation = 0;

	public const int kStatus_Start = 1;

	public const int kStatus_Clear = 2;

	public const int kStatus_Fail = 3;

	public const int kStatus_Disqualification = 4;

	public const int kStatus_Sweep = 5;

	public const int kRefreshLogType_Free = 1;

	public const int kRefreshLogType_Paid = 2;

	public const int kRewardLogStatus_Clear = 1;

	public const int kRewardLogStatus_Fail = 2;

	public const int kRewardLogStatus_Disqualification = 3;

	public const int kRewardLogStatus_Sweep = 4;

	private Guid m_id = Guid.Empty;

	private Hero m_hero;

	private ProofOfValorBossMonsterArrange m_bossMonsterArrange;

	private CreatureCard m_creatureCard;

	private int m_nStatus = -1;

	private int m_nLevel;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	public Guid id => m_id;

	public Hero hero => m_hero;

	public ProofOfValorBossMonsterArrange bossMonsterArrange => m_bossMonsterArrange;

	public CreatureCard creatureCard => m_creatureCard;

	public int status
	{
		get
		{
			return m_nStatus;
		}
		set
		{
			m_nStatus = value;
		}
	}

	public int level
	{
		get
		{
			return m_nLevel;
		}
		set
		{
			m_nLevel = value;
		}
	}

	public DateTimeOffset regTime => m_regTime;

	public HeroProofOfValorInstance(Hero hero)
	{
		m_hero = hero;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		Resource res = Resource.instance;
		m_id = SFDBUtil.ToGuid(dr["instanceId"]);
		int nProofOfValorBossMonsterArrangeId = Convert.ToInt32(dr["proofOfValorBossMonsterArrangeId"]);
		m_bossMonsterArrange = res.proofOfValor.GetBossMonaterArrange(nProofOfValorBossMonsterArrangeId);
		if (m_bossMonsterArrange == null)
		{
			throw new Exception("nProofOfValorBossMonsterArrangeId = " + nProofOfValorBossMonsterArrangeId);
		}
		int nCreatureCardId = Convert.ToInt32(dr["creatureCardId"]);
		m_creatureCard = res.GetCreatureCard(nCreatureCardId);
		if (m_creatureCard == null)
		{
			throw new Exception("nCreatureCardId = " + nCreatureCardId);
		}
		m_nStatus = Convert.ToInt32(dr["status"]);
		m_nLevel = Convert.ToInt32(dr["level"]);
		m_regTime = SFDBUtil.ToDateTimeOffset(dr["regTime"], DateTimeOffset.MinValue);
	}

	public void Init(ProofOfValorBossMonsterArrange bossMonsterArrange, DateTimeOffset regTime)
	{
		if (bossMonsterArrange == null)
		{
			throw new ArgumentNullException("bossMonsterArrange");
		}
		m_id = Guid.NewGuid();
		m_bossMonsterArrange = bossMonsterArrange;
		m_creatureCard = bossMonsterArrange.creatureCardPool.SelectCreatureCard();
		m_nStatus = 0;
		m_regTime = regTime;
	}

	public PDHeroProofOfValorInstance ToPDHeroProofOfValorInstance()
	{
		PDHeroProofOfValorInstance inst = new PDHeroProofOfValorInstance();
		inst.bossMonsterArrangeId = m_bossMonsterArrange.id;
		inst.creatureCardId = m_creatureCard.id;
		return inst;
	}
}
