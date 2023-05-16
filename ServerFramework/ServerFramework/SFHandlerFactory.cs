using System;
using System.Collections.Generic;

namespace ServerFramework;

public class SFHandlerFactory<TKey, THandler> : ISFHandlerFactory<TKey, THandler> where THandler : class, ISFHandler
{
	private Dictionary<TKey, Type> m_handlerTypes = new Dictionary<TKey, Type>();

	public virtual void AddHandlerType<T>(TKey key) where T : THandler
	{
		m_handlerTypes.Add(key, typeof(T));
	}

	public virtual Type GetHandlerType(TKey key)
	{
		if (!m_handlerTypes.TryGetValue(key, out var value))
		{
			return null;
		}
		return value;
	}

	public virtual THandler CreateHandler(TKey key)
	{
		Type handlerType = GetHandlerType(key);
		if (!(handlerType != null))
		{
			return null;
		}
		return (THandler)Activator.CreateInstance(handlerType);
	}
}
