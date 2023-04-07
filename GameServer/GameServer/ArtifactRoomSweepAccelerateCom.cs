using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class ArtifactRoomSweepAccelerateCommadHandler : InGameCommandHandler<ArtifactRoomSweepAccelerateCommandBody, ArtifactRoomSweepAccelerateResponseBody>
{
	public const short kResult_NotSweeping = 101;

	public const short kResult_NotEnoughtDia = 102;

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
		if (m_myHero.isArtifactRoomSweepCompleted)
		{
			throw new CommandHandleException(1, "이미 소탕을 완료했습니다.");
		}
		int nHeroBestFloor = m_myHero.artifactRoomBestFloor;
		int nTotalSweepDia = ArtifactRoomFloor.GetTotalSweepDia(m_myHero.artifactRoomSweepProgressFloor, nHeroBestFloor);
		int nHeroDia = m_myHero.dia;
		if (nHeroDia < nTotalSweepDia)
		{
			throw new CommandHandleException(102, "소탕에 필요한 다이아가 부족합니다. nHeroDia = " + nHeroDia + ", nTotalSweepDia = " + nTotalSweepDia);
		}
		m_currentTime = DateTimeUtil.currentTime;
		int nUsedOwnDia = 0;
		int nUsedUnOwnDia = 0;
		m_myHero.UseDia(nTotalSweepDia, m_currentTime, out nUsedOwnDia, out nUsedUnOwnDia);
		int nStartFloor = m_myHero.artifactRoomCurrentFloor;
		m_myHero.CompleteArtifactRoomSweep();
		m_myHero.SetArtifactRoomCurrentFloor(m_myHero.artifactRoomBestFloor + 1);
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
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB(nStartFloor, nUsedOwnDia, nUsedUnOwnDia);
		ArtifactRoomSweepAccelerateResponseBody resBody = new ArtifactRoomSweepAccelerateResponseBody();
		resBody.currentFloor = m_myHero.artifactRoomCurrentFloor;
		resBody.booties = m_booties.ToArray();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nOldFloor, int nUsedOwnDia, int nUsedUnOwnDia)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ArtifactRoomCurrentFloor(m_myHero.id, m_myHero.artifactRoomCurrentFloor));
		if (nUsedUnOwnDia > 0)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		}
		if (nUsedOwnDia > 0)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_OwnDia(m_myHero.id, m_myHero.ownDia));
		}
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
		SaveToDB_Log(nOldFloor, nUsedOwnDia, nUsedUnOwnDia);
	}

	private void SaveToDB_Log(int nOldFloor, int nUsedOwnDia, int nUsedUnOwnDia)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddArtifactRoomPlayLog(Guid.NewGuid(), m_myHero.id, 3, nOldFloor, m_myHero.artifactRoomCurrentFloor, m_myHero.artifactRoomBestFloor, nUsedOwnDia, nUsedUnOwnDia, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
