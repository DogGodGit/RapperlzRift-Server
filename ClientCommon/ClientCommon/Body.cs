using System;
using System.IO;

namespace ClientCommon;

public abstract class Body
{
	public virtual byte[] Serialize()
	{
		using MemoryStream memoryStream = new MemoryStream();
		PacketWriter writer = new PacketWriter(memoryStream);
		Serialize(writer);
		return memoryStream.ToArray();
	}

	public virtual void Serialize(PacketWriter writer)
	{
	}

	public virtual long Deserialize(byte[] data)
	{
		using MemoryStream memoryStream = new MemoryStream(data);
		PacketReader reader = new PacketReader(memoryStream);
		Deserialize(reader);
		return memoryStream.Position;
	}

	public virtual void Deserialize(PacketReader reader)
	{
	}

	public virtual string Trace()
	{
		return GetType().Name;
	}

	public static byte[] SerializeRaw(Body body)
	{
		return body?.Serialize();
	}

	public static Body DeserializeRaw(byte[] rawBody, Type type, out long lnReadCount)
	{
		lnReadCount = 0L;
		Type typeFromHandle = typeof(Body);
		if (type != typeFromHandle && !type.IsSubclassOf(typeFromHandle))
		{
			throw new ArgumentException("type이 Body의 서브클래스가 아닙니다.");
		}
		if (rawBody == null)
		{
			return null;
		}
		Body body = (Body)Activator.CreateInstance(type);
		lnReadCount = body.Deserialize(rawBody);
		return body;
	}

	public static Body DeserializeRaw(byte[] rawBody, Type type)
	{
		long lnReadCount;
		Body body = DeserializeRaw(rawBody, type, out lnReadCount);
		if (body == null)
		{
			return null;
		}
		if (lnReadCount != rawBody.LongLength)
		{
			throw new Exception($"Deserialization Error !!! : [{body.GetType().Name}] rawBody.LongLength = {rawBody.LongLength}, lnReadCount = {lnReadCount}");
		}
		return body;
	}

	public static T DeserializeRaw<T>(byte[] rawBody, out long lnReadCount) where T : Body
	{
		return (T)DeserializeRaw(rawBody, typeof(T), out lnReadCount);
	}

	public static T DeserializeRaw<T>(byte[] rawBody) where T : Body
	{
		return (T)DeserializeRaw(rawBody, typeof(T));
	}
}
