using System;
using ClientCommon;

namespace GameServer;

public class WisdomTempleTreasureBoxMonsterInstance : WisdomTempleMonsterInstance
{
	private MonsterArrange m_monsterArrange;

	private WisdomTempleArrangePosition m_arrangePosition;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.WisdomTempleTreasureBoxMonster;

	public override Monster monster => m_monsterArrange.monster;

	public WisdomTempleArrangePosition arrangePosition => m_arrangePosition;

	public void Init(WisdomTempleInstance wisdomTempleInst, WisdomTempleMonsterAttrFactor monsterAttrFactor, MonsterArrange monsterArrange, WisdomTempleArrangePosition arrangePosition)
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
		if (arrangePosition == null)
		{
			throw new ArgumentNullException("arrangePosition");
		}
		m_monsterArrange = monsterArrange;
		m_arrangePosition = arrangePosition;
		InitWisdomTempleMonster(wisdomTempleInst, monsterAttrFactor, arrangePosition.position, arrangePosition.yRotation);
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDWisdomTempleTreasureBoxMonsterInstance();
	}
}
