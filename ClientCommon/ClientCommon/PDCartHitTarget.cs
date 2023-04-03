namespace ClientCommon;

public class PDCartHitTarget : PDHitTarget
{
	public long cartInstanceId;

	public override int type => 3;

	public PDCartHitTarget()
	{
	}

	public PDCartHitTarget(long cartInstanceId)
	{
		this.cartInstanceId = cartInstanceId;
	}

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(cartInstanceId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		cartInstanceId = reader.ReadInt64();
	}
}
