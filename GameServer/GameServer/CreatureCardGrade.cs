using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureCardGrade
{
	public const int kGrade_White = 1;

	public const int kGrade_Green = 2;

	public const int kGrade_Blue = 3;

	public const int kGrade_Purple = 4;

	public const int kGrade_Orange = 5;

	public const int kCount = 5;

	private int m_nId;

	private int m_nSaleSoulPowder;

	private int m_nDisassembleSoulPowder;

	private int m_nCompositionSoulPowder;

	public int id => m_nId;

	public int saleSoulPowder => m_nSaleSoulPowder;

	public int disassembleSoulPowder => m_nDisassembleSoulPowder;

	public int compositionSoulPowder => m_nCompositionSoulPowder;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["grade"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "등급이 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nSaleSoulPowder = Convert.ToInt32(dr["saleSoulPowder"]);
		if (m_nSaleSoulPowder <= 0)
		{
			SFLogUtil.Warn(GetType(), "판매영혼가루가 유효하지 않습니다. m_nId = " + m_nId + ", m_nSaleSoulPowder = " + m_nSaleSoulPowder);
		}
		m_nDisassembleSoulPowder = Convert.ToInt32(dr["disassembleSoulPowder"]);
		if (m_nDisassembleSoulPowder <= 0)
		{
			SFLogUtil.Warn(GetType(), "분해영혼가루가 유효하지 않습니다. m_nId = " + m_nId + ", m_nDisassembleSoulPowder = " + m_nDisassembleSoulPowder);
		}
		m_nCompositionSoulPowder = Convert.ToInt32(dr["compositionSoulPowder"]);
		if (m_nCompositionSoulPowder <= 0)
		{
			SFLogUtil.Warn(GetType(), "합성영혼가루가 유효하지 않습니다. m_nId = " + m_nId + ", m_nCompositionSoulPowder = " + m_nCompositionSoulPowder);
		}
	}
}
