namespace ClientCommon;

public class CartAccelerateResponseBody : ResponseBody
{
	public bool isSuccess;

	public float remainingAccelCoolTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(isSuccess);
		writer.Write(remainingAccelCoolTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		isSuccess = reader.ReadBoolean();
		remainingAccelCoolTime = reader.ReadSingle();
	}
}
