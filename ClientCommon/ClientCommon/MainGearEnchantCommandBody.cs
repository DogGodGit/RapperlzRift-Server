using System;

namespace ClientCommon;

public class MainGearEnchantCommandBody : CommandBody
{
	public Guid heroMainGearId;

	public bool usePenaltyPreventItem;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(heroMainGearId);
		writer.Write(usePenaltyPreventItem);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		heroMainGearId = reader.ReadGuid();
		usePenaltyPreventItem = reader.ReadBoolean();
	}
}
