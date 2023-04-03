using System;

namespace ClientCommon;

public class PDHeroDailyQuest : PDPacketData
{
	public Guid id;

	public int slotIndex;

	public int missionId;

	public bool isAccepted;

	public bool missionImmediateCompleted;

	public int progressCount;

	public float autoCompletionRemainingTime;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(slotIndex);
		writer.Write(missionId);
		writer.Write(isAccepted);
		writer.Write(missionImmediateCompleted);
		writer.Write(progressCount);
		writer.Write(autoCompletionRemainingTime);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		slotIndex = reader.ReadInt32();
		missionId = reader.ReadInt32();
		isAccepted = reader.ReadBoolean();
		missionImmediateCompleted = reader.ReadBoolean();
		progressCount = reader.ReadInt32();
		autoCompletionRemainingTime = reader.ReadSingle();
	}
}
