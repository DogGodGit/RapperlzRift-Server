using System;
using ServerFramework;

namespace GameServer;

public class HeroSynchronizer : Synchronizer
{
	private Hero m_hero;

	protected override object syncObject => m_hero.syncObject;

	protected override Hero hero => m_hero;

	public HeroSynchronizer(Hero hero, ISFRunnable runnable)
		: base(runnable)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
	}

	public static void Exec(Hero hero, ISFRunnable runnable)
	{
		HeroSynchronizer inst = new HeroSynchronizer(hero, runnable);
		inst.Run();
	}
}
