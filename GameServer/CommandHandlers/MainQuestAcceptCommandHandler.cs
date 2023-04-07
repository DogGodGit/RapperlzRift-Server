using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class MainQuestAcceptCommandHandler : InGameCommandHandler<MainQuestAcceptCommandBody, MainQuestAcceptResponseBody>
{
    public const short kResult_CurrentlyMainQuestProgressing = 101;

    public const short kResult_NotEnoughHeroLevel = 102;

    public const short kResult_UnableInteractionPositionWithStartNPC = 103;

    public const short kResult_PlayingCartQuest = 104;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    private MainQuestCartInstance m_cartInst;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nMainQuestNo = m_body.mainQuestNo;
        if (nMainQuestNo <= 0)
        {
            throw new CommandHandleException(1, "메인퀘스트 번호가 유효하지 않습니다. nMainQuestNo = " + nMainQuestNo);
        }
        if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
        {
            throw new CommandHandleException(1, "현재장소에서는 사용할 수 없는 명령입니다.");
        }
        HeroMainQuest currentHeroMainQuest = m_myHero.currentHeroMainQuest;
        MainQuest nextMainQuest = null;
        if (currentHeroMainQuest != null)
        {
            if (!currentHeroMainQuest.completed)
            {
                throw new CommandHandleException(101, "현재 메인퀘스트를 진행중입니다.");
            }
            if (currentHeroMainQuest.mainQuest.isLast)
            {
                throw new CommandHandleException(1, "현재 퀘스트가 마지막 퀘스트입니다.");
            }
            nextMainQuest = currentHeroMainQuest.mainQuest.nextMainQuest;
        }
        else
        {
            nextMainQuest = Resource.instance.startMainQuest;
        }
        if (nextMainQuest.no != nMainQuestNo)
        {
            throw new CommandHandleException(1, "진행해야될 메인퀘스트가 아닙니다. nMainQuestId = " + nMainQuestNo);
        }
        if (nextMainQuest.requiredHeroLevel > m_myHero.level)
        {
            throw new CommandHandleException(102, "영웅레벨이 부족합니다.");
        }
        Npc startNpc = nextMainQuest.startNpc;
        if (startNpc != null)
        {
            if (!currentPlace.IsSame(startNpc.continent.id, m_myHero.nationId))
            {
                throw new CommandHandleException(1, "시작 NPC가 있는 장소가 아닙니다.");
            }
            if (!startNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
            {
                throw new CommandHandleException(103, "시작 NPC와 상호작용할 수 있는 위치가 아닙니다.");
            }
        }
        if (nextMainQuest.type == 6 && m_myHero.isPlayingCartQuest)
        {
            throw new CommandHandleException(104, "영웅이 카트퀘스트를 진행중입니다.");
        }
        HeroMainQuest heroMainQuest = new HeroMainQuest(m_myHero, nextMainQuest);
        m_myHero.currentHeroMainQuest = heroMainQuest;
        if (nextMainQuest.type == 6)
        {
            m_cartInst = new MainQuestCartInstance();
            lock (m_cartInst.syncObject)
            {
                m_cartInst.Init(heroMainQuest, m_currentTime);
                m_cartInst.SetPositionAndRotation(m_myHero.position, m_myHero.rotationY);
                currentPlace.EnterCart(m_cartInst, m_currentTime, bSendEvent: false);
                m_cartInst.GetOn(m_currentTime);
                Finish();
                return;
            }
        }
        Finish();
    }

    private void Finish()
    {
        List<long> removedAbnormalStateEffects = new List<long>();
        m_myHero.TransformMainQuestMonster(m_currentTime, removedAbnormalStateEffects);
        SaveToDB();
        MainQuestAcceptResponseBody resBody = new MainQuestAcceptResponseBody();
        if (m_cartInst != null)
        {
            resBody.cartInst = (PDMainQuestCartInstance)m_cartInst.ToPDCartInstance(m_currentTime);
        }
        resBody.transformationMonsterId = m_myHero.isTransformMainQuestMonster ? m_myHero.mainQuestTransformationMonsterEffect.transformationMonster.id : 0;
        resBody.maxHP = m_myHero.realMaxHP;
        resBody.hp = m_myHero.hp;
        resBody.removedAbnormalStateEffects = removedAbnormalStateEffects.ToArray();
        SendResponseOK(resBody);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        HeroMainQuest quest = m_myHero.currentHeroMainQuest;
        dbWork.AddSqlCommand(GameDac.CSC_AddHeroMainQuest(m_myHero.id, quest.mainQuest.no, quest.isCartRiding, quest.cartContinentId, quest.cartPosition.x, quest.cartPosition.y, quest.cartPosition.z, quest.cartRotationY, m_currentTime));
        dbWork.Schedule();
    }
}
