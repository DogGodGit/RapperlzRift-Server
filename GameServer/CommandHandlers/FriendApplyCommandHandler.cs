using System;
using ClientCommon;

namespace GameServer.CommandHandlers;

public class FriendApplyCommandHandler : InGameCommandHandler<FriendApplyCommandBody, FriendApplyResponseBody>
{
    public const short kResult_ApplicationExist = 101;

    public const short kResult_AlreadyFriend = 102;

    public const short kResult_TargetNotLoggedIn = 103;

    public const short kResult_ExistInMyBlacklist = 104;

    public const short kResult_ExistInTargetBlacklist = 105;

    private Hero m_target;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        Guid targetHeroId = m_body.targetHeroId;
        if (targetHeroId == Guid.Empty)
        {
            throw new CommandHandleException(1, "대상영웅ID가 유효하지 않습니다.");
        }
        if (targetHeroId == m_myHero.id)
        {
            throw new CommandHandleException(1, "자신에게 친구신청할 수 없습니다.");
        }
        if (m_myHero.ContainsFriendApplication(targetHeroId))
        {
            throw new CommandHandleException(101, "대상에 대한 친구신청이 존재합니다.");
        }
        if (m_myHero.IsFriend(targetHeroId))
        {
            throw new CommandHandleException(102, "대상은 이미 친구입니다.");
        }
        if (m_myHero.IsBlacklistEntry(targetHeroId))
        {
            throw new CommandHandleException(104, "대상은 나의 블랙리스트에 존재합니다.");
        }
        m_target = Cache.instance.GetLoggedInHero(targetHeroId);
        if (m_target == null)
        {
            throw new CommandHandleException(103, "대상영웅이 로그인중이 아닙니다.");
        }
        lock (m_target.syncObject)
        {
            Process();
        }
    }

    private void Process()
    {
        if (m_target.IsBlacklistEntry(m_myHero.id))
        {
            throw new CommandHandleException(105, "나는 대상영웅의 블랙리스트에 존재합니다.");
        }
        FriendApplication app = m_myHero.ApplyFriend(m_target);
        FriendApplyResponseBody resBody = new FriendApplyResponseBody();
        resBody.app = app.ToPDFriendApplication();
        SendResponseOK(resBody);
    }
}
