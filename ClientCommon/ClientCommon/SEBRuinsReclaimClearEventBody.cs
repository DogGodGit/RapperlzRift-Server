using System;

namespace ClientCommon;

public class SEBRuinsReclaimClearEventBody : SEBServerEventBody
{
	public Guid monsterTerminatorHeroId;

	public string monsterTerminatorHeroName;

	public PDItemBooty monsterTerminatorBooty;

	public Guid ultimateAttackKingHeroId;

	public string ultimateAttackKingHeroName;

	public PDItemBooty ultimateAttackKingBooty;

	public Guid partyVolunteerHeroId;

	public string partyVolunteerHeroName;

	public PDItemBooty partyVolunteerBooty;

	public PDItemBooty randomBooty;

	public PDItemBooty[] booties;

	public PDInventorySlot[] changedInventorySlots;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(monsterTerminatorHeroId);
		writer.Write(monsterTerminatorHeroName);
		writer.Write(monsterTerminatorBooty);
		writer.Write(ultimateAttackKingHeroId);
		writer.Write(ultimateAttackKingHeroName);
		writer.Write(ultimateAttackKingBooty);
		writer.Write(partyVolunteerHeroId);
		writer.Write(partyVolunteerHeroName);
		writer.Write(partyVolunteerBooty);
		writer.Write(randomBooty);
		writer.Write(booties);
		writer.Write(changedInventorySlots);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		monsterTerminatorHeroId = reader.ReadGuid();
		monsterTerminatorHeroName = reader.ReadString();
		monsterTerminatorBooty = reader.ReadPDBooty<PDItemBooty>();
		ultimateAttackKingHeroId = reader.ReadGuid();
		ultimateAttackKingHeroName = reader.ReadString();
		ultimateAttackKingBooty = reader.ReadPDBooty<PDItemBooty>();
		partyVolunteerHeroId = reader.ReadGuid();
		partyVolunteerHeroName = reader.ReadString();
		partyVolunteerBooty = reader.ReadPDBooty<PDItemBooty>();
		randomBooty = reader.ReadPDBooty<PDItemBooty>();
		booties = reader.ReadPDBooties<PDItemBooty>();
		changedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
	}
}
