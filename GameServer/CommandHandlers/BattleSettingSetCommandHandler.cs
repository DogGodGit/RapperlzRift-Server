using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class BattleSettingSetCommandHandler : InGameCommandHandler<BattleSettingSetCommandBody, BattleSettingSetResponseBody>
{
    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nLootingItemMinGrade = m_body.lootingItemMinGrade;
        if (nLootingItemMinGrade <= 0)
        {
            throw new CommandHandleException(1, "루팅아이템최소등급이 유효하지 않습니다. nLootingItemMinGrade = " + nLootingItemMinGrade);
        }
        if (!Enum.IsDefined(typeof(LootingItemMinGrade), nLootingItemMinGrade))
        {
            throw new CommandHandleException(1, "존재하지 않는 루팅아이템최소등급입니다. nLootingItemMinGrade = " + nLootingItemMinGrade);
        }
        m_myHero.SetBattleSetting((LootingItemMinGrade)nLootingItemMinGrade);
        SaveToDB();
        SendResponseOK(null);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_BattleSetting(m_myHero.id, (int)m_myHero.lootingItemMinGrade));
        dbWork.Schedule();
    }
}
