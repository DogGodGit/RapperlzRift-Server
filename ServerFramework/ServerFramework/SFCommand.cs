namespace ServerFramework;

public class SFCommand
{
	private short m_snName;

	private object m_state;

	public short name => m_snName;

	public object state => m_state;

	public SFCommand(short snName, object state)
	{
		m_snName = snName;
		m_state = state;
	}
}
