using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class SupplySupportQuestCartInstance : CartInstance
{
	private HeroSupplySupportQuest m_quest;

	public override bool abnormalStateHitEnabled => true;

	public override CartInstanceType cartInstanceType => CartInstanceType.SupplySupportQuest;

	public HeroSupplySupportQuest quest => m_quest;

	public void Init(HeroSupplySupportQuest quest, DateTimeOffset currentTime, bool bFirstCreation)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		InitCartInstance(quest.cart.cart, quest.hero, currentTime);
		m_quest = quest;
		m_quest.cartInst = this;
		if (!bFirstCreation)
		{
			m_nHP = quest.cartHp;
		}
		Cache.instance.AddCartInstance(this);
	}

	protected override void OnGetOn()
	{
		base.OnGetOn();
		m_quest.RefreshCartInfo();
	}

	protected override void OnGetOff()
	{
		base.OnGetOff();
		m_quest.RefreshCartInfo();
	}

	public void ChangeCart()
	{
		m_cart = m_quest.cart.SelectChangeCart().cart;
		m_quest.RefreshCartInfo();
		if (m_currentPlace != null)
		{
			ServerEvent.SendCartChanged(m_currentPlace.GetDynamicClientPeers(m_sector, m_owner.id), m_lnInstanceId, m_cart.id);
		}
	}

	protected override void OnDead()
	{
		base.OnDead();
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_owner.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateSupplySupportQuest_Cart(m_quest));
		dbWork.Schedule();
	}

	protected override void OnDeadAsync(DateTimeOffset time)
	{
		base.OnDeadAsync(time);
		ItemReward itemReward = m_quest.cart.destructionItemReward;
		if (itemReward != null)
		{
			Item item = itemReward.item;
			int nCount = itemReward.count;
			bool bOwned = itemReward.owned;
			PDItemBooty booty = new PDItemBooty();
			booty.id = item.id;
			booty.count = nCount;
			booty.owned = bOwned;
			foreach (CartReceivedDamage damage in m_receivedDamages.Values)
			{
				Hero hero = m_currentPlace.GetHero(damage.attackerId);
				if (hero == null)
				{
					continue;
				}
				lock (hero.syncObject)
				{
					if (hero.isDead || !IsInterestSector(hero.sector))
					{
						continue;
					}
					List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
					Mail mail = null;
					int nRewardItemRemainCount = hero.AddItem(item, bOwned, nCount, changedInventorySlots);
					if (nRewardItemRemainCount > 0)
					{
						mail = Mail.Create("MAIL_REWARD_N_3", "MAIL_REWARD_D_3", time);
						mail.AddAttachmentWithNo(new MailAttachment(item, nRewardItemRemainCount, bOwned));
						hero.AddMail(mail, bSendEvent: true);
					}
					SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(hero.id);
					foreach (InventorySlot slot in changedInventorySlots)
					{
						dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
					}
					if (mail != null)
					{
						dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(mail));
					}
					dbWork.Schedule();
					try
					{
						SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
						logWork.AddSqlCommand(GameLogDac.CSC_AddSupplySupportQuestCartDestructionRwardLog(Guid.NewGuid(), hero.id, m_quest.id, m_cart.id, item.id, nCount, bOwned, time));
						logWork.Schedule();
					}
					catch (Exception ex)
					{
						SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
					}
					if (changedInventorySlots.Count > 0)
					{
						ServerEvent.SendSupplySupportQuestCartDestructionReward(hero.account.peer, booty, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
					}
				}
			}
		}
		lock (m_owner.syncObject)
		{
			m_quest.Fail(time, bSendFailEvent: true);
		}
	}

	protected override PDCartInstance CreatePDCartInstance()
	{
		return new PDSupplySupportQuestCartInstance();
	}
}
