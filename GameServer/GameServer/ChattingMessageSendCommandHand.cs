using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class ChattingMessageSendCommandHandler : InGameCommandHandler<ChattingMessageSendCommandBody, ChattingMessageSendResponseBody>
{
	public const short kResult_MessageLengthOverflowed = 101;

	public const short kResult_ChattingIntervalNotElapsed = 102;

	public const short kResult_NoPartyMember = 103;

	public const short kResult_TargetIsSelf = 104;

	public const short kResult_TargetNotExist = 105;

	public const short kResult_NotEnoughItem = 106;

	public const short kResult_NoGuildMember = 107;

	private int m_nType;

	private Guid m_targetHeroId = Guid.Empty;

	private string[] m_messages;

	private PDChattingLink m_link;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private Hero m_target;

	private InventorySlot m_changedInventorySlot;

	protected override bool globalLockRequired => true;

	protected override void PreHandleCommand()
	{
		base.PreHandleCommand();
		if (m_body == null)
		{
			throw new CommandHandleException(1, "바디가 null입니다.");
		}
		m_nType = m_body.type;
		m_messages = m_body.messages;
		if (m_messages == null || m_messages.Length == 0)
		{
			throw new CommandHandleException(1, "메시지가 없습니다.");
		}
		int nTotalMessageLength = 0;
		string[] messages = m_messages;
		foreach (string sMessage in messages)
		{
			if (sMessage != null)
			{
				nTotalMessageLength += sMessage.Length;
			}
		}
		if (nTotalMessageLength > Resource.instance.chattingMaxLength)
		{
			throw new CommandHandleException(101, "메시지 길이가 최대 길이를 초과합니다.");
		}
		m_link = m_body.link;
		m_targetHeroId = (Guid)m_body.targetHeroId;
	}

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (!m_myHero.IsChattingIntervalElapsed(m_currentTime))
		{
			throw new CommandHandleException(102, "최소 채팅간격이 경과되지 않았습니다.");
		}
		switch (m_nType)
		{
		case 1:
			ProcessNation();
			break;
		case 2:
			ProcessAlliance();
			break;
		case 3:
			ProcessWorld();
			break;
		case 4:
			ProcessParty();
			break;
		case 5:
			ProcessGuild();
			break;
		case 6:
			Process1vs1();
			break;
		default:
			throw new EventHandleException("채팅타입이 유효하지 않습니다. m_nType = " + m_nType);
		}
		m_myHero.lastChattingTime = m_currentTime;
		ChattingMessageSendResponseBody resBody = new ChattingMessageSendResponseBody();
		resBody.changedInventorySlot = ((m_changedInventorySlot != null) ? m_changedInventorySlot.ToPDInventorySlot() : null);
		SendResponseOK(resBody);
		SaveToGameLogDB();
	}

	private void ProcessNation()
	{
		SendEvent(m_myHero.nationInst.GetClientPeers(Guid.Empty));
	}

	private void ProcessAlliance()
	{
		List<ClientPeer> targetPeers = new List<ClientPeer>();
		NationInstance myNationInst = m_myHero.nationInst;
		targetPeers.AddRange(myNationInst.GetClientPeers(Guid.Empty));
		NationAlliance alliance = myNationInst.alliance;
		if (alliance != null)
		{
			NationInstance allianceNationInst = alliance.GetAllianceNation(myNationInst.nationId);
			targetPeers.AddRange(allianceNationInst.GetClientPeers(Guid.Empty));
		}
		SendEvent(targetPeers);
	}

	private void ProcessWorld()
	{
		int nItemId = Resource.instance.worldChattingItemId;
		if (nItemId > 0)
		{
			if (m_myHero.GetItemCount(nItemId) <= 0)
			{
				throw new CommandHandleException(106, "세계채팅 아이템이 부족합니다.");
			}
			List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
			int nUsedOwnCount = 0;
			int nUsedUnOwnCount = 0;
			m_myHero.UseItem(nItemId, bFisetUseOwn: true, 1, changedInventorySlots, out nUsedOwnCount, out nUsedUnOwnCount);
			m_changedInventorySlot = changedInventorySlots[0];
			ProcessWorld_SaveToDB(m_changedInventorySlot);
			ProcessWorld_SaveToGameLogDB(nItemId, nUsedOwnCount, nUsedUnOwnCount);
		}
		SendEvent(Cache.instance.GetClientPeers(Guid.Empty));
	}

	private void ProcessWorld_SaveToDB(InventorySlot changedInventorySlot)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(changedInventorySlot));
		dbWork.Schedule();
	}

	private void ProcessWorld_SaveToGameLogDB(int nItemId, int nUsedOwnCount, int nUsedUnOwnCount)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, nItemId, nUsedOwnCount, nUsedUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}

	private void ProcessParty()
	{
		PartyMember partyMember = m_myHero.partyMember;
		if (partyMember == null)
		{
			throw new CommandHandleException(103, "파티에 가입되어 있지 않습니다.");
		}
		Party party = partyMember.party;
		SendEvent(party.GetClientPeers(Guid.Empty));
	}

	private void ProcessGuild()
	{
		GuildMember guildMember = m_myHero.guildMember;
		if (guildMember == null)
		{
			throw new CommandHandleException(107, "길드에 가입되어 있지 않습니다.");
		}
		Guild guild = guildMember.guild;
		SendEvent(guild.GetClientPeers(Guid.Empty));
	}

	private void Process1vs1()
	{
		if (m_targetHeroId == Guid.Empty)
		{
			throw new CommandHandleException(1, "대상ID가 유효하지 않습니다.");
		}
		if (m_targetHeroId == m_myHero.id)
		{
			throw new CommandHandleException(104, "자신에게 메시지를 보낼 수 없습니다.");
		}
		m_target = Cache.instance.GetLoggedInHero(m_targetHeroId);
		if (m_target == null)
		{
			throw new CommandHandleException(105, "대상영웅이 존재하지 않습니다.", null, bLoggingEnabled: false);
		}
		List<ClientPeer> peers = new List<ClientPeer>();
		peers.Add(m_myHero.account.peer);
		peers.Add(m_target.account.peer);
		SendEvent(peers);
		lock (m_target.syncObject)
		{
			m_myHero.AddTempFriend(m_target, m_currentTime);
			m_target.AddTempFriend(m_myHero, m_currentTime);
		}
	}

	private void SendEvent(IEnumerable<ClientPeer> peers)
	{
		ServerEvent.SendChattingMessageReceived(peers, m_nType, m_messages, m_link, m_myHero.ToPDSimpleHero(), (m_target != null) ? m_target.ToPDSimpleHeroWithLock() : null);
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			int nLinkType = ((m_link != null) ? m_link.type : 0);
			Guid? targetId = ((m_target != null) ? new Guid?(m_target.id) : null);
			logWork.AddSqlCommand(GameLogDac.CSC_AddChattingLog(logId, m_nType, nLinkType, m_myHero.id, targetId, m_currentTime));
			for (int i = 0; i < m_messages.Length; i++)
			{
				string sMessage = m_messages[i];
				logWork.AddSqlCommand(GameLogDac.CSC_AddChattingMessageLog(logId, i + 1, sMessage));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
