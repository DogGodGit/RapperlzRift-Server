using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class MailAttachment
{
	private Mail m_mail;

	private int m_nNo;

	private Item m_item;

	private int m_nItemCount;

	private bool m_bItemOwned;

	public Mail mail
	{
		get
		{
			return m_mail;
		}
		set
		{
			m_mail = value;
		}
	}

	public int no
	{
		get
		{
			return m_nNo;
		}
		set
		{
			m_nNo = value;
		}
	}

	public Item item => m_item;

	public int itemCount => m_nItemCount;

	public bool itemOwned => m_bItemOwned;

	public MailAttachment()
		: this(null, 0, bOwned: false)
	{
	}

	public MailAttachment(Item item, int nCount, bool bOwned)
	{
		m_item = item;
		m_nItemCount = nCount;
		m_bItemOwned = bOwned;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["attachmentNo"]);
		int nItemId = Convert.ToInt32(dr["itemId"]);
		m_item = Resource.instance.GetItem(nItemId);
		if (m_item == null)
		{
			throw new Exception("아이템이 존재하지 않습니다. m_nNo = " + m_nNo + ", nItemId = " + nItemId);
		}
		m_nItemCount = Convert.ToInt32(dr["itemCount"]);
		m_bItemOwned = Convert.ToBoolean(dr["itemOwned"]);
	}

	public PDMailAttachment ToPDMailAttachment()
	{
		PDMailAttachment inst = new PDMailAttachment();
		inst.no = m_nNo;
		inst.itemId = m_item.id;
		inst.itemCount = m_nItemCount;
		inst.itemOwned = m_bItemOwned;
		return inst;
	}

	public static List<PDMailAttachment> ToPDMailAttachments(IEnumerable<MailAttachment> attachments)
	{
		List<PDMailAttachment> insts = new List<PDMailAttachment>();
		foreach (MailAttachment attachment in attachments)
		{
			insts.Add(attachment.ToPDMailAttachment());
		}
		return insts;
	}
}
