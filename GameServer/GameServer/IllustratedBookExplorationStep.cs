using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class IllustratedBookExplorationStepReward
{
	private IllustratedBookExplorationStep m_step;

	private int m_nNo;

	private ItemReward m_itemReward;

	public IllustratedBookExplorationStep step => m_step;

	public int no => m_nNo;

	public ItemReward itemReward => m_itemReward;

	public IllustratedBookExplorationStepReward(IllustratedBookExplorationStep step)
	{
		m_step = step;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. stepNo = " + step.no + ", m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
		else if (lnItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. stepNo = " + step.no + ", m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
public class IllustratedBookExplorationStepRewardReceiveCommandHandler : InGameCommandHandler<IllustratedBookExplorationStepRewardReceiveCommandBody, IllustratedBookExplorationStepRewardReceiveResponseBody>
{
	public const short kResult_AlreadyReceived = 101;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue.Date;

	private IllustratedBookExplorationStep m_currentStep;

	private long m_lnRewardGold;

	private HashSet<PDItemBooty> m_itemBooties = new HashSet<PDItemBooty>();

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		m_currentStep = m_myHero.illustratedBookExplorationStep;
		if (m_currentStep == null)
		{
			throw new CommandHandleException(1, "현재 도감탐험단계가 없습니다.");
		}
		if (m_myHero.illustratedBookExplorationStepRewardReceivedDate == m_currentDate && m_myHero.illustratedBookExplorationStepRewardReceivedStepNo == m_currentStep.no)
		{
			throw new CommandHandleException(101, "이미 보상을 받았습니다.");
		}
		m_lnRewardGold = m_currentStep.goldRewardValue;
		m_myHero.AddGold(m_lnRewardGold);
		foreach (IllustratedBookExplorationStepReward reward in m_currentStep.rewards)
		{
			ItemReward itemReward = reward.itemReward;
			if (itemReward == null)
			{
				continue;
			}
			Item item = itemReward.item;
			int nCount = itemReward.count;
			bool bOwned = itemReward.owned;
			int nRemainingCount = m_myHero.AddItem(item, bOwned, nCount, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_NAME_00018", "MAIL_DESC_00018", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(item, nRemainingCount, bOwned));
			}
			PDItemBooty itemBooty = new PDItemBooty();
			itemBooty.id = item.id;
			itemBooty.count = nCount;
			itemBooty.owned = bOwned;
			m_itemBooties.Add(itemBooty);
		}
		m_myHero.illustratedBookExplorationStepRewardReceivedDate = m_currentDate;
		m_myHero.illustratedBookExplorationStepRewardReceivedStepNo = m_currentStep.no;
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB();
		SaveToDB_Log();
		IllustratedBookExplorationStepRewardReceiveResponseBody resBody = new IllustratedBookExplorationStepRewardReceiveResponseBody();
		resBody.date = (DateTime)m_currentDate;
		resBody.gold = m_myHero.gold;
		resBody.maxGold = m_myHero.maxGold;
		resBody.booties = m_itemBooties.ToArray();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_IllustratedBookStepReward(m_myHero.id, m_myHero.illustratedBookExplorationStepRewardReceivedDate, m_myHero.illustratedBookExplorationStepRewardReceivedStepNo));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroIllustratedBookExplorationStepRewardLog(logId, m_myHero.id, m_currentStep.no, m_lnRewardGold, m_currentTime));
			foreach (PDItemBooty itemBooty in m_itemBooties)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroIllustratedBookExplorationStepRewardDetailLog(Guid.NewGuid(), logId, itemBooty.id, itemBooty.count, itemBooty.owned));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
public class IllustratedBookExplorationStep
{
	private int m_nNo;

	private int m_nActivationExplorationPoint;

	private GoldReward m_goldReward;

	private List<IllustratedBookExplorationStepAttr> m_attrs = new List<IllustratedBookExplorationStepAttr>();

	private List<IllustratedBookExplorationStepReward> m_rewards = new List<IllustratedBookExplorationStepReward>();

	public int no => m_nNo;

	public int activationExplorationPoint => m_nActivationExplorationPoint;

	public GoldReward goldReward => m_goldReward;

	public long goldRewardValue
	{
		get
		{
			if (m_goldReward == null)
			{
				return 0L;
			}
			return m_goldReward.value;
		}
	}

	public List<IllustratedBookExplorationStepAttr> attrs => m_attrs;

	public List<IllustratedBookExplorationStepReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["stepNo"]);
		m_nActivationExplorationPoint = Convert.ToInt32(dr["activationExplorationPoint"]);
		long lnGoldRewardId = Convert.ToInt64(dr["goldRewardId"]);
		if (lnGoldRewardId > 0)
		{
			m_goldReward = Resource.instance.GetGoldReward(lnGoldRewardId);
			if (m_goldReward == null)
			{
				SFLogUtil.Warn(GetType(), "골드보상이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnGoldRewardId = " + lnGoldRewardId);
			}
		}
		else if (lnGoldRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "골드보상ID가 유효하지 않습니다. m_nNo = " + m_nNo + ", lnGoldRewardId = " + lnGoldRewardId);
		}
	}

	public void AddAttr(IllustratedBookExplorationStepAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_attrs.Add(attr);
	}

	public void AddReward(IllustratedBookExplorationStepReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}
}
public class IllustratedBookExplorationStepAttr
{
	private IllustratedBookExplorationStep m_step;

	private int m_nId;

	private AttrValue m_attrValue;

	public IllustratedBookExplorationStep step => m_step;

	public int id => m_nId;

	public AttrValue attrValue => m_attrValue;

	public IllustratedBookExplorationStepAttr(IllustratedBookExplorationStep step)
	{
		m_step = step;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["attrId"]);
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		if (lnAttrValueId > 0)
		{
			m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
			if (m_attrValue == null)
			{
				SFLogUtil.Warn(GetType(), "속성값이 존재하지 않습니다. stepNo = " + step.no + ", m_nId = " + m_nId + ", lnAttrValueId = " + lnAttrValueId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "속성값ID가 유효하지 않습니다. stepNo = " + step.no + ", m_nId = " + m_nId + ", lnAttrValueId = " + lnAttrValueId);
		}
	}
}
public class IllustratedBookExplorationStepAcquireCommandHandler : InGameCommandHandler<IllustratedBookExplorationStepAcquireCommandBody, IllustratedBookExplorationStepAcquireResponseBody>
{
	public const short kResult_NotEnoughExplorationPoint = 101;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nTargetStepNo = m_body.targetStepNo;
		if (nTargetStepNo <= 0)
		{
			throw new CommandHandleException(1, "대상 단계번호가 유효하지 않습니다. nTargetStepNo = " + nTargetStepNo);
		}
		IllustratedBookExplorationStep targetStep = Resource.instance.GetIllustratedBookExplorationStep(nTargetStepNo);
		if (targetStep == null)
		{
			throw new CommandHandleException(1, "대상 도감탐험단계가 존재하지 않습니다. nTargetStepNo = " + nTargetStepNo);
		}
		if (nTargetStepNo != m_myHero.illustratedBookExplorationStepNo + 1)
		{
			throw new CommandHandleException(1, "대상 도감탐험단계을 획득할 수 없습니다.");
		}
		if (m_myHero.explorationPoint < targetStep.activationExplorationPoint)
		{
			throw new CommandHandleException(101, "탐험점수가 부족합니다.");
		}
		m_myHero.SetIllustratedBookExplorationStep(targetStep);
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		m_currentTime = DateTimeUtil.currentTime;
		SaveToDB();
		SaveToDB_Log();
		IllustratedBookExplorationStepAcquireResponseBody resBody = new IllustratedBookExplorationStepAcquireResponseBody();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_IllustratedBookExplorationStep(m_myHero.id, m_myHero.illustratedBookExplorationStepNo));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroIllustratedBookExplorationStepActivationLog(Guid.NewGuid(), m_myHero.id, m_myHero.illustratedBookExplorationStepNo, m_myHero.explorationPoint, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
