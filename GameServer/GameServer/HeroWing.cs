using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroWing
{
	private Hero m_hero;

	private Wing m_wing;

	private int m_nMemoryPieceStep;

	private List<HeroWingMemoryPieceSlot> m_memoryPieceSlots = new List<HeroWingMemoryPieceSlot>();

	private Dictionary<int, AttrValuePair> m_attrTotalValues = new Dictionary<int, AttrValuePair>();

	public Hero hero => m_hero;

	public Wing wing => m_wing;

	public bool isEquipped => m_hero.equippedWingId == m_wing.id;

	public int memoryPieceStep
	{
		get
		{
			return m_nMemoryPieceStep;
		}
		set
		{
			m_nMemoryPieceStep = value;
		}
	}

	public bool isMemoryPieceLastStep => m_nMemoryPieceStep >= m_wing.lastMemoryPieceStep.step;

	public List<HeroWingMemoryPieceSlot> memoryPieceSlots => m_memoryPieceSlots;

	public bool isMemoryPieceSlotAllInstalled
	{
		get
		{
			foreach (HeroWingMemoryPieceSlot slot in m_memoryPieceSlots)
			{
				if (slot.isOpend && !slot.isCompleted)
				{
					return false;
				}
			}
			return true;
		}
	}

	public Dictionary<int, AttrValuePair> attrTotalValues => m_attrTotalValues;

	public HeroWing(Hero hero)
		: this(hero, null)
	{
	}

	public HeroWing(Hero hero, Wing wing)
	{
		m_hero = hero;
		m_wing = wing;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nWingId = Convert.ToInt32(dr["wingId"]);
		m_wing = Resource.instance.GetWing(nWingId);
		if (m_wing == null)
		{
			throw new Exception("존재하지 않는 영웅날개입니다.");
		}
		m_nMemoryPieceStep = Convert.ToInt32(dr["memoryPieceStep"]);
		InitMemoryPieceSlot();
	}

	public void Init()
	{
		if (m_wing.memoryPieceInstallationEnabled)
		{
			m_nMemoryPieceStep = 1;
			InitMemoryPieceSlot();
		}
	}

	public void InitMemoryPieceSlot()
	{
		if (!m_wing.memoryPieceInstallationEnabled)
		{
			return;
		}
		foreach (WingMemoryPieceSlot slot in m_wing.memoryPieceSlots)
		{
			HeroWingMemoryPieceSlot heroSlot = new HeroWingMemoryPieceSlot(this, slot);
			AddMemoryPieceSlot(heroSlot);
		}
	}

	public void AddMemoryPieceSlot(HeroWingMemoryPieceSlot slot)
	{
		if (slot == null)
		{
			throw new ArgumentNullException("slot");
		}
		m_memoryPieceSlots.Add(slot);
	}

	public HeroWingMemoryPieceSlot GetMemoryPieceSlot(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_memoryPieceSlots.Count)
		{
			return null;
		}
		return m_memoryPieceSlots[nIndex];
	}

	public List<HeroWingMemoryPieceSlot> SelectMemoryPieceSlots(int nCount)
	{
		List<HeroWingMemoryPieceSlot> slotPool = new List<HeroWingMemoryPieceSlot>();
		int nTotalPickPoint = 0;
		foreach (HeroWingMemoryPieceSlot slot in m_memoryPieceSlots)
		{
			if (slot.isOpend && !slot.isCompleted)
			{
				slotPool.Add(slot);
				nTotalPickPoint += slot.point;
			}
		}
		return Util.SelectPickEntries(slotPool, nTotalPickPoint, nCount, bDuplicated: false);
	}

	private AttrValuePair GetAttrTotalValue(int nAttrId)
	{
		if (!m_attrTotalValues.TryGetValue(nAttrId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddAttrTotalValue(int nAttrId, int nValue)
	{
		AttrValuePair attrValue = GetAttrTotalValue(nAttrId);
		if (attrValue == null)
		{
			attrValue = new AttrValuePair(nAttrId, 0);
			m_attrTotalValues.Add(nAttrId, attrValue);
		}
		attrValue.value += nValue;
	}

	private void ClearAttrTotalValues()
	{
		m_attrTotalValues.Clear();
	}

	public void RefreshAttrTotalValues()
	{
		ClearAttrTotalValues();
		RefreshAttrTotalValues_Sum();
	}

	private void RefreshAttrTotalValues_Sum()
	{
		foreach (HeroWingMemoryPieceSlot heroWingSlot in m_memoryPieceSlots)
		{
			WingMemoryPieceSlot slot = heroWingSlot.slot;
			AddAttrTotalValue(slot.attrId, heroWingSlot.accAttrValue);
		}
	}

	public FieldOfHonorHeroWing ToFieldOfHonorHeroWing(FieldOfHonorHero fieldOfHonorHero)
	{
		FieldOfHonorHeroWing inst = new FieldOfHonorHeroWing(fieldOfHonorHero);
		inst.id = m_wing.id;
		return inst;
	}

	public PDHeroWing ToPDHeroWing()
	{
		PDHeroWing inst = new PDHeroWing();
		inst.wingId = m_wing.id;
		inst.memoryPieceStep = m_nMemoryPieceStep;
		inst.memoryPieceSlots = HeroWingMemoryPieceSlot.ToPDHeroWingMemoryPieceSlots(m_memoryPieceSlots).ToArray();
		return inst;
	}

	public static List<PDHeroWing> ToPDHeroWings(IEnumerable<HeroWing> wings)
	{
		List<PDHeroWing> insts = new List<PDHeroWing>();
		foreach (HeroWing wing in wings)
		{
			insts.Add(wing.ToPDHeroWing());
		}
		return insts;
	}
}
