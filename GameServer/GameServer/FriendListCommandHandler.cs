using ClientCommon;

namespace GameServer;

public class FriendListCommandHandler : InGameCommandHandler<FriendListCommandBody, FriendListResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		FriendListResponseBody resBody = new FriendListResponseBody();
		resBody.friends = m_myHero.GetPDFriends().ToArray();
		SendResponseOK(resBody);
	}
}
