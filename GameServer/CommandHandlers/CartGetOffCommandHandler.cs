using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class CartGetOffCommandHandler : InGameCommandHandler<CartGetOffCommandBody, CartGetOffResponseBody>
{
    protected override void HandleInGameCommand()
    {
        if (!(m_myHero.currentPlace is ContinentInstance))
        {
            throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
        }
        CartInstance cartInst = m_myHero.ridingCartInst;
        if (cartInst == null)
        {
            throw new CommandHandleException(1, "카트를 탑승하고 있지 않습니다.");
        }
        lock (cartInst.syncObject)
        {
            cartInst.GetOff(DateTimeUtil.currentTime, bSendEvent: true);
            SaveToDB(cartInst);
            SendResponseOK(null);
        }
    }

    private void SaveToDB(CartInstance cartInst)
    {
        switch (cartInst.cartInstanceType)
        {
            case CartInstanceType.MainQuest:
                SaveToDB_MainQuest((MainQuestCartInstance)cartInst);
                break;
            case CartInstanceType.SupplySupportQuest:
                SaveToDB_SupplySupportQuest((SupplySupportQuestCartInstance)cartInst);
                break;
            case CartInstanceType.GuildSupplySupportQuest:
                SaveToDB_GuildSupplySupportQuest((GuildSupplySupportQuestCartInstance)cartInst);
                break;
        }
    }

    private void SaveToDB_MainQuest(MainQuestCartInstance cartInst)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHeroMainQuest_Cart(cartInst.quest));
        dbWork.Schedule();
    }

    private void SaveToDB_SupplySupportQuest(SupplySupportQuestCartInstance cartInst)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateSupplySupportQuest_Cart(cartInst.quest));
        dbWork.Schedule();
    }

    private void SaveToDB_GuildSupplySupportQuest(GuildSupplySupportQuestCartInstance cartInst)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateGuildSupplySupportQuest_Cart(cartInst.quest));
        dbWork.Schedule();
    }
}
