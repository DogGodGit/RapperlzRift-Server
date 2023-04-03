using System;
using System.IO;
using System.Text;

namespace ClientCommon;

public class PacketReader : BinReader
{
	public PacketReader(Stream output)
		: base(output)
	{
	}

	public PacketReader(Stream output, Encoding encoding)
		: base(output, encoding)
	{
	}

	public PDVector3 ReadPDVector3()
	{
		PDVector3 result = default(PDVector3);
		result.x = ReadSingle();
		result.y = ReadSingle();
		result.z = ReadSingle();
		return result;
	}

	public T ReadPDPacketData<T>() where T : PDPacketData
	{
		if (!ReadBoolean())
		{
			return null;
		}
		T result = Activator.CreateInstance<T>();
		result.Deserialize(this);
		return result;
	}

	public T[] ReadPDPacketDatas<T>(int nMaxLength) where T : PDPacketData
	{
		if (!ReadBoolean())
		{
			return null;
		}
		int num = ReadInt32();
		if (num < 0 || num > nMaxLength)
		{
			throw new Exception("배열크기가 유효하지 않습니다. nMaxLength = " + nMaxLength + ", nCount = " + num);
		}
		T[] array = new T[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadPDPacketData<T>();
		}
		return array;
	}

	public T[] ReadPDPacketDatas<T>() where T : PDPacketData
	{
		return ReadPDPacketDatas<T>(32767);
	}

	public T ReadPDMonsterInstance<T>() where T : PDMonsterInstance
	{
		if (!ReadBoolean())
		{
			return null;
		}
		T result = (T)PDMonsterInstance.Create((MonsterInstanceType)ReadInt32());
		result.Deserialize(this);
		return result;
	}

	public PDMonsterInstance ReadPDMonsterInstance()
	{
		return ReadPDMonsterInstance<PDMonsterInstance>();
	}

	public T[] ReadPDMonsterInstances<T>(int nMaxLength) where T : PDMonsterInstance
	{
		if (!ReadBoolean())
		{
			return null;
		}
		int num = ReadInt32();
		if (num < 0 || num > nMaxLength)
		{
			throw new Exception("배열의크기가 유효하지 않습니다. nMaxLength = " + nMaxLength + ", nCount = " + num);
		}
		T[] array = new T[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadPDMonsterInstance<T>();
		}
		return array;
	}

	public T[] ReadPDMonsterInstances<T>() where T : PDMonsterInstance
	{
		return ReadPDMonsterInstances<T>(32767);
	}

	public PDMonsterInstance[] ReadPDMonsterInstances(int nMaxLength)
	{
		return ReadPDMonsterInstances<PDMonsterInstance>(nMaxLength);
	}

	public PDMonsterInstance[] ReadPDMonsterInstances()
	{
		return ReadPDMonsterInstances(32767);
	}

	public T ReadPDAttacker<T>() where T : PDAttacker
	{
		if (!ReadBoolean())
		{
			return null;
		}
		T result = (T)PDAttacker.Create(ReadInt32());
		result.Deserialize(this);
		return result;
	}

	public PDAttacker ReadPDAttacker()
	{
		return ReadPDAttacker<PDAttacker>();
	}

	public T ReadPDHitTarget<T>() where T : PDHitTarget
	{
		if (!ReadBoolean())
		{
			return null;
		}
		T result = (T)PDHitTarget.Create(ReadInt32());
		result.Deserialize(this);
		return result;
	}

	public PDHitTarget ReadPDHitTarget()
	{
		return ReadPDHitTarget<PDHitTarget>();
	}

	public T[] ReadPDHitTargets<T>(int nMaxLength) where T : PDHitTarget
	{
		if (!ReadBoolean())
		{
			return null;
		}
		int num = ReadInt32();
		if (num < 0 || num > nMaxLength)
		{
			throw new Exception("배열크기가 유효하지 않습니다. nMaxLength = " + nMaxLength + ", nCount = " + num);
		}
		T[] array = new T[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadPDHitTarget<T>();
		}
		return array;
	}

	public T[] ReadPDHitTargets<T>() where T : PDHitTarget
	{
		return ReadPDHitTargets<T>(32767);
	}

	public PDHitTarget[] ReadPDHitTargets(int nMaxLength)
	{
		return ReadPDHitTargets<PDHitTarget>(nMaxLength);
	}

	public PDHitTarget[] ReadPDHitTargets()
	{
		return ReadPDHitTargets(32767);
	}

	public T ReadPDInventoryObject<T>() where T : PDInventoryObject
	{
		if (!ReadBoolean())
		{
			return null;
		}
		T result = (T)PDInventoryObject.Create(ReadInt32());
		result.Deserialize(this);
		return result;
	}

	public PDInventoryObject ReadPDInventoryObject()
	{
		return ReadPDInventoryObject<PDInventoryObject>();
	}

	public T[] ReadPDInventoryObjects<T>(int nMaxLength) where T : PDInventoryObject
	{
		if (!ReadBoolean())
		{
			return null;
		}
		int num = ReadInt32();
		if (num < 0 || num > nMaxLength)
		{
			throw new Exception("배열크기가 유효하지 않습니다. nMaxLength = " + nMaxLength + ", nCount = " + num);
		}
		T[] array = new T[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadPDInventoryObject<T>();
		}
		return array;
	}

	public T[] ReadPDInventoryObjects<T>() where T : PDInventoryObject
	{
		return ReadPDInventoryObjects<T>(32767);
	}

	public PDInventoryObject[] ReadPDInventoryObjects(int nMaxLength)
	{
		return ReadPDInventoryObjects<PDInventoryObject>(nMaxLength);
	}

	public PDInventoryObject[] ReadPDInventoryObjecs()
	{
		return ReadPDInventoryObjects(32767);
	}

	public T ReadPDDropObject<T>() where T : PDDropObject
	{
		if (!ReadBoolean())
		{
			return null;
		}
		T result = (T)PDDropObject.Create(ReadInt32());
		result.Deserialize(this);
		return result;
	}

	public PDDropObject ReadPDDropObject()
	{
		return ReadPDDropObject<PDDropObject>();
	}

	public T[] ReadPDDropObjects<T>(int nMaxLength) where T : PDDropObject
	{
		if (!ReadBoolean())
		{
			return null;
		}
		int num = ReadInt32();
		if (num < 0 || num > nMaxLength)
		{
			throw new Exception("배열의크기가 유효하지 않습니다. nMaxLength = " + nMaxLength + " , nCount = " + num);
		}
		T[] array = new T[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadPDDropObject<T>();
		}
		return array;
	}

	public T[] ReadPDDropObjects<T>() where T : PDDropObject
	{
		return ReadPDDropObjects<T>(32767);
	}

	public PDDropObject[] ReadPDDropObjects(int nMaxLength)
	{
		return ReadPDDropObjects<PDDropObject>(nMaxLength);
	}

	public PDDropObject[] ReadPDDropObjects()
	{
		return ReadPDDropObjects(32767);
	}

	public T ReadPDChattingLink<T>() where T : PDChattingLink
	{
		if (!ReadBoolean())
		{
			return null;
		}
		T result = (T)PDChattingLink.Create(ReadInt32());
		result.Deserialize(this);
		return result;
	}

	public PDChattingLink ReadPDChattingLink()
	{
		return ReadPDChattingLink<PDChattingLink>();
	}

	public T[] ReadPDChattingLinks<T>(int nMaxLength) where T : PDChattingLink
	{
		if (!ReadBoolean())
		{
			return null;
		}
		int num = ReadInt32();
		if (num < 0 || num > nMaxLength)
		{
			throw new Exception("배열의크기가 유효하지 않습니다. nMaxLength = " + nMaxLength + " , nCount = " + num);
		}
		T[] array = new T[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadPDChattingLink<T>();
		}
		return array;
	}

	public T[] ReadPDChattingLinks<T>() where T : PDChattingLink
	{
		return ReadPDChattingLinks<T>(32767);
	}

	public PDChattingLink[] ReadPDChattingLinks(int nMaxLength)
	{
		return ReadPDChattingLinks<PDChattingLink>(nMaxLength);
	}

	public PDChattingLink[] ReadPDChattingLinks()
	{
		return ReadPDChattingLinks(32767);
	}

	public T ReadPDBooty<T>() where T : PDBooty
	{
		if (!ReadBoolean())
		{
			return null;
		}
		T result = (T)PDBooty.Create(ReadInt32());
		result.Deserialize(this);
		return result;
	}

	public PDBooty ReadPDBooty()
	{
		return ReadPDBooty<PDBooty>();
	}

	public T[] ReadPDBooties<T>(int nMaxLength) where T : PDBooty
	{
		if (!ReadBoolean())
		{
			return null;
		}
		int num = ReadInt32();
		if (num < 0 || num > nMaxLength)
		{
			throw new Exception("배열크기가 유효하지 않습니다. nMaxLength = " + nMaxLength + ", nCount = " + num);
		}
		T[] array = new T[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadPDBooty<T>();
		}
		return array;
	}

	public T[] ReadPDBooties<T>() where T : PDBooty
	{
		return ReadPDBooties<T>(32767);
	}

	public PDBooty[] ReadPDBooties(int nMaxLength)
	{
		return ReadPDBooties<PDBooty>(nMaxLength);
	}

	public PDBooty[] ReadPDBooties()
	{
		return ReadPDBooties(32767);
	}

	public T ReadPDCartInstance<T>() where T : PDCartInstance
	{
		if (!ReadBoolean())
		{
			return null;
		}
		T result = (T)PDCartInstance.Create(ReadInt32());
		result.Deserialize(this);
		return result;
	}

	public PDCartInstance ReadPDCartInstance()
	{
		return ReadPDCartInstance<PDCartInstance>();
	}

	public T[] ReadPDCartInstances<T>(int nMaxLength) where T : PDCartInstance
	{
		if (!ReadBoolean())
		{
			return null;
		}
		int num = ReadInt32();
		if (num < 0 || num > nMaxLength)
		{
			throw new Exception("배열의크기가 유효하지 않습니다. nMaxLength = " + nMaxLength + ", nCount = " + num);
		}
		T[] array = new T[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadPDCartInstance<T>();
		}
		return array;
	}

	public T[] ReadPDCartInstances<T>() where T : PDCartInstance
	{
		return ReadPDCartInstances<T>(32767);
	}

	public PDCartInstance[] ReadPDCartInstances(int nMaxLength)
	{
		return ReadPDCartInstances<PDCartInstance>(nMaxLength);
	}

	public PDCartInstance[] ReadPDCartInstances()
	{
		return ReadPDCartInstances(32767);
	}

	public T ReadPDWarehouseObject<T>() where T : PDWarehouseObject
	{
		if (!ReadBoolean())
		{
			return null;
		}
		T result = (T)PDWarehouseObject.Create(ReadInt32());
		result.Deserialize(this);
		return result;
	}

	public PDWarehouseObject ReadPDWarehouseObject()
	{
		return ReadPDWarehouseObject<PDWarehouseObject>();
	}

	public T[] ReadPDWarehouseObjects<T>(int nMaxLength) where T : PDWarehouseObject
	{
		if (!ReadBoolean())
		{
			return null;
		}
		int num = ReadInt32();
		if (num < 0 || num > nMaxLength)
		{
			throw new Exception("배열크기가 유효하지 않습니다. nMaxLength = " + nMaxLength + ", nCount = " + num);
		}
		T[] array = new T[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadPDWarehouseObject<T>();
		}
		return array;
	}

	public T[] ReadPDWarehouseObjects<T>() where T : PDWarehouseObject
	{
		return ReadPDWarehouseObjects<T>(32767);
	}

	public PDWarehouseObject[] ReadPDWarehouseObjects(int nMaxLength)
	{
		return ReadPDWarehouseObjects<PDWarehouseObject>(nMaxLength);
	}

	public PDWarehouseObject[] ReadPDWarehouseObjecs()
	{
		return ReadPDWarehouseObjects(32767);
	}

	public T ReadPDSystemMessage<T>() where T : PDSystemMessage
	{
		if (!ReadBoolean())
		{
			return null;
		}
		T result = (T)PDSystemMessage.Create(ReadInt32());
		result.Deserialize(this);
		return result;
	}

	public PDSystemMessage ReadPDSystemMessage()
	{
		return ReadPDSystemMessage<PDSystemMessage>();
	}

	public T[] ReadPDSystemMessages<T>(int nMaxLength) where T : PDSystemMessage
	{
		if (!ReadBoolean())
		{
			return null;
		}
		int num = ReadInt32();
		if (num < 0 || num > nMaxLength)
		{
			throw new Exception("배열크기가 유효하지 않습니다. nMaxLength = " + nMaxLength + ", nCount = " + num);
		}
		T[] array = new T[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadPDSystemMessage<T>();
		}
		return array;
	}

	public T[] ReadPDSystemMessages<T>() where T : PDSystemMessage
	{
		return ReadPDSystemMessages<T>(32767);
	}

	public PDSystemMessage[] ReadPDSystemMessages(int nMaxLength)
	{
		return ReadPDSystemMessages<PDSystemMessage>(nMaxLength);
	}

	public PDSystemMessage[] ReadPDSystemMessages()
	{
		return ReadPDSystemMessages(32767);
	}
}
