using System;
using ClientCommon;

namespace GameServer;

public class WisdomTempleColorMatchingMonsterInstance : WisdomTempleMonsterInstance
{
	private MonsterArrange m_monsterArrange;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.WisdomTempleColorMatchingMonster;

	public override Monster monster => m_monsterArrange.monster;

	public void Init(WisdomTempleInstance wisdomTempleInst, WisdomTempleMonsterAttrFactor monsterAttrFactor, MonsterArrange monsterArrange)
	{
		if (wisdomTempleInst == null)
		{
			throw new ArgumentNullException("wisdomTempleInst");
		}
		if (monsterAttrFactor == null)
		{
			throw new ArgumentNullException("monsterAttrFactor");
		}
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArrange = monsterArrange;
		WisdomTemple wisdomTemple = wisdomTempleInst.wisdomTemple;
		InitWisdomTempleMonster(wisdomTempleInst, monsterAttrFactor, wisdomTemple.colorMatchingMonsterPosition, wisdomTemple.SelectColorMatchingMonsterRotationY());
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDWisdomTempleColorMatchingMonsterInstance();
	}
}
