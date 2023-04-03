using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildAltarDonateCommandHandler : InGameCommandHandler<GuildAltarDonateCommandBody, GuildAltarDonateResponseBody>
{
	public const short kResult_MoralPointIsMax = 101;

	public const short kResult_NotEnoughGold = 102;

	public const short kResult_SpellInjectionMissionPerforming = 103;

	public const short kResult_DefenseMissionPerforming = 104;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTimeOffset.MinValue.Date;

	private GuildAltar m_altar;

	private Guild m_guild;

	private int m_nAddedMoralPoint;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		m_altar = Resource.instance.guildAltar;
		if (!(m_myHero.currentPlace is GuildTerritoryInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소가 길드영지가 아닙니다.");
		}
		m_guild = currentPlace.guild;
		if (m_myHero.guildAltarSpellInjectionMission != null)
		{
			throw new CommandHandleException(103, "길드제단 마력주입미션을 수행중입니다.");
		}
		if (m_myHero.guildAltarDefenseMission != null)
		{
			throw new CommandHandleException(104, "길드제단 수비미션을 수행중입니다.");
		}
		GuildTerritoryNpc npc = m_altar.npc;
		if (!npc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		m_myHero.RefreshGuildMoralPoint(m_currentDate);
		if (m_myHero.guildMoralPoint >= m_altar.dailyHeroMaxMoralPoint)
		{
			throw new CommandHandleException(101, "금일 채울수 있는 모럴포인트가 최대입니다.");
		}
		if (m_myHero.gold < m_altar.donationGold)
		{
			throw new CommandHandleException(102, "골드가 부족합니다.");
		}
		m_myHero.UseGold(m_altar.donationGold);
		m_nAddedMoralPoint = m_myHero.AddGuildMoralPoint(m_altar.donationRewardMoralPoint);
		m_guild.AddMoralPoint(m_currentDate, m_nAddedMoralPoint, m_myHero.id);
		SaveToDB();
		SaveToGameLogDB();
		GuildAltarDonateResponseBody resBody = new GuildAltarDonateResponseBody();
		resBody.date = (DateTime)m_currentDate;
		resBody.gold = m_myHero.gold;
		resBody.guildMoralPoint = m_myHero.guildMoralPoint;
		resBody.giMoralPoint = m_guild.moralPoint;
		SendResponseOK(resBody);
		if (m_myHero.guildMoralPoint >= m_altar.dailyHeroMaxMoralPoint)
		{
			m_myHero.CompleteGuildAltar(m_currentTime);
			m_myHero.ProcessTodayTask(21, m_currentDate);
			m_guild.CompleteGuildDailyObjective(m_currentDate, 3, m_myHero.guildMember);
		}
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_guild.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_myHero.id));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_MoralPoint(m_guild.id, m_guild.moralPointDate, m_guild.moralPoint));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_GuildMoralPoint(m_myHero.id, m_myHero.guildMoralPointDate, m_myHero.guildMoralPoint));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Gold(m_myHero.id, m_myHero.gold));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildAltarDonationLog(Guid.NewGuid(), m_guild.id, m_myHero.id, m_altar.donationGold, m_altar.donationRewardMoralPoint, m_nAddedMoralPoint, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
