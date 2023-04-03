using System;
using System.Data;
using System.Data.SqlClient;
using ServerFramework;

namespace GameServer;

public static class GameLogDac
{
	public static SqlCommand CSC_AddConnectionLog(Guid logId, int nClientCount, int nHeroCount, DateTimeOffset logTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddConnectionLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nClientCount", SqlDbType.Int).Value = nClientCount;
		sc.Parameters.Add("@nHeroCount", SqlDbType.Int).Value = nHeroCount;
		sc.Parameters.Add("@logTime", SqlDbType.DateTimeOffset).Value = logTime;
		return sc;
	}

	public static SqlCommand CSC_AddNationConnectionLog(Guid logId, int nNationId, int nHeroCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationConnectionLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@nHeroCount", SqlDbType.Int).Value = nHeroCount;
		return sc;
	}

	public static int AddWorkLog(SqlConnection conn, SqlTransaction trans, Guid logId, DateTimeOffset logTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddWorkLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@logTime", SqlDbType.DateTimeOffset).Value = logTime;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static int AddWorkLogEntry(SqlConnection conn, SqlTransaction trans, Guid entryId, Guid logId, string sTypeName, long lnRequestCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddWorkLogEntry";
		sc.Parameters.Add("@entryId", SqlDbType.UniqueIdentifier).Value = entryId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@sTypeName", SqlDbType.NVarChar).Value = sTypeName;
		sc.Parameters.Add("@lnRequestCount", SqlDbType.BigInt).Value = lnRequestCount;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static SqlCommand CSC_AddWorkQueueLog(Guid logId, int nWorkCount, DateTimeOffset logTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddWorkQueueLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nWorkCount", SqlDbType.Int).Value = nWorkCount;
		sc.Parameters.Add("@logTime", SqlDbType.DateTimeOffset).Value = logTime;
		return sc;
	}

	public static SqlCommand CSC_AddAccountLoginLog(Guid logId, Guid accountId, string sIp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAccountLoginLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@sIp", SqlDbType.VarChar).Value = sIp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddSubGearLevelUpLog(Guid logId, Guid heroId, int nSubGearId, int nOldLevel, int nLevel, long lnUsedGold, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSubGearLevelUpLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nOldLevel", SqlDbType.Int).Value = nOldLevel;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddSubGearGradeUpLog(Guid logId, Guid heroId, int nSubGearId, int nOldLevel, int nLevel, int nOldGrade, int nGrade, int nMaterialItem1Id, int nMaterialItem1OwnCount, int nMaterialItem1UnOwnCount, int nMaterialItem2Id, int nMaterialItem2OwnCount, int nMaterialItem2UnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSubGearGradeUpLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nOldLevel", SqlDbType.Int).Value = nOldLevel;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nOldGrade", SqlDbType.Int).Value = nOldGrade;
		sc.Parameters.Add("@nGrade", SqlDbType.Int).Value = nGrade;
		sc.Parameters.Add("@nMaterialItem1Id", SqlDbType.Int).Value = nMaterialItem1Id;
		sc.Parameters.Add("@nMaterialItem1OwnCount", SqlDbType.Int).Value = nMaterialItem1OwnCount;
		sc.Parameters.Add("@nMaterialItem1UnOwnCount", SqlDbType.Int).Value = nMaterialItem1UnOwnCount;
		sc.Parameters.Add("@nMaterialItem2Id", SqlDbType.Int).Value = nMaterialItem2Id;
		sc.Parameters.Add("@nMaterialItem2OwnCount", SqlDbType.Int).Value = nMaterialItem2OwnCount;
		sc.Parameters.Add("@nMaterialItem2UnOwnCount", SqlDbType.Int).Value = nMaterialItem2UnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddSubGearQualityUpLog(Guid logId, Guid heroId, int nSubGearId, int nLevel, int nOldQuality, int nQuality, int nMaterialItem1Id, int nMaterialItem1OwnCount, int nMaterialItem1UnOwnCount, int nMaterialItem2Id, int nMaterialItem2OwnCount, int nMaterialItem2UnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSubGearQualityUpLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nOldQuality", SqlDbType.Int).Value = nOldQuality;
		sc.Parameters.Add("@nQuality", SqlDbType.Int).Value = nQuality;
		sc.Parameters.Add("@nMaterialItem1Id", SqlDbType.Int).Value = nMaterialItem1Id;
		sc.Parameters.Add("@nMaterialItem1OwnCount", SqlDbType.Int).Value = nMaterialItem1OwnCount;
		sc.Parameters.Add("@nMaterialItem1UnOwnCount", SqlDbType.Int).Value = nMaterialItem1UnOwnCount;
		sc.Parameters.Add("@nMaterialItem2Id", SqlDbType.Int).Value = nMaterialItem2Id;
		sc.Parameters.Add("@nMaterialItem2OwnCount", SqlDbType.Int).Value = nMaterialItem2OwnCount;
		sc.Parameters.Add("@nMaterialItem2UnOwnCount", SqlDbType.Int).Value = nMaterialItem2UnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroSubGearSoulstoneLevelSetActivationLog(Guid logId, Guid heroId, int nSetNo, int nSubGearSoulstoneTotalLevel, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroSubGearSoulstoneLevelSetActivationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSetNo", SqlDbType.Int).Value = nSetNo;
		sc.Parameters.Add("@nSubGearSoulstoneTotalLevel", SqlDbType.Int).Value = nSubGearSoulstoneTotalLevel;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMainGearEnchantLog(Guid logId, Guid heroId, Guid heroMainGearId, int nOldEnchantLevel, int nEnchantLevel, bool bOldOwned, bool bOwned, bool bIsSuccess, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, int nPenaltyPreventItemId, int nPenaltyPreventItemOwnCount, int nPenaltyPreventItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeoMainGearEnchantLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@nOldEnchantLevel", SqlDbType.Int).Value = nOldEnchantLevel;
		sc.Parameters.Add("@nEnchantLevel", SqlDbType.Int).Value = nEnchantLevel;
		sc.Parameters.Add("@bOldOwned", SqlDbType.Bit).Value = bOldOwned;
		sc.Parameters.Add("@bOwned", SqlDbType.Bit).Value = bOwned;
		sc.Parameters.Add("@bIsSuccess", SqlDbType.Bit).Value = bIsSuccess;
		sc.Parameters.Add("@nMaterialItemId", SqlDbType.Int).Value = nMaterialItemId;
		sc.Parameters.Add("@nMaterialItemOwnCount", SqlDbType.Int).Value = nMaterialItemOwnCount;
		sc.Parameters.Add("@nMaterialItemUnOwnCount", SqlDbType.Int).Value = nMaterialItemUnOwnCount;
		sc.Parameters.Add("@nPenaltyPreventItemId", SqlDbType.Int).Value = nPenaltyPreventItemId;
		sc.Parameters.Add("@nPenaltyPreventItemOwnCount", SqlDbType.Int).Value = nPenaltyPreventItemOwnCount;
		sc.Parameters.Add("@nPenaltyPreventItemUnOwnCount", SqlDbType.Int).Value = nPenaltyPreventItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMainGearRefinementLog(Guid logId, Guid heroId, Guid heroMainGearId, bool bOldOwned, bool bOwned, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, int nProtectionItemId, int nProtectionItemOwnCount, int nProtectionItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMainGearRefinementLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@bOldOwned", SqlDbType.Bit).Value = bOldOwned;
		sc.Parameters.Add("@bOwned", SqlDbType.Bit).Value = bOwned;
		sc.Parameters.Add("@nMaterialItemId", SqlDbType.Int).Value = nMaterialItemId;
		sc.Parameters.Add("@nMaterialItemOwnCount", SqlDbType.Int).Value = nMaterialItemOwnCount;
		sc.Parameters.Add("@nMaterialItemUnOwnCount", SqlDbType.Int).Value = nMaterialItemUnOwnCount;
		sc.Parameters.Add("@nProtectionItemId", SqlDbType.Int).Value = nProtectionItemId;
		sc.Parameters.Add("@nProtectionItemOwnCount", SqlDbType.Int).Value = nProtectionItemOwnCount;
		sc.Parameters.Add("@nProtectionItemUnOwnCount", SqlDbType.Int).Value = nProtectionItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMainGearRefinmenetDetailLog(Guid detailLogId, Guid logId, int nTurn, int nIndex, int nGrade, int nAttrId, long lnAttrValueId, bool bProtected)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMainGearRefinementDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nTurn", SqlDbType.Int).Value = nTurn;
		sc.Parameters.Add("@nIndex", SqlDbType.Int).Value = nIndex;
		sc.Parameters.Add("@nGrade", SqlDbType.Int).Value = nGrade;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		sc.Parameters.Add("@lnAttrValueId", SqlDbType.BigInt).Value = lnAttrValueId;
		sc.Parameters.Add("@bProtected", SqlDbType.Bit).Value = bProtected;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMainGearRefinementApplicationLog(Guid logId, int nIndex, Guid heroId, Guid heroMainGearId, int nOldGrade, int nGrade, int nOldAttrId, int nAttrId, long lnOldAttrValueId, long lnAttrValueId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMainGearRefinementApplicationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nIndex", SqlDbType.Int).Value = nIndex;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@nOldGrade", SqlDbType.Int).Value = nOldGrade;
		sc.Parameters.Add("@nGrade", SqlDbType.Int).Value = nGrade;
		sc.Parameters.Add("@nOldAttrId", SqlDbType.Int).Value = nOldAttrId;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		sc.Parameters.Add("@lnOldAttrValueId", SqlDbType.BigInt).Value = lnOldAttrValueId;
		sc.Parameters.Add("@lnAttrValueId", SqlDbType.BigInt).Value = lnAttrValueId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMainGearDisassembleLog(Guid logId, Guid heroId, Guid heroMainGearId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMainGearDisassembleLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMainGearDisassembleDetailLog(Guid logId, int nNo, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMainGearDisassembleDetailLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nNo", SqlDbType.Int).Value = nNo;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMainGearEnchantLevelSetActiviationLog(Guid logId, Guid heroId, int nSetNo, Guid weaponHeroMainGearId, int nWeaponHeroMainGearEnchantLevel, Guid armorHeroMainGearId, int nArmorHeroMainGearEnchantLevel, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMainGearEnchantLevelSetActivationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSetNo", SqlDbType.Int).Value = nSetNo;
		sc.Parameters.Add("@weaponHeroMainGearId", SqlDbType.UniqueIdentifier).Value = weaponHeroMainGearId;
		sc.Parameters.Add("@nWeaponHeroMainGearEnchantLevel", SqlDbType.Int).Value = nWeaponHeroMainGearEnchantLevel;
		sc.Parameters.Add("@armorHeroMainGearId", SqlDbType.UniqueIdentifier).Value = armorHeroMainGearId;
		sc.Parameters.Add("@nArmorHeroMainGearEnchantLevel", SqlDbType.Int).Value = nArmorHeroMainGearEnchantLevel;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroSkillLevelUpLog(Guid logId, Guid heroId, int nSkillId, int nOldLevel, int nLevel, long lnUsedGold, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroSkillLevelUpLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSkillId", SqlDbType.Int).Value = nSkillId;
		sc.Parameters.Add("@nOldLevel", SqlDbType.Int).Value = nOldLevel;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@nMaterialItemId", SqlDbType.Int).Value = nMaterialItemId;
		sc.Parameters.Add("@nMaterialItemOwnCount", SqlDbType.Int).Value = nMaterialItemOwnCount;
		sc.Parameters.Add("@nMaterialItemUnOwnCount", SqlDbType.Int).Value = nMaterialItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddSimpleShopProductBuyLog(Guid logId, Guid heroId, int nProductId, int nBuyCount, int nItemId, int nItemCount, bool bItemOwned, long lnUsedGold, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSimpleShopProductBuyLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		sc.Parameters.Add("@nBuyCount", SqlDbType.Int).Value = nBuyCount;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddSimpleShopSellLog(Guid logId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSimpleShopSellLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddSimpleSellDetailLog(Guid detailLogId, Guid logId, int nSellSlotIndex, int nInventorySlotIndex, int nType, Guid? heroMainGearId, int nItemId, int nItemCount, bool bItemOwned, Guid? heroMountGearId, long lnGainedGold)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSimpleShopSellDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nSellSlotIndex", SqlDbType.Int).Value = nSellSlotIndex;
		sc.Parameters.Add("@nInventorySlotIndex", SqlDbType.Int).Value = nInventorySlotIndex;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = SFDBUtil.NullToDBNull(heroMainGearId);
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@heroMountGearId", SqlDbType.UniqueIdentifier).Value = SFDBUtil.NullToDBNull(heroMountGearId);
		sc.Parameters.Add("@lnGainedGold", SqlDbType.BigInt).Value = lnGainedGold;
		return sc;
	}

	public static SqlCommand CSC_AddHeroRevivalLog(Guid logId, Guid heroId, int nType, int nOwnDia, int nUnOwnDia, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroRevivalLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@nOwnDia", SqlDbType.Int).Value = nOwnDia;
		sc.Parameters.Add("@nUnOwnDia", SqlDbType.Int).Value = nUnOwnDia;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddInventorySlotExtendLog(Guid logId, Guid heroId, int nOldPaidInventorySlotcount, int nPaidInventorySlotCount, int nUsedOwnDia, int nUsedUnOwnDia, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddInventorySlotExtendLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nOldPaidInventorySlotcount", SqlDbType.Int).Value = nOldPaidInventorySlotcount;
		sc.Parameters.Add("@nPaidInventorySlotCount", SqlDbType.Int).Value = nPaidInventorySlotCount;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddItemCompositionLog(Guid logId, Guid heroId, int nItemId, int nItemCount, bool bItemOwned, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, long lnUsedGold, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddItemCompositionLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@nMaterialItemId", SqlDbType.Int).Value = nMaterialItemId;
		sc.Parameters.Add("@nMaterialItemOwnCount", SqlDbType.Int).Value = nMaterialItemOwnCount;
		sc.Parameters.Add("@nMaterialItemUnOwnCount", SqlDbType.Int).Value = nMaterialItemUnOwnCount;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddItemUseLog(Guid logId, Guid heroId, int nItemId, int nItemOwnCount, int nItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddItemUseLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemOwnCount", SqlDbType.Int).Value = nItemOwnCount;
		sc.Parameters.Add("@nItemUnOwnCount", SqlDbType.Int).Value = nItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddMainGearItemUseDetailLog(Guid detailLogId, Guid logId, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddMainGearBoxItemUseDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddPickBoxItemUseDetailLog(Guid detailLogId, Guid logId, int nType, Guid? heroMainGearId, int? nItemId, int? nItemCount, bool? bItemOwned, Guid? heroMountGearId, int? nCreatureCardId, int? nCreatureId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddPickBoxItemUseDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = SFDBUtil.NullToDBNull(heroMainGearId);
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = SFDBUtil.NullToDBNull(nItemId);
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = SFDBUtil.NullToDBNull(nItemCount);
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = SFDBUtil.NullToDBNull(bItemOwned);
		sc.Parameters.Add("@heroMountGearId", SqlDbType.UniqueIdentifier).Value = SFDBUtil.NullToDBNull(heroMountGearId);
		sc.Parameters.Add("@nCreatureCardId", SqlDbType.Int).Value = SFDBUtil.NullToDBNull(nCreatureCardId);
		sc.Parameters.Add("@nCreatureId", SqlDbType.Int).Value = SFDBUtil.NullToDBNull(nCreatureId);
		return sc;
	}

	public static SqlCommand CSC_AddHeroRestRewardLog(Guid logId, Guid heroId, int nLevel, int nRestTime, long lnRewardExp, int nType, long lnUsedGold, int nUsedOwnDia, int nUsedUnOwnDia, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroRestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nRestTime", SqlDbType.Int).Value = nRestTime;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddChattingLog(Guid logId, int nChattingType, int nLinkType, Guid senderId, Guid? targetId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddChattingLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nChattingType", SqlDbType.Int).Value = nChattingType;
		sc.Parameters.Add("@nLinkType", SqlDbType.Int).Value = nLinkType;
		sc.Parameters.Add("@senderId", SqlDbType.UniqueIdentifier).Value = senderId;
		sc.Parameters.Add("@targetId", SqlDbType.UniqueIdentifier).Value = SFDBUtil.NullToDBNull(targetId);
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddChattingMessageLog(Guid logId, int nNo, string sMessage)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddChattingMessageLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nNo", SqlDbType.Int).Value = nNo;
		sc.Parameters.Add("@sMessage", SqlDbType.NVarChar).Value = sMessage;
		return sc;
	}

	public static SqlCommand CSC_AddLevelUpRewardLog(Guid logId, Guid heroId, int nLevel, int nEntryId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddLevelUpRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nEntryId", SqlDbType.Int).Value = nEntryId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddLevelUpRewardDetailLog(Guid detailLogId, Guid logId, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddLevelUpRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddDailyAttendRewardLog(Guid logId, Guid heroId, int nDay, int nDailyRewardItemId, int nDailyRewardItemCount, bool bDailyRewardItemOwned, int nWeekendRewardItemId, int nWeekendRewardItemCount, bool bWeekendRewardItemOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddDailyAttendRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDay", SqlDbType.Int).Value = nDay;
		sc.Parameters.Add("@nDailyRewardItemId", SqlDbType.Int).Value = nDailyRewardItemId;
		sc.Parameters.Add("@nDailyRewardItemCount", SqlDbType.Int).Value = nDailyRewardItemCount;
		sc.Parameters.Add("@bDailyRewardItemOwned", SqlDbType.Bit).Value = bDailyRewardItemOwned;
		sc.Parameters.Add("@nWeekendRewardItemId", SqlDbType.Int).Value = nWeekendRewardItemId;
		sc.Parameters.Add("@nWeekendRewardItemCount", SqlDbType.Int).Value = nWeekendRewardItemCount;
		sc.Parameters.Add("@bWeekendRewardItemOwned", SqlDbType.Bit).Value = bWeekendRewardItemOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddAccessRewardLog(Guid logId, Guid heroId, float fAccessTime, int nEntryId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAccessRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@fAccessTime", SqlDbType.Float).Value = fAccessTime;
		sc.Parameters.Add("@nEntryId", SqlDbType.Int).Value = nEntryId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddAccessRewardDetailLog(Guid detailLogId, Guid logId, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAccessRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMountLevelUpLog(Guid logId, Guid heroId, int nMountId, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, int nOldLevel, int nLevel, int nOldSatiety, int nSatiety, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMountLevelUpLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMountId", SqlDbType.Int).Value = nMountId;
		sc.Parameters.Add("@nMaterialItemId", SqlDbType.Int).Value = nMaterialItemId;
		sc.Parameters.Add("@nMaterialItemOwnCount", SqlDbType.Int).Value = nMaterialItemOwnCount;
		sc.Parameters.Add("@nMaterialItemUnOwnCount", SqlDbType.Int).Value = nMaterialItemUnOwnCount;
		sc.Parameters.Add("@nOldLevel", SqlDbType.Int).Value = nOldLevel;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nOldSatiety", SqlDbType.Int).Value = nOldSatiety;
		sc.Parameters.Add("@nSatiety", SqlDbType.Int).Value = nSatiety;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMountGearRefinementLog(Guid logId, Guid heroId, Guid heroMountGearId, bool bOldOwned, bool bOwned, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, int nIndex, int nOldGrade, int nGrade, int nOldAttrId, int nAttrId, long lnOldAttrValueId, long lnAttrValueId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMountGearRefinementLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@heroMountGearId", SqlDbType.UniqueIdentifier).Value = heroMountGearId;
		sc.Parameters.Add("@bOldOwned", SqlDbType.Bit).Value = bOldOwned;
		sc.Parameters.Add("@bOwned", SqlDbType.Bit).Value = bOwned;
		sc.Parameters.Add("@nMaterialItemId", SqlDbType.Int).Value = nMaterialItemId;
		sc.Parameters.Add("@nMaterialItemOwnCount", SqlDbType.Int).Value = nMaterialItemOwnCount;
		sc.Parameters.Add("@nMaterialItemUnOwnCount", SqlDbType.Int).Value = nMaterialItemUnOwnCount;
		sc.Parameters.Add("@nIndex", SqlDbType.Int).Value = nIndex;
		sc.Parameters.Add("@nOldGrade", SqlDbType.Int).Value = nOldGrade;
		sc.Parameters.Add("@nGrade", SqlDbType.Int).Value = nGrade;
		sc.Parameters.Add("@nOldAttrId", SqlDbType.Int).Value = nOldAttrId;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		sc.Parameters.Add("@lnOldAttrValueId", SqlDbType.BigInt).Value = lnOldAttrValueId;
		sc.Parameters.Add("@lnAttrValueId", SqlDbType.BigInt).Value = lnAttrValueId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMountGearPickBoxMakingLog(Guid logId, Guid heroId, long lnUsedGold, int nItemId, int nItemCount, bool bItemOwned, int nMaterialItem1Id, int nMaterialItem1OwnCount, int nMaterialItem1UnOwnCount, int nMaterialItem2Id, int nMaterialItem2OwnCount, int nMaterialItem2UnOwnCount, int nMaterialItem3Id, int nMaterialItem3OwnCount, int nMaterialItem3UnOwnCount, int nMaterialItem4Id, int nMaterialItem4OwnCount, int nMaterialItem4UnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMountGearPickBoxMakingLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@nMaterialItem1Id", SqlDbType.Int).Value = nMaterialItem1Id;
		sc.Parameters.Add("@nMaterialItem1OwnCount", SqlDbType.Int).Value = nMaterialItem1OwnCount;
		sc.Parameters.Add("@nMaterialItem1UnOwnCount", SqlDbType.Int).Value = nMaterialItem1UnOwnCount;
		sc.Parameters.Add("@nMaterialItem2Id", SqlDbType.Int).Value = nMaterialItem2Id;
		sc.Parameters.Add("@nMaterialItem2OwnCount", SqlDbType.Int).Value = nMaterialItem2OwnCount;
		sc.Parameters.Add("@nMaterialItem2UnOwnCount", SqlDbType.Int).Value = nMaterialItem2UnOwnCount;
		sc.Parameters.Add("@nMaterialItem3Id", SqlDbType.Int).Value = nMaterialItem3Id;
		sc.Parameters.Add("@nMaterialItem3OwnCount", SqlDbType.Int).Value = nMaterialItem3OwnCount;
		sc.Parameters.Add("@nMaterialItem3UnOwnCount", SqlDbType.Int).Value = nMaterialItem3UnOwnCount;
		sc.Parameters.Add("@nMaterialItem4Id", SqlDbType.Int).Value = nMaterialItem4Id;
		sc.Parameters.Add("@nMaterialItem4OwnCount", SqlDbType.Int).Value = nMaterialItem4OwnCount;
		sc.Parameters.Add("@nMaterialItem4UnOwnCount", SqlDbType.Int).Value = nMaterialItem4UnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMountAwakeningLog(Guid logId, Guid heroId, int nMountId, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, int nOldAwakeningLevel, int nAwakeningLevel, int nOldAwakeningExp, int nAwakeningExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMountAwakeningLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMountId", SqlDbType.Int).Value = nMountId;
		sc.Parameters.Add("@nMaterialItemId", SqlDbType.Int).Value = nMaterialItemId;
		sc.Parameters.Add("@nMaterialItemOwnCount", SqlDbType.Int).Value = nMaterialItemOwnCount;
		sc.Parameters.Add("@nMaterialItemUnOwnCount", SqlDbType.Int).Value = nMaterialItemUnOwnCount;
		sc.Parameters.Add("@nOldAwakeningLevel", SqlDbType.Int).Value = nOldAwakeningLevel;
		sc.Parameters.Add("@nAwakeningLevel", SqlDbType.Int).Value = nAwakeningLevel;
		sc.Parameters.Add("@nOldAwakeningExp", SqlDbType.Int).Value = nOldAwakeningExp;
		sc.Parameters.Add("@nAwakeningExp", SqlDbType.Int).Value = nAwakeningExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMountPotionAttrLog(Guid logId, Guid heroId, int nMountId, int nCount, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMountPotionAttrLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMountId", SqlDbType.Int).Value = nMountId;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWingEnchantLog(Guid logId, Guid heroId, int nPartId, int nPickEnchantCount, int nEnchantCount, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, int nOldStep, int nOldLevel, int nOldExp, int nStep, int nLevel, int nExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWingEnchantLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nPartId", SqlDbType.Int).Value = nPartId;
		sc.Parameters.Add("@nPickEnchantCount", SqlDbType.Int).Value = nPickEnchantCount;
		sc.Parameters.Add("@nEnchantCount", SqlDbType.Int).Value = nEnchantCount;
		sc.Parameters.Add("@nMaterialItemId", SqlDbType.Int).Value = nMaterialItemId;
		sc.Parameters.Add("@nMaterialItemOwnCount", SqlDbType.Int).Value = nMaterialItemOwnCount;
		sc.Parameters.Add("@nMaterialItemUnOwnCount", SqlDbType.Int).Value = nMaterialItemUnOwnCount;
		sc.Parameters.Add("@nOldStep", SqlDbType.Int).Value = nOldStep;
		sc.Parameters.Add("@nOldLevel", SqlDbType.Int).Value = nOldLevel;
		sc.Parameters.Add("@nOldExp", SqlDbType.Int).Value = nOldExp;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nExp", SqlDbType.Int).Value = nExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddStoryDungeonPlayLog(Guid logId, Guid instanceId, Guid heroId, int nDungeonNo, int nDifficulty, int nStatus, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddStoryDungeonPlayLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDungeonNo", SqlDbType.Int).Value = nDungeonNo;
		sc.Parameters.Add("@nDifficulty", SqlDbType.Int).Value = nDifficulty;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateStoryDungeonPlayLog(Guid logId, int nStatus)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateStoryDungeonPlayLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		return sc;
	}

	public static SqlCommand CSC_AddStoryDungeonPlayRewardLog(Guid logId, int nNo, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddStoryDungeonPlayRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nNo", SqlDbType.Int).Value = nNo;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddExpDungeonPlayLog(Guid logId, Guid instanceId, Guid heroId, int nDifficulty, int nStatus, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddExpDungeonPlayLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDifficulty", SqlDbType.Int).Value = nDifficulty;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateExpDungeonPlayLog(Guid logId, int nStatus, long lnRewardExp)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateExpDungeonPlayLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		return sc;
	}

	public static SqlCommand CSC_AddGoldDungeonPlayLog(Guid logId, Guid instanceId, Guid heroId, int nDifficulty, int nStatus, long lnRewardGold, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGoldDungeonPlayLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDifficulty", SqlDbType.Int).Value = nDifficulty;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@lnRewardGold", SqlDbType.BigInt).Value = lnRewardGold;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateGoldDungeonPlayLog(Guid logId, int nStatus, long lnRewardGold)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGoldDungeonPlayLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@lnRewardGold", SqlDbType.Int).Value = lnRewardGold;
		return sc;
	}

	public static SqlCommand CSC_AddUndergroundMazePlayLog(Guid logId, Guid instanceId, Guid heroId, int nPlayTime, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddUndergroundMazePlayLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nPlayTime", SqlDbType.Int).Value = nPlayTime;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateUndergroundMazePlayLog(Guid logId, int nPlayTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateUndergroundMazePlayLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nPlayTime", SqlDbType.Int).Value = nPlayTime;
		return sc;
	}

	public static SqlCommand CSC_AddBountyHunterQuestRewardLog(Guid logId, Guid heroId, Guid instanceId, int nLevel, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddBountyHunterQuestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddFishingQuestRewardLog(Guid logId, Guid heroId, DateTime questDate, int nQuestCount, int nBaitItemId, int nCastingCount, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFishingQuestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@questDate", SqlDbType.Date).Value = questDate;
		sc.Parameters.Add("@nQuestCount", SqlDbType.Int).Value = nQuestCount;
		sc.Parameters.Add("@nBaitItemId", SqlDbType.Int).Value = nBaitItemId;
		sc.Parameters.Add("@nCastingCount", SqlDbType.Int).Value = nCastingCount;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddArtifactRoomPlayLog(Guid logId, Guid heroId, int nType, int nOldFloor, int nFloor, int nBestFloor, int nUsedOwnDia, int nUsedUnOwnDia, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddArtifactRoomPlayLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@nOldFloor", SqlDbType.Int).Value = nOldFloor;
		sc.Parameters.Add("@nFloor", SqlDbType.Int).Value = nFloor;
		sc.Parameters.Add("@nBestFloor", SqlDbType.Int).Value = nBestFloor;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddArtifactRoomInitLog(Guid logId, Guid heroId, int nBestFloor, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddArtifactRoomInitLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nBestFloor", SqlDbType.Int).Value = nBestFloor;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddMysteryBoxQuestRewardLog(Guid logId, Guid heroId, Guid instanceId, int nRewardExploitPoint, int nAcquiredExploitPoint, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddMysteryBoxQuestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nRewardExploitPoint", SqlDbType.Int).Value = nRewardExploitPoint;
		sc.Parameters.Add("@nAcquiredExploitPoint", SqlDbType.Int).Value = nAcquiredExploitPoint;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddSecretLetterQuestRewardLog(Guid logId, Guid heroId, Guid instanceId, int nRewardExploitPoint, int nAcquiredExploitPoint, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSecretLetterQuestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nRewardExploitPoint", SqlDbType.Int).Value = nRewardExploitPoint;
		sc.Parameters.Add("@nAcquiredExploitPoint", SqlDbType.Int).Value = nAcquiredExploitPoint;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddDimensionRaidQuestRewardLog(Guid logId, Guid heroId, Guid instanceId, int nRewardExploitPoint, int nAcquiredExploitPoint, long lnRewardExp, int nItemId, int nItemCount, bool bItemOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddDimensionRaidQuestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nRewardExploitPoint", SqlDbType.Int).Value = nRewardExploitPoint;
		sc.Parameters.Add("@nAcquiredExploitPoint", SqlDbType.Int).Value = nAcquiredExploitPoint;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHolyWarQuestRewardLog(Guid logId, Guid heroId, Guid instanceId, int nKillCount, int nRewardExploitPoint, int nAcquiredExploitPoint, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHolyWarQuestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nKillCount", SqlDbType.Int).Value = nKillCount;
		sc.Parameters.Add("@nRewardExploitPoint", SqlDbType.Int).Value = nRewardExploitPoint;
		sc.Parameters.Add("@nAcquiredExploitPoint", SqlDbType.Int).Value = nAcquiredExploitPoint;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddSeriesMissionRewardLog(Guid logId, Guid heroId, int nMissionId, int nStep, int nProgressCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSeriesMissionRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMissionId", SqlDbType.Int).Value = nMissionId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddSeriesMissionRewardDetailLog(Guid detailLogId, Guid logId, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSeriesMissionRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddTodayMissionRewardLog(Guid logId, Guid heroId, int nMissionId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddTodayMissionRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMissionId", SqlDbType.Int).Value = nMissionId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddTodayMissionRewardDetailLog(Guid detailLogId, Guid logId, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddTodayMissionRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddAchievementRewardLog(Guid logId, Guid heroId, int nAchievementPoint, int nRewardNo, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAchievementRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nAchievementPoint", SqlDbType.Int).Value = nAchievementPoint;
		sc.Parameters.Add("@nRewardNo", SqlDbType.Int).Value = nRewardNo;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddAchievementRewardDetailLog(Guid detailLogId, Guid logId, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAchievementRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddAncientRelicRewardLog(Guid logId, Guid heroId, Guid instanceId, int nStep, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAncientRelicRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddAncientRelicRwardDetailLog(Guid detailLogId, Guid logId, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAncientRelicRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddVipLevelRewardLog(Guid logId, Guid accountId, Guid heroId, int nVipLevel, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddVipLevelRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nVipLevel", SqlDbType.Int).Value = nVipLevel;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddVipLevelRewardDetailLog(Guid detailLogId, Guid logId, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddVipLevelRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddRankRewardLog(Guid logId, Guid heroId, int nRankNo, long lnRewardGold, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddRankRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRankNo", SqlDbType.Int).Value = nRankNo;
		sc.Parameters.Add("@lnRewardGold", SqlDbType.BigInt).Value = lnRewardGold;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddRankRewardDetailLog(Guid detailLogId, Guid logId, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddRankRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddHonorShopProductBuyLog(Guid logId, Guid heroId, int nProductId, int nBuyCount, int nUsedHonorPoint, int nItemId, bool bItemOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHonorShopProductBuyLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		sc.Parameters.Add("@nBuyCount", SqlDbType.Int).Value = nBuyCount;
		sc.Parameters.Add("@nUsedHonorPoint", SqlDbType.Int).Value = nUsedHonorPoint;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddLevelRankingRewardLog(Guid logId, Guid heroId, int nRankingNo, int nRanking, int nItemId, int nItemCount, bool bItemOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddLevelRankingRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		sc.Parameters.Add("@nRanking", SqlDbType.Int).Value = nRanking;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Int).Value = bItemOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddAttainmentEntryRewardLog(Guid logId, Guid heroId, int nEntryNo, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAttainmentEntryRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nEntryNo", SqlDbType.Int).Value = nEntryNo;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddAttainmentEntryRewardDetailLog(Guid detailLogId, Guid logId, int nType, Guid heroMainGearId, int nItemId, int nItemCount, bool bItemOwned, int nContentId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAttainmentEntryRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@nContentId", SqlDbType.Int).Value = nContentId;
		return sc;
	}

	public static SqlCommand CSC_AddFieldOfHonorRankingRewardLog(Guid logId, Guid heroId, int nRankingNo, int nRanking, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldOfHonorRankingRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		sc.Parameters.Add("@nRanking", SqlDbType.Int).Value = nRanking;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddFieldOfHonorRankingRewardDetailLog(Guid detailLogId, Guid logId, int nItemId, int nItemCount, bool bItemOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldOfHonorRankingRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddSupplySupportQuestStartLog(Guid logId, Guid heroId, Guid instanceId, int nOrderItemId, int nOrderItemCount, bool bOrderItemOwned, int nUsedGuaranteeGold, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSupplySupportQuestStartLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nOrderItemId", SqlDbType.Int).Value = nOrderItemId;
		sc.Parameters.Add("@nOrderItemCount", SqlDbType.Int).Value = nOrderItemCount;
		sc.Parameters.Add("@bOrderItemOwned", SqlDbType.Bit).Value = bOrderItemOwned;
		sc.Parameters.Add("@nUsedGuaranteeGold", SqlDbType.Int).Value = nUsedGuaranteeGold;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddSupplySupportQuestRewardLog(Guid logId, Guid heroId, Guid instanceId, int nStatus, int nCartId, long lnRewardExp, long lnRewardGold, int nRewardExploitPoint, int nAcquiredExploitPoint, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSupplySupportQuestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@nCartId", SqlDbType.Int).Value = nCartId;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@lnRewardGold", SqlDbType.BigInt).Value = lnRewardGold;
		sc.Parameters.Add("@nRewardExploitPoint", SqlDbType.Int).Value = nRewardExploitPoint;
		sc.Parameters.Add("@nAcquiredExploitPoint", SqlDbType.Int).Value = nAcquiredExploitPoint;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddSupplySupportQuestCartDestructionRwardLog(Guid logId, Guid heroId, Guid instanceId, int nCartId, int nItemId, int nItemCount, bool bItemOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSupplySupportQuestCartDestructionRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nCartId", SqlDbType.Int).Value = nCartId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildCreationLog(Guid logId, Guid guildId, Guid heroId, int nUsedOwnDia, int nUsedUnOwnDia, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildCreationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildDonationLog(Guid logId, Guid guildId, Guid heroId, int nEntryId, long lnUsedGold, int nUsedOwnDia, int nUsedUnOwnDia, int nRewardGuildContributionPoint, int nRewardGuildFund, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildDonationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nEntryId", SqlDbType.Int).Value = nEntryId;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@nRewardGuildContributionPoint", SqlDbType.Int).Value = nRewardGuildContributionPoint;
		sc.Parameters.Add("@nRewardGuildFund", SqlDbType.Int).Value = nRewardGuildFund;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildApplicationAcceptanceLog(Guid logId, Guid guildId, Guid acceptanceHeroId, int nGuildMemberGrade, Guid applicationHeroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildApplicationAcceptanceLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@acceptanceHeroId", SqlDbType.UniqueIdentifier).Value = acceptanceHeroId;
		sc.Parameters.Add("@nGuildMemberGrade", SqlDbType.Int).Value = nGuildMemberGrade;
		sc.Parameters.Add("@applicationHeroId", SqlDbType.UniqueIdentifier).Value = applicationHeroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildInvitationAcceptanceLog(Guid logId, Guid guildId, Guid invitationHeroId, Guid acceptanceHeroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildInvitationAcceptanceLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@invitationHeroId", SqlDbType.UniqueIdentifier).Value = invitationHeroId;
		sc.Parameters.Add("@acceptanceHeroId", SqlDbType.UniqueIdentifier).Value = acceptanceHeroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildAppointmentLog(Guid logId, Guid guildId, Guid heroId, int nGuildMemberGrade, Guid targetHeroId, int nOldTargetHeroGuildMemberGrade, int nTargetHeroGuildMemberGrade, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildAppointmentLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nGuildMemberGrade", SqlDbType.Int).Value = nGuildMemberGrade;
		sc.Parameters.Add("@targetHeroId", SqlDbType.UniqueIdentifier).Value = targetHeroId;
		sc.Parameters.Add("@nOldTargetHeroGuildMemberGrade", SqlDbType.Int).Value = nOldTargetHeroGuildMemberGrade;
		sc.Parameters.Add("@nTargetHeroGuildMemberGrade", SqlDbType.Int).Value = nTargetHeroGuildMemberGrade;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildBanishmentLog(Guid logId, Guid guildId, Guid heroId, int nGuildMemberGrade, Guid banishedHeroId, int nBanishedHeroGuildMemberGrade, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildBanishmentLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nGuildMemberGrade", SqlDbType.Int).Value = nGuildMemberGrade;
		sc.Parameters.Add("@banishedHeroId", SqlDbType.UniqueIdentifier).Value = banishedHeroId;
		sc.Parameters.Add("@nBanishedHeroGuildMemberGrade", SqlDbType.Int).Value = nBanishedHeroGuildMemberGrade;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildFarmQuestRewardLog(Guid logId, Guid guildId, Guid heroId, Guid questInstanceId, int nItemId, int nItemCount, bool bItemOwned, long lnRewardExp, int nRewardGuildContributionPoint, int nRewardGuildBuildingPoint, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildFarmQuestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@nRewardGuildContributionPoint", SqlDbType.Int).Value = nRewardGuildContributionPoint;
		sc.Parameters.Add("@nRewardGuildBuildingPoint", SqlDbType.Int).Value = nRewardGuildBuildingPoint;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildBuildingLevelUpLog(Guid logId, Guid guildId, Guid heroId, int nBuildingId, int nOldLevel, int nLevel, int nGuildLobbyLevel, int nGuildBuildingPoint, int nUsedGuildFund, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildBuildingLevelUpLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nBuildingId", SqlDbType.Int).Value = nBuildingId;
		sc.Parameters.Add("@nOldLevel", SqlDbType.Int).Value = nOldLevel;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nGuildLobbyLevel", SqlDbType.Int).Value = nGuildLobbyLevel;
		sc.Parameters.Add("@nGuildBuildingPoint", SqlDbType.Int).Value = nGuildBuildingPoint;
		sc.Parameters.Add("@nUsedGuildFund", SqlDbType.Int).Value = nUsedGuildFund;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroGuildSkillLevelUpLog(Guid logId, Guid heroId, Guid guildId, int nGuildSkillId, int nOldLevel, int nLevel, int nUsedGuildContributionPoint, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroGuildSkillLevelUpLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nGuildSkillId", SqlDbType.Int).Value = nGuildSkillId;
		sc.Parameters.Add("@nOldLevel", SqlDbType.Int).Value = nOldLevel;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nUsedGuildContributionPoint", SqlDbType.Int).Value = nUsedGuildContributionPoint;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildFoodWarehouseStockLog(Guid logId, Guid guildId, Guid heroId, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, int nOldLevel, int nOldExp, int nLevel, int nExp, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildFoodWarehouseStockLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@nOldLevel", SqlDbType.Int).Value = nOldLevel;
		sc.Parameters.Add("@nOldExp", SqlDbType.Int).Value = nOldExp;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nExp", SqlDbType.Int).Value = nExp;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildFoodWarehouseCollectionLog(Guid collectionId, Guid guildId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildFoodWarehouseCollectionLog";
		sc.Parameters.Add("@collectionId", SqlDbType.UniqueIdentifier).Value = collectionId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildFoodWarehouseRewardLog(Guid logId, Guid collectionId, Guid guildId, Guid heroId, int nItemId, int nItemCount, bool bItemOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildFoodWarehouseRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@collectionId", SqlDbType.UniqueIdentifier).Value = collectionId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildAltarDonationLog(Guid logId, Guid guildId, Guid heroId, int nUsedGold, int nRewardMoralPoint, int nAddedMoralPoint, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildAltarDonationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nUsedGold", SqlDbType.Int).Value = nUsedGold;
		sc.Parameters.Add("@nRewardMoralPoint", SqlDbType.Int).Value = nRewardMoralPoint;
		sc.Parameters.Add("@nAddedMoralPoint", SqlDbType.Int).Value = nAddedMoralPoint;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildAltarSpellInjectionLog(Guid logId, Guid guildId, Guid heroId, int nRewardMoralPoint, int nAddedMoralPoint, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildAltarSpellInjectionLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardMoralPoint", SqlDbType.Int).Value = nRewardMoralPoint;
		sc.Parameters.Add("@nAddedMoralPoint", SqlDbType.Int).Value = nAddedMoralPoint;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildAltarDefenseLog(Guid logId, Guid guildId, Guid heroId, int nLevel, int nGuildLevel, int nRewardMoralPoint, int nAddedMoralPoint, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildAltarDefenseLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nGuildLevel", SqlDbType.Int).Value = nGuildLevel;
		sc.Parameters.Add("@nRewardMoralPoint", SqlDbType.Int).Value = nRewardMoralPoint;
		sc.Parameters.Add("@nAddedMoralPoint", SqlDbType.Int).Value = nAddedMoralPoint;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildAltarRewardLog(Guid logId, Guid guildId, Guid heroId, int nGuildMoralPoint, int nGuildLevel, int nItemId, int nItemCount, bool bItemOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildAltarRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nGuildMoralPoint", SqlDbType.Int).Value = nGuildMoralPoint;
		sc.Parameters.Add("@nGuildLevel", SqlDbType.Int).Value = nGuildLevel;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildAltarCompletionRewardLog(Guid logId, Guid guildId, Guid heroId, long lnRewardExp, int nRewardGuildContributionPoint, int nRewardGuildFund, int nRewardguildBuildingPoint, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildAltarCompletionRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@nRewardGuildContributionPoint", SqlDbType.Int).Value = nRewardGuildContributionPoint;
		sc.Parameters.Add("@nRewardGuildFund", SqlDbType.Int).Value = nRewardGuildFund;
		sc.Parameters.Add("@nRewardGuildBuildingPoint", SqlDbType.Int).Value = nRewardguildBuildingPoint;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildMissionRewardLog(Guid logId, Guid guildId, Guid heroId, Guid missionInstanceId, int nMissionId, int nRewardGuildContributionPoint, int nRewardGuildFund, int nRewardGuildBuildingPoint, long lnRewardExp, int nRewardItemId, int nRewardItemCount, bool bRewardItmeOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildMissionRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@missionInstanceId", SqlDbType.UniqueIdentifier).Value = missionInstanceId;
		sc.Parameters.Add("@nMissionId", SqlDbType.Int).Value = nMissionId;
		sc.Parameters.Add("@nRewardGuildContributionPoint", SqlDbType.Int).Value = nRewardGuildContributionPoint;
		sc.Parameters.Add("@nRewardGuildFund", SqlDbType.Int).Value = nRewardGuildFund;
		sc.Parameters.Add("@nRewardGuildBuildingPoint", SqlDbType.Int).Value = nRewardGuildBuildingPoint;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItmeOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildDailyItemRewardLog(Guid logId, Guid guildId, Guid heroId, int nItemId, int nItemCount, bool bItemOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildDailyItemRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildBlessingBuffBuyLog(Guid logId, Guid guildId, Guid heroId, int nMemberGrade, int nBuffId, int nUsedOwnDia, int nUsedUnOwnDia, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildBlessingBuffBuyLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMemberGrade", SqlDbType.Int).Value = nMemberGrade;
		sc.Parameters.Add("@nBuffId", SqlDbType.Int).Value = nBuffId;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWingOpenLog(Guid logId, Guid heroId, int nRewardWingId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWingOpenLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardWingId", SqlDbType.Int).Value = nRewardWingId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddNationNoblesseApponitmentLog(Guid logId, int nNationId, Guid heroId, int nNoblesseId, Guid targetHeroId, int nTargetNoblesseId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationNoblesseAppointmentLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nNoblesseId", SqlDbType.Int).Value = nNoblesseId;
		sc.Parameters.Add("@targetHeroId", SqlDbType.UniqueIdentifier).Value = targetHeroId;
		sc.Parameters.Add("@nTargetNoblesseId", SqlDbType.Int).Value = nTargetNoblesseId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddNationWarDeclarationLog(Guid logId, int nType, Guid declarationId, int nNationId, Guid heroId, int nTargetNationId, int nUsedNationFund, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationWarDeclarationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = declarationId;
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nTargetNationId", SqlDbType.Int).Value = nTargetNationId;
		sc.Parameters.Add("@nUsedNationFund", SqlDbType.Int).Value = nUsedNationFund;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddNationWarHeroTransmissionLog(Guid logId, Guid declarationId, Guid heroId, int nType, int nCount, int nTargetArrangeId, int nUsedUnOwnDia, int nUsedOwnDia, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationWarHeroTransmissionLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = declarationId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		sc.Parameters.Add("@nTargetArrangeId", SqlDbType.Int).Value = nTargetArrangeId;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddNationWarCallLog(Guid logId, Guid declarationId, Guid heroId, int nContinentId, float fXPosition, float fYPosition, float fZPosition, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationWarCallLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = declarationId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nContinentId", SqlDbType.Int).Value = nContinentId;
		sc.Parameters.Add("@fXPosition", SqlDbType.Float).Value = fXPosition;
		sc.Parameters.Add("@fYPosition", SqlDbType.Float).Value = fYPosition;
		sc.Parameters.Add("@fZPosition", SqlDbType.Float).Value = fZPosition;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddNationWarConvergingAttackLog(Guid logId, Guid declarationId, Guid heroId, int nTargetArrangeId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationWarConvergingAttackLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = declarationId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nTargetArrangeId", SqlDbType.Int).Value = nTargetArrangeId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddNationWarRerwardLog(Guid logId, Guid declarationId, Guid heroId, bool bIsWin, int nRewardExploitPoint, int nAcquiredExploitPoint, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationWarRerwardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = declarationId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@bIsWin", SqlDbType.Bit).Value = bIsWin;
		sc.Parameters.Add("@nRewardExploitPoint", SqlDbType.Int).Value = nRewardExploitPoint;
		sc.Parameters.Add("@nAcquiredExploitPoint", SqlDbType.Int).Value = nAcquiredExploitPoint;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddNationWarRewardDetailLog(Guid detailLogId, Guid logId, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationWarRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Int).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddNationWarHeroObjectiveRewardLog(Guid logId, Guid declarationId, Guid heroId, bool bIsWin, int nKillCount, int nImmediateRevivalCount, int nRewardOwnDia, int nRewardExploitPoint, int nAcquiredExploitPoint, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationWarHeroObjectiveRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = declarationId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@bIsWin", SqlDbType.Bit).Value = bIsWin;
		sc.Parameters.Add("@nKillCount", SqlDbType.Int).Value = nKillCount;
		sc.Parameters.Add("@nImmediateRevivalCount", SqlDbType.Int).Value = nImmediateRevivalCount;
		sc.Parameters.Add("@nRewardOwnDia", SqlDbType.Int).Value = nRewardOwnDia;
		sc.Parameters.Add("@nRewardExploitPoint", SqlDbType.Int).Value = nRewardExploitPoint;
		sc.Parameters.Add("@nAcquiredExploitPoint", SqlDbType.Int).Value = nAcquiredExploitPoint;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddNationWarRankingRewardLog(Guid logId, Guid heroId, int nType, int nRanking, int nItemId, int nItemCount, bool bItemOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationWarRankingRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@nRanking", SqlDbType.Int).Value = nRanking;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Int).Value = bItemOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddNationWarAllianceRewardLog(Guid logId, Guid declarationId, Guid heroId, bool bIsWin, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationWarAllianceRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = declarationId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@bIsWin", SqlDbType.Bit).Value = bIsWin;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddNationWarNationPowerReward(Guid logId, Guid declarationId, int nNationId, bool bIsWin, int nAcquiredNationPower, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationWarNationPowerReward";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@bIsWin", SqlDbType.Bit).Value = bIsWin;
		sc.Parameters.Add("@nAcquiredNationPower", SqlDbType.Int).Value = nAcquiredNationPower;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddNationDonationLog(Guid logId, Guid heroId, int nEntryId, long lnUsedGold, int nUsedOwnDia, int nUsedUnOwnDia, int nRewardExploitPoint, int nAcquiredExploitPoint, int nRewardNationFund)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationDonationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nEntryId", SqlDbType.Int).Value = nEntryId;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@nRewardExploitPoint", SqlDbType.Int).Value = nRewardExploitPoint;
		sc.Parameters.Add("@nAcquiredExploitPoint", SqlDbType.Int).Value = nAcquiredExploitPoint;
		sc.Parameters.Add("@nRewardNationFund", SqlDbType.Int).Value = nRewardNationFund;
		return sc;
	}

	public static SqlCommand CSC_AddGuildSupplySupportQuestRewardLog(Guid logId, Guid guildId, Guid heroId, Guid questInstanceId, int nRewardGuildBuildingPoint, int nRewardGuildFund, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildSupplySupportQuestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nRewardGuildBuildingPoint", SqlDbType.Int).Value = nRewardGuildBuildingPoint;
		sc.Parameters.Add("@nRewardGuildFund", SqlDbType.Int).Value = nRewardGuildFund;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildSupplySupportQuestExtraRewardLog(Guid logId, Guid guildId, Guid heroId, Guid questInstanceId, int nRewardGuildContributionPoint, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildSupplySupportQuestExtraRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nRewardGuildContributionPoint", SqlDbType.Int).Value = nRewardGuildContributionPoint;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddSoulCoveterRewardLog(Guid logId, Guid heroId, Guid instanceId, int nDifficulty, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSoulCoveterRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nDifficulty", SqlDbType.Int).Value = nDifficulty;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddSoulCoveterRewardDetailLog(Guid detailLogId, Guid logId, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSoulCoveterRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddGuildHuntingQuestMissionRewardLog(Guid logId, Guid guildId, Guid heroId, Guid questInstanceId, int nItemId, int nItemCount, bool bItemOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildHuntingQuestMissionRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildHuntingDonationLog(Guid logId, Guid guildId, Guid heroId, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, int nRewardItemId, int nRewardItemCount, bool bRewardItemOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildHuntingDonationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildHuntingDonationCompletionRewardLog(Guid logId, Guid guildId, Guid heroId, int nItemId, int nItemCount, bool bItemOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildHuntingDonationCompletionReawrdLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildDailyObjectiveLog(Guid logId, Guid guildId, DateTime date, int nObjectiveContentId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildDailyObjectiveLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@date", SqlDbType.DateTime).Value = date;
		sc.Parameters.Add("@nObjectiveContentId", SqlDbType.Int).Value = nObjectiveContentId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildDailyObjectiveRewardLog(Guid logId, Guid guildId, DateTime date, Guid heroId, int nRewardNo, int nCompletionMemberCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuidDailyObjectiveRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@date", SqlDbType.DateTime).Value = date;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardNo", SqlDbType.Int).Value = nRewardNo;
		sc.Parameters.Add("@nCompletionMemberCount", SqlDbType.Int).Value = nCompletionMemberCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildDailyObjectiveRewardDetailLog(Guid detailLogId, Guid logId, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildDailyObjectiveRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddGuildWeeklyObjectiveLog(Guid logId, Guid guildId, int nType, Guid heroId, int nObjectiveId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildWeeklyObjectiveLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nObjectiveId", SqlDbType.Int).Value = nObjectiveId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildWeeklyObjectiveRewardLog(Guid logId, Guid guildId, Guid heroId, int nObjectiveId, int nCompletionMemberCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildWeeklyObjectiveRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nObjectiveId", SqlDbType.Int).Value = nObjectiveId;
		sc.Parameters.Add("@nCompletionMemberCount", SqlDbType.Int).Value = nCompletionMemberCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddGuildWeeklyObjectiveRewardDetailLog(Guid detailLogId, Guid logId, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildWeeklyObjectiveRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddHeroIllustratedBookActivationLog(Guid logId, Guid heroId, int nIllustratedBookId, int nExplorationPoint, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroIllustratedBookActivationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nIllustratedBookId", SqlDbType.Int).Value = nIllustratedBookId;
		sc.Parameters.Add("@nExplorationPoint", SqlDbType.Int).Value = nExplorationPoint;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroIllustratedBookExplorationStepActivationLog(Guid logId, Guid heroId, int nStepNo, int nExplorationPoint, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroIllustratedBookExplorationStepActivationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nStepNo", SqlDbType.Int).Value = nStepNo;
		sc.Parameters.Add("@nExplorationPoint", SqlDbType.Int).Value = nExplorationPoint;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroIllustratedBookExplorationStepRewardLog(Guid logId, Guid heroId, int nStepNo, long lnRewardGold, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroIllustratedBookExplorationStepRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nStepNo", SqlDbType.Int).Value = nStepNo;
		sc.Parameters.Add("@lnRewardGold", SqlDbType.BigInt).Value = lnRewardGold;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroIllustratedBookExplorationStepRewardDetailLog(Guid detailLogId, Guid logId, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroIllustratedBookExplorationStepRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static SqlCommand CSC_AddHeroAccomplishmentRewardLog(Guid logId, Guid heroId, int nAccomplishmentId, int nRewardItemId, int nRewardItemCount, bool bRewardItemOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroAccomplishmentRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nAccomplishmentId", SqlDbType.Int).Value = nAccomplishmentId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroTitleAcquiredLog(Guid logId, Guid heroId, int nTitleId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroTitleAcquiredLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nTitleId", SqlDbType.Int).Value = nTitleId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroTitleActivationLog(Guid logId, Guid heroId, int nTitleId, int nType, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroTitleActivationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nTitleId", SqlDbType.Int).Value = nTitleId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCardCollectionActivationLog(Guid logId, Guid heroId, int nCollectionId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCardCollectionActivationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCollectionId", SqlDbType.Int).Value = nCollectionId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCardCollectionActivationDetailLog(Guid detailLogId, Guid logId, int nCreatureCardId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCardCollectionActivationDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nCreatureCardId", SqlDbType.Int).Value = nCreatureCardId;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCardDisassembleLog(Guid logId, Guid heroId, int nCreatureCardId, int nCount, int nAcquiredSoulPowder, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCardDisassembleLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCreatureCardId", SqlDbType.Int).Value = nCreatureCardId;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		sc.Parameters.Add("@nAcquiredSoulPowder", SqlDbType.Int).Value = nAcquiredSoulPowder;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCardComposititonLog(Guid logId, Guid heroId, int nCreatureCardId, int nCount, int nUsedSoulPowder, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreaturCardCompositionLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCreatureCardId", SqlDbType.Int).Value = nCreatureCardId;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		sc.Parameters.Add("@nUsedSoulPowder", SqlDbType.Int).Value = nUsedSoulPowder;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCardDisassembleAllLog(Guid logId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCardDisassembleAllLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCardDisassembleAllDetailLog(Guid detailLogId, Guid logId, int nCreatureCardId, int nCount, int nAcquiredSoulPowder)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCardDisassembleAllDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nCreatureCardId", SqlDbType.Int).Value = nCreatureCardId;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		sc.Parameters.Add("@nAcquiredSoulPowder", SqlDbType.Int).Value = nAcquiredSoulPowder;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCardShopLog(Guid shopId, Guid heroId, int nType, int nUsedOwnDia, int nUsedUnOwnDia, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCardShopLog";
		sc.Parameters.Add("@shopId", SqlDbType.UniqueIdentifier).Value = shopId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCardShopRandomProductLog(Guid shopId, int nProductId, int nCreatureCardId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCardShopRandomProoductLog";
		sc.Parameters.Add("@shopId", SqlDbType.UniqueIdentifier).Value = shopId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		sc.Parameters.Add("@nCreatureCardId", SqlDbType.Int).Value = nCreatureCardId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroCreatureCardShopRandomProductLog_Purchase(Guid shopId, int nProductId, int nUsedSoulPowder, DateTimeOffset purchasedTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroCreatureCardShopRandomProoductLog_Purchase";
		sc.Parameters.Add("@shopId", SqlDbType.UniqueIdentifier).Value = shopId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		sc.Parameters.Add("@nUsedSoulPowder", SqlDbType.Int).Value = nUsedSoulPowder;
		sc.Parameters.Add("@purchasedTime", SqlDbType.DateTimeOffset).Value = purchasedTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCardShopFixedProductBuyLog(Guid shopId, int nProductId, int nItemId, bool bItemOwned, int nUsedSoulPowder, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCardShopFixedProductBuyLog";
		sc.Parameters.Add("@shopId", SqlDbType.UniqueIdentifier).Value = shopId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@nUsedSoulPowder", SqlDbType.Int).Value = nUsedSoulPowder;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddContinentEliteMonsterSpawnLog(Guid logId, int nEliteMonsterId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddContinentElitemonsterSpawnLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nEliteMonsterId", SqlDbType.Int).Value = nEliteMonsterId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroContinentEliteMonsterKillLog(Guid killLogId, Guid logId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroContinentEliteMonsterKillLog";
		sc.Parameters.Add("@killLogId", SqlDbType.UniqueIdentifier).Value = killLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddStaminaBuyLog(Guid logId, Guid heroId, int nOldStamina, int nStamina, int nBuyCount, int nUsedOwnDia, int nUsedUnOwnDia, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddStaminaBuyLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nOldStamina", SqlDbType.Int).Value = nOldStamina;
		sc.Parameters.Add("@nStamina", SqlDbType.Int).Value = nStamina;
		sc.Parameters.Add("@nBuyCount", SqlDbType.Int).Value = nBuyCount;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroStaminaRecoveryLog(Guid logId, Guid heroId, int nRecoveryStamina, int nOldStamina, int nStamina, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroStaminaRecoveryLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRecoveryStamina", SqlDbType.Int).Value = nRecoveryStamina;
		sc.Parameters.Add("@nOldStamina", SqlDbType.Int).Value = nOldStamina;
		sc.Parameters.Add("@nStamina", SqlDbType.Int).Value = nStamina;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroProofOfValorRefreshLog(Guid logId, Guid heroId, int nType, Guid instanceId, int nUsedOwnDia, int nUsedUnOwnDia, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroProofOfValorRefreshLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroProofOfValorRewardLog(Guid logId, Guid heroId, Guid instanceId, int nStatus, int nCreatureCardId, int nRewardSoulPowder, long lnRewardExp, int nSpecialRewardSoulPowder, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroProofOfValorRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@nCreatureCardId", SqlDbType.Int).Value = nCreatureCardId;
		sc.Parameters.Add("@nRewardSoulPowder", SqlDbType.Int).Value = nRewardSoulPowder;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@nSpecialRewardSoulPowder", SqlDbType.Int).Value = nSpecialRewardSoulPowder;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroNpcShopProductBuyLog(Guid logId, Guid heroId, int nProductId, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, int nAcquiredItemId, bool bAcquiredItemOwned, int nAcquiredItemCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroNpcShopProductBuyLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@nAcquiredItemId", SqlDbType.Int).Value = nAcquiredItemId;
		sc.Parameters.Add("@bAcquiredItemOwned", SqlDbType.Bit).Value = bAcquiredItemOwned;
		sc.Parameters.Add("@nAcquiredItemCount", SqlDbType.Int).Value = nAcquiredItemCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroRankActiveSkillLevelUpLog(Guid logId, Guid heroId, int nSkillId, int nOldLevel, int nLevel, long lnUsedGold, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroRankActiveSkillLevelUpLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSkillId", SqlDbType.Int).Value = nSkillId;
		sc.Parameters.Add("@nOldLevel", SqlDbType.Int).Value = nOldLevel;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroRankPassiveSkillLevelUpLog(Guid logId, Guid heroId, int nSkillId, int nOldLevel, int nLevel, long lnUsedGold, int nUsedSpiritStone, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroRankPassiveSkillLevelUpLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSkillId", SqlDbType.Int).Value = nSkillId;
		sc.Parameters.Add("@nOldLevel", SqlDbType.Int).Value = nOldLevel;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@nUsedSpiritStone", SqlDbType.Int).Value = nUsedSpiritStone;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroRookieGiftRewardLog(Guid logId, Guid heroId, int nGiftNo, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroRookieGiftRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nGiftNo", SqlDbType.Int).Value = nGiftNo;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroRookieGiftRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroRookieGiftRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroOpenGiftRewardLog(Guid logId, Guid heroId, int nDay, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroOpenGiftRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDay", SqlDbType.Int).Value = nDay;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroOpenGiftRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bReawrdItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroOpenGiftRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bReawrdItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroDailyQuestCreationLog(Guid questInstanceId, Guid heroId, int nSlotIndex, int nMissionId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroDailyQuestCreationLog";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nMissionId", SqlDbType.Int).Value = nMissionId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroDailyQuestRefreshLog(Guid logId, Guid heroId, int nUsedGold, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroDailyQuestRefreshLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nUsedGold", SqlDbType.Int).Value = nUsedGold;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroDailyQuestRefreshDetailLog(Guid detailLogId, Guid logId, Guid questInstanceId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroDailyQuestRefreshDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		return sc;
	}

	public static SqlCommand CSC_AddHeroDailyQuestImmediateCompletionLog(Guid logId, Guid heroId, Guid questInstanceId, int nUsedGold, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroDailyQuestImmediateCompletionLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nUsedGold", SqlDbType.Int).Value = nUsedGold;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroDailyQuestRewardLog(Guid logId, Guid heroId, Guid questInstanceId, int nLevel, int nRewardVipPoint, long lnRewardExp, int nReawrdItemId, bool bRewardItemOwned, int nRewardItemCount, Guid newQuestInstanceId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroDailyQuestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nRewardVipPoint", SqlDbType.Int).Value = nRewardVipPoint;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nReawrdItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		sc.Parameters.Add("@newQuestInstanceId", SqlDbType.UniqueIdentifier).Value = newQuestInstanceId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroDailyQuestAbandonLog(Guid logId, Guid heroId, Guid questInstanceId, Guid newQuestInstanceId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroDailyQuestAbandonLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@newQuestInstanceId", SqlDbType.UniqueIdentifier).Value = newQuestInstanceId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWeeklyQuestRoundCreationLog(Guid roundId, Guid heroId, DateTime weekStartDate, int nRoundNo, int nMissionId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWeeklyQuestRoundCreationLog";
		sc.Parameters.Add("@roundId", SqlDbType.UniqueIdentifier).Value = roundId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weekStartDate", SqlDbType.Date).Value = weekStartDate;
		sc.Parameters.Add("@nRoundNo", SqlDbType.Int).Value = nRoundNo;
		sc.Parameters.Add("@nMissionId", SqlDbType.Int).Value = nMissionId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWeeklyQuestRoundAcceptanceLog(Guid logId, Guid heroId, Guid roundId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWeeklyQuestRoundAcceptanceLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@roundId", SqlDbType.UniqueIdentifier).Value = roundId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWeeklyQuestImmediateComletionLog1(Guid logId, Guid heroId, Guid roundId, int nCompletionRoundCount, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, long lnRewardGold, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWeeklyQuestImmediateCompletionLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@roundId", SqlDbType.UniqueIdentifier).Value = roundId;
		sc.Parameters.Add("@nCompletionRoundCount", SqlDbType.Int).Value = nCompletionRoundCount;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@lnRewardGold", SqlDbType.BigInt).Value = lnRewardGold;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWeeklyQuestRoundRewardLog(Guid logId, Guid heroId, Guid roundId, long lnRewardGold, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWeeklyQuestRoundRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@roundId", SqlDbType.UniqueIdentifier).Value = roundId;
		sc.Parameters.Add("@lnRewardGold", SqlDbType.BigInt).Value = lnRewardGold;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWeeklyQuestTenRoundRewardLog(Guid logId, Guid heroId, DateTime weekStartDate, int nCompletionRoundCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWeeklyQuestTenRoundRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weekStartDate", SqlDbType.Date).Value = weekStartDate;
		sc.Parameters.Add("@nCompletionRoundCount", SqlDbType.Int).Value = nCompletionRoundCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWeeklyQuestTenRoundRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWeeklyQuestTenRoundRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWisdomTempleRewardLog(Guid logId, Guid heroId, Guid instanceId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWisdomTempleRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWisdomTempleRewardDetailLog(Guid detailLogId, Guid logId, int nStepNo, long lnRewardExp, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWisdomTempleRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStepNo;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroOpen7DayEventRewardLog(Guid logId, Guid heroId, int nMissionId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroOpen7DayEventRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMissionId", SqlDbType.Int).Value = nMissionId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroOpen7DayEventRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroOpen7DayEventRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroOpen7DayEventProductBuyLog(Guid logId, Guid heroId, int nProductId, int nUsedOwnDia, int nUsedUnOwnDia, int nItemId, bool bItemOwned, int nItemCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroOpen7DayEventBuyLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroOpen7DayEventCostumeRewardLog(Guid logId, Guid heroId, int nUsedItemId, int nUsedItemOwnCount, int nUsedItmUnOwnCount, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroOpen7DayEventCostumeRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItmUnOwnCount;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroRetrievalLogs(Guid logId, Guid heroId, int nRetrievalId, int nCount, int nLevel, int nVipLevel, int nType, long lnUsedGold, int nUsedOwnDia, int nUsedUnOwnDia, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroRetrievalLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRetrievalId", SqlDbType.Int).Value = nRetrievalId;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nVipLevel", SqlDbType.Int).Value = nVipLevel;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroRetrievalDetailLog(Guid detailLogId, Guid logId, long lnRewardExp, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroRetrievalDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroTaskConsignmentLog(Guid consignmentInstanceId, Guid heroId, int nConsignmentId, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, int nUsedExpItemId, int nUsedExpItemOwnCount, int nUsedExpItemUnOwnCount, int nAchievementPoint, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroTaskConsignmentLog";
		sc.Parameters.Add("@consignmentInstanceId", SqlDbType.UniqueIdentifier).Value = consignmentInstanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nConsignmentId", SqlDbType.Int).Value = nConsignmentId;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@nUsedExpItemId", SqlDbType.Int).Value = nUsedExpItemId;
		sc.Parameters.Add("@nUsedExpItemOwnCount", SqlDbType.Int).Value = nUsedExpItemOwnCount;
		sc.Parameters.Add("@nUsedExpItemUnOwnCount", SqlDbType.Int).Value = nUsedExpItemUnOwnCount;
		sc.Parameters.Add("@nAchievementPoint", SqlDbType.Int).Value = nAchievementPoint;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroTaskConsignmentCompletionLog(Guid logId, Guid heroId, Guid consignmentInstanceId, int nRemainTime, long lnUsedGold, long lnRewardExp, int nRewardGuildContributionPoint, int nRewardGuildBuildingPoint, int nRewardGuildFund, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroTaskConsignmentCompletionLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@consignmentInstanceId", SqlDbType.UniqueIdentifier).Value = consignmentInstanceId;
		sc.Parameters.Add("@nRemainTime", SqlDbType.Int).Value = nRemainTime;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@nRewardGuildContributionPoint", SqlDbType.Int).Value = nRewardGuildContributionPoint;
		sc.Parameters.Add("@nRewardGuildBuildingPoint", SqlDbType.Int).Value = nRewardGuildBuildingPoint;
		sc.Parameters.Add("@nRewardGuildFund", SqlDbType.Int).Value = nRewardGuildFund;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroTaskConsignmentCompletionDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroTaskConsignmentCompletionDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddRuinsReclaimCreationLog(Guid instanceId, int nScheduleId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddRuinsReclaimCreationLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nScheduleId", SqlDbType.Int).Value = nScheduleId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddRuinsReclaimMemberLog(Guid instanceId, Guid heroId, int nType, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddRuinsReclaimMemberLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static SqlCommand CSC_AddRuinsReclaimDisqualificationLog(Guid instanceId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddRuinsReclaimDisqualificationLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddRuinsReclaimObjectRewardLog(Guid logId, Guid instanceId, Guid heroId, int nStep, long lnRewardGold, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddRuinsReclaimObjectRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@lnRewardGold", SqlDbType.BigInt).Value = lnRewardGold;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddRuinsReclaimStepCompletionLog(Guid instanceId, int nStep, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddRuinsReclaimStepCompletionLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddRuinsReclaimStepCompletionRewardLog(Guid rewardLogId, Guid instanceId, int nStep, Guid heroId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddRuinsReclaimStepCompletionRewardLog";
		sc.Parameters.Add("@rewardLogId", SqlDbType.UniqueIdentifier).Value = rewardLogId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddRuinsReclaimCompletionLog(Guid instanceId, int nStatus, int nPlayTime, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddRuinsReclaimCompletionLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@nPlayTime", SqlDbType.Int).Value = nPlayTime;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddRuinsReclaimCompletionMemberLog(Guid instanceId, Guid heroId, int nBossMonsterDamage, int nSummonMonsterDamage)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddRuinsReclaimCompletionMemberLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nBossMonsterDamage", SqlDbType.Int).Value = nBossMonsterDamage;
		sc.Parameters.Add("@nSummonMonsterDamage", SqlDbType.Int).Value = nSummonMonsterDamage;
		return sc;
	}

	public static SqlCommand CSC_AddRuinsReclaimCompletionMemberRewardLog(Guid rewardLogId, Guid instanceId, Guid heroId, int nType, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddRuinsReclaimCompletionMemberRewardLog";
		sc.Parameters.Add("@rewardLogId", SqlDbType.UniqueIdentifier).Value = rewardLogId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroTrueHeroQuestStartLog(Guid questInstanceId, Guid heroId, int nVipLevel, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroTrueHeroQuestStartLog";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nVipLevel", SqlDbType.Int).Value = nVipLevel;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroTrueHeroQuestStepRewardLog(Guid logId, Guid heroId, Guid questInstanceId, int nStep, int nTargetNationId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroTrueHeroQuestStepRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@nTargetNationId", SqlDbType.Int).Value = nTargetNationId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroTrueHeroQuestRewardLog(Guid logId, Guid heroId, Guid questInstanceId, int nRewardExploitPoint, int nAcquiredExploitPoint, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroTrueHeroQuestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nRewardExploitPoint", SqlDbType.Int).Value = nRewardExploitPoint;
		sc.Parameters.Add("@nAcquiredExploitPoint", SqlDbType.Int).Value = nAcquiredExploitPoint;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.Int).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddInfiniteWarCreationLog(Guid instanceId, int nScheduleId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddInfiniteWarCreationLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nScheduleId", SqlDbType.Int).Value = nScheduleId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddInfiniteWarMemberLog(Guid instanceId, Guid heroId, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddInfiniteWarMemberLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static SqlCommand CSC_AddInfiniteWarDisqualificationLog(Guid instanceId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddInfiniteWarDisqualificationLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddInfiniteWarCompletionLog(Guid instanceId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddInfiniteWarCompletionLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddInfiniteWarCompletionMemberLog(Guid instanceId, Guid heroId, int nPoint, DateTimeOffset lastKillTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddInfiniteWarCompletionMemberLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nPoint", SqlDbType.Int).Value = nPoint;
		sc.Parameters.Add("@lastKillTime", SqlDbType.DateTimeOffset).Value = lastKillTime;
		return sc;
	}

	public static SqlCommand CSC_AddInfiniteWarCompletionRewardDetailLog(Guid detailLogId, Guid instanceId, Guid heroId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddInfiniteWarCompletionRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddFieldBossCreationLog(Guid logId, long lnMonsterInstanceId, int nScheduleId, int nFieldBossId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldBossCreationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@lnMonsterInstanceId", SqlDbType.BigInt).Value = lnMonsterInstanceId;
		sc.Parameters.Add("@nScheduleId", SqlDbType.Int).Value = nScheduleId;
		sc.Parameters.Add("@nFieldBossId", SqlDbType.Int).Value = nFieldBossId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddFieldBosskillLog(Guid logId, long lnMonsterInstanceId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldBossKillLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@lnMonsterInstanceId", SqlDbType.BigInt).Value = lnMonsterInstanceId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddFieldBossRewardLog(Guid detailLogId, Guid logId, Guid heroId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldBossRewardLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroLimitationGiftRewardLog(Guid logId, Guid heroId, int nScheduleId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroLimitationGiftRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nScheduleId", SqlDbType.Int).Value = nScheduleId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroLimitationGiftRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewarditemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroLimitationGiftReawrdDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewarditemOwned", SqlDbType.Bit).Value = bRewarditemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWeekendRewardLog(Guid logId, Guid heroId, DateTime weekStartDate, int nSelection1, int nSelection2, int nSelection3, int nRewardOwnDia, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWeekendRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weekStartDate", SqlDbType.Date).Value = weekStartDate;
		sc.Parameters.Add("@nSelection1", SqlDbType.Int).Value = nSelection1;
		sc.Parameters.Add("@nSelection2", SqlDbType.Int).Value = nSelection2;
		sc.Parameters.Add("@nSelection3", SqlDbType.Int).Value = nSelection3;
		sc.Parameters.Add("@nRewardOwnDia", SqlDbType.Int).Value = nRewardOwnDia;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWeekendRewardItemRewardLog(Guid logId, Guid heroId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWeekendRewardItemRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddWarehouseSlotExtendLog(Guid logId, Guid heroId, int nOldPaidWarehouseSlotCount, int nPaidWarehouseSlotCount, int nUsedOwnDia, int nUsedUnOwnDia, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddWarehouseSlotExtendLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nOldPaidWarehouseSlotCount", SqlDbType.Int).Value = nOldPaidWarehouseSlotCount;
		sc.Parameters.Add("@nPaidWarehouseSlotCount", SqlDbType.Int).Value = nPaidWarehouseSlotCount;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroDiaShopProductBuyLog(Guid logId, Guid heroId, int nCategoryId, int nProductId, int nUsedOwnDia, int nUsedUnOwnDia, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, int nBuyItemId, bool bBuyItemOwned, int nBuyItemCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroDiaShopProductBuyLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCategoryId", SqlDbType.Int).Value = nCategoryId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@nBuyItemId", SqlDbType.Int).Value = nBuyItemId;
		sc.Parameters.Add("@bBuyItemOwned", SqlDbType.Bit).Value = bBuyItemOwned;
		sc.Parameters.Add("@nBuyItemCount", SqlDbType.Int).Value = nBuyItemCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWingMemoryPieceInstallationLog(Guid logId, Guid heroId, int nWingId, int nMemoryPieceStep, int nType, bool bSucceeded, bool bCriticalSucceeded, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWingMemoryPieceInstallationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nWingId", SqlDbType.Int).Value = nWingId;
		sc.Parameters.Add("@nMemoryPieceStep", SqlDbType.Int).Value = nMemoryPieceStep;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@bSucceeded", SqlDbType.Bit).Value = bSucceeded;
		sc.Parameters.Add("@bCriticalSucceeded", SqlDbType.Bit).Value = bCriticalSucceeded;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWingMemoryPieceInstallationDetailLog(Guid detailLogId, Guid logId, int nSlotIndex, int nOldAttrValue, int nAttrValue)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWingMemoryPieceInstallationDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nOldAttrValue", SqlDbType.Int).Value = nOldAttrValue;
		sc.Parameters.Add("@nAttrValue", SqlDbType.Int).Value = nAttrValue;
		return sc;
	}

	public static SqlCommand CSC_AddFearAltarCreationLog(Guid instanceId, int nStageId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFearAltarCreationLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStageId", SqlDbType.Int).Value = nStageId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddFearAltarMemberLog(Guid instanceId, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFearAltarMemberLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static SqlCommand CSC_AddFearAltarDisqualificationLog(Guid instanceId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFearAltarDisqualificationLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddFearAltarHalidomAcquisitionLog(Guid logId, Guid instanceId, Guid heroId, int nHalidomId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFearAltarHalidomAcquisitionLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nHalidomId", SqlDbType.Int).Value = nHalidomId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddFearAltarCompletionLog(Guid instanceId, int nStatus, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFearAltarCompletionLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddFearAltarCompletionMemberLog(Guid instanceId, Guid heroId, long lnRewardExp)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFearAltarCompletionMemberLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		return sc;
	}

	public static SqlCommand CSC_AddHeroFearAltarHalidomElementalRewardLog(Guid logId, Guid heroId, int nHalidomElementalId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroFearAltarHalidomElementalRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nHalidomElementalId", SqlDbType.Int).Value = nHalidomElementalId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroFearAltarHalidomCollectionRewardLog(Guid logId, Guid heroId, int nRewardNo, int nCollectionCount, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroFearAltarHalidomCollectionRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardNo", SqlDbType.Int).Value = nRewardNo;
		sc.Parameters.Add("@nCollectionCount", SqlDbType.Int).Value = nCollectionCount;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroSubQuestRewardLog(Guid logId, Guid heroId, int nSubQuestId, long lnRewardExp, long lnRewardGold, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroSubQuestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubQuestId", SqlDbType.Int).Value = nSubQuestId;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@lnRewardGold", SqlDbType.BigInt).Value = lnRewardGold;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroSubQuestRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroSubQuestRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroSubQuestAcceptanceLog(Guid logId, Guid heroId, int nSubQuestId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroSubQuestAcceptanceLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubQuestId", SqlDbType.Int).Value = nSubQuestId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroSubQuestAbandonmentLog(Guid logId, Guid heroId, int nSubQuestId, int nProgressCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroSubQuestAbandonmentLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubQuestId", SqlDbType.Int).Value = nSubQuestId;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddWarMemoryCreationLog(Guid instanceId, int nScheduleId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddWarMemoryCreationLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nScheduleId", SqlDbType.Int).Value = nScheduleId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddWarMemoryMemberLog(Guid instanceId, Guid heroId, int nType, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddWarMemoryMemberLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static SqlCommand CSC_AddWarMemoryDisqualificationLog(Guid instanceId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddWarMemoryDisqualificationLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddWarMemoryCompletionLog(Guid instanceId, int nStatus, int nPlayTime, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddWarMemoryCompletionLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@nPlayTime", SqlDbType.Int).Value = nPlayTime;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddWarMemoryCompletionMemberLog(Guid instanceId, Guid heroId, long lnRewardExp, int nPoint, DateTimeOffset pointUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddWarMemoryCompletionMemberLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@nPoint", SqlDbType.Int).Value = nPoint;
		sc.Parameters.Add("@pointUpdateTime", SqlDbType.DateTimeOffset).Value = pointUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_AddWarMemoryCompletionRewardDetailLog(Guid detailLogId, Guid instanceId, Guid heroId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddWarMemoryCompletionRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroOrdealQuestRewardLog(Guid logId, Guid heroId, int nQuestNo, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroOrdealQuestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nQuestNo", SqlDbType.Int).Value = nQuestNo;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroOrdealQuestMissionRewardLog(Guid logId, Guid heroId, int nQuestNo, int nSlotIndex, int nMissionNo, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroOrdealQuestMissionRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nQuestNo", SqlDbType.Int).Value = nQuestNo;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nMissionNo", SqlDbType.Int).Value = nMissionNo;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroOsirisRoomRewardLog(Guid logId, Guid heroId, int nDifficulty, int nPlayTime, long lnRewardGold, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroOsirisRoomRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDifficulty", SqlDbType.Int).Value = nDifficulty;
		sc.Parameters.Add("@nPlayTime", SqlDbType.Int).Value = nPlayTime;
		sc.Parameters.Add("@lnRewardGold", SqlDbType.BigInt).Value = lnRewardGold;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMoneyBuffUsedLog(Guid logId, Guid heroId, int nBuffId, long lnUsedGold, int nUsedOwnDia, int nUsedUnOwnDia, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMoneyBuffUsedLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nBuffId", SqlDbType.Int).Value = nBuffId;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUSedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroBiographyStartLog(Guid logId, Guid heroId, int nBiographyId, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroBiographyStartLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nBiographyId", SqlDbType.Int).Value = nBiographyId;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroBiographyRewardLog(Guid logId, Guid heroId, int nBiographyId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroBiographyReawrdLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nBiographyId", SqlDbType.Int).Value = nBiographyId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroBiographyRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroBiographyRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroBiographyQuestRewardLog(Guid logId, Guid heroId, int nBiographyId, int nQuestNo, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroBiographyQuestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nBiographyId", SqlDbType.Int).Value = nBiographyId;
		sc.Parameters.Add("@nQuestNo", SqlDbType.Int).Value = nQuestNo;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroItemLuckyShopPickLog(Guid logId, Guid heroId, int nType, int nUsedOwnDia, int nUsedUnOwnDia, long lnRewardGold, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroItemLuckyShopPickLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@lnRewardGold", SqlDbType.BigInt).Value = lnRewardGold;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroItemLuckyShopPickDetailLog(Guid detailLogId, Guid logId, int nItemId, bool bItemOwned, int nItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroItemLuckyShopPickDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCardLuckyShopPickLog(Guid logId, Guid heroId, int nType, int nUsedOwnDia, int nUsedUnOwnDia, long lnRewardGold, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCardLuckyShopPickLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@lnRewardGold", SqlDbType.Int).Value = lnRewardGold;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCardLuckyShopPickDetailLog(Guid detailLogid, Guid logId, int nCreatureCardId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreauteCardLuckyShopPickDetailLog";
		sc.Parameters.Add("@detailLogid", SqlDbType.UniqueIdentifier).Value = detailLogid;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nCreatureCardId", SqlDbType.Int).Value = nCreatureCardId;
		return sc;
	}

	public static SqlCommand CSC_AddHeroBlessingSendingLog(Guid logId, Guid heroId, Guid blessingReceiveHeroId, int nTargetLevelId, int nBlessingId, long lnUsedGold, int nUsedOwnDia, int nUsedUnOwnDia, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroBlessingSendingLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@blessingReceiveHeroId", SqlDbType.UniqueIdentifier).Value = blessingReceiveHeroId;
		sc.Parameters.Add("@nTargetLevelId", SqlDbType.Int).Value = nTargetLevelId;
		sc.Parameters.Add("@nBlessingId", SqlDbType.Int).Value = nBlessingId;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroBlessingReceivingLog(Guid logId, Guid sendingLogId, long lnRewardGold, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroBlessingReceivingLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@sendingLogId", SqlDbType.UniqueIdentifier).Value = sendingLogId;
		sc.Parameters.Add("@lnRewardGold", SqlDbType.BigInt).Value = lnRewardGold;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroProspectQuestRewardLog(Guid logId, Guid questInstanceId, Guid heroId, bool bIsOwner, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroProspectQuestRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@bIsOwner", SqlDbType.Bit).Value = bIsOwner;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroProspectQuestRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroProspectQuestRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddDragonNestCreationLog(Guid instanceId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddDragonNestCreationLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddDragonNestMemberLog(Guid instanceId, Guid heroId, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddDragonNestMemberLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static SqlCommand CSC_AddDragonNestMemberDisqualificationLog(Guid instanceId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddDragonNestMemberDisqualificationLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddDargonNestMemberDisqualificationLog(Guid instanceId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddDragonNestMemberDisqualificationLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddDragonNestCompletionLog(Guid instanceId, int nStatus, bool bAdditionalStepOpened, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddDragonNestCompletionLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@bAdditionalStepOpened", SqlDbType.Bit).Value = bAdditionalStepOpened;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddDragonNestCompletionMemberLog(Guid instanceId, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddDragonNestCompletionMemberLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static SqlCommand CSC_AddDragonNestCompletionMemberRewardLog(Guid logId, Guid instanceId, Guid heroId, int nStepNo, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddDragonNestCompletionMemberRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nStepNo", SqlDbType.Int).Value = nStepNo;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCreationLog(Guid heroCreatureId, Guid heroId, int nCreatureId, int nQuality, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCreationLog";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCreatureId", SqlDbType.Int).Value = nCreatureId;
		sc.Parameters.Add("@nQuality", SqlDbType.Int).Value = nQuality;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCreationSkillLog(Guid heroCreatureId, int nSlotIndex, int nSkillId, int nSkillGrade)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCreationSkillLog";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nSkillId", SqlDbType.Int).Value = nSkillId;
		sc.Parameters.Add("@nSkillGrade", SqlDbType.Int).Value = nSkillGrade;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCreationBaseAttrLog(Guid heroCreatureId, int nAttrId, int nValue)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCreationBaseAttrLog";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		sc.Parameters.Add("@nValue", SqlDbType.Int).Value = nValue;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCreationAdditionalAttrLog(Guid heroCreatureId, int nAttrId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCreationAdditionalAttrLog";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureStatusUpdateLog(Guid logId, Guid heroCreatureId, Guid heroId, bool bOldParticipated, bool bParticipated, bool bOldCheered, bool bCheered, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureStatusUpdateLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@bOldParticipated", SqlDbType.Bit).Value = bOldParticipated;
		sc.Parameters.Add("@bParticipated", SqlDbType.Bit).Value = bParticipated;
		sc.Parameters.Add("@bOldCheered", SqlDbType.Bit).Value = bOldCheered;
		sc.Parameters.Add("@bCheered", SqlDbType.Bit).Value = bCheered;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureRearingLog(Guid logId, Guid heroCreatureId, Guid heroId, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, int nOldLevel, int nLevel, int nOldExp, int nExp, int nAcquisitionExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureRearingLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@nOldLevel", SqlDbType.Int).Value = nOldLevel;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nOldExp", SqlDbType.Int).Value = nOldExp;
		sc.Parameters.Add("@nExp", SqlDbType.Int).Value = nExp;
		sc.Parameters.Add("@nAcquisitionExp", SqlDbType.Int).Value = nAcquisitionExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureReleaseLog(Guid heroCreatureId, Guid heroId, int nLevel, int nExp, int nInjectionLevel, int nInjectionExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureReleaseLog";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nExp", SqlDbType.Int).Value = nExp;
		sc.Parameters.Add("@nInjectionLevel", SqlDbType.Int).Value = nInjectionLevel;
		sc.Parameters.Add("@nInjectionExp", SqlDbType.Int).Value = nInjectionExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureReleaseDetailLog(Guid detailLogId, Guid heroCreatureId, int nRetrievalItemId, bool bRetrievalItemOwned, int nRetrievalItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureReleaseDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@nRetrievalItemId", SqlDbType.Int).Value = nRetrievalItemId;
		sc.Parameters.Add("@bRetrievalItemOwned", SqlDbType.Bit).Value = bRetrievalItemOwned;
		sc.Parameters.Add("@nRetrievalItemCount", SqlDbType.Int).Value = nRetrievalItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureVariationLog(Guid logId, Guid heroCreatureId, Guid heroId, int nOldQuality, int nQuality, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureVariationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nOldQuality", SqlDbType.Int).Value = nOldQuality;
		sc.Parameters.Add("@nQuality", SqlDbType.Int).Value = nQuality;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureVariationDetailLog(Guid detailLogId, Guid logId, int nAttrId, int nOldValue, int nValue)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureVariationDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		sc.Parameters.Add("@nOldValue", SqlDbType.Int).Value = nOldValue;
		sc.Parameters.Add("@nValue", SqlDbType.Int).Value = nValue;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureAdditionalAttrSwitchLog(Guid logId, Guid heroCreatureId, Guid heroId, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureAdditionalAttrSwitchLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureAdditionalAttrSwitchDetailLog(Guid detailLogId, Guid logId, int nOldAttrId, int nAttrId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureAdditionalAttrSwitchDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nOldAttrId", SqlDbType.Int).Value = nOldAttrId;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureInjectionLog(Guid logId, Guid heroCreatureId, Guid heroId, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, long lnUsedGold, int nOldInjectionLevel, int nInjectionLevel, int nOldInjectionExp, int nInjectionExp, int nAcquisitionInjectionExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureInjectionLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@nOldInjectionLevel", SqlDbType.Int).Value = nOldInjectionLevel;
		sc.Parameters.Add("@nInjectionLevel", SqlDbType.Int).Value = nInjectionLevel;
		sc.Parameters.Add("@nOldInjectionExp", SqlDbType.Int).Value = nOldInjectionExp;
		sc.Parameters.Add("@nInjectionExp", SqlDbType.Int).Value = nInjectionExp;
		sc.Parameters.Add("@nAcquisitionInjectionExp", SqlDbType.Int).Value = nAcquisitionInjectionExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureInjectionRetrievalLog(Guid logId, Guid heroCreatureId, Guid heroId, int nInjectionLevel, int nInjectionExp, int nRetrievalItemId, bool bRetrievalItemOwned, int nRetrievalItemCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureInjectionRetrievalLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nInjectionLevel", SqlDbType.Int).Value = nInjectionLevel;
		sc.Parameters.Add("@nInjectionExp", SqlDbType.Int).Value = nInjectionExp;
		sc.Parameters.Add("@nRetrievalItemId", SqlDbType.Int).Value = nRetrievalItemId;
		sc.Parameters.Add("@bRetrievalItemOwned", SqlDbType.Bit).Value = bRetrievalItemOwned;
		sc.Parameters.Add("@nRetrievalItemCount", SqlDbType.Int).Value = nRetrievalItemCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureSkilSlotOpenLog(Guid logId, Guid heroCreatureId, Guid heroId, int nOldSlotCount, int nSlotCount, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureSkillSlotOpenLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nOldSlotCount", SqlDbType.Int).Value = nOldSlotCount;
		sc.Parameters.Add("@nSlotCount", SqlDbType.Int).Value = nSlotCount;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCompositionLog(Guid logId, Guid heroCreatureId, Guid heroId, Guid materialHeroCreatureId, int nMaterialHeroCreatureLevel, int nMaterialHeroCreatureExp, int nMaterialHeroCreatureInjectionLevel, int nMaterialHeroCreatureInjectionExp, int nSkillProtectionItemId, int nSkillProtectionItemOwnCount, int nSkillProtectionItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCompositionLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@materialHeroCreatureId", SqlDbType.UniqueIdentifier).Value = materialHeroCreatureId;
		sc.Parameters.Add("@nMaterialHeroCreatureLevel", SqlDbType.Int).Value = nMaterialHeroCreatureLevel;
		sc.Parameters.Add("@nMaterialHeroCreatureExp", SqlDbType.Int).Value = nMaterialHeroCreatureExp;
		sc.Parameters.Add("@nMaterialHeroCreatureInjectionLevel", SqlDbType.Int).Value = nMaterialHeroCreatureInjectionLevel;
		sc.Parameters.Add("@nMaterialHeroCreatureInjectionExp", SqlDbType.Int).Value = nMaterialHeroCreatureInjectionExp;
		sc.Parameters.Add("@nSkillProtectionItemId", SqlDbType.Int).Value = nSkillProtectionItemId;
		sc.Parameters.Add("@nSkillProtectionItemOwnCount", SqlDbType.Int).Value = nSkillProtectionItemOwnCount;
		sc.Parameters.Add("@nSkillProtectionItemUnOwnCount", SqlDbType.Int).Value = nSkillProtectionItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCompositionSkillLog(Guid detailLogId, Guid logId, int nSlotIndex, int nOldSkillId, int nOldSkillGrade, int nSkillId, int nSkillGrade, bool bProtected)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCompositionSkillLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nOldSkillId", SqlDbType.Int).Value = nOldSkillId;
		sc.Parameters.Add("@nOldSkillGrade", SqlDbType.Int).Value = nOldSkillGrade;
		sc.Parameters.Add("@nSkillId", SqlDbType.Int).Value = nSkillId;
		sc.Parameters.Add("@nSkillGrade", SqlDbType.Int).Value = nSkillGrade;
		sc.Parameters.Add("@bProtected", SqlDbType.Bit).Value = bProtected;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCompositionDetailLog(Guid detailLogId, Guid logId, int nRetrievalItemId, bool bRetrievalItemOwned, int nRetrievalItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCompositionDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRetrievalItemId", SqlDbType.Int).Value = nRetrievalItemId;
		sc.Parameters.Add("@bRetrievalItemOwned", SqlDbType.Bit).Value = bRetrievalItemOwned;
		sc.Parameters.Add("@nRetrievalItemCount", SqlDbType.Int).Value = nRetrievalItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroPresentLog(Guid logId, Guid heroId, Guid targetHeroId, int nPresentId, int nAddedPresentContributionPoint, int nAddedPresentPopularityPoint, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroPresentLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@targetHeroId", SqlDbType.UniqueIdentifier).Value = targetHeroId;
		sc.Parameters.Add("@nPresentId", SqlDbType.Int).Value = nPresentId;
		sc.Parameters.Add("@nAddedPresentContributionPoint", SqlDbType.Int).Value = nAddedPresentContributionPoint;
		sc.Parameters.Add("@nAddedPresentPopularityPoint", SqlDbType.Int).Value = nAddedPresentPopularityPoint;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWeeklyPresentPopularityPointRankingRewardLog(Guid logId, Guid heroId, int nRankingNo, int nRanking, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWeeklyPresentPopularityPointRankingRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		sc.Parameters.Add("@nRanking", SqlDbType.Int).Value = nRanking;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWeeklyPresentPopularityPointRankingRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWeeklyPresentPopularityPointRankingRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWeeklyPresentContributionPointRankingRewardLog(Guid logId, Guid heroId, int nRankingNo, int nRanking, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWeeklyPresentContributionPointRankingRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		sc.Parameters.Add("@nRanking", SqlDbType.Int).Value = nRanking;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroWeeklyPresentContributionPointRankingRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWeeklyPresentContributionPointRankingRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCostumeAcquisitionLog(Guid logId, Guid heroId, int nCostumeId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCostumeAcquisitionLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCostumeId", SqlDbType.Int).Value = nCostumeId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCostumeEffectApplicationLog(Guid logId, Guid heroId, int nCostumeId, int nOldCostumeEffectId, int nCostumeEffectId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCostumeEffectApplicationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCostumeId", SqlDbType.Int).Value = nCostumeId;
		sc.Parameters.Add("@nOldCostumeEffectId", SqlDbType.Int).Value = nOldCostumeEffectId;
		sc.Parameters.Add("@nCostumeEffectId", SqlDbType.Int).Value = nCostumeEffectId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCostumeEnchantLog(Guid logId, Guid heroId, int nCostumeId, int nOldEnchantLevel, int nOldLuckyValue, int nEnchantLevel, int nLuckyValue, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCostumeEnchantLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCostumeId", SqlDbType.Int).Value = nCostumeId;
		sc.Parameters.Add("@nOldEnchantLevel", SqlDbType.Int).Value = nOldEnchantLevel;
		sc.Parameters.Add("@nOldLuckyValue", SqlDbType.Int).Value = nOldLuckyValue;
		sc.Parameters.Add("@nEnchantLevel", SqlDbType.Int).Value = nEnchantLevel;
		sc.Parameters.Add("@nLuckyValue", SqlDbType.Int).Value = nLuckyValue;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCostumeCollectionActivationLog(Guid logId, Guid heroId, int nCostumeCollectionId, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCostumeCollectionActivationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCostumeCollectionId", SqlDbType.Int).Value = nCostumeCollectionId;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCostumeCollectionSuffleLog(Guid logId, Guid heroId, int nOldCostumeCollectionId, int nCostumeCollectionId, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCostumeCollectionShuffleLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nOldCostumeCollectionId", SqlDbType.Int).Value = nOldCostumeCollectionId;
		sc.Parameters.Add("@nCostumeCollectionId", SqlDbType.Int).Value = nCostumeCollectionId;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureFarmQuestMissionRewardLog(Guid logId, Guid questInstanceId, Guid heroId, int nMissionNo, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureFarmQuestMissionRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMissionNo", SqlDbType.Int).Value = nMissionNo;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureFarmQuestRewardLog(Guid questInstanceId, Guid heroId, long lnRewardExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureFarmQuestRewardLog";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureFarmQuestRewardDetailLog(Guid detailLogId, Guid questInstanceId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureFarmQuestRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddNationAllianceLog(Guid applicationId, int nNationId, long lnUsedFund, int nTargetNationId, long lnTargetUsedFund, Guid allianceId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationAllianceLog";
		sc.Parameters.Add("@applicationId", SqlDbType.UniqueIdentifier).Value = applicationId;
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@lnUsedFund", SqlDbType.Int).Value = lnUsedFund;
		sc.Parameters.Add("@nTargetNationId", SqlDbType.Int).Value = nTargetNationId;
		sc.Parameters.Add("@lnTargetUsedFund", SqlDbType.BigInt).Value = lnTargetUsedFund;
		sc.Parameters.Add("@allianceId", SqlDbType.UniqueIdentifier).Value = allianceId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddNationAllianceBrokenLog(Guid allianceId, int nNationId, int nTargetNationId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationAllianceBrokenLog";
		sc.Parameters.Add("@allianceId", SqlDbType.UniqueIdentifier).Value = allianceId;
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@nTargetNationId", SqlDbType.Int).Value = nTargetNationId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddPurchaseProvideCompletionLog(Guid logId, Guid purchaseId, int nProductId, Guid heroId, int nUnOwnDia, int nItemId, bool bItemOwned, int nItemCount, int nFirstPurchaseBonusUnOwnDia, int nVipPoint, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddPurchaseProvideCompletionLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@purchaseId", SqlDbType.UniqueIdentifier).Value = purchaseId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nUnOwnDia", SqlDbType.Int).Value = nUnOwnDia;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@nFirstPurchaseBonusUnOwnDia", SqlDbType.Int).Value = nFirstPurchaseBonusUnOwnDia;
		sc.Parameters.Add("@nVipPoint", SqlDbType.Int).Value = nVipPoint;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroFirstChargeEventRewardLog(Guid logId, Guid accountId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroFirstChargeEventRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroFirstChargeEventRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroFirstChargeEventRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroRechargeEventRewardLog(Guid logId, Guid accountId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroRechargeEventRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroRechargeEventRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroRechargeEventRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroChargeEventMissionRewardLog(Guid logId, Guid accountId, Guid heroId, int nEventId, int nMissionNo, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroChargeEventMissionRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nEventId", SqlDbType.Int).Value = nEventId;
		sc.Parameters.Add("@nMissionNo", SqlDbType.Int).Value = nMissionNo;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroChargeEventMissionRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroChargeEventMissionRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroDailyChargeEventMissionRewardLog(Guid logId, Guid accountId, Guid heroId, DateTime date, int nMissionNo, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroDailyChargeEventMissionRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nMissionNo", SqlDbType.Int).Value = nMissionNo;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroDailyChargeEventMissionRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroDailyChargeEventMissionRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroConsumeEventMissionRewardLog(Guid logId, Guid accountId, Guid heroId, int nEventId, int nMissionNo, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroConsumeEventMissionRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nEventId", SqlDbType.Int).Value = nEventId;
		sc.Parameters.Add("@nMissionNo", SqlDbType.Int).Value = nMissionNo;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroConsumeEventMissionRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroConsumeEventMissionRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroDailyConsumeEventMissionRewardLog(Guid logId, Guid accountId, Guid heroId, DateTime date, int nMissionNo, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroDailyConsumeEventMissionRewardLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nMissionNo", SqlDbType.Int).Value = nMissionNo;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroDailyConsumeEventMissionRewardDetailLog(Guid detailLogId, Guid logId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroDailyConsumeEventMissionRewardDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddHeroJobChangeLog(Guid logId, Guid heroId, int nOldJobId, int nJobId, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroJobChangeLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nOldJobId", SqlDbType.Int).Value = nOldJobId;
		sc.Parameters.Add("@nJobId", SqlDbType.Int).Value = nJobId;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroJobChangeQuestRewardLog(Guid questInstanceId, Guid heroId, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroJobChangeQuestRewardLog";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroPotionAttrLog(Guid logId, Guid heroId, int nPotionAttrId, int nCount, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroPotionAttrLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nPotionAttrId", SqlDbType.Int).Value = nPotionAttrId;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		sc.Parameters.Add("@nUsedItemId", SqlDbType.Int).Value = nUsedItemId;
		sc.Parameters.Add("@nUsedItemOwnCount", SqlDbType.Int).Value = nUsedItemOwnCount;
		sc.Parameters.Add("@nUsedItemUnOwnCount", SqlDbType.Int).Value = nUsedItemUnOwnCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddAnkouTombCreationLog(Guid instanceId, int nDifficulty, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAnkouTombCreationLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nDifficulty", SqlDbType.Int).Value = nDifficulty;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddAnkouTombMemberLog(Guid instanceId, Guid heroId, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAnkouTombMemberLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static SqlCommand CSC_AddAnkouTombMemberDisqualificationLog(Guid instanceId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAnkouTombMemberDisqualificationLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddAnkouTombCompletionLog(Guid instanceId, int nStatus, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAnkouTombCompletionLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddAnkouTombCompletionMemberRewardLog(Guid instanceId, Guid heroId, long lnRewardGold, long lnRewardExp, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAnkouTombCompletionMemberRewardLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@lnRewardGold", SqlDbType.BigInt).Value = lnRewardGold;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddAnkouTombCompletionMemberAdditionalRewardLog(Guid instanceId, Guid heroId, int nRewardExpType, long lnRewardExp, int nUsedUnOwnDia)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAnkouTombCompletionMemberAdditionalRewardLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardExpType", SqlDbType.Int).Value = nRewardExpType;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		return sc;
	}

	public static SqlCommand CSC_AddHeroConstellationActivationLog(Guid logId, Guid heroId, int nConstellationId, int nStep, int nCycle, int nEntryNo, int nUsedStarEssense, long lnUsedGold, bool bActivated, int nFailPoint, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroConstellationActivationLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nConstellationId", SqlDbType.Int).Value = nConstellationId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@nCycle", SqlDbType.Int).Value = nCycle;
		sc.Parameters.Add("@nEntryNo", SqlDbType.Int).Value = nEntryNo;
		sc.Parameters.Add("@nUsedStarEssense", SqlDbType.Int).Value = nUsedStarEssense;
		sc.Parameters.Add("@lnUsedGold", SqlDbType.BigInt).Value = lnUsedGold;
		sc.Parameters.Add("@bActivated", SqlDbType.Bit).Value = bActivated;
		sc.Parameters.Add("@nFailPoint", SqlDbType.Int).Value = nFailPoint;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroConstellationStepOpenLog(Guid logId, Guid heroId, int nConsellationId, int nStep, int nUsedOwnDia, int nUsedUnOwnDia, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroConstellationStepOpenLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nConsellationId", SqlDbType.Int).Value = nConsellationId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@nUsedOwnDia", SqlDbType.Int).Value = nUsedOwnDia;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroArtifactLevelUpLog(Guid logId, Guid heroId, int nOldArtifactNo, int nOldArtifactLevel, int nOldArtifactExp, int nArtifactNo, int nArtifactLevel, int nArtifactExp, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroArtifactLevelUpLog";
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nOldArtifactNo", SqlDbType.Int).Value = nOldArtifactNo;
		sc.Parameters.Add("@nOldArtifactLevel", SqlDbType.Int).Value = nOldArtifactLevel;
		sc.Parameters.Add("@nOldArtifactExp", SqlDbType.Int).Value = nOldArtifactExp;
		sc.Parameters.Add("@nArtifactNo", SqlDbType.Int).Value = nArtifactNo;
		sc.Parameters.Add("@nArtifactLevel", SqlDbType.Int).Value = nArtifactLevel;
		sc.Parameters.Add("@nArtifactExp", SqlDbType.Int).Value = nArtifactExp;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroArtifactLevelUpDetailLog(Guid detailLogId, Guid logId, Guid heroMainGearId, int nExp)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroArtifactLevelUpDetailLog";
		sc.Parameters.Add("@detailLogId", SqlDbType.UniqueIdentifier).Value = detailLogId;
		sc.Parameters.Add("@logId", SqlDbType.UniqueIdentifier).Value = logId;
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@nExp", SqlDbType.Int).Value = nExp;
		return sc;
	}

	public static SqlCommand CSC_AddTradeShipCreationLog(Guid instanceId, int nDifficulty, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddTradeShipCreationLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nDifficulty", SqlDbType.Int).Value = nDifficulty;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddTradeShipMemberLog(Guid instanceId, Guid heroId, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddTradeShipMemberLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static SqlCommand CSC_AddTradeShipMemberDisqualificationLog(Guid instanceId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddTradeShipMemberDisqualificationLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddTradeShipCompletionLog(Guid instanceId, int nStatus, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddTradeShipCompletionLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddTradeShipCompletionMemberRewardLog(Guid instanceId, Guid heroId, long lnRewardGold, long lnRewardExp, int nRewardItemId, bool bRewardItemOwned, int nRewardItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddTradeShipCompletionMemberRewardLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@lnRewardGold", SqlDbType.BigInt).Value = lnRewardGold;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@nRewardItemId", SqlDbType.Int).Value = nRewardItemId;
		sc.Parameters.Add("@bRewardItemOwned", SqlDbType.Bit).Value = bRewardItemOwned;
		sc.Parameters.Add("@nRewardItemCount", SqlDbType.Int).Value = nRewardItemCount;
		return sc;
	}

	public static SqlCommand CSC_AddTradeShipCompletionMemberAdditionalRewardLog(Guid instanceId, Guid heroId, int nRewardExpType, long lnRewardExp, int nUsedUnOwnDia)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddTradeShipCompletionMemberAdditionalRewardLog";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardExpType", SqlDbType.Int).Value = nRewardExpType;
		sc.Parameters.Add("@lnRewardExp", SqlDbType.BigInt).Value = lnRewardExp;
		sc.Parameters.Add("@nUsedUnOwnDia", SqlDbType.Int).Value = nUsedUnOwnDia;
		return sc;
	}
}
