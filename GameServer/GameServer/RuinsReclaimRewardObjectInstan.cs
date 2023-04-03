using System;
using ClientCommon;

namespace GameServer;

public class RuinsReclaimRewardObjectInstance : RuinsReclaimObjectInstance
{
	private RuinsReclaimObjectArrange m_arrange;

	private Vector3 m_position = Vector3.zero;

	public override int type => 1;

	public override Vector3 position => m_position;

	public override float interactionDuration => m_arrange.objectInteractionDuration;

	public RuinsReclaimObjectArrange arrange => m_arrange;

	public void Init(RuinsReclaimInstance ruinsReclaimInst, RuinsReclaimObjectArrange arrange, Vector3 position)
	{
		if (ruinsReclaimInst == null)
		{
			throw new ArgumentNullException("ruinsReclaimInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		m_position = position;
		InitObject(ruinsReclaimInst);
	}

	public override bool IsInteractionEnabledPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_position, (float)m_arrange.objectInteractionMaxRange * 1.1f + fRadius * 2f, position);
	}

	public PDRuinsReclaimRewardObjectInstance ToPDRuinsReclaimRewardObjectInstance()
	{
		PDRuinsReclaimRewardObjectInstance inst = new PDRuinsReclaimRewardObjectInstance();
		inst.instanceId = m_lnInstanceId;
		inst.arrangeNo = m_arrange.no;
		inst.position = m_position;
		return inst;
	}
}
