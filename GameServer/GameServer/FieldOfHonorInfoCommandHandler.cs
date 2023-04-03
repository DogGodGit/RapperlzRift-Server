using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class FieldOfHonorInfoCommandHandler : InGameCommandHandler<FieldOfHonorInfoCommandBody, FieldOfHonorInfoResponseBody>
{
	public const short kResult_LevelUnderflowed = 101;

	public const short kResult_NotClearedMainQuest = 102;

	private FieldOfHonor m_fieldOfHonor;

	private List<int> m_matchingRankings;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_fieldOfHonor = Resource.instance.fieldOfHonor;
		if (m_fieldOfHonor.requiredConditionType == 1)
		{
			int nHeroLevel = m_myHero.level;
			int nRequiredHeroLevel = m_fieldOfHonor.requiredHeroLevel;
			if (nHeroLevel < nRequiredHeroLevel)
			{
				throw new CommandHandleException(101, "영웅의 레벨이 낮아 결투장을 이용할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroLevel = " + nRequiredHeroLevel);
			}
		}
		else if (!m_myHero.IsMainQuestCompleted(m_fieldOfHonor.requiredMainQuestNo))
		{
			throw new CommandHandleException(102, "결투장 이용에 필요한 메인퀘스트를 클리어하지 않았습니다.");
		}
		Biz.RegisterFieldOfHonorRanker(m_myHero);
		int nHistoryCount = m_myHero.fieldOfHonorHistories.Count;
		int nStartIndex = Math.Max(nHistoryCount - 3, 0);
		m_myHero.SortFieldOfHonorHistory();
		List<FieldOfHonorHistory> histories = new List<FieldOfHonorHistory>();
		for (int j = nStartIndex; j < nHistoryCount; j++)
		{
			histories.Add(m_myHero.GetFieldOfHonorHistory(j));
		}
		m_matchingRankings = m_myHero.fieldOfHonorTargets;
		int nOldTargetCount = m_matchingRankings.Count;
		int nHeroRanking = m_myHero.fieldOfHonorRanking;
		if (nOldTargetCount < 5)
		{
			for (int i = nOldTargetCount; i < 5; i++)
			{
				int nTargetRanking = m_fieldOfHonor.SelectTarget((nHeroRanking > 0) ? nHeroRanking : 10000, i + 1);
				m_matchingRankings.Add(nTargetRanking);
			}
		}
		SaveToDB(nOldTargetCount);
		List<PDFieldOfHonorHero> matchingRankers = new List<PDFieldOfHonorHero>();
		foreach (int ranking in m_matchingRankings)
		{
			matchingRankers.Add(Cache.instance.GetPDFieldOfHonorHero(ranking));
		}
		DateTime currentDate = DateTimeUtil.currentTime.Date;
		FieldOfHonorInfoResponseBody resBody = new FieldOfHonorInfoResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.myRanking = m_myHero.fieldOfHonorRanking;
		resBody.successiveCount = m_myHero.fieldOfHonorSuccessiveCount;
		m_myHero.RefreshDailyFieldOfHonorPlayCount(currentDate);
		resBody.playCount = m_myHero.dailyFieldOfHonorPlayCount.value;
		resBody.histories = FieldOfHonorHistory.ToPDFieldOfHonorHistories(histories).ToArray();
		resBody.matchedRankings = matchingRankers.ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nStartIndex)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		for (int i = nStartIndex; i < m_matchingRankings.Count; i++)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddFieldOfHonorTarget(m_myHero.id, m_matchingRankings[i]));
		}
		dbWork.Schedule();
	}
}
