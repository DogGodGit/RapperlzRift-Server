namespace GameServer;

public class Aggro
{
	private Hero m_target;

	private long m_lnValue;

	public Hero target => m_target;

	public long value => m_lnValue;

	public Aggro(Hero target)
	{
		m_target = target;
	}

	public void AddValue(long lnValue)
	{
		m_lnValue += lnValue;
	}

	public int CompareTo(Aggro other)
	{
		if (other == null)
		{
			return 1;
		}
		return m_lnValue.CompareTo(other.value);
	}

	public static int Compare(Aggro x, Aggro y)
	{
		if (x == null)
		{
			if (y != null)
			{
				return -1;
			}
			return 0;
		}
		return x.CompareTo(y);
	}
}
