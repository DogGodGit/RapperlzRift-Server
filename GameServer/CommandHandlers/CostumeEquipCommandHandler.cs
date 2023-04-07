using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class CostumeEquipCommandHandler : InGameCommandHandler<CostumeEquipCommandBody, CostumeEquipResponseBody>
{
    public const short kResult_NotEnoughLevel = 101;

    private HeroCostume m_heroCostume;

    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nCostumeId = m_body.costumeId;
        if (nCostumeId <= 0)
        {
            throw new CommandHandleException(1, "코스튬ID가 유효하지 않습니다. nCostumeId = " + nCostumeId);
        }
        m_heroCostume = m_myHero.GetCostume(nCostumeId);
        if (m_heroCostume == null)
        {
            throw new CommandHandleException(1, "영웅코스튬이 존재하지 않습니다. nCostumeId = " + nCostumeId);
        }
        if (m_heroCostume.isEquipped)
        {
            throw new CommandHandleException(1, "이미 장착중인 코스튬 입니다.");
        }
        if (m_heroCostume.costume.requiredHeroLevel > m_myHero.level)
        {
            throw new CommandHandleException(101, "영웅레벨이 부족합니다.");
        }
        m_myHero.EquipCostume(m_heroCostume);
        SaveToDB();
        SendResponseOK(null);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_EquippedHeroCostume(m_myHero.id, m_heroCostume.costumeId));
        dbWork.Schedule();
    }
}
