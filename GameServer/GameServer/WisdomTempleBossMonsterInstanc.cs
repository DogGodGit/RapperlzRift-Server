using System;
using ClientCommon;

namespace GameServer;

public class WisdomTempleBossMonsterInstance : WisdomTempleMonsterInstance
{
	private MonsterArrange m_monsterArrange;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.WisdomTempleBossMonster;

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
		InitWisdomTempleMonster(wisdomTempleInst, monsterAttrFactor, wisdomTemple.bossMonsterPosition, wisdomTemple.bossMonsterYRotation);
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDWisdomTempleBossMonsterInstance();
	}
}
