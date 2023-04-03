using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class FieldOfHonorHeroEquippedSubGear
{
	private FieldOfHonorHero m_hero;

	private SubGear m_subGear;

	private int m_nLevel;

	private int m_nQuality;

	private Dictionary<int, FieldOfHonorHeroSubGearRuneSocket> m_runeSockets = new Dictionary<int, FieldOfHonorHeroSubGearRuneSocket>();

	private Dictionary<int, FieldOfHonorHeroSubGearSoulstoneSocket> m_soulstoneSockets = new Dictionary<int, FieldOfHonorHeroSubGearSoulstoneSocket>();

	private Dictionary<int, AttrValuePair> m_attrTotalValues = new Dictionary<int, AttrValuePair>();

	private long m_lnBattlePower;

	public FieldOfHonorHero hero => m_hero;

	public SubGear subGear
	{
		get
		{
			return m_subGear;
		}
		set
		{
			m_subGear = value;
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

	public int quality
	{
		get
		{
			return m_nQuality;
		}
		set
		{
			m_nQuality = value;
		}
	}

	public Dictionary<int, FieldOfHonorHeroSubGearRuneSocket> runeSockets => m_runeSockets;

	public Dictionary<int, FieldOfHonorHeroSubGearSoulstoneSocket> soulstoneSockets => m_soulstoneSockets;

	public Dictionary<int, AttrValuePair> attrTotalValues => m_attrTotalValues;

	public long battlePower => m_lnBattlePower;

	public FieldOfHonorHeroEquippedSubGear(FieldOfHonorHero hero)
	{
		m_hero = hero;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nSubGearId = Convert.ToInt32(dr["subGearId"]);
		if (nSubGearId > 0)
		{
			m_subGear = Resource.instance.GetSubGear(nSubGearId);
			if (m_subGear == null)
			{
				SFLogUtil.Warn(GetType(), string.Concat("보조장비가 존재하지 않습니다. heroId = ", m_hero.id, ", nSubGearId = ", nSubGearId));
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), string.Concat("보조장비ID가 유효하지 않습니다. heroId = ", m_hero.id, ", nSubGearId = ", nSubGearId));
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		m_nQuality = Convert.ToInt32(dr["quality"]);
		RefreshAttrTotalValues();
	}

	public void AddRuneSocket(FieldOfHonorHeroSubGearRuneSocket runeSocket)
	{
		if (runeSocket == null)
		{
			throw new ArgumentNullException("runeSocket");
		}
		m_runeSockets.Add(runeSocket.index, runeSocket);
	}

	public FieldOfHonorHeroSubGearRuneSocket GetRuneSocket(int nSocketIndex)
	{
		if (!m_runeSockets.TryGetValue(nSocketIndex, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddSoulstoneSocket(FieldOfHonorHeroSubGearSoulstoneSocket soulstoneSocket)
	{
		if (soulstoneSocket == null)
		{
			throw new ArgumentNullException("soulstoneSocket");
		}
		m_soulstoneSockets.Add(soulstoneSocket.index, soulstoneSocket);
	}

	public FieldOfHonorHeroSubGearSoulstoneSocket GetSoulstoneSocket(int nIndex)
	{
		if (!m_soulstoneSockets.TryGetValue(nIndex, out var value))
		{
			return null;
		}
		return value;
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
		RefreshBattlePower();
	}

	private void RefreshAttrTotalValues_Sum()
	{
		SubGearLevelQuality quality = m_subGear.GetLevel(m_nLevel).GetQuality(m_nQuality);
		foreach (SubGearAttrValue levelAttr in quality.attrValues)
		{
			AddAttrTotalValue(levelAttr.id, levelAttr.value);
		}
		foreach (FieldOfHonorHeroSubGearRuneSocket socket2 in m_runeSockets.Values)
		{
			if (socket2.item != null)
			{
				Item socketItem2 = socket2.item;
				int nAttrId2 = socketItem2.value1;
				AttrValue attrValue2 = Resource.instance.GetAttrValue(socketItem2.longValue1);
				AddAttrTotalValue(nAttrId2, attrValue2.value);
			}
		}
		foreach (FieldOfHonorHeroSubGearSoulstoneSocket socket in m_soulstoneSockets.Values)
		{
			if (socket.item != null)
			{
				Item socketItem = socket.item;
				int nAttrId = socketItem.value1;
				AttrValue attrValue = Resource.instance.GetAttrValue(socketItem.longValue1);
				AddAttrTotalValue(nAttrId, attrValue.value);
			}
		}
	}

	private void RefreshBattlePower()
	{
		m_lnBattlePower = Util.CalcBattlePower(m_attrTotalValues.Values);
	}

	public PDFullHeroSubGear ToPDFullHeroSubGear()
	{
		PDFullHeroSubGear inst = new PDFullHeroSubGear();
		inst.subGearId = m_subGear.id;
		inst.level = m_nLevel;
		inst.quality = m_nQuality;
		inst.equipped = true;
		List<PDHeroSubGearSoulstoneSocket> pdSoulStoneSockets = new List<PDHeroSubGearSoulstoneSocket>();
		foreach (FieldOfHonorHeroSubGearSoulstoneSocket soulstoneSocket in m_soulstoneSockets.Values)
		{
			pdSoulStoneSockets.Add(soulstoneSocket.ToPDHeroSubGearSoulstoneSocket());
		}
		inst.equippedSoulstoneSockets = pdSoulStoneSockets.ToArray();
		List<PDHeroSubGearRuneSocket> pdRuneSockets = new List<PDHeroSubGearRuneSocket>();
		foreach (FieldOfHonorHeroSubGearRuneSocket runeSocket in m_runeSockets.Values)
		{
			pdRuneSockets.Add(runeSocket.ToPDHeroSubGearRuneSocket());
		}
		inst.equippedRuneSockets = pdRuneSockets.ToArray();
		return inst;
	}
}
