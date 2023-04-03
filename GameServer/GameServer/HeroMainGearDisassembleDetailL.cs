namespace GameServer;

public class HeroMainGearDisassembleDetailLog
{
	private int m_nNo;

	private int m_nItemId;

	private int m_nItemCount;

	private bool m_bItemOwned;

	public int no => m_nNo;

	public int itemId => m_nItemId;

	public int itemCount => m_nItemCount;

	public bool itemOwned => m_bItemOwned;

	public HeroMainGearDisassembleDetailLog(int nNo, int nItemId, int nItemCount, bool bItemOwned)
	{
		m_nNo = nNo;
		m_nItemId = nItemId;
		m_nItemCount = nItemCount;
		m_bItemOwned = bItemOwned;
	}
}
