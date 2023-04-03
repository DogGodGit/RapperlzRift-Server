using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class NationWarResult
{
	public const int kTopRanking = 10;

	private NationWarDeclaration m_declaration;

	private int m_nWinNationId;

	private int m_nOffenseNationId;

	private List<NationWarMember> m_offenseNationMembers = new List<NationWarMember>();

	private int m_nDefenseNationId;

	private List<NationWarMember> m_defenseNationMembers = new List<NationWarMember>();

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	public NationWarDeclaration declaration => m_declaration;

	public int winNationId => m_nWinNationId;

	public int offenseNationId => m_nOffenseNationId;

	public int defenseNationId => m_nDefenseNationId;

	public DateTimeOffset regTime => m_regTime;

	public void Init(NationWarDeclaration declaration, int nWinNationId, int nOffenseNationId, List<NationWarMember> offenseNationMembers, int nDefenseNationId, List<NationWarMember> defenseNationMembers, DateTimeOffset time)
	{
		if (declaration == null)
		{
			throw new ArgumentNullException("declaration");
		}
		if (offenseNationMembers == null)
		{
			throw new ArgumentNullException("offenseNationMembers");
		}
		if (defenseNationMembers == null)
		{
			throw new ArgumentNullException("defenseNationMembers");
		}
		m_declaration = declaration;
		m_nWinNationId = nWinNationId;
		m_nOffenseNationId = nOffenseNationId;
		m_offenseNationMembers = offenseNationMembers;
		m_nDefenseNationId = nDefenseNationId;
		m_defenseNationMembers = defenseNationMembers;
		m_regTime = time;
	}

	public NationWarMember GetNationWarMember(Guid heroId)
	{
		foreach (NationWarMember member2 in m_offenseNationMembers)
		{
			if (heroId == member2.heroId)
			{
				return member2;
			}
		}
		foreach (NationWarMember member in m_defenseNationMembers)
		{
			if (heroId == member.heroId)
			{
				return member;
			}
		}
		return null;
	}

	public List<PDNationWarRanking> GetPDOffenseNationWarRankings()
	{
		List<PDNationWarRanking> results = new List<PDNationWarRanking>();
		foreach (NationWarMember member in m_offenseNationMembers)
		{
			if (member.ranking <= 10)
			{
				results.Add(member.ToPDNationWarRanking());
				continue;
			}
			return results;
		}
		return results;
	}

	public List<PDNationWarRanking> GetPDDefenseNationWarRankings()
	{
		List<PDNationWarRanking> results = new List<PDNationWarRanking>();
		foreach (NationWarMember member in m_defenseNationMembers)
		{
			if (member.ranking <= 10)
			{
				results.Add(member.ToPDNationWarRanking());
				continue;
			}
			return results;
		}
		return results;
	}
}
