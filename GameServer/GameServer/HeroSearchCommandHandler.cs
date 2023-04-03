using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroSearchCommandHandler : InGameCommandHandler<HeroSearchCommandBody, HeroSearchResponseBody>
{
	private DataRowCollection m_drcHeroesOfNationByName;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null 입니다.");
		}
		string sSearchName = m_body.searchName;
		if (string.IsNullOrEmpty(sSearchName))
		{
			throw new CommandHandleException(1, "검색할 이름이 유효하지 않습니다. sSearchName = " + sSearchName);
		}
		if (sSearchName.Length < 2)
		{
			throw new CommandHandleException(1, "검색할 이름길이가 유효하지 않습니다.");
		}
		SFRunnableStandaloneWork work = new SFRunnableStandaloneWork();
		work.runnable = new SFAction<string>(ProcessGetHeroes, sSearchName);
		RunWork(work);
	}

	private void ProcessGetHeroes(string sSearchName)
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			m_drcHeroesOfNationByName = GameDac.HeroesOfNationByName(conn, null, m_myHero.nationId, sSearchName);
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
		List<PDSearchHero> heroes = new List<PDSearchHero>();
		foreach (DataRow dr in m_drcHeroesOfNationByName)
		{
			PDSearchHero hero = new PDSearchHero();
			hero.heroId = (Guid)SFDBUtil.ToGuid(dr["heroId"]);
			hero.name = Convert.ToString(dr["name"]);
			hero.nationId = Convert.ToInt32(dr["nationId"]);
			hero.jobId = Convert.ToInt32(dr["jobId"]);
			hero.level = Convert.ToInt32(dr["level"]);
			heroes.Add(hero);
		}
		HeroSearchResponseBody resBody = new HeroSearchResponseBody();
		resBody.heroes = heroes.ToArray();
		SendResponseOK(resBody);
	}
}
