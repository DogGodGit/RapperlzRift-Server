using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class GuildSupplySupportQuestAcceptCommandHandler : InGameCommandHandler<GuildSupplySupportQuestAcceptCommandBody, GuildSupplySupportQuestAcceptResponseBody>
{
	public const short kResult_NotGuildMember = 101;

	public const short kResult_NotAuthority = 102;

	public const short kResult_ProgressedQuest = 103;

	public const short kResult_AlreadyAccpeted = 104;

	public const short kResult_UnableInteractionPositionWithStartNPC = 105;

	public const short kResult_PlayingCartQuest = 106;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private GuildSupplySupportQuestPlay m_quest;

	private GuildSupplySupportQuestCartInstance m_cartInst;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재장소에서 사용할 수 없는 명령입니다.");
		}
		GuildMember guildMember = m_myHero.guildMember;
		if (guildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입하지 않았습니다.");
		}
		if (!guildMember.isMaster && !guildMember.isViceMaster)
		{
			throw new CommandHandleException(102, "길드물자지원퀘스트 수락 권한이 없습니다.");
		}
		if (m_myHero.isTransformMonster)
		{
			throw new CommandHandleException(1, "몬스터 변신중에는 사용할 수 없는 명령입니다.");
		}
		Guild guild = guildMember.guild;
		if (guild.guildSupplySupportQuestPlay != null)
		{
			throw new CommandHandleException(103, "이미 퀘스트가 진행중 입니다.");
		}
		if (m_myHero.isPlayingCartQuest)
		{
			throw new CommandHandleException(106, "영웅이 카트퀘스트를 진행중입니다.");
		}
		GuildSupplySupportQuest quest = Resource.instance.guildSupplySupportQuest;
		guild.RefreshDailyGuildSupplySupportQuestStartCount(m_currentTime.Date);
		DateValuePair<int> dailyGuildSupplySupportQuestStartCount = guild.dailyGuildSupplySupportQuestStartCount;
		if (dailyGuildSupplySupportQuestStartCount.value > 0)
		{
			throw new CommandHandleException(104, "금일 이미 퀘스트를 수락했습니다.");
		}
		Npc startNpc = quest.startNpc;
		if (!currentPlace.IsSame(startNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "시작 NPC에 있는 장소가 아닙니다.");
		}
		if (!startNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(105, "시작 NPC와 상호작용할 수 있는 위치가 아닙니다.");
		}
		m_quest = new GuildSupplySupportQuestPlay(guild);
		m_quest.Init(m_myHero.id, m_currentTime);
		guild.SetGuildSupplySupportQuest(m_quest, m_myHero);
		dailyGuildSupplySupportQuestStartCount.value++;
		m_cartInst = new GuildSupplySupportQuestCartInstance();
		lock (m_cartInst.syncObject)
		{
			m_cartInst.Init(m_quest, m_myHero, m_currentTime, bFirstCreation: true);
			m_cartInst.SetPositionAndRotation(m_myHero.position, m_myHero.rotationY);
			currentPlace.EnterCart(m_cartInst, m_currentTime, bSendEvent: false);
			m_cartInst.GetOn(m_currentTime);
			SaveToDB();
			ServerEvent.SendGuildSupplySupportQuestStarted(guild.GetClientPeers(m_myHero.id), dailyGuildSupplySupportQuestStartCount.date, dailyGuildSupplySupportQuestStartCount.value);
			GuildSupplySupportQuestAcceptResponseBody resBody = new GuildSupplySupportQuestAcceptResponseBody();
			resBody.cartInst = (PDGuildSupplySupportQuestCartInstance)m_cartInst.ToPDCartInstance(m_currentTime);
			resBody.remainingTime = m_quest.GetRemainingTime(m_currentTime);
			resBody.date = (DateTime)dailyGuildSupplySupportQuestStartCount.date;
			resBody.dailyGuildSupplySupportQuestStartCount = dailyGuildSupplySupportQuestStartCount.value;
			SendResponseOK(resBody);
		}
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_quest.guild.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddGuildSupplySupportQuest(m_quest.id, m_quest.guild.id, m_quest.heroId, m_quest.cart.id, m_quest.cartHp, m_quest.isCartRiding, m_quest.cartContinentId, m_quest.cartPosition.x, m_quest.cartPosition.y, m_quest.cartPosition.z, m_quest.cartYRotation, 0, m_currentTime));
		dbWork.Schedule();
	}
}
