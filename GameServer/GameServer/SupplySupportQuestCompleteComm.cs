using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class SupplySupportQuestCompleteCommandHandler : InGameCommandHandler<SupplySupportQuestCompleteCommandBody, SupplySupportQuestCompleteResponseBody>
{
	public const short kResult_UnableQuestCompletePositionWithCompletionNPC_Hero = 101;

	public const short kResult_UnableQuestCompletePositionWithCompletionNPC_Cart = 102;

	private HeroSupplySupportQuest m_heroQuest;

	private SupplySupportQuest m_quest;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private float m_fTime;

	private long m_lnRewardExp;

	private int m_nRewardExploitPoint;

	private int m_nAcquiredExploitPoint;

	private long m_lnRewardGold;

	private SupplySupportQuestCartInstance m_cartInst;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_quest = Resource.instance.supplySupportQuest;
		m_currentTime = DateTimeUtil.currentTime;
		m_fTime = (float)m_currentTime.TimeOfDay.TotalSeconds;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		m_heroQuest = m_myHero.supplySupportQuest;
		if (m_heroQuest == null)
		{
			throw new CommandHandleException(1, "현재 진행중인 퀘스트가 존재하지 않습니다.");
		}
		Npc completionNpc = m_quest.completionNpc;
		if (!currentPlace.IsSame(completionNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "영웅이 퀘스트완료NPC와 다른장소에 있습니다.");
		}
		if (!completionNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(101, "영웅이 퀘스트완료NPC 상호작용 범위에 있지 않습니다.");
		}
		m_cartInst = m_heroQuest.cartInst;
		lock (m_cartInst.syncObject)
		{
			ValidationCheckCart();
		}
	}

	private void ValidationCheckCart()
	{
		Npc completionNpc = m_quest.completionNpc;
		ContinentInstance cartPlace = (ContinentInstance)m_cartInst.currentPlace;
		if (!cartPlace.IsSame(completionNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "카트가 퀘스트완료NPC와 다른장소에 있습니다.");
		}
		if (!completionNpc.IsInteractionEnabledPosition(m_cartInst.position, m_myHero.radius))
		{
			throw new CommandHandleException(102, "카트가 퀘스트완료NPC 주위에 있지 않습니다");
		}
		Reward();
	}

	private void Reward()
	{
		SupplySupportQuestReward reward = m_quest.GetReward(m_heroQuest.cart.id, m_myHero.level);
		if (reward != null)
		{
			m_lnRewardExp = reward.expRewardValue;
			m_lnRewardExp = (long)Math.Floor((float)m_lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
			m_myHero.AddExp(m_lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
			m_lnRewardGold = reward.goldRewardValue;
			m_myHero.AddGold(m_lnRewardGold);
			m_nRewardExploitPoint = reward.exploitPointRewardValue;
			BattlefieldSupportEvent battlefieldSupportEvent = Resource.instance.battlefieldSupportEvent;
			if (battlefieldSupportEvent != null && battlefieldSupportEvent.IsEventTime(m_fTime))
			{
				m_nRewardExploitPoint = (int)Math.Floor((float)m_nRewardExploitPoint * battlefieldSupportEvent.supplySupportQuestExploitPointFactor);
			}
			m_nAcquiredExploitPoint = m_myHero.AddExploitPoint(m_nRewardExploitPoint, m_currentTime, bSaveToDB: false);
		}
		m_myHero.supplySupportQuest.Release();
		m_myHero.supplySupportQuest = null;
		RemoveCart();
	}

	private void RemoveCart()
	{
		if (m_cartInst.isRiding)
		{
			m_cartInst.GetOff(m_currentTime, bSendEvent: false);
			ServerEvent.SendHeroEnter(m_myHero.currentPlace.GetDynamicClientPeers(m_myHero.sector, m_myHero.id), m_myHero.ToPDHero(m_currentTime), bIsRevivalEnter: false);
		}
		((ContinentInstance)m_cartInst.currentPlace)?.ExitCart(m_cartInst, bSendEvent: true, bResetPlaceReferenceOfCartInst: true);
		Cache.instance.RemoveCartInstance(m_cartInst);
		m_heroQuest.cartInst = null;
		Finish();
	}

	private void Finish()
	{
		SaveToDB();
		SaveToDB_Log();
		SupplySupportQuestCompleteResponseBody resBody = new SupplySupportQuestCompleteResponseBody();
		resBody.acquiredExp = m_lnRewardExp;
		resBody.acquiredExploitPoint = m_nAcquiredExploitPoint;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.exploitPoint = m_myHero.exploitPoint;
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.gold = m_myHero.gold;
		resBody.maxGold = m_myHero.maxGold;
		resBody.date = (DateTime)m_currentTime.Date;
		resBody.dailyExploitPoint = m_myHero.dailyExploitPoint.value;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateSupplySupportQuest_Status(m_heroQuest.id, 1, m_currentTime));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Exploit(m_myHero));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddSupplySupportQuestRewardLog(Guid.NewGuid(), m_myHero.id, m_heroQuest.id, 1, m_heroQuest.cart.id, m_lnRewardExp, m_lnRewardGold, m_nRewardExploitPoint, m_nAcquiredExploitPoint, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
