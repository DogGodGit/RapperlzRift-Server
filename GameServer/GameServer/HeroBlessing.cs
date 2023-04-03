using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroBlessing
{
	private long m_lnInstanceId;

	private Guid m_sendingLogId = Guid.Empty;

	private Hero m_owner;

	private Blessing m_blessing;

	private BlessingTargetLevel m_targetLevel;

	private Guid m_senderHeroId = Guid.Empty;

	private string m_sSenderName;

	public static readonly SFSynchronizedLongFactory instanceIdFactory = new SFSynchronizedLongFactory();

	public long instanceId => m_lnInstanceId;

	public Guid sendingLogId => m_sendingLogId;

	public Hero owner => m_owner;

	public Blessing blessing => m_blessing;

	public BlessingTargetLevel targetLevel => m_targetLevel;

	public Guid senderHeroId => m_senderHeroId;

	public string senderName => m_sSenderName;

	public HeroBlessing(Hero owner, Blessing blessing, BlessingTargetLevel targetLevel, Guid senderHeroId, string sSenderName)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		if (blessing == null)
		{
			throw new ArgumentNullException("blessing");
		}
		if (targetLevel == null)
		{
			throw new ArgumentNullException("targetLevel");
		}
		m_lnInstanceId = instanceIdFactory.NewValue();
		m_sendingLogId = Guid.NewGuid();
		m_owner = owner;
		m_blessing = blessing;
		m_targetLevel = targetLevel;
		m_senderHeroId = senderHeroId;
		m_sSenderName = sSenderName;
	}

	public PDHeroBlessing ToPDHeroBlessing()
	{
		PDHeroBlessing inst = new PDHeroBlessing();
		inst.instanceId = m_lnInstanceId;
		inst.blessingId = m_blessing.id;
		inst.blessingTargetLevelId = m_targetLevel.id;
		inst.senderHeroId = (Guid)m_senderHeroId;
		inst.senderName = m_sSenderName;
		return inst;
	}
}
