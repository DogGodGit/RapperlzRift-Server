namespace ClientCommon;

public class SEBRechargeEventProgressEventBody : SEBServerEventBody
{
	public int accUnOwnDia;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(accUnOwnDia);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		accUnOwnDia = reader.ReadInt32();
	}
}
