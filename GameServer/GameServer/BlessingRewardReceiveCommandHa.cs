using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class BlessingRewardReceiveCommandHandler : InGameCommandHandler<BlessingRewardReceiveCommandBody, BlessingRewardReceiveResponseBody>
{
	public enum FriendApplicationResult
	{
		OK,
		ApplicationExist,
		AlreadyFriend,
		ExistInMyBlacklist,
		TargetNotLoggedIn,
		ExistInTargetBlacklist
	}

	public const short kResult_ReceivedBlessingNotExist = 101;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private HeroBlessing m_heroBlessing;

	private long m_lnRewardGold;

	private FriendApplicationResult m_friendApplicationResult;

	private FriendApplication m_friendApplication;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		long lnInstanceId = m_body.instanceId;
		if (lnInstanceId <= 0)
		{
			throw new CommandHandleException(1, "축복인스턴스ID가 유효하지 않습니다. lnInstanceId = " + lnInstanceId);
		}
		m_currentTime = DateTimeUtil.currentTime;
		m_heroBlessing = m_myHero.GetReceivedBlessing(lnInstanceId);
		if (m_heroBlessing == null)
		{
			throw new CommandHandleException(101, "해당 축복이 존재하지 않습니다. lnInstanceId = " + lnInstanceId);
		}
		m_lnRewardGold = m_heroBlessing.blessing.receiverGoldRewardValue;
		m_myHero.AddGold(m_lnRewardGold);
		m_myHero.RemoveReceivedBlessing(lnInstanceId);
		Hero target = Cache.instance.GetLoggedInHero(m_heroBlessing.senderHeroId);
		if (target != null)
		{
			ServerEvent.SendBlessingThanksMessageReceived(target.account.peer, m_myHero.id, m_myHero.name);
			lock (target.syncObject)
			{
				ProcessFriendApplication(target);
				Finish();
				return;
			}
		}
		m_friendApplicationResult = FriendApplicationResult.TargetNotLoggedIn;
		Finish();
	}

	private void ProcessFriendApplication(Hero target)
	{
		if (m_myHero.ContainsFriendApplication(target.id))
		{
			m_friendApplicationResult = FriendApplicationResult.ApplicationExist;
			return;
		}
		if (m_myHero.IsFriend(target.id))
		{
			m_friendApplicationResult = FriendApplicationResult.AlreadyFriend;
			return;
		}
		if (m_myHero.IsBlacklistEntry(target.id))
		{
			m_friendApplicationResult = FriendApplicationResult.ExistInMyBlacklist;
			return;
		}
		if (target.IsBlacklistEntry(m_myHero.id))
		{
			m_friendApplicationResult = FriendApplicationResult.ExistInTargetBlacklist;
			return;
		}
		m_friendApplication = m_myHero.ApplyFriend(target);
		m_friendApplicationResult = FriendApplicationResult.OK;
	}

	private void Finish()
	{
		SaveToDB();
		SaveToGameLogDB();
		BlessingRewardReceiveResponseBody resBody = new BlessingRewardReceiveResponseBody();
		resBody.gold = m_myHero.gold;
		resBody.maxGold = m_myHero.maxGold;
		resBody.friendApplicationResult = (int)m_friendApplicationResult;
		resBody.friendAppication = ((m_friendApplication != null) ? m_friendApplication.ToPDFriendApplication() : null);
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroBlessingReceivingLog(Guid.NewGuid(), m_heroBlessing.sendingLogId, m_lnRewardGold, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
