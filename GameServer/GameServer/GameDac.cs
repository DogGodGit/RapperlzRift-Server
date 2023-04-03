using System;
using System.Data;
using System.Data.SqlClient;
using ServerFramework;

namespace GameServer;

public static class GameDac
{
	public static DateTimeOffset CurrentTime(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CurrentTime";
		return (DateTimeOffset)sc.ExecuteScalar();
	}

	public static DataRow AccountByUserIdAndVirtualGameServerId_x(SqlConnection conn, SqlTransaction trans, Guid userId, int nVirtualGameServerId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AccountByUserIdAndVirtualGameServerId_x";
		sc.Parameters.Add("@userId", SqlDbType.UniqueIdentifier).Value = userId;
		sc.Parameters.Add("@nVirtualGameServerId", SqlDbType.Int).Value = nVirtualGameServerId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static int AddAccount(SqlConnection conn, SqlTransaction trans, Guid accountId, Guid userId, int nVirtualGameServerId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAccount";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@userId", SqlDbType.UniqueIdentifier).Value = userId;
		sc.Parameters.Add("@nVirtualGameServerId", SqlDbType.Int).Value = nVirtualGameServerId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static DataRow Account(SqlConnection conn, SqlTransaction trans, Guid accountId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Account";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_UpdateAccount_Login(Guid accountId, DateTimeOffset lastLoginTime, string sLastLoginIp)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateAccount_Login";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@lastLoginTime", SqlDbType.DateTimeOffset).Value = lastLoginTime;
		sc.Parameters.Add("@sLastLoginIp", SqlDbType.VarChar).Value = sLastLoginIp;
		return sc;
	}

	public static SqlCommand CSC_UpdateAccount_LastHero(Guid accountId, Guid lastHeroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateAccount_LastHero";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@lastHeroId", SqlDbType.UniqueIdentifier).Value = lastHeroId;
		return sc;
	}

	public static DataRowCollection LobbyHeroes(SqlConnection conn, SqlTransaction trans, Guid accountId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LobbyHeroes";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int HeroCount_x(SqlConnection conn, SqlTransaction trans, Guid accountId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroCount_x";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static int AddHero(SqlConnection conn, SqlTransaction trans, Guid heroId, Guid accountId, int nBaseJobId, int nJobId, int nNationId, int nLevel, long lnExp, int nWingStep, int nWingLevel, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHero";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@nBaseJobId", SqlDbType.Int).Value = nBaseJobId;
		sc.Parameters.Add("@nJobId", SqlDbType.Int).Value = nJobId;
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@lnExp", SqlDbType.BigInt).Value = lnExp;
		sc.Parameters.Add("@nWingStep", SqlDbType.Int).Value = nWingStep;
		sc.Parameters.Add("@nWingLevel", SqlDbType.Int).Value = nWingLevel;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static DataRow Hero_x(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Hero_x";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static int UpdateHero_CompleteNamingTutorial(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_CompleteNamingTutorial";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static int UpdateHero_Name(SqlConnection conn, SqlTransaction trans, Guid heroId, string sName)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_Name";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@sName", SqlDbType.NVarChar).Value = sName;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static int UpdateHero_CompleteCreation(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_CompleteCreation";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static SqlCommand CSC_UpdateHero_LogOut(Guid heroId, int nLevel, long lnExp, int nLak, int nHp, int nStamina, DateTimeOffset? staminaUpdateTime, DateTimeOffset? lastLoginTime, DateTimeOffset? lastLogoutTime, int nLastLocationId, int nLastLocationParam, Guid lastInstanceId, float fLastXPosition, float fLastYPosition, float fLastZPosition, float fLastYRotation, int nPreviousNationId, int nPreviousContinentId, float fPreviousXPosition, float fPreviousYPosition, float fPreviousZPosition, float fPreviousYRotation, int nRestTime, DateTimeOffset dailyAccessTimeUpdateTime, float fDailyAccessTime, bool bIsRiding, DateTime undergroundMazeDate, int nUndergroundMazePlayTime, DateTimeOffset? artifactRoomSweepStartTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_LogOut";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@lnExp", SqlDbType.BigInt).Value = lnExp;
		sc.Parameters.Add("@nLak", SqlDbType.Int).Value = nLak;
		sc.Parameters.Add("@nHp", SqlDbType.Int).Value = nHp;
		sc.Parameters.Add("@nStamina", SqlDbType.Int).Value = nStamina;
		sc.Parameters.Add("@staminaUpdateTime", SqlDbType.DateTimeOffset).Value = SFDBUtil.NullToDBNull(staminaUpdateTime);
		sc.Parameters.Add("@lastLoginTime", SqlDbType.DateTimeOffset).Value = SFDBUtil.NullToDBNull(lastLoginTime);
		sc.Parameters.Add("@lastLogoutTime", SqlDbType.DateTimeOffset).Value = SFDBUtil.NullToDBNull(lastLogoutTime);
		sc.Parameters.Add("@nLastLocationId", SqlDbType.Int).Value = nLastLocationId;
		sc.Parameters.Add("@nLastLocationParam", SqlDbType.Int).Value = nLastLocationParam;
		sc.Parameters.Add("@lastInstanceId", SqlDbType.UniqueIdentifier).Value = lastInstanceId;
		sc.Parameters.Add("@fLastXPosition", SqlDbType.Float).Value = fLastXPosition;
		sc.Parameters.Add("@fLastYPosition", SqlDbType.Float).Value = fLastYPosition;
		sc.Parameters.Add("@fLastZPosition", SqlDbType.Float).Value = fLastZPosition;
		sc.Parameters.Add("@fLastYRotation", SqlDbType.Float).Value = fLastYRotation;
		sc.Parameters.Add("@nPreviousNationId", SqlDbType.Int).Value = nPreviousNationId;
		sc.Parameters.Add("@nPreviousContinentId", SqlDbType.Int).Value = nPreviousContinentId;
		sc.Parameters.Add("@fPreviousXPosition", SqlDbType.Float).Value = fPreviousXPosition;
		sc.Parameters.Add("@fPreviousYPosition", SqlDbType.Float).Value = fPreviousYPosition;
		sc.Parameters.Add("@fPreviousZPosition", SqlDbType.Float).Value = fPreviousZPosition;
		sc.Parameters.Add("@fPreviousYRotation", SqlDbType.Float).Value = fPreviousYRotation;
		sc.Parameters.Add("@nRestTime", SqlDbType.Int).Value = nRestTime;
		sc.Parameters.Add("@dailyAccessTimeUpdateTime", SqlDbType.DateTimeOffset).Value = dailyAccessTimeUpdateTime;
		sc.Parameters.Add("@fDailyAccessTime", SqlDbType.Float).Value = fDailyAccessTime;
		sc.Parameters.Add("@bIsRiding", SqlDbType.Bit).Value = bIsRiding;
		sc.Parameters.Add("@undergroundMazeDate", SqlDbType.Date).Value = undergroundMazeDate;
		sc.Parameters.Add("@nUndergroundMazePlayTime", SqlDbType.Int).Value = nUndergroundMazePlayTime;
		sc.Parameters.Add("@artifactRoomSweepStartTime", SqlDbType.DateTimeOffset).Value = SFDBUtil.NullToDBNull(artifactRoomSweepStartTime);
		return sc;
	}

	public static int DeleteHero(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTimeOffset delTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHero";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@delTime", SqlDbType.DateTimeOffset).Value = delTime;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static int HeroCount(SqlConnection conn, SqlTransaction trans, Guid accountId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroCount";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRow Hero(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Hero";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static DataRow Hero_pk(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Hero_pk";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static DataRowCollection InventorySlots(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_InventorySlots";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddInventorySlot(Guid heroId, int nSlotIndex, int nSlotType, Guid? heroMainGearId, int nSubGearId, int nItemId, int nItemCount, bool bItemOwned, Guid? heroMountGearId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddInventorySlot";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nSlotType", SqlDbType.Int).Value = nSlotType;
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = SFDBUtil.NullToDBNull(heroMainGearId);
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@heroMountGearId", SqlDbType.UniqueIdentifier).Value = SFDBUtil.NullToDBNull(heroMountGearId);
		return sc;
	}

	public static SqlCommand CSC_UpdateInventorySlot(Guid heroId, int nSlotIndex, int nSlotType, Guid? heroMainGearId, int nSubGearId, int nItemId, int nItemCount, bool bItemOwned, Guid? heroMountGearId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateInventorySlot";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nSlotType", SqlDbType.Int).Value = nSlotType;
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = SFDBUtil.NullToDBNull(heroMainGearId);
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@heroMountGearId", SqlDbType.UniqueIdentifier).Value = SFDBUtil.NullToDBNull(heroMountGearId);
		return sc;
	}

	public static SqlCommand CSC_AddOrUpdateInventorySlot(Guid heroId, int nSlotIndex, int nSlotType, Guid? heroMainGearId, int nSubGearId, int nItemId, int nItemCount, bool bItemOwned, Guid? heroMountGearId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateInventorySlot";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nSlotType", SqlDbType.Int).Value = nSlotType;
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = SFDBUtil.NullToDBNull(heroMainGearId);
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@heroMountGearId", SqlDbType.UniqueIdentifier).Value = SFDBUtil.NullToDBNull(heroMountGearId);
		return sc;
	}

	public static SqlCommand CSC_DeleteInventorySlot(Guid heroId, int nSlotIndex)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteInventorySlot";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		return sc;
	}

	public static DataRowCollection HeroMainGears(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroMainGears";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_UpdateHero_MainGear(Guid heroId, Guid weaponHeroMainGearId, Guid armorHeroMainGearId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_MainGear";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weaponHeroMainGearId", SqlDbType.UniqueIdentifier).Value = weaponHeroMainGearId;
		sc.Parameters.Add("@armorHeroMainGearId", SqlDbType.UniqueIdentifier).Value = armorHeroMainGearId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroMainGear_Enchant(Guid heroMainGearId, int nEnchantLevel, bool bOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroMainGear_Enchant";
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@nEnchantLevel", SqlDbType.Int).Value = nEnchantLevel;
		sc.Parameters.Add("@bOwned", SqlDbType.Bit).Value = bOwned;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroMainGear_Owned(Guid heroMainGearId, bool bOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroMainGear_Owned";
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@bOwned", SqlDbType.Bit).Value = bOwned;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMainGear(Guid heroMainGearId, Guid heroId, int nMainGearId, int nEnchantLevel, bool bOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMainGear";
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMainGearId", SqlDbType.Int).Value = nMainGearId;
		sc.Parameters.Add("@nEnchantLevel", SqlDbType.Int).Value = nEnchantLevel;
		sc.Parameters.Add("@bOwned", SqlDbType.Bit).Value = bOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroMainGear(Guid heroMainGearId, DateTimeOffset delTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroMainGear";
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@delTime", SqlDbType.DateTimeOffset).Value = delTime;
		return sc;
	}

	public static DataRowCollection HeroMainGearOptionAttrs(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroMainGearOptionAttrs";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_UpdateHeroMainGearOptionAttr(Guid heroMainGearId, int nIndex, int nGrade, int nAttrId, long lnAttrValueId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroMainGearOptionAttrs";
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@nIndex", SqlDbType.Int).Value = nIndex;
		sc.Parameters.Add("@nGrade", SqlDbType.Int).Value = nGrade;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		sc.Parameters.Add("@lnAttrValueId", SqlDbType.BigInt).Value = lnAttrValueId;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMainGearOptionAttr(Guid heroMainGearId, int nIndex, int nGrade, int nAttrId, long lnAttrValueId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMainGearOptionAttr";
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@nIndex", SqlDbType.Int).Value = nIndex;
		sc.Parameters.Add("@nGrade", SqlDbType.Int).Value = nGrade;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		sc.Parameters.Add("@lnAttrValueId", SqlDbType.BigInt).Value = lnAttrValueId;
		return sc;
	}

	public static DataRowCollection HeroMainGearRefinementAttrs(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroMainGearRefinementAttrs";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_DeleteHeroMainGearRefinementAttrs(Guid heroMainGearId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroMainGearRefinementAttrs";
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		return sc;
	}

	public static SqlCommand CSC_AddHeroMainGearRefinementAttr(Guid heroMainGearId, int nTurn, int nIndex, int nGrade, int nAttrId, long lnAttrValueId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMainGearRefinementAttr";
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@nTurn", SqlDbType.Int).Value = nTurn;
		sc.Parameters.Add("@nIndex", SqlDbType.Int).Value = nIndex;
		sc.Parameters.Add("@nGrade", SqlDbType.Int).Value = nGrade;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		sc.Parameters.Add("@lnAttrValueId", SqlDbType.BigInt).Value = lnAttrValueId;
		return sc;
	}

	public static DataRowCollection HeroSubGears(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroSubGears";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_UpdateHeroSubGear_Equip(Guid heroId, int nSubGearId, bool bEquipped)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroSubGear_Equip";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("bEquipped", SqlDbType.Bit).Value = bEquipped;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroSubGear_LevelUp(Guid heroId, int nSubGearId, int nLevel, int nQuality)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroSubGear_LevelUp";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nQuality", SqlDbType.Int).Value = nQuality;
		return sc;
	}

	public static SqlCommand CSC_AddHeroSubGear(Guid heroId, int nSubGearId, int nLevel, int nQuality, bool bEquipped, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroSubGear";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nQuality", SqlDbType.Int).Value = nQuality;
		sc.Parameters.Add("@bEquipped", SqlDbType.Bit).Value = bEquipped;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static DataRowCollection HeroSubGearSoulstoneSockets(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroSubGearSoulstoneSockets";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroSubGearSoulstoneSocket(Guid heroId, int nSubGearId, int nSocketIndex, int nItemId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroSubGearSoulstoneSocket";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nSocketIndex", SqlDbType.Int).Value = nSocketIndex;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroSubGearSoulstoneSocket_Item(Guid heroId, int nSubGearId, int nSocketIndex, int nItemId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroSubGearSoulstoneSocket_Item";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nSocketIndex", SqlDbType.Int).Value = nSocketIndex;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroSubGearSoulstoneSocket(Guid heroId, int nSubGearId, int nSocketIndex)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroSubGearSoulstoneSocket";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nSocketIndex", SqlDbType.Int).Value = nSocketIndex;
		return sc;
	}

	public static DataRowCollection HeroSubGearRuneSockets(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroSubGearRuneSockets";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroSubGearRuneSocket(Guid heroId, int nSubGearId, int nSocketIndex, int nItemId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroSubGearRuneSocket";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nSocketIndex", SqlDbType.Int).Value = nSocketIndex;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroSubGearRuneSocket_Item(Guid heroId, int nSubGearId, int nSocketIndex, int nItemId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroSubGearRuneSocket_Item";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nSocketIndex", SqlDbType.Int).Value = nSocketIndex;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroSubGearRuneSocket(Guid heroId, int nSubGearId, int nSocketIndex)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroSubGearRuneSocket";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nSocketIndex", SqlDbType.Int).Value = nSocketIndex;
		return sc;
	}

	public static DataRowCollection Mails(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTimeOffset currentTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Mails";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@currentTime", SqlDbType.DateTimeOffset).Value = currentTime;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_UpdateMail_Receive(Guid mailId, DateTimeOffset receiveTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateMail_Receive";
		sc.Parameters.Add("@mailId", SqlDbType.UniqueIdentifier).Value = mailId;
		sc.Parameters.Add("@receiveTime", SqlDbType.DateTimeOffset).Value = receiveTime;
		return sc;
	}

	public static SqlCommand CSC_AddMail(Guid mailId, Guid heroId, int nTitleType, string sTitle, int nContentType, string sContent, DateTimeOffset regTime, DateTimeOffset expireTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddMail";
		sc.Parameters.Add("@mailId", SqlDbType.UniqueIdentifier).Value = mailId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nTitleType", SqlDbType.Int).Value = nTitleType;
		sc.Parameters.Add("@sTitle", SqlDbType.NVarChar).Value = sTitle;
		sc.Parameters.Add("@nContentType", SqlDbType.Int).Value = nContentType;
		sc.Parameters.Add("@sContent", SqlDbType.NVarChar).Value = sContent;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		sc.Parameters.Add("@expireTime", SqlDbType.DateTimeOffset).Value = expireTime;
		return sc;
	}

	public static SqlCommand CSC_DeleteMail(Guid mailId, DateTimeOffset delTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteMail";
		sc.Parameters.Add("@mailId", SqlDbType.UniqueIdentifier).Value = mailId;
		sc.Parameters.Add("@delTime", SqlDbType.DateTimeOffset).Value = delTime;
		return sc;
	}

	public static DataRowCollection MailAttachments(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTimeOffset currentTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MailAttachments";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@currentTime", SqlDbType.DateTimeOffset).Value = currentTime;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddMailAttachments(Guid mailId, int nAttachmentNo, int nItemId, int nItemCount, bool bItemOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGapi_AddMailAttachment";
		sc.Parameters.Add("@mailId", SqlDbType.UniqueIdentifier).Value = mailId;
		sc.Parameters.Add("@nAttachmentNo", SqlDbType.Int).Value = nAttachmentNo;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		return sc;
	}

	public static DataRowCollection MailDeliveryTargets(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MailDeliveryTargets";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow MailById_x(SqlConnection conn, SqlTransaction trans, Guid mailId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MailById_x";
		sc.Parameters.Add("@mailId", SqlDbType.UniqueIdentifier).Value = mailId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static DataRowCollection MailAttachmentById_x(SqlConnection conn, SqlTransaction trans, Guid mailId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MailAttachmentById_x";
		sc.Parameters.Add("@mailId", SqlDbType.UniqueIdentifier).Value = mailId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int UpdateMail_DeliveryCompletion(SqlConnection conn, SqlTransaction trans, Guid mailId, DateTimeOffset deliveryTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateMail_DeliveryCompletion";
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.Parameters.Add("@mailId", SqlDbType.UniqueIdentifier).Value = mailId;
		sc.Parameters.Add("@deliveryTime", SqlDbType.DateTimeOffset).Value = deliveryTime;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static SqlCommand CSC_UpdateHero_Gold(Guid heroId, long lnGold)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_Gold";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@lnGold", SqlDbType.BigInt).Value = lnGold;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_MainGearEnchantDateCount(Guid heroId, DateTime mainGearEnchantDate, int nMainGearEnchantCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_MainGearEnchantDateCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@mainGearEnchantDate", SqlDbType.Date).Value = mainGearEnchantDate;
		sc.Parameters.Add("@nMainGearEnchantCount", SqlDbType.Int).Value = nMainGearEnchantCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_MainGearRefinementCount(Guid heroId, DateTime mainGearRefinementDate, int nMainGearRefinementCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_MainGearRefinementDateCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@mainGearRefinementDate", SqlDbType.Date).Value = mainGearRefinementDate;
		sc.Parameters.Add("@nMainGearRefinementCount", SqlDbType.Int).Value = nMainGearRefinementCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_PaidInventorySlotCount(Guid heroId, int nPaidInventorySlotCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_PaidInventorySlotCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nPaidInventorySlotCount", SqlDbType.Int).Value = nPaidInventorySlotCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_RestTime(Guid heroId, int nRestTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_RestTime";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRestTime", SqlDbType.Int).Value = nRestTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_DailyAttendDateCount(Guid heroId, DateTime dailyAttendRewardDate, int nDailyAttendRewardDay)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_DailyAttendDateCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@dailyAttendRewardDate", SqlDbType.Date).Value = dailyAttendRewardDate;
		sc.Parameters.Add("@nDailyAttendRewardDay", SqlDbType.Int).Value = nDailyAttendRewardDay;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_WeekendAttendRewardDate(Guid heroId, DateTime weekendAttendRewardDate)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_WeekendAttendRewardDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weekendAttendRewardDate", SqlDbType.Date).Value = weekendAttendRewardDate;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_DailAccessTime(Guid heroId, int nDailyAccessTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_DailyAccessTime";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDailyAccessTime", SqlDbType.Int).Value = nDailyAccessTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_ExpPotionDateCount(Guid heroId, DateTime expPotionUseDate, int nExpPotionUseCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_ExpPotionDateCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@expPotionUseDate", SqlDbType.Date).Value = expPotionUseDate;
		sc.Parameters.Add("@nExpPotionUseCount", SqlDbType.Int).Value = nExpPotionUseCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_EquippedMount(Guid heroId, int nEquippedMountId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_EquippedMount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nEquippedMountId", SqlDbType.Int).Value = nEquippedMountId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_MountGearRefinementDateCount(Guid heroId, DateTime mountGearRefinementDate, int nMountGearRefinementCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_MountGearRefinementDateCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@mountGearRefinementDate", SqlDbType.Date).Value = mountGearRefinementDate;
		sc.Parameters.Add("@nMountGearRefinementCount", SqlDbType.Int).Value = nMountGearRefinementCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_EquipWing(Guid heroId, int nEquippedWingId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_EquipWing";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nEquippedWingId", SqlDbType.Int).Value = nEquippedWingId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_EnchantWing(Guid heroId, int nWingStep, int nWingLevel, int nWingExp)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_EnchantWing";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nWingStep", SqlDbType.Int).Value = nWingStep;
		sc.Parameters.Add("@nWingLevel", SqlDbType.Int).Value = nWingLevel;
		sc.Parameters.Add("@nWingExp", SqlDbType.Int).Value = nWingExp;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_MainGearEnchantLevelSetNo(Guid heroId, int nMainGearEnchantLevelSetNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_MainGearEnchantLevelSetNo";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMainGearEnchantLevelSetNo", SqlDbType.Int).Value = nMainGearEnchantLevelSetNo;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_SubGearSoulstoneLevelSetNo(Guid heroId, int nSubGearSoulstoneLevelSetNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_SubGearSoulstoneLevelSetNo";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearSoulstoneLevelSetNo", SqlDbType.Int).Value = nSubGearSoulstoneLevelSetNo;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_UseExpScroll(Guid heroId, DateTime expScrollUseDate, int nExpScrollUseCount, DateTimeOffset? expScrollStartTime, int nExpScrollDuration, int nExpScrollItemId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_UseExpScroll";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@expScrollUseDate", SqlDbType.Date).Value = expScrollUseDate;
		sc.Parameters.Add("@nExpScrollUseCount", SqlDbType.Int).Value = nExpScrollUseCount;
		sc.Parameters.Add("@expScrollStartTime", SqlDbType.DateTimeOffset).Value = SFDBUtil.NullToDBNull(expScrollStartTime);
		sc.Parameters.Add("@nExpScrollDuration", SqlDbType.Int).Value = nExpScrollDuration;
		sc.Parameters.Add("@nExpScrollItemId", SqlDbType.Int).Value = nExpScrollItemId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_Exploit(Guid heroId, int nExploitPoint, DateTimeOffset exploitPointUpdateTime, DateTime dailyExploitPointDate, int nDailyExploitPoint)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_Exploit";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nExploitPoint", SqlDbType.Int).Value = nExploitPoint;
		sc.Parameters.Add("@exploitPointUpdateTime", SqlDbType.DateTimeOffset).Value = exploitPointUpdateTime;
		sc.Parameters.Add("@dailyExploitPointDate", SqlDbType.Date).Value = dailyExploitPointDate;
		sc.Parameters.Add("@nDailyExploitPoint", SqlDbType.Int).Value = nDailyExploitPoint;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_Rank(Guid heroId, int nRankNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_Rank";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRankNo", SqlDbType.Int).Value = nRankNo;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_RankReward(Guid heroId, int nRankRewardReceivedRankNo, DateTime rankRewardReceivedDate)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_RankReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRankRewardReceivedRankNo", SqlDbType.Int).Value = nRankRewardReceivedRankNo;
		sc.Parameters.Add("@rankRewardReceivedDate", SqlDbType.Date).Value = rankRewardReceivedDate;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_HonorPoint(Guid heroId, int nHonorPoint)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_HonorPoint";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nHonorPoint", SqlDbType.Int).Value = nHonorPoint;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_UseDistortionScroll(Guid heroId, DateTime distortionScrollUseDate, int nDistortionScrollUseCount, DateTimeOffset distortionScrollStartTime, int nDistortionScrollDuration)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_UseDistortionScroll";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@distortionScrollUseDate", SqlDbType.Date).Value = distortionScrollUseDate;
		sc.Parameters.Add("@nDistortionScrollUseCount", SqlDbType.Int).Value = nDistortionScrollUseCount;
		sc.Parameters.Add("@distortionScrollStartTime", SqlDbType.DateTimeOffset).Value = distortionScrollStartTime;
		sc.Parameters.Add("@nDistortionScrollDuration", SqlDbType.Int).Value = nDistortionScrollDuration;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_DistortionCanceled(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_DistortionCanceled";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_DailyServerLevelRankingReward(Guid heroId, int nRewardedDailyServerLevelRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_DailyServerLevelRankingReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardedDailyServerLevelRankingNo", SqlDbType.Int).Value = nRewardedDailyServerLevelRankingNo;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_AttainmentEntryReawrd(Guid heroId, int nRewardedAttainmentEntryNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_AttainmentEntryReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardedAttainmentEntryNo", SqlDbType.Int).Value = nRewardedAttainmentEntryNo;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_FieldOfHonorRanking(Guid heroId, int nFieldOfHonorRanking)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_FieldOfHonorRanking";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nFieldOfHonorRanking", SqlDbType.Int).Value = nFieldOfHonorRanking;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_FieldOfHonorSuccessiveCount(Guid heroId, int nFieldOfHonorSuccessiveCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_FieldOfHonorSuccessiveCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nFieldOfHonorSuccessiveCount", SqlDbType.Int).Value = nFieldOfHonorSuccessiveCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_RewardedDailyFieldOfHonorRankingNo(Guid heroId, int nRewardedDailyFieldOfHonorRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_RewardedDailyFieldOfHonorRankingNo";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardedDailyFieldOfHonorRankingNo", SqlDbType.Int).Value = nRewardedDailyFieldOfHonorRankingNo;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_NationDonateDateCount(Guid heroId, DateTime nationDonationDate, int nNationDonationCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_NationDonateDateCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nationDonationDate", SqlDbType.Date).Value = nationDonationDate;
		sc.Parameters.Add("@nNationDonationCount", SqlDbType.Int).Value = nNationDonationCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_MonsterKillCount(Guid heroId, int nAccMonsterKillCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_MonsterKillCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nAccMonsterKillCount", SqlDbType.Int).Value = nAccMonsterKillCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_EpicBaitItemUseCount(Guid heroId, int nAccEpicBaitItemUseCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_EpicBaitItemUseCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nAccEpicBaitItemUseCount", SqlDbType.Int).Value = nAccEpicBaitItemUseCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_LegendBaitItemUseCount(Guid heroId, int nAccLegendBaitItemUseCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_LegendBaitItemUseCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nAccLegendBaitItemUseCount", SqlDbType.Int).Value = nAccLegendBaitItemUseCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_NationWarWinCount(Guid heroId, int nAccNationWarWinCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_NationWarWinCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nAccNationWarWinCount", SqlDbType.Int).Value = nAccNationWarWinCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_NationWarKillCount(Guid heroId, int nAccNationWarKillCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_NationWarKillCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nAccNationWarKillCount", SqlDbType.Int).Value = nAccNationWarKillCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_NationWarCommanderKillCount(Guid heroId, int nAccNationWarCommanderKillCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_NationWarCommanderKillCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nAccNationWarCommanderKillCount", SqlDbType.Int).Value = nAccNationWarCommanderKillCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_NationWarImmediateRevivalCount(Guid heroId, int nAccNationWarImmediateRevivalCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_NationWarImmediateRevivalCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nAccNationWarImmediateRevivalCount", SqlDbType.Int).Value = nAccNationWarImmediateRevivalCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_MaxBattlePower(Guid heroId, long lnMaxBattlePower)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_MaxBattlePower";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@lnMaxBattlePower", SqlDbType.BigInt).Value = lnMaxBattlePower;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_MaxGold(Guid heroId, long lnMaxGold)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_MaxGold";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@lnMaxGold", SqlDbType.BigInt).Value = lnMaxGold;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_MaxAcquisitionGearGrade(Guid heroId, int nMaxAcquisitionMainGearGrade)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_MaxAcquisitionMainGearGrade";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMaxAcquisitionMainGearGrade", SqlDbType.Int).Value = nMaxAcquisitionMainGearGrade;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_MaxEquippedGearEnchantLevel(Guid heroId, int nMaxEquippedMainGearEnchantLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_MaxEquippedMainGearEnchantLevel";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMaxEquippedMainGearEnchantLevel", SqlDbType.Int).Value = nMaxEquippedMainGearEnchantLevel;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_SoulPowder(Guid heroId, int nSoulPowder)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_SoulPowder";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSoulPowder", SqlDbType.Int).Value = nSoulPowder;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_CreaturCardCollectionFamePoint(Guid heroId, int nCreatureCardCollectionFamePoint, DateTimeOffset creatureCardCollectionFamePointUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_CreatureCardCollectionFamePoint";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCreatureCardCollectionFamePoint", SqlDbType.Int).Value = nCreatureCardCollectionFamePoint;
		sc.Parameters.Add("@creatureCardCollectionFamePointUpdateTime", SqlDbType.DateTimeOffset).Value = creatureCardCollectionFamePointUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_CreatureCardShopId(Guid heroId, Guid creatureCardShopId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_CreatureCardShopId";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@creatureCardShopId", SqlDbType.UniqueIdentifier).Value = creatureCardShopId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_CreatureCardShopSchedule(Guid heroId, DateTime creatureCardShopRefreshDate, int nCreatureCardShopRefreshScheduleId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_CreatureCardShopSchedule";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@creatureCardShopRefreshDate", SqlDbType.Date).Value = creatureCardShopRefreshDate;
		sc.Parameters.Add("@nCreatureCardShopRefreshScheduleId", SqlDbType.Int).Value = nCreatureCardShopRefreshScheduleId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_CreatureCardShopPaidRefresh(Guid heroId, DateTime creatureCardShopPaidRefreshDate, int nCreatureCardShopPaidRefreshCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_CreatureCardPaidRefresh";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@creatureCardShopPaidRefreshDate", SqlDbType.Date).Value = creatureCardShopPaidRefreshDate;
		sc.Parameters.Add("@nCreatureCardShopPaidRefreshCount", SqlDbType.Int).Value = nCreatureCardShopPaidRefreshCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_Stamina(Guid heroId, int nStamina)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_Stamina";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nStamina", SqlDbType.Int).Value = nStamina;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_StaminaRecoverySchedule(Guid heroId, DateTime staminaRecoveryDate, int nStaminaRecoveryScheduleId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_StaminaRecoverySchedule";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@staminaRecoveryDate", SqlDbType.Date).Value = staminaRecoveryDate;
		sc.Parameters.Add("@nStaminaRecoveryScheduleId", SqlDbType.Int).Value = nStaminaRecoveryScheduleId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_StaminaBuyDateCount(Guid heroId, DateTime staminaBuyDate, int nStaminaBuyCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGapi_UpdateHero_StaminaBuyDateCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@staminaBuyDate", SqlDbType.Date).Value = staminaBuyDate;
		sc.Parameters.Add("@nStaminaBuyCount", SqlDbType.Int).Value = nStaminaBuyCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_BattleSetting(Guid heroId, int nLootingItemMinGrade)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_BattleSetting";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nLootingItemMinGrade", SqlDbType.Int).Value = nLootingItemMinGrade;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_TodayMissionTurialStarted(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_TodayMissionTutorialStarted";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection HeroSkills(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroSkills";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroSkill(Guid heroId, int nSkillId, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroSkill";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSkillId", SqlDbType.Int).Value = nSkillId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static int AddHeroSkill(SqlConnection conn, SqlTransaction trans, Guid heroId, int nSkillId, int nLevel)
	{
		SqlCommand sc = CSC_AddHeroSkill(heroId, nSkillId, nLevel);
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static SqlCommand CSC_UpdateHeroSkill_Level(Guid heroId, int nSkillId, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroSkill_Level";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSkillId", SqlDbType.Int).Value = nSkillId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static DataRow HeroMainQuest_Current(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroMainQuest_Current";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddHeroMainQuest(Guid heroId, int nMainQuestNo, bool bIsCartRiding, int nCartContinentId, float fCartXPosition, float fCartYPosition, float fCartZPosition, float fCartYRotation, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMainQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMainQuestNo", SqlDbType.Int).Value = nMainQuestNo;
		sc.Parameters.Add("@bIsCartRiding", SqlDbType.Bit).Value = bIsCartRiding;
		sc.Parameters.Add("@nCartContinentId", SqlDbType.Int).Value = nCartContinentId;
		sc.Parameters.Add("@fCartXPosition", SqlDbType.Float).Value = fCartXPosition;
		sc.Parameters.Add("@fCartYPosition", SqlDbType.Float).Value = fCartYPosition;
		sc.Parameters.Add("@fCartZPosition", SqlDbType.Float).Value = fCartZPosition;
		sc.Parameters.Add("@fCartYRotation", SqlDbType.Float).Value = fCartYRotation;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroMainQuest_ProgressCount(Guid heroId, int nMainQuestNo, int nProgressCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroMainQuest_ProgressCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMainQuestNo", SqlDbType.Int).Value = nMainQuestNo;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroMainQuest_Complete(Guid heroId, int nMainQuestNo, DateTimeOffset completionTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroMainQuest_Complete";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMainQuestNo", SqlDbType.Int).Value = nMainQuestNo;
		sc.Parameters.Add("@completionTime", SqlDbType.DateTimeOffset).Value = completionTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroMainQuest_Cart(Guid heroId, int nMainQuestNo, bool bIsCartRiding, int nCartContinentId, float fCartXPosition, float fCartYPosition, float fCartZPosition, float fCartYRotation)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroMainQuest_Cart";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMainQuestNo", SqlDbType.Int).Value = nMainQuestNo;
		sc.Parameters.Add("@bIsCartRiding", SqlDbType.Bit).Value = bIsCartRiding;
		sc.Parameters.Add("@nCartContinentId", SqlDbType.Int).Value = nCartContinentId;
		sc.Parameters.Add("@fCartXPosition", SqlDbType.Float).Value = fCartXPosition;
		sc.Parameters.Add("@fCartYPosition", SqlDbType.Float).Value = fCartYPosition;
		sc.Parameters.Add("@fCartZPosition", SqlDbType.Float).Value = fCartZPosition;
		sc.Parameters.Add("@fCartYRotation", SqlDbType.Float).Value = fCartYRotation;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_FreeImmediateRevivalCount(Guid heroId, DateTime date, int nFreeImmediateRevivalDailyCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_FreeImmediateRevivalCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@freeImmediateRevivalDate", SqlDbType.DateTime).Value = date;
		sc.Parameters.Add("@nFreeImmediateRevivalCount", SqlDbType.Int).Value = nFreeImmediateRevivalDailyCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_PaidImmediateRevivalCount(Guid heroId, DateTime date, int nPaidImmediateRevivalDailyCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_PaidImmediateRevivalCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@paidImmediateRevivalDate", SqlDbType.DateTime).Value = date;
		sc.Parameters.Add("@nPaidImmediateRevivalCount", SqlDbType.Int).Value = nPaidImmediateRevivalDailyCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateAccount_UnOwnDia(Guid accountId, int nBaseUnOwnDia, int nBonusUnOwnDia)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateAccount_UnOwnDia";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@nBaseUnOwnDia", SqlDbType.Int).Value = nBaseUnOwnDia;
		sc.Parameters.Add("@nBonusUnOwnDia", SqlDbType.Int).Value = nBonusUnOwnDia;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_OwnDia(Guid heroId, int nOwnDia)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_OwnDia";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nOwnDia", SqlDbType.Int).Value = nOwnDia;
		return sc;
	}

	public static DataRowCollection HeroMainQuestDungeonRewards(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroMainQuestDungeonRewards";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroMainQuestDungeonReward(Guid heroId, int nDungeonId, int nStepNo, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMainQuestDungeonReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDungeonId", SqlDbType.Int).Value = nDungeonId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStepNo;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static DataRowCollection HeroLevelUpRewards(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroLevelUpRewards";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroLevelUpReward(Guid heroId, int nEntryId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroLevelUpReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nEntryId", SqlDbType.Int).Value = nEntryId;
		return sc;
	}

	public static DataRowCollection HeroAccessRewards(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroAccessRewards";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroAccessReward(Guid heroId, DateTime date, int nEntryId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroAccessReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nEntryId", SqlDbType.Int).Value = nEntryId;
		return sc;
	}

	public static SqlCommand CSC_UpdateAccount_VipPoint(Guid accountId, int nVipPoint)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateAccount_VipPoint";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@nVipPoint", SqlDbType.Int).Value = nVipPoint;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_VipPoint(Guid heroId, int nVipPoint)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_VipPoint";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nVipPoint", SqlDbType.Int).Value = nVipPoint;
		return sc;
	}

	public static DataRowCollection VipLevelRewards(SqlConnection conn, SqlTransaction trans, Guid accountId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_VipLevelRewards";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddVipLevelReward(Guid accountId, int nVipLevel, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddVipLevelReward";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@nVipLevel", SqlDbType.Int).Value = nVipLevel;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection HeroMounts(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroMounts";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroMount(Guid heroId, int nMountId, int nLevel, int nSatiety)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMountId", SqlDbType.Int).Value = nMountId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nSatiety", SqlDbType.Int).Value = nSatiety;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroMount_Level(Guid heroId, int nMountId, int nLevel, int nSatiety)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroMount_Level";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMountId", SqlDbType.Int).Value = nMountId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nSatiety", SqlDbType.Int).Value = nSatiety;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroMount_AwakeningLevel(Guid heroId, int nMountId, int nAwakeningLevel, int nAwakeningExp)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroMount_AwakeningLevel";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMountId", SqlDbType.Int).Value = nMountId;
		sc.Parameters.Add("@nAwakeningLevel", SqlDbType.Int).Value = nAwakeningLevel;
		sc.Parameters.Add("@nAwakeningExp", SqlDbType.Int).Value = nAwakeningExp;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroMount_PotionAttr(Guid heroId, int nMountId, int nPotionAttrCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroMount_PotionAttr";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMountId", SqlDbType.Int).Value = nMountId;
		sc.Parameters.Add("@nPotionAttrCount", SqlDbType.Int).Value = nPotionAttrCount;
		return sc;
	}

	public static DataRowCollection HeroMountGears(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroMountGears";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroMountGear(Guid heroMountGearId, Guid heroId, int nMountGearId, bool bOwned, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMountGear";
		sc.Parameters.Add("@heroMountGearId", SqlDbType.UniqueIdentifier).Value = heroMountGearId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMountGearId", SqlDbType.Int).Value = nMountGearId;
		sc.Parameters.Add("@bOwned", SqlDbType.Bit).Value = bOwned;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroMountGear_Owned(Guid heroMountGearId, bool bOwned)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroMountGear_Owned";
		sc.Parameters.Add("@heroMountGearId", SqlDbType.UniqueIdentifier).Value = heroMountGearId;
		sc.Parameters.Add("@bOwned", SqlDbType.Bit).Value = bOwned;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroMountGear(Guid heroMountGearId, DateTimeOffset delTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroMountGear";
		sc.Parameters.Add("@heroMountGearId", SqlDbType.UniqueIdentifier).Value = heroMountGearId;
		sc.Parameters.Add("@delTime", SqlDbType.DateTimeOffset).Value = delTime;
		return sc;
	}

	public static DataRowCollection HeroMountGearOptionAttrs(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroMoutnGearOptionAttrs";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroMountGearOptionAttr(Guid heroMountGearId, int nIndex, int nGrade, int nAttrId, long lnAttrValueId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroMountGearOptionAttr";
		sc.Parameters.Add("@heroMountGearId", SqlDbType.UniqueIdentifier).Value = heroMountGearId;
		sc.Parameters.Add("@nIndex", SqlDbType.Int).Value = nIndex;
		sc.Parameters.Add("@nGrade", SqlDbType.Int).Value = nGrade;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		sc.Parameters.Add("@lnAttrValueId", SqlDbType.BigInt).Value = lnAttrValueId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroMountGearOptionAttr(Guid heroMountGearId, int nIndex, int nGrade, int nAttrId, long lnAttrValueId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroMountGearOptionAttr";
		sc.Parameters.Add("@heroMountGearId", SqlDbType.UniqueIdentifier).Value = heroMountGearId;
		sc.Parameters.Add("@nIndex", SqlDbType.Int).Value = nIndex;
		sc.Parameters.Add("@nGrade", SqlDbType.Int).Value = nGrade;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		sc.Parameters.Add("@lnAttrValueId", SqlDbType.BigInt).Value = lnAttrValueId;
		return sc;
	}

	public static DataRowCollection EquippedHeroMountGearSlots(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_EquippedHeroMountGearSlots";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateEquippedHeroMountGearSlot(Guid heroId, int nSlotIndex, Guid heroMountGearId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateEquippedHeroMountGearSlot";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@heroMountGearId", SqlDbType.UniqueIdentifier).Value = heroMountGearId;
		return sc;
	}

	public static SqlCommand CSC_DeleteEquippedHeroMountGearSlot(Guid heroId, int nSlotIndex)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteEquippedHeroMountGearSlot";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_HPAndStamina(Guid heroId, int nHP, int nStamina)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_HPAndStamina";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nHP", SqlDbType.Int).Value = nHP;
		sc.Parameters.Add("@nStamina", SqlDbType.Int).Value = nStamina;
		return sc;
	}

	public static int UpdateHero_HPAndStamina(SqlConnection conn, SqlTransaction trans, Guid heroId, int nHP, int nStamina)
	{
		SqlCommand sc = CSC_UpdateHero_HPAndStamina(heroId, nHP, nStamina);
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static SqlCommand CSC_UpdateHero_BattlePower(Guid heroId, long lnBattlePower, DateTimeOffset battlePowerUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_BattlePower";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@lnBattlePower", SqlDbType.BigInt).Value = lnBattlePower;
		sc.Parameters.Add("@battlePowerUpdateTime", SqlDbType.DateTimeOffset).Value = battlePowerUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_Level(Guid heroId, int nLevel, long lnExp, DateTimeOffset levelUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_Level";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@lnExp", SqlDbType.BigInt).Value = lnExp;
		sc.Parameters.Add("@levelUpdateTime", SqlDbType.DateTimeOffset).Value = levelUpdateTime;
		return sc;
	}

	public static DataRowCollection HeroWings(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroWings";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroWing(Guid heroId, int nWingId, int nMemoryPieceStep)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWing";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nWingId", SqlDbType.Int).Value = nWingId;
		sc.Parameters.Add("@nMemoryPieceStep", SqlDbType.Int).Value = nMemoryPieceStep;
		return sc;
	}

	public static DataRowCollection HeroWingEnchants(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroWingEnchants";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroWingEnchant(Guid heroId, int nPartId, int nStep, int nLevel, int nEnchantCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroWingEnchant";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nPartId", SqlDbType.Int).Value = nPartId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nEnchantCount", SqlDbType.Int).Value = nEnchantCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroWingEnchant(Guid heroId, int nPartId, int nStep, int nLevel, int nEnchantCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroWingEnchant";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nPartId", SqlDbType.Int).Value = nPartId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nEnchantCount", SqlDbType.Int).Value = nEnchantCount;
		return sc;
	}

	public static SqlCommand CSC_AddOrUpdateHeroWingEnchant(Guid heroId, int nPartId, int nStep, int nLevel, int nEnchantCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroWingEnchant";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nPartId", SqlDbType.Int).Value = nPartId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nEnchantCount", SqlDbType.Int).Value = nEnchantCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroWing_MemoryPieceStep(Guid heroId, int nWingId, int nMemoryPieceStep)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroWing_MemoryPieceStep";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nWingId", SqlDbType.Int).Value = nWingId;
		sc.Parameters.Add("@nMemoryPieceStep", SqlDbType.Int).Value = nMemoryPieceStep;
		return sc;
	}

	public static DataRowCollection HeroWingMemoryPieceSlots(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroWingMemoryPieceSlots";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateHeroWingMemoryPieceSlot(Guid heroId, int nWingId, int nSlotIndex, int nAccAttrValue)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroWingMemoryPieceSlot";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nWingId", SqlDbType.Int).Value = nWingId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nAccAttrValue", SqlDbType.Int).Value = nAccAttrValue;
		return sc;
	}

	public static DataRowCollection StoryDungeonClears(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_StoryDungeonClears";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateStoryDungeonClear(Guid heroId, int nDungeonNo, int nMaxDifficulty)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateStoryDungeonClear";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDungeonNo", SqlDbType.Int).Value = nDungeonNo;
		sc.Parameters.Add("@nMaxDifficulty", SqlDbType.Int).Value = nMaxDifficulty;
		return sc;
	}

	public static DataRowCollection StoryDungeonEnterCountsOfDate(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_StoryDungeonEnterCountsOfDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateStoryDungeonPlay(Guid heroId, DateTime date, int nDungeonNo, int nCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateStoryDungeonPlay";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nDungeonNo", SqlDbType.Int).Value = nDungeonNo;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		return sc;
	}

	public static DataRowCollection HeroRealAttrValues(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroRealAttrValues";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_DeleteHeroRealAttrValues(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroRealAttrValues";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static SqlCommand CSC_AddHeroRealAttrValue(Guid heroId, int nAttrId, int nValue)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroRealAttrValue";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		sc.Parameters.Add("@nValue", SqlDbType.Int).Value = nValue;
		return sc;
	}

	public static DataRowCollection EquippedHeroMainGears(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_EquippedHeroMainGears";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection EquippedHeroMainGearOptionAttrs(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_EquippedHeroMainGearOptionAttrs";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection EquippedHeroSubGears(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_EquippedHeroSubGears";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection EquippedHeroSubGearSoulstoneSockets(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_EquippedHeroSubGearSoulstoneSockets";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection EquippedHeroSubGearRuneSockets(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_EquippedHeroSubGearRuneSockets";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_UpdateHero_FreeSweepCount(Guid heroId, DateTime freeSweepDate, int nFreeSweepCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_FreeSweepCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@freeSweepDate", SqlDbType.DateTime).Value = freeSweepDate;
		sc.Parameters.Add("@nFreeSweepCount", SqlDbType.Int).Value = nFreeSweepCount;
		return sc;
	}

	public static DataRowCollection ExpDungeonClearDifficulties(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ExpDungeonClearDifficulties";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddExpDungeonClear(Guid heroId, int nDifficulty)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddExpDungeonClear";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDifficulty", SqlDbType.Int).Value = nDifficulty;
		return sc;
	}

	public static DataRow ExpDungeonEnterCountOfDate(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ExpDungeonEnterCountOfDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddOrUpdateExpDungeonPlay(Guid heroId, DateTime date, int nCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateExpDungeonPlay";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		return sc;
	}

	public static DataRowCollection GoldDungeonClearDifficulties(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GoldDungeonClearDifficulties";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddGoldDungeonClear(Guid heroId, int nDifficulty)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGoldDungeonClear";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDifficulty", SqlDbType.Int).Value = nDifficulty;
		return sc;
	}

	public static DataRow GoldDungeonEnterCountOfDate(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GoldDungeonEnterCountOfDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddOrUpdateGoldDungeonPlay(Guid heroId, DateTime date, int nCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateGoldDungeonPlay";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		return sc;
	}

	public static DataRow TreatOfFarmQuest(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TreatOfFarmQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddTreatOfFarmQuest(Guid instanceId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddTreatOfFarmQuest";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateTreatOfFarmQuest_Complete(Guid instanceId, DateTimeOffset completionTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateTreatOfFarmQuest_Complete";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@completionTime", SqlDbType.DateTimeOffset).Value = completionTime;
		return sc;
	}

	public static DataRowCollection TreatOfFarmQuestMissions(SqlConnection conn, SqlTransaction trans, Guid instanceId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TreatOfFarmQeustMissions";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddTreatOfFarmQuestMission(Guid missionInstanceId, Guid instanceId, int nMissionId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddTreatOfFarmQuestMission";
		sc.Parameters.Add("@missionInstanceId", SqlDbType.UniqueIdentifier).Value = missionInstanceId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nMissionId", SqlDbType.Int).Value = nMissionId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateTreatOfFamrMission_MonsterSpawn(Guid missionInstanceId, DateTimeOffset monsterSpawnTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateTreatOfFarmMission_MonsterSpawn";
		sc.Parameters.Add("@missionInstanceId", SqlDbType.UniqueIdentifier).Value = missionInstanceId;
		sc.Parameters.Add("@monsterSpawnTime", SqlDbType.DateTimeOffset).Value = monsterSpawnTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateTreatOfFarmMission_Status(Guid missionInstanceId, int nStatus, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateTreatOfFarmMission_Status";
		sc.Parameters.Add("@missionInstanceId", SqlDbType.UniqueIdentifier).Value = missionInstanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static int MysteryBoxQuestStartCount(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MysteryBoxQuestStartCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRow PerformingMysteryBoxQuest(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_PerformingMysteryBoxQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddMysteryBoxQuest(Guid instanceId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddMysteryBoxQuest";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateMysteryBoxQuest_Pick(Guid instanceId, int nCount, int nGrade)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateMysteryBoxQuest_Pick";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		sc.Parameters.Add("@nGrade", SqlDbType.Int).Value = nGrade;
		return sc;
	}

	public static SqlCommand CSC_UpdateMysteryBoxQuest_Complete(Guid instanceId, DateTimeOffset completionTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateMysteryBoxQuest_Complete";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@completionTime", SqlDbType.DateTimeOffset).Value = completionTime;
		return sc;
	}

	public static int SecretLetterQuestStartCount(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SecretLetterQuestStartCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRow PerformingSecretLetterQuest(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_PerformingSecretLetterQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddSecretLetterQuest(Guid instanceId, Guid heroId, int nTargetNationId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSecretLetterQuest";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nTargetNationId", SqlDbType.Int).Value = nTargetNationId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateSecretLetterQuest_Pick(Guid instanceId, int nCount, int nGrade)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateSecretLetterQuest_Pick";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		sc.Parameters.Add("@nGrade", SqlDbType.Int).Value = nGrade;
		return sc;
	}

	public static SqlCommand CSC_UpdateSecretLetterQuest_Complete(Guid instanceId, DateTimeOffset completionTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateSecretLetterQuest_Complete";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@completionTime", SqlDbType.DateTimeOffset).Value = completionTime;
		return sc;
	}

	public static int DimensionRaidQuestStartCount(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DimensionRaidQuestStartCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRow PerformingDimensionRaidQuest(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_PerformingDimensionRaidQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddDimensionRaidQuest(Guid instanceId, Guid heroId, int nStep, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddDimensionRaidQuest";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateDimensionRaidQuest_Step(Guid instanceId, int nStep)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateDimensionRaidQuest_Step";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		return sc;
	}

	public static SqlCommand CSC_UpdateDimensionRaidQuest_Status(Guid instanceId, int nStatus, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateDimensionRaidQuest_Status";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static DataRowCollection StartedHolyWarQuests(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_StartedHolyWarQuests";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow PerformingHolyWarQuest(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_PerformingHolyWarQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddHolyWarQuest(Guid instanceId, Guid heroId, int nScheduleId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHolyWarQuest";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nScheduleId", SqlDbType.Int).Value = nScheduleId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHolyWarQuest_KillCount(Guid instanceId, int nKillCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHolyWarQuest_KillCount";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nKillCount", SqlDbType.Int).Value = nKillCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHolyWarQuest_Completion(Guid instanceId, DateTimeOffset completionTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHolyWarQuest_Completion";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@completionTime", SqlDbType.DateTimeOffset).Value = completionTime;
		return sc;
	}

	public static int BountyHunterQuest_DailyCount(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_BountyHunterQuest_DailyCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRow BountyHunterQuest_OnStart(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_BountyHunterQuest_OnStart";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddBountyHunterQuest(Guid instanceId, Guid heroId, int nQuestId, int nQuestItemGrade, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddBountyHunterQuest";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nQuestId", SqlDbType.Int).Value = nQuestId;
		sc.Parameters.Add("@nQuestItemGrade", SqlDbType.Int).Value = nQuestItemGrade;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateBountyHunterQuest_ProgressCount(Guid instanceId, int nProgressCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateBountyHunterQuest_ProgressCount";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateBountyHunterQuest_Status(Guid instanceId, int nStatus, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateBountyHunterQuest_Status";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_UseFishingQuestItem(Guid heroId, DateTime fishingQuestDate, int nFishingQuestCount, int nFishingQuestBaitItemId, int nFisihgQuestCastingCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_UseFishingQuestItem";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@fishingQuestDate", SqlDbType.DateTime).Value = fishingQuestDate;
		sc.Parameters.Add("@nFishingQuestCount", SqlDbType.Int).Value = nFishingQuestCount;
		sc.Parameters.Add("@nFishingQuestBaitItemId", SqlDbType.Int).Value = nFishingQuestBaitItemId;
		sc.Parameters.Add("@nFishingQuestCastingCount", SqlDbType.Int).Value = nFisihgQuestCastingCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_FishingQuest_Casting(Guid heroId, int nFishingQuestBaitItemId, int nFishingQuestCastingCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_FishingQuest_Casting";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nFishingQuestBaitItemId", SqlDbType.Int).Value = nFishingQuestBaitItemId;
		sc.Parameters.Add("@nFishingQuestCastingCount", SqlDbType.Int).Value = nFishingQuestCastingCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_ArtifactRoomBestFloor(Guid heroId, int nArtifactRoomBestFloor)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_ArtifactRoomBestFloor";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nArtifactRoomBestFloor", SqlDbType.Int).Value = nArtifactRoomBestFloor;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_ArtifactRoomCurrentFloor(Guid heroId, int nArtifactRoomCurrentFloor)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_ArtifactRoomCurrentFloor";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nArtifactRoomCurrentFloor", SqlDbType.Int).Value = nArtifactRoomCurrentFloor;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_ArtifactRoomInit(Guid heroId, DateTime artifactRoomInitDate, int nArtifactRoomInitCount, int nArtifactRoomCurrentFloor)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_ArtifactRoomInit";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@artifactRoomInitDate", SqlDbType.Date).Value = artifactRoomInitDate;
		sc.Parameters.Add("@nArtifactRoomInitCount", SqlDbType.Int).Value = nArtifactRoomInitCount;
		sc.Parameters.Add("@nArtifactRoomCurrentFloor", SqlDbType.Int).Value = nArtifactRoomCurrentFloor;
		return sc;
	}

	public static DataRowCollection HeroSeriesMissions(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroSeriesMissions";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateHeroSeriesMission(Guid heroId, int nMissionId, int nProgressCount, int nCurrentStep)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroSeriesMission";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMissionId", SqlDbType.Int).Value = nMissionId;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		sc.Parameters.Add("@nCurrentStep", SqlDbType.Int).Value = nCurrentStep;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroSeriesMission_Step(Guid heroId, int nMissionId, int nCurrentStep)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroSeriesMission_Step";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMissionId", SqlDbType.Int).Value = nMissionId;
		sc.Parameters.Add("@nCurrentStep", SqlDbType.Int).Value = nCurrentStep;
		return sc;
	}

	public static DataRowCollection HeroTodayMissions(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroTodayMissions";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroTodayMission(Guid heroId, DateTime date, int nMissionId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroTodayMission";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nMissionId", SqlDbType.Int).Value = nMissionId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroTodayMission_ProgressCount(Guid heroId, DateTime date, int nMissionId, int nProgressCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroTodayMission_ProgressCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nMissionId", SqlDbType.Int).Value = nMissionId;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroTodayMission_ReceivedReward(Guid heroId, DateTime date, int nMissionId, DateTimeOffset rewardReceivedTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroTodayMission_ReceivedReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nMissionId", SqlDbType.Int).Value = nMissionId;
		sc.Parameters.Add("@rewardReceivedTime", SqlDbType.DateTimeOffset).Value = rewardReceivedTime;
		return sc;
	}

	public static DataRowCollection HeroTodayTasks(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroTodayTasks";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateHeroTodayTask(Guid heroId, DateTime date, int nTaskId, int nProgressCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroTodayTask";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nTaskId", SqlDbType.Int).Value = nTaskId;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_AchievementPoint(Guid heroId, DateTime achievementPointDate, int nAchievementPoint, int nAchievementRewardNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_AchievementPoint";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@achievementPointDate", SqlDbType.Date).Value = achievementPointDate;
		sc.Parameters.Add("@nAchievementPoint", SqlDbType.Int).Value = nAchievementPoint;
		sc.Parameters.Add("@nAchievementRewardNo", SqlDbType.Int).Value = nAchievementRewardNo;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_AchievementReward(Guid heroId, int nAchievementRewardNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_AchievementReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nAchievementRewardNo", SqlDbType.Int).Value = nAchievementRewardNo;
		return sc;
	}

	public static DataRow AncientRelicEnterCountOfDate(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AncientRelicEnterCountOfDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddAncientRelicInstance(Guid instanceId, int nAverageLevel, int nStatus, int nPlayTime, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAncientRelicInstance";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nAverageLevel", SqlDbType.Int).Value = nAverageLevel;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@nPlayTime", SqlDbType.Int).Value = nPlayTime;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateAncientRelicInstance(Guid instanceId, int nStatus, int nPlayTime, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateAncientRelicInstance";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@nPlayTime", SqlDbType.Int).Value = nPlayTime;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_AddAncientRelicInstanceMember(Guid instanceId, Guid heroId, int nLevel, int nStatus, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAncientRelicInstanceMember";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateAncientRelicInstanceMember(Guid instanceId, Guid heroId, int nStatus, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateAncientRelicInstanceMember";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static int LastServerBattlePowerRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastServerBattlePowerRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection ServerBattlePowerRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ServerBattlePowerRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int LastNationBattlePowerRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastNationBattlePowerRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection NationBattlePowerRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationBattlePowerRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int LastServerLevelRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastServerLevelRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection ServerLevelRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ServerLevelRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int LastNationExploitPointRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastNationExploitPointRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection NationExploitPointRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationExploitPointRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int LastServerJobBattlePowerRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastServerJobBattlePowerRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection ServerJobBattlePowerRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ServerJobBattlePowerRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int LastDailyServerLevelRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastDailyServerLevelRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection DailyServerLevelRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DailyServerLevelRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int LastServerGuildRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastServerGuildRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection ServerGuildRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ServerGuildRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int LastNationGuildRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastNationGuildRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection NationGuildRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationGuildRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int LastServerCreatureCardRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastServerCreatureCardRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection ServerCreatureCardRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ServerCreatureCardRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int LastServerIllustratedBookRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastServerIllustratedBookRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection ServerIllustratedBookRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ServerIllustratedBookRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int LastDailyFieldOfHonorRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastDailyFieldOfHonorRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection DailyFieldOfHonorRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DailyFieldOfHonorRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow FieldOfHonorEnterCountOfDate(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonorEnterCountOfDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddOrUpdateFieldOfHonorPlay(Guid heroId, DateTime date, int nCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateFieldOfHonorPlay";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		return sc;
	}

	public static DataRowCollection FieldOfHonorHistories(SqlConnection conn, SqlTransaction trans, Guid heroId, int nRowCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGapi_FieldOfHonorHistories";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRowCount", SqlDbType.Int).Value = nRowCount;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddFieldOfHonorHistory(Guid heroId, Guid historyId, int nType, Guid targetHeroId, int nOldRanking, int nRanking, bool bIsWin, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldOfHonorHistory";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@historyId", SqlDbType.UniqueIdentifier).Value = historyId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@targetHeroId", SqlDbType.UniqueIdentifier).Value = targetHeroId;
		sc.Parameters.Add("@nOldRanking", SqlDbType.Int).Value = nOldRanking;
		sc.Parameters.Add("@nRanking", SqlDbType.Int).Value = nRanking;
		sc.Parameters.Add("@bIsWin", SqlDbType.Bit).Value = bIsWin;
		sc.Parameters.Add("regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static DataRowCollection FieldOfHonorTargets(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonorTargets";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddFieldOfHonorTarget(Guid heroId, int nRanking)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldOfHonorTarget";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRanking", SqlDbType.Int).Value = nRanking;
		return sc;
	}

	public static SqlCommand CSC_DeleteFieldOfHonorTargets(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteFieldOfHonorTargets";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection FieldOfHonorHeroes(SqlConnection conn, SqlTransaction trans, int nToRanking)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonorHeroes";
		sc.Parameters.Add("@nToRanking", SqlDbType.Int).Value = nToRanking;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateFieldOfHonorHero(Guid heroId, int nJobId, int nNationId, string sName, int nLevel, long lnBattlePower, int nRankId, int nEquippedWingId, int nWingStep, int nWingLevel, int nDisplayTitleId, Guid guildId, string sGuildName, int nGuildMemberGrade, int nMainGearEnchantLevelSetNo, int nSubGearSoulstoneLevelSetNo, int nCustomPresetHair, int nCustomFaceJawHeight, int nCustomFaceJawWidth, int nCustomFaceJawEndHeight, int nCustomFaceWidth, int nCustomFaceEyebrowHeight, int nCustomFaceEyebrowRotation, int nCustomFaceEyesWidth, int nCustomFaceNoseHeight, int nCustomFaceNoseWidth, int nCustomFaceMouthHeight, int nCustomFaceMouthWidth, int nCustomBodyHeadSize, int nCustomBodyArmsLength, int nCustomBodyArmsWidth, int nCustomBodyChestSize, int nCustomBodyWaistWidth, int nCustomBodyHipsSize, int nCustomBodyPelvisWidth, int nCustomBodyLegsLength, int nCustomBodyLegsWidth, int nCustomColorSkin, int nCustomColorEyes, int nCustomColorBeardAndEyebrow, int nCustomColorHair, int nCostumeId, int nCostumeEffectId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateFieldOfHonorHero";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nJobId", SqlDbType.Int).Value = nJobId;
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@sName", SqlDbType.VarChar).Value = sName;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@lnBattlePower", SqlDbType.BigInt).Value = lnBattlePower;
		sc.Parameters.Add("@nRankId", SqlDbType.Int).Value = nRankId;
		sc.Parameters.Add("@nEquippedWingId", SqlDbType.Int).Value = nEquippedWingId;
		sc.Parameters.Add("@nWingStep", SqlDbType.Int).Value = nWingStep;
		sc.Parameters.Add("@nWingLevel", SqlDbType.Int).Value = nWingLevel;
		sc.Parameters.Add("@nDisplayTitleId", SqlDbType.Int).Value = nDisplayTitleId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@sGuildName", SqlDbType.VarChar).Value = SFDBUtil.NullToDBNull(sGuildName);
		sc.Parameters.Add("@nGuildMemberGrade", SqlDbType.Int).Value = nGuildMemberGrade;
		sc.Parameters.Add("@nMainGearEnchantLevelSetNo", SqlDbType.Int).Value = nMainGearEnchantLevelSetNo;
		sc.Parameters.Add("@nSubGearSoulstoneLevelSetNo", SqlDbType.Int).Value = nSubGearSoulstoneLevelSetNo;
		sc.Parameters.Add("@nCustomPresetHair", SqlDbType.Int).Value = nCustomPresetHair;
		sc.Parameters.Add("@nCustomFaceJawHeight", SqlDbType.Int).Value = nCustomFaceJawHeight;
		sc.Parameters.Add("@nCustomFaceJawWidth", SqlDbType.Int).Value = nCustomFaceJawWidth;
		sc.Parameters.Add("@nCustomFaceJawEndHeight", SqlDbType.Int).Value = nCustomFaceJawEndHeight;
		sc.Parameters.Add("@nCustomFaceWidth", SqlDbType.Int).Value = nCustomFaceWidth;
		sc.Parameters.Add("@nCustomFaceEyebrowHeight", SqlDbType.Int).Value = nCustomFaceEyebrowHeight;
		sc.Parameters.Add("@nCustomFaceEyebrowRotation", SqlDbType.Int).Value = nCustomFaceEyebrowRotation;
		sc.Parameters.Add("@nCustomFaceEyesWidth", SqlDbType.Int).Value = nCustomFaceEyesWidth;
		sc.Parameters.Add("@nCustomFaceNoseHeight", SqlDbType.Int).Value = nCustomFaceNoseHeight;
		sc.Parameters.Add("@nCustomFaceNoseWidth", SqlDbType.Int).Value = nCustomFaceNoseWidth;
		sc.Parameters.Add("@nCustomFaceMouthHeight", SqlDbType.Int).Value = nCustomFaceMouthHeight;
		sc.Parameters.Add("@nCustomFaceMouthWidth", SqlDbType.Int).Value = nCustomFaceMouthWidth;
		sc.Parameters.Add("@nCustomBodyHeadSize", SqlDbType.Int).Value = nCustomBodyHeadSize;
		sc.Parameters.Add("@nCustomBodyArmsLength", SqlDbType.Int).Value = nCustomBodyArmsLength;
		sc.Parameters.Add("@nCustomBodyArmsWidth", SqlDbType.Int).Value = nCustomBodyArmsWidth;
		sc.Parameters.Add("@nCustomBodyChestSize", SqlDbType.Int).Value = nCustomBodyChestSize;
		sc.Parameters.Add("@nCustomBodyWaistWidth", SqlDbType.Int).Value = nCustomBodyWaistWidth;
		sc.Parameters.Add("@nCustomBodyHipsSize", SqlDbType.Int).Value = nCustomBodyHipsSize;
		sc.Parameters.Add("@nCustomBodyPelvisWidth", SqlDbType.Int).Value = nCustomBodyPelvisWidth;
		sc.Parameters.Add("@nCustomBodyLegsLength", SqlDbType.Int).Value = nCustomBodyLegsLength;
		sc.Parameters.Add("@nCustomBodyLegsWidth", SqlDbType.Int).Value = nCustomBodyLegsWidth;
		sc.Parameters.Add("@nCustomColorSkin", SqlDbType.Int).Value = nCustomColorSkin;
		sc.Parameters.Add("@nCustomColorEyes", SqlDbType.Int).Value = nCustomColorEyes;
		sc.Parameters.Add("@nCustomColorBeardAndEyebrow", SqlDbType.Int).Value = nCustomColorBeardAndEyebrow;
		sc.Parameters.Add("@nCustomColorHair", SqlDbType.Int).Value = nCustomColorHair;
		sc.Parameters.Add("@nCostumeId", SqlDbType.Int).Value = nCostumeId;
		sc.Parameters.Add("@nCostumeEffectId", SqlDbType.Int).Value = nCostumeEffectId;
		return sc;
	}

	public static DataRowCollection FieldOfHonorHeroSkills(SqlConnection conn, SqlTransaction trans, int nToRanking)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonorHeroSkills";
		sc.Parameters.Add("@nToRanking", SqlDbType.Int).Value = nToRanking;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroSkill(Guid heroId, int nSkillId, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldOfHonorHeroSkill";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSkillId", SqlDbType.Int).Value = nSkillId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static SqlCommand CSC_DeleteFieldOfHonorHeroSkills(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteFieldOfHonorHeroSkills";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection FieldOfHonorHeroWings(SqlConnection conn, SqlTransaction trans, int nToRanking)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonorHeroWings";
		sc.Parameters.Add("@nToRanking", SqlDbType.Int).Value = nToRanking;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroWing(Guid heroId, int nWingId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldOfHonorHeroWing";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nWingId", SqlDbType.Int).Value = nWingId;
		return sc;
	}

	public static SqlCommand CSC_DeleteFieldOfHonorHeroWings(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteFieldOfHonorHeroWings";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection FieldOfHonorHeroWingEnchants(SqlConnection conn, SqlTransaction trans, int nToRanking)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonorHeroWingEnchants";
		sc.Parameters.Add("@nToRanking", SqlDbType.Int).Value = nToRanking;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroWingEnchant(Guid heroId, int nPartId, int nStep, int nLevel, int nEnchantCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldOfHonorHeroWingEnchant";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nPartId", SqlDbType.Int).Value = nPartId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nEnchantCount", SqlDbType.Int).Value = nEnchantCount;
		return sc;
	}

	public static SqlCommand CSC_DeleteFieldOfHonorHeroWingEnchants(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteFieldOfHonorHeroWingEnchants";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection FieldOfHonorHeroEquippedMainGears(SqlConnection conn, SqlTransaction trans, int nToRanking)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonorHeroEquippedMainGears";
		sc.Parameters.Add("@nToRanking", SqlDbType.Int).Value = nToRanking;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroEquippedMainGear(Guid heroMainGearId, Guid heroId, int nMainGearId, int nEnchantLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldOfHonorHeroEquippedMainGear";
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMainGearId", SqlDbType.Int).Value = nMainGearId;
		sc.Parameters.Add("@nEnchantLevel", SqlDbType.Int).Value = nEnchantLevel;
		return sc;
	}

	public static SqlCommand CSC_DeleteFieldOfHonorHeroEquippedMainGears(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteFieldOfHonorHeroEquippedMainGears";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection FieldOfHonorHeroMainGearOptionAttrs(SqlConnection conn, SqlTransaction trans, int nToRanking)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonorHeroMainGearOptionAttrs";
		sc.Parameters.Add("@nToRanking", SqlDbType.Int).Value = nToRanking;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroMainGearOptionAttr(Guid heroMainGearId, int nIndex, int nGrade, int nAttrId, long lnAttrValueId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldOfHonorHeroMainGearOptionAttr";
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@nIndex", SqlDbType.Int).Value = nIndex;
		sc.Parameters.Add("@nGrade", SqlDbType.Int).Value = nGrade;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		sc.Parameters.Add("@lnAttrValueId", SqlDbType.BigInt).Value = lnAttrValueId;
		return sc;
	}

	public static SqlCommand CSC_DeleteFieldOfHonorHeroMainGearOptionAttrs(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteFieldOfHonorHeroMainGearOptionAttrs";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection FieldOfHonorHeroEquippedSubGears(SqlConnection conn, SqlTransaction trans, int nToRanking)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonorHeroEquippedSubGears";
		sc.Parameters.Add("@nToRanking", SqlDbType.Int).Value = nToRanking;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroEquippedSubGear(Guid heroId, int nSubGearId, int nLevel, int nQuality)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldOfHonorHeroEquippedSubGear";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nQuality", SqlDbType.Int).Value = nQuality;
		return sc;
	}

	public static SqlCommand CSC_DeleteFieldOfHonorHeroEquippedSubGears(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteFieldOfHonorHeroEquippedSubGears";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection FieldOfHonorHeroSubGearRuneSockets(SqlConnection conn, SqlTransaction trans, int nToRanking)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonorHeroSubGearRuneSockets";
		sc.Parameters.Add("@nToRanking", SqlDbType.Int).Value = nToRanking;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroSubGearRuneSocket(Guid heroId, int nSubGearId, int nSocketIndex, int nItemId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldOfHonorHeroSubGearRuneSocket";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nSocketIndex", SqlDbType.Int).Value = nSocketIndex;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		return sc;
	}

	public static SqlCommand CSC_DeleteFieldOfHonorHeroSubGearRuneSockets(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteFieldOfHonorHeroSubGearRuneSockets";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection FieldOfHonorHeroSubGearSoulstoneSockets(SqlConnection conn, SqlTransaction trans, int nToRanking)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonorHeroSubGearSoulstoneSockets";
		sc.Parameters.Add("@nToRanking", SqlDbType.Int).Value = nToRanking;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroSubGearSoulstoneSocket(Guid heroId, int nSubGearId, int nSocketIndex, int nItemId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldOfHonorHeroSubGearSoulstoneSocket";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nSocketIndex", SqlDbType.Int).Value = nSocketIndex;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		return sc;
	}

	public static SqlCommand CSC_DeleteFieldOfHonorHeroSubGearSoulstoneSockets(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteFieldOfHonorHeroSubGearSoulstoneSockets";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection FieldOfHonorHeroRealAttrs(SqlConnection conn, SqlTransaction trans, int nToRanking)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonorHeroRealAttrs";
		sc.Parameters.Add("@nToRanking", SqlDbType.Int).Value = nToRanking;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroRealAttr(Guid heroId, int nAttrId, int nValue)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFieldOfHonorHeroRealAttr";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		sc.Parameters.Add("@nValue", SqlDbType.Int).Value = nValue;
		return sc;
	}

	public static SqlCommand CSC_DeleteFieldOfHonorHeroRealAttrs(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteFieldOfHonorHeroRealAttrs";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection Guilds(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Guilds";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddGuild(Guid guildId, string sName, int nNationId, int nFoodWarehouseLevel, int nFoodWarehouseExp, DateTime dailyObjectiveDate, int nDailyObjectiveContentId, DateTime weeklyObjectiveDate, int nWeeklyObjectiveId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuild";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@sName", SqlDbType.NVarChar).Value = sName;
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@nFoodWarehouseLevel", SqlDbType.Int).Value = nFoodWarehouseLevel;
		sc.Parameters.Add("@nFoodWarehouseExp", SqlDbType.Int).Value = nFoodWarehouseExp;
		sc.Parameters.Add("@dailyObjectiveDate", SqlDbType.DateTime).Value = dailyObjectiveDate;
		sc.Parameters.Add("@nDailyObjectiveContentId", SqlDbType.Int).Value = nDailyObjectiveContentId;
		sc.Parameters.Add("@weeklyObjectiveDate", SqlDbType.DateTime).Value = weeklyObjectiveDate;
		sc.Parameters.Add("@nWeeklyObjectiveId", SqlDbType.Int).Value = nWeeklyObjectiveId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_SetGuild(Guid heroId, Guid guildId, int nGuildMemberGrade)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_SetGuild";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@nGuildMemberGrade", SqlDbType.Int).Value = nGuildMemberGrade;
		return sc;
	}

	public static DataRowCollection GuildMembers(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildMembers";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddGuildApplication(Guid applicationId, Guid guildId, Guid heroId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildApplication";
		sc.Parameters.Add("@applicationId", SqlDbType.UniqueIdentifier).Value = applicationId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static DataRowCollection GuildApplications(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildApplications";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int GuildApplicationCountOfDate(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildApplicationCountOfDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static SqlCommand CSC_UpdateGuildApplication(Guid applicationId, int nStatus)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGuildApplication";
		sc.Parameters.Add("@applicationId", SqlDbType.UniqueIdentifier).Value = applicationId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_ExitGuild(Guid heroId, DateTimeOffset guildWithdrawalTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_ExitGuild";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildWithdrawalTime", SqlDbType.DateTimeOffset).Value = guildWithdrawalTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateGuild_MemberBanishmentDateCount(Guid guildId, DateTime memberBanishmentDate, int nMemberBanishmentCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGuild_MemberBanishmentDateCount";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@memberBanishmentDate", SqlDbType.Date).Value = memberBanishmentDate;
		sc.Parameters.Add("@nMemberBanishmentCount", SqlDbType.Int).Value = nMemberBanishmentCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_GuildMemberGrade(Guid heroId, int nGuildMemberGrade)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_GuildMemberGrade";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nGuildMemberGrade", SqlDbType.Int).Value = nGuildMemberGrade;
		return sc;
	}

	public static SqlCommand CSC_UpdateGuild_Notice(Guid guildId, string sNotice)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGuild_Notice";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@sNotice", SqlDbType.NVarChar).Value = ((sNotice != null) ? sNotice : string.Empty);
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_GuildDonationDateCount(Guid heroId, DateTime guildDonationDate, int nGuildDonationCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_GuildDonationDateCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildDonationDate", SqlDbType.Date).Value = guildDonationDate;
		sc.Parameters.Add("@nGuildDonationCount", SqlDbType.Int).Value = nGuildDonationCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_GuildContributionPoint(Guid heroId, int nGuildTotalContributionPoint, int nGuildContributionPoint)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_GuildContributionPoint";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nGuildTotalContributionPoint", SqlDbType.Int).Value = nGuildTotalContributionPoint;
		sc.Parameters.Add("@nGuildContributionPoint", SqlDbType.Int).Value = nGuildContributionPoint;
		return sc;
	}

	public static SqlCommand CSC_UpdateGuild_Fund(Guid guildId, long lnFund)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGuild_Fund";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@lnFund", SqlDbType.BigInt).Value = lnFund;
		return sc;
	}

	public static SqlCommand CSC_UpdateGuild_BuildingPoint(Guid guildId, int nBuildingPoint)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGuild_BuildingPoint";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@nBuildingPoint", SqlDbType.Int).Value = nBuildingPoint;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_GuildDailyRewardReceivedDate(Guid heroId, DateTime guildDailyRewardReceivedDate)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_GuildDailyRewardReceivedDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildDailyRewardReceivedDate", SqlDbType.DateTime).Value = guildDailyRewardReceivedDate;
		return sc;
	}

	public static SqlCommand CSC_UpdateGuild_BlessingBuff(Guid guildId, DateTimeOffset blessingBuffStartTime, int nBlessBuffId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGuild_BlessingBuff";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@blessingBuffStartTime", SqlDbType.DateTimeOffset).Value = blessingBuffStartTime;
		sc.Parameters.Add("@nBlessingBuffId", SqlDbType.Int).Value = nBlessBuffId;
		return sc;
	}

	public static DataRowCollection GuildBuildings(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildBuildings";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddGuildBuilding(Guid guildId, int nBuildingId, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildBuilding";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@nBuildingId", SqlDbType.Int).Value = nBuildingId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static SqlCommand CSC_UpdateGuildBuilding_Level(Guid guildId, int nBuildingId, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGuildBuilding_Level";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@nBuildingId", SqlDbType.Int).Value = nBuildingId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static int SupplySupportQuestCountOfDate(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SupplySupportQuestCountOfDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRow SupplySupportQuest_OnStart(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SupplySupportQuest_OnStart";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddSupplySupportQuest(Guid instanceId, Guid heroId, int nCartId, int nCartHp, bool bIsCartRiding, int nCartContinentId, float fCartXPosition, float fCartYPosition, float fCartZPosition, float fCartYRotation, int nOrderId, int nStatus, DateTimeOffset regTime, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSupplySupportQuest";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCartId", SqlDbType.Int).Value = nCartId;
		sc.Parameters.Add("@nCartHp", SqlDbType.Int).Value = nCartHp;
		sc.Parameters.Add("@bIsCartRiding", SqlDbType.Bit).Value = bIsCartRiding;
		sc.Parameters.Add("@nCartContinentId", SqlDbType.Int).Value = nCartContinentId;
		sc.Parameters.Add("@fCartXPosition", SqlDbType.Float).Value = fCartXPosition;
		sc.Parameters.Add("@fCartYPosition", SqlDbType.Float).Value = fCartYPosition;
		sc.Parameters.Add("@fCartZPosition", SqlDbType.Float).Value = fCartZPosition;
		sc.Parameters.Add("@fCartYRotation", SqlDbType.Float).Value = fCartYRotation;
		sc.Parameters.Add("@nOrderId", SqlDbType.Int).Value = nOrderId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateSupplySupportQuest_Status(Guid instanceId, int nStatus, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateSupplySupportQuest_Status";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateSupplySupportQuest_WayPoint(Guid instanceId, int nWayPoint)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateSupplySupportQuest_WayPoint";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nWayPoint", SqlDbType.Int).Value = nWayPoint;
		return sc;
	}

	public static SqlCommand CSC_UpdateSupplySupportQuest_Cart(Guid instanceId, int nCartId, int nCartHp, bool bIsCartRiding, int nCartContinentId, float fCartXPosition, float fCartYPosition, float fCartZPosition, float fCartYRotation)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateSupplySupportQuest_Cart";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nCartId", SqlDbType.Int).Value = nCartId;
		sc.Parameters.Add("@nCartHp", SqlDbType.Int).Value = nCartHp;
		sc.Parameters.Add("@bIsCartRiding", SqlDbType.Bit).Value = bIsCartRiding;
		sc.Parameters.Add("@nCartContinentId", SqlDbType.Int).Value = nCartContinentId;
		sc.Parameters.Add("@fCartXPosition", SqlDbType.Float).Value = fCartXPosition;
		sc.Parameters.Add("@fCartYPosition", SqlDbType.Float).Value = fCartYPosition;
		sc.Parameters.Add("@fCartZPosition", SqlDbType.Float).Value = fCartZPosition;
		sc.Parameters.Add("@fCartYRotation", SqlDbType.Float).Value = fCartYRotation;
		return sc;
	}

	public static DataRowCollection SupplySupportQuestVisitedWayPoints(SqlConnection conn, SqlTransaction trans, Guid instanceId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SupplySupportQuestVisitedWayPoints";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddSupplySupportQuestVisitedWayPoint(Guid instanceId, int nWayPoint)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSupplySupportQuestVisitedWayPoint";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nWayPoint", SqlDbType.Int).Value = nWayPoint;
		return sc;
	}

	public static DataRow PerformingHeroGuildFarmQuest(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_PerformingHeroGuildFarmQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddHeroGuildFarmQuest(Guid instanceId, Guid heroId, Guid guildId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroGuildFarmQuest";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroGuildFarmQuest_ObjectiveCompletion(Guid instanceId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroGuildFarmQuest_ObjectiveCompletion";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroGuildFarmQuest_Status(Guid instanceId, int nStatus, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroGuildFarmQuest_Status";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroGuildFarmQuest_PerformingQuestFail(Guid heroId, Guid guildId, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroGuildFarmQuest_PerformingQuestFail";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static int HeroGuildFarmQuestCountOfDate(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroGuildFarmQuestCountOfDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static SqlCommand CSC_UpdateHero_GuildFoodWarehouseStockCount(Guid heroId, DateTime guildFoodWarehouseStockDate, int nGuildFoodWarehouseStockCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_GuildFoodWarehouseStockCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildFoodWarehouseStockDate", SqlDbType.Date).Value = guildFoodWarehouseStockDate;
		sc.Parameters.Add("@nGuildFoodWarehouseStockCount", SqlDbType.Int).Value = nGuildFoodWarehouseStockCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_GuildFoodWarehouseReward(Guid heroId, Guid receivedGuildFoodWarehouseCollectionId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_GuildFoodWarehouseReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@receivedGuildFoodWarehouseCollectionId", SqlDbType.UniqueIdentifier).Value = receivedGuildFoodWarehouseCollectionId;
		return sc;
	}

	public static SqlCommand CSC_UpdateGuild_FoodWarehouse(Guid guildId, int nFoodWarehouseLevel, int nFoodWarehouseExp, Guid foodWarehouseCollectionId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGuild_FoodWarehouse";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@nFoodWarehouseLevel", SqlDbType.Int).Value = nFoodWarehouseLevel;
		sc.Parameters.Add("@nFoodWarehouseExp", SqlDbType.Int).Value = nFoodWarehouseExp;
		sc.Parameters.Add("@foodWarehouseCollectionId", SqlDbType.UniqueIdentifier).Value = foodWarehouseCollectionId;
		return sc;
	}

	public static DataRowCollection GuildSkillLevels(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildSkillLevels";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateGuildSkillLevel(Guid heroId, int nGuildSkillId, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateGuildSkillLevel";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nGuildSkillId", SqlDbType.Int).Value = nGuildSkillId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static DataRow HeroGuildMissionQuest(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroGuildMissionQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddHeroGuildMissionQuest(Guid instanceId, Guid heroId, Guid guildId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroGuildMissionQuest";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroGuildMissionQuest_Complete(Guid instanceId, DateTimeOffset completionTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroGuildMissionQuest_Complete";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@completionTime", SqlDbType.DateTimeOffset).Value = completionTime;
		return sc;
	}

	public static DataRowCollection HeroGuildMissionQuestMissions(SqlConnection conn, SqlTransaction trans, Guid instanceId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroGuildMissionQuestMissions";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroGuildMissionQuestMission(Guid missionInstanceId, Guid instanceId, Guid guildId, int nMissionId, int nSpawnedMonsterContinentId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroGuildMissionQuestMission";
		sc.Parameters.Add("@missionInstanceId", SqlDbType.UniqueIdentifier).Value = missionInstanceId;
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@nMissionId", SqlDbType.Int).Value = nMissionId;
		sc.Parameters.Add("@nSpawnedMonsterContinentId", SqlDbType.Int).Value = nSpawnedMonsterContinentId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroGuildMissionQuestMission_ProgressCount(Guid missionInstanceId, int nProgressCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroGuildMissionQuestMission_ProgressCount";
		sc.Parameters.Add("@missionInstanceId", SqlDbType.UniqueIdentifier).Value = missionInstanceId;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroGuildMissionQuestMission_Status(Guid missionInstanceId, int nStatus, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroGuildMissionQuestMission_Status";
		sc.Parameters.Add("@missionInstanceId", SqlDbType.UniqueIdentifier).Value = missionInstanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateGuild_MoralPoint(Guid guildId, DateTime moralPointDate, int nMoralPoint)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGuild_MoralPoint";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@moralPointDate", SqlDbType.Date).Value = moralPointDate;
		sc.Parameters.Add("@nMoralPoint", SqlDbType.Int).Value = nMoralPoint;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_GuildMoralPoint(Guid heroId, DateTime guildMoralPointDate, int nGuildMoralPoint)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_GuildMoralPoint";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildMoralPointDate", SqlDbType.Date).Value = guildMoralPointDate;
		sc.Parameters.Add("@nGuildMoralPoint", SqlDbType.Int).Value = nGuildMoralPoint;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_GuildAltarDefenseStartTime(Guid heroId, DateTimeOffset guildAltarDefenseStartTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_GuildAltarDefenseStartTime";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildAltarDefenseStartTime", SqlDbType.DateTimeOffset).Value = guildAltarDefenseStartTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_GuildAltarReward(Guid heroId, DateTime guildAltarRewardReceivedDate)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_GuildAltarReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildAltarRewardReceivedDate", SqlDbType.Date).Value = guildAltarRewardReceivedDate;
		return sc;
	}

	public static SqlCommand CSC_UpdateNationIncumbentNoblesse(int nNationId, int nNoblesseId, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateNationIncumbentNoblesse";
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@nNoblesseId", SqlDbType.Int).Value = nNoblesseId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRow System(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_System";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static DataRow NationInstance(SqlConnection conn, SqlTransaction trans, int nNationId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationInstance";
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_UpdateNationInstance_Fund(int nNationId, long lnFund)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateNationInstance_Fund";
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@lnFund", SqlDbType.BigInt).Value = lnFund;
		return sc;
	}

	public static DataRowCollection NationIncumbentNoblesses(SqlConnection conn, SqlTransaction trans, int nNationId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationIncumbentNoblesses";
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddNationIncumbentNoblesse(int nNationId, int nNoblesseId, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationIncumbentNoblesse";
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@nNoblesseId", SqlDbType.Int).Value = nNoblesseId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static SqlCommand CSC_DeleteNationIncumbentNoblesse(int nNationId, int nNoblesseId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteNationIncumbentNoblesse";
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@nNoblesseId", SqlDbType.Int).Value = nNoblesseId;
		return sc;
	}

	public static DataRowCollection NationNoblesseAppointment(SqlConnection conn, SqlTransaction trans, int nNationId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationNoblesseAppointments";
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateNationNoblesseAppointment(int nNationId, int nNoblesseId, DateTimeOffset appointmentTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateNationNoblesseAppointment";
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@nNoblesseId", SqlDbType.Int).Value = nNoblesseId;
		sc.Parameters.Add("@appointmentTime", SqlDbType.DateTimeOffset).Value = appointmentTime;
		return sc;
	}

	public static DataRowCollection HeroesOfNationByName(SqlConnection conn, SqlTransaction trans, int nNationId, string sName)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroesOfNationByName";
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@sName", SqlDbType.NVarChar).Value = sName;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationWarDeclaration_ByPast(SqlConnection conn, SqlTransaction trans, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWarDeclaration_ByPast";
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationWarDeclaration_OnDeclare(SqlConnection conn, SqlTransaction trans, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWarDeclaration_OnDeclare";
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddNationWarDeclaration(Guid declarationId, int nNationId, int nTargetNationId, int nStatus, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationWarDeclaration";
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = declarationId;
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@nTargetNationId", SqlDbType.Int).Value = nTargetNationId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateNationWarDeclaration(Guid declarationId, int nStatus, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateNationWarDeclaration";
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = declarationId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static DataRow NationWarDeclarationCountOfWeekly(SqlConnection conn, SqlTransaction trans, int nNationId, DateTime dateOfMonday)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWarDeclarationCountOfWeekly";
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@dateOfMonday", SqlDbType.Date).Value = dateOfMonday;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_UpdateHero_NationWarFreeTransmissionDateCount(Guid heroId, DateTime nationWarFreeTransmissionDate, int nNationWarFreeTransmissionCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_NationWarFreeTransmissionDateCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nationWarFreeTransmissionDate", SqlDbType.Date).Value = nationWarFreeTransmissionDate;
		sc.Parameters.Add("@nNationWarFreeTransmissionCount", SqlDbType.Int).Value = nNationWarFreeTransmissionCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_NationWarPaidTransmissionDateCount(Guid heroId, DateTime nationWarPaidTransmissionDate, int nNationWarPaidTransmissionCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_NationWarPaidTransmissionDateCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nationWarPaidTransmissionDate", SqlDbType.Date).Value = nationWarPaidTransmissionDate;
		sc.Parameters.Add("@nNationWarPaidTransmissionCount", SqlDbType.Int).Value = nNationWarPaidTransmissionCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateNationInstance_NationWarCallDateCount(int nNationId, DateTime nationWarCallDate, int nNationWarCallCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateNationInstance_NationWarCallDateCount";
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@nationWarCallDate", SqlDbType.Date).Value = nationWarCallDate;
		sc.Parameters.Add("@nNationWarCallCount", SqlDbType.Int).Value = nNationWarCallCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateNationInstance_ConvergingAttackDateCount(int nNationId, DateTime convergingAttackDate, int nConvergingAttackCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateNationInstance_ConvergingAttackDateCount";
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@convergingAttackDate", SqlDbType.Date).Value = convergingAttackDate;
		sc.Parameters.Add("@nConvergingAttackCount", SqlDbType.Int).Value = nConvergingAttackCount;
		return sc;
	}

	public static SqlCommand CSC_AddNationWarMember(Guid declarationId, Guid heroId, int nKillCount, int nAssistCount, int nDeadCount, int nImmediateRevivalCount, DateTimeOffset regTime, bool bRewarded)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationWarMember";
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = declarationId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nKillCount", SqlDbType.Int).Value = nKillCount;
		sc.Parameters.Add("@nAssistCount", SqlDbType.Int).Value = nAssistCount;
		sc.Parameters.Add("@nDeadCount", SqlDbType.Int).Value = nDeadCount;
		sc.Parameters.Add("@nImmediateRevivalCount", SqlDbType.Int).Value = nImmediateRevivalCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		sc.Parameters.Add("@bRewarded", SqlDbType.Bit).Value = bRewarded;
		return sc;
	}

	public static DataRow NationWarMember(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWarMember";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_UpdateNationWarMember_KillCount(Guid declarationId, Guid heroId, int nKillCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateNationWarMember_KillCount";
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = declarationId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nKillCount", SqlDbType.Int).Value = nKillCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateNationWarMember_AssistCount(Guid declarationId, Guid heroId, int nAssistCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateNationWarMember_AssistCount";
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = declarationId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nAssistCount", SqlDbType.Int).Value = nAssistCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateNationWarMember_DeadCount(Guid declarationId, Guid heroId, int nDeadCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateNationWarMember_DeadCount";
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = declarationId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDeadCount", SqlDbType.Int).Value = nDeadCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateNationWarMember_ImmediateRevivalCount(Guid declarationId, Guid heroId, int nImmediateRevivalCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateNationWarMember_ImmediateRevivalCount";
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = declarationId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nImmediateRevivalCount", SqlDbType.Int).Value = nImmediateRevivalCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateNationWarMember_Rewarded(Guid declarationId, Guid heroId, bool bRewarded)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateNationWarMember_Rewarded";
		sc.Parameters.Add("@declarationId", SqlDbType.UniqueIdentifier).Value = declarationId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@bRewarded", SqlDbType.Int).Value = bRewarded;
		return sc;
	}

	public static int NationHeroCount(SqlConnection conn, SqlTransaction trans, int nNationId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationHeroCount";
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static SqlCommand CSC_UpdateNationInstance_NationWarPoint(int nNationId, int nNationWarPoint)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateNationInstance_NationWarPoint";
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@nNationWarPoint", SqlDbType.Int).Value = nNationWarPoint;
		return sc;
	}

	public static SqlCommand CSC_UpdateNationInstance_Alliance(int nNationId, Guid allianceId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateNationInstance_Alliance";
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@allianceId", SqlDbType.UniqueIdentifier).Value = allianceId;
		return sc;
	}

	public static DataRowCollection NationAllianceApplications(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationAllianceApplications";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddNationAllianceApplication(Guid applicationId, int nNationId, int nTargetNationId, long lnFund, int nStatus, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddNationAllianceApplication";
		sc.Parameters.Add("@applicationId", SqlDbType.UniqueIdentifier).Value = applicationId;
		sc.Parameters.Add("@nNationId", SqlDbType.Int).Value = nNationId;
		sc.Parameters.Add("@nTargetNationId", SqlDbType.Int).Value = nTargetNationId;
		sc.Parameters.Add("@lnFund", SqlDbType.BigInt).Value = lnFund;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateNationAllianceApplication(Guid applicationId, int nStatus, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateNationAllianceApplication";
		sc.Parameters.Add("@applicationId", SqlDbType.UniqueIdentifier).Value = applicationId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static int LastServerNationPowerRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastServerNationPowerRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection ServerNationPowerRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ServerNationPowerRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int GuildSupplySupportQuestCountOfDate(SqlConnection conn, SqlTransaction trans, Guid guildId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildSupplySupportQuestCountOfDate";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRow GuildSupplySupportQuest_OnAccept(SqlConnection conn, SqlTransaction trans, Guid guildId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildSupplySupportQuest_OnAccept";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddGuildSupplySupportQuest(Guid instanceId, Guid guildId, Guid heroId, int nCartId, int nCartHp, bool bIsCartRiding, int nCartContinentId, float fCartXPosition, float fCartYPosition, float fCartZPosition, float fCartYRotation, int nStatus, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildSupplySupportQuest";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCartId", SqlDbType.Int).Value = nCartId;
		sc.Parameters.Add("@nCartHp", SqlDbType.Int).Value = nCartHp;
		sc.Parameters.Add("@bIsCartRiding", SqlDbType.Bit).Value = bIsCartRiding;
		sc.Parameters.Add("@nCartContinentId", SqlDbType.Int).Value = nCartContinentId;
		sc.Parameters.Add("@fCartXPosition", SqlDbType.Float).Value = fCartXPosition;
		sc.Parameters.Add("@fCartYPosition", SqlDbType.Float).Value = fCartYPosition;
		sc.Parameters.Add("@fCartZPosition", SqlDbType.Float).Value = fCartZPosition;
		sc.Parameters.Add("@fCartYRotation", SqlDbType.Float).Value = fCartYRotation;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateGuildSupplySupportQuest_Cart(Guid instanceId, int nCartId, int nCartHp, bool bIsCartRiding, int nCartContinentId, float fCartXPosition, float fCartYPosition, float fCartZPosition, float fCartYRotation)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGuildSupplySupportQuest_Cart";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nCartId", SqlDbType.Int).Value = nCartId;
		sc.Parameters.Add("@nCartHp", SqlDbType.Int).Value = nCartHp;
		sc.Parameters.Add("@bIsCartRiding", SqlDbType.Bit).Value = bIsCartRiding;
		sc.Parameters.Add("@nCartContinentId", SqlDbType.Int).Value = nCartContinentId;
		sc.Parameters.Add("@fCartXPosition", SqlDbType.Float).Value = fCartXPosition;
		sc.Parameters.Add("@fCartYPosition", SqlDbType.Float).Value = fCartYPosition;
		sc.Parameters.Add("@fCartZPosition", SqlDbType.Float).Value = fCartZPosition;
		sc.Parameters.Add("@fCartYRotation", SqlDbType.Float).Value = fCartYRotation;
		return sc;
	}

	public static SqlCommand CSC_UpdateGuildSupplySupportQuest_Status(Guid instanceId, int nStatus, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGuildSupplySupportQuest_Status";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static DataRow HeroGuildHuntingQuest(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroGuildHuntingQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static int HeroGuildHuntingQuest_DateCount(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroGuildHuntingQeust_DateCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static SqlCommand CSC_AddHeroGuildHuntingQuest(Guid questInstanceId, Guid guildId, Guid heroId, int nObjectiveId, DateTimeOffset regTime, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroGuildHuntingQuest";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nObjectiveId", SqlDbType.Int).Value = nObjectiveId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroGuildHuntingQuest_ProgressCount(Guid questInstanceId, int nProgressCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroGuildHuntingQuest_ProgressCount";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroGuildHuntingQuest_Status(Guid questInstanceId, int nStatus, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroGuildHuntingQuest_Status";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static DataRow SoulCoveterEnterCountOfWeekly(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime dateOfMonday)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SoulCoveterEnterCountOfWeekly";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@dateOfMonday", SqlDbType.Date).Value = dateOfMonday;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddSoulCoveterInstance(Guid instanceId, int nDifficulty, int nStatus, int nPlayTime, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSoulCoveterInstance";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nDifficulty", SqlDbType.Int).Value = nDifficulty;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@nPlayTime", SqlDbType.Int).Value = nPlayTime;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateSoulCoveterInstance(Guid instanceId, int nStatus, int nPlayTime, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateSoulCoveterInstance";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@nPlayTime", SqlDbType.Int).Value = nPlayTime;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_AddSoulCoveterInstanceMember(Guid instanceId, Guid heroId, int nStatus, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddSoulCoveterInstanceMember";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateSoulCoveterInstanceMember(Guid instanceId, Guid heroId, int nStatus, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateSoulCoveterInstanceMember";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroGuildHuntingQuest_PerformingQuestFail(Guid heroId, Guid guildId, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroGuildHuntingQuest_PerformingQuestFail";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_HuntingDonationDate(Guid heroId, DateTime guildHuntingDonationDate)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_HuntingDonationDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildHuntingDonationDate", SqlDbType.Date).Value = guildHuntingDonationDate;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_HuntingDonationCompletionRewardRecivedDate(Guid heroId, DateTime guildHuntingDonationCompletionRewardReceivedDate)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_HuntingDonationCompletionRewardReceivedDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildHuntingDonationCompletionRewardReceivedDate", SqlDbType.Date).Value = guildHuntingDonationCompletionRewardReceivedDate;
		return sc;
	}

	public static SqlCommand CSC_UpdateGuild_HuntingDonationDateCount(Guid guildId, DateTime huntingDonationDate, int nHuntingdonationCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGuild_HuntingDonationDateCount";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@huntingDonationDate", SqlDbType.Date).Value = huntingDonationDate;
		sc.Parameters.Add("@nHuntingdonationCount", SqlDbType.Int).Value = nHuntingdonationCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_GuildWeeklyObjectiveRewardReceivedDate(Guid heroId, DateTime guildWeeklyObjectiveRewardReceivedDate)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_GuildWeeklyObjectiveRewardReceivedDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildWeeklyObjectiveRewardReceivedDate", SqlDbType.Date).Value = guildWeeklyObjectiveRewardReceivedDate;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_GuildDailyObjectiveReward(Guid heroId, DateTime guildDailyObjectiveRewardReceivedDate, int nGuildDailyObjectiveRewardReceivedNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_GuildDailyObjectiveReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@guildDailyObjectiveRewardReceivedDate", SqlDbType.Date).Value = guildDailyObjectiveRewardReceivedDate;
		sc.Parameters.Add("@nGuildDailyObjectiveRewardReceivedNo", SqlDbType.Int).Value = nGuildDailyObjectiveRewardReceivedNo;
		return sc;
	}

	public static SqlCommand CSC_UpdateGuild_DailyObjective(Guid guildId, DateTime dailyObjectiveDate, int nDailyObjectiveContentId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGuild_DailyObjective";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@dailyObjectiveDate", SqlDbType.Date).Value = dailyObjectiveDate;
		sc.Parameters.Add("@nDailyObjectiveContentId", SqlDbType.Int).Value = nDailyObjectiveContentId;
		return sc;
	}

	public static DataRowCollection GuildDailyObjectiveComletions(SqlConnection conn, SqlTransaction trans, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildDailyObjectiveComletions";
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddGuildDailyObjectiveCompletion(Guid guildId, DateTime date, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildDailyObjectiveCompletion";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@date", SqlDbType.DateTime).Value = date;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection GuildDailyObjectiveCompletions_WeeklyCompletionCount(SqlConnection conn, SqlTransaction trans, DateTime startDate)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildDailyObjectiveCompletions_WeeklyCompletionCount";
		sc.Parameters.Add("@startDate", SqlDbType.Date).Value = startDate;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_UpdateGuild_WeeklyObjective(Guid guildId, DateTime weeklyObjectiveDate, int nWeeklyObjectiveId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGuild_WeeklyObjective";
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@weeklyObjectiveDate", SqlDbType.Date).Value = weeklyObjectiveDate;
		sc.Parameters.Add("@nWeeklyObjectiveId", SqlDbType.Int).Value = nWeeklyObjectiveId;
		return sc;
	}

	public static DataRowCollection HeroIllustratedBooks(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroIllustratedBooks";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroIllustratedBook(Guid heroId, int nIllustratedBookId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroIllustratedBook";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nIllustratedBookId", SqlDbType.Int).Value = nIllustratedBookId;
		return sc;
	}

	public static DataRowCollection HeroSceneryQuests(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroSceneryQuests";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroSceneryQuest(Guid heroId, int nQuestId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroSceneryQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nQuestId", SqlDbType.Int).Value = nQuestId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_ExplorationPoint(Guid heroId, int nExplorationPoint)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_ExplorationPoint";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nExplorationPoint", SqlDbType.Int).Value = nExplorationPoint;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_IllustratedBookExplorationStep(Guid heroId, int nIllustratedBookExplorationstepNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_IllustratedBookExplorationStep";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nIllustratedBookExplorationstepNo", SqlDbType.Int).Value = nIllustratedBookExplorationstepNo;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_IllustratedBookStepReward(Guid heroId, DateTime illustratedBookExplorationStepRewardReceivedDate, int nIllustratedBookExplorationStepRewardReceivedStepNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_IllustratedBookStepReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@illustratedBookExplorationStepRewardReceivedDate", SqlDbType.Date).Value = illustratedBookExplorationStepRewardReceivedDate;
		sc.Parameters.Add("@nIllustratedBookExplorationStepRewardReceivedStepNo", SqlDbType.Int).Value = nIllustratedBookExplorationStepRewardReceivedStepNo;
		return sc;
	}

	public static DataRowCollection HeroAccomplishmentRewards(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroAccomplishmentRewards";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroAccomplishmentReward(Guid heroId, int nAccomplishmentId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroAccomplishmentReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nAccomplishmentId", SqlDbType.Int).Value = nAccomplishmentId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static int SoulCoverterInstanceMember_AccEnterCount(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SoulCoveterInstanceMember_AccEnterCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection HeroTitles(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroTitles";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroTitle(Guid heroId, int nTitleId, DateTimeOffset startTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroTitle";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nTitleId", SqlDbType.Int).Value = nTitleId;
		sc.Parameters.Add("@startTime", SqlDbType.DateTimeOffset).Value = startTime;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroTitle(Guid heroId, int nTitleId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroTtitle";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nTitleId", SqlDbType.Int).Value = nTitleId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_DisplayTitle(Guid heroId, int nDisplayTitleId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_DisplayTitle";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDisplayTitleId", SqlDbType.Int).Value = nDisplayTitleId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_ActivationTitle(Guid heroId, int nActivationTitleId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_ActivationTitle";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nActivationTitleId", SqlDbType.Int).Value = nActivationTitleId;
		return sc;
	}

	public static DataRowCollection HeroCreatureCards(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroCreatureCards";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroCreatureCard(Guid heroId, int nCreatureCardId, int nCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCard";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCreatureCardId", SqlDbType.Int).Value = nCreatureCardId;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroCreatureCard(Guid heroId, int nCreatureCardId, int nCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroCreaturCard";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCreatureCardId", SqlDbType.Int).Value = nCreatureCardId;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroCreatureCard(Guid heroId, int nCreatureCardId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroCreatureCard";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCreatureCardId", SqlDbType.Int).Value = nCreatureCardId;
		return sc;
	}

	public static SqlCommand CSC_AddOrUpdateHeroCreatureCard(Guid heroId, int nCreatureCardId, int nCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroCreatureCard";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCreatureCardId", SqlDbType.Int).Value = nCreatureCardId;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		return sc;
	}

	public static DataRowCollection HeroCreatureCardCollections(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroCreatureCardCollections";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroCreatureCardCollection(Guid heroId, int nCollectionId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCardCollection";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCollectionId", SqlDbType.Int).Value = nCollectionId;
		return sc;
	}

	public static DataRowCollection HeroCreatureCardShopFixedProductBuys(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroCreatureCardShopFixedProductBuys";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection HeroCreatureCardShopRandomProducts(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroCreatureCardShopRandomProducts";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroCreatureCardShopFixedProductBuy(Guid heroId, int nProductId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCardShopFixedProductBuy";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroCreatureCardshopFixedProductBuy(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroCreatureCardShopFixedProductBuy";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static SqlCommand CSC_AddHeroCreatureCardShopRandomProduct(Guid heroId, int nProductId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureCardShopRandomProduct";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroCreatureCardShopRandomProduct(Guid heroId, int nProductId, bool bPurchased)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroCreatureCardShopRandomProduct";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		sc.Parameters.Add("@bPurchased", SqlDbType.Bit).Value = bPurchased;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroCreatureCardShopRandomProducts(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroCreatureCardShopRandomProducts";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection HeroEliteMonsterKills(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroEliteMonsterKills";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateHeroEliteMonsterSkill(Guid heroId, int nEliteMonsterId, int nKillCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroEliteMonsterKill";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nEliteMonsterId", SqlDbType.Int).Value = nEliteMonsterId;
		sc.Parameters.Add("@nKillCount", SqlDbType.Int).Value = nKillCount;
		return sc;
	}

	public static DataRow EliteDungeonEnterCountOfDate(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_EliteDungeonEnterCountOfDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddHeroEliteDungeonPlay(Guid instanceId, Guid heroId, int nEliteMonsterId, int nStatus, int nPlayTime, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroEliteDungeonPlay";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nEliteMonsterId", SqlDbType.Int).Value = nEliteMonsterId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@nPlayTime", SqlDbType.Int).Value = nPlayTime;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroEliteDungeonPlay(Guid instanceId, int nStatus, int nPlayTime, DateTimeOffset statusUpdateTIme)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroEliteDungeonPlay";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@nPlayTime", SqlDbType.Int).Value = nPlayTime;
		sc.Parameters.Add("@statusUpdateTIme", SqlDbType.DateTimeOffset).Value = statusUpdateTIme;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_ProofOfValorFreeRefresh(Guid heroId, DateTime proofOfValorFreeRefreshDate, int nProofOfValorFreeRefreshCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_ProofOfValorFreeRefresh";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@proofOfValorFreeRefreshDate", SqlDbType.Date).Value = proofOfValorFreeRefreshDate;
		sc.Parameters.Add("@nProofOfValorFreeRefreshCount", SqlDbType.Int).Value = nProofOfValorFreeRefreshCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_ProofOfValorPaidRefreshCount(Guid heroId, int nProofOfValorPaidRefreshCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_ProofOfValorPaidRefreshCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nProofOfValorPaidRefreshCount", SqlDbType.Int).Value = nProofOfValorPaidRefreshCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_ProofOfValorDailyPaidRefresh(Guid heroId, DateTime proofOfValorDailyPaidRefreshDate, int nProofOfValorDailyPaidRefreshCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_ProofOfValorDailyPaidRefresh";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@proofOfValorDailyPaidRefreshDate", SqlDbType.Date).Value = proofOfValorDailyPaidRefreshDate;
		sc.Parameters.Add("@nProofOfValorDailyPaidRefreshCount", SqlDbType.Int).Value = nProofOfValorDailyPaidRefreshCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_ProofOfValorAutoRefresh(Guid heroId, DateTime proofOfValorAutoRefreshDate, int nProofOfValorAutoRefreshScheduleId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_ProofOfValorAutoRefresh";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@proofOfValorAutoRefreshDate", SqlDbType.Date).Value = proofOfValorAutoRefreshDate;
		sc.Parameters.Add("@nProofOfValorAutoRefreshScheduleId", SqlDbType.Int).Value = nProofOfValorAutoRefreshScheduleId;
		return sc;
	}

	public static DataRow ProofOfValorEnterCountOfDate(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ProofOfValorEnterCountOfDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static DataRow LastHeroProofOfValorInstance(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastHeroProofOfValorInstance";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddHeroProofOfValorInstance(Guid instanceId, Guid heroId, int nProofOfValorBossMonsterArrangeId, int nCreatureCardId, int nStatus, int nLevel, int nPlayTime, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroProofOfValorInstance";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nProofOfValorBossMonsterArrangeId", SqlDbType.Int).Value = nProofOfValorBossMonsterArrangeId;
		sc.Parameters.Add("@nCreatureCardId", SqlDbType.Int).Value = nCreatureCardId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nPlayTime", SqlDbType.Int).Value = nPlayTime;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroProofOfValorInstance(Guid instanceId, int nStatus, int nLevel, int nPlayTime, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroProofOfValorInstance";
		sc.Parameters.Add("@instanceId", SqlDbType.UniqueIdentifier).Value = instanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nPlayTime", SqlDbType.Int).Value = nPlayTime;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static int ProofOfValorClearedCount(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ProofOfValorClearedCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection ServerNotices(SqlConnection conn, SqlTransaction trans, DateTime fromTime, DateTime toTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ServerNotices";
		sc.Parameters.Add("@fromTime", SqlDbType.DateTime).Value = fromTime;
		sc.Parameters.Add("@toTime", SqlDbType.DateTime).Value = toTime;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_UpdateHero_Custom(Guid heroId, int nCustomPresetHair, int nCustomFaceJawHeight, int nCustomFaceJawWidth, int nCustomFaceJawEndHeight, int nCustomFaceWidth, int nCustomFaceEyebrowHeight, int nCustomFaceEyebrowRotation, int nCustomFaceEyesWidth, int nCustomFaceNoseHeight, int nCustomFaceNoseWidth, int nCustomFaceMouthHeight, int nCustomFaceMouthWidth, int nCustomBodyHeadSize, int nCustomBodyArmsLength, int nCustomBodyArmsWidth, int nCustomBodyChestSize, int nCustomBodyWaistWidth, int nCustomBodyHipsSize, int nCustomBodyPelvisWidth, int nCustomBodyLegsLength, int nCustomBodyLegsWidth, int nCustomColorSkin, int nCustomColorEyes, int nCustomColorBeardAndEyebrow, int nCustomColorHair)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_Custom";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCustomPresetHair", SqlDbType.Int).Value = nCustomPresetHair;
		sc.Parameters.Add("@nCustomFaceJawHeight", SqlDbType.Int).Value = nCustomFaceJawHeight;
		sc.Parameters.Add("@nCustomFaceJawWidth", SqlDbType.Int).Value = nCustomFaceJawWidth;
		sc.Parameters.Add("@nCustomFaceJawEndHeight", SqlDbType.Int).Value = nCustomFaceJawEndHeight;
		sc.Parameters.Add("@nCustomFaceWidth", SqlDbType.Int).Value = nCustomFaceWidth;
		sc.Parameters.Add("@nCustomFaceEyebrowHeight", SqlDbType.Int).Value = nCustomFaceEyebrowHeight;
		sc.Parameters.Add("@nCustomFaceEyebrowRotation", SqlDbType.Int).Value = nCustomFaceEyebrowRotation;
		sc.Parameters.Add("@nCustomFaceEyesWidth", SqlDbType.Int).Value = nCustomFaceEyesWidth;
		sc.Parameters.Add("@nCustomFaceNoseHeight", SqlDbType.Int).Value = nCustomFaceNoseHeight;
		sc.Parameters.Add("@nCustomFaceNoseWidth", SqlDbType.Int).Value = nCustomFaceNoseWidth;
		sc.Parameters.Add("@nCustomFaceMouthHeight", SqlDbType.Int).Value = nCustomFaceMouthHeight;
		sc.Parameters.Add("@nCustomFaceMouthWidth", SqlDbType.Int).Value = nCustomFaceMouthWidth;
		sc.Parameters.Add("@nCustomBodyHeadSize", SqlDbType.Int).Value = nCustomBodyHeadSize;
		sc.Parameters.Add("@nCustomBodyArmsLength", SqlDbType.Int).Value = nCustomBodyArmsLength;
		sc.Parameters.Add("@nCustomBodyArmsWidth", SqlDbType.Int).Value = nCustomBodyArmsWidth;
		sc.Parameters.Add("@nCustomBodyChestSize", SqlDbType.Int).Value = nCustomBodyChestSize;
		sc.Parameters.Add("@nCustomBodyWaistWidth", SqlDbType.Int).Value = nCustomBodyWaistWidth;
		sc.Parameters.Add("@nCustomBodyHipsSize", SqlDbType.Int).Value = nCustomBodyHipsSize;
		sc.Parameters.Add("@nCustomBodyPelvisWidth", SqlDbType.Int).Value = nCustomBodyPelvisWidth;
		sc.Parameters.Add("@nCustomBodyLegsLength", SqlDbType.Int).Value = nCustomBodyLegsLength;
		sc.Parameters.Add("@nCustomBodyLegsWidth", SqlDbType.Int).Value = nCustomBodyLegsWidth;
		sc.Parameters.Add("@nCustomColorSkin", SqlDbType.Int).Value = nCustomColorSkin;
		sc.Parameters.Add("@nCustomColorEyes", SqlDbType.Int).Value = nCustomColorEyes;
		sc.Parameters.Add("@nCustomColorBeardAndEyebrow", SqlDbType.Int).Value = nCustomColorBeardAndEyebrow;
		sc.Parameters.Add("@nCustomColorHair", SqlDbType.Int).Value = nCustomColorHair;
		return sc;
	}

	public static int UpdateHero_Custom(SqlConnection conn, SqlTransaction trans, Guid heroId, int nCustomPresetHair, int nCustomFaceJawHeight, int nCustomFaceJawWidth, int nCustomFaceJawEndHeight, int nCustomFaceWidth, int nCustomFaceEyebrowHeight, int nCustomFaceEyebrowRotation, int nCustomFaceEyesWidth, int nCustomFaceNoseHeight, int nCustomFaceNoseWidth, int nCustomFaceMouthHeight, int nCustomFaceMouthWidth, int nCustomBodyHeadSize, int nCustomBodyArmsLength, int nCustomBodyArmsWidth, int nCustomBodyChestSize, int nCustomBodyWaistWidth, int nCustomBodyHipsSize, int nCustomBodyPelvisWidth, int nCustomBodyLegsLength, int nCustomBodyLegsWidth, int nCustomColorSkin, int nCustomColorEyes, int nCustomColorBeardAndEyebrow, int nCustomColorHair)
	{
		SqlCommand sc = CSC_UpdateHero_Custom(heroId, nCustomPresetHair, nCustomFaceJawHeight, nCustomFaceJawWidth, nCustomFaceJawEndHeight, nCustomFaceWidth, nCustomFaceEyebrowHeight, nCustomFaceEyebrowRotation, nCustomFaceEyesWidth, nCustomFaceNoseHeight, nCustomFaceNoseWidth, nCustomFaceMouthHeight, nCustomFaceMouthWidth, nCustomBodyHeadSize, nCustomBodyArmsLength, nCustomBodyArmsWidth, nCustomBodyChestSize, nCustomBodyWaistWidth, nCustomBodyHipsSize, nCustomBodyPelvisWidth, nCustomBodyLegsLength, nCustomBodyLegsWidth, nCustomColorSkin, nCustomColorEyes, nCustomColorBeardAndEyebrow, nCustomColorHair);
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static DataRowCollection HeroNpcShopProducts(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroNpcShopProducts";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateHeroNpcShopProduct(Guid heroId, int nProductId, int nBuyCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroNpcShopProduct";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		sc.Parameters.Add("@nBuyCount", SqlDbType.Int).Value = nBuyCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_SelectedRankActiveSkill(Guid heroId, int nSelectedRankActiveSkillId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_SelectedRankActiveSkill";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSelectedRankActiveSkillId", SqlDbType.Int).Value = nSelectedRankActiveSkillId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_RankActiveSkillCastingTime(Guid heroId, DateTimeOffset rankActiveSkillCastTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_RankActiveSkillCastingTime";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@rankActiveSkillCastingTime", SqlDbType.DateTimeOffset).Value = rankActiveSkillCastTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_SpiritStone(Guid heroId, int nSpiritStone)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_SpiritStone";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSpiritStone", SqlDbType.Int).Value = nSpiritStone;
		return sc;
	}

	public static SqlCommand CSC_AddHeroRankActiveSkill(Guid heroId, int nSkillId, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGAPi_AddHeroRankActiveSkill";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSkillId", SqlDbType.Int).Value = nSkillId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroRankActiveSkill(Guid heroId, int nSkillId, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGAPi_UpdateHeroRankActiveSkill";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSkillId", SqlDbType.Int).Value = nSkillId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static DataRowCollection HeroRankActiveSkills(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroRankActiveSkills";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroRankPassiveSkill(Guid heroId, int nSkillId, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroRankPassiveSkill";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSkillId", SqlDbType.Int).Value = nSkillId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroRankPassiveSkill(Guid heroId, int nSkillId, int nLevel)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroRankPassiveSkill";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSkillId", SqlDbType.Int).Value = nSkillId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		return sc;
	}

	public static DataRowCollection HeroRankPassiveSkills(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroRankPassiveSkills";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_UpdateHero_RookieGift(Guid heroId, int nRookieGiftNo, float fRookieGiftLoginDuration)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_RookieGift";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRookieGiftNo", SqlDbType.Int).Value = nRookieGiftNo;
		sc.Parameters.Add("@fRookieGiftLoginDuration", SqlDbType.Float).Value = fRookieGiftLoginDuration;
		return sc;
	}

	public static DataRowCollection HeroOpenGiftRewards(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroOpenGiftRewards";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroOpenGiftReward(Guid heroId, int nDay)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroOpenGiftReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDay", SqlDbType.Int).Value = nDay;
		return sc;
	}

	public static int HeroDailyQuest_Count(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroDailyQuest_Count";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection HeroDailyQuests(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroDailyQuests";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroDailyQuest(Guid questInstanceId, Guid heroId, int nSlotIndex, int nMissionId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroDailyQuest";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nMissionId", SqlDbType.Int).Value = nMissionId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroDailyQuest_Accept(Guid questInstanceId, DateTimeOffset startTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroDailyQuest_Accept";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@startTime", SqlDbType.DateTimeOffset).Value = startTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroDailyQuest_Abandon(Guid questInstanceId, DateTimeOffset abandonTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroDailyQuest_Abandon";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@abandonTime", SqlDbType.DateTimeOffset).Value = abandonTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroDailyQuest_MissionImmediateComplete(Guid questInstanceId, DateTimeOffset missionImmediateCompletionTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroDailyQuest_MissionImmediateComplete";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@missionImmediateCompletionTime", SqlDbType.DateTimeOffset).Value = missionImmediateCompletionTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroDailyQuest_Complete(Guid questInstanceId, DateTimeOffset completionTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroDailyQuest_Complete";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@completionTime", SqlDbType.DateTimeOffset).Value = completionTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroDailyQuest_ProgressCount(Guid questInstanceId, int nProgressCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroDailyQuest_ProgressCount";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroDailyQuest(Guid questInstanceId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroDailyQuest";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_DailyQuestFreeRefreshDateCount(Guid heroId, DateTime dailyQuestFreeRefreshDate, int nDailyQuestFreeRefreshCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_DailyQuestFreeRefreshDateCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@dailyQuestFreeRefreshDate", SqlDbType.Date).Value = dailyQuestFreeRefreshDate;
		sc.Parameters.Add("@nDailyQuestFreeRefreshCount", SqlDbType.Int).Value = nDailyQuestFreeRefreshCount;
		return sc;
	}

	public static DataRow HeroWeeklyQuest(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroWeeklyQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddOrUpdateHeroWeeklyQuest(Guid heroId, DateTime weekStartDate, int nRoundNo, Guid roundId, int nRoundMissionId, int nRoundProgressCount, int nRoundStatus)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroWeeklyQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weekStartDate", SqlDbType.Date).Value = weekStartDate;
		sc.Parameters.Add("@nRoundNo", SqlDbType.Int).Value = nRoundNo;
		sc.Parameters.Add("@roundId", SqlDbType.UniqueIdentifier).Value = roundId;
		sc.Parameters.Add("@nRoundMissionId", SqlDbType.Int).Value = nRoundMissionId;
		sc.Parameters.Add("@nRoundProgressCount", SqlDbType.Int).Value = nRoundProgressCount;
		sc.Parameters.Add("@nRoundStatus", SqlDbType.Int).Value = nRoundStatus;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroWeeklyQuest_Accept(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroWeeklyQuest_Accept";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroWeeklyQuest_ProgressCount(Guid heroId, int nRoundProgressCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroWeeklyQuest_ProgressCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRoundProgressCount", SqlDbType.Int).Value = nRoundProgressCount;
		return sc;
	}

	public static DataRow WisdomTempleEnterCountOfDate(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTempleEnterCountOfDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddOrUpdateHeroWisdomTemplePlay(Guid heroId, DateTime date, int nCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroWisdomTemplePlay";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_WisdomTempleCleared(Guid heroId, bool bWisdomTempleCleared)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_WisdomTempleCleared";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@bWisdomTempleCleared", SqlDbType.Bit).Value = bWisdomTempleCleared;
		return sc;
	}

	public static DataRowCollection HeroOpen7DayEventMissions(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroOpen7DayEventMissions";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroOpen7DayEventMission(Guid heroId, int nMissionId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroOpen7DayEventMission";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMissionId", SqlDbType.Int).Value = nMissionId;
		return sc;
	}

	public static DataRowCollection HeroOpen7DayEventProducts(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroOpen7DayEventProducts";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroOpen7DayEventProduct(Guid heroId, int nProductId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroOpen7DayEventProduct";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		return sc;
	}

	public static DataRowCollection HeroOpen7DayEventProgressCounts(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroOpen7DayEventProgressCounts";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateHeroOpen7DayEventProgressCount(Guid heroId, int nType, int nAccProgressCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroOpen7DayEventProgressCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nType", SqlDbType.Int).Value = nType;
		sc.Parameters.Add("@nAccProgressCount", SqlDbType.Int).Value = nAccProgressCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_Open7DayEventRewarded(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_Open7DayEventRewarded";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection HeroRetrievalProgressCounts(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroRetrievalProgressCounts";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateHeroRetrievalProgressCount(Guid heroId, int nRetrievalId, DateTime date, int nProgressCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroRetrievalProgressCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRetrievalId", SqlDbType.Int).Value = nRetrievalId;
		sc.Parameters.Add("@date", SqlDbType.DateTime).Value = date;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		return sc;
	}

	public static DataRowCollection HeroRetrievals(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroRetrivals";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateHeroRetrieval(Guid heroId, DateTime date, int nRetrievalId, int nCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroRetrieval";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.DateTime).Value = date;
		sc.Parameters.Add("@nRetrievalId", SqlDbType.Int).Value = nRetrievalId;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		return sc;
	}

	public static DataRowCollection HeroTaskConsignments_Current(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroTaskConsignments_Current";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection HeroTaskConsignmentCountsOfDate(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroTaskConsignmentCountsOfDate";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroTaskConsignment(Guid consignmentInstanceId, Guid heroId, int nConsignmentId, int nUsedExpItemId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroTaskConsignment";
		sc.Parameters.Add("@consignmentInstanceId", SqlDbType.UniqueIdentifier).Value = consignmentInstanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nConsignmentId", SqlDbType.Int).Value = nConsignmentId;
		sc.Parameters.Add("@nUsedExpItemId", SqlDbType.Int).Value = nUsedExpItemId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroTaskConsignment_Status(Guid consignmentInstanceId, int nStatus, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroTaskConsignment_Status";
		sc.Parameters.Add("@consignmentInstanceId", SqlDbType.UniqueIdentifier).Value = consignmentInstanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_RuinsReclaimFreePlay(Guid heroId, DateTime ruinsReclaimFreePlayDate, int nRuinsReclaimFreePlayCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_RuinsReclaimFreePlay";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@ruinsReclaimFreePlayDate", SqlDbType.Date).Value = ruinsReclaimFreePlayDate;
		sc.Parameters.Add("@nRuinsReclaimFreePlayCount", SqlDbType.Int).Value = nRuinsReclaimFreePlayCount;
		return sc;
	}

	public static DataRow HeroTrueHeroQuest(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroTrueHeroQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddOrUpdateHeroTrueHeroQuest(Guid heroId, Guid questInstanceId, int nStepNo, int nVipLevel, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroTrueHeroQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nStepNo", SqlDbType.Int).Value = nStepNo;
		sc.Parameters.Add("@nVipLevel", SqlDbType.Int).Value = nVipLevel;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroTrueHeroQuest_Step(Guid heroId, int nStepNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroTrueHeroQuest_Step";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nStepNo", SqlDbType.Int).Value = nStepNo;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroTrueHeroQuest_Completion(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroTrueHeroQuest_Completion";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroTrueHeroQuest(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroTrueHeroQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_InfiniteWarPlay(Guid heroId, DateTime infiniteWarPlayDate, int nInfiniteWarPlayCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_InfiniteWarPlay";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@infiniteWarPlayDate", SqlDbType.Date).Value = infiniteWarPlayDate;
		sc.Parameters.Add("@nInfiniteWarPlayCount", SqlDbType.Int).Value = nInfiniteWarPlayCount;
		return sc;
	}

	public static DataRowCollection HeroLimitationGiftRewards(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroLimitationGiftRewards";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroLimitationGiftReawrd(Guid heroId, DateTime date, int nScheduleId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroLimitationGiftReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nScheduleId", SqlDbType.Int).Value = nScheduleId;
		return sc;
	}

	public static DataRow HeroWeekendReward(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroWeekendReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddOrUpdateHeroWeekendReward(Guid heroId, DateTime weekStartDate, int nSelection1, int nSelection2, int nSelection3)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroWeekendReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weekStartDate", SqlDbType.Date).Value = weekStartDate;
		sc.Parameters.Add("@nSelection1", SqlDbType.Int).Value = nSelection1;
		sc.Parameters.Add("@nSelection2", SqlDbType.Int).Value = nSelection2;
		sc.Parameters.Add("@nSelection3", SqlDbType.Int).Value = nSelection3;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroWeekendReward_Rewarded(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroWeekendReward_Rewarded";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static DataRowCollection WarehouseSlots(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WarehouseSlots";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_UpdateWarehouseSlot(Guid heroId, int nSlotIndex, int nSlotType, Guid heroMainGearId, int nSubGearId, int nItemId, int nItemCount, bool bItemOwned, Guid heroMountGearId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateWarehouseSlot";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nSlotType", SqlDbType.Int).Value = nSlotType;
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@heroMountGearId", SqlDbType.UniqueIdentifier).Value = heroMountGearId;
		return sc;
	}

	public static SqlCommand CSC_AddOrUpdateWarehouseSlot(Guid heroId, int nSlotIndex, int nSlotType, Guid heroMainGearId, int nSubGearId, int nItemId, int nItemCount, bool bItemOwned, Guid heroMountGearId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateWarehouseSlot";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nSlotType", SqlDbType.Int).Value = nSlotType;
		sc.Parameters.Add("@heroMainGearId", SqlDbType.UniqueIdentifier).Value = heroMainGearId;
		sc.Parameters.Add("@nSubGearId", SqlDbType.Int).Value = nSubGearId;
		sc.Parameters.Add("@nItemId", SqlDbType.Int).Value = nItemId;
		sc.Parameters.Add("@nItemCount", SqlDbType.Int).Value = nItemCount;
		sc.Parameters.Add("@bItemOwned", SqlDbType.Bit).Value = bItemOwned;
		sc.Parameters.Add("@heroMountGearId", SqlDbType.UniqueIdentifier).Value = heroMountGearId;
		return sc;
	}

	public static SqlCommand CSC_DeleteWarehouseSlot(Guid heroId, int nSlotIndex)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteWarehouseSlot";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_PaidWarehouseSlotCount(Guid heroId, int nPaidWarehouseSlotCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_PaidWarehouseSlotCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nPaidWarehouseSlotCount", SqlDbType.Int).Value = nPaidWarehouseSlotCount;
		return sc;
	}

	public static DataRowCollection HeroDiaShopProducts(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroDiaShopProducts";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection HeroDiaShopProducts_AccBuyCount(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroDiaShopProducts_AccBuyCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateHeroDiaShopProduct(Guid heroId, DateTime date, int nProductId, int nBuyCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroDiaShopProduct";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		sc.Parameters.Add("@nBuyCount", SqlDbType.Int).Value = nBuyCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_FearAltarPlay(Guid heroId, DateTime fearAltarPlayDate, int nFearAltarPlayCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_FearAltarPlay";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@fearAltarPlayDate", SqlDbType.Date).Value = fearAltarPlayDate;
		sc.Parameters.Add("@nFearAltarPlayCount", SqlDbType.Int).Value = nFearAltarPlayCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_FearAltarHalidomCollectionReward(Guid heroId, DateTime fearAltarHalidomCollectionRewardWeekStartDate, int nFearAltarHalidomCollectionRewardNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_FearAltarHalidomCollectionReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@fearAltarHalidomCollectionRewardWeekStartDate", SqlDbType.Date).Value = fearAltarHalidomCollectionRewardWeekStartDate;
		sc.Parameters.Add("@nFearAltarHalidomCollectionRewardNo", SqlDbType.Int).Value = nFearAltarHalidomCollectionRewardNo;
		return sc;
	}

	public static DataRowCollection HeroFearAltarHalidoms(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime weekStartDate)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroFearAltarHalidoms";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weekStartDate", SqlDbType.Date).Value = weekStartDate;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroFearAltarHalidom(Guid heroId, DateTime weekStartDate, int nHalidomId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroFearAltarHalidom";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weekStartDate", SqlDbType.Date).Value = weekStartDate;
		sc.Parameters.Add("@nHalidomId", SqlDbType.Int).Value = nHalidomId;
		return sc;
	}

	public static DataRowCollection HeroFearAltarHalidomElementalRewards(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime weekStartDate)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroFearAltarHalidomElementalRewards";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weekStartDate", SqlDbType.Date).Value = weekStartDate;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroFearAltarHalidomElementalReward(Guid heroId, DateTime weekStartDate, int nHalidomElementalId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroFearAltarHalidomElementalReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weekStartDate", SqlDbType.Date).Value = weekStartDate;
		sc.Parameters.Add("@nHalidomElementalId", SqlDbType.Int).Value = nHalidomElementalId;
		return sc;
	}

	public static DataRowCollection HeroSubQuests(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroSubQuests";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroSubQuest(Guid heroId, int nSubQuestId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroSubQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubQuestId", SqlDbType.Int).Value = nSubQuestId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_AddOrUpdateHeroSubQuest(Guid heroid, int nSubQuestId, int nProgressCount, DateTimeOffset regTime, int nStatus, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroSubquest";
		sc.Parameters.Add("@heroid", SqlDbType.UniqueIdentifier).Value = heroid;
		sc.Parameters.Add("@nSubQuestId", SqlDbType.Int).Value = nSubQuestId;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroSubQuest_ProgressCount(Guid heroId, int nSubQuestId, int nProgressCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroSubQuest_ProgressCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubQuestId", SqlDbType.Int).Value = nSubQuestId;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroSubQuest_Complete(Guid heroId, int nSubQuestId, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroSubQuest_Complete";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubQuestId", SqlDbType.Int).Value = nSubQuestId;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroSubQuest_Abandon(Guid heroId, int nSubQuestId, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroSubQuest_Abandon";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nSubQuestId", SqlDbType.Int).Value = nSubQuestId;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_WarMemoryFreePlay(Guid heroId, DateTime warMemoryFreePlayDate, int nWarMemoryFreePlayCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_WarMemoryFreePlay";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@warMemoryFreePlayDate", SqlDbType.Date).Value = warMemoryFreePlayDate;
		sc.Parameters.Add("@nWarMemoryFreePlayCount", SqlDbType.Int).Value = nWarMemoryFreePlayCount;
		return sc;
	}

	public static DataRow HeroOrdealQuest(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroOrdealQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddHeroOrdealQuest(Guid heroId, int nQuestNo, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroOrdealQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nQuestNo", SqlDbType.Int).Value = nQuestNo;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroOrdealQuest_Complete(Guid heroId, int nQuestNo, DateTimeOffset completionTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroOrdealQuest_Complete";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nQuestNo", SqlDbType.Int).Value = nQuestNo;
		sc.Parameters.Add("@completionTime", SqlDbType.DateTimeOffset).Value = completionTime;
		return sc;
	}

	public static DataRowCollection HeroOrdealQuestMissions(SqlConnection conn, SqlTransaction trans, Guid heroId, int nQuestNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroOrdealQuestMissions";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nQuestNo", SqlDbType.Int).Value = nQuestNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroOrdealQuestMission(Guid heroId, int nQuestNo, int nSlotIndex, int nMissionNo, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroOrdealQuestMission";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nQuestNo", SqlDbType.Int).Value = nQuestNo;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nMissionNo", SqlDbType.Int).Value = nMissionNo;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroOrdealQuestMission_ProgressCount(Guid heroId, int nQuestNo, int nSlotIndex, int nMissionNo, int nProgressCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroOrdealQuestMission_PrgressCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nQuestNo", SqlDbType.Int).Value = nQuestNo;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nMissionNo", SqlDbType.Int).Value = nMissionNo;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroOrdealQuestMission_Complete(Guid heroId, int nQuestNo, int nSlotIndex, int nMissionNo, DateTimeOffset completionTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroOrdealQuestMission_Complete";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nQuestNo", SqlDbType.Int).Value = nQuestNo;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nMissionNo", SqlDbType.Int).Value = nMissionNo;
		sc.Parameters.Add("@completionTime", SqlDbType.DateTimeOffset).Value = completionTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_OsirisRoomPlay(Guid heroId, DateTime osirisRoomPlayDate, int nOsirisRoomPlayCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_OsirisRoomPlay";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@osirisRoomPlayDate", SqlDbType.DateTime).Value = osirisRoomPlayDate;
		sc.Parameters.Add("@nOsirisRoomPlayCount", SqlDbType.Int).Value = nOsirisRoomPlayCount;
		return sc;
	}

	public static DataRowCollection Friends(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Friends";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddFriend(Guid heroId, Guid friendId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddFriend";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@friendId", SqlDbType.UniqueIdentifier).Value = friendId;
		return sc;
	}

	public static SqlCommand CSC_DeleteFriend(Guid heroId, Guid friendId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteFriend";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@friendId", SqlDbType.UniqueIdentifier).Value = friendId;
		return sc;
	}

	public static DataRowCollection TempFriends(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TempFriends";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddTempFriend(Guid heroId, Guid friendId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddTempFriend";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@friendId", SqlDbType.UniqueIdentifier).Value = friendId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_DeleteTempFriend(Guid heroId, Guid friendId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteTempFriend";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@friendId", SqlDbType.UniqueIdentifier).Value = friendId;
		return sc;
	}

	public static DataRowCollection BlacklistEntries(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_BlacklistEntries";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddBlacklistEntry(Guid heroId, Guid targetHeroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddBlacklistEntry";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@targetHeroId", SqlDbType.UniqueIdentifier).Value = targetHeroId;
		return sc;
	}

	public static SqlCommand CSC_DeleteBlacklistEntry(Guid heroId, Guid targetHeroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteBlacklistEntry";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@targetHeroId", SqlDbType.UniqueIdentifier).Value = targetHeroId;
		return sc;
	}

	public static DataRowCollection DeadRecords(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeadRecords";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddDeadRecord(Guid recordId, Guid heroId, Guid killerId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddDeadRecord";
		sc.Parameters.Add("@recordId", SqlDbType.UniqueIdentifier).Value = recordId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@killerId", SqlDbType.UniqueIdentifier).Value = killerId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_DeleteDeadRecord(Guid recordId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteDeadRecord";
		sc.Parameters.Add("@recordId", SqlDbType.UniqueIdentifier).Value = recordId;
		return sc;
	}

	public static DataRowCollection HeroBiographies(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroBiographies";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroBiography(Guid heroId, int nBiographyId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroBiography";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nBiographyId", SqlDbType.Int).Value = nBiographyId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroBiography_Complete(Guid heroId, int nBiographyId, DateTimeOffset completionTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroBiography_Complete";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nBiographyId", SqlDbType.Int).Value = nBiographyId;
		sc.Parameters.Add("@completionTime", SqlDbType.DateTimeOffset).Value = completionTime;
		return sc;
	}

	public static DataRowCollection HeroBiographyQuests(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroBiographyQuests";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroBiographyQuest(Guid heroId, int nBiographyId, int nQuestNo, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroBiographyQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nBiographyId", SqlDbType.Int).Value = nBiographyId;
		sc.Parameters.Add("@nQuestNo", SqlDbType.Int).Value = nQuestNo;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroBiography_ProgressCount(Guid heroId, int nBiographyId, int nQuestNo, int nProgressCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroBiographyQuest_ProgressCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nBiographyId", SqlDbType.Int).Value = nBiographyId;
		sc.Parameters.Add("@nQuestNo", SqlDbType.Int).Value = nQuestNo;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroBiographyQuest_Complete(Guid heroId, int nBiographyId, int nQuestNo, DateTimeOffset completionTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroBiographyQuest_Complete";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nBiographyId", SqlDbType.Int).Value = nBiographyId;
		sc.Parameters.Add("@nQuestNo", SqlDbType.Int).Value = nQuestNo;
		sc.Parameters.Add("@completionTime", SqlDbType.DateTimeOffset).Value = completionTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_ItemLuckyShopPickCount(Guid heroId, DateTime itemLuckyShopPickDate, int nItemLuckyShopFreePickCount, int nItemLuckyShopPick1TimeCount, int nItemLuckyShopPick5TimeCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_ItemLuckyShopPickCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@itemLuckyShopPickDate", SqlDbType.Date).Value = itemLuckyShopPickDate;
		sc.Parameters.Add("@nItemLuckyShopFreePickCount", SqlDbType.Int).Value = nItemLuckyShopFreePickCount;
		sc.Parameters.Add("@nItemLuckyShopPick1TimeCount", SqlDbType.Int).Value = nItemLuckyShopPick1TimeCount;
		sc.Parameters.Add("@nItemLuckyShopPick5TimeCount", SqlDbType.Int).Value = nItemLuckyShopPick5TimeCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_ItemLuckyShopFreePickTime(Guid heroId, DateTimeOffset itemLuckyShopFreePickTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_ItemLuckyShopFreePickTime";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@itemLuckyShopFreePickTime", SqlDbType.DateTimeOffset).Value = itemLuckyShopFreePickTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_CreatureCardLuckyShopPickCount(Guid heroId, DateTime creatureCardLuckyShopPickDate, int nCreatureCardLuckyShopFreePickCount, int nCreatureCardLuckyShopPick1TimeCount, int nCreatureCardLuckyShopPick5TimeCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_CreatureCardLuckyShopPickCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@creatureCardLuckyShopPickDate", SqlDbType.Date).Value = creatureCardLuckyShopPickDate;
		sc.Parameters.Add("@nCreatureCardLuckyShopFreePickCount", SqlDbType.Int).Value = nCreatureCardLuckyShopFreePickCount;
		sc.Parameters.Add("@nCreatureCardLuckyShopPick1TimeCount", SqlDbType.Int).Value = nCreatureCardLuckyShopPick1TimeCount;
		sc.Parameters.Add("@nCreatureCardLuckyShopPick5TimeCount", SqlDbType.Int).Value = nCreatureCardLuckyShopPick5TimeCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_CreatureCardLuckyShopFreePickTime(Guid heroId, DateTimeOffset creatureCardLuckyShopFreePickTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_CreatureCardLuckyShopFreePickTime";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@creatureCardLuckyShopFreePickTime", SqlDbType.DateTimeOffset).Value = creatureCardLuckyShopFreePickTime;
		return sc;
	}

	public static SqlCommand CSC_AddHeroProspectQuest(Guid questInstanceId, Guid ownerId, Guid targetId, int nTargetLevelId, int nStatus, DateTimeOffset regTime, DateTimeOffset? statusUpdateTIme)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroProspectQuest";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@ownerId", SqlDbType.UniqueIdentifier).Value = ownerId;
		sc.Parameters.Add("@targetId", SqlDbType.UniqueIdentifier).Value = targetId;
		sc.Parameters.Add("@nTargetLevelId", SqlDbType.Int).Value = nTargetLevelId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		sc.Parameters.Add("@statusUpdateTIme", SqlDbType.DateTimeOffset).Value = SFDBUtil.NullToDBNull(statusUpdateTIme);
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroProspectQuest_Status(Guid questInstanceId, int nStatus, DateTimeOffset? statusUpdateTIme)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroProspectQuest_Status";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTIme", SqlDbType.DateTimeOffset).Value = SFDBUtil.NullToDBNull(statusUpdateTIme);
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroProspectQuest_OwnerRewarded(Guid questInstanceId, bool bOwnerRewarded)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroProspectQuest_OwnerRewarded";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@bOwnerRewarded", SqlDbType.Bit).Value = bOwnerRewarded;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroProspectQuest_TargetRewarded(Guid questInstanceId, bool bTargetRewarded)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroProspectQuest_TargetRewarded";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@bTargetRewarded", SqlDbType.Bit).Value = bTargetRewarded;
		return sc;
	}

	public static DataRowCollection HeroProspectQuests_Progressing(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroProspectQuests_Progressing";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection HeroProspectQuestsOfHero_Owner(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroProspectQuestsOfHero_Owner";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection HeroProspectQuestsOfHero_Target(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroProspectQuestsOfHero_Target";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection HeroCreatures(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroCreatures";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroCreature(Guid heroCreatureId, Guid heroId, int nCreatureId, int nLevel, int nInjectionLevel, int nQuality)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreature";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCreatureId", SqlDbType.Int).Value = nCreatureId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nInjectionLevel", SqlDbType.Int).Value = nInjectionLevel;
		sc.Parameters.Add("@nQuality", SqlDbType.Int).Value = nQuality;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroCreature(Guid heroCreatureId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroCreature";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroCreature_Cheer(Guid heroCreatureId, bool bCheered)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroCreature_Cheer";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@bCheered", SqlDbType.Bit).Value = bCheered;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroCreature_Level(Guid heroCreatureId, int nLevel, int nExp)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroCreature_Level";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@nLevel", SqlDbType.Int).Value = nLevel;
		sc.Parameters.Add("@nExp", SqlDbType.Int).Value = nExp;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroCreature_InjectionLevel(Guid heroCreatureId, int nInjectionLevel, int nInjectionExp, int nInjectionItemCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroCreature_InjectionLevel";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@nInjectionLevel", SqlDbType.Int).Value = nInjectionLevel;
		sc.Parameters.Add("@nInjectionExp", SqlDbType.Int).Value = nInjectionExp;
		sc.Parameters.Add("@nInjectionItemCount", SqlDbType.Int).Value = nInjectionItemCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroCreature_Quality(Guid heroCreatureId, int nQuality)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroCreature_Quality";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@nQuality", SqlDbType.UniqueIdentifier).Value = nQuality;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroCreature_AdditionalOpenSkillSlotCount(Guid heroCreatureId, int nAdditionalOpenSkillSlotCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroCreature_AdditionalOpenSkillSlotCount";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@nAdditionalOpenSkillSlotCount", SqlDbType.Int).Value = nAdditionalOpenSkillSlotCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_CreatureParticipation(Guid heroId, Guid participationHeroCreatureId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_CreatureParticipation";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@participationHeroCreatureId", SqlDbType.UniqueIdentifier).Value = participationHeroCreatureId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_CreatureVariationDateCount(Guid heroId, DateTime creatureVariationDate, int nCreatureVariationCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_CreatureVariationDateCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@creatureVariationDate", SqlDbType.Date).Value = creatureVariationDate;
		sc.Parameters.Add("@nCreatureVariationCount", SqlDbType.Int).Value = nCreatureVariationCount;
		return sc;
	}

	public static DataRowCollection HeroCreatureBaseAttrs(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroCreatureBaseAttrs";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroCreatureBaseAttr(Guid heroCreatureId, int nAttrId, int nBaseValue)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureBaseAttr";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		sc.Parameters.Add("@nBaseValue", SqlDbType.Int).Value = nBaseValue;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroCreatureBaseAttrs(Guid heroCreatureId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroCreautreBaseAttrs";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		return sc;
	}

	public static DataRowCollection HeroCreatureAdditionalAttrs(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroCreatureAdditionalAttrs";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroCreatureAdditionalAttr(Guid heroCreatureId, int nAttrNo, int nAttrId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureAdditionalAttr";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@nAttrNo", SqlDbType.Int).Value = nAttrNo;
		sc.Parameters.Add("@nAttrId", SqlDbType.Int).Value = nAttrId;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroCreatureAdditionalAttrs(Guid heroCreatureId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroCreatureAdditionalAttrs";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		return sc;
	}

	public static DataRowCollection HeroCreatureSkills(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroCreatureSkills";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroCreatureSkill(Guid heroCreatureId, int nSlotIndex, int nSkillId, int nSkillGrade)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureSkill";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nSkillId", SqlDbType.Int).Value = nSkillId;
		sc.Parameters.Add("@nSkillGrade", SqlDbType.Int).Value = nSkillGrade;
		return sc;
	}

	public static DataRow HeroWeeklyPresentPopularityPoint(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime weekStartDate)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroWeeklyPresentPopularityPoint";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weekStartDate", SqlDbType.Date).Value = weekStartDate;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddOrUpdateHeroWeeklyPresentPopularityPoint(Guid heroId, DateTime weekStartDate, int nPoint, DateTimeOffset pointUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroWeeklyPresentPopularityPoint";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weekStartDate", SqlDbType.Date).Value = weekStartDate;
		sc.Parameters.Add("@nPoint", SqlDbType.Int).Value = nPoint;
		sc.Parameters.Add("@pointUpdateTime", SqlDbType.DateTimeOffset).Value = pointUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_AddOrUpdateHeroWeeklyPresentPopularityPoint_Add(Guid heroId, DateTime weekStartDate, int nAddingPoint, DateTimeOffset pointUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroWeeklyPresentPopularityPoint_Add";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weekStartDate", SqlDbType.Date).Value = weekStartDate;
		sc.Parameters.Add("@nAddingPoint", SqlDbType.Int).Value = nAddingPoint;
		sc.Parameters.Add("@pointUpdateTime", SqlDbType.DateTimeOffset).Value = pointUpdateTime;
		return sc;
	}

	public static DataRow HeroWeeklyPresentContributionPoint(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime weekStartDate)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroWeeklyPresentContributionPoint";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weekStartDate", SqlDbType.Date).Value = weekStartDate;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddOrUpdateHeroWeeklyPresentContributionPoint(Guid heroId, DateTime weekStartDate, int nPoint, DateTimeOffset pointUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroWeeklyPresentContributionPoint";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@weekStartDate", SqlDbType.Date).Value = weekStartDate;
		sc.Parameters.Add("@nPoint", SqlDbType.Int).Value = nPoint;
		sc.Parameters.Add("@pointUpdateTime", SqlDbType.DateTimeOffset).Value = pointUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroCreatureSkills(Guid heroCreatureId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroCreatureSkills";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		return sc;
	}

	public static SqlCommand CSC_AddOrUpdateHeroCreatureSkill(Guid heroCreatureId, int nSlotIndex, int nSkillId, int nSkillGrade)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroCreatureSkill";
		sc.Parameters.Add("@heroCreatureId", SqlDbType.UniqueIdentifier).Value = heroCreatureId;
		sc.Parameters.Add("@nSlotIndex", SqlDbType.Int).Value = nSlotIndex;
		sc.Parameters.Add("@nSkillId", SqlDbType.Int).Value = nSkillId;
		sc.Parameters.Add("@nSkillGrade", SqlDbType.Int).Value = nSkillGrade;
		return sc;
	}

	public static int LastServerPresentPopularityPointRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastServerPresentPopularityPointRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection ServerPresentPopularityPointRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ServerPresentPopularityPointRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int LastNationWeeklyPresentPopularityPointRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastNationWeeklyPresentPopularityPointRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection NationWeeklyPresentPopularityPointRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWeeklyPresentPopularityPointRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_UpdateHero_NationWeeklyPresentPopularityPointRankingReward(Guid heroId, int nRewardedNationWeeklyPresentPopularityPointRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_NationWeeklyPresentPopularityPointRankingReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardedNationWeeklyPresentPopularityPointRankingNo", SqlDbType.Int).Value = nRewardedNationWeeklyPresentPopularityPointRankingNo;
		return sc;
	}

	public static int LastServerPresentContributionPointRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastServerPresentContributionPointRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection ServerPresentContributionPointRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ServerPresentContributionPointRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int LastNationWeeklyPresentContributionPointRankingNo(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LastNationWeeklyPresentContributionPointRankingNo";
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static DataRowCollection NationWeeklyPresentContributionPointRankings(SqlConnection conn, SqlTransaction trans, int nRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWeeklyPresentContributionPointRankings";
		sc.Parameters.Add("@nRankingNo", SqlDbType.Int).Value = nRankingNo;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_UpdateHero_NationWeeklyPresentContributionPointRankingReward(Guid heroId, int nRewardedNationWeeklyPresentContributionPointRankingNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_NationWeeklyPresentContributionPointRankingReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nRewardedNationWeeklyPresentContributionPointRankingNo", SqlDbType.Int).Value = nRewardedNationWeeklyPresentContributionPointRankingNo;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_EquippedHeroCostume(Guid heroId, int nEquippedCostumeId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_EquippedHeroCostume";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nEquippedCostumeId", SqlDbType.Int).Value = nEquippedCostumeId;
		return sc;
	}

	public static DataRowCollection HeroCostumes(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroCostumes";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow HeroCostume(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroCostume";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddHeroCostume(Guid heroId, int nCostumeId, int nCostumeEffectId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCostume";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCostumeId", SqlDbType.Int).Value = nCostumeId;
		sc.Parameters.Add("@nCostumeEffectId", SqlDbType.Int).Value = nCostumeEffectId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroCostume_CostumeEffect(Guid heroId, int nCostumeId, int nCostumeEffectId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroCostume_CostumeEffect";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCostumeId", SqlDbType.Int).Value = nCostumeId;
		sc.Parameters.Add("@nCostumeEffectId", SqlDbType.Int).Value = nCostumeEffectId;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroCostume(Guid heroId, int nCostumeId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroCostume";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCostumeId", SqlDbType.Int).Value = nCostumeId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroCostume_EnchantLevel(Guid heroId, int nCostumeId, int nEnchantLevel, int nLuckyValue)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroCostume_EnchantLevel";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCostumeId", SqlDbType.Int).Value = nCostumeId;
		sc.Parameters.Add("@nEnchantLevel", SqlDbType.Int).Value = nEnchantLevel;
		sc.Parameters.Add("@nLuckyValue", SqlDbType.Int).Value = nLuckyValue;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_CostumeCollectionId(Guid heroId, int nCostumeCollectionId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_CostumeCollectionId";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nCostumeCollectionId", SqlDbType.Int).Value = nCostumeCollectionId;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_CostumeCollectionActivation(Guid heroId, bool bCostumeCollectionActivated)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_CostumeCollectionActivation";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@bCostumeCollectionActivated", SqlDbType.Bit).Value = bCostumeCollectionActivated;
		return sc;
	}

	public static DataRow HeroCreatureFarmQuest(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroCreatureFarmQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static int HeroCreatureFarmQuest_AcceptionCount(SqlConnection conn, SqlTransaction trans, Guid heroId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroCreatureFarmQuest_AcceptionCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static SqlCommand CSC_AddHeroCreatureFarmQuest(Guid questInstanceId, Guid heroId, int nMissionNo, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroCreatureFarmQuest";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nMissionNo", SqlDbType.Int).Value = nMissionNo;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroCreatureFarmQuest_ProgressCount(Guid questInstanceId, int nProgressCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroCreautrFarmQuest_ProgressCount";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroCreatureFarmQuest_Mission(Guid questInstanceId, int nMissionNo, DateTimeOffset missionStartTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroCreatureFarmQuest_Mission";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nMissionNo", SqlDbType.Int).Value = nMissionNo;
		sc.Parameters.Add("@missionStartTime", SqlDbType.DateTimeOffset).Value = missionStartTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroCreatureFarmQuest_Completion(Guid questInstanceId, DateTimeOffset completionTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroCreatureFarmQuest_Completion";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@completionTime", SqlDbType.DateTimeOffset).Value = completionTime;
		return sc;
	}

	public static DataRowCollection Alliances(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Alliances";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddAlliance(Guid allianceId, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAlliance";
		sc.Parameters.Add("@allianceId", SqlDbType.UniqueIdentifier).Value = allianceId;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_DeleteAlliance(Guid allianceId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteAlliance";
		sc.Parameters.Add("@allianceId", SqlDbType.UniqueIdentifier).Value = allianceId;
		return sc;
	}

	public static DataRowCollection CashProductPurchaseCounts(SqlConnection conn, SqlTransaction trans, Guid accountId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CashProductPurchaseCounts";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateCashProductPurchaseCount(Guid accountId, int nProductId, int nPurchaseCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateCashProductPurchaseCount";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		sc.Parameters.Add("@nPurchaseCount", SqlDbType.Int).Value = nPurchaseCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateAccount_FirstChargeEventObjectiveCompletion(Guid accountId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateAccount_FirstChargeEventObjectiveCompletion";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		return sc;
	}

	public static SqlCommand CSC_UpdateAccount_FirstChargeEventReward(Guid accountId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateAccount_FirstChargeEventReward";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		return sc;
	}

	public static SqlCommand CSC_UpdateAccount_RechargeEventAccUnOwnDia(Guid accountId, int nRechargeEventAccUnOwnDia)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateAccount_RechargeEventAccUnOwnDia";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@nRechargeEventAccUnOwnDia", SqlDbType.Int).Value = nRechargeEventAccUnOwnDia;
		return sc;
	}

	public static SqlCommand CSC_UpdateAccount_RechargeEventReward(Guid accountId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateAccount_RechargeEventReward";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		return sc;
	}

	public static DataRow AccountChargeEvent(SqlConnection conn, SqlTransaction trans, Guid accountId, int nEventId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AccountChargeEvent";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@nEventId", SqlDbType.Int).Value = nEventId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static DataRowCollection AccountChargeEventMissionRewards(SqlConnection conn, SqlTransaction trans, Guid accountId, int nEventId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AccountChargeEventMissionRewards";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@nEventId", SqlDbType.Int).Value = nEventId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateAccountChargeEvent(Guid accountId, int nEventId, int nAccUnOwnDia)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateAccountChargeEvent";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@nEventId", SqlDbType.Int).Value = nEventId;
		sc.Parameters.Add("@nAccUnOwnDia", SqlDbType.Int).Value = nAccUnOwnDia;
		return sc;
	}

	public static SqlCommand CSC_AddAccountChargeEventMissionReward(Guid accountId, int nEventId, int nMissionNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAccountChargeEventMissionReward";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@nEventId", SqlDbType.Int).Value = nEventId;
		sc.Parameters.Add("@nMissionNo", SqlDbType.Int).Value = nMissionNo;
		return sc;
	}

	public static DataRow AccountDailyChargeEvent(SqlConnection conn, SqlTransaction trans, Guid accountId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AccountDailyChargeEvent";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static DataRowCollection AccountDailyChargeEventMissionRewards(SqlConnection conn, SqlTransaction trans, Guid accountId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AccountDailyChargeEventMissionRewards";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateAccountDailyChargeEvent(Guid accountId, DateTime date, int nAccUnOwnDia)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateAccountDailyChargeEvent";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nAccUnOwnDia", SqlDbType.Int).Value = nAccUnOwnDia;
		return sc;
	}

	public static SqlCommand CSC_AddAccountDailyChargeEventMissionReward(Guid accountId, DateTime date, int nMissionNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAccountDailyChargeEventMissionReward";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nMissionNo", SqlDbType.Int).Value = nMissionNo;
		return sc;
	}

	public static DataRow AccountConsumeEvent(SqlConnection conn, SqlTransaction trans, Guid accountId, int nEventId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AccountConsumeEvent";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@nEventId", SqlDbType.Int).Value = nEventId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static DataRowCollection AccountConsumeEventMissionRewards(SqlConnection conn, SqlTransaction trans, Guid accountId, int nEventId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AccountConsumeEventMissionRewards";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@nEventId", SqlDbType.Int).Value = nEventId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateAccountConsumeEvent(Guid accountId, int nEventId, int nAccDia)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateAccountConsumeEvent";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@nEventId", SqlDbType.Int).Value = nEventId;
		sc.Parameters.Add("@nAccDia", SqlDbType.Int).Value = nAccDia;
		return sc;
	}

	public static SqlCommand CSC_AddAccountConsumeEventMissionReward(Guid accountId, int nEventId, int nMissionNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAccountConsumeEventMissionReward";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@nEventId", SqlDbType.Int).Value = nEventId;
		sc.Parameters.Add("@nMissionNo", SqlDbType.Int).Value = nMissionNo;
		return sc;
	}

	public static DataRow AccountDailyConsumeEvent(SqlConnection conn, SqlTransaction trans, Guid accountId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AccountDailyConsumeEvent";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static DataRowCollection AccountDailyConsumeEventMissionRewards(SqlConnection conn, SqlTransaction trans, Guid accountId, DateTime date)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AccountDailyConsumeEventMissionRewards";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateAccountDailyConsumeEvent(Guid accountId, DateTime date, int nAccDia)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateAccountDailyConsumeEvent";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nAccDia", SqlDbType.Int).Value = nAccDia;
		return sc;
	}

	public static SqlCommand CSC_AddAccountDailyConsumeEventMissionReward(Guid accountId, DateTime date, int nMissionNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddAccountDailyConsumeEventMissionReward";
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@date", SqlDbType.Date).Value = date;
		sc.Parameters.Add("@nMissionNo", SqlDbType.Int).Value = nMissionNo;
		return sc;
	}

	public static DataRow HeroJobChangeQuest(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroJobChangeQuest";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		if (dt.Rows.Count <= 0)
		{
			return null;
		}
		return dt.Rows[0];
	}

	public static SqlCommand CSC_AddHeroJobChangeQuest(Guid questInstanceId, int nQuestNo, Guid heroId, int nDifficulty, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroJobChangeQuest";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nQuestNo", SqlDbType.Int).Value = nQuestNo;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDifficulty", SqlDbType.Int).Value = nDifficulty;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroJobChangeQuest_ProgressCount(Guid questInstanceId, int nProgressCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroJobChangeQuest_ProgressCount";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nProgressCount", SqlDbType.Int).Value = nProgressCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroJobChangeQuest_Status(Guid questInstanceId, int nStatus, DateTimeOffset statusUpdateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroJobChangeQuest_Status";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroJobChangeQuest_Difficulty(Guid questInstanceId, int nDifficulty)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroJobChangeQuest_Difficulty";
		sc.Parameters.Add("@questInstanceId", SqlDbType.UniqueIdentifier).Value = questInstanceId;
		sc.Parameters.Add("@nDifficulty", SqlDbType.Int).Value = nDifficulty;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_Job(Guid heroId, int nJobId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_Job";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nJobId", SqlDbType.Int).Value = nJobId;
		return sc;
	}

	public static DataRowCollection HeroPotionAttrs(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroPotionAttrs";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddOrUpdateHeroPotionAttr(Guid heroId, int nPotionAttrId, int nCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroPotionAttr";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nPotionAttrId", SqlDbType.Int).Value = nPotionAttrId;
		sc.Parameters.Add("@nCount", SqlDbType.Int).Value = nCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_AnkouTombPlay(Guid heroId, DateTime ankouTombPlayDate, int nAnkouTombPlayCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_AnkouTombPlay";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@ankouTombPlayDate", SqlDbType.Date).Value = ankouTombPlayDate;
		sc.Parameters.Add("@nAnkouTombPlayCount", SqlDbType.Int).Value = nAnkouTombPlayCount;
		return sc;
	}

	public static DataRowCollection HeroAnkouTombBestRecords(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroAnkouTombBestRecords";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AnkouTombServerBestRecords(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AnkouTombServerBestRecords";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddorUpdateHeroAnkouTombBestRecord(Guid heroId, int nDifficulty, int nPoint, DateTimeOffset updateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroAnkouTombBestRecord";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDifficulty", SqlDbType.Int).Value = nDifficulty;
		sc.Parameters.Add("@nPoint", SqlDbType.Int).Value = nPoint;
		sc.Parameters.Add("@updateTime", SqlDbType.DateTimeOffset).Value = updateTime;
		return sc;
	}

	public static DataRowCollection HeroConstellationSteps(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroConstellationSteps";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroConstellationStep(Guid heroId, int nConstellationId, int nStep, int nCycle, int nEntryNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroConstellationStep";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nConstellationId", SqlDbType.Int).Value = nConstellationId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@nCycle", SqlDbType.Int).Value = nCycle;
		sc.Parameters.Add("@nEntryNo", SqlDbType.Int).Value = nEntryNo;
		return sc;
	}

	public static SqlCommand CSC_UpdateHeroConstellationStep(Guid heroId, int nConstellationId, int nStep, int nCycle, int nEntryNo, int nFailPoint, bool bActivated)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroConstellationStep";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nConstellationId", SqlDbType.Int).Value = nConstellationId;
		sc.Parameters.Add("@nStep", SqlDbType.Int).Value = nStep;
		sc.Parameters.Add("@nCycle", SqlDbType.Int).Value = nCycle;
		sc.Parameters.Add("@nEntryNo", SqlDbType.Int).Value = nEntryNo;
		sc.Parameters.Add("@nFailPoint", SqlDbType.Int).Value = nFailPoint;
		sc.Parameters.Add("@bActivated", SqlDbType.Bit).Value = bActivated;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_StarEssense(Guid heroId, int nStarEssense)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_StarEssense";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nStarEssense", SqlDbType.Int).Value = nStarEssense;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_StarEssenseItemUseDateCount(Guid heroId, DateTime starEssenseItemUseDate, int nStarEssenseItemUseCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_StarEssenseItemUseDateCount";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@starEssenseItemUseDate", SqlDbType.Date).Value = starEssenseItemUseDate;
		sc.Parameters.Add("@nStarEssenseItemUseCount", SqlDbType.Int).Value = nStarEssenseItemUseCount;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_ArtifactLevelUp(Guid heroId, int nArtifactNo, int nArtifactLevel, int nArtifactExp)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_ArtifactLevelUp";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nArtifactNo", SqlDbType.Int).Value = nArtifactNo;
		sc.Parameters.Add("@nArtifactLevel", SqlDbType.Int).Value = nArtifactLevel;
		sc.Parameters.Add("@nArtifactExp", SqlDbType.Int).Value = nArtifactExp;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_ArtifactOpen(Guid heroId, int nArtifactNo, int nArtifactLevel, int nArtifactExp, int nEquippedArtifactNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_ArtifactOpen";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nArtifactNo", SqlDbType.Int).Value = nArtifactNo;
		sc.Parameters.Add("@nArtifactLevel", SqlDbType.Int).Value = nArtifactLevel;
		sc.Parameters.Add("@nArtifactExp", SqlDbType.Int).Value = nArtifactExp;
		sc.Parameters.Add("@nEquippedArtifactNo", SqlDbType.Int).Value = nEquippedArtifactNo;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_EquippedArtifact(Guid heroId, int nEquippedArtifactNo)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_EquippedArtifact";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nEquippedArtifactNo", SqlDbType.Int).Value = nEquippedArtifactNo;
		return sc;
	}

	public static SqlCommand CSC_UpdateHero_TradeShipPlay(Guid heroId, DateTime tradeShipPlayDate, int nTradeShipCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHero_TradeShipPlay";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@tradeShipPlayDate", SqlDbType.Date).Value = tradeShipPlayDate;
		sc.Parameters.Add("@nTradeShipPlayCount", SqlDbType.Int).Value = nTradeShipCount;
		return sc;
	}

	public static DataRowCollection HeroTradeShipBestRecords(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroTradeShipBestRecords";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TradeShipServerBestRecords(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TradeShipServerBestRecords";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddorUpdateHeroTradeShipBestRecord(Guid heroId, int nDifficulty, int nPoint, DateTimeOffset updateTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddOrUpdateHeroTradeShipBestRecord";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nDifficulty", SqlDbType.Int).Value = nDifficulty;
		sc.Parameters.Add("@nPoint", SqlDbType.Int).Value = nPoint;
		sc.Parameters.Add("@updateTime", SqlDbType.DateTimeOffset).Value = updateTime;
		return sc;
	}

	public static DataRowCollection HeroTimeDesignationEventRewards(SqlConnection conn, SqlTransaction trans, Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroTimeDesignationEventRewards";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static SqlCommand CSC_AddHeroTimeDesignationEventReward(Guid heroId, int nEventId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroTimeDesignationEventReward";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nEventId", SqlDbType.Int).Value = nEventId;
		return sc;
	}
}
