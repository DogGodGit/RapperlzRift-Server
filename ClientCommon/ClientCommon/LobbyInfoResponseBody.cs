using System;

namespace ClientCommon;

public class LobbyInfoResponseBody : ResponseBody
{
	public Guid lastHeroId;

	public PDLobbyHero[] heroes;

	public int heroCreationDefaultNationId;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(lastHeroId);
		writer.Write(heroes);
		writer.Write(heroCreationDefaultNationId);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		lastHeroId = reader.ReadGuid();
		heroes = reader.ReadPDPacketDatas<PDLobbyHero>();
		heroCreationDefaultNationId = reader.ReadInt32();
	}
}
