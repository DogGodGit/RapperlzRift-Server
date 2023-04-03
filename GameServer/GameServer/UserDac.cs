using System;
using System.Data;
using System.Data.SqlClient;
using ServerFramework;

namespace GameServer;

public static class UserDac
{
	public static DataRowCollection VirtualGameServers(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_VirtualGameServers";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow User(SqlConnection conn, SqlTransaction trans, Guid userId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_User";
		sc.Parameters.Add("@userId", SqlDbType.UniqueIdentifier).Value = userId;
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

	public static SqlCommand CSC_UpdateUser_LastVirtualGameServer(Guid userId, int nLastVirtualGameServerId1, int nLastVirtualGameServerId2)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateUser_LastVirtualGameServer";
		sc.Parameters.Add("@userId", SqlDbType.UniqueIdentifier).Value = userId;
		sc.Parameters.Add("@nLastVirtualGameServerId1", SqlDbType.Int).Value = nLastVirtualGameServerId1;
		sc.Parameters.Add("@nLastVirtualGameServerId2", SqlDbType.Int).Value = nLastVirtualGameServerId2;
		return sc;
	}

	public static DataRowCollection GameServers(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GameServers";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Languages(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Languages";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SupportedLanguages(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SupportedLanguages";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow SystemSetting(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SystemSetting";
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

	public static DataRow GameConfig(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GameConfig";
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

	public static DataRowCollection Jobs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Jobs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Nations(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Nations";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int HeroNameCount_x(SqlConnection conn, SqlTransaction trans, string sName)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HeroNameCount_x";
		sc.Parameters.Add("@sName", SqlDbType.NVarChar).Value = sName;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static int AddHeroName(SqlConnection conn, SqlTransaction trans, string sName)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddHeroName";
		sc.Parameters.Add("@sName", SqlDbType.NVarChar).Value = sName;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static SqlCommand CSC_UpdateHeroName_HeroInfo(string sName, Guid heroId, int nVirtualGameServerId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateHeroName_HeroInfo";
		sc.Parameters.Add("@sName", SqlDbType.NVarChar).Value = sName;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nVirtualGameServerId", SqlDbType.Int).Value = nVirtualGameServerId;
		return sc;
	}

	public static SqlCommand CSC_DeleteHeroName(string sName)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteHeroName";
		sc.Parameters.Add("@sName", SqlDbType.NVarChar).Value = sName;
		return sc;
	}

	public static SqlCommand CSC_AddUserHero(Guid heroId, Guid userId, int nVirtualGameServerId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddUserHero";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@userId", SqlDbType.UniqueIdentifier).Value = userId;
		sc.Parameters.Add("@nVirtualGameServerId", SqlDbType.Int).Value = nVirtualGameServerId;
		return sc;
	}

	public static SqlCommand CSC_UpdateUserHero_Name(Guid heroId, string sName)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateUserHero_Name";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@sName", SqlDbType.NVarChar).Value = sName;
		return sc;
	}

	public static SqlCommand CSC_DeleteUserHero(Guid heroId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteUserHero";
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		return sc;
	}

	public static SqlCommand CSC_UpdateGameServer_CurrentUserCount(int nServerId, int nCurrentUserCount)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGameServer_CurrentUserCount";
		sc.Parameters.Add("@nServerId", SqlDbType.Int).Value = nServerId;
		sc.Parameters.Add("@nCurrentUserCount", SqlDbType.Int).Value = nCurrentUserCount;
		return sc;
	}

	public static DataRowCollection MainGears(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGears";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearTypes(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearTypes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearTiers(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearTiers";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearGrades(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearGrades";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearQualities(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearQualities";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearBaseAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearBaseAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearBaseAttrEnchantLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearBaseAttrEnchantLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearEnchantLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearEnchantLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearEnchantSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearEnchantSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearOptionAttrPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearOptionAttrPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearRefinementRecieps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearRefinementRecipes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearCategories(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearCategories";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearDisassembleResultCountPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearDisassembleResultCountPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearDisassembleResultPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearDisassembleResultPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearEnchantLevelSet(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearEnchantLevelSets";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearEnchantLevelSetAttr(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearEnchantLevelSetAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearSets(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearSets";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainGearSetAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainGearSetAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SubGears(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SubGears";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SubGearRuneSockets(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SubGearRuneSockets";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SubGearSoulstoneSockets(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SubGearSoulstoneSockets";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SubGearNames(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SubGearNames";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SubGearGrades(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SubGearGrades";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SubGearLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SubGearLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SubGearLevelQualities(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SubGearLevelQualities";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SubGearAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SubGearAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SubGearAttrValues(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SubGearAttrValues";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SubGearRuneSocketAvailableItemTypes(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SubGearRuneSocketAvailableItemTypes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SubGearSoulstoneLevelSets(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SubGearSoulstoneLevelSets";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SubGearSoulstoneLevelSetAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SubGearSoulstoneLevelSetAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ItemTypes(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ItemTypes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Items(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Items";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ItemCompositionRecipes(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ItemCompositionRecipes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ItemGrades(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ItemGrades";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Attrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Attrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AttrValues(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AttrValues";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Continents(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Continents";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ContinentMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ContinentMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Portals(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Portals";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Npcs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Npcs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ContinentTransmissionExits(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ContinentTransmissionExits";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ContinentObjects(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ContinentObjects";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ContinentObjectArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ContinentObjectArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AbnormalStates(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AbnormalStates";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AbnormalStateLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AbnormalStateLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AbnormalStateRankSkillLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AbnormalStateRankSkillLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MonsterCharacters(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MonsterCharacters";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Monsters(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Monsters";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MonsterSkills(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MonsterSkills";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MonsterOwnSkills(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MonsterOwnSkills";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MonsterSkillHits(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MonsterSkillHits";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection JobSkills(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_JobSkills";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection JobskillLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_JobskillLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection JobSkillHits(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_JobSkillHits";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection JobSkillHitAbnormalStates(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_JobSkillHitAbnormalStates";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection JobChainSkills(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_JobChainskills";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection JobChainSkillHits(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_JobChainSkillHits";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection JobSkillMasters(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_JobSkillMasters";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection JobSkillLevelMasters(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_JobSkillLevelMasters";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection JobLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_JobLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection JobLevelMasters(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_JobLevelMasters";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainQuests(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainQuests";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainQuestRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainQuestRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ExpRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ExpRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GoldRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GoldRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ItemRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ItemRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ExploitPointRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ExploitPointRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection HonorPointRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HonorPointRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildContributionPointRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildContributionPointRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildFundRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildFundRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildBuildingPointRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildBuildingPointRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildPointRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildPointRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationFundRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationFundRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection OwnDiaRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_OwnDiaRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SimpleShopProducts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SimpleShopProducts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection PaidImmediateRevivals(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_PaidImmediateRevivals";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection InventorySlotExtendRecipe(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_InventorySlotExtendRecipes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RestRewardTimes(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RestRewardTimes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection PickPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_PickPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection DropCountPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DropCountPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection DropObjectPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DropObjectPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection LevelUpRewardEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LevelUpRewardEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection LevelUpRewardItems(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LevelUpRewardItems";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection DailyAttendRewardEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DailyAttendRewardEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WeekendAttendRewardAvailableDaysOfWeek(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WeekendAttendRewardAvailableDaysOfWeek";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AccessRewardEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AccessRewardEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AccessRewardItems(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AccessRewardItems";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection VipLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_VipLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection VipLevelRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_VipLevelRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainQuestDungeons(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainQuestDungeons";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainQuestDungeonSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainQuestDungeonSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainQuestDungeonMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainQuestDungeonMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MainQuestDungeonSummons(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MainQuestDungeonSummons";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Mounts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Mounts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MountLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MountLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MountLevelMasters(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MountLevelMasters";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MountQualities(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MountQualities";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MountQualityMasters(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MountQualityMasters";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MountGears(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MountGears";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MountGearTypes(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MountGearTypes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MountGearGrades(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MountGearGrades";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MountGearQualities(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MountGearQualities";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MountGearOptionAttrPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MountGearOptionAttrPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MountGearPickBoxRecipes(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MountGearPickBoxRecipes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MountGearSlots(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MountGearSlots";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MountPotionAttrCounts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MountPotionAttrCounts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MountAwakeningLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MountAwakeningLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MountAwakeningLevelMasters(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MountAwakeningLevelMasters";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Elementals(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Elementals";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection StoryDungeons(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_StoryDungeons";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection StoryDungeonDifficulties(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_StoryDungeonDifficulties";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection StoryDungeonRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_StoryDungeonRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection StoryDungeonSweepRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_StoryDungeonSweepRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection StoryDungeonSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_StoryDungeonSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection StoryDungeonMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_StoryDungeonMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection StoryDungeonTraps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_StoryDungeonTraps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Wings(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Wings";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WingParts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WingParts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WingSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WingSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WingStepLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WingStepLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WingStepParts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WingStepParts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WingEnchantCountPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WingEnchantCountPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WingMemoryPiecesSlots(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WingMemoryPieceSlots";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WingMemoryPieceSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WingMemoryPieceSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WingMemoryPieceSlotSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WingMemoryPieceSlotSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WingMemoryPieceTypes(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WingMemoryPieceTypes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WingMemoryPieceCriticalCountPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WingMemoryPieceCriticalCountPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WingMemoryPieceSuccessFactorPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WingMemoryPieceSuccessFactorPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow ExpDungeon(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ExpDungeon";
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

	public static DataRowCollection ExpDungeonDifficulties(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ExpDungeonDifficulties";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ExpDungeonDifficultyWaves(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ExpDungeonDifficultyWaves";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ExpDungeonMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ExpDungeonMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow GoldDungeon(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GoldDungeon";
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

	public static DataRowCollection GoldDungeonDifficulties(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GoldDungeonDifficulties";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GoldDungeonSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GoldDungeonSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GoldDungeonStepWaves(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GoldDungeonStepWaves";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GoldDungeonStepMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GoldDungeonStepMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow TreatOfFarmQuest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TreatOfFarmQuest";
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

	public static DataRowCollection TreatOfFarmQuestRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TreatOfFameQuestRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TreatOfFarmQuestMissions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TreatOfFarmQuestMissions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TreatOfFarmQuestMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TreatOfFarmQuestMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CartGrades(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CartGrades";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Carts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Carts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow UndergroundMaze(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UndergroundMaze";
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

	public static DataRowCollection UndergroundMazeFloors(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UndergroundMazeFloors";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection UndergroundMazeEntrances(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UndergroundMazeEntrances";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection UndergroundMazePortals(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UndergroundMazePortals";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection UndergroundMazeMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UndergroundMazeMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection UndergroundMazeNpcs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UndergroundMazeNpcs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection UndergroundMazeNpcTransmissionEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UndergroundMazeNpcTransmissionEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow MysteryBoxQuest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MysteryBoxQuest";
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

	public static DataRowCollection MysteryBoxQuestRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MysteryBoxQuestRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MysteryBoxGrades(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MysteryBoxGrades";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MysteryBoxGradePoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MysteryBoxGradePoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow SecretLetterQuest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SecretLetterQuest";
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

	public static DataRowCollection SecretLetterQuestRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SecretLetterQuestRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SecretLetterGrades(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SecretLetterGrades";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SecretLetterGradePoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SecretLetterGradePoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow DimensionRaidQuest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DimensionRaidQuest";
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

	public static DataRowCollection DimensionRaidQuestSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DimensionRaidQuestSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection DimensionRaidQuestRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DimensionRaidQuestRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow HolyWarQuest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HolyWarQuest";
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

	public static DataRowCollection HolyWarQuestSchedules(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HolyWarQuestSchedules";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection HolyWarQuestGloryLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HolyWarQuestGloryLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection HolyWarQuestRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HolyWarQuestRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection PvpExploits(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_PvpExploits";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection BountyHunterQuests(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_BountyHunterQuests";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection BountyHunterQuestRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_BountyHunterQuestRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow FishingQuest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FishingQuest";
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

	public static DataRowCollection FishinhQuestSpots(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FishingQuestSpots";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection FishingQuestGuildTerritorySpots(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FishingQuestGuildTerritorySpots";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection FishingQuestCastingRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FishingQuestCastingRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow ArtifactRoom(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ArtifactRoom";
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

	public static DataRowCollection ArtifactRoomFloors(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ArtifactRoomFloors";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ArtifactRoomMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ArtifactRoomMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SeriesMissions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SeriesMissions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SeriesMissionSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SeriesMissionSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SeriesMissionStepRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SeriesMissionStepRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TodayMissions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TodayMissions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TodayMissionRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TodayMissionRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TodayTasks(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TodaydayTasks";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AchievementRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AchievementRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AchievementRewardEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AchievementRewardEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow AncientRelic(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AncientRelic";
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

	public static DataRowCollection AncientRelicMonsterAttrFactors(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AncientRelicMonsterAttrFactors";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AncientRelicRoutes(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AncientRelicRoutes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AncientRelicTraps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AncientRelicTraps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AncientRelicSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AncientRelicSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AncientRelicStepRoutes(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AncientRelicStepRoutes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AncientRelicStepRewardPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AncientRelicStepRewardPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AncientRelicStepWaves(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AncientRelicStepWaves";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AncientRelicMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AncientRelicMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Ranks(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Ranks";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RankAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RankAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RankRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RankRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RankActiveSkills(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RankActiveSkills";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RankActiveSkillLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RankActiveSkillLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RankPassiveSkills(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RankPassiveSkills";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RankPassiveSkillAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RankPassiveSkillAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RankPassiveSkillLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RankPassiveSkillLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RankPassiveSkillAttrLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RankPassiveSkillAttrLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection HonorShopProducts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_HonorShopProducts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow DimensionInfiltrationEvent(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DimensionInfiltrationEvent";
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

	public static DataRow BattlefieldSupportEvent(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_BattlefieldSupportEvent";
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

	public static DataRowCollection LevelRankingRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LevelRankingRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AttainmentEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AttainmentEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AttainmentEntryRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AttainmentEntryRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow FieldOfHonor(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonor";
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

	public static DataRowCollection FieldOfHonorLevelRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonorLevelRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection FieldOfHonorRankingRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonorRankingRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection FieldOfHonorTargets(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldOfHonorTargets";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int GuildNameCount_x(SqlConnection conn, SqlTransaction trans, string sName)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildNameCount_x";
		sc.Parameters.Add("@sName", SqlDbType.NVarChar).Value = sName;
		return Convert.ToInt32(sc.ExecuteScalar());
	}

	public static int AddGuildName(SqlConnection conn, SqlTransaction trans, string sName)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddGuildName";
		sc.Parameters.Add("@sName", SqlDbType.NVarChar).Value = sName;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static SqlCommand CSC_UpdateGuildName_GuildInfo(string sName, Guid guildId, int nVirtualGameServerId)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdateGuildName_GuildInfo";
		sc.Parameters.Add("@sName", SqlDbType.NVarChar).Value = sName;
		sc.Parameters.Add("@guildId", SqlDbType.UniqueIdentifier).Value = guildId;
		sc.Parameters.Add("@nVirtualGameServerId", SqlDbType.Int).Value = nVirtualGameServerId;
		return sc;
	}

	public static SqlCommand CSC_DeleteGuildName(string sName)
	{
		SqlCommand sc = new SqlCommand();
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DeleteGuildName";
		sc.Parameters.Add("@sName", SqlDbType.NVarChar).Value = sName;
		return sc;
	}

	public static DataRowCollection GuildLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildMemberGrades(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildMemberGrades";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildDonationEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildDonationEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow SupplySupportQuest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SupplySupportQuest";
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

	public static DataRowCollection SupplySupportQuestOrders(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SupplySupportQuestOrders";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SupplySupportQuestCartPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SupplySupportQuestCartPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SupplySupportQuestWayPoints(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SupplySupportQuestWayPoints";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SupplySupportQuestCarts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SupplySupportQuestCarts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SupplySupportQuestChangeCartPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SupplySupportQuestChangeCartPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SupplySupportQuestRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SupplySupportQuestRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection BanWords(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_BanWords";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow GuildTerritory(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildTerritory";
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

	public static DataRowCollection GuildTerritoryNpcs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildTerritoryNpcs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow GuildFarmQuest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildFarmQuest";
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

	public static DataRowCollection GuildFarmQuestRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildFarmQuestRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
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

	public static DataRowCollection GuildBuildingLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildBuildingLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildSkills(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildSkills";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildSkillLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildSkillLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildSkillAttrValues(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildSkillLevelAttrValues";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow GuildMissionQuest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildMissionQuest";
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

	public static DataRowCollection GuildMissions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildMissions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildMissionQuestRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildMissionQuestRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow GuildFoodWarehouse(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildFoodWarehouse";
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

	public static DataRowCollection GuildFoodWarehouseLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildFoodWarehouseLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildFoodWarehouseStockRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildFoodWarehouseStockRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow GuildAltar(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildAltar";
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

	public static DataRowCollection GuildAltarDefenseMonsterAttrFactors(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildAltarDefenseMonsterAttrFactors";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildAltarRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildAltarRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow GuildSupplySupportQuest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildSupplySupportQuest";
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

	public static DataRowCollection GuildSupplySupportQuestRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildSupplySupportQuestRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildBlessingBuffs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildBlessingBuffs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationNoblesses(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationNoblesses";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationNoblesseAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationNoblesseAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationNoblesseAppointmentAuthorities(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationNoblesseAppointmentAuthorities";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow NationWar(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWar";
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

	public static DataRowCollection NationWarNpcs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWarNpcs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationWarTransmissionExits(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWarTransmissionExits";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationWarPaidTransmissions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWarPaidTransmissions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationWarHeroObjectiveEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWarHeroObjectiveEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationWarExpRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWarExpRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationWarAvailableDayOfWeeks(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWarAvailableDayOfWeeks";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SystemNationWarDeclarations(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SystemNationWarDeclarations";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationWarRevivalPoints(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWarRevivalPoints";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationWarMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWarMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationWarRevivalPointActivationConditions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWarRevivalPointActivationConditions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationWarRankingRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWarRankingRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationWarPointRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationWarPointRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NationDonationEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NationDonationEntires";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow GuildHuntingQuest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildHuntingQuest";
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

	public static DataRowCollection GuildHuntingQuestObjectives(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildHuntingQuestObjectives";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow SoulCoveter(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SoulCoveter";
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

	public static DataRowCollection SoulCoveterDifficulties(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SoulCoveterDifficulties";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SoulCoveterDifficultyRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SoulCoveterDifficultyRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SoulCoveterDifficultyWaves(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SoulCoveterDifficultyWaves";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SoulCoveterMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SoulCoveterMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildContents(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildContents";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildDailyObjectiveRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildDailyObjectiveRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GuildWeeklyObjectives(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GuildWeeklyObjectives";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection IllustratedBookCategories(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_IllustratedBookCategories";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection IllustratedBookTypes(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_IllustratedBookTypes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection IllustratedBooks(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_IllustratedBooks";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection IllustratedBookAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_IllustratedBookAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection IllustratedBookExplorationSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_IllustratedBookExplorationSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection IllustratedBookExplorationStepAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_IllustratedBookExplorationStepAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection IllustratedBookExplorationStepRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_IllustratedBookExplorationStepRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SceneryQuests(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SceneryQuests";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Accomplishments(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Accomplishment";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TitleTypes(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TitleTypes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Titles(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Titles";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TitleActiveAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TitleActiveAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TitlePassiveAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TitlePassiveAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureCardGrades(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureCardGrade";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureCards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureCards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureCardCollectionCategories(SqlConnection conn, SqlTransaction trnas)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trnas;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureCardCollectionCategories";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureCardCollectionGrades(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureCardCollectionGrades";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureCardCollections(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureCardCollections";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureCardCollectionEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureCardCollectionEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureCardCollectionAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureCardCollectionAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureCardShopRefreshSchedules(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureCardShopRefreshSchedules";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureCardShopFixedProducts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureCardShopFixedProducts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureCardShopRandomProducts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureCardShopRandomProducts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection EliteMonsterCategories(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_EliteMonsterCategories";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection EliteMonsterMasters(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_EliteMonsterMasters";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection EliteMonsterSpawnSchedules(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_EliteMonsterSpawnSchedules";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection EliteMonsters(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_EliteMonsters";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection EliteMonsterKillAttrValues(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_EliteMonsterKillAttrValues";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow EliteDungeon(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_EliteDungeon";
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

	public static DataRowCollection StaminaBuyCounts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_StaminaBuyCounts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection StaminaRecoverySchedule(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_StaminaRecoverySchedule";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow ProofOfValor(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ProofOfValor";
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

	public static DataRowCollection ProofOfValorBuffBoxs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ProofOfValorBuffBoxs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ProofOfValorBuffBoxArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ProofOfValorBuffBoxArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ProofOfValorMonsterAttrFactors(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ProofOfValorMonsterAttrFactors";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ProofOfValorPaidRefreshs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ProofOfValorPaidRefreshs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ProofOfValorCreatureCardPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ProofOfValorCreatureCardPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ProofOfValorBossMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ProofOfValorBossMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ProofOfValorNormalMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ProofOfValorNormalMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ProofOfValorRefreshSchedules(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ProofOfValorRefreshSchedules";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ProofOfValorRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ProofOfValorRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ProofOfValorClearGrades(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ProofOfValorClearGrades";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection GlobalNotices(SqlConnection conn, SqlTransaction trans, DateTime fromTime, DateTime toTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_GlobalNotices";
		sc.Parameters.Add("@fromTime", SqlDbType.DateTime).Value = fromTime;
		sc.Parameters.Add("@toTime", SqlDbType.DateTime).Value = toTime;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MonsterKillExpFactors(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MonsterkillExpFactors";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WorldLevelExpFactors(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WorldLevelExpFactors";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection PartyExpFactors(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_PartyExpFactors";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection JobCommonSkills(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_JobCommonSkills";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NpcShops(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NpcShops";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NpcShopCategories(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NpcShopCategories";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection NpcShopProducts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_NpcShopProducts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RookieGifts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RookieGifts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RookieGiftRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RookieGiftRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection OpenGiftRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_OpenGiftRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow DailyQuest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DailyQuest";
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

	public static DataRowCollection DailyQuestRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DailyQuestRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection DailyQuestGrades(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DailyQuestGrades";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection DailyQuestMissions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DailyQuestMissions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow WeeklyQuest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WeeklyQuest";
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

	public static DataRowCollection WeeklyQuestRoundRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WeeklyQuestRoundRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WeeklyQuestTenRoudRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WeeklyQuestTenRoundRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WeeklyQuestMissions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WeeklyQuestMissions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow WisdomTemple(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTemple";
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

	public static DataRowCollection WisdomTempleMonsterAttrFactors(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTempleMonsterAttrFactors";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WisdomTempleColorMatchingObjects(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTempleColorMatchingObjects";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WisdomTempleArrangePositions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTempleArrangePositions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WisdomTempleSweepRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTempleSweepRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WisdomTemplePuzzleRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTemplePuzzleRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WisdomTempleSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTempleSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WisdomTempleQuizMonsterPositions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTempleQuizMonsterPositions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WisdomTempleQuizPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTempleQuizPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WisdomTempleQuizRightAnswerPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTempleQuizRightAnswerPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WisdomTempleQuizWrongAnswerPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTempleQuizWrongAnswerPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WisdomTemplePuzzles(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTemplePuzzles";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WisdomTempleStepRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTempleStepRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WisdomTempleFindTreasureBoxCounts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTempleFindTreasureBoxCounts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WisdomTemplePuzzleRewardObjectOffsets(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTemplePuzzleRewardObjectOffsets";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WisdomTemplePuzzleRewardPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WisdomTemplePuzzleRewardPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Open7DayEventDays(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Open7DayEventDays";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Open7DayEventProducts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Open7DayEventProducts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Open7DayEventMissions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Open7DayEventMissions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Open7DayEventMissionRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Open7DayEventMissionRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Retrievals(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Retrievals";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RetrievalRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RetrivalRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TaskConsignments(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TaskConsignments";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TaskConsignmentExpRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TaskConsignmentExpRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TaskConsignmentItemRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TaskConsignmentItemRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow RuinsReclaim(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaim";
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

	public static DataRowCollection RuinsReclaimMonsterAttrFactors(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimMonsterAttrFactors";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimRevivalPoints(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimRevivalPoints";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimTraps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimTraps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimPortals(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimPortals";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimOpenSchedules(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimOpenSchedules";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimObjectArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimObjectArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimStepRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimStepRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimStepWaves(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimStepWaves";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimStepWaveSkills(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimStepWaveSkills";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimSummonMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimSummonMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimRandomRewardPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimRandomRewardPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimMonsterTerminatorRewardPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimMonsterTerminatorRewardPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimUltimateAttackKingRewardPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimUltimateAttackKingRewardPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection RuinsReclaimPartyVolunteerRewardPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RuinsReclaimPartyVolunteerRewardPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow TrueHeroQuest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TrueHeroQuest";
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

	public static DataRowCollection TrueHeroQuestSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TrueHeroQuestSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TrueHeroQuestRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TrueHeroQuestRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow InfiniteWar(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_InfiniteWar";
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

	public static DataRowCollection InfiniteWarBuffBoxs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_InfiniteWarBuffBoxs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection InfiniteWarMonsterAttrFactors(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_InfiniteWarMonsterAttrFactors";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection InfiniteWarMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_InfiniteWarMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection InfiniteWarOpenSchedules(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_InfiniteWarOpenSchedules";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection InfiniteWarStartPositions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_InfiniteWarStartPositions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection InfiniteWarRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_InfiniteWarRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection InfiniteWarRankingRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_InfiniteWarRankingRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow FieldBossEvent(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldBossEvent";
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

	public static DataRowCollection FieldBossEventSchedules(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldBossEventSchedules";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection FieldBosses(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FieldBosses";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow LimitationGift(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LimitationGift";
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

	public static DataRowCollection LimitationGiftRewardDayOfWeeks(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LimitationGiftRewardDayOfWeeks";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection LimitationGiftRewardSchedules(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LimitationGiftRewardSchedules";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection LimitationGiftRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_LimitationGiftRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow WeekendReward(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WeekendReward";
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

	public static DataRowCollection WeekendRewardNumberPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WeekendRewardNumberPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WarehouseSlotExtendRecipes(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WarehouseSlotExtendRecipes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection DiaShopCategories(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DiaShopCategories";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection DiaShopProducts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DiaShopProducts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow FearAltar(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FearAltar";
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

	public static DataRowCollection FearAltarMonsterAttrFactors(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FearAltarMonsterAttrFactors";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection FearAltarRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FearAltarRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection FearAltarHalidomCollectionRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FearAltarHalidomCollectionRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection FearAltarHalidomElementals(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FearAltarHalidomElementals";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection FearAltarHalidomLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FearAltarHalidomLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection FearAltarHalidoms(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FearAltarHalidoms";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection FearAltarStages(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FearAltarStages";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection FearAltarStageWaves(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FearAltarStageWaves";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection FearAltarStageWaveMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FearAltarStageWaveMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SubQuests(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SubQuests";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SubQuestRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SubQuestRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow WarMemory(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WarMemory";
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

	public static DataRowCollection WarMemoryMonsterAttrFactors(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WarMemoryMonsterAttrFactors";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WarMemoryStartPositions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WarMemoryStartPositions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WarMemorySchedules(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WarMemorySchedules";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WarMemoryRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WarMemoryRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WarMemoryRankingRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WarMemoryRankingRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WarMemoryWaves(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WarMemoryWaves";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WarMemoryTransformationObjects(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WarMemoryTransformationObjects";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WarMemoryMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WarMemoryMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WarMemorySummonMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WarMemorySummonMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection OrdealQuests(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_OrdealQuests";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection OrdealQuestMissions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_OrdealQuestMissions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow OsirisRoom(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_OsirisRoom";
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

	public static DataRowCollection OsirisRoomDifficulties(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_OsirisRoomDifficulties";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection OsirisRoomDifficultyWaves(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_OsirisRoomDifficultyWaves";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection OsirisRoomMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_OsirisRoomMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MoneyBuffs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MoneyBuffs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection MoneyBuffAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_MoneyBuffAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Biographies(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Biograpies";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection BioGraphyRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_BiographyRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection BiographyQuests(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_BiographyQuests";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection BiographyQuestDungeons(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_BiographyQuestDungeons";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection BiographyQuestDungeonWaves(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_BiographyQuestDungeonWaves";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection BiographyQuestMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_BiogarphyQuestMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow ItemLuckyShop(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ItemLuckyShop";
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

	public static DataRowCollection ItemLuckyShopNormalPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ItemLuckyShopNormalPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ItemLuckyShopSpecialPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ItemLuckyShopSpecialPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow CreatureCardLuckyShop(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureCardLuckyShop";
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

	public static DataRowCollection CreatureCardLuckyShopNormalPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureCardLuckyShopNormalPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureCardLuckyShopSpecialPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureCardLuckyShopSpecialPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Blessings(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Blessings";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection BlessingTargetLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_BlessingTargetLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ProspectQuestOwnerRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ProspectQuestOwnerRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ProspectQuestTargetRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ProspectQuestTargetRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureCharacters(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CretureCharacters";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureCharacterSkillPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureCharacterSkillPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureSkills(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureSkills";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureSkillAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureSkillAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureSkilllGrades(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureSkillGrades";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureSkillCountPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureSkillCountPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Creatures(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Creatures";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureGrades(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureGrades";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureBaseAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureBaseAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureBaseAttrValues(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureBaseAttrValues";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureSkillSlotOpenRecipes(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureSkillSlotOpenRecipes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureSkillSlotProtections(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureSkillSlotProtections";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureAdditionalAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureAdditionalAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureInjectionLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureInjectionLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureAdditionalAttrValues(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureAdditionalAttrValues";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureInjectionLevelUpEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureInjectionLevelUpEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow DragonNest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DragonNest";
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

	public static DataRowCollection DragonNestMonsterAttrFactors(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DragonNestMonsterAttrFactors";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection DragonNestTraps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DragonNestTraps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection DragonNestSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DragonNestSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection DragonNestStepReward(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DragonNestStepRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection DragonNestMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DragonNestMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Presents(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Presents";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WeeklyPresentPopularityPointRankingRewardGroups(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WeeklyPresentPopularityPointRankingRewardGroups";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WeeklyPresentPopularityPointRankingRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WeeklyPresentPopularityPointRankingRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WeeklyPresentContributionPointRankingRewardGroups(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WeeklyPresentContributionPointRankingRewardGroups";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection WeeklyPresentContributionPointRankingRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_WeeklyPresentContributionPointRankingRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Costumes(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Costumes";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CostumeEffects(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CostumeEffects";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CostumeCollections(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CostumeCollections";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CostumeCollectionAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CostumeCollectionAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CostumeCollectionEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CostumeCollectionEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CostumeEnchantLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CostumeEnchantLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CostumeAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CostumeAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CostumeEnchantLevelAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CostumeEnchantLevelAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow CreatureFarmQuest(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureFarmQuest";
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

	public static DataRowCollection CreatureFarmQuestExpRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureFarmQuestExpRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureFarmQuestItemReawrds(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureFarmQuestItemRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureFarmQuestMissions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureFarmQuestMissions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureFarmQuestMissionMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureFarmQuestMissionMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection CreatureFarmQuestMissionRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CreatureFarmQuestMissionRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow SafeTimeEvent(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SafeTimeEvent";
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

	public static DataRowCollection CashProducts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_CashProducts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static int AddPurchase(SqlConnection conn, SqlTransaction trans, Guid purchaseId, Guid userId, int nVirtualGameServerId, Guid accountId, Guid heroId, int nProductId, int nStoreType, DateTimeOffset regTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AddPurchase";
		sc.Parameters.Add("@purchaseId", SqlDbType.UniqueIdentifier).Value = purchaseId;
		sc.Parameters.Add("@userId", SqlDbType.UniqueIdentifier).Value = userId;
		sc.Parameters.Add("@nVirtualGameServerId", SqlDbType.Int).Value = nVirtualGameServerId;
		sc.Parameters.Add("@accountId", SqlDbType.UniqueIdentifier).Value = accountId;
		sc.Parameters.Add("@heroId", SqlDbType.UniqueIdentifier).Value = heroId;
		sc.Parameters.Add("@nProductId", SqlDbType.Int).Value = nProductId;
		sc.Parameters.Add("@nStoreType", SqlDbType.Int).Value = nStoreType;
		sc.Parameters.Add("@regTime", SqlDbType.DateTimeOffset).Value = regTime;
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static DataRow Purchase_x(SqlConnection conn, SqlTransaction trans, Guid purchaseId)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Purchase_x";
		sc.Parameters.Add("@purchaseId", SqlDbType.UniqueIdentifier).Value = purchaseId;
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

	public static int UpdatePurchase(SqlConnection conn, SqlTransaction trans, Guid purchaseId, int nStatus, DateTimeOffset statusUpdateTime, string sFailReason)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_UpdatePurchase";
		sc.Parameters.Add("@purchaseId", SqlDbType.UniqueIdentifier).Value = purchaseId;
		sc.Parameters.Add("@nStatus", SqlDbType.Int).Value = nStatus;
		sc.Parameters.Add("@statusUpdateTime", SqlDbType.DateTimeOffset).Value = statusUpdateTime;
		sc.Parameters.Add("@sFailReason", SqlDbType.NVarChar).Value = SFDBUtil.NullToDBNull(sFailReason);
		sc.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
		sc.ExecuteNonQuery();
		return Convert.ToInt32(sc.Parameters["ReturnValue"].Value);
	}

	public static DataRow FirstChargeEvent(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FirstChargeEvent";
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

	public static DataRowCollection FirstChargeEventRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_FirstChargeEventRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow RechargeEvent(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RechargeEvent";
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

	public static DataRowCollection RechargeEventRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_RechargeEventRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ChargeEvents(SqlConnection conn, SqlTransaction trans, DateTime currentTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ChargeEvents";
		sc.Parameters.Add("@currentTime", SqlDbType.DateTime).Value = currentTime;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ChargeEventMissions(SqlConnection conn, SqlTransaction trans, DateTime currentTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ChargeEventMissions";
		sc.Parameters.Add("@currentTime", SqlDbType.DateTime).Value = currentTime;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ChargeEventMissionRewards(SqlConnection conn, SqlTransaction trans, DateTime currentTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ChargeEventMissionRewards";
		sc.Parameters.Add("@currentTime", SqlDbType.DateTime).Value = currentTime;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow DailyChargeEvent(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DailyChargeEvent";
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

	public static DataRowCollection DailyChargeEventMissions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DailyChargeEventMissions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection DailyChargeEventMissionRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DailyChargeEventMissionRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ConsumeEvents(SqlConnection conn, SqlTransaction trans, DateTime currentTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ConsumeEvents";
		sc.Parameters.Add("@currentTime", SqlDbType.DateTime).Value = currentTime;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ConsumeEventMissions(SqlConnection conn, SqlTransaction trans, DateTime currentTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ConsumeEventMissions";
		sc.Parameters.Add("@currentTime", SqlDbType.DateTime).Value = currentTime;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ConsumeEventMissionRewards(SqlConnection conn, SqlTransaction trans, DateTime currentTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ConsumeEventMissionRewards";
		sc.Parameters.Add("@currentTime", SqlDbType.DateTime).Value = currentTime;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow DailyConsumeEvent(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DailyConsumeEvent";
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

	public static DataRowCollection DailyConsumeEventMissions(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DailyConsumeEventMissions";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection DailyConsumeEventMissionRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_DailyConsumeEventMissionRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection JobChangeQuests(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_JobChangeQuests";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection JobChangeQuestDifficulties(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_JobChangeQuestDifficulties";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection PotionAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_PotionAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow AnkouTomb(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AnkouTomb";
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

	public static DataRowCollection AnkouTombSchedules(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AnkouTombSchedules";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AnkouTombDifficulty(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AnkouTombDifficulty";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AnkouTombRewardPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AnkouTombRewardPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection AnkouTombMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_AnkouTombMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Constellations(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Constellations";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ConstellationSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ConstellationSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ConstellationCycles(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ConstellationCycles";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ConstellationCycleBuffs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ConstellationCycleBuffs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ConstellationEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ConstellationEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ConstellationEntryBuffs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ConstellationEntryBuffs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection Artifacts(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_Artifacts";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ArtifactAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ArtifactAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ArtifactLevels(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ArtifactLevels";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ArtifactLevelAttrs(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ArtifactLevelAttrs";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection ArtifactLevelUpMaterials(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_ArtifactLevelUpMaterials";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRow TradeShip(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TradeShip";
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

	public static DataRowCollection TradeShipSchedules(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TradeShipSchedules";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TradeShipSteps(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TradeShipSteps";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TradeShipDifficulties(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TradeShipDifficulties";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TradeShipRewardPoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TradeShipRewardPoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TradeShipMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TradeShipMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TradeShipAdditionalMonsterArrangePoolEntries(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TradeShipAdditionalMonsterArrangePoolEntries";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TradeShipAdditionalMonsterArranges(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TradeShipAdditionalMonsterArranges";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TradeShipObjects(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TradeShipObjects";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TradeShipObjectDestroyerRewards(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TradeShipObjectDestroyerRewards";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection SystemMessages(SqlConnection conn, SqlTransaction trans)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_SystemMessages";
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TimeDesignationEvents(SqlConnection conn, SqlTransaction trans, DateTime currentTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TimeDesignationEvents";
		sc.Parameters.Add("@currentTime", SqlDbType.DateTime).Value = currentTime;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}

	public static DataRowCollection TimeDesignationEventRewards(SqlConnection conn, SqlTransaction trans, DateTime currentTime)
	{
		SqlCommand sc = new SqlCommand();
		sc.Connection = conn;
		sc.Transaction = trans;
		sc.CommandType = CommandType.StoredProcedure;
		sc.CommandText = "uspPGApi_TimeDesignationEventRewards";
		sc.Parameters.Add("@currentTime", SqlDbType.DateTime).Value = currentTime;
		DataTable dt = new DataTable();
		SqlDataAdapter sda = new SqlDataAdapter();
		sda.SelectCommand = sc;
		sda.Fill(dt);
		return dt.Rows;
	}
}
