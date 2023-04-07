using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class MainGearEquipCommandHandler : InGameCommandHandler<MainGearEquipCommandBody, MainGearEquipResponseBody>
{
    private HeroMainGear m_targetMainGear;

    private InventorySlot m_targetInventorySlot;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        Guid herMainGearId = m_body.heroMainGearId;
        m_targetMainGear = m_myHero.GetMainGear(herMainGearId);
        if (m_targetMainGear == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 메인장비입니다. herMainGearId = " + herMainGearId);
        }
        if (m_targetMainGear.inventorySlot == null)
        {
            throw new CommandHandleException(1, "인벤토리에 존재하지 않는 장비입니다.");
        }
        if (m_targetMainGear.gear.tier.requiredHeroLevel > m_myHero.level)
        {
            throw new CommandHandleException(1, "레벨이 맞지 않은 장비입니다.");
        }
        HeroMainGear unequippedMainGear = null;
        if (m_targetMainGear.isWeapon)
        {
            if (m_targetMainGear.gear.jobId != m_myHero.baseJobId)
            {
                throw new CommandHandleException(1, "직업이 맞지 않은 장비입니다.");
            }
            unequippedMainGear = m_myHero.equippedWeapon;
            m_myHero.equippedWeapon = m_targetMainGear;
        }
        else
        {
            unequippedMainGear = m_myHero.equippedArmor;
            m_myHero.equippedArmor = m_targetMainGear;
        }
        m_targetInventorySlot = m_targetMainGear.inventorySlot;
        m_targetInventorySlot.Clear();
        if (unequippedMainGear != null)
        {
            m_targetInventorySlot.Place(unequippedMainGear);
        }
        m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
        if (m_myHero.currentPlace != null)
        {
            Place currentPlace = m_myHero.currentPlace;
            ServerEvent.SendHeroMainGearEquip(currentPlace.GetDynamicClientPeers(m_myHero.sector, m_myHero.id), m_myHero.id, m_targetMainGear.ToPDHeroMainGear());
        }
        m_targetMainGear.owned = true;
        if (m_targetMainGear.enchantLevel > m_myHero.maxEquippedMainGearEnchantLevel)
        {
            m_myHero.maxEquippedMainGearEnchantLevel = m_targetMainGear.enchantLevel;
        }
        SaveToDB();
        MainGearEquipResponseBody resBody = new MainGearEquipResponseBody();
        resBody.mainGearOwned = m_targetMainGear.owned;
        resBody.maxEquippedMainGearEnchantLevel = m_myHero.maxEquippedMainGearEnchantLevel;
        resBody.changedInventorySlotIndex = m_targetInventorySlot.isEmpty ? -1 : m_targetInventorySlot.index;
        resBody.maxHp = m_myHero.realMaxHP;
        resBody.hp = m_myHero.hp;
        SendResponseOK(resBody);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_MainGear(m_myHero));
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMainGear_Owned(m_targetMainGear.id, m_targetMainGear.owned));
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_targetInventorySlot));
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_MaxEquippedGearEnchantLevel(m_myHero.id, m_myHero.maxEquippedMainGearEnchantLevel));
        dbWork.Schedule();
    }
}
