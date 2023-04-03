using System;

namespace ClientCommon;

public abstract class PDCartInstance : PDPacketData
{
	public const int kType_MainQuest = 1;

	public const int kType_SupplySupportQuest = 2;

	public const int kType_GuildSupplySupportQuest = 3;

	public long instanceId;

	public int cartId;

	public int level;

	public int maxHP;

	public int hp;

	public bool isHighSpeed;

	public float remainingAccelCoolTime;

	public PDVector3 position;

	public float rotationY;

	public Guid ownerId;

	public string ownerName;

	public int ownerNationId;

	public PDHero rider;

	public abstract int type { get; }

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(type);
		writer.Write(instanceId);
		writer.Write(cartId);
		writer.Write(level);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(isHighSpeed);
		writer.Write(remainingAccelCoolTime);
		writer.Write(position);
		writer.Write(rotationY);
		writer.Write(ownerId);
		writer.Write(ownerName);
		writer.Write(ownerNationId);
		writer.Write(rider);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		instanceId = reader.ReadInt64();
		cartId = reader.ReadInt32();
		level = reader.ReadInt32();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		isHighSpeed = reader.ReadBoolean();
		remainingAccelCoolTime = reader.ReadSingle();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
		ownerId = reader.ReadGuid();
		ownerName = reader.ReadString();
		ownerNationId = reader.ReadInt32();
		rider = reader.ReadPDPacketData<PDHero>();
	}

	public static PDCartInstance Create(int nType)
	{
		return nType switch
		{
			1 => new PDMainQuestCartInstance(), 
			2 => new PDSupplySupportQuestCartInstance(), 
			3 => new PDGuildSupplySupportQuestCartInstance(), 
			_ => null, 
		};
	}
}
