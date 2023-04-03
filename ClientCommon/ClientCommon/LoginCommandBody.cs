namespace ClientCommon;

public class LoginCommandBody : CommandBody
{
	public int virtualGameServerId;

	public string accessToken;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(virtualGameServerId);
		writer.Write(accessToken);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		virtualGameServerId = reader.ReadInt32();
		accessToken = reader.ReadString();
	}
}
