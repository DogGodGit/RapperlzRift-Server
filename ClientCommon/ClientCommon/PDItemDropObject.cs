namespace ClientCommon;

public class PDItemDropObject : PDDropObject
{
	public int id;

	public int count;

	public bool owned;

	public override int type => 2;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(count);
		writer.Write(owned);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadInt32();
		count = reader.ReadInt32();
		owned = reader.ReadBoolean();
	}
}
