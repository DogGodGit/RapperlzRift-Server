using System;
using ClientCommon;

namespace GameServer;

public class GuildCallTransmissionCommandHandler : InGameCommandHandler<GuildCallTransmissionCommandBody, GuildCallTransmissionResponseBody>
{
	public const short kResult_Dead = 101;

	public const short kResult_AlreadyCartRiding = 102;

	public const short kResult_NotEnoughHeroLevel = 103;

	public const short kResult_NoGuildMember = 104;

	public const short kResult_NotExistGuildCall = 105;

	public const short kResult_NationWar = 106;

	public const short KResult_NotEnoughHeroLevelForEnter = 107;

	private Guild m_myGuild;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		long lnCallId = m_body.callId;
		if (lnCallId <= 0)
		{
			throw new CommandHandleException(1, "소집ID 가 유효하지 않습니다. lnCallId = " + lnCallId);
		}
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		NationWarInstance nationWarInst = m_myHero.nationInst.nationWarInst;
		if (nationWarInst != null)
		{
			throw new CommandHandleException(106, "국가전이 진행중입니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은 상태입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(102, "영웅이 카트에 탑승중입니다.");
		}
		if (m_myHero.level < Resource.instance.pvpMinHeroLevel)
		{
			throw new CommandHandleException(103, "영웅의 레벨이 부족합니다.");
		}
		if (m_myHero.guildMember == null)
		{
			throw new CommandHandleException(104, "길드에 가입되어있지 않습니다.");
		}
		m_myGuild = m_myHero.guildMember.guild;
		GuildCall guildCall = m_myGuild.GetGuildCall(lnCallId);
		if (guildCall == null)
		{
			throw new CommandHandleException(105, "존재하지 않은 길드소집입니다. lnCallId = " + lnCallId);
		}
		if (guildCall.callerId == m_myHero.id)
		{
			throw new CommandHandleException(1, "소집자는 이용할 수 없습니다.");
		}
		if (guildCall.continent.requiredHeroLevel > m_myHero.level)
		{
			throw new CommandHandleException(107, "대상 대륙에 입장하기 위한 영웅레벨이 부족합니다.");
		}
		Continent targetContinent = guildCall.continent;
		int nTargetNationId = guildCall.nationId;
		Vector3 targetPosition = guildCall.SelectPosition();
		float fTargetRotationY = guildCall.rotationY;
		currentPlace.Exit(m_myHero, isLogOut: false, new ContinentEnterForGuildCallTransmissionParam(targetContinent, nTargetNationId, targetPosition, fTargetRotationY, m_currentTime));
		GuildCallTransmissionResponseBody resBody = new GuildCallTransmissionResponseBody();
		resBody.targetContinentId = targetContinent.id;
		resBody.targetNationId = nTargetNationId;
		SendResponseOK(resBody);
	}
}
