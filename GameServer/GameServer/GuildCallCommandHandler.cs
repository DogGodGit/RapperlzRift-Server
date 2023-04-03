using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildCallCommandHandler : InGameCommandHandler<GuildCallCommandBody, GuildCallResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_NoAuthority = 102;

	public const short kResult_NotEnoughItem = 103;

	public const short kResult_NationWar = 104;

	private Guild m_myGuild;

	private GuildMember m_myGuildMember;

	private InventorySlot m_targetInventorySlot;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nSlotIndex = m_body.slotIndex;
		if (nSlotIndex < 0)
		{
			throw new CommandHandleException(1, "슬롯 인덱스가 유효하지 않습니다. nSlotIndex = " + nSlotIndex);
		}
		NationWarInstance nationWarInst = m_myHero.nationInst.nationWarInst;
		if (nationWarInst != null)
		{
			throw new CommandHandleException(104, "국가전이 진행중입니다.");
		}
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		m_myGuildMember = m_myHero.guildMember;
		if (m_myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어있지 않습니다.");
		}
		if (!m_myGuildMember.grade.guildCallEnabled)
		{
			throw new CommandHandleException(102, "길드를 소집할 권한이 없습니다.");
		}
		m_myGuild = m_myGuildMember.guild;
		m_targetInventorySlot = m_myHero.GetInventorySlot(nSlotIndex);
		if (m_targetInventorySlot == null)
		{
			throw new CommandHandleException(1, "존재하지 않은 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
		}
		if (m_targetInventorySlot.isEmpty)
		{
			throw new CommandHandleException(1, "빈 슬롯 입니다. nSlotIndex = " + nSlotIndex);
		}
		if (m_targetInventorySlot.obj.inventoryObjectType != 3)
		{
			throw new CommandHandleException(1, "해당 슬롯 오브젝트 타입이 아이템이 아닙니다. nSlotIndex = " + nSlotIndex);
		}
		ItemInventoryObject itemInventoryObject = (ItemInventoryObject)m_targetInventorySlot.obj;
		Item targetItem = itemInventoryObject.item;
		bool itemOwned = itemInventoryObject.owned;
		if (itemInventoryObject.count < 1)
		{
			throw new CommandHandleException(103, "아이템이 부족합니다.");
		}
		if (!targetItem.IsUseableHeroLevel(m_myHero.level))
		{
			throw new CommandHandleException(1, "아이템을 사용할 수 없는 영웅레벨입니다.");
		}
		if (targetItem.type.id != 23)
		{
			throw new CommandHandleException(1, "아이템 타입이 길드소집아이템이 아닙니다.");
		}
		m_myHero.UseItem(nSlotIndex, 1);
		m_myGuild.CallGuildMembers(m_myHero, currentPlace.continent, currentPlace.nationId);
		SaveToDB();
		SaveToGameLogDB(targetItem.id, itemOwned, 1);
		GuildCallResponseBody resBody = new GuildCallResponseBody();
		resBody.changedInventorySlot = m_targetInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_targetInventorySlot));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nItemId, bool bItemOwned, int nUsedItemCount)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			int nItemOwnCount = 0;
			int nItemUnOwnCount = 0;
			if (bItemOwned)
			{
				nItemOwnCount = nUsedItemCount;
			}
			else
			{
				nItemUnOwnCount = nUsedItemCount;
			}
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, nItemId, nItemOwnCount, nItemUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
