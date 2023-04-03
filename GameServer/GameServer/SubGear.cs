using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GameServer;

public class SubGear
{
	private int m_nId;

	private int m_nSlotIndex;

	private List<SubGearRuneSocket> m_runeSockets = new List<SubGearRuneSocket>();

	private List<SubGearSoulstoneSocket> m_soulstoneSockets = new List<SubGearSoulstoneSocket>();

	private SubGearName[] m_names = new SubGearName[7];

	private List<SubGearLevel> m_levels = new List<SubGearLevel>();

	private int[] m_maxLevelOfGrades = new int[7];

	private List<SubGearAttr> m_attrs = new List<SubGearAttr>();

	public int id => m_nId;

	public int slotIndex => m_nSlotIndex;

	public SubGearLevel lastLevel => m_levels.LastOrDefault();

	public List<SubGearSoulstoneSocket> soulstoneSockets => m_soulstoneSockets;

	public int soulstoneSocketCount => m_soulstoneSockets.Count;

	public List<SubGearRuneSocket> runeSockets => m_runeSockets;

	public int runeSocketCount => m_runeSockets.Count;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["subGearId"]);
		m_nSlotIndex = Convert.ToInt32(dr["slotIndex"]);
	}

	public void AddRuneSocket(SubGearRuneSocket socket)
	{
		if (socket == null)
		{
			throw new ArgumentNullException("socket");
		}
		if (socket.gear != null)
		{
			throw new Exception("이미 보조장비에 추가된 룬 소켓입니다. subGearId = " + m_nId + ", socketIndex = " + socket.index);
		}
		m_runeSockets.Add(socket);
		socket.gear = this;
	}

	public SubGearRuneSocket GetRuneSocket(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_runeSockets.Count)
		{
			return null;
		}
		return m_runeSockets[nIndex];
	}

	public void AddSoulstoneSocket(SubGearSoulstoneSocket socket)
	{
		if (socket == null)
		{
			throw new ArgumentNullException("socket");
		}
		if (socket.gear != null)
		{
			throw new Exception("이미 보조장비에 추가된 소울스톤 소켓입니다. subGearId = " + m_nId + ", socketIndex = " + socket.index);
		}
		m_soulstoneSockets.Add(socket);
		socket.gear = this;
	}

	public SubGearSoulstoneSocket GetSoulstoneSocket(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_soulstoneSockets.Count)
		{
			return null;
		}
		return m_soulstoneSockets[nIndex];
	}

	public void AddName(SubGearName name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (name.gear != null)
		{
			throw new Exception("이미 보조장비에 추가된 이름입니다.");
		}
		m_names[name.grade.id - 1] = name;
		name.gear = this;
	}

	public SubGearName GetName(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_names.Length)
		{
			return null;
		}
		return m_names[nIndex];
	}

	public void AddLevel(SubGearLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		if (level.subGear != null)
		{
			throw new Exception("이미 보조장비에 추가된 레벨입니다.");
		}
		m_levels.Add(level);
		level.subGear = this;
		int nOldMaxLevel = GetMaxLevelOfGrade(level.grade);
		if (level.level > nOldMaxLevel)
		{
			SetMaxLevelOfGrade(level.grade, level.level);
		}
	}

	public SubGearLevel GetLevel(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_levels.Count)
		{
			return null;
		}
		return m_levels[nIndex];
	}

	public int GetMaxLevelOfGrade(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_maxLevelOfGrades.Length)
		{
			return 0;
		}
		return m_maxLevelOfGrades[nIndex];
	}

	private void SetMaxLevelOfGrade(int nGrade, int nLevel)
	{
		m_maxLevelOfGrades[nGrade - 1] = nLevel;
	}

	public void AddAttr(SubGearAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		if (attr.subGear != null)
		{
			throw new Exception("이미 보조장비에 추가된 속성입니다.");
		}
		m_attrs.Add(attr);
		attr.subGear = this;
	}

	public SubGearAttr GetAttr(int nAttrId)
	{
		int nIndex = nAttrId - 1;
		if (nIndex < 0 || nIndex >= m_attrs.Count)
		{
			return null;
		}
		return m_attrs[nIndex];
	}
}
