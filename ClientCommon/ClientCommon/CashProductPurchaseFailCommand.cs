using System;

namespace ClientCommon;

public class CashProductPurchaseFailCommandBody : CommandBody
{
	public Guid purchaseId;

	public string failReason;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(purchaseId);
		writer.Write(failReason);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		purchaseId = reader.ReadGuid();
		failReason = reader.ReadString();
	}
}
