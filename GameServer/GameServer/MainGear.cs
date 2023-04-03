using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MainGear
{
	private int m_nId;

	private MainGearType m_type;

	private string m_sNameKey;

	private Job m_job;

	private MainGearTier m_tier;

	private MainGearGrade m_grade;

	private MainGearQuality m_quality;

	private int m_nSaleGold;

	private Dictionary<int, MainGearBaseAttr> m_baseAttrs = new Dictionary<int, MainGearBaseAttr>();

	public int id => m_nId;

	public MainGearType type => m_type;

	public MainGearCategory category => m_type.category;

	public string nameKey => m_sNameKey;

	public Job job => m_job;

	public int jobId
	{
		get
		{
			if (m_job == null)
			{
				return 0;
			}
			return m_job.id;
		}
	}

	public MainGearTier tier => m_tier;

	public MainGearGrade grade => m_grade;

	public MainGearQuality quality => m_quality;

	public int saleGold => m_nSaleGold;

	public IEnumerable<MainGearBaseAttr> baseAttrs => m_baseAttrs.Values;

	public bool isWeapon => m_type.category.id == 1;

	public bool isArmor => m_type.category.id == 2;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["mainGearId"]);
		int nType = Convert.ToInt32(dr["mainGearType"]);
		m_type = Resource.instance.GetMainGearType(nType);
		if (m_type == null)
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다 nType = " + nType);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		int nJobId = Convert.ToInt32(dr["jobId"]);
		if (nJobId > 0)
		{
			m_job = Resource.instance.GetJob(nJobId);
		}
		if (isWeapon)
		{
			if (m_job == null)
			{
				SFLogUtil.Warn(GetType(), "Weapon : 직업이 존재하지 않습니다. nJobId = " + nJobId);
			}
		}
		else if (m_job != null)
		{
			SFLogUtil.Warn(GetType(), "Armor : 갑옷은 공통직업입니다. nJobId = " + nJobId);
		}
		int nTier = Convert.ToInt32(dr["tier"]);
		m_tier = Resource.instance.GetMainGearTier(nTier);
		if (m_tier == null)
		{
			SFLogUtil.Warn(GetType(), "티어가 존재하지 않습니다. nTier = " + nTier);
		}
		int nGrade = Convert.ToInt32(dr["grade"]);
		m_grade = Resource.instance.GetMainGearGrade(nGrade);
		if (m_grade == null)
		{
			SFLogUtil.Warn(GetType(), "등급이 존재하지 않습니다. nGrade = " + nGrade);
		}
		int nQuality = Convert.ToInt32(dr["quality"]);
		m_quality = Resource.instance.GetMainGearQuality(nQuality);
		if (m_quality == null)
		{
			SFLogUtil.Warn(GetType(), "품질이 존재하지 않습니다. nQuality = " + nQuality);
		}
		m_nSaleGold = Convert.ToInt32(dr["saleGold"]);
	}

	public void AddBaseAttr(MainGearBaseAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		if (attr.gear != null)
		{
			throw new Exception("이미 메인장비에 추가된 기본속성입니다.");
		}
		m_baseAttrs.Add(attr.id, attr);
		attr.gear = this;
	}

	public MainGearBaseAttr GetBaseAttr(int nId)
	{
		if (!m_baseAttrs.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}
}
