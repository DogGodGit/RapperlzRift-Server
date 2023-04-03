using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ProofOfValorBuffBoxInstance
{
	private long m_lnInstanceId;

	private ProofOfValorBuffBoxArrange m_arrange;

	private ProofOfValorInstance m_currentPlace;

	private Vector3 m_position = Vector3.zero;

	private float m_fRotationY;

	private DateTimeOffset m_creationTime = DateTimeOffset.MinValue;

	public static readonly SFSynchronizedLongFactory instanceIdFactory = new SFSynchronizedLongFactory();

	public long instanceId => m_lnInstanceId;

	public ProofOfValorBuffBoxArrange arrange => m_arrange;

	public int arrangeId => m_arrange.id;

	public ProofOfValorBuffBox buffBox => m_arrange.buffBox;

	public ProofOfValorInstance currentPlace => m_currentPlace;

	public Vector3 position => m_position;

	public float rotationY => m_fRotationY;

	public DateTimeOffset creationTime => m_creationTime;

	public ProofOfValorBuffBoxInstance()
	{
		m_lnInstanceId = instanceIdFactory.NewValue();
	}

	public void Init(ProofOfValorInstance proofOfValorInst, ProofOfValorBuffBoxArrange arrange, DateTimeOffset time)
	{
		if (proofOfValorInst == null)
		{
			throw new ArgumentNullException("proofOfValorInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_currentPlace = proofOfValorInst;
		m_arrange = arrange;
		m_position = arrange.position;
		m_fRotationY = arrange.SelectRotationY();
		m_creationTime = time;
	}

	public bool IsBuffBoxAcquisitionRange(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_position, m_arrange.acquisitionRange * 1.1f + fRadius * 2f, position);
	}

	public PDProofOfValorBuffBoxInstance ToPDProofOfValorBuffBoxInstance()
	{
		PDProofOfValorBuffBoxInstance inst = new PDProofOfValorBuffBoxInstance();
		inst.instanceId = m_lnInstanceId;
		inst.arrangeId = arrangeId;
		inst.position = m_position;
		inst.rotationY = m_fRotationY;
		return inst;
	}
}
