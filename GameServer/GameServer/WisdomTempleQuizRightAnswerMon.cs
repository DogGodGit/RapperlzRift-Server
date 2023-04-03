using System;

namespace GameServer;

public class WisdomTempleQuizRightAnswerMonsterInstance : WisdomTempleQuizMonsterInstance
{
	private WisdomTempleQuizRightAnswerPoolEntry m_entry;

	public override IWisdomTempleQuizAnswerPoolEntry entry => m_entry;

	public override bool isRightMonster => true;

	public void Init(WisdomTempleInstance wisdomTempleInst, WisdomTempleQuizRightAnswerPoolEntry entry, WisdomTempleMonsterAttrFactor attrFactor, WisdomTempleArrangePosition arrangePosition)
	{
		if (wisdomTempleInst == null)
		{
			throw new ArgumentNullException("wisdomTempleInst");
		}
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		if (attrFactor == null)
		{
			throw new ArgumentNullException("attrFactor");
		}
		if (arrangePosition == null)
		{
			throw new ArgumentNullException("arrangePosition");
		}
		m_entry = entry;
		InitQuizMonster(wisdomTempleInst, attrFactor, arrangePosition);
	}
}
