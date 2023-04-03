using System;

namespace GameServer;

public class DateValuePair<TValue>
{
	private DateTime m_date = DateTime.MinValue.Date;

	private TValue m_value = default(TValue);

	public DateTime date
	{
		get
		{
			return m_date;
		}
		set
		{
			m_date = value;
		}
	}

	public TValue value
	{
		get
		{
			return m_value;
		}
		set
		{
			m_value = value;
		}
	}

	public DateValuePair()
		: this(DateTime.MinValue.Date, default(TValue))
	{
	}

	public DateValuePair(DateTime date, TValue value)
	{
		m_date = date;
		m_value = value;
	}

	public void Set(DateTime date, TValue value)
	{
		m_date = date;
		m_value = value;
	}
}
