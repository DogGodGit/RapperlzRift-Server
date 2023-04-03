using System;

namespace ClientCommon;

public class PDHeroMountGear : PDPacketData
{
	public Guid id;

	public int mountGearId;

	public bool owned;

	public PDHeroMountGearOptionAttr[] optionAttrs;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(mountGearId);
		writer.Write(owned);
		writer.Write(optionAttrs);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		mountGearId = reader.ReadInt32();
		owned = reader.ReadBoolean();
		optionAttrs = reader.ReadPDPacketDatas<PDHeroMountGearOptionAttr>();
	}
}
