namespace ServerFramework;

public class SFSynchronizedIntFactory
{
	private int m_nValue;

	public int NewValue()
	{
		lock (this)
		{
			return ++m_nValue;
		}
	}
}
