using System;

namespace ClientCommon;

public class CashProductPurchaseCancelCommandBody : CommandBody
{
	public Guid purchaseId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(purchaseId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		purchaseId = reader.ReadGuid();
	}
}
