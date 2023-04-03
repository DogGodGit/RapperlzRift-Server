namespace ClientCommon;

public class CashProductPurchaseCompleteResponseBody : ResponseBody
{
	public int unOwnDia;

	public int vipPoint;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(unOwnDia);
		writer.Write(vipPoint);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		unOwnDia = reader.ReadInt32();
		vipPoint = reader.ReadInt32();
	}
}
