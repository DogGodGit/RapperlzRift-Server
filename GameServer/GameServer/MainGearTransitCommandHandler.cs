using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class MainGearTransitCommandHandler : InGameCommandHandler<MainGearTransitCommandBody, MainGearTransitResponseBody>
{
	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid targetHeroMainGearId = (Guid)m_body.targetHeroMainGearId;
		Guid materialHeroMainGearId = (Guid)m_body.materialHeroMainGearId;
		bool bIsEnchantLevelTransit = m_body.isEnchantLevelTransit;
		bool bIsOptionAttrTransit = m_body.isOptionAttrTransit;
		HeroMainGear targetHeroMainGear = m_myHero.GetMainGear(targetHeroMainGearId);
		if (targetHeroMainGear == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 대상 영웅메인장비입니다. targetHeroMainGearId = " + targetHeroMainGearId);
		}
		HeroMainGear materialHeroMainGear = m_myHero.GetMainGear(materialHeroMainGearId);
		if (materialHeroMainGear == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 재료 영웅메인장비입니다. materialHeroMainGearId = " + materialHeroMainGearId);
		}
		if (targetHeroMainGear == materialHeroMainGear)
		{
			throw new CommandHandleException(1, "같은 장비는 전이를 할 수 없습니다.");
		}
		if (targetHeroMainGear.gear.category.id != materialHeroMainGear.gear.category.id)
		{
			throw new CommandHandleException(1, "영웅메인장비의 타입이 서로 다릅니다.");
		}
		if (!bIsEnchantLevelTransit && !bIsOptionAttrTransit)
		{
			throw new CommandHandleException(1, "항목을 적어도 하나는 선택해야합니다.");
		}
		if (bIsEnchantLevelTransit && targetHeroMainGear.enchantLevel <= 0 && materialHeroMainGear.enchantLevel <= 0)
		{
			throw new CommandHandleException(1, "영웅메인장비의 강화레벨이 둘 다 0입니다.");
		}
		if (bIsOptionAttrTransit && targetHeroMainGear.optionAttrCount != materialHeroMainGear.optionAttrCount)
		{
			throw new CommandHandleException(1, "영웅메인장비의 옵션속성 갯수가 서로 다릅니다.");
		}
		if (bIsEnchantLevelTransit)
		{
			int nTempEnchantLevel = targetHeroMainGear.enchantLevel;
			targetHeroMainGear.enchantLevel = materialHeroMainGear.enchantLevel;
			materialHeroMainGear.enchantLevel = nTempEnchantLevel;
		}
		if (bIsOptionAttrTransit)
		{
			foreach (HeroMainGearOptionAttr targetOptionAttr in targetHeroMainGear.optionAttrs)
			{
				int nTargetIndex = targetOptionAttr.index;
				int nTargetGrade = targetOptionAttr.attrGrade;
				int nTargetAttrId = targetOptionAttr.attrId;
				AttrValue nTargetAttrValue = targetOptionAttr.attrValue;
				HeroMainGearOptionAttr materialOptionAttr = materialHeroMainGear.GetOptionAttr(nTargetIndex);
				targetOptionAttr.SetAttrValue(materialOptionAttr.attrGrade, materialOptionAttr.attrId, materialOptionAttr.attrValue);
				materialOptionAttr.SetAttrValue(nTargetGrade, nTargetAttrId, nTargetAttrValue);
			}
		}
		if (targetHeroMainGear.owned || materialHeroMainGear.owned)
		{
			targetHeroMainGear.owned = true;
			materialHeroMainGear.owned = true;
		}
		targetHeroMainGear.RefreshAttrTotalValues();
		materialHeroMainGear.RefreshAttrTotalValues();
		if (targetHeroMainGear.isEquipped || materialHeroMainGear.isEquipped)
		{
			m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
		SaveToDB(targetHeroMainGear, materialHeroMainGear);
		MainGearTransitResponseBody resBody = new MainGearTransitResponseBody();
		resBody.targetOwned = targetHeroMainGear.owned;
		resBody.targetEnchantLevel = targetHeroMainGear.enchantLevel;
		resBody.targetOptionAttrs = HeroMainGearOptionAttr.ToPDHeroMainGearOptionAttrs(targetHeroMainGear.optionAttrs).ToArray();
		resBody.materialOwned = materialHeroMainGear.owned;
		resBody.materialEnchantLevel = materialHeroMainGear.enchantLevel;
		resBody.materialOptionAttrs = HeroMainGearOptionAttr.ToPDHeroMainGearOptionAttrs(materialHeroMainGear.optionAttrs).ToArray();
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	private void SaveToDB(HeroMainGear targetHeroMainGear, HeroMainGear materialHeroMainGear)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMainGear_Enchant(targetHeroMainGear.id, targetHeroMainGear.enchantLevel, targetHeroMainGear.owned));
		foreach (HeroMainGearOptionAttr attr2 in targetHeroMainGear.optionAttrs)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMainGearOptionAttr(attr2.heroMainGear.id, attr2.index, attr2.attrGrade, attr2.attrId, attr2.attrValue.id));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMainGear_Enchant(materialHeroMainGear.id, materialHeroMainGear.enchantLevel, materialHeroMainGear.owned));
		foreach (HeroMainGearOptionAttr attr in materialHeroMainGear.optionAttrs)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMainGearOptionAttr(attr.heroMainGear.id, attr.index, attr.attrGrade, attr.attrId, attr.attrValue.id));
		}
		dbWork.Schedule();
	}
}
