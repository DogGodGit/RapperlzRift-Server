using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class ConstellationEntryActivateCommandHandler : InGameCommandHandler<ConstellationEntryActivateCommandBody, ConstellationEntryActivateResponseBody>
{
	public const short kResult_ConstellationNotOpened = 101;

	public const short kResult_ConstellationStepNotOpened = 102;

	public const short kResult_NotEnoughStarEssense = 103;

	public const short kResult_NotEnoughGold = 104;

	private int m_nRequiredStarEssense;

	private long m_lnRequiredGold;

	private HeroConstellationStep m_heroConstellationStep;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nConstellationId = m_body.constellationId;
		int nStep = m_body.step;
		int nCycle = m_body.cycle;
		int nEntryNo = m_body.entryNo;
		if (nConstellationId <= 0)
		{
			throw new CommandHandleException(1, "별자리ID가 유효하지 않습니다. nConstellationId = " + nConstellationId);
		}
		if (nStep <= 0)
		{
			throw new CommandHandleException(1, "단계가 유효하지 않습니다. nStep = " + nStep);
		}
		if (nCycle <= 0)
		{
			throw new CommandHandleException(1, "사이클이 유효하지 않습니다. nCycle = " + nCycle);
		}
		if (nEntryNo <= 0)
		{
			throw new CommandHandleException(1, "항목번호가 유효하지 않습니다. nEntryNo = " + nEntryNo);
		}
		Constellation constellation = Resource.instance.GetConstellation(nConstellationId);
		if (constellation == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 별자리입니다. nConstellationId = " + nConstellationId);
		}
		ConstellationStep step = constellation.GetStep(nStep);
		if (step == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 단계입니다. nConstellationId = " + nConstellationId + ", nStep = " + nStep);
		}
		HeroConstellation heroConstellation = m_myHero.GetConstellation(nConstellationId);
		if (heroConstellation == null)
		{
			throw new CommandHandleException(101, "별자리가 개방되지 않았습니다. nConstellationId = " + nConstellationId);
		}
		m_heroConstellationStep = heroConstellation.GetStep(nStep);
		if (m_heroConstellationStep == null)
		{
			throw new CommandHandleException(102, "별자리단계가 개방되지 않습니다. nConstellationId = " + nConstellationId + ", nStep = " + nStep);
		}
		ConstellationEntry entry = m_heroConstellationStep.entry;
		if (entry.cycle.cycle != nCycle)
		{
			throw new CommandHandleException(1, "진행해야될 사이클이 아닙니다. nCycle = " + nCycle);
		}
		if (entry.no != nEntryNo)
		{
			throw new CommandHandleException(1, "진행해야될 항목번호가 아닙니다. nEntryNo = " + nEntryNo);
		}
		if (m_heroConstellationStep.activated)
		{
			throw new CommandHandleException(1, "이미 활성화된 별자리항목입니다.");
		}
		m_nRequiredStarEssense = entry.requiredStarEssense;
		m_lnRequiredGold = entry.requiredGold;
		if (m_myHero.starEssense < m_nRequiredStarEssense)
		{
			throw new CommandHandleException(103, "별의정수가 부족합니다.");
		}
		if (m_myHero.gold < m_lnRequiredGold)
		{
			throw new CommandHandleException(104, "골드가 부족합니다.");
		}
		m_myHero.UseStarEssense(m_nRequiredStarEssense);
		m_myHero.UseGold(m_lnRequiredGold);
		bool bSuccess = false;
		int nFailPoint = m_heroConstellationStep.failPoint;
		int nSuccessRate = entry.successRate + nFailPoint;
		bSuccess = nSuccessRate >= 10000 || Util.DrawLots(nSuccessRate);
		if (bSuccess)
		{
			m_heroConstellationStep.activated = true;
			ConstellationCycle constellationCycle = entry.cycle;
			ConstellationEntry constellationEntry = entry;
			if (!constellationCycle.isLastCycle || !constellationEntry.isLastEntry)
			{
				ConstellationEntry nextEntry = null;
				int nNextEntryId = constellationEntry.no + 1;
				if (nNextEntryId <= constellationCycle.lastEntry)
				{
					nextEntry = constellationCycle.GetEntry(nNextEntryId);
				}
				else
				{
					ConstellationCycle nextCycle = step.GetCycle(constellationCycle.cycle + 1);
					nextEntry = nextCycle.GetEntry(1);
				}
				m_heroConstellationStep.entry = nextEntry;
				m_heroConstellationStep.activated = false;
				m_heroConstellationStep.failPoint = 0;
			}
			base.hero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
		else
		{
			m_heroConstellationStep.failPoint += entry.failPoint;
		}
		SaveToDB();
		SaveToLogDB(entry.cycle.cycle, entry.no, nFailPoint);
		ConstellationEntryActivateResponseBody resBody = new ConstellationEntryActivateResponseBody();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.starEssense = m_myHero.starEssense;
		resBody.gold = m_myHero.gold;
		resBody.success = bSuccess;
		resBody.failPoint = m_heroConstellationStep.failPoint;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_StarEssense(m_myHero.id, m_myHero.starEssense));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroConstellationStep(m_heroConstellationStep.constellation.hero.id, m_heroConstellationStep.constellation.id, m_heroConstellationStep.step, m_heroConstellationStep.entry.cycle.cycle, m_heroConstellationStep.entry.no, m_heroConstellationStep.failPoint, m_heroConstellationStep.activated));
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nCycle, int nEntryId, int nFailPoint)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroConstellationActivationLog(Guid.NewGuid(), m_myHero.id, m_heroConstellationStep.constellation.id, m_heroConstellationStep.step, nCycle, nEntryId, m_nRequiredStarEssense, m_lnRequiredGold, m_heroConstellationStep.activated, nFailPoint, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
