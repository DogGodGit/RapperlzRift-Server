using System;

namespace GameServer;

public class RuinsReclaimDebuffArea
{
	private RuinsReclaim m_ruinsReclaim;

	private RuinsReclaimInstance m_currentPlace;

	private Vector3 m_position = Vector3.zero;

	private float m_fRotationY;

	private int m_nWidth;

	private int m_nHeight;

	private float m_fDebuffOffenseFactor;

	public RuinsReclaim ruinsReclaim => m_ruinsReclaim;

	public RuinsReclaimInstance currnetPlace => m_currentPlace;

	public Vector3 position => m_position;

	public float rotationY => m_fRotationY;

	public int width => m_nWidth;

	public int height => m_nHeight;

	public void Init(RuinsReclaimInstance ruinsReclaimInst)
	{
		if (ruinsReclaimInst == null)
		{
			throw new ArgumentNullException("ruinsReclaimInst");
		}
		m_ruinsReclaim = ruinsReclaimInst.ruinsReclaim;
		m_currentPlace = ruinsReclaimInst;
		m_position = m_ruinsReclaim.debuffAreaPosition;
		m_fRotationY = m_ruinsReclaim.debuffAreaYRotation;
		m_nWidth = m_ruinsReclaim.debuffAreaWidth;
		m_nHeight = m_ruinsReclaim.debuffAreaHeight;
		m_fDebuffOffenseFactor = m_ruinsReclaim.debuffAreaOffenseFactor;
	}

	public bool Contains(Vector3 targetPosition)
	{
		Vector3 revisionTargetPosition = MathUtil.PositionRotation(targetPosition - m_position, m_fRotationY);
		return MathUtil.SquareContains(Vector3.zero, m_nWidth, m_nHeight, revisionTargetPosition);
	}
}
