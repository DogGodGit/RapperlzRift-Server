using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class GuildDailyObjectiveNoticeCommandHandler : InGameCommandHandler<GuildDailyObjectiveNoticeCommandBody, GuildDailyObjectiveNoticeResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_CoolTimeNotElapsed = 102;

	private GuildMember m_myGuildMember;

	private Guild m_myGuild;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_myGuildMember = m_myHero.guildMember;
		if (m_myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어 있지 않습니다.");
		}
		m_myGuild = m_myGuildMember.guild;
		if (!m_myHero.IsGuildDailyObjectiveNoticeCoolTimeElapsed(m_currentTime))
		{
			throw new CommandHandleException(102, "아직 쿨타임입니다.");
		}
		ServerEvent.SendGuildDailyObjectiveNotice(m_myGuild.GetClientPeers(Guid.Empty), m_myHero.ToPDSimpleHero(), m_myGuild.dailyObjectiveContentId);
		m_myHero.guildDailyObjectiveNoticeTime = m_currentTime;
		GuildDailyObjectiveNoticeResponseBody resBody = new GuildDailyObjectiveNoticeResponseBody();
		resBody.remainingCoolTime = m_myHero.GetGuildDailyObjecvtiveNoticeRemainingCoolTime(m_currentTime);
		SendResponseOK(resBody);
	}
}
