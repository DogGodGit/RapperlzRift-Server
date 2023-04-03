using System;
using ClientCommon;

namespace GameServer;

public class GuildInviteCommandHandler : InGameCommandHandler<GuildInviteCommandBody, GuildInviteResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_NoAuthority = 102;

	public const short kResult_GuildMemberFull = 103;

	public const short kResult_TargetNotExistOrDifferentNation = 104;

	public const short kResult_TargetNotEnoughLevel = 105;

	public const short kResult_TargetJoinWaitTimeNotElapsed = 106;

	public const short kResult_TargetAlreadyGuildMember = 107;

	public const short kResult_TargetAlreadyInvited = 108;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid targetHeroId = (Guid)m_body.heroId;
		if (targetHeroId == Guid.Empty)
		{
			throw new CommandHandleException(1, "영웅ID가 유효하지 않습니다.");
		}
		if (targetHeroId == m_myHero.id)
		{
			throw new CommandHandleException(1, "자기자신은 초대할 수 없습니다.");
		}
		GuildMember myMember = m_myHero.guildMember;
		if (myMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입하지 않았습니다.");
		}
		if (!myMember.grade.invitationEnabled)
		{
			throw new CommandHandleException(102, "권한이 없습니다.");
		}
		Guild myGuild = myMember.guild;
		if (myGuild.isMemberFull)
		{
			throw new CommandHandleException(103, "길드멤버수가 최대입니다.");
		}
		if (myGuild.ContainsInvitationForHero(targetHeroId))
		{
			throw new CommandHandleException(108, "이미 초대한 영웅입니다.");
		}
		Hero targetHero = m_myHero.nationInst.GetHero(targetHeroId);
		if (targetHero == null)
		{
			throw new CommandHandleException(104, "대상영웅이 존재하지 않거나 다른 국가 영웅입니다. targetHeroId = " + targetHeroId);
		}
		lock (targetHero.syncObject)
		{
			if (targetHero.level < Resource.instance.guildRequiredHeroLevel)
			{
				throw new CommandHandleException(105, "목표 영웅의 레벨이 부족합니다.");
			}
			if (targetHero.GetGuildJoinWaitTime(m_currentTime) > 0f)
			{
				throw new CommandHandleException(106, "아직 재가입대기시간이 남은 영웅입니다.");
			}
			if (targetHero.guildMember != null)
			{
				throw new CommandHandleException(107, "이미 길드에 가입한 영웅입니다.");
			}
			myGuild.Invite(targetHero, myMember, m_currentTime);
			SendResponseOK(null);
		}
	}
}
