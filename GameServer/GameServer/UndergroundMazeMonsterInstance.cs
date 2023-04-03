using System;
using ClientCommon;

namespace GameServer;

public class UndergroundMazeMonsterInstance : MonsterInstance
{
	private UndergroundMazeMonsterArrange m_arrange;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.UndergroundMazeMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public UndergroundMazeMonsterArrange arrange => m_arrange;

	public void Init(UndergroundMazeInstance undergroundMazeInst, UndergroundMazeMonsterArrange arrange)
	{
		if (undergroundMazeInst == null)
		{
			throw new ArgumentNullException("undergroundMazeInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		InitMonsterInstance(undergroundMazeInst, arrange.SelectPosition(), arrange.SelectRotationY());
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDUndergroundMazeMonsterInstance();
	}
}
