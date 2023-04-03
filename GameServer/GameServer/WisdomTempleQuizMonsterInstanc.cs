using System;
using ClientCommon;

namespace GameServer;

public abstract class WisdomTempleQuizMonsterInstance : WisdomTempleMonsterInstance
{
	protected WisdomTempleArrangePosition m_arrangePosition;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.WisdomTempleQuizMonster;

	public WisdomTempleArrangePosition arrangePosition => m_arrangePosition;

	public abstract IWisdomTempleQuizAnswerPoolEntry entry { get; }

	public override Monster monster => entry.monsterArrange.monster;

	public abstract bool isRightMonster { get; }

	protected void InitQuizMonster(WisdomTempleInstance wisdomTempleInst, WisdomTempleMonsterAttrFactor monsterAttrFactor, WisdomTempleArrangePosition arrangePosition)
	{
		if (wisdomTempleInst == null)
		{
			throw new ArgumentNullException("wisdomTempleInst");
		}
		if (monsterAttrFactor == null)
		{
			throw new ArgumentNullException("monsterAttrFactor");
		}
		if (arrangePosition == null)
		{
			throw new ArgumentNullException("arrangePosition");
		}
		m_arrangePosition = arrangePosition;
		InitWisdomTempleMonster(wisdomTempleInst, monsterAttrFactor, arrangePosition.position, arrangePosition.yRotation);
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDWisdomTempleQuizMonsterInstance();
	}
}
