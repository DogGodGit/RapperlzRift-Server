using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ConstellationStepOpenCommandHandler : InGameCommandHandler<ConstellationStepOpenCommandBody, ConstellationStepOpenResponseBody>
{
	public const short kResult_NotEnoughDia = 101;

	private HeroConstellationStep m_heroConstellationStep;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

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
		if (nConstellationId <= 0)
		{
			throw new CommandHandleException(1, "별자리ID가 유효하지 않습니다. nConstellationId = " + nConstellationId);
		}
		if (nStep <= 0)
		{
			throw new CommandHandleException(1, "단계가 유효하지 않습니다. nStep = " + nStep);
		}
		Constellation constellation = Resource.instance.GetConstellation(nConstellationId);
		if (constellation == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 별자리입니다. nConstellationId = " + nConstellationId);
		}
		ConstellationStep constellationStep = constellation.GetStep(nStep);
		if (constellationStep == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 단계입니다. nConstellationId = " + nConstellationId + ", nStep = " + nStep);
		}
		int nRequiredDia = constellationStep.requiredDia;
		if (m_myHero.dia < nRequiredDia)
		{
			throw new CommandHandleException(101, "다이아가 부족합니다.");
		}
		HeroConstellation heroConstellation = m_myHero.GetConstellation(nConstellationId);
		if (heroConstellation == null)
		{
			throw new CommandHandleException(1, "영웅별자리가 존재하지 않습니다.");
		}
		m_heroConstellationStep = heroConstellation.GetStep(nStep);
		if (m_heroConstellationStep != null)
		{
			throw new CommandHandleException(1, "이미 개방한 단계입니다. nStep = " + nStep);
		}
		HeroConstellationStep prevheroConstellationStep = heroConstellation.GetStep(nStep - 1);
		if (prevheroConstellationStep == null)
		{
			throw new CommandHandleException(1, "이전 단계가 존재하지 않습니다. nStep = " + nStep);
		}
		if (prevheroConstellationStep.entry.cycle.cycle <= 1)
		{
			throw new CommandHandleException(1, "이전 단계의 1사이클을 완료해야합니다.");
		}
		m_myHero.UseDia(nRequiredDia, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
		m_heroConstellationStep = heroConstellation.CreateStep(constellationStep);
		SaveToDB();
		SaveToLogDB();
		ConstellationStepOpenResponseBody resBody = new ConstellationStepOpenResponseBody();
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroConstellationStep(m_heroConstellationStep.constellation.hero.id, m_heroConstellationStep.constellation.id, m_heroConstellationStep.step, m_heroConstellationStep.entry.cycle.cycle, m_heroConstellationStep.entry.no));
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroConstellationStepOpenLog(Guid.NewGuid(), m_myHero.id, m_heroConstellationStep.constellation.id, m_heroConstellationStep.step, m_nUsedOwnDia, m_nUsedUnOwnDia, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
