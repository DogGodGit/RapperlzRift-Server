using System;

namespace ServerFramework;

public class SFAction : ISFRunnable
{
	private Action m_action;

	public SFAction(Action action)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		m_action = action;
	}

	public void Run()
	{
		m_action();
	}
}
public class SFAction<T> : ISFRunnable
{
	private Action<T> m_action;

	private T m_arg = default(T);

	public SFAction(Action<T> action, T arg)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		m_action = action;
		m_arg = arg;
	}

	public void Run()
	{
		m_action(m_arg);
	}
}
public class SFAction<T1, T2> : ISFRunnable
{
	private Action<T1, T2> m_action;

	private T1 m_arg1 = default(T1);

	private T2 m_arg2 = default(T2);

	public SFAction(Action<T1, T2> action, T1 arg1, T2 arg2)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		m_action = action;
		m_arg1 = arg1;
		m_arg2 = arg2;
	}

	public void Run()
	{
		m_action(m_arg1, m_arg2);
	}
}
public class SFAction<T1, T2, T3> : ISFRunnable
{
	private Action<T1, T2, T3> m_action;

	private T1 m_arg1 = default(T1);

	private T2 m_arg2 = default(T2);

	private T3 m_arg3 = default(T3);

	public SFAction(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		m_action = action;
		m_arg1 = arg1;
		m_arg2 = arg2;
		m_arg3 = arg3;
	}

	public void Run()
	{
		m_action(m_arg1, m_arg2, m_arg3);
	}
}
public class SFAction<T1, T2, T3, T4> : ISFRunnable
{
	private Action<T1, T2, T3, T4> m_action;

	private T1 m_arg1 = default(T1);

	private T2 m_arg2 = default(T2);

	private T3 m_arg3 = default(T3);

	private T4 m_arg4 = default(T4);

	public SFAction(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		m_action = action;
		m_arg1 = arg1;
		m_arg2 = arg2;
		m_arg3 = arg3;
		m_arg4 = arg4;
	}

	public void Run()
	{
		m_action(m_arg1, m_arg2, m_arg3, m_arg4);
	}
}
public class SFAction<T1, T2, T3, T4, T5> : ISFRunnable
{
	private Action<T1, T2, T3, T4, T5> m_action;

	private T1 m_arg1 = default(T1);

	private T2 m_arg2 = default(T2);

	private T3 m_arg3 = default(T3);

	private T4 m_arg4 = default(T4);

	private T5 m_arg5 = default(T5);

	public SFAction(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		m_action = action;
		m_arg1 = arg1;
		m_arg2 = arg2;
		m_arg3 = arg3;
		m_arg4 = arg4;
		m_arg5 = arg5;
	}

	public void Run()
	{
		m_action(m_arg1, m_arg2, m_arg3, m_arg4, m_arg5);
	}
}
public class SFAction<T1, T2, T3, T4, T5, T6> : ISFRunnable
{
	private Action<T1, T2, T3, T4, T5, T6> m_action;

	private T1 m_arg1 = default(T1);

	private T2 m_arg2 = default(T2);

	private T3 m_arg3 = default(T3);

	private T4 m_arg4 = default(T4);

	private T5 m_arg5 = default(T5);

	private T6 m_arg6 = default(T6);

	public SFAction(Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		m_action = action;
		m_arg1 = arg1;
		m_arg2 = arg2;
		m_arg3 = arg3;
		m_arg4 = arg4;
		m_arg5 = arg5;
		m_arg6 = arg6;
	}

	public void Run()
	{
		m_action(m_arg1, m_arg2, m_arg3, m_arg4, m_arg5, m_arg6);
	}
}
public class SFAction<T1, T2, T3, T4, T5, T6, T7> : ISFRunnable
{
	private Action<T1, T2, T3, T4, T5, T6, T7> m_action;

	private T1 m_arg1 = default(T1);

	private T2 m_arg2 = default(T2);

	private T3 m_arg3 = default(T3);

	private T4 m_arg4 = default(T4);

	private T5 m_arg5 = default(T5);

	private T6 m_arg6 = default(T6);

	private T7 m_arg7 = default(T7);

	public SFAction(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		m_action = action;
		m_arg1 = arg1;
		m_arg2 = arg2;
		m_arg3 = arg3;
		m_arg4 = arg4;
		m_arg5 = arg5;
		m_arg6 = arg6;
		m_arg7 = arg7;
	}

	public void Run()
	{
		m_action(m_arg1, m_arg2, m_arg3, m_arg4, m_arg5, m_arg6, m_arg7);
	}
}
public class SFAction<T1, T2, T3, T4, T5, T6, T7, T8> : ISFRunnable
{
	private Action<T1, T2, T3, T4, T5, T6, T7, T8> m_action;

	private T1 m_arg1 = default(T1);

	private T2 m_arg2 = default(T2);

	private T3 m_arg3 = default(T3);

	private T4 m_arg4 = default(T4);

	private T5 m_arg5 = default(T5);

	private T6 m_arg6 = default(T6);

	private T7 m_arg7 = default(T7);

	private T8 m_arg8 = default(T8);

	public SFAction(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		m_action = action;
		m_arg1 = arg1;
		m_arg2 = arg2;
		m_arg3 = arg3;
		m_arg4 = arg4;
		m_arg5 = arg5;
		m_arg6 = arg6;
		m_arg7 = arg7;
		m_arg8 = arg8;
	}

	public void Run()
	{
		m_action(m_arg1, m_arg2, m_arg3, m_arg4, m_arg5, m_arg6, m_arg7, m_arg8);
	}
}
public class SFAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ISFRunnable
{
	private Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> m_action;

	private T1 m_arg1 = default(T1);

	private T2 m_arg2 = default(T2);

	private T3 m_arg3 = default(T3);

	private T4 m_arg4 = default(T4);

	private T5 m_arg5 = default(T5);

	private T6 m_arg6 = default(T6);

	private T7 m_arg7 = default(T7);

	private T8 m_arg8 = default(T8);

	private T9 m_arg9 = default(T9);

	public SFAction(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		m_action = action;
		m_arg1 = arg1;
		m_arg2 = arg2;
		m_arg3 = arg3;
		m_arg4 = arg4;
		m_arg5 = arg5;
		m_arg6 = arg6;
		m_arg7 = arg7;
		m_arg8 = arg8;
		m_arg9 = arg9;
	}

	public void Run()
	{
		m_action.Invoke(m_arg1, m_arg2, m_arg3, m_arg4, m_arg5, m_arg6, m_arg7, m_arg8, m_arg9);
	}
}
public class SFAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : ISFRunnable
{
	private Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> m_action;

	private T1 m_arg1 = default(T1);

	private T2 m_arg2 = default(T2);

	private T3 m_arg3 = default(T3);

	private T4 m_arg4 = default(T4);

	private T5 m_arg5 = default(T5);

	private T6 m_arg6 = default(T6);

	private T7 m_arg7 = default(T7);

	private T8 m_arg8 = default(T8);

	private T9 m_arg9 = default(T9);

	private T10 m_arg10 = default(T10);

	public SFAction(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		m_action = action;
		m_arg1 = arg1;
		m_arg2 = arg2;
		m_arg3 = arg3;
		m_arg4 = arg4;
		m_arg5 = arg5;
		m_arg6 = arg6;
		m_arg7 = arg7;
		m_arg8 = arg8;
		m_arg9 = arg9;
		m_arg10 = arg10;
	}

	public void Run()
	{
		m_action.Invoke(m_arg1, m_arg2, m_arg3, m_arg4, m_arg5, m_arg6, m_arg7, m_arg8, m_arg9, m_arg10);
	}
}
