using System;

namespace GameServer;

public class ProofOfValorEnterParam : PlaceEntranceParam
{
	private DateTimeOffset m_enterTime = DateTimeOffset.MinValue;

	public DateTimeOffset enterTime => m_enterTime;

	public ProofOfValorEnterParam(DateTimeOffset enterTime)
	{
		m_enterTime = enterTime;
	}
}
