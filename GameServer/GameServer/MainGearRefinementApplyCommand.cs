using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class MainGearRefinementApplyCommandHandler : InGameCommandHandler<MainGearRefinementApplyCommandBody, MainGearRefinementApplyResponseBody>
{
	public class HeroMainGearRefinementApplicationLog
	{
		public int index;

		public Guid heroId = Guid.Empty;

		public Guid heroMainGearId = Guid.Empty;

		public int oldGrade;

		public int grade;

		public int oldAttrId;

		public int attrId;

		public long oldAttrValueId;

		public long attrValueId;

		public HeroMainGearRefinementApplicationLog(int nIndex, Guid heroId, Guid heroMainGearId, int nOldGrade, int nGrade, int nOldAttrId, int nAttrId, long lnOldAttrValueId, long lnAttrValueId)
		{
			index = nIndex;
			this.heroId = heroId;
			this.heroMainGearId = heroMainGearId;
			oldGrade = nOldGrade;
			grade = nGrade;
			oldAttrId = nOldAttrId;
			attrId = nAttrId;
			oldAttrValueId = lnOldAttrValueId;
			attrValueId = lnAttrValueId;
		}
	}

	private List<HeroMainGearRefinementApplicationLog> m_refinementApplyLog = new List<HeroMainGearRefinementApplicationLog>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid heroMainGearId = (Guid)m_body.heroMainGearId;
		int nTurn = m_body.turn;
		if (nTurn <= 0)
		{
			throw new CommandHandleException(1, "횟수가 유효하지 않습니다. nTurn = " + nTurn);
		}
		HeroMainGear heroMainGear = m_myHero.GetMainGear(heroMainGearId);
		if (heroMainGear == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 영웅메인장비입니다. heroMainGearId = " + heroMainGearId);
		}
		HeroMainGearRefinement refinement = heroMainGear.GetRefinement(nTurn);
		if (refinement == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 세련횟수입니다. nTurn = " + nTurn);
		}
		foreach (HeroMainGearOptionAttr heroMainGearOptionAttr in heroMainGear.optionAttrs)
		{
			HeroMainGearRefinementAttr refinementOptionAttr = refinement.GetAttr(heroMainGearOptionAttr.index);
			m_refinementApplyLog.Add(new HeroMainGearRefinementApplicationLog(heroMainGearOptionAttr.index, m_myHero.id, heroMainGear.id, heroMainGearOptionAttr.attrGrade, refinementOptionAttr.grade, heroMainGearOptionAttr.attrId, refinementOptionAttr.attrId, heroMainGearOptionAttr.attrValue.id, refinementOptionAttr.attrValue.id));
			heroMainGearOptionAttr.SetAttrValue(refinementOptionAttr.grade, refinementOptionAttr.attrId, refinementOptionAttr.attrValue);
		}
		heroMainGear.RefreshAttrTotalValues();
		if (heroMainGear.isEquipped)
		{
			m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
		heroMainGear.ClearRefinement();
		SaveToDB(heroMainGear);
		MainGearRefinementApplyResponseBody resBody = new MainGearRefinementApplyResponseBody();
		resBody.optionAttrs = HeroMainGearOptionAttr.ToPDHeroMainGearOptionAttrs(heroMainGear.optionAttrs).ToArray();
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	private void SaveToDB(HeroMainGear heroMainGear)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (HeroMainGearOptionAttr attr in heroMainGear.optionAttrs)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMainGearOptionAttr(attr.heroMainGear.id, attr.index, attr.attrGrade, attr.attrId, attr.attrValue.id));
		}
		dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroMainGearRefinementAttrs(heroMainGear.id));
		dbWork.Schedule();
		SaveToDB_AddHeroMainGearRefinementApplicationLog(heroMainGear);
	}

	private void SaveToDB_AddHeroMainGearRefinementApplicationLog(HeroMainGear heroMainGear)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			foreach (HeroMainGearRefinementApplicationLog log in m_refinementApplyLog)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroMainGearRefinementApplicationLog(Guid.NewGuid(), log.index, log.heroId, log.heroMainGearId, log.oldGrade, log.grade, log.oldAttrId, log.attrId, log.oldAttrValueId, log.attrValueId, m_currentTime));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
