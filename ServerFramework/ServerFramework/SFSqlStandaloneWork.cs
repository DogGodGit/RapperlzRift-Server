using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ServerFramework;

public class SFSqlStandaloneWork : SFStandaloneWork
{
	private string m_sConnectionString;

	private List<SqlCommand> m_sqlCommands = new List<SqlCommand>();

	public string connectionString => m_sConnectionString;

	public SFSqlStandaloneWork(string sConnectionString)
	{
		m_sConnectionString = sConnectionString;
	}

	public void AddSqlCommand(SqlCommand sc)
	{
		if (sc == null)
		{
			throw new ArgumentNullException("sc");
		}
		m_sqlCommands.Add(sc);
	}

	public void AddSqlCommand(IEnumerable<SqlCommand> sqlCommands)
	{
		if (sqlCommands == null)
		{
			return;
		}
		foreach (SqlCommand sqlCommand in sqlCommands)
		{
			AddSqlCommand(sqlCommand);
		}
	}

	protected override void RunWork()
	{
		if (m_sqlCommands.Count == 0)
		{
			return;
		}
		SqlConnection conn = null;
		SqlTransaction trans = null;
		try
		{
			conn = SFDBUtil.OpenConnection(m_sConnectionString);
			trans = conn.BeginTransaction();
			int num = 0;
			foreach (SqlCommand sqlCommand in m_sqlCommands)
			{
				sqlCommand.Connection = conn;
				sqlCommand.Transaction = trans;
				sqlCommand.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
				sqlCommand.ExecuteNonQuery();
				num = Convert.ToInt32(sqlCommand.Parameters["ReturnValue"].Value);
				if (num != 0)
				{
					throw new Exception("DB Error : " + num + Environment.NewLine + SFUtil.TraceSqlCommand(sqlCommand));
				}
			}
			SFDBUtil.Commit(ref trans);
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Rollback(ref trans);
			SFDBUtil.Close(ref conn);
		}
	}
}
