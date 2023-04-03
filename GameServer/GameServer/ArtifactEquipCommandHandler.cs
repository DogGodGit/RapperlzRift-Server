using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ArtifactEquipCommandHandler : InGameCommandHandler<ArtifactEquipCommandBody, ArtifactEquipResponseBody>
{
	public const short kResult_ArtifactNotOpened = 101;

	public const short kResult_TargetArtifactNotOpened = 102;

	private int m_nArtifactNo;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_nArtifactNo = m_body.artifactNo;
		if (!Resource.instance.IsValidArtifactNo(m_nArtifactNo))
		{
			throw new CommandHandleException(1, "아티팩트번호가 유효하지 않습니다. m_nArtifactNo = " + m_nArtifactNo);
		}
		if (!m_myHero.isArtifactOpened)
		{
			throw new CommandHandleException(101, "아티팩트가 개방되지 않았습니다.");
		}
		if (m_nArtifactNo > m_myHero.artifactNo)
		{
			throw new CommandHandleException(102, "대상 아티팩트가 개방되지 않았습니다. m_nArtifactNo = " + m_nArtifactNo);
		}
		m_myHero.SetEquippedArtifact(m_nArtifactNo);
		SaveToDB();
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_EquippedArtifact(m_myHero.id, m_myHero.equippedArtifactNo));
		dbWork.Schedule();
	}
}
