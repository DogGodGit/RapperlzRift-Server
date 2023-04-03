using System;

namespace GameServer;

public class WisdomTempleQuizWrongAnswerMonsterInstance : WisdomTempleQuizMonsterInstance
{
	private WisdomTempleQuizWrongAnswerPoolEntry m_entry;

	public override IWisdomTempleQuizAnswerPoolEntry entry => m_entry;

	public override bool isRightMonster => false;

	public void Init(WisdomTempleInstance wisdomTempleInst, WisdomTempleQuizWrongAnswerPoolEntry entry, WisdomTempleMonsterAttrFactor monsterAttrFactor, WisdomTempleArrangePosition arrangePosition)
	{
		if (wisdomTempleInst == null)
		{
			throw new ArgumentNullException("wisdomTempleInst");
		}
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		if (monsterAttrFactor == null)
		{
			throw new ArgumentNullException("monsterAttrFactor");
		}
		if (arrangePosition == null)
		{
			throw new ArgumentNullException("arrangePosition");
		}
		m_entry = entry;
		InitQuizMonster(wisdomTempleInst, monsterAttrFactor, arrangePosition);
	}
}
