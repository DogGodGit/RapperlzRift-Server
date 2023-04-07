using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class FriendApplicationAcceptCommandHandler : InGameCommandHandler<FriendApplicationAcceptCommandBody, FriendApplicationAcceptResponseBody>
{
	public const short kResult_ApplicationNotExist = 101;

	public const short kResult_MyFriendCountIsMax = 102;

	public const short kResult_ApplicantFriendCountIsMax = 103;

	public const short kResult_ExistInMyBlacklist = 104;

	public const short kResult_ExistInApplicantBlacklist = 105;

	private FriendApplication m_app;

	private Hero m_applicant;

	private Friend m_newFriend;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		long lnApplicationNo = m_body.applicationNo;
		if (lnApplicationNo <= 0)
		{
			throw new CommandHandleException(1, "신청번호가 유효하지 않습니다. lnApplicationNo = " + lnApplicationNo);
		}
		m_app = m_myHero.GetReceivedFriendApplication(lnApplicationNo);
		if (m_app == null)
		{
			throw new CommandHandleException(101, "신청이 존재하지 않습니다. lnApplicationNo = " + lnApplicationNo);
		}
		m_applicant = m_app.applicant;
		if (m_myHero.isFriendListFull)
		{
			throw new CommandHandleException(102, "나의 친구수가 최대입니다.");
		}
		if (m_myHero.IsBlacklistEntry(m_applicant.id))
		{
			throw new CommandHandleException(104, "신청자는 나의 블랙리스트에 존재합니다.");
		}
		lock (m_applicant.syncObject)
		{
			Process();
		}
	}

	private void Process()
	{
		if (m_applicant.isFriendListFull)
		{
			throw new CommandHandleException(103, "신청자의 친구수가 최대입니다.");
		}
		if (m_applicant.IsBlacklistEntry(m_myHero.id))
		{
			throw new CommandHandleException(105, "나는 신청자의 블랙리스트에 존재합니다.");
		}
		m_newFriend = m_myHero.AcceptFriendApplication(m_app);
		SaveToDB();
		FriendApplicationAcceptResponseBody resBody = new FriendApplicationAcceptResponseBody();
		if (m_newFriend != null)
		{
			resBody.newFriend = m_newFriend.ToPDFriend();
		}
		SendResponseOK(resBody);
		m_myHero.ProcessSubQuestForContent(26);
		m_applicant.ProcessSubQuestForContent(26);
	}

	private void SaveToDB()
	{
		if (m_newFriend != null)
		{
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
			dbWork.AddSqlCommand(GameDac.CSC_AddFriend(m_myHero.id, m_newFriend.id));
			dbWork.Schedule();
		}
	}
}
