namespace ClientCommon;

public class HeroCreateCommandBody : CommandBody
{
	public int jobId;

	public int nationId;

	public int customPresetHair;

	public int customFaceJawHeight;

	public int customFaceJawWidth;

	public int customFaceJawEndHeight;

	public int customFaceWidth;

	public int customFaceEyebrowHeight;

	public int customFaceEyebrowRotation;

	public int customFaceEyesWidth;

	public int customFaceNoseHeight;

	public int customFaceNoseWidth;

	public int customFaceMouthHeight;

	public int customFaceMouthWidth;

	public int customBodyHeadSize;

	public int customBodyArmsLength;

	public int customBodyArmsWidth;

	public int customBodyChestSize;

	public int customBodyWaistWidth;

	public int customBodyHipsSize;

	public int customBodyPelvisWidth;

	public int customBodyLegsLength;

	public int customBodyLegsWidth;

	public int customColorSkin;

	public int customColorEyes;

	public int customColorBeardAndEyebrow;

	public int customColorHair;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(jobId);
		writer.Write(nationId);
		writer.Write(customPresetHair);
		writer.Write(customFaceJawHeight);
		writer.Write(customFaceJawWidth);
		writer.Write(customFaceJawEndHeight);
		writer.Write(customFaceWidth);
		writer.Write(customFaceEyebrowHeight);
		writer.Write(customFaceEyebrowRotation);
		writer.Write(customFaceEyesWidth);
		writer.Write(customFaceNoseHeight);
		writer.Write(customFaceNoseWidth);
		writer.Write(customFaceMouthHeight);
		writer.Write(customFaceMouthWidth);
		writer.Write(customBodyHeadSize);
		writer.Write(customBodyArmsLength);
		writer.Write(customBodyArmsWidth);
		writer.Write(customBodyChestSize);
		writer.Write(customBodyWaistWidth);
		writer.Write(customBodyHipsSize);
		writer.Write(customBodyPelvisWidth);
		writer.Write(customBodyLegsLength);
		writer.Write(customBodyLegsWidth);
		writer.Write(customColorSkin);
		writer.Write(customColorEyes);
		writer.Write(customColorBeardAndEyebrow);
		writer.Write(customColorHair);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		jobId = reader.ReadInt32();
		nationId = reader.ReadInt32();
		customPresetHair = reader.ReadInt32();
		customFaceJawHeight = reader.ReadInt32();
		customFaceJawWidth = reader.ReadInt32();
		customFaceJawEndHeight = reader.ReadInt32();
		customFaceWidth = reader.ReadInt32();
		customFaceEyebrowHeight = reader.ReadInt32();
		customFaceEyebrowRotation = reader.ReadInt32();
		customFaceEyesWidth = reader.ReadInt32();
		customFaceNoseHeight = reader.ReadInt32();
		customFaceNoseWidth = reader.ReadInt32();
		customFaceMouthHeight = reader.ReadInt32();
		customFaceMouthWidth = reader.ReadInt32();
		customBodyHeadSize = reader.ReadInt32();
		customBodyArmsLength = reader.ReadInt32();
		customBodyArmsWidth = reader.ReadInt32();
		customBodyChestSize = reader.ReadInt32();
		customBodyWaistWidth = reader.ReadInt32();
		customBodyHipsSize = reader.ReadInt32();
		customBodyPelvisWidth = reader.ReadInt32();
		customBodyLegsLength = reader.ReadInt32();
		customBodyLegsWidth = reader.ReadInt32();
		customColorSkin = reader.ReadInt32();
		customColorEyes = reader.ReadInt32();
		customColorBeardAndEyebrow = reader.ReadInt32();
		customColorHair = reader.ReadInt32();
	}
}
