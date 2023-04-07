using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class FriendDeleteCommandHandler : InGameCommandHandler<FriendDeleteCommandBody, FriendDeleteResponseBody>
{
    public const short kResult_NotFriend = 101;

    public const short kResult_FriendIdDuplicated = 102;

    private HashSet<Guid> m_targetIds = new HashSet<Guid>();

    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        Guid[] friendIds = (Guid[])(object)m_body.friendIds;
        if (friendIds == null || friendIds.Length == 0)
        {
            throw new CommandHandleException(1, "친구ID목록이 유효하지 않습니다.");
        }
        Guid[] array = friendIds;
        foreach (Guid friendId in array)
        {
            if (!m_myHero.IsFriend(friendId))
            {
                throw new CommandHandleException(101, "친구가 아닙니다. friendId = " + friendId);
            }
            if (!m_targetIds.Add(friendId))
            {
                throw new CommandHandleException(102, "친구ID가 중복되었습니다. friendId = " + friendId);
            }
        }
        foreach (Guid targetId in m_targetIds)
        {
            m_myHero.RemoveFriend(targetId);
        }
        SaveToDB();
        SendResponseOK(null);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        foreach (Guid targetId in m_targetIds)
        {
            dbWork.AddSqlCommand(GameDac.CSC_DeleteFriend(m_myHero.id, targetId));
        }
        dbWork.Schedule();
    }
}
