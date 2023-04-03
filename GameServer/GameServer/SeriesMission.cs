using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class SeriesMission
{
	public const int kId_FishingQuest = 1;

	public const int kId_BountyHunterQuest = 2;

	public const int kId_OsirisRoomDungeon = 3;

	public const int kId_ExpDungeon = 4;

	public const int kId_FieldOfHonor = 5;

	public const int kId_DimensionRaidQuest = 7;

	public const int kId_MysteryBoxQuest = 8;

	public const int kId_SupplySupportQuest = 9;

	private int m_nId;

	private string m_sNameKey;

	private List<SeriesMissionStep> m_steps = new List<SeriesMissionStep>();

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public SeriesMissionStep lastStep => m_steps.LastOrDefault();

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["missionId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "미션ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
	}

	public void AddStep(SeriesMissionStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_steps.Add(step);
		step.mission = this;
	}

	public SeriesMissionStep GetStep(int nStep)
	{
		int nIndex = nStep - 1;
		if (nIndex < 0 || nIndex >= m_steps.Count)
		{
			return null;
		}
		return m_steps[nIndex];
	}
}
