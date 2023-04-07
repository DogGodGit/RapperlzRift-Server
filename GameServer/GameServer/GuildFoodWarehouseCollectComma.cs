using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class GuildFoodWarehouseCollectCommandHandler : InGameCommandHandler<GuildFoodWarehouseCollectCommandBody, GuildFoodWarehouseCollectResponseBody>
{
	public const short kResult_NoAuthority = 101;

	public const short kResult_NotMaxLevel = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private Guild m_guild;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		GuildFoodWarehouse warehouse = Resource.instance.guildFoodWarehouse;
		if (!(m_myHero.currentPlace is GuildTerritoryInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소가 길드영지가 아닙니다.");
		}
		m_guild = currentPlace.guild;
		if (!m_myHero.guildMember.grade.foodWarehouseRewardCollectionEnabled)
		{
			throw new CommandHandleException(101, "권한이 없습니다.");
		}
		GuildTerritoryNpc npc = warehouse.npc;
		if (!npc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		if (!m_guild.isFoodWarehouseLevelMax)
		{
			throw new CommandHandleException(102, "군량창고가 최대레벨이 아닙니다.");
		}
		m_guild.CollectFoodWarehouse();
		SaveToDB();
		SaveToGameLogDB();
		ServerEvent.SendGuildFoodWarehouseCollected(m_guild.GetClientPeers(m_myHero.id), m_guild.foodWarehouseCollectionId);
		GuildFoodWarehouseCollectResponseBody resBody = new GuildFoodWarehouseCollectResponseBody();
		resBody.collectionId = (Guid)m_guild.foodWarehouseCollectionId;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_guild.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateGuild_FoodWarehouse(m_guild));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildFoodWarehouseCollectionLog(m_guild.foodWarehouseCollectionId, m_guild.id, m_myHero.id, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
