using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class ArtifactRoomSweepCompleteCommandHandler : InGameCommandHandler<ArtifactRoomSweepCompleteCommandBody, ArtifactRoomSweepCompleteResponseBody>
{
	public const short kResult_NotSweeping = 101;

	public const short kResult_NotCompletedSweep = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private List<PDItemBooty> m_booties = new List<PDItemBooty>();

	private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

	private Mail m_mail;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!m_myHero.artifactRoomSweepStartTime.HasValue)
		{
			throw new CommandHandleException(101, "현재 소탕중이 아닙니다.");
		}
		if (!m_myHero.isArtifactRoomSweepCompleted)
		{
			throw new CommandHandleException(102, "아직 소탕이 완료되지 않았습니다.");
		}
		m_currentTime = DateTimeUtil.currentTime;
		int nHeroBestFloor = m_myHero.artifactRoomBestFloor;
		int nStartFloor = m_myHero.artifactRoomCurrentFloor;
		foreach (ArtifactRoomFloor floor in Resource.instance.artifactRoom.floors)
		{
			if (floor.floor > nHeroBestFloor)
			{
				break;
			}
			if (floor.floor < nStartFloor)
			{
				continue;
			}
			ItemReward itemReward = floor.itemReward;
			Item item = itemReward.item;
			int nCount = itemReward.count;
			bool bOwned = itemReward.owned;
			int nRewardItemRemainCount = m_myHero.AddItem(item, bOwned, nCount, m_changedInventorySlots);
			PDItemBooty booty = new PDItemBooty();
			booty.id = item.id;
			booty.count = nCount;
			booty.owned = bOwned;
			m_booties.Add(booty);
			if (nRewardItemRemainCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_REWARD_N_3", "MAIL_REWARD_D_3", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(item, nRewardItemRemainCount, bOwned));
			}
		}
		m_myHero.SetArtifactRoomCurrentFloor(nHeroBestFloor + 1);
		m_myHero.CompleteArtifactRoomSweep();
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB(nStartFloor);
		ArtifactRoomSweepCompleteResponseBody resBody = new ArtifactRoomSweepCompleteResponseBody();
		resBody.currentFloor = m_myHero.artifactRoomCurrentFloor;
		resBody.booties = m_booties.ToArray();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nOldFloor)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ArtifactRoomCurrentFloor(m_myHero.id, m_myHero.artifactRoomCurrentFloor));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
		SaveToDB_Log(nOldFloor);
	}

	private void SaveToDB_Log(int nOldFloor)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddArtifactRoomPlayLog(Guid.NewGuid(), m_myHero.id, 2, nOldFloor, m_myHero.artifactRoomCurrentFloor, m_myHero.artifactRoomBestFloor, 0, 0, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
