using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class LobbyInfoCommandHandler : LobbyCommandHandler<LobbyInfoCommandBody, LobbyInfoResponseBody>
{
	private DataRowCollection m_drcHeroes;

	protected override void HandleLobbyCommand()
	{
		SFRunnableStandaloneWork work = new SFRunnableStandaloneWork();
		work.runnable = new SFAction(GetLobbyInfo);
		RunWork(work);
	}

	private void GetLobbyInfo()
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			m_drcHeroes = GameDac.LobbyHeroes(conn, null, m_myAccount.id);
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
		LobbyInfoResponseBody resBody = new LobbyInfoResponseBody();
		resBody.lastHeroId = (Guid)m_myAccount.lastHeroId;
		List<PDLobbyHero> resHeroes = new List<PDLobbyHero>();
		foreach (DataRow dr in m_drcHeroes)
		{
			PDLobbyHero inst = new PDLobbyHero();
			inst.id = (Guid)(Guid)dr["heroId"];
			inst.jobId = Convert.ToInt32(dr["jobId"]);
			inst.nationId = Convert.ToInt32(dr["nationId"]);
			inst.namingTutorialCompleted = Convert.ToBoolean(dr["namingTutorialCompleted"]);
			inst.name = SFDBUtil.ToString(dr["name"]);
			inst.level = Convert.ToInt32(dr["level"]);
			inst.battlePower = Convert.ToInt64(dr["battlePower"]);
			Guid weaponHeroMainGearId = (Guid)dr["weaponHeroMainGearId"];
			int nWeaponMainGearId = SFDBUtil.ToInt32(dr["weaponMainGearId"]);
			int nWeaponEnchantLevel = SFDBUtil.ToInt32(dr["weaponEnchantLevel"]);
			if (weaponHeroMainGearId != Guid.Empty)
			{
				inst.equippedWeapon = new PDHeroMainGear((Guid)weaponHeroMainGearId, nWeaponMainGearId, nWeaponEnchantLevel);
			}
			Guid armorHeroMainGearId = (Guid)dr["armorHeroMainGearId"];
			int nArmorMainGearId = SFDBUtil.ToInt32(dr["armorMainGearId"]);
			int nArmorEnchantLevel = SFDBUtil.ToInt32(dr["armorEnchantLevel"]);
			if (armorHeroMainGearId != Guid.Empty)
			{
				inst.equippedArmor = new PDHeroMainGear((Guid)armorHeroMainGearId, nArmorMainGearId, nArmorEnchantLevel);
			}
			inst.equippedWingId = Convert.ToInt32(dr["equippedWingId"]);
			inst.customPresetHair = Convert.ToInt32(dr["customPresetHair"]);
			inst.customFaceJawHeight = Convert.ToInt32(dr["customFaceJawHeight"]);
			inst.customFaceJawWidth = Convert.ToInt32(dr["customFaceJawWidth"]);
			inst.customFaceJawEndHeight = Convert.ToInt32(dr["customFaceJawEndHeight"]);
			inst.customFaceWidth = Convert.ToInt32(dr["customFaceWidth"]);
			inst.customFaceEyebrowHeight = Convert.ToInt32(dr["customFaceEyebrowHeight"]);
			inst.customFaceEyebrowRotation = Convert.ToInt32(dr["customFaceEyebrowRotation"]);
			inst.customFaceEyesWidth = Convert.ToInt32(dr["customFaceEyesWidth"]);
			inst.customFaceNoseHeight = Convert.ToInt32(dr["customFaceNoseHeight"]);
			inst.customFaceNoseWidth = Convert.ToInt32(dr["customFaceNoseWidth"]);
			inst.customFaceMouthHeight = Convert.ToInt32(dr["customFaceMouthHeight"]);
			inst.customFaceMouthWidth = Convert.ToInt32(dr["customFaceMouthWidth"]);
			inst.customBodyHeadSize = Convert.ToInt32(dr["customBodyHeadSize"]);
			inst.customBodyArmsLength = Convert.ToInt32(dr["customBodyArmsLength"]);
			inst.customBodyArmsWidth = Convert.ToInt32(dr["customBodyArmsWidth"]);
			inst.customBodyChestSize = Convert.ToInt32(dr["customBodyChestSize"]);
			inst.customBodyWaistWidth = Convert.ToInt32(dr["customBodyWaistWidth"]);
			inst.customBodyHipsSize = Convert.ToInt32(dr["customBodyHipsSize"]);
			inst.customBodyPelvisWidth = Convert.ToInt32(dr["customBodyPelvisWidth"]);
			inst.customBodyLegsLength = Convert.ToInt32(dr["customBodyLegsLength"]);
			inst.customBodyLegsWidth = Convert.ToInt32(dr["customBodyLegsWidth"]);
			inst.customColorSkin = Convert.ToInt32(dr["customColorSkin"]);
			inst.customColorEyes = Convert.ToInt32(dr["customColorEyes"]);
			inst.customColorBeardAndEyebrow = Convert.ToInt32(dr["customColorBeardAndEyebrow"]);
			inst.customColorHair = Convert.ToInt32(dr["customColorHair"]);
			inst.equippedCostumeId = SFDBUtil.ToInt32(dr["costumeId"]);
			inst.appliedCostumeEffectId = SFDBUtil.ToInt32(dr["costumeEffectId"]);
			resHeroes.Add(inst);
		}
		resBody.heroes = resHeroes.ToArray();
		resBody.heroCreationDefaultNationId = Cache.instance.GetHeroCreationDefaultNationIdWithLock();
		SendResponseOK(resBody);
	}
}
