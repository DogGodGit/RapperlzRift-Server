using System;
using ClientCommon;

namespace GameServer;

public class GuildMemberTabInfoCommandHandler : InGameCommandHandler<GuildMemberTabInfoCommandBody, GuildMemberTabInfoResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_myHero.guildMember == null)
		{
			throw new CommandHandleException(101, "길드멤버가 아닙니다.");
		}
		Guild guild = m_myHero.guildMember.guild;
		guild.RefreshDailyBanishmentCount(m_currentTime.Date);
		GuildMemberTabInfoResponseBody resBody = new GuildMemberTabInfoResponseBody();
		resBody.members = GuildMember.GetPDGuildMembers(guild.members.Values, m_currentTime).ToArray();
		resBody.date = (DateTime)guild.dailyBanishmentCount.date;
		resBody.dailyBanishmentCount = guild.dailyBanishmentCount.value;
		SendResponseOK(resBody);
	}
}
