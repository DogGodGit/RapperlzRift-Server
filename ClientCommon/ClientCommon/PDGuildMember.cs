using System;

namespace ClientCommon;

public class PDGuildMember : PDPacketData
{
	public Guid id;

	public string name;

	public int jobId;

	public int level;

	public int vipLevel;

	public int totalContributionPoint;

	public int memberGrade;

	public bool isLoggedIn;

	public float logoutElapsedTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(name);
		writer.Write(jobId);
		writer.Write(level);
		writer.Write(vipLevel);
		writer.Write(totalContributionPoint);
		writer.Write(memberGrade);
		writer.Write(isLoggedIn);
		writer.Write(logoutElapsedTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		name = reader.ReadString();
		jobId = reader.ReadInt32();
		level = reader.ReadInt32();
		vipLevel = reader.ReadInt32();
		totalContributionPoint = reader.ReadInt32();
		memberGrade = reader.ReadInt32();
		isLoggedIn = reader.ReadBoolean();
		logoutElapsedTime = reader.ReadSingle();
	}
}
