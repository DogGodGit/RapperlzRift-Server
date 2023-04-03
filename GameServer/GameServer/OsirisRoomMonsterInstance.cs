using System;
using ClientCommon;

namespace GameServer;

public class OsirisRoomMonsterInstance : MonsterInstance
{
	private OsirisRoomMonsterArrange m_arrange;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.OsirisMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public override bool abnormalStateHitEnabled => false;

	public OsirisRoomMonsterArrange arrange => m_arrange;

	public void Init(OsirisRoomInstance osirisRoomInst, OsirisRoomMonsterArrange arrange)
	{
		if (osirisRoomInst == null)
		{
			throw new ArgumentNullException("osirisRoomInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		OsirisRoom osirisRoom = osirisRoomInst.osirisRoom;
		InitMonsterInstance(osirisRoomInst, osirisRoom.monsterSpawnPosition, osirisRoom.monsterSpawnYRotation);
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDOsirisRoomMonsterInstance();
	}
}
