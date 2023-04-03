using System;
using ClientCommon;

namespace GameServer;

public class GuildAltarMonsterInstance : MonsterInstance
{
	private MonsterArrange m_arrange;

	private Guid m_missionHeroId = Guid.Empty;

	private string m_sMissionHeroName;

	private int m_nMissionHeroLevel;

	private GuildAltarDefenseMonsterAttrFactor m_attrFactor;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.GuildAltarMonster;

	public override Monster monster => m_arrange.monster;

	protected override float maxHpFactor => base.maxHpFactor * ((m_attrFactor != null) ? m_attrFactor.maxHpFactor : 1f);

	protected override float offenseFactor => base.offenseFactor * ((m_attrFactor != null) ? m_attrFactor.offenseFactor : 1f);

	public override bool isExclusive => true;

	public override Guid exclusiveHeroId => m_missionHeroId;

	public override string exclusiveHeroName => m_sMissionHeroName;

	public int missionHeroLevel => m_nMissionHeroLevel;

	public GuildTerritoryInstance guildTerritoryInst => (GuildTerritoryInstance)m_currentPlace;

	public void Init(GuildTerritoryInstance guildTerritoryInst, Hero missionHero)
	{
		if (guildTerritoryInst == null)
		{
			throw new ArgumentNullException("guildTerritoryInst");
		}
		if (missionHero == null)
		{
			throw new ArgumentNullException("missionHero");
		}
		GuildAltar altar = Resource.instance.guildAltar;
		m_arrange = altar.defenseMonsterArrange;
		m_missionHeroId = missionHero.id;
		m_sMissionHeroName = missionHero.name;
		m_nMissionHeroLevel = missionHero.level;
		m_attrFactor = altar.GetDefenseMonsterAttrFactor(m_nMissionHeroLevel);
		InitMonsterInstance(guildTerritoryInst, missionHero.position, Util.SelectAngle());
	}

	protected override void OnDeadAsync(DateTimeOffset time)
	{
		base.OnDeadAsync(time);
		Hero hero = Cache.instance.GetLoggedInHero(m_missionHeroId);
		if (hero == null)
		{
			return;
		}
		lock (hero.syncObject)
		{
			ProcessMission(hero);
		}
	}

	private void ProcessMission(Hero hero)
	{
		if (hero.guildMember == null || hero.guildMember.guild.id != guildTerritoryInst.guild.id)
		{
			return;
		}
		HeroGuildAltarDefenseMission mission = hero.guildAltarDefenseMission;
		if (mission != null)
		{
			if (!hero.isDead && hero.currentPlace == m_currentPlace && IsQuestAreaPosition(hero.position))
			{
				mission.Complete();
			}
			else
			{
				mission.Fail(bSendEventToMySelf: true);
			}
		}
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDGuildAltarMonsterInstance();
	}
}
