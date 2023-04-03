using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Monster
{
	public const float kStealRadiusFactor = 1.1f;

	private int m_nId;

	private MonsterCharacter m_monsterCharacter;

	private string m_sNameKey;

	private int m_nLevel;

	private float m_fRadius;

	private int m_nMoveSpeed;

	private int m_nBattleMoveSpeed;

	private float m_fVisibilityRange;

	private float m_fActiveAreaRadius;

	private float m_fQuestAreaRadius;

	private float m_fReturnCompletionRadius;

	private float m_fSkillCastingInterval;

	private int m_nMaxHP;

	private int m_nPhysicalOffense;

	private DropCountPool m_dropCountPool;

	private DropObjectPool m_dropObjectPool;

	private int m_nExp;

	private bool m_bMoveEnabled;

	private bool m_bAttackEnabled;

	private bool m_bTamingEnabled;

	private int m_nMentalStrength;

	private float m_fStealRadius;

	private int m_nStealSuccessRate;

	private ItemReward m_stealItemReward;

	private List<MonsterOwnSkill> m_ownSkills = new List<MonsterOwnSkill>();

	public int id => m_nId;

	public MonsterCharacter monsterCharacter => m_monsterCharacter;

	public string nameKey => m_sNameKey;

	public int level => m_nLevel;

	public float radius => m_fRadius;

	public int moveSpeed => m_nMoveSpeed;

	public float visibilityRange => m_fVisibilityRange;

	public float activeAreaRadius => m_fActiveAreaRadius;

	public float questAreaRadius => m_fQuestAreaRadius;

	public float returnCompletionRadius => m_fReturnCompletionRadius;

	public int battleMoveSpeed => m_nBattleMoveSpeed;

	public float skillCastingInterval => m_fSkillCastingInterval;

	public int maxHP => m_nMaxHP;

	public int physicalOffense => m_nPhysicalOffense;

	public DropCountPool dropCountPool => m_dropCountPool;

	public DropObjectPool dropObjectPool => m_dropObjectPool;

	public int exp => m_nExp;

	public bool moveEnabled => m_bMoveEnabled;

	public bool attackEnabled => m_bAttackEnabled;

	public bool tamingEnabled => m_bTamingEnabled;

	public int mentalStrength => m_nMentalStrength;

	public float stealRadius => m_fStealRadius;

	public int stealSuccessRate => m_nStealSuccessRate;

	public ItemReward stealItemReward => m_stealItemReward;

	public List<MonsterOwnSkill> ownSkills => m_ownSkills;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentException("dr");
		}
		Resource res = Resource.instance;
		m_nId = Convert.ToInt32(dr["monsterId"]);
		int nMonsterCharacterId = Convert.ToInt32(dr["monsterCharacterId"]);
		if (nMonsterCharacterId > 0)
		{
			m_monsterCharacter = Resource.instance.GetMonsterCharacter(nMonsterCharacterId);
			if (m_monsterCharacter == null)
			{
				SFLogUtil.Warn(GetType(), "몬스터캐릭터가 존재하지 않습니다. nMonsterCharacterId = " + nMonsterCharacterId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터캐릭터ID가 유효하지 않습니다. nMonsterCharacterId = " + nMonsterCharacterId);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_nLevel = Convert.ToInt32(dr["level"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		m_nMoveSpeed = Convert.ToInt32(dr["moveSpeed"]);
		m_nBattleMoveSpeed = Convert.ToInt32(dr["battleMoveSpeed"]);
		m_fVisibilityRange = Convert.ToSingle(dr["visibilityRange"]);
		m_fActiveAreaRadius = Convert.ToSingle(dr["activeAreaRadius"]);
		if (m_fActiveAreaRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "활동영역반지름이 유효하지 않습니다. m_fActiveAreaRadius = " + m_fActiveAreaRadius);
		}
		m_fQuestAreaRadius = Convert.ToSingle(dr["questAreaRadius"]);
		m_fReturnCompletionRadius = Convert.ToSingle(dr["returnCompletionRadius"]);
		if (m_fReturnCompletionRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "복귀완료반지름이 유효하지 않습니다. m_fReturnCompletionRadius = " + m_fReturnCompletionRadius);
		}
		m_fSkillCastingInterval = Convert.ToSingle(dr["skillCastingInterval"]);
		m_nMaxHP = Convert.ToInt32(dr["maxHp"]);
		if (m_nMaxHP <= 0)
		{
			SFLogUtil.Warn(GetType(), "최대체력이 유효하지 않습니다. m_nMaxHP = " + m_nMaxHP);
		}
		m_nPhysicalOffense = Convert.ToInt32(dr["physicalOffense"]);
		int nDropCountPoolId = Convert.ToInt32(dr["dropCountPoolId"]);
		if (nDropCountPoolId > 0)
		{
			m_dropCountPool = res.GetDropCountPool(nDropCountPoolId);
			if (m_dropCountPool == null)
			{
				SFLogUtil.Warn(GetType(), "드롭개수풀이 존재하지 않습니다. nDropCountPoolId = " + nDropCountPoolId);
			}
		}
		else if (nDropCountPoolId < 0)
		{
			SFLogUtil.Warn(GetType(), "드롭개수풀ID가 유효하지 않습니다. nDropCountPoolId = " + nDropCountPoolId);
		}
		int nDropObjectPoolId = Convert.ToInt32(dr["dropObjectPoolId"]);
		if (nDropObjectPoolId > 0)
		{
			m_dropObjectPool = Resource.instance.GetDropObjectPool(nDropObjectPoolId);
			if (m_dropObjectPool == null)
			{
				SFLogUtil.Warn(GetType(), "드롭객체풀이 존재하지 않습니다. nDropObjectPoolId = " + nDropObjectPoolId);
			}
		}
		else if (nDropCountPoolId < 0)
		{
			SFLogUtil.Warn(GetType(), "드롭객체풀ID가 유효하지 않습니다. nDropObjectPoolId = " + nDropObjectPoolId);
		}
		m_nExp = Convert.ToInt32(dr["exp"]);
		if (m_nExp < 0)
		{
			SFLogUtil.Warn(GetType(), "경험치가 유효하지 않습니다. m_nId = " + m_nId + ", m_nExp = " + m_nExp);
		}
		m_bMoveEnabled = Convert.ToBoolean(dr["moveEnabled"]);
		m_bAttackEnabled = Convert.ToBoolean(dr["attackEnabled"]);
		m_bTamingEnabled = Convert.ToBoolean(dr["tamingEnabled"]);
		m_nMentalStrength = Convert.ToInt32(dr["mentalStrength"]);
		if (m_bTamingEnabled)
		{
			if (m_nMentalStrength <= 0)
			{
				SFLogUtil.Warn(GetType(), "정신력이 유효하지 않습니다. m_nId = " + m_nId + ", m_nMentalStrength = " + m_nMentalStrength);
			}
		}
		else if (m_nMentalStrength != 0)
		{
			SFLogUtil.Warn(GetType(), "정신력이 유효하지 않습니다. m_nId = " + m_nId + ", m_nMentalStrength = " + m_nMentalStrength);
		}
		m_fStealRadius = Convert.ToSingle(dr["stealRadius"]);
		if (m_fStealRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "훔치기반지름이 유효하지 않습니다. m_nId = " + m_nId + ", m_fStealRadius = " + m_fStealRadius);
		}
		m_nStealSuccessRate = Convert.ToInt32(dr["stealSuccessRate"]);
		if (m_nStealSuccessRate < 0)
		{
			SFLogUtil.Warn(GetType(), "훔치기성공율이 유효하지 않습니다. m_nId = " + m_nId + ", m_nStealSuccessRate = " + m_nStealSuccessRate);
		}
		long lnStealItemRewardId = Convert.ToInt64(dr["stealItemRewardId"]);
		if (lnStealItemRewardId > 0)
		{
			m_stealItemReward = res.GetItemReward(lnStealItemRewardId);
			if (m_stealItemReward == null)
			{
				SFLogUtil.Warn(GetType(), "훔치기아이템보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnStealItemRewardId = " + lnStealItemRewardId);
			}
		}
		else if (lnStealItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "훔치기아이템보상ID가 유효하지 않습니다. m_nId = " + m_nId + ", lnStealItemRewardId = " + lnStealItemRewardId);
		}
	}

	public void AddOwnSkill(MonsterOwnSkill ownSkill)
	{
		if (ownSkill == null)
		{
			throw new ArgumentNullException("ownSkill");
		}
		if (ownSkill.monster != null)
		{
			throw new Exception("이미 몬스터에 존재하는 몬스터보유스킬 입니다.");
		}
		m_ownSkills.Add(ownSkill);
		ownSkill.monster = this;
	}
}
