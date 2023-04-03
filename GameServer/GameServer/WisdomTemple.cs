using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WisdomTemple : Location
{
	public const int kColorMatchingMonsterYRotationType_Fixed = 1;

	public const int kColorMatchingMonsterYRotationType_Random = 2;

	public const float kPuzzleRewardObjectInteractionMaxRangeFactor = 1.1f;

	public const int kRequiredConditionType_Level = 1;

	public const int kRequiredConditionType_MainQuest = 2;

	private int m_nLocationId;

	private int m_nRequiredConditionType;

	private int m_nRequiredHeroLevel;

	private int m_nRequiredMainQuestNo;

	private int m_nRequiredStamina;

	private int m_nStartDelayTime;

	private int m_nLimitTime;

	private int m_nExitDelayTime;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartYRotation;

	private int m_nColorMatchingPoint;

	private int m_nColorMatchingObjectivePoint;

	private int m_nColorMatchingMonsterSpawnInterval;

	private MonsterArrange m_colorMatchingMonsterArrange;

	private Vector3 m_colorMatchingMonsterPosition = Vector3.zero;

	private int m_nColorMatchingMonsterYRotationType;

	private float m_fColorMatchingMonsterYRotation;

	private int m_nColorMatchingMonsterKillPoint;

	private int m_nColorMatchingMonsterKillObjectId;

	private MonsterArrange m_findTreasureBoxMonsterArrange;

	private float m_fPuzzleRewardObjectInteractionDuration;

	private float m_fPuzzleRewardObjectInteractionMaxRange;

	private int m_nBossMonsterSpawnDelay;

	private MonsterArrange m_bossMonsterArrange;

	private Vector3 m_bossMonsterPosition = Vector3.zero;

	private float m_fBossMonsterYRotation;

	private ItemReward m_bossMonsterKillItemReward;

	private ItemReward m_sweepItemReward;

	private Rect3D m_mapRect = Rect3D.zero;

	private Dictionary<int, WisdomTempleMonsterAttrFactor> m_monsterAttrFactors = new Dictionary<int, WisdomTempleMonsterAttrFactor>();

	private Dictionary<int, WisdomTempleColorMatchingObject> m_colorMatchingObjects = new Dictionary<int, WisdomTempleColorMatchingObject>();

	private List<WisdomTempleColorMatchingObject> m_randomCreateColorMatchingObjects = new List<WisdomTempleColorMatchingObject>();

	private int m_nRandomCreateColorMatchingObjectTotalPoint;

	private List<WisdomTempleArrangePosition> m_arrangePositions = new List<WisdomTempleArrangePosition>();

	private int m_nArrangePositionMaxRow;

	private int m_nArrangePositionMaxCol;

	private Dictionary<int, WisdomTempleSweepReward> m_sweepRewards = new Dictionary<int, WisdomTempleSweepReward>();

	private Dictionary<int, WisdomTemplePuzzleReward> m_puzzleRewards = new Dictionary<int, WisdomTemplePuzzleReward>();

	private List<WisdomTempleStep> m_steps = new List<WisdomTempleStep>();

	private Dictionary<int, WisdomTemplePuzzle> m_puzzles = new Dictionary<int, WisdomTemplePuzzle>();

	private int m_nPuzzleTotalPoint;

	private Dictionary<int, WisdomTempleStepReward> m_stepRewards = new Dictionary<int, WisdomTempleStepReward>();

	private List<WisdomTempleFindTreasureBoxCount> m_findTreasureBoxCounts = new List<WisdomTempleFindTreasureBoxCount>();

	private Dictionary<int, List<WisdomTemplePuzzleRewardObjectOffset>> m_puzzleRewardObjectOffsetsCollection = new Dictionary<int, List<WisdomTemplePuzzleRewardObjectOffset>>();

	private List<WisdomTemplePuzzleRewardPoolEntry> m_puzzleRewardPoolEntries = new List<WisdomTemplePuzzleRewardPoolEntry>();

	private int m_nPuzzleRewardTotalPoint;

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.WisdomTemple;

	public override bool mountRidingEnabled => false;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => false;

	public override bool evasionCastEnabled => true;

	public int requiredConditionType => m_nRequiredConditionType;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int requiredMainQuestNo => m_nRequiredMainQuestNo;

	public int requiredStamina => m_nRequiredStamina;

	public int startDelayTime => m_nStartDelayTime;

	public int limitTime => m_nLimitTime;

	public int exitDelayTime => m_nExitDelayTime;

	public Vector3 startPosition => m_startPosition;

	public float startYRotation => m_fStartYRotation;

	public int colorMatchingPoint => m_nColorMatchingPoint;

	public int colorMatchingObjectivePoint => m_nColorMatchingObjectivePoint;

	public int colorMatchingMonsterSpawnInterval => m_nColorMatchingMonsterSpawnInterval;

	public MonsterArrange colorMatchingMonsterArrange => m_colorMatchingMonsterArrange;

	public Vector3 colorMatchingMonsterPosition => m_colorMatchingMonsterPosition;

	public int colorMatchingMonsterYRotationType => m_nColorMatchingMonsterYRotationType;

	public float colorMatchingMonsterYRotation => m_fColorMatchingMonsterYRotation;

	public int colorMatchingMonsterKillPoint => m_nColorMatchingMonsterKillPoint;

	public int colorMatchingMonsterKillObjectId => m_nColorMatchingMonsterKillObjectId;

	public MonsterArrange findTreasureBoxMonsterArrange => m_findTreasureBoxMonsterArrange;

	public float puzzleRewardObjectInerationDuration => m_fPuzzleRewardObjectInteractionDuration;

	public float puzzleRewardObjectInteractionMaxRange => m_fPuzzleRewardObjectInteractionMaxRange;

	public int bossMonsterSpawnDelay => m_nBossMonsterSpawnDelay;

	public MonsterArrange bossMonsterArrange => m_bossMonsterArrange;

	public Vector3 bossMonsterPosition => m_bossMonsterPosition;

	public float bossMonsterYRotation => m_fBossMonsterYRotation;

	public ItemReward bossMonsterKillItemReward => m_bossMonsterKillItemReward;

	public ItemReward sweepItemReward => m_sweepItemReward;

	public Rect3D mapRect => m_mapRect;

	public int stepCount => m_steps.Count;

	public List<WisdomTempleArrangePosition> arrangePositions => m_arrangePositions;

	public int arrangePositionMaxRow => m_nArrangePositionMaxCol;

	public int arrangePositionMaxCol => m_nArrangePositionMaxCol;

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		Resource res = Resource.instance;
		m_nLocationId = Convert.ToInt32(dr["locationId"]);
		m_nRequiredConditionType = Convert.ToInt32(dr["requiredConditionType"]);
		if (!IsDefinedRequiredConditionType(m_nRequiredConditionType))
		{
			SFLogUtil.Warn(GetType(), "필요조건타입이 유효하지 않습니다. m_nRequiredConditionType = " + m_nRequiredConditionType);
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "필요영웅레벨이 유효하지 않습니다. m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		m_nRequiredMainQuestNo = Convert.ToInt32(dr["requiredMainQuestNo"]);
		if (m_nRequiredMainQuestNo < 0)
		{
			SFLogUtil.Warn(GetType(), "필요메인퀘스트번호가 유효하지 않습니다. m_nRequiredMainQuestNo = " + m_nRequiredMainQuestNo);
		}
		m_nRequiredStamina = Convert.ToInt32(dr["requiredStamina"]);
		m_nStartDelayTime = Convert.ToInt32(dr["startDelayTime"]);
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		m_nExitDelayTime = Convert.ToInt32(dr["exitDelayTime"]);
		m_startPosition.x = Convert.ToSingle(dr["startXPosition"]);
		m_startPosition.y = Convert.ToSingle(dr["startYPosition"]);
		m_startPosition.z = Convert.ToSingle(dr["startZPosition"]);
		m_fStartYRotation = Convert.ToSingle(dr["startYRotation"]);
		m_nColorMatchingPoint = Convert.ToInt32(dr["colorMatchingPoint"]);
		if (m_nColorMatchingPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "색맞추기점수가 유효하지 않습니다. m_nColorMatchingPoint = " + m_nColorMatchingPoint);
		}
		m_nColorMatchingObjectivePoint = Convert.ToInt32(dr["colorMatchingObjectivePoint"]);
		if (m_nColorMatchingObjectivePoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "색맞추기목표점수가 유효하지 않습니다. m_nColorMatchingObjectivePoint = " + m_nColorMatchingObjectivePoint);
		}
		m_nColorMatchingMonsterSpawnInterval = Convert.ToInt32(dr["colorMatchingMonsterSpawnInterval"]);
		if (m_nColorMatchingMonsterSpawnInterval < 0)
		{
			SFLogUtil.Warn(GetType(), "색맞추기몬스터출몰간격이 유효하지 않습니다. m_nColorMatchingMonsterSpawnInterval = " + m_nColorMatchingMonsterSpawnInterval);
		}
		long lnColorMatchingMonsterArrangeId = Convert.ToInt64(dr["colorMatchingMonsterArrangeId"]);
		if (lnColorMatchingMonsterArrangeId > 0)
		{
			m_colorMatchingMonsterArrange = res.GetMonsterArrange(lnColorMatchingMonsterArrangeId);
			if (m_colorMatchingMonsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "색맞추기몬스터배치가 존재하지 않습니다. lnColorMatchingMonsterArrangeId = " + lnColorMatchingMonsterArrangeId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "색맞추기몬스터배치ID가 유효하지 않습니다. lnColorMatchingMonsterArrangeId = " + lnColorMatchingMonsterArrangeId);
		}
		m_colorMatchingMonsterPosition.x = Convert.ToSingle(dr["colorMatchingMonsterXPosition"]);
		m_colorMatchingMonsterPosition.y = Convert.ToSingle(dr["colorMatchingMonsterYPosition"]);
		m_colorMatchingMonsterPosition.z = Convert.ToSingle(dr["colorMatchingMonsterZPosition"]);
		m_nColorMatchingMonsterYRotationType = Convert.ToInt32(dr["colorMatchingMonsterYRotationType"]);
		if (!IsDefinedColorMatchingMonsterYRotationType(m_nColorMatchingMonsterYRotationType))
		{
			SFLogUtil.Warn(GetType(), "색맞추기몬스터방향타입이 유효하지 않습니다. m_nColorMatchingMonsterYRotationType = " + m_nColorMatchingMonsterYRotationType);
		}
		m_fColorMatchingMonsterYRotation = Convert.ToSingle(dr["colorMatchingMonsterYRotation"]);
		m_nColorMatchingMonsterKillPoint = Convert.ToInt32(dr["colorMatchingMonsterKillPoint"]);
		m_nColorMatchingMonsterKillObjectId = Convert.ToInt32(dr["colorMatchingMonsterKillObjectId"]);
		long lnFindTreasureBoxMonsterArrangeId = Convert.ToInt64(dr["findTreasureBoxMonsterArrangeId"]);
		if (lnFindTreasureBoxMonsterArrangeId > 0)
		{
			m_findTreasureBoxMonsterArrange = res.GetMonsterArrange(lnFindTreasureBoxMonsterArrangeId);
			if (m_findTreasureBoxMonsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "보물상자찾기몬스터배치가 존재하지 않습니다. lnFindTreasureBoxMonsterArrangeId = " + lnFindTreasureBoxMonsterArrangeId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "보물상자찾기몬스터배치ID가 유효하지 않습니다. lnFindTreasureBoxMonsterArrangeId = " + lnFindTreasureBoxMonsterArrangeId);
		}
		m_fPuzzleRewardObjectInteractionDuration = Convert.ToSingle(dr["puzzleRewardObjectInteractionDuration"]);
		if (m_fPuzzleRewardObjectInteractionDuration < 0f)
		{
			SFLogUtil.Warn(GetType(), "퍼즐보상오브젝트상호작용시간이 유효하지 않습니다. m_fPuzzleRewardObjectInteractionDuration = " + m_fPuzzleRewardObjectInteractionDuration);
		}
		m_fPuzzleRewardObjectInteractionMaxRange = Convert.ToSingle(dr["puzzleRewardObjectInteractionMaxRange"]);
		if (m_fPuzzleRewardObjectInteractionMaxRange <= 0f)
		{
			SFLogUtil.Warn(GetType(), "퍼즐보상오브젝트상호작용최대범위가 유효하지 않습니다. m_fPuzzleRewardObjectInteractionMaxRange = " + m_fPuzzleRewardObjectInteractionMaxRange);
		}
		m_nBossMonsterSpawnDelay = Convert.ToInt32(dr["bossMonsterSpawnDelayTime"]);
		if (m_nBossMonsterSpawnDelay < 0)
		{
			SFLogUtil.Warn(GetType(), "보스몬스터출몰대기시간이 유효하지 않습니다. m_nBossMonsterSpawnDelay = " + m_nBossMonsterSpawnDelay);
		}
		long lnBossMonsterArrangeId = Convert.ToInt64(dr["bossMonsterArrangeId"]);
		if (lnBossMonsterArrangeId > 0)
		{
			m_bossMonsterArrange = res.GetMonsterArrange(lnBossMonsterArrangeId);
			if (m_bossMonsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "보스몬스터배치가 존재하지 않습니다. lnBossMonsterArrangeId = " + lnBossMonsterArrangeId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "보스몬스터배치ID가 유효하지 않습니다. lnBossMonsterArrangeId = " + lnBossMonsterArrangeId);
		}
		m_bossMonsterPosition.x = Convert.ToSingle(dr["bossMonsterXPosition"]);
		m_bossMonsterPosition.y = Convert.ToSingle(dr["bossMonsterYPosition"]);
		m_bossMonsterPosition.z = Convert.ToSingle(dr["bossMonsterZPosition"]);
		m_fBossMonsterYRotation = Convert.ToSingle(dr["bossMonsterYRotation"]);
		long lnBossMonsterKillItemRewardId = Convert.ToInt64(dr["bossMonsterKillItemRewardId"]);
		if (lnBossMonsterKillItemRewardId > 0)
		{
			m_bossMonsterKillItemReward = res.GetItemReward(lnBossMonsterKillItemRewardId);
			if (m_bossMonsterKillItemReward == null)
			{
				SFLogUtil.Warn(GetType(), "보스몬스터처치아이템보상이 존재하지 않습니다. lnBossMonsterKillItemRewardId = " + lnBossMonsterKillItemRewardId);
			}
		}
		else if (lnBossMonsterKillItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "보스몬스터처치아이템보상ID가 유효하지 않습니다. lnBossMonsterKillItemRewardId = " + lnBossMonsterKillItemRewardId);
		}
		long lnSweepItemRewardId = Convert.ToInt64(dr["sweepItemRewardId"]);
		if (lnSweepItemRewardId > 0)
		{
			m_sweepItemReward = res.GetItemReward(lnSweepItemRewardId);
			if (m_sweepItemReward == null)
			{
				SFLogUtil.Warn(GetType(), "소탕아이템보상이 존재하지 않습니다. lnSweepItemRewardId = " + lnSweepItemRewardId);
			}
		}
		else if (lnSweepItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "소탕아이템보상ID가 유효하지 않습니다. lnSweepItemRewardId = " + lnSweepItemRewardId);
		}
		m_mapRect.x = Convert.ToSingle(dr["x"]);
		m_mapRect.y = Convert.ToSingle(dr["y"]);
		m_mapRect.z = Convert.ToSingle(dr["z"]);
		m_mapRect.sizeX = Convert.ToSingle(dr["xSize"]);
		m_mapRect.sizeY = Convert.ToSingle(dr["ySize"]);
		m_mapRect.sizeZ = Convert.ToSingle(dr["zSize"]);
	}

	public float SelectColorMatchingMonsterRotationY()
	{
		if (m_nColorMatchingMonsterYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fColorMatchingMonsterYRotation);
		}
		return m_fColorMatchingMonsterYRotation;
	}

	public void AddMonsterAttrFactor(WisdomTempleMonsterAttrFactor monsterAttrFactor)
	{
		if (monsterAttrFactor == null)
		{
			throw new ArgumentNullException("monsterAttrFactor");
		}
		m_monsterAttrFactors.Add(monsterAttrFactor.level, monsterAttrFactor);
	}

	public WisdomTempleMonsterAttrFactor GetMonsterAttrFactor(int nLevel)
	{
		if (!m_monsterAttrFactors.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddColorMatchingObject(WisdomTempleColorMatchingObject colorMatchingObject)
	{
		if (colorMatchingObject == null)
		{
			throw new ArgumentNullException("colorMatchingObject");
		}
		m_colorMatchingObjects.Add(colorMatchingObject.id, colorMatchingObject);
		if (colorMatchingObject.id != m_nColorMatchingMonsterKillObjectId)
		{
			m_randomCreateColorMatchingObjects.Add(colorMatchingObject);
			m_nRandomCreateColorMatchingObjectTotalPoint += colorMatchingObject.point;
		}
	}

	public WisdomTempleColorMatchingObject SelectRandomCreateColorMatchingObject_ToExclude(ICollection<int> objectIdToExcludes)
	{
		List<WisdomTempleColorMatchingObject> entries = new List<WisdomTempleColorMatchingObject>();
		int nTotalPoint = 0;
		foreach (WisdomTempleColorMatchingObject entry in m_randomCreateColorMatchingObjects)
		{
			bool bInculde = true;
			foreach (int nExcludeId in objectIdToExcludes)
			{
				if (nExcludeId == entry.id)
				{
					bInculde = false;
					break;
				}
			}
			if (bInculde)
			{
				entries.Add(entry);
				nTotalPoint += entry.point;
			}
		}
		return Util.SelectPickEntry(entries, nTotalPoint);
	}

	public WisdomTempleColorMatchingObject SelectRandomCreateColorMatchingObject()
	{
		return Util.SelectPickEntry(m_randomCreateColorMatchingObjects, m_nRandomCreateColorMatchingObjectTotalPoint);
	}

	public void AddArrangePosition(WisdomTempleArrangePosition arrangePosition)
	{
		if (arrangePosition == null)
		{
			throw new ArgumentNullException("arrangePosition");
		}
		m_arrangePositions.Add(arrangePosition);
		if (m_nArrangePositionMaxRow < arrangePosition.row)
		{
			m_nArrangePositionMaxRow = arrangePosition.row;
		}
		if (m_nArrangePositionMaxCol < arrangePosition.col)
		{
			m_nArrangePositionMaxCol = arrangePosition.col;
		}
	}

	public WisdomTempleArrangePosition GetArrangePosition(int nRow, int nCol)
	{
		foreach (WisdomTempleArrangePosition arrangePosition in m_arrangePositions)
		{
			if (arrangePosition.row == nRow && arrangePosition.col == nCol)
			{
				return arrangePosition;
			}
		}
		return null;
	}

	public void AddSweepReward(WisdomTempleSweepReward sweepReward)
	{
		if (sweepReward == null)
		{
			throw new ArgumentNullException("sweepReward");
		}
		m_sweepRewards.Add(sweepReward.level, sweepReward);
	}

	public WisdomTempleSweepReward GetSweepReward(int nLevel)
	{
		if (!m_sweepRewards.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddPuzzleReward(WisdomTemplePuzzleReward puzzleReward)
	{
		if (puzzleReward == null)
		{
			throw new ArgumentNullException("puzzleReward");
		}
		m_puzzleRewards.Add(puzzleReward.level, puzzleReward);
	}

	public WisdomTemplePuzzleReward GetPuzzleReward(int nLevel)
	{
		if (!m_puzzleRewards.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddStep(WisdomTempleStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_steps.Add(step);
	}

	public WisdomTempleStep GetStep(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_steps.Count)
		{
			return null;
		}
		return m_steps[nIndex];
	}

	public void AddPuzzle(WisdomTemplePuzzle puzzle)
	{
		if (puzzle == null)
		{
			throw new ArgumentNullException("puzzle");
		}
		m_puzzles.Add(puzzle.id, puzzle);
		m_nPuzzleTotalPoint += puzzle.point;
	}

	public WisdomTemplePuzzle SelectPuzzle()
	{
		return Util.SelectPickEntry(m_puzzles.Values, m_nPuzzleTotalPoint);
	}

	public void AddStepReward(WisdomTempleStepReward stepReward)
	{
		if (stepReward == null)
		{
			throw new ArgumentNullException("stepReward");
		}
		m_stepRewards.Add(stepReward.level, stepReward);
	}

	public WisdomTempleStepReward GetStepReward(int nLevel)
	{
		if (!m_stepRewards.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddFindTreasureBoxCount(WisdomTempleFindTreasureBoxCount findTreasureBoxCount)
	{
		if (findTreasureBoxCount == null)
		{
			throw new ArgumentNullException("findTreasureBoxCount");
		}
		m_findTreasureBoxCounts.Add(findTreasureBoxCount);
	}

	public WisdomTempleFindTreasureBoxCount GetFindTreasureBoxCount(int nCount)
	{
		int nIndex = nCount - 1;
		if (nIndex < 0 || nIndex >= m_findTreasureBoxCounts.Count)
		{
			return null;
		}
		return m_findTreasureBoxCounts[nIndex];
	}

	public void AddPuzzleRewardObjectOffset(WisdomTemplePuzzleRewardObjectOffset puzzleRewardObjectOffset)
	{
		if (puzzleRewardObjectOffset == null)
		{
			throw new ArgumentNullException("puzzleRewardObjectOffset");
		}
		GetOrCreatePuzzleRewardObjectOffsets(puzzleRewardObjectOffset.rewardCount).Add(puzzleRewardObjectOffset);
	}

	public List<WisdomTemplePuzzleRewardObjectOffset> GetPuzzleRewardObjectOffsets(int nRewardCount)
	{
		if (!m_puzzleRewardObjectOffsetsCollection.TryGetValue(nRewardCount, out var value))
		{
			return null;
		}
		return value;
	}

	private List<WisdomTemplePuzzleRewardObjectOffset> GetOrCreatePuzzleRewardObjectOffsets(int nRewardCount)
	{
		List<WisdomTemplePuzzleRewardObjectOffset> puzzleRewardObjectOffsets = GetPuzzleRewardObjectOffsets(nRewardCount);
		if (puzzleRewardObjectOffsets == null)
		{
			puzzleRewardObjectOffsets = new List<WisdomTemplePuzzleRewardObjectOffset>();
			m_puzzleRewardObjectOffsetsCollection.Add(nRewardCount, puzzleRewardObjectOffsets);
		}
		return puzzleRewardObjectOffsets;
	}

	public void AddPuzzleRewardPoolEntry(WisdomTemplePuzzleRewardPoolEntry puzzleRewardPoolEntry)
	{
		if (puzzleRewardPoolEntry == null)
		{
			throw new ArgumentNullException("puzzleRewardPoolEntry");
		}
		m_puzzleRewardPoolEntries.Add(puzzleRewardPoolEntry);
		m_nPuzzleRewardTotalPoint += puzzleRewardPoolEntry.point;
	}

	public ItemReward SelectPuzzleReward()
	{
		return SelectPuzzleRewardPoolEntry().itemReward;
	}

	private WisdomTemplePuzzleRewardPoolEntry SelectPuzzleRewardPoolEntry()
	{
		return Util.SelectPickEntry(m_puzzleRewardPoolEntries, m_nPuzzleRewardTotalPoint);
	}

	public static bool IsDefinedColorMatchingMonsterYRotationType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}

	public static bool IsDefinedRequiredConditionType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
