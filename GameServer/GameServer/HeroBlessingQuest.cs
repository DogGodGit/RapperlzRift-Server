using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroBlessingQuest
{
	private long m_lnId;

	private Hero m_owner;

	private Guid m_targetHeroId = Guid.Empty;

	private string m_sTargetName;

	private BlessingTargetLevel m_targetLevel;

	public static readonly SFSynchronizedLongFactory idFactory = new SFSynchronizedLongFactory();

	public long id => m_lnId;

	public Hero owner => m_owner;

	public Guid targetHeroId => m_targetHeroId;

	public string targetName => m_sTargetName;

	public BlessingTargetLevel targetLevel => m_targetLevel;

	public HeroBlessingQuest(Hero owner, Guid targetHeroId, string sTargetName, BlessingTargetLevel targetLevel)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		if (targetLevel == null)
		{
			throw new ArgumentNullException("targetLevel");
		}
		m_lnId = idFactory.NewValue();
		m_owner = owner;
		m_targetHeroId = targetHeroId;
		m_sTargetName = sTargetName;
		m_targetLevel = targetLevel;
	}

	public PDHeroBlessingQuest ToPDHeroBlessingQuest()
	{
		PDHeroBlessingQuest inst = new PDHeroBlessingQuest();
		inst.id = m_lnId;
		inst.targetHeroId = (Guid)m_targetHeroId;
		inst.targetName = m_sTargetName;
		inst.blessingTargetLevelId = m_targetLevel.id;
		return inst;
	}
}
