using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WarMemoryWave
{
	public const int kTargetType_AllMonster = 1;

	public const int kTargetType_TargetMonster = 2;

	private WarMemory m_warMemory;

	private int m_nNo;

	private int m_nStartDelayTime;

	private int m_nClearPoint;

	private int m_nTargetType;

	private int m_nTargetArrangeKey;

	private List<WarMemoryTransformationObject> m_transformationObjects = new List<WarMemoryTransformationObject>();

	private List<WarMemoryMonsterArrange> m_monsterArranges = new List<WarMemoryMonsterArrange>();

	public WarMemory warMemory => m_warMemory;

	public int no => m_nNo;

	public int startDelayTime => m_nStartDelayTime;

	public int clearPoint => m_nClearPoint;

	public int targetType => m_nTargetType;

	public int targetArrangeKey => m_nTargetArrangeKey;

	public List<WarMemoryTransformationObject> transformationObjects => m_transformationObjects;

	public List<WarMemoryMonsterArrange> monsterArranges => m_monsterArranges;

	public WarMemoryWave(WarMemory warMemory)
	{
		m_warMemory = warMemory;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["waveNo"]);
		m_nStartDelayTime = Convert.ToInt32(dr["startDelayTime"]);
		if (m_nStartDelayTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "시작대기시간이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nStartDelayTime = " + m_nStartDelayTime);
		}
		m_nClearPoint = Convert.ToInt32(dr["clearPoint"]);
		if (m_nClearPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "클리어점수가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nClearPoint = " + m_nClearPoint);
		}
		m_nTargetType = Convert.ToInt32(dr["targetType"]);
		if (!IsDefinedTargetType(m_nTargetType))
		{
			SFLogUtil.Warn(GetType(), "목표타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetType = " + m_nTargetType);
		}
		m_nTargetArrangeKey = Convert.ToInt32(dr["targetArrangeKey"]);
		if (m_nTargetArrangeKey < 0)
		{
			SFLogUtil.Warn(GetType(), "목표배치키가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetArrangeKey = " + m_nTargetArrangeKey);
		}
	}

	public void AddTransformationObject(WarMemoryTransformationObject transformationObject)
	{
		if (transformationObject == null)
		{
			throw new ArgumentNullException("transformationObject");
		}
		m_transformationObjects.Add(transformationObject);
	}

	public void AddMonsterArrange(WarMemoryMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange);
	}

	public static bool IsDefinedTargetType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
