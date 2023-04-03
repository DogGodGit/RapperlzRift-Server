namespace GameServer;

public class Offense
{
	private Unit m_attacker;

	private Skill m_skill;

	private int m_nSkillLevel;

	private OffenseType m_offenseType;

	private int m_nElementalId;

	private int m_nOffenseAmp;

	private int m_nOffensePoint;

	private int m_nPower;

	private int m_nCritical;

	private int m_nCriticalDamageIncRate;

	private int m_nPenetration;

	private int m_nFireOffense;

	private int m_nLightningOffense;

	private int m_nDarkOffense;

	private int m_nHolyOffense;

	private int m_nDamageIncRate;

	public Unit attacker => m_attacker;

	public Skill skill => m_skill;

	public int skillLevel => m_nSkillLevel;

	public OffenseType offenseType => m_offenseType;

	public int elementalId => m_nElementalId;

	public int offenseAmp => m_nOffenseAmp;

	public int offensePoint => m_nOffensePoint;

	public int power => m_nPower;

	public int critical => m_nCritical;

	public int criticalDamageIncRate => m_nCriticalDamageIncRate;

	public int penetration => m_nPenetration;

	public int fireOffense => m_nFireOffense;

	public int lightningOffense => m_nLightningOffense;

	public int darkOffense => m_nDarkOffense;

	public int holyOffense => m_nHolyOffense;

	public int damageIncRate => m_nDamageIncRate;

	public Offense(Unit attacker, Skill skill, int nSkillLevel, OffenseType offenseType, int nElementalId, int nOffenseAmp, int nOffensePoint)
	{
		m_attacker = attacker;
		m_skill = skill;
		m_nSkillLevel = nSkillLevel;
		m_offenseType = offenseType;
		m_nElementalId = nElementalId;
		m_nOffenseAmp = nOffenseAmp;
		m_nOffensePoint = nOffensePoint;
		m_nPower = ((m_offenseType == OffenseType.Physical) ? m_attacker.realPhysicalOffense : m_attacker.realMagicalOffense);
		m_nCritical = m_attacker.realCritical;
		m_nCriticalDamageIncRate = m_attacker.realCriticalDamageIncRate;
		m_nPenetration = m_attacker.realPenetration;
		m_nFireOffense = m_attacker.realFireOffense;
		m_nLightningOffense = m_attacker.realLightningOffense;
		m_nDarkOffense = m_attacker.realDarkOffense;
		m_nHolyOffense = m_attacker.realHolyOffense;
		m_nDamageIncRate = m_attacker.realDamageIncRate;
	}

	public Offense(Unit attacker, Skill skill, int nSkillLevel, OffenseType offenseType, int nElementalId)
		: this(attacker, skill, nSkillLevel, offenseType, nElementalId, 10000, 0)
	{
	}
}
