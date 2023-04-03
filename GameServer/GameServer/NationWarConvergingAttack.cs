using System;

namespace GameServer;

public class NationWarConvergingAttack
{
	private NationWarInstance m_nationWarInst;

	private int m_nTargetArrangeId;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	public NationWarInstance nationWarInst => m_nationWarInst;

	public int targetArrangeId => m_nTargetArrangeId;

	public DateTimeOffset regTime => m_regTime;

	public NationWarConvergingAttack(NationWarInstance nationWarInst, int nTargetArrangeId, DateTimeOffset regTime)
	{
		m_nationWarInst = nationWarInst;
		m_nTargetArrangeId = nTargetArrangeId;
		m_regTime = regTime;
	}
}
