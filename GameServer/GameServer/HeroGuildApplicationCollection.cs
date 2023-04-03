using System;
using System.Collections.Generic;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class HeroGuildApplicationCollection
{
	private Guid m_heroId = Guid.Empty;

	private Dictionary<Guid, GuildApplication> m_applications = new Dictionary<Guid, GuildApplication>();

	public Guid heroId => m_heroId;

	public Dictionary<Guid, GuildApplication> applications => m_applications;

	public HeroGuildApplicationCollection(Guid heroId)
	{
		m_heroId = heroId;
	}

	public void Add(GuildApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		m_applications.Add(app.id, app);
	}

	public void Remove(Guid id)
	{
		m_applications.Remove(id);
	}

	public void CancelAll()
	{
		GuildApplication[] array = m_applications.Values.ToArray();
		foreach (GuildApplication app in array)
		{
			Guild guild = app.guild;
			guild.RemoveApplication(app);
			if (app.hero != null)
			{
				app.hero.RemoveGuildApplication(app.id);
			}
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(guild.id);
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(app.heroId));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateGuildApplication(app.id, 3));
			dbWork.Schedule();
		}
	}
}
