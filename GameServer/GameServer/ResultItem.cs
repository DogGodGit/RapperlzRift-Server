namespace GameServer;

public class ResultItem
{
	private Item m_item;

	private bool m_bOwned;

	private int m_nCount;

	public Item item => m_item;

	public bool owned => m_bOwned;

	public int count
	{
		get
		{
			return m_nCount;
		}
		set
		{
			m_nCount = value;
		}
	}

	public ResultItem(Item item, bool bOwned)
	{
		m_item = item;
		m_bOwned = bOwned;
	}
}
