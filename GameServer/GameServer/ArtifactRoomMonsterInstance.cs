using System;
using ClientCommon;

namespace GameServer;

public class ArtifactRoomMonsterInstance : MonsterInstance
{
	private ArtifactRoomMonsterArrange m_arrange;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.ArtifactRoomMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public ArtifactRoomMonsterArrange arrange => m_arrange;

	public void Init(ArtifactRoomInstance artifactRoomInst, ArtifactRoomMonsterArrange arrange)
	{
		if (artifactRoomInst == null)
		{
			throw new ArgumentNullException("artifactRoomInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		InitMonsterInstance(artifactRoomInst, arrange.SelectPosition(), arrange.SelectRotationY());
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDArtifactRoomMonsterInstance();
	}
}
