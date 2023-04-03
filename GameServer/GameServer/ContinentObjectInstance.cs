using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ContinentObjectInstance
{
	private ContinentObjectArrange m_arrange;

	private long m_lnInstanceId;

	private ContinentInstance m_currentContinentInst;

	private Sector m_sector;

	private Vector3 m_position = Vector3.zero;

	private bool m_bIsPublic;

	private Hero m_interactionHero;

	private bool m_bIsReleased;

	public static readonly SFSynchronizedLongFactory instanceIdFactory = new SFSynchronizedLongFactory();

	public ContinentObjectArrange arrange => m_arrange;

	public ContinentObject obj => m_arrange.obj;

	public long instanceId => m_lnInstanceId;

	public ContinentInstance currentContinentInst => m_currentContinentInst;

	public Sector sector => m_sector;

	public Vector3 position => m_position;

	public bool isPublic => m_bIsPublic;

	public Hero interactionHero
	{
		get
		{
			return m_interactionHero;
		}
		set
		{
			m_interactionHero = value;
		}
	}

	public bool isInteractionEnabled
	{
		get
		{
			if (m_interactionHero != null)
			{
				return false;
			}
			return true;
		}
	}

	public bool isReleased => m_bIsReleased;

	public ContinentObjectInstance()
	{
		m_lnInstanceId = instanceIdFactory.NewValue();
	}

	public void Init(ContinentInstance continentInst, ContinentObjectArrange arrange)
	{
		if (continentInst == null)
		{
			throw new ArgumentNullException("continentInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		m_currentContinentInst = continentInst;
		m_position = arrange.position;
		m_bIsPublic = arrange.obj.isPublic;
		AddToSector(m_currentContinentInst.GetSectorOfPosition(m_position));
	}

	private void AddToSector(Sector sector)
	{
		if (sector == null)
		{
			throw new ArgumentNullException("sector");
		}
		if (m_sector != null)
		{
			throw new Exception("sector");
		}
		sector.AddContinentObject(this);
		m_sector = sector;
	}

	public void RemoveFromSector()
	{
		if (m_sector != null)
		{
			m_sector.RemoveContinentObject(m_lnInstanceId);
		}
	}

	public void Release()
	{
		m_bIsReleased = true;
	}

	public bool IsInteractionEnabledPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_position, obj.interactionMaxRange * 1.1f + fRadius * 2f, position);
	}

	public PDContinentObjectInstance ToPDContinentObjectInstance()
	{
		PDContinentObjectInstance inst = new PDContinentObjectInstance();
		inst.instanceId = m_lnInstanceId;
		inst.arrangeNo = m_arrange.no;
		inst.interactionHeroId = (Guid)((m_interactionHero != null) ? m_interactionHero.id : Guid.Empty);
		return inst;
	}
}
