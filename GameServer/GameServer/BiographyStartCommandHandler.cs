using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class BiographyStartCommandHandler : InGameCommandHandler<BiographyStartCommandBody, BiographyStartResponseBody>
{
	public const short kResult_AlreadyStartedBiography = 101;

	public const short kResult_NotEnoughItem = 102;

	private HeroBiography m_heroBiography;

	private int m_nItemId;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

	private InventorySlot m_changedInventorySlot;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nBiographyId = m_body.biographyId;
		if (nBiographyId <= 0)
		{
			throw new CommandHandleException(1, "전기ID가 유효하지 않습니다. nBiographyId = " + nBiographyId);
		}
		if (m_myHero.IsStartedBiography(nBiographyId))
		{
			throw new CommandHandleException(101, "이미 시작한 전기입니다. nBiographyId = " + nBiographyId);
		}
		Biography biography = Resource.instance.GetBiography(nBiographyId);
		if (biography == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 전기입니다. nBiographyId = " + nBiographyId);
		}
		Item requiredItem = biography.requiredItem;
		m_nItemId = requiredItem.id;
		if (m_myHero.GetItemCount(m_nItemId) < 1)
		{
			throw new CommandHandleException(102, "아이템이 부족합니다.");
		}
		if (!requiredItem.IsUseableHeroLevel(m_myHero.level))
		{
			throw new CommandHandleException(1, "아이템을 사용할 수 있는 레벨이 아닙니다.");
		}
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		m_myHero.UseItem(m_nItemId, bFisetUseOwn: true, 1, changedInventorySlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
		m_changedInventorySlot = changedInventorySlots[0];
		m_heroBiography = new HeroBiography(m_myHero, biography);
		m_myHero.AddBiography(m_heroBiography);
		SaveToDB();
		SaveToLogDB();
		BiographyStartResponseBody resBody = new BiographyStartResponseBody();
		resBody.changedInventorySlot = m_changedInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroBiography(m_heroBiography.hero.id, m_heroBiography.biography.id, m_currentTime));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_changedInventorySlot));
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroBiographyStartLog(Guid.NewGuid(), m_myHero.id, m_heroBiography.biography.id, m_nItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
