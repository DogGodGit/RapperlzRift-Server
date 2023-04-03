using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroInfoCommandHandler : InGameCommandHandler<HeroInfoCommandBody, HeroInfoResponseBody>
{
	public const short kResult_HeroNotExist = 101;

	private DataRow m_drHero;

	private DataRowCollection m_drcEquippedHeroMainGears;

	private DataRowCollection m_drcEquippedHeroMainGearOptionAttrs;

	private DataRowCollection m_drcEquippedHeroSubGears;

	private DataRowCollection m_drcEquippedHeroSubGearRuneSockets;

	private DataRowCollection m_drcEquippedHeroSubGearSoulstoneSockets;

	private DataRowCollection m_drcHeroWings;

	private DataRowCollection m_drcHeroWingEnchants;

	private DataRowCollection m_drcHeroRealAttrValues;

	private DataRow m_drEquippedCostume;

	private List<HeroMainGear> m_mainGears = new List<HeroMainGear>();

	private HeroMainGear m_weaponMainGear;

	private HeroMainGear m_armorMainGear;

	private List<HeroSubGear> m_subGears = new List<HeroSubGear>();

	private List<HeroMountGear> m_mountGears = new List<HeroMountGear>();

	private List<int> m_wings = new List<int>();

	private List<HeroWingPart> m_wingParts = new List<HeroWingPart>();

	private Guild m_guild;

	private List<AttrValuePair> m_heroRealAttrValues = new List<AttrValuePair>();

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		Guid heroId = (Guid)m_body.heroId;
		if (heroId == m_myHero.id)
		{
			throw new CommandHandleException(1, "영웅ID가 자기자신 입니다.");
		}
		Hero targetHero = Cache.instance.GetLoggedInHero(heroId);
		if (targetHero == null)
		{
			SFRunnableQueuingWork work = RunnableQueuingWorkUtil.CreateHeroWork(m_myHero.id);
			work.runnable = new SFAction<Guid>(ProcessGetLoggedOutHeroInfo, heroId);
			RunWork(work);
			return;
		}
		lock (targetHero.syncObject)
		{
			HeroInfoResponseBody resBody = new HeroInfoResponseBody();
			resBody.isLoggedIn = true;
			resBody.jobId = targetHero.jobId;
			resBody.nationId = targetHero.nationId;
			resBody.name = targetHero.name;
			resBody.level = targetHero.level;
			resBody.battlePower = targetHero.battlePower;
			resBody.rankNo = targetHero.rankNo;
			resBody.mainGearEnchantLevelSetNo = targetHero.mainGearEnchantLevelSetNo;
			resBody.subGearSoulstoneLevelSetNo = targetHero.subGearSoulstoneLevelSetNo;
			resBody.equippedWeapon = ((targetHero.equippedWeapon != null) ? targetHero.equippedWeapon.ToPDFullHeroMainGear(bIncludeRefinements: false) : null);
			resBody.equippedArmor = ((targetHero.equippedArmor != null) ? targetHero.equippedArmor.ToPDFullHeroMainGear(bIncludeRefinements: false) : null);
			resBody.equippedHeroSubGears = targetHero.GetEquippedPDFullHeroSubGears().ToArray();
			resBody.wingStep = targetHero.wingStep.step;
			resBody.wingLevel = targetHero.wingStepLevel.level;
			resBody.equippedWingId = targetHero.equippedWingId;
			resBody.wings = targetHero.GetHeroWings().ToArray();
			resBody.heroWingParts = targetHero.GetPDHeroWingParts().ToArray();
			GuildMember guildMember = targetHero.guildMember;
			if (guildMember != null)
			{
				resBody.guildId = (Guid)guildMember.guild.id;
				resBody.guildName = guildMember.guild.name;
				resBody.guildMemberGrade = guildMember.grade.id;
			}
			resBody.displayTitleId = targetHero.displayTitleId;
			resBody.realAttrValues = targetHero.GetPDAttrValuePairsForRealAttrValues().ToArray();
			resBody.customPresetHair = targetHero.customPresetHair;
			resBody.customFaceJawHeight = targetHero.customFaceJawHeight;
			resBody.customFaceJawWidth = targetHero.customFaceJawWidth;
			resBody.customFaceJawEndHeight = targetHero.customFaceJawEndHeight;
			resBody.customFaceWidth = targetHero.customFaceWidth;
			resBody.customFaceEyebrowHeight = targetHero.customFaceEyebrowHeight;
			resBody.customFaceEyebrowRotation = targetHero.customFaceEyebrowRotation;
			resBody.customFaceEyesWidth = targetHero.customFaceEyesWidth;
			resBody.customFaceNoseHeight = targetHero.customFaceNoseHeight;
			resBody.customFaceNoseWidth = targetHero.customFaceNoseWidth;
			resBody.customFaceMouthHeight = targetHero.customFaceMouthHeight;
			resBody.customFaceMouthWidth = targetHero.customFaceMouthWidth;
			resBody.customBodyHeadSize = targetHero.customBodyHeadSize;
			resBody.customBodyArmsLength = targetHero.customBodyArmsLength;
			resBody.customBodyArmsWidth = targetHero.customBodyArmsWidth;
			resBody.customBodyChestSize = targetHero.customBodyChestSize;
			resBody.customBodyWaistWidth = targetHero.customBodyWaistWidth;
			resBody.customBodyHipsSize = targetHero.customBodyHipsSize;
			resBody.customBodyPelvisWidth = targetHero.customBodyPelvisWidth;
			resBody.customBodyLegsLength = targetHero.customBodyLegsLength;
			resBody.customBodyLegsWidth = targetHero.customBodyLegsWidth;
			resBody.customColorSkin = targetHero.customColorSkin;
			resBody.customColorEyes = targetHero.customColorEyes;
			resBody.customColorBeardAndEyebrow = targetHero.customColorBeardAndEyebrow;
			resBody.customColorHair = targetHero.customColorHair;
			HeroCostume equippedCostume = targetHero.equippedCostume;
			resBody.equippedCostumeId = equippedCostume?.costumeId ?? 0;
			resBody.appliedCostumeEffectId = equippedCostume?.costumeEffectId ?? 0;
			SendResponseOK(resBody);
		}
	}

	private void ProcessGetLoggedOutHeroInfo(Guid heroId)
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			m_drHero = GameDac.Hero(conn, null, heroId);
			if (m_drHero == null)
			{
				throw new CommandHandleException(101, "해당 영웅이 존재하지 않습니다. heroId = " + heroId);
			}
			if (!Convert.ToBoolean(m_drHero["created"]))
			{
				throw new CommandHandleException(1, "영웅 생성이 완료되지 않았습니다. heroId = " + heroId);
			}
			m_drcEquippedHeroMainGears = GameDac.EquippedHeroMainGears(conn, null, heroId);
			m_drcEquippedHeroMainGearOptionAttrs = GameDac.EquippedHeroMainGearOptionAttrs(conn, null, heroId);
			m_drcEquippedHeroSubGears = GameDac.EquippedHeroSubGears(conn, null, heroId);
			m_drcEquippedHeroSubGearRuneSockets = GameDac.EquippedHeroSubGearRuneSockets(conn, null, heroId);
			m_drcEquippedHeroSubGearSoulstoneSockets = GameDac.EquippedHeroSubGearSoulstoneSockets(conn, null, heroId);
			m_drcHeroWings = GameDac.HeroWings(conn, null, heroId);
			m_drcHeroWingEnchants = GameDac.HeroWingEnchants(conn, null, heroId);
			m_drcHeroRealAttrValues = GameDac.HeroRealAttrValues(conn, null, heroId);
			Guid equippedHeroCostumeId = SFDBUtil.ToGuid(m_drHero["equippedHeroCostumeId"], Guid.Empty);
			if (equippedHeroCostumeId != Guid.Empty)
			{
				m_drEquippedCostume = GameDac.HeroCostume(conn, null, equippedHeroCostumeId);
			}
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	protected override void OnWork_Success(SFWork work)
	{
		base.OnWork_Success(work);
		InItHeroInfos();
		HeroInfoResponseBody resBody = new HeroInfoResponseBody();
		resBody.isLoggedIn = false;
		resBody.jobId = Convert.ToInt32(m_drHero["jobId"]);
		resBody.nationId = Convert.ToInt32(m_drHero["nationId"]);
		resBody.name = Convert.ToString(m_drHero["name"]);
		resBody.level = Convert.ToInt32(m_drHero["level"]);
		resBody.battlePower = Convert.ToInt64(m_drHero["battlePower"]);
		resBody.rankNo = Convert.ToInt32(m_drHero["rankNo"]);
		resBody.mainGearEnchantLevelSetNo = Convert.ToInt32(m_drHero["mainGearEnchantLevelSetNo"]);
		resBody.subGearSoulstoneLevelSetNo = Convert.ToInt32(m_drHero["subGearSoulstoneLevelSetNo"]);
		if (m_weaponMainGear != null)
		{
			resBody.equippedWeapon = m_weaponMainGear.ToPDFullHeroMainGear(bIncludeRefinements: false);
		}
		if (m_armorMainGear != null)
		{
			resBody.equippedArmor = m_armorMainGear.ToPDFullHeroMainGear(bIncludeRefinements: false);
		}
		resBody.equippedHeroSubGears = HeroSubGear.ToPDFullHeroSubGears(m_subGears).ToArray();
		resBody.wingStep = Convert.ToInt32(m_drHero["wingStep"]);
		resBody.wingLevel = Convert.ToInt32(m_drHero["wingLevel"]);
		resBody.equippedWingId = Convert.ToInt32(m_drHero["equippedWingId"]);
		resBody.wings = m_wings.ToArray();
		resBody.heroWingParts = HeroWingPart.ToPDHeroWingParts(m_wingParts).ToArray();
		if (m_guild != null)
		{
			resBody.guildName = m_guild.name;
		}
		resBody.realAttrValues = AttrValuePair.ToPDAttrValuePairs(m_heroRealAttrValues).ToArray();
		resBody.customPresetHair = Convert.ToInt32(m_drHero["customPresetHair"]);
		resBody.customFaceJawHeight = Convert.ToInt32(m_drHero["customFaceJawHeight"]);
		resBody.customFaceJawWidth = Convert.ToInt32(m_drHero["customFaceJawWidth"]);
		resBody.customFaceJawEndHeight = Convert.ToInt32(m_drHero["customFaceJawEndHeight"]);
		resBody.customFaceWidth = Convert.ToInt32(m_drHero["customFaceWidth"]);
		resBody.customFaceEyebrowHeight = Convert.ToInt32(m_drHero["customFaceEyebrowHeight"]);
		resBody.customFaceEyebrowRotation = Convert.ToInt32(m_drHero["customFaceEyebrowRotation"]);
		resBody.customFaceEyesWidth = Convert.ToInt32(m_drHero["customFaceEyesWidth"]);
		resBody.customFaceNoseHeight = Convert.ToInt32(m_drHero["customFaceNoseHeight"]);
		resBody.customFaceNoseWidth = Convert.ToInt32(m_drHero["customFaceNoseWidth"]);
		resBody.customFaceMouthHeight = Convert.ToInt32(m_drHero["customFaceMouthHeight"]);
		resBody.customFaceMouthWidth = Convert.ToInt32(m_drHero["customFaceMouthWidth"]);
		resBody.customBodyHeadSize = Convert.ToInt32(m_drHero["customBodyHeadSize"]);
		resBody.customBodyArmsLength = Convert.ToInt32(m_drHero["customBodyArmsLength"]);
		resBody.customBodyArmsWidth = Convert.ToInt32(m_drHero["customBodyArmsWidth"]);
		resBody.customBodyChestSize = Convert.ToInt32(m_drHero["customBodyChestSize"]);
		resBody.customBodyWaistWidth = Convert.ToInt32(m_drHero["customBodyWaistWidth"]);
		resBody.customBodyHipsSize = Convert.ToInt32(m_drHero["customBodyHipsSize"]);
		resBody.customBodyPelvisWidth = Convert.ToInt32(m_drHero["customBodyPelvisWidth"]);
		resBody.customBodyLegsLength = Convert.ToInt32(m_drHero["customBodyLegsLength"]);
		resBody.customBodyLegsWidth = Convert.ToInt32(m_drHero["customBodyLegsWidth"]);
		resBody.customColorSkin = Convert.ToInt32(m_drHero["customColorSkin"]);
		resBody.customColorEyes = Convert.ToInt32(m_drHero["customColorEyes"]);
		resBody.customColorBeardAndEyebrow = Convert.ToInt32(m_drHero["customColorBeardAndEyebrow"]);
		resBody.customColorHair = Convert.ToInt32(m_drHero["customColorHair"]);
		int nEquippedCostumeId = 0;
		int nAppliedCostumeEffectId = 0;
		if (m_drEquippedCostume != null)
		{
			nEquippedCostumeId = Convert.ToInt32(m_drEquippedCostume["cosutmeId"]);
			nAppliedCostumeEffectId = Convert.ToInt32(m_drEquippedCostume["costumeEffectId"]);
		}
		resBody.equippedCostumeId = nEquippedCostumeId;
		resBody.appliedCostumeEffectId = nAppliedCostumeEffectId;
		SendResponseOK(resBody);
	}

	private void InItHeroInfos()
	{
		Guid equippedWeaponId = (Guid)m_drHero["weaponHeroMainGearId"];
		foreach (DataRow dr3 in m_drcEquippedHeroMainGears)
		{
			HeroMainGear mainGear = new HeroMainGear();
			mainGear.Init(dr3);
			m_mainGears.Add(mainGear);
		}
		foreach (DataRow dr7 in m_drcEquippedHeroMainGearOptionAttrs)
		{
			Guid mainGearId = (Guid)dr7["heroMainGearId"];
			HeroMainGearOptionAttr attr = new HeroMainGearOptionAttr();
			attr.Init(dr7);
			HeroMainGear mainGear3 = GetMainGear(mainGearId);
			mainGear3.AddOptinAttr(attr);
		}
		foreach (HeroMainGear mainGear2 in m_mainGears)
		{
			if (equippedWeaponId == mainGear2.id)
			{
				m_weaponMainGear = mainGear2;
			}
			else
			{
				m_armorMainGear = mainGear2;
			}
		}
		foreach (DataRow dr8 in m_drcEquippedHeroSubGears)
		{
			HeroSubGear subGear3 = new HeroSubGear();
			subGear3.Init(dr8);
			m_subGears.Add(subGear3);
		}
		foreach (DataRow dr6 in m_drcEquippedHeroSubGearRuneSockets)
		{
			int nSubGearId2 = Convert.ToInt32(dr6["subGearId"]);
			int nSocketIndex2 = Convert.ToInt32(dr6["socketIndex"]);
			int nItemId2 = Convert.ToInt32(dr6["itemId"]);
			HeroSubGear subGear2 = GetSubGear(nSubGearId2);
			Item item2 = Resource.instance.GetItem(nItemId2);
			subGear2.GetRuneSocket(nSocketIndex2).Mount(item2);
		}
		foreach (DataRow dr5 in m_drcEquippedHeroSubGearSoulstoneSockets)
		{
			int nSubGearId = Convert.ToInt32(dr5["subGearId"]);
			int nSocketIndex = Convert.ToInt32(dr5["socketIndex"]);
			int nItemId = Convert.ToInt32(dr5["itemId"]);
			HeroSubGear subGear = GetSubGear(nSubGearId);
			Item item = Resource.instance.GetItem(nItemId);
			subGear.GetSoulstoneSocket(nSocketIndex).Mount(item);
		}
		foreach (DataRow dr4 in m_drcHeroWings)
		{
			int wingId = Convert.ToInt32(dr4["wingId"]);
			m_wings.Add(wingId);
		}
		foreach (WingPart part2 in Resource.instance.wingParts.Values)
		{
			HeroWingPart heroWingPart = new HeroWingPart(part2);
			m_wingParts.Add(heroWingPart);
		}
		foreach (DataRow dr2 in m_drcHeroWingEnchants)
		{
			int nWingStep = Convert.ToInt32(dr2["step"]);
			int nWingLevel = Convert.ToInt32(dr2["level"]);
			WingStepLevel stepLevel = Resource.instance.GetWingStep(nWingStep).GetLevel(nWingLevel);
			int nPartId = Convert.ToInt32(dr2["partId"]);
			HeroWingPart part = GetWingPart(nPartId);
			HeroWingEnchant enchant = new HeroWingEnchant(part, stepLevel);
			part.AddEnchant(enchant);
		}
		Guid guildId = (Guid)m_drHero["guildId"];
		m_guild = Cache.instance.GetGuild(guildId);
		foreach (DataRow dr in m_drcHeroRealAttrValues)
		{
			int nAttrId = Convert.ToInt32(dr["attrId"]);
			int nValue = Convert.ToInt32(dr["value"]);
			AttrValuePair attrValuePair = new AttrValuePair(nAttrId, nValue);
			m_heroRealAttrValues.Add(attrValuePair);
		}
	}

	private HeroMainGear GetMainGear(Guid gearId)
	{
		foreach (HeroMainGear heroMainGear in m_mainGears)
		{
			if (gearId == heroMainGear.id)
			{
				return heroMainGear;
			}
		}
		return null;
	}

	private HeroSubGear GetSubGear(int nGearId)
	{
		foreach (HeroSubGear heroSubGear in m_subGears)
		{
			if (nGearId == heroSubGear.subGearId)
			{
				return heroSubGear;
			}
		}
		return null;
	}

	private HeroWingPart GetWingPart(int nPartId)
	{
		foreach (HeroWingPart part in m_wingParts)
		{
			if (nPartId == part.part.id)
			{
				return part;
			}
		}
		return null;
	}
}
