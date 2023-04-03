using System.IO;
using System.Text;

namespace ClientCommon;

public class PacketWriter : BinWriter
{
	public PacketWriter()
	{
	}

	public PacketWriter(Stream output)
		: base(output)
	{
	}

	public PacketWriter(Stream output, Encoding encoding)
		: base(output, encoding)
	{
	}

	public void Write(PDVector3 value)
	{
		Write(value.x);
		Write(value.y);
		Write(value.z);
	}

	public void Write(PDPacketData value)
	{
		if (value == null)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		value.Serialize(this);
	}

	public void Write(PDPacketData[] values)
	{
		if (values == null)
		{
			Write(value: false);
			return;
		}
		Write(value: true);
		Write(values.Length);
		foreach (PDPacketData value in values)
		{
			Write(value);
		}
	}
}
