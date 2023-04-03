namespace ClientCommon;

public class ArtifactLevelUpResponseBody : ResponseBody
{
	public int artifactNo;

	public int artifactLevel;

	public int artifactExp;

	public int maxHP;

	public int hp;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(artifactNo);
		writer.Write(artifactLevel);
		writer.Write(artifactExp);
		writer.Write(maxHP);
		writer.Write(hp);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		artifactNo = reader.ReadInt32();
		artifactLevel = reader.ReadInt32();
		artifactExp = reader.ReadInt32();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
	}
}
