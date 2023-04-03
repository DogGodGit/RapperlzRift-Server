using ServerFramework;

namespace GameServer;

public abstract class WisdomTempleObjectInstance
{
	public const int kType_ColorMatchingObject = 1;

	public const int kType_PuzzleRewardObject = 2;

	protected long m_lnInstanceId;

	protected WisdomTempleInstance m_currentPlace;

	protected Hero m_interactionHero;

	protected bool m_bIsReleased;

	public static readonly SFSynchronizedLongFactory instanceIdFactory = new SFSynchronizedLongFactory();

	public abstract int type { get; }

	public long instanceId => m_lnInstanceId;

	public WisdomTempleInstance currentPlace => m_currentPlace;

	public abstract Vector3 position { get; }

	public abstract float interactionDuration { get; }

	public Hero interactionHero
	{
		get
		{
			return m_interactionHero;
		}
		set
		{
			m_interactionHero = value;
		}
	}

	public bool isReleased => m_bIsReleased;

	public WisdomTempleObjectInstance()
	{
		m_lnInstanceId = instanceIdFactory.NewValue();
	}

	protected void InitObject(WisdomTempleInstance currentPlace)
	{
		m_currentPlace = currentPlace;
	}

	public abstract bool IsInteractionEnabledPosition(Vector3 position, float fRadius);

	public void Release()
	{
		m_bIsReleased = true;
	}
}
