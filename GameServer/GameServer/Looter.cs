using System;
using System.Collections.Generic;

namespace GameServer;

public class Looter
{
	private Hero m_hero;

	private List<DropObject> m_dropObjects = new List<DropObject>();

	public Hero hero => m_hero;

	public List<DropObject> dropObjects => m_dropObjects;

	public Looter(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
	}

	public void AddDropObject(DropObject dropObject)
	{
		if (dropObject == null)
		{
			throw new ArgumentNullException("dropObject");
		}
		m_dropObjects.Add(dropObject);
	}
}
