using System;

namespace ClientCommon;

public class PDHero : PDPacketData
{
	public Guid id;

	public string name;

	public int nationId;

	public int jobId;

	public int level;

	public int vipLevel;

	public int rankNo;

	public int maxHP;

	public int hp;

	public bool isWalking;

	public bool accelerationMode;

	public long battlePower;

	public PDAbnormalStateEffect[] abnormalStateEffects;

	public PDHeroMainGear equippedWeapon;

	public PDHeroMainGear equippedArmor;

	public int equippedWingId;

	public PDVector3 position;

	public float rotationY;

	public bool isRiding;

	public int mountId;

	public int mountLevel;

	public bool isBattleMode;

	public bool isFishing;

	public bool isDistorting;

	public bool isMysteryBoxPicking;

	public int pickedMysteryBoxGrade;

	public bool isSecretLetterPicking;

	public int pickedSecretLetterGrade;

	public Guid guildId;

	public string guildName;

	public int guildMemberGrade;

	public int displayTitleId;

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

	public int transformationMonsterId;

	public int participatedCreatureId;

	public int equippedCostumeId;

	public int appliedCostumeEffectId;

	public bool isSafeMode;

	public int equippedArtifactNo;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(id);
		writer.Write(name);
		writer.Write(nationId);
		writer.Write(jobId);
		writer.Write(level);
		writer.Write(vipLevel);
		writer.Write(rankNo);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(isWalking);
		writer.Write(accelerationMode);
		writer.Write(battlePower);
		writer.Write(abnormalStateEffects);
		writer.Write(equippedWeapon);
		writer.Write(equippedArmor);
		writer.Write(equippedWingId);
		writer.Write(position);
		writer.Write(rotationY);
		writer.Write(isRiding);
		writer.Write(mountId);
		writer.Write(mountLevel);
		writer.Write(isBattleMode);
		writer.Write(isFishing);
		writer.Write(isDistorting);
		writer.Write(isMysteryBoxPicking);
		writer.Write(pickedMysteryBoxGrade);
		writer.Write(isSecretLetterPicking);
		writer.Write(pickedSecretLetterGrade);
		writer.Write(guildId);
		writer.Write(guildName);
		writer.Write(guildMemberGrade);
		writer.Write(displayTitleId);
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
		writer.Write(transformationMonsterId);
		writer.Write(participatedCreatureId);
		writer.Write(equippedCostumeId);
		writer.Write(appliedCostumeEffectId);
		writer.Write(isSafeMode);
		writer.Write(equippedArtifactNo);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		id = reader.ReadGuid();
		name = reader.ReadString();
		nationId = reader.ReadInt32();
		jobId = reader.ReadInt32();
		level = reader.ReadInt32();
		vipLevel = reader.ReadInt32();
		rankNo = reader.ReadInt32();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		isWalking = reader.ReadBoolean();
		accelerationMode = reader.ReadBoolean();
		battlePower = reader.ReadInt64();
		abnormalStateEffects = reader.ReadPDPacketDatas<PDAbnormalStateEffect>();
		equippedWeapon = reader.ReadPDPacketData<PDHeroMainGear>();
		equippedArmor = reader.ReadPDPacketData<PDHeroMainGear>();
		equippedWingId = reader.ReadInt32();
		position = reader.ReadPDVector3();
		rotationY = reader.ReadSingle();
		isRiding = reader.ReadBoolean();
		mountId = reader.ReadInt32();
		mountLevel = reader.ReadInt32();
		isBattleMode = reader.ReadBoolean();
		isFishing = reader.ReadBoolean();
		isDistorting = reader.ReadBoolean();
		isMysteryBoxPicking = reader.ReadBoolean();
		pickedMysteryBoxGrade = reader.ReadInt32();
		isSecretLetterPicking = reader.ReadBoolean();
		pickedSecretLetterGrade = reader.ReadInt32();
		guildId = reader.ReadGuid();
		guildName = reader.ReadString();
		guildMemberGrade = reader.ReadInt32();
		displayTitleId = reader.ReadInt32();
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
		transformationMonsterId = reader.ReadInt32();
		participatedCreatureId = reader.ReadInt32();
		equippedCostumeId = reader.ReadInt32();
		appliedCostumeEffectId = reader.ReadInt32();
		isSafeMode = reader.ReadBoolean();
		equippedArtifactNo = reader.ReadInt32();
	}
}
