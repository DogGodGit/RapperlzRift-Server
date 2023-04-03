using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class SubGearSoulstoneLevelSetActivateCommandHandler : InGameCommandHandler<SubGearSoulstoneLevelSetActivateCommandBody, SubGearSoulstoneLevelSetActivateResponseBody>
{
	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nSetNo = m_body.setNo;
		if (nSetNo <= 0)
		{
			throw new CommandHandleException(1, "세트번호가 유효하지 않습니다. nSetNo = " + nSetNo);
		}
		if (nSetNo != m_myHero.subGearSoulstoneLevelSetNo + 1)
		{
			throw new CommandHandleException(1, "해당 보조장비소울스톤레벨세트를 활성화할 수 없습니다. nSetNo = " + nSetNo);
		}
		SubGearSoulstoneLevelSet targetSubGearSoulstoneLevelSet = Resource.instance.GetSubGearSoulstoneLevelSet(nSetNo);
		if (targetSubGearSoulstoneLevelSet == null)
		{
			throw new CommandHandleException(1, "해당 보조장비소울스톤레벨세트가 존재하지 않습니다. nSetNo = " + nSetNo);
		}
		int nTotalSubGearSoulstoneLevel = m_myHero.totalMountedSoulstoneLevel;
		if (nTotalSubGearSoulstoneLevel < targetSubGearSoulstoneLevelSet.requiredTotalLevel)
		{
			throw new CommandHandleException(1, "장착한 보조장비 소울스톤레벨 총합이 부족합니다. requiredTotalLevel = " + targetSubGearSoulstoneLevelSet.requiredTotalLevel);
		}
		m_myHero.subGearSoulstoneLevelSet = targetSubGearSoulstoneLevelSet;
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB(nTotalSubGearSoulstoneLevel);
		SubGearSoulstoneLevelSetActivateResponseBody resBody = new SubGearSoulstoneLevelSetActivateResponseBody();
		resBody.maxHp = m_myHero.realMaxHP;
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nSubGearSoulstoneTotalLevel)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SubGearSoulstoneLevelSetNo(m_myHero.id, m_myHero.subGearSoulstoneLevelSetNo));
		dbWork.Schedule();
		SaveToDB_AddSubGearSoulstoneLevelSetActivationLog(nSubGearSoulstoneTotalLevel);
	}

	private void SaveToDB_AddSubGearSoulstoneLevelSetActivationLog(int nSubGearSoulstoneTotalLevel)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroSubGearSoulstoneLevelSetActivationLog(Guid.NewGuid(), m_myHero.id, m_myHero.subGearSoulstoneLevelSet.setNo, nSubGearSoulstoneTotalLevel, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
