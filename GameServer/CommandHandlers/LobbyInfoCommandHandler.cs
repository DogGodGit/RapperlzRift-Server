using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

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
        resBody.lastHeroId = m_myAccount.lastHeroId;
        List<PDLobbyHero> resHeroes = new List<PDLobbyHero>();
        foreach (DataRow dr in m_drcHeroes)
        {
            PDLobbyHero inst = new PDLobbyHero();
            inst.id = SFDBUtil.ToGuid(dr["heroId"]);
            inst.jobId = SFDBUtil.ToInt32(dr["jobId"]);
            inst.nationId = SFDBUtil.ToInt32(dr["nationId"]);
            inst.namingTutorialCompleted = Convert.ToBoolean(dr["namingTutorialCompleted"]);
            inst.name = SFDBUtil.ToString(dr["name"]);
            inst.level = SFDBUtil.ToInt32(dr["level"]);
            inst.battlePower = SFDBUtil.ToInt64(dr["battlePower"]);
            Guid weaponHeroMainGearId = (Guid)dr["weaponHeroMainGearId"];
            int nWeaponMainGearId = SFDBUtil.ToInt32(dr["weaponMainGearId"]);
            int nWeaponEnchantLevel = SFDBUtil.ToInt32(dr["weaponEnchantLevel"]);
            if (weaponHeroMainGearId != Guid.Empty)
            {
                inst.equippedWeapon = new PDHeroMainGear(weaponHeroMainGearId, nWeaponMainGearId, nWeaponEnchantLevel);
            }
            Guid armorHeroMainGearId = (Guid)dr["armorHeroMainGearId"];
            int nArmorMainGearId = SFDBUtil.ToInt32(dr["armorMainGearId"]);
            int nArmorEnchantLevel = SFDBUtil.ToInt32(dr["armorEnchantLevel"]);
            if (armorHeroMainGearId != Guid.Empty)
            {
                inst.equippedArmor = new PDHeroMainGear(armorHeroMainGearId, nArmorMainGearId, nArmorEnchantLevel);
            }
            inst.equippedWingId = SFDBUtil.ToInt32(dr["equippedWingId"]);
            inst.customPresetHair = SFDBUtil.ToInt32(dr["customPresetHair"]);
            inst.customFaceJawHeight = SFDBUtil.ToInt32(dr["customFaceJawHeight"]);
            inst.customFaceJawWidth = SFDBUtil.ToInt32(dr["customFaceJawWidth"]);
            inst.customFaceJawEndHeight = SFDBUtil.ToInt32(dr["customFaceJawEndHeight"]);
            inst.customFaceWidth = SFDBUtil.ToInt32(dr["customFaceWidth"]);
            inst.customFaceEyebrowHeight = SFDBUtil.ToInt32(dr["customFaceEyebrowHeight"]);
            inst.customFaceEyebrowRotation = SFDBUtil.ToInt32(dr["customFaceEyebrowRotation"]);
            inst.customFaceEyesWidth = SFDBUtil.ToInt32(dr["customFaceEyesWidth"]);
            inst.customFaceNoseHeight = SFDBUtil.ToInt32(dr["customFaceNoseHeight"]);
            inst.customFaceNoseWidth = SFDBUtil.ToInt32(dr["customFaceNoseWidth"]);
            inst.customFaceMouthHeight = SFDBUtil.ToInt32(dr["customFaceMouthHeight"]);
            inst.customFaceMouthWidth = SFDBUtil.ToInt32(dr["customFaceMouthWidth"]);
            inst.customBodyHeadSize = SFDBUtil.ToInt32(dr["customBodyHeadSize"]);
            inst.customBodyArmsLength = SFDBUtil.ToInt32(dr["customBodyArmsLength"]);
            inst.customBodyArmsWidth = SFDBUtil.ToInt32(dr["customBodyArmsWidth"]);
            inst.customBodyChestSize = SFDBUtil.ToInt32(dr["customBodyChestSize"]);
            inst.customBodyWaistWidth = SFDBUtil.ToInt32(dr["customBodyWaistWidth"]);
            inst.customBodyHipsSize = SFDBUtil.ToInt32(dr["customBodyHipsSize"]);
            inst.customBodyPelvisWidth = SFDBUtil.ToInt32(dr["customBodyPelvisWidth"]);
            inst.customBodyLegsLength = SFDBUtil.ToInt32(dr["customBodyLegsLength"]);
            inst.customBodyLegsWidth = SFDBUtil.ToInt32(dr["customBodyLegsWidth"]);
            inst.customColorSkin = SFDBUtil.ToInt32(dr["customColorSkin"]);
            inst.customColorEyes = SFDBUtil.ToInt32(dr["customColorEyes"]);
            inst.customColorBeardAndEyebrow = SFDBUtil.ToInt32(dr["customColorBeardAndEyebrow"]);
            inst.customColorHair = SFDBUtil.ToInt32(dr["customColorHair"]);
            inst.equippedCostumeId = SFDBUtil.ToInt32(dr["costumeId"]);
            inst.appliedCostumeEffectId = SFDBUtil.ToInt32(dr["costumeEffectId"]);
            resHeroes.Add(inst);
        }
        resBody.heroes = resHeroes.ToArray();
        resBody.heroCreationDefaultNationId = Cache.instance.GetHeroCreationDefaultNationIdWithLock();
        SendResponseOK(resBody);
    }
}
