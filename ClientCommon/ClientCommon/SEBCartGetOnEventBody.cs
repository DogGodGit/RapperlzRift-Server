using System;

namespace ClientCommon;

public class SEBCartGetOnEventBody : SEBServerEventBody
{
	public PDCartInstance cartInst;

	public Guid heroId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(cartInst);
		writer.Write(heroId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		cartInst = reader.ReadPDCartInstance();
		heroId = reader.ReadGuid();
	}
}
