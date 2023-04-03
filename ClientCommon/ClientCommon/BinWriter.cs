using System;
using System.IO;
using System.Text;

namespace ClientCommon;

public class BinWriter : BinaryWriter
{
	public BinWriter()
	{
	}

	public BinWriter(Stream output)
		: base(output)
	{
	}

	public BinWriter(Stream output, Encoding encoding)
		: base(output, encoding)
	{
	}

	public virtual void WriteEnumByte<T>(T value)
	{
		Type type = value.GetType();
		if (!type.IsEnum)
		{
			throw new ArgumentException("형식파라미터 T가 열거형이 아닙니다.");
		}
		Write((byte)Convert.ChangeType(value, Enum.GetUnderlyingType(type)));
	}

	public virtual void WriteEnumBytes<T>(T[] values)
	{
		if (values == null)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		Write(values.Length);
		foreach (T value in values)
		{
			WriteEnumByte(value);
		}
	}

	public virtual void WriteEnumInt<T>(T value)
	{
		Type type = value.GetType();
		if (!type.IsEnum)
		{
			throw new ArgumentException("형식파라미터 T가 열거형이 아닙니다.");
		}
		Write((int)Convert.ChangeType(value, Enum.GetUnderlyingType(type)));
	}

	public virtual void WriteEnumInts<T>(T[] values)
	{
		if (values == null)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		Write(values.Length);
		foreach (T value in values)
		{
			WriteEnumInt(value);
		}
	}

	public override void Write(string value)
	{
		if (value == null)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		base.Write(value);
	}

	public virtual void Write(string[] values)
	{
		if (values == null)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		Write(values.Length);
		foreach (string value in values)
		{
			Write(value);
		}
	}

	public override void Write(byte[] values)
	{
		if (values == null)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		Write(values.Length);
		foreach (byte value in values)
		{
			Write(value);
		}
	}

	public virtual void Write(short[] values)
	{
		if (values == null)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		Write(values.Length);
		foreach (short value in values)
		{
			Write(value);
		}
	}

	public virtual void Write(int[] values)
	{
		if (values == null)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		Write(values.Length);
		foreach (int value in values)
		{
			Write(value);
		}
	}

	public virtual void Write(long[] values)
	{
		if (values == null)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		Write(values.Length);
		foreach (long value in values)
		{
			Write(value);
		}
	}

	public virtual void Write(TimeSpan value)
	{
		Write(value.Ticks);
	}

	public virtual void Write(TimeSpan? value)
	{
		WriteNullableLong((!value.HasValue) ? null : new long?(value.Value.Ticks));
	}

	public virtual void Write(DateTime value)
	{
		Write(value.ToBinary());
	}

	public virtual void Write(DateTime? value)
	{
		WriteNullableLong((!value.HasValue) ? null : new long?(value.Value.ToBinary()));
	}

	public virtual void Write(DateTimeOffset value)
	{
		Write(value.DateTime);
		Write(value.Offset);
	}

	public virtual void Write(DateTimeOffset? value)
	{
		if (!value.HasValue)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		Write(value.Value);
	}

	public virtual void Write(Guid value)
	{
		Write(value.ToByteArray());
	}

	public virtual void Write(Guid? value)
	{
		Write((!value.HasValue) ? null : value.Value.ToByteArray());
	}

	public virtual void Write(Guid[] values)
	{
		if (values == null)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		Write(values.Length);
		foreach (Guid value in values)
		{
			Write(value);
		}
	}

	public virtual void WriteNullableByte(byte? value)
	{
		if (!((int?)value).HasValue)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		base.Write(value.Value);
	}

	public virtual void WriteNullableShort(short? value)
	{
		if (!((int?)value).HasValue)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		base.Write(value.Value);
	}

	public virtual void WriteNullableInt(int? value)
	{
		if (!value.HasValue)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		base.Write(value.Value);
	}

	public virtual void WriteNullableLong(long? value)
	{
		if (!value.HasValue)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		base.Write(value.Value);
	}
}
