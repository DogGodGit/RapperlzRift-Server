using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroSeriesMission
{
	private Hero m_hero;

	private SeriesMission m_mission;

	private int m_nCurrentStep;

	private int m_nProgressCount;

	public Hero hero => m_hero;

	public SeriesMission mission => m_mission;

	public int progressCount
	{
		get
		{
			return m_nProgressCount;
		}
		set
		{
			m_nProgressCount = value;
		}
	}

	public int currentStep
	{
		get
		{
			return m_nCurrentStep;
		}
		set
		{
			m_nCurrentStep = value;
		}
	}

	public HeroSeriesMission(Hero hero, SeriesMission mission, int nCurrentStep)
	{
		m_hero = hero;
		m_mission = mission;
		m_nCurrentStep = nCurrentStep;
	}

	public void IncreaseProgressCount()
	{
		m_nProgressCount++;
		ServerEvent.SendSeriesMissionUpdated(m_hero.account.peer, m_mission.id, m_nProgressCount);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroSeriesMission(m_hero.id, m_mission.id, m_nProgressCount, m_nCurrentStep));
		dbWork.Schedule();
	}

	public PDHeroSeriesMission ToPDHeroSeriesMission()
	{
		PDHeroSeriesMission inst = new PDHeroSeriesMission();
		inst.missionId = m_mission.id;
		inst.currentStep = m_nCurrentStep;
		inst.progressCount = m_nProgressCount;
		return inst;
	}
}
