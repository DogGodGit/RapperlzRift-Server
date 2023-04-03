using System;
using System.IO;
using System.Text;

namespace ClientCommon;

public class BinReader : BinaryReader
{
	public BinReader(Stream output)
		: base(output)
	{
	}

	public BinReader(Stream output, Encoding encoding)
		: base(output, encoding)
	{
	}

	public virtual T ReadEnumByte<T>()
	{
		return (T)Enum.ToObject(typeof(T), ReadByte());
	}

	public virtual T[] ReadEnumBytes<T>(int nMaxLength)
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
			array[i] = ReadEnumByte<T>();
		}
		return array;
	}

	public virtual T[] ReadEnumBytes<T>()
	{
		return ReadEnumBytes<T>(32767);
	}

	public virtual T ReadEnumInt<T>()
	{
		return (T)Enum.ToObject(typeof(T), ReadInt32());
	}

	public virtual T[] ReadEnumInts<T>(int nMaxLength)
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
			array[i] = ReadEnumInt<T>();
		}
		return array;
	}

	public virtual T[] ReadEnumInts<T>()
	{
		return ReadEnumInts<T>(32767);
	}

	public override string ReadString()
	{
		if (!ReadBoolean())
		{
			return null;
		}
		return base.ReadString();
	}

	public virtual string[] ReadStrings(int nMaxLength)
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
		string[] array = new string[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadString();
		}
		return array;
	}

	public virtual string[] ReadStrings()
	{
		return ReadStrings(32767);
	}

	public override byte[] ReadBytes(int nMaxLength)
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
		return base.ReadBytes(num);
	}

	public virtual byte[] ReadBytes()
	{
		return ReadBytes(32767);
	}

	public virtual short[] ReadShorts(int nMaxLength)
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
		short[] array = new short[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadInt16();
		}
		return array;
	}

	public virtual short[] ReadShorts()
	{
		return ReadShorts(32767);
	}

	public virtual int[] ReadInts(int nMaxLength)
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
		int[] array = new int[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadInt32();
		}
		return array;
	}

	public virtual int[] ReadInts()
	{
		return ReadInts(32767);
	}

	public virtual long[] ReadLongs(int nMaxLength)
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
		long[] array = new long[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadInt64();
		}
		return array;
	}

	public virtual long[] ReadLongs()
	{
		return ReadLongs(32767);
	}

	public virtual TimeSpan ReadTimeSpan()
	{
		return TimeSpan.FromTicks(ReadInt64());
	}

	public virtual TimeSpan? ReadNullableTimeSpan()
	{
		long? num = ReadNullableLong();
		if (num.HasValue)
		{
			return TimeSpan.FromTicks(num.Value);
		}
		return null;
	}

	public virtual DateTime ReadDateTime()
	{
		return DateTime.FromBinary(ReadInt64());
	}

	public virtual DateTime? ReadNullableDateTime()
	{
		long? num = ReadNullableLong();
		if (num.HasValue)
		{
			return DateTime.FromBinary(num.Value);
		}
		return null;
	}

	public virtual DateTimeOffset ReadDateTimeOffset()
	{
		return new DateTimeOffset(ReadDateTime(), ReadTimeSpan());
	}

	public virtual DateTimeOffset? ReadNullableDateTimeOffset()
	{
		if (!ReadBoolean())
		{
			return null;
		}
		return ReadDateTimeOffset();
	}

	public virtual Guid ReadGuid()
	{
		return new Guid(ReadBytes());
	}

	public virtual Guid? ReadNullableGuid()
	{
		byte[] array = ReadBytes();
		if (array != null)
		{
			return new Guid(array);
		}
		return null;
	}

	public virtual Guid[] ReadGuids(int nMaxLength)
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
		Guid[] array = new Guid[num];
		for (int i = 0; i < num; i++)
		{
			ref Guid reference = ref array[i];
			reference = ReadGuid();
		}
		return array;
	}

	public virtual Guid[] ReadGuids()
	{
		return ReadGuids(32767);
	}

	public virtual byte? ReadNullableByte()
	{
		if (!ReadBoolean())
		{
			return null;
		}
		return base.ReadByte();
	}

	public virtual short? ReadNullableShort()
	{
		if (!ReadBoolean())
		{
			return null;
		}
		return base.ReadInt16();
	}

	public virtual int? ReadNullableInt()
	{
		if (!ReadBoolean())
		{
			return null;
		}
		return base.ReadInt32();
	}

	public virtual long? ReadNullableLong()
	{
		if (!ReadBoolean())
		{
			return null;
		}
		return base.ReadInt64();
	}
}
