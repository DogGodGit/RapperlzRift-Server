using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public abstract class Location
{
	private bool m_bAccelerationEnabled;

	public abstract int locationId { get; }

	public abstract LocationType locationType { get; }

	public abstract bool mountRidingEnabled { get; }

	public abstract bool hpPotionUseEnabled { get; }

	public abstract bool returnScrollUseEnabled { get; }

	public abstract bool evasionCastEnabled { get; }

	public bool accelerationEnabled => m_bAccelerationEnabled;

	public virtual void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		if (dr["accelerationEnabled"] == DBNull.Value)
		{
			SFLogUtil.Warn(GetType(), "가속사용여부가 존재하지 않습니다. locationId = " + dr["locationId"]);
		}
		else
		{
			m_bAccelerationEnabled = Convert.ToBoolean(dr["accelerationEnabled"]);
		}
	}
}
