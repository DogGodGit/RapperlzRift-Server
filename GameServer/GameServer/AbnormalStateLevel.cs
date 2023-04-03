namespace GameServer;

public abstract class AbnormalStateLevel
{
	protected int m_nLevel;

	protected int m_nDuration;

	protected int m_nValue1;

	protected int m_nValue2;

	protected int m_nValue3;

	protected int m_nValue4;

	protected int m_nValue5;

	protected int m_nValue6;

	public abstract AbnormalState abnormalState { get; }

	public abstract int level { get; }

	public abstract int duration { get; }

	public abstract int value1 { get; }

	public abstract int value2 { get; }

	public abstract int value3 { get; }

	public abstract int value4 { get; }

	public abstract int value5 { get; }

	public abstract int value6 { get; }
}
