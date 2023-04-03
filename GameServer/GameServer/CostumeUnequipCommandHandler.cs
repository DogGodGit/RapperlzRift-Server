using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CostumeUnequipCommandHandler : InGameCommandHandler<CostumeUnequipCommandBody, CostumeUnequipResponseBody>
{
	protected override void HandleInGameCommand()
	{
		if (m_myHero.equippedCostume == null)
		{
			throw new CommandHandleException(1, "장착중인 코스튬이 없습니다.");
		}
		m_myHero.UnequipCostume();
		SaveToDB();
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_EquippedHeroCostume(m_myHero.id, 0));
		dbWork.Schedule();
	}
}
