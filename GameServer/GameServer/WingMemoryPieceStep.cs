using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WingMemoryPieceStep
{
	private int m_nStep;

	private int m_nRequiredMemoryPieceCount;

	private Dictionary<int, WingMemoryPieceCriticalCountPool> m_criticalCountPools = new Dictionary<int, WingMemoryPieceCriticalCountPool>();

	private Dictionary<int, WingMemoryPieceSuccessFactorPool> m_successFactorPools = new Dictionary<int, WingMemoryPieceSuccessFactorPool>();

	public int step => m_nStep;

	public int requiredMemoryPieceCount => m_nRequiredMemoryPieceCount;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nStep = Convert.ToInt32(dr["step"]);
		if (m_nStep <= 0)
		{
			SFLogUtil.Warn(GetType(), "단계가 유효하지 않습니다. m_nStep = " + m_nStep);
		}
		m_nRequiredMemoryPieceCount = Convert.ToInt32(dr["requiredMemoryPieceCount"]);
		if (m_nRequiredMemoryPieceCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "필요기억조각수가 유효하지 않습니다. m_nRequiredMemoryPieceCount = " + m_nRequiredMemoryPieceCount);
		}
	}

	public void AddCriticalCountEnty(WingMemoryPieceCriticalCountPoolEntry criticalCountEntry)
	{
		if (criticalCountEntry == null)
		{
			throw new ArgumentNullException("criticalCountEntry");
		}
		WingMemoryPieceCriticalCountPool pool = GetCriticalCountPool(criticalCountEntry.type.type);
		if (pool == null)
		{
			pool = new WingMemoryPieceCriticalCountPool(criticalCountEntry.type.type);
			AddCriticalCountPool(pool);
		}
		pool.AddEnty(criticalCountEntry);
	}

	public void AddCriticalCountPool(WingMemoryPieceCriticalCountPool pool)
	{
		if (pool == null)
		{
			throw new ArgumentNullException("pool");
		}
		m_criticalCountPools.Add(pool.type, pool);
	}

	public WingMemoryPieceCriticalCountPool GetCriticalCountPool(int nType)
	{
		if (!m_criticalCountPools.TryGetValue(nType, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddSuccessFactorEntry(WingMemoryPieceSuccessFactorPoolEntry successFactorEntry)
	{
		if (successFactorEntry == null)
		{
			throw new ArgumentNullException("successFactorEntry");
		}
		WingMemoryPieceSuccessFactorPool pool = GetSuccessFactorPool(successFactorEntry.type.type);
		if (pool == null)
		{
			pool = new WingMemoryPieceSuccessFactorPool(successFactorEntry.type.type);
			AddSuccessFactorPool(pool);
		}
		pool.AddEnty(successFactorEntry);
	}

	public void AddSuccessFactorPool(WingMemoryPieceSuccessFactorPool pool)
	{
		if (pool == null)
		{
			throw new ArgumentNullException("pool");
		}
		m_successFactorPools.Add(pool.type, pool);
	}

	public WingMemoryPieceSuccessFactorPool GetSuccessFactorPool(int nType)
	{
		if (!m_successFactorPools.TryGetValue(nType, out var value))
		{
			return null;
		}
		return value;
	}
}
