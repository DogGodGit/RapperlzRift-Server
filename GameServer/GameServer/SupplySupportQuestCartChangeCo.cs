using ClientCommon;
using ServerFramework;

namespace GameServer;

public class SupplySupportQuestCartChangeCommandHandler : InGameCommandHandler<SupplySupportQuestCartChangeCommandBody, SupplySupportQuestCartChangeResponseBody>
{
	public const short kResult_NotStartSupplySupportQuest = 101;

	public const short kResult_VisitedWayPoint = 102;

	public const short kResult_UnableCartChangePositionWithCartChangeNPC_Hero = 103;

	public const short kResult_UnableCartChangePositionWithCartChangeNPC_Cart = 104;

	private HeroSupplySupportQuest m_heroQuest;

	protected override void HandleInGameCommand()
	{
		int nWayPoint = m_body.wayPoint;
		SupplySupportQuest quest = Resource.instance.supplySupportQuest;
		SupplySupportQuestWayPoint wayPoint = quest.GetWayPoint(nWayPoint);
		if (wayPoint == null)
		{
			throw new CommandHandleException(1, "중간지점이 유효하지 않습니다. nWayPoint = " + nWayPoint);
		}
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재장소에서는 사용할 수 없는 명령입니다.");
		}
		m_heroQuest = m_myHero.supplySupportQuest;
		if (m_heroQuest == null)
		{
			throw new CommandHandleException(101, "보급지원퀘스트를 진행하는 중이 아닙니다.");
		}
		if (m_heroQuest.ContainsVisitedWayPoint(nWayPoint))
		{
			throw new CommandHandleException(102, "이미 방문한 중간지점 입니다. nWayPoint = " + nWayPoint);
		}
		Npc cartChangeNpc = wayPoint.cartChangeNpc;
		if (!currentPlace.IsSame(cartChangeNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "영웅이 카트변경NPC와 다른장소에 있습니다.");
		}
		if (!cartChangeNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(103, "영웅이 카트변경NPC와 상호작용할 수 없는 위치에 있습니다.");
		}
		SupplySupportQuestCartInstance cartInst = m_heroQuest.cartInst;
		lock (cartInst.syncObject)
		{
			ContinentInstance cartPlace = (ContinentInstance)cartInst.currentPlace;
			if (!cartPlace.IsSame(cartChangeNpc.continent.id, m_myHero.nationId))
			{
				throw new CommandHandleException(1, "카트가 카트변경NPC와 다른장소에 있습니다.");
			}
			if (!cartChangeNpc.IsInteractionEnabledPosition(cartInst.position, m_myHero.radius))
			{
				throw new CommandHandleException(104, "카트가 카트변경NPC 주위에 있지 않습니다.");
			}
			m_heroQuest.AddVisitedWayPoint(nWayPoint);
			if (cartInst.cart.grade.id != 5)
			{
				cartInst.ChangeCart();
			}
			SaveToDB(nWayPoint);
			SupplySupportQuestCartChangeResponseBody resBody = new SupplySupportQuestCartChangeResponseBody();
			resBody.cartId = m_heroQuest.cartInst.cart.id;
			SendResponseOK(resBody);
		}
	}

	private void SaveToDB(int nWayPoint)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateSupplySupportQuest_Cart(m_heroQuest));
		dbWork.AddSqlCommand(GameDac.CSC_AddSupplySupportQuestVisitedWayPoint(m_heroQuest.id, nWayPoint));
		dbWork.Schedule();
	}
}
