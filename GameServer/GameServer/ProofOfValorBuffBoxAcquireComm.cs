using System;
using ClientCommon;

namespace GameServer;

public class ProofOfValorBuffBoxAcquireCommandHandler : InGameCommandHandler<ProofOfValorBuffBoxAcquireCommandBody, ProofOfValorBuffBoxAcquireResponseBody>
{
	public const short kResult_Dead = 101;

	public const short kResult_BuffBoxNotExist = 102;

	public const short kResult_UnableAcquirePosition_WithBuffBox = 103;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ProofOfValorInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		long lnBuffBoxInstanceId = m_body.buffBoxInstanceId;
		ProofOfValorBuffBoxInstance buffBoxInst = currentPlace.GetBuffBoxInstance(lnBuffBoxInstanceId);
		if (buffBoxInst == null)
		{
			throw new CommandHandleException(102, "버프상자가 존재하지 않습니다. lnBuffBoxInstanceId = " + lnBuffBoxInstanceId);
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은상태입니다.");
		}
		if (!buffBoxInst.IsBuffBoxAcquisitionRange(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(103, "버프상자를 획득할 수 없는 위치입니다. heroPosition = " + m_myHero.position);
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		m_myHero.AcquireProofOfValorBuffBox(buffBoxInst, currentTime);
		ProofOfValorBuffBoxAcquireResponseBody resBody = new ProofOfValorBuffBoxAcquireResponseBody();
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}
}
