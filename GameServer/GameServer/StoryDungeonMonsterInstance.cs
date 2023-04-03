using System;
using ClientCommon;

namespace GameServer;

public class StoryDungeonMonsterInstance : MonsterInstance
{
	private StoryDungeonMonsterArrange m_arrange;

	private Hero m_tamer;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.MainQuestDungeonSummonMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public StoryDungeonMonsterArrange arrange => m_arrange;

	public int monsterType => m_arrange.monsterType;

	public Hero tamer
	{
		get
		{
			return m_tamer;
		}
		set
		{
			m_tamer = value;
		}
	}

	public void Init(StoryDungeonInstance storyDungeonInst, StoryDungeonMonsterArrange arrange)
	{
		if (storyDungeonInst == null)
		{
			throw new ArgumentNullException("storyDungeonInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		InitMonsterInstance(storyDungeonInst, arrange.SelectPosition(), arrange.SelectRotationY());
	}

	public bool IsTamingEnabledPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_position, monster.stealRadius * 1.1f + fRadius * 2f, position);
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		PDStoryDungeonMonsterInstance inst = new PDStoryDungeonMonsterInstance();
		inst.monsterType = monsterType;
		return inst;
	}
}
