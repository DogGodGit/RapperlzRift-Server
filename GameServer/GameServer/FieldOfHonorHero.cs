using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class FieldOfHonorHero
{
	private Guid m_accountId = Guid.Empty;

	private int m_nRanking;

	private Guid m_id = Guid.Empty;

	private Job m_job;

	private Nation m_nation;

	private string m_sName;

	private int m_nLevel;

	private long m_lnBattlePower;

	private int m_nRankId;

	private int m_nEquippedWingId;

	private int m_nWingStep;

	private int m_nWingLevel;

	private int m_nDisplayTitleId;

	private Guid m_guildId = Guid.Empty;

	private string m_sGuildName;

	private int m_nGuildMemberGrade;

	private int m_nMainGearEnchantLevelSetNo;

	private int m_nSubGearSoulstoneLevelSetNo;

	private int m_nCustomPresetHair;

	private int m_nCustomFaceJawHeight;

	private int m_nCustomFaceJawWidth;

	private int m_nCustomFaceJawEndHeight;

	private int m_nCustomFaceWidth;

	private int m_nCustomFaceEyebrowHeight;

	private int m_nCustomFaceEyebrowRotation;

	private int m_nCustomFaceEyesWidth;

	private int m_nCustomFaceNoseHeight;

	private int m_nCustomFaceNoseWidth;

	private int m_nCustomFaceMouthHeight;

	private int m_nCustomFaceMouthWidth;

	private int m_nCustomBodyHeadSize;

	private int m_nCustomBodyArmsLength;

	private int m_nCustomBodyArmsWidth;

	private int m_nCustomBodyChestSize;

	private int m_nCustomBodyWaistWidth;

	private int m_nCustomBodyHipsSize;

	private int m_nCustomBodyPelvisWidth;

	private int m_nCustomBodyLegsLength;

	private int m_nCustomBodyLegsWidth;

	private int m_nCustomColorSkin;

	private int m_nCustomColorEyes;

	private int m_nCustomColorBeardAndEyebrow;

	private int m_nCustomColorHair;

	private int m_nCostumeId;

	private int m_nCostumeEffectId;

	private Dictionary<int, FieldOfHonorHeroRealAttr> m_realAttrs = new Dictionary<int, FieldOfHonorHeroRealAttr>();

	private Dictionary<int, FieldOfHonorHeroSkill> m_skills = new Dictionary<int, FieldOfHonorHeroSkill>();

	private Dictionary<int, FieldOfHonorHeroWing> m_wings = new Dictionary<int, FieldOfHonorHeroWing>();

	private List<FieldOfHonorHeroWingPart> m_wingParts = new List<FieldOfHonorHeroWingPart>();

	private FieldOfHonorHeroEquippedMainGear m_equippedWeapon;

	private FieldOfHonorHeroEquippedMainGear m_equippedArmor;

	private Dictionary<int, FieldOfHonorHeroEquippedSubGear> m_equippedSubGears = new Dictionary<int, FieldOfHonorHeroEquippedSubGear>();

	public Guid accountId
	{
		get
		{
			return m_accountId;
		}
		set
		{
			m_accountId = value;
		}
	}

	public int ranking
	{
		get
		{
			return m_nRanking;
		}
		set
		{
			m_nRanking = value;
		}
	}

	public Guid id
	{
		get
		{
			return m_id;
		}
		set
		{
			m_id = value;
		}
	}

	public Job job
	{
		get
		{
			return m_job;
		}
		set
		{
			m_job = value;
		}
	}

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

	public Nation nation
	{
		get
		{
			return m_nation;
		}
		set
		{
			m_nation = value;
		}
	}

	public int nationId
	{
		get
		{
			if (m_nation == null)
			{
				return 0;
			}
			return m_nation.id;
		}
	}

	public string name
	{
		get
		{
			return m_sName;
		}
		set
		{
			m_sName = value;
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

	public long battlePower
	{
		get
		{
			return m_lnBattlePower;
		}
		set
		{
			m_lnBattlePower = value;
		}
	}

	public int rankId
	{
		get
		{
			return m_nRankId;
		}
		set
		{
			m_nRankId = value;
		}
	}

	public int equippedWingId
	{
		get
		{
			return m_nEquippedWingId;
		}
		set
		{
			m_nEquippedWingId = value;
		}
	}

	public int wingStep
	{
		get
		{
			return m_nWingStep;
		}
		set
		{
			m_nWingStep = value;
		}
	}

	public int wingLevel
	{
		get
		{
			return m_nWingLevel;
		}
		set
		{
			m_nWingLevel = value;
		}
	}

	public int displayTitleId
	{
		get
		{
			return m_nDisplayTitleId;
		}
		set
		{
			m_nDisplayTitleId = value;
		}
	}

	public Guid guildId
	{
		get
		{
			return m_guildId;
		}
		set
		{
			m_guildId = value;
		}
	}

	public string guildName
	{
		get
		{
			return m_sGuildName;
		}
		set
		{
			m_sGuildName = value;
		}
	}

	public int guildMemberGrade
	{
		get
		{
			return m_nGuildMemberGrade;
		}
		set
		{
			m_nGuildMemberGrade = value;
		}
	}

	public int mainGearEnchantLevelSetNo
	{
		get
		{
			return m_nMainGearEnchantLevelSetNo;
		}
		set
		{
			m_nMainGearEnchantLevelSetNo = value;
		}
	}

	public int subGearSoulstoneLevelSetNo
	{
		get
		{
			return m_nSubGearSoulstoneLevelSetNo;
		}
		set
		{
			m_nSubGearSoulstoneLevelSetNo = value;
		}
	}

	public int customPresetHair
	{
		get
		{
			return m_nCustomPresetHair;
		}
		set
		{
			m_nCustomPresetHair = value;
		}
	}

	public int customFaceJawHeight
	{
		get
		{
			return m_nCustomFaceJawHeight;
		}
		set
		{
			m_nCustomFaceJawHeight = value;
		}
	}

	public int customFaceJawWidth
	{
		get
		{
			return m_nCustomFaceJawWidth;
		}
		set
		{
			m_nCustomFaceJawWidth = value;
		}
	}

	public int customFaceJawEndHeight
	{
		get
		{
			return m_nCustomFaceJawEndHeight;
		}
		set
		{
			m_nCustomFaceJawEndHeight = value;
		}
	}

	public int customFaceWidth
	{
		get
		{
			return m_nCustomFaceWidth;
		}
		set
		{
			m_nCustomFaceWidth = value;
		}
	}

	public int customFaceEyebrowHeight
	{
		get
		{
			return m_nCustomFaceEyebrowHeight;
		}
		set
		{
			m_nCustomFaceEyebrowHeight = value;
		}
	}

	public int customFaceEyebrowRotation
	{
		get
		{
			return m_nCustomFaceEyebrowRotation;
		}
		set
		{
			m_nCustomFaceEyebrowRotation = value;
		}
	}

	public int customFaceEyesWidth
	{
		get
		{
			return m_nCustomFaceEyesWidth;
		}
		set
		{
			m_nCustomFaceEyesWidth = value;
		}
	}

	public int customFaceNoseHeight
	{
		get
		{
			return m_nCustomFaceNoseHeight;
		}
		set
		{
			m_nCustomFaceNoseHeight = value;
		}
	}

	public int customFaceNoseWidth
	{
		get
		{
			return m_nCustomFaceNoseWidth;
		}
		set
		{
			m_nCustomFaceNoseWidth = value;
		}
	}

	public int customFaceMouthHeight
	{
		get
		{
			return m_nCustomFaceMouthHeight;
		}
		set
		{
			m_nCustomFaceMouthHeight = value;
		}
	}

	public int customFaceMouthWidth
	{
		get
		{
			return m_nCustomFaceMouthWidth;
		}
		set
		{
			m_nCustomFaceMouthWidth = value;
		}
	}

	public int customBodyHeadSize
	{
		get
		{
			return m_nCustomBodyHeadSize;
		}
		set
		{
			m_nCustomBodyHeadSize = value;
		}
	}

	public int customBodyArmsLength
	{
		get
		{
			return m_nCustomBodyArmsLength;
		}
		set
		{
			m_nCustomBodyArmsLength = value;
		}
	}

	public int customBodyArmsWidth
	{
		get
		{
			return m_nCustomBodyArmsWidth;
		}
		set
		{
			m_nCustomBodyArmsWidth = value;
		}
	}

	public int customBodyChestSize
	{
		get
		{
			return m_nCustomBodyChestSize;
		}
		set
		{
			m_nCustomBodyChestSize = value;
		}
	}

	public int customBodyWaistWidth
	{
		get
		{
			return m_nCustomBodyWaistWidth;
		}
		set
		{
			m_nCustomBodyWaistWidth = value;
		}
	}

	public int customBodyHipsSize
	{
		get
		{
			return m_nCustomBodyHipsSize;
		}
		set
		{
			m_nCustomBodyHipsSize = value;
		}
	}

	public int customBodyPelvisWidth
	{
		get
		{
			return m_nCustomBodyPelvisWidth;
		}
		set
		{
			m_nCustomBodyPelvisWidth = value;
		}
	}

	public int customBodyLegsLength
	{
		get
		{
			return m_nCustomBodyLegsLength;
		}
		set
		{
			m_nCustomBodyLegsLength = value;
		}
	}

	public int customBodyLegsWidth
	{
		get
		{
			return m_nCustomBodyLegsWidth;
		}
		set
		{
			m_nCustomBodyLegsWidth = value;
		}
	}

	public int customColorSkin
	{
		get
		{
			return m_nCustomColorSkin;
		}
		set
		{
			m_nCustomColorSkin = value;
		}
	}

	public int customColorEyes
	{
		get
		{
			return m_nCustomColorEyes;
		}
		set
		{
			m_nCustomColorEyes = value;
		}
	}

	public int customColorBeardAndEyebrow
	{
		get
		{
			return m_nCustomColorBeardAndEyebrow;
		}
		set
		{
			m_nCustomColorBeardAndEyebrow = value;
		}
	}

	public int customColorHair
	{
		get
		{
			return m_nCustomColorHair;
		}
		set
		{
			m_nCustomColorHair = value;
		}
	}

	public int costumeId
	{
		get
		{
			return m_nCostumeId;
		}
		set
		{
			m_nCostumeId = value;
		}
	}

	public int costumeEffectId
	{
		get
		{
			return m_nCostumeEffectId;
		}
		set
		{
			m_nCostumeEffectId = value;
		}
	}

	public Dictionary<int, FieldOfHonorHeroRealAttr> realAttrs => m_realAttrs;

	public Dictionary<int, FieldOfHonorHeroSkill> skills => m_skills;

	public Dictionary<int, FieldOfHonorHeroWing> wings => m_wings;

	public List<FieldOfHonorHeroWingPart> wingParts => m_wingParts;

	public FieldOfHonorHeroEquippedMainGear equippedWeapon => m_equippedWeapon;

	public FieldOfHonorHeroEquippedMainGear equippedArmor => m_equippedArmor;

	public Dictionary<int, FieldOfHonorHeroEquippedSubGear> equippedSubGears => m_equippedSubGears;

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_accountId = SFDBUtil.ToGuid(dr["accountId"]);
		m_nRanking = Convert.ToInt32(dr["fieldOfHonorRanking"]);
		m_id = SFDBUtil.ToGuid(dr["heroId"]);
		int nJobId = Convert.ToInt32(dr["jobId"]);
		if (nJobId > 0)
		{
			m_job = Resource.instance.GetJob(nJobId);
			if (m_job == null)
			{
				SFLogUtil.Warn(GetType(), string.Concat("직업이 존재하지 않습니다. m_id = ", m_id, ", nJobId = ", nJobId));
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), string.Concat("직업ID가 유효하지 않습니다. m_id = ", m_id, ", nJobId = ", nJobId));
		}
		int nNationId = Convert.ToInt32(dr["nationId"]);
		if (nNationId > 0)
		{
			m_nation = Resource.instance.GetNation(nNationId);
			if (m_nation == null)
			{
				SFLogUtil.Warn(GetType(), string.Concat("국가가 존재하지 않습니다. m_id = ", m_id, ", nNationId = ", nNationId));
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), string.Concat("국가ID가 유효하지 않습니다. m_id = ", m_id, ", nNationId = ", nNationId));
		}
		m_sName = Convert.ToString(dr["name"]);
		m_nLevel = Convert.ToInt32(dr["level"]);
		m_lnBattlePower = Convert.ToInt32(dr["battlePower"]);
		m_nRankId = Convert.ToInt32(dr["rankId"]);
		m_nEquippedWingId = Convert.ToInt32(dr["equippedWingId"]);
		m_nWingStep = Convert.ToInt32(dr["wingStep"]);
		m_nWingLevel = Convert.ToInt32(dr["wingLevel"]);
		m_nDisplayTitleId = Convert.ToInt32(dr["displayTitleId"]);
		m_sGuildName = Convert.ToString(dr["guildName"]);
		m_nGuildMemberGrade = Convert.ToInt32(dr["guildMemberGrade"]);
		m_nMainGearEnchantLevelSetNo = Convert.ToInt32(dr["mainGearEnchantLevelSetNo"]);
		m_nSubGearSoulstoneLevelSetNo = Convert.ToInt32(dr["subGearSoulstoneLevelSetNo"]);
		m_nCustomPresetHair = Convert.ToInt32(dr["customPresetHair"]);
		m_nCustomFaceJawHeight = Convert.ToInt32(dr["customFaceJawHeight"]);
		m_nCustomFaceJawWidth = Convert.ToInt32(dr["customFaceJawWidth"]);
		m_nCustomFaceJawEndHeight = Convert.ToInt32(dr["customFaceJawEndHeight"]);
		m_nCustomFaceWidth = Convert.ToInt32(dr["customFaceWidth"]);
		m_nCustomFaceEyebrowHeight = Convert.ToInt32(dr["customFaceEyebrowHeight"]);
		m_nCustomFaceEyebrowRotation = Convert.ToInt32(dr["customFaceEyebrowRotation"]);
		m_nCustomFaceEyesWidth = Convert.ToInt32(dr["customFaceEyesWidth"]);
		m_nCustomFaceNoseHeight = Convert.ToInt32(dr["customFaceNoseHeight"]);
		m_nCustomFaceNoseWidth = Convert.ToInt32(dr["customFaceNoseWidth"]);
		m_nCustomFaceMouthHeight = Convert.ToInt32(dr["customFaceMouthHeight"]);
		m_nCustomFaceMouthWidth = Convert.ToInt32(dr["customFaceMouthWidth"]);
		m_nCustomBodyHeadSize = Convert.ToInt32(dr["customBodyHeadSize"]);
		m_nCustomBodyArmsLength = Convert.ToInt32(dr["customBodyArmsLength"]);
		m_nCustomBodyArmsWidth = Convert.ToInt32(dr["customBodyArmsWidth"]);
		m_nCustomBodyChestSize = Convert.ToInt32(dr["customBodyChestSize"]);
		m_nCustomBodyWaistWidth = Convert.ToInt32(dr["customBodyWaistWidth"]);
		m_nCustomBodyHipsSize = Convert.ToInt32(dr["customBodyHipsSize"]);
		m_nCustomBodyPelvisWidth = Convert.ToInt32(dr["customBodyPelvisWidth"]);
		m_nCustomBodyLegsLength = Convert.ToInt32(dr["customBodyLegsLength"]);
		m_nCustomBodyLegsWidth = Convert.ToInt32(dr["customBodyLegsWidth"]);
		m_nCustomColorSkin = Convert.ToInt32(dr["customColorSkin"]);
		m_nCustomColorEyes = Convert.ToInt32(dr["customColorEyes"]);
		m_nCustomColorBeardAndEyebrow = Convert.ToInt32(dr["customColorBeardAndEyebrow"]);
		m_nCustomColorHair = Convert.ToInt32(dr["customColorHair"]);
		m_nCostumeId = Convert.ToInt32(dr["costumeId"]);
		m_nCostumeEffectId = Convert.ToInt32(dr["costumeEffectId"]);
	}

	public void AddRealAttr(FieldOfHonorHeroRealAttr realAttr)
	{
		if (realAttr == null)
		{
			throw new ArgumentNullException("realAttr");
		}
		m_realAttrs.Add(realAttr.id, realAttr);
	}

	public void AddSkill(FieldOfHonorHeroSkill skill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_skills.Add(skill.id, skill);
	}

	public void AddWing(FieldOfHonorHeroWing wing)
	{
		if (wing == null)
		{
			throw new ArgumentNullException("wing");
		}
		m_wings.Add(wing.id, wing);
	}

	public void AddWingPart(FieldOfHonorHeroWingPart wingPart)
	{
		if (wingPart == null)
		{
			throw new ArgumentNullException("wingPart");
		}
		m_wingParts.Add(wingPart);
	}

	public FieldOfHonorHeroWingPart GetWingPart(int nPartId)
	{
		int nIndex = nPartId - 1;
		if (nIndex < 0 || nIndex >= m_wingParts.Count)
		{
			return null;
		}
		return m_wingParts[nIndex];
	}

	public void SetEquippedMainGear(FieldOfHonorHeroEquippedMainGear equippedMainGear)
	{
		if (equippedMainGear != null)
		{
			if (equippedMainGear.mainGear.isWeapon)
			{
				m_equippedWeapon = equippedMainGear;
			}
			else
			{
				m_equippedArmor = equippedMainGear;
			}
		}
	}

	public FieldOfHonorHeroEquippedMainGear GetEquippedMainGear(Guid heroMainGearId)
	{
		if (m_equippedWeapon != null && m_equippedWeapon.heroMainGearId == heroMainGearId)
		{
			return m_equippedWeapon;
		}
		if (m_equippedArmor != null && m_equippedArmor.heroMainGearId == heroMainGearId)
		{
			return m_equippedArmor;
		}
		return null;
	}

	public void AddEquippedSubGear(FieldOfHonorHeroEquippedSubGear equippedSubGear)
	{
		if (equippedSubGear == null)
		{
			throw new ArgumentNullException("equippedSubGear");
		}
		m_equippedSubGears.Add(equippedSubGear.subGear.id, equippedSubGear);
	}

	public FieldOfHonorHeroEquippedSubGear GetEquippedSubGear(int nSubGearId)
	{
		if (!m_equippedSubGears.TryGetValue(nSubGearId, out var value))
		{
			return null;
		}
		return value;
	}

	public Hero ToHero(Hero controller, DateTimeOffset time)
	{
		Account account = new Account(m_accountId);
		Hero inst = new Hero(account, m_id, time, controller);
		inst.SetHeroByFieldOfHonorHero(this);
		return inst;
	}

	public PDFieldOfHonorHero ToPDFieldOfHonorHero()
	{
		PDFieldOfHonorHero inst = new PDFieldOfHonorHero();
		inst.ranking = m_nRanking;
		inst.name = m_sName;
		inst.nationId = nationId;
		inst.jobId = jobId;
		inst.level = m_nLevel;
		inst.rankNo = m_nRankId;
		inst.battlePower = m_lnBattlePower;
		inst.mainGearEnchantLevelSetNo = m_nMainGearEnchantLevelSetNo;
		inst.subGearSoulstoneLevelSetNo = m_nSubGearSoulstoneLevelSetNo;
		if (m_equippedWeapon != null)
		{
			inst.equippedWeapon = m_equippedWeapon.ToPDFullHeroMainGear();
		}
		if (m_equippedArmor != null)
		{
			inst.equippedArmor = m_equippedArmor.ToPDFullHeroMainGear();
		}
		List<PDFullHeroSubGear> pdEquippedsubGears = new List<PDFullHeroSubGear>();
		foreach (FieldOfHonorHeroEquippedSubGear equippedSubGear in m_equippedSubGears.Values)
		{
			pdEquippedsubGears.Add(equippedSubGear.ToPDFullHeroSubGear());
		}
		inst.equippedSubGears = pdEquippedsubGears.ToArray();
		inst.wingStep = m_nWingStep;
		inst.wingLevel = m_nWingLevel;
		inst.equippedWingId = m_nEquippedWingId;
		List<int> pdWings = new List<int>();
		foreach (FieldOfHonorHeroWing wing in m_wings.Values)
		{
			pdWings.Add(wing.id);
		}
		inst.wings = pdWings.ToArray();
		List<PDHeroWingPart> pdWingParts = new List<PDHeroWingPart>();
		foreach (FieldOfHonorHeroWingPart wingPart in m_wingParts)
		{
			pdWingParts.Add(wingPart.ToPDHeroWingPart());
		}
		inst.wingParts = pdWingParts.ToArray();
		List<PDAttrValuePair> pdRealAttrs = new List<PDAttrValuePair>();
		foreach (FieldOfHonorHeroRealAttr realAttr in m_realAttrs.Values)
		{
			pdRealAttrs.Add(realAttr.ToPDAttrValuePair());
		}
		inst.realAttrs = pdRealAttrs.ToArray();
		inst.guildId = (Guid)m_guildId;
		inst.guildName = m_sGuildName;
		inst.guildMemberGrade = m_nGuildMemberGrade;
		inst.displayTitleId = m_nDisplayTitleId;
		inst.customPresetHair = m_nCustomPresetHair;
		inst.customFaceJawHeight = m_nCustomFaceJawHeight;
		inst.customFaceJawWidth = m_nCustomFaceJawWidth;
		inst.customFaceJawEndHeight = m_nCustomFaceJawEndHeight;
		inst.customFaceWidth = m_nCustomFaceWidth;
		inst.customFaceEyebrowHeight = m_nCustomFaceEyebrowHeight;
		inst.customFaceEyebrowRotation = m_nCustomFaceEyebrowRotation;
		inst.customFaceEyesWidth = m_nCustomFaceEyesWidth;
		inst.customFaceNoseHeight = m_nCustomFaceNoseHeight;
		inst.customFaceNoseWidth = m_nCustomFaceNoseWidth;
		inst.customFaceMouthHeight = m_nCustomFaceMouthHeight;
		inst.customFaceMouthWidth = m_nCustomFaceMouthWidth;
		inst.customBodyHeadSize = m_nCustomBodyHeadSize;
		inst.customBodyArmsLength = m_nCustomBodyArmsLength;
		inst.customBodyArmsWidth = m_nCustomBodyArmsWidth;
		inst.customBodyChestSize = m_nCustomBodyChestSize;
		inst.customBodyWaistWidth = m_nCustomBodyWaistWidth;
		inst.customBodyHipsSize = m_nCustomBodyHipsSize;
		inst.customBodyPelvisWidth = m_nCustomBodyPelvisWidth;
		inst.customBodyLegsLength = m_nCustomBodyLegsLength;
		inst.customBodyLegsWidth = m_nCustomBodyLegsWidth;
		inst.customColorSkin = m_nCustomColorSkin;
		inst.customColorEyes = m_nCustomColorEyes;
		inst.customColorBeardAndEyebrow = m_nCustomColorBeardAndEyebrow;
		inst.customColorHair = m_nCustomColorHair;
		inst.equippedCostumeId = m_nCostumeId;
		inst.appliedCostumeEffectId = m_nCostumeEffectId;
		return inst;
	}

	public PDFieldOfHonorRanking ToPDFieldOfHonorRanking()
	{
		PDFieldOfHonorRanking inst = new PDFieldOfHonorRanking();
		inst.ranking = m_nRanking;
		inst.heroId = (Guid)m_id;
		inst.name = m_sName;
		inst.nationId = nationId;
		inst.battlePower = m_lnBattlePower;
		return inst;
	}
}
