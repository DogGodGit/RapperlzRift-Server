namespace GameServer;

public class WisdomTempleQuizMonsterPosition
{
	private WisdomTempleStep m_step;

	private WisdomTempleArrangePosition m_arrangePosition;

	public WisdomTempleStep step => m_step;

	public WisdomTempleArrangePosition arrangePosition => m_arrangePosition;

	public WisdomTempleQuizMonsterPosition(WisdomTempleStep step, WisdomTempleArrangePosition arrangePosition)
	{
		m_step = step;
		m_arrangePosition = arrangePosition;
	}
}
