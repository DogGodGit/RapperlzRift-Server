using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ProofOfValorBuffBoxArrange
{
	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private int m_nId;

	private ProofOfValorBuffBox m_buffBox;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private int m_nYRotationType;

	private float m_fYRotation;

	private float m_fAcquisitionRange;

	public int id => m_nId;

	public ProofOfValorBuffBox buffBox => m_buffBox;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public float acquisitionRange => m_fAcquisitionRange;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["arrangeId"]);
		int nBuffBoxId = Convert.ToInt32(dr["buffBoxId"]);
		if (nBuffBoxId > 0)
		{
			m_buffBox = Resource.instance.proofOfValor.GetBuffBox(nBuffBoxId);
			if (m_buffBox == null)
			{
				SFLogUtil.Warn(GetType(), "버프상자가 존재하지 않습니다. m_nId = " + m_nId + ", nBuffBoxId = " + nBuffBoxId);
			}
			else
			{
				m_buffBox.AddArrange(this);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "버프상자ID가 유효하지 않습니다. m_nId = " + m_nId + ", nBuffBoxId = " + nBuffBoxId);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		if (m_fRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "반지름이 유효하지 않습니다. m_nId = " + m_nId + ", m_fRadius = " + m_fRadius);
		}
		m_nYRotationType = Convert.ToInt32(dr["yRotationType"]);
		if (!IsDefinedYRotationType(m_nYRotationType))
		{
			SFLogUtil.Warn(GetType(), "방향타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nYRotationType = " + m_nYRotationType);
		}
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_fAcquisitionRange = Convert.ToSingle(dr["acquisitionRange"]);
		if (m_fAcquisitionRange < 0f)
		{
			SFLogUtil.Warn(GetType(), "획득반경이 유효하지 않습니다. m_nId = " + m_nId + ", m_fAcquisitionRange = " + m_fAcquisitionRange);
		}
	}

	public Vector3 SelectPosition()
	{
		return Util.SelectPoint(m_position, m_fRadius);
	}

	public float SelectRotationY()
	{
		if (m_nYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fYRotation);
		}
		return m_fYRotation;
	}

	public static bool IsDefinedYRotationType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
