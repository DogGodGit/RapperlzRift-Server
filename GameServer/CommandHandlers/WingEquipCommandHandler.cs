using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class WingEquipCommandHandler : InGameCommandHandler<WingEquipCommandBody, WingEquipResponseBody>
{
    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nWingId = m_body.wingId;
        if (nWingId <= 0)
        {
            throw new CommandHandleException(1, "날개ID가 유효하지 않습니다. nWingId = " + nWingId);
        }
        HeroWing heroWing = m_myHero.GetWing(nWingId);
        if (heroWing == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 영웅 날개입니다. nWingId = " + nWingId);
        }
        if (heroWing.isEquipped)
        {
            throw new CommandHandleException(1, "이미 장착한 영웅 날개입니다. nWingId = " + nWingId);
        }
        m_myHero.EquipWing(heroWing);
        SaveToDB();
        SendResponseOK(null);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_EquipWing(m_myHero.id, m_myHero.equippedWingId));
        dbWork.Schedule();
    }
}
