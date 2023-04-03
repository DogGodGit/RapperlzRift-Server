using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CartGetOnCommandHandler : InGameCommandHandler<CartGetOnCommandBody, CartGetOnResponseBody>
{
	public const short kResult_Dead = 101;

	public const short kResult_BattleMode = 102;

	public const short kResult_AlreadyRidingMount = 103;

	public const short kResult_AlreadyRidingCart = 104;

	public const short kResult_CartNotExist = 105;

	public const short kResult_NotCartOwner = 106;

	public const short kResult_NotRidableRange = 107;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "Body가 null입니다.");
		}
		long lnInstanceId = m_body.instanceId;
		if (lnInstanceId <= 0)
		{
			throw new CommandHandleException(1, "카트 인스턴스ID가 유효하지 않습니다. lnInstanceId = " + lnInstanceId);
		}
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은 상태입니다.");
		}
		if (m_myHero.isBattleMode)
		{
			throw new CommandHandleException(102, "영웅이 전투상태입니다.");
		}
		if (m_myHero.isRiding)
		{
			throw new CommandHandleException(103, "영웅이 탈것을 타고있는 상태입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(104, "영웅이 카트에 탑승중입니다.");
		}
		if (m_myHero.isTransformMonster)
		{
			throw new CommandHandleException(1, "몬스터 변신중에는 사용할 수 없는 명령입니다.");
		}
		CartInstance cartInst = currentPlace.GetCartInstance(lnInstanceId);
		if (cartInst == null)
		{
			throw new CommandHandleException(105, "카트가 존재하지 않습니다. lnInstanceId = " + lnInstanceId);
		}
		lock (cartInst.syncObject)
		{
			if (cartInst.owner.id != m_myHero.id)
			{
				throw new CommandHandleException(106, "카트의 소유주가 아닙니다. lnInstanceId = " + lnInstanceId);
			}
			if (!cartInst.IsRidablePosition(m_myHero.position, m_myHero.radius))
			{
				throw new CommandHandleException(107, "탑승할 수 있는 거리가 아닙니다. lnInstanceId = " + lnInstanceId);
			}
			cartInst.GetOn(currentTime);
			SaveToDB(cartInst);
			CartGetOnResponseBody resBody = new CartGetOnResponseBody();
			resBody.cartInst = cartInst.ToPDCartInstance(currentTime);
			SendResponseOK(resBody);
		}
	}

	private void SaveToDB(CartInstance cartInst)
	{
		switch (cartInst.cartInstanceType)
		{
		case CartInstanceType.MainQuest:
			SaveToDB_MainQuest((MainQuestCartInstance)cartInst);
			break;
		case CartInstanceType.SupplySupportQuest:
			SaveToDB_SupplySupportQuest((SupplySupportQuestCartInstance)cartInst);
			break;
		case CartInstanceType.GuildSupplySupportQuest:
			SaveToDB_GuildSupplySupportQuest((GuildSupplySupportQuestCartInstance)cartInst);
			break;
		}
	}

	private void SaveToDB_MainQuest(MainQuestCartInstance cartInst)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHeroMainQuest_Cart(cartInst.quest));
		dbWork.Schedule();
	}

	private void SaveToDB_SupplySupportQuest(SupplySupportQuestCartInstance cartInst)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateSupplySupportQuest_Cart(cartInst.quest));
		dbWork.Schedule();
	}

	private void SaveToDB_GuildSupplySupportQuest(GuildSupplySupportQuestCartInstance cartInst)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateGuildSupplySupportQuest_Cart(cartInst.quest));
		dbWork.Schedule();
	}
}
