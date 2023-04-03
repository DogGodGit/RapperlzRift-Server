using System;

namespace GameServer;

public class FieldOfHonorChallengeParam : PlaceEntranceParam
{
	private DateTimeOffset m_challengeTime = DateTimeOffset.MinValue;

	private Hero m_nTargetRanker;

	private int m_nTargetRanking;

	public DateTimeOffset challengeTime => m_challengeTime;

	public Hero targetRanker => m_nTargetRanker;

	public int targetRanking => m_nTargetRanking;

	public FieldOfHonorChallengeParam(DateTimeOffset challengeTime, Hero targetRanker, int nTargetRanking)
	{
		m_challengeTime = challengeTime;
		m_nTargetRanker = targetRanker;
		m_nTargetRanking = nTargetRanking;
	}
}
