using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class RankAcquireCommandHandler : InGameCommandHandler<RankAcquireCommandBody, RankAcquireResponseBody>
{
    public const short kResult_NotEnoughExploitPoint = 101;

    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nTargetRankNo = m_body.targetRankNo;
        if (nTargetRankNo <= 0)
        {
            throw new CommandHandleException(1, "대상계급번호가 유효하지 않습니다. nTargetRankNo = " + nTargetRankNo);
        }
        Rank targetRank = Resource.instance.GetRank(nTargetRankNo);
        if (targetRank == null)
        {
            throw new CommandHandleException(1, "대상계급이 존재하지 않습니다. nTargetRankNo = " + nTargetRankNo);
        }
        if (nTargetRankNo != m_myHero.rankNo + 1)
        {
            throw new CommandHandleException(1, "대상 계급을 획득할 수 없습니다.");
        }
        if (m_myHero.exploitPoint < targetRank.requiredExploitPoint)
        {
            throw new CommandHandleException(101, "공적포인트가 부족합니다.");
        }
        m_myHero.SetRank(targetRank);
        m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
        SaveToDB();
        RankAcquireResponseBody resBody = new RankAcquireResponseBody();
        resBody.maxHP = m_myHero.realMaxHP;
        SendResponseOK(resBody);
        if (m_myHero.currentPlace != null)
        {
            ServerEvent.SendHeroRankAcquired(m_myHero.currentPlace.GetDynamicClientPeers(m_myHero.sector, m_myHero.id), m_myHero.id, nTargetRankNo);
        }
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Rank(m_myHero));
        dbWork.Schedule();
    }
}
