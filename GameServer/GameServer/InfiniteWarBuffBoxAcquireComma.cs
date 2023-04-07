using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class InfiniteWarBuffBoxAcquireCommandHandler : InGameCommandHandler<InfiniteWarBuffBoxAcquireCommandBody, InfiniteWarBuffBoxAcquireResponseBody>
{
	public const short kResult_Dead = 101;

	public const short kResult_BuffBoxNotExist = 102;

	public const short kResult_UnableAcquirePosition_WithBuffBox = 103;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is InfiniteWarInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		long lnBuffBoxInstanceId = m_body.instanceId;
		InfiniteWarBuffBoxInstance buffBoxInst = currentPlace.GetBuffBox(lnBuffBoxInstanceId);
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
		m_myHero.AcquireInfiniteWarBuffBox(buffBoxInst, currentTime);
		InfiniteWarBuffBoxAcquireResponseBody resBody = new InfiniteWarBuffBoxAcquireResponseBody();
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}
}
