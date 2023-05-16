namespace ServerFramework;

public class SFSynchronizedLongFactory
{
	private long m_lnValue;

	public long NewValue()
	{
		lock (this)
		{
			return ++m_lnValue;
		}
	}
}
