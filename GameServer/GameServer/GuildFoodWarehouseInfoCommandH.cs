using ClientCommon;

namespace GameServer;

public class GuildFoodWarehouseInfoCommandHandler : InGameCommandHandler<GuildFoodWarehouseInfoCommandBody, GuildFoodWarehouseInfoResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is GuildTerritoryInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소가 길드영지가 아닙니다.");
		}
		Guild guild = currentPlace.guild;
		GuildFoodWarehouse warehouse = Resource.instance.guildFoodWarehouse;
		GuildTerritoryNpc npc = warehouse.npc;
		if (!npc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		GuildFoodWarehouseInfoResponseBody resBody = new GuildFoodWarehouseInfoResponseBody();
		resBody.level = guild.foodWarehouseLevel;
		resBody.exp = guild.foodWarehouseExp;
		SendResponseOK(resBody);
	}
}
