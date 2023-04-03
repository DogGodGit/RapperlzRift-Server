using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class Continent : Location
{
	private int m_nLocationId;

	private int m_nId;

	private string m_sNameKey;

	private string m_sDescriptionKey;

	private bool m_bIsNationTerritory;

	private int m_nRequiredHeroLevel;

	private bool m_bIsNationWarTarget;

	private Rect3D m_mapRect = Rect3D.zero;

	private Dictionary<int, ContinentMonsterArrange> m_monsterArranges = new Dictionary<int, ContinentMonsterArrange>();

	private Dictionary<int, Portal> m_portals = new Dictionary<int, Portal>();

	private Dictionary<int, Npc> m_npcs = new Dictionary<int, Npc>();

	private Dictionary<int, ContinentObjectArrange> m_objectArranges = new Dictionary<int, ContinentObjectArrange>();

	private EliteMonsterCategory m_eliteMonsterCategory;

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.Continent;

	public override bool mountRidingEnabled => true;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => true;

	public override bool evasionCastEnabled => true;

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public string descriptionKey => m_sDescriptionKey;

	public bool isNationTerritory => m_bIsNationTerritory;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public bool isNationWarTarget => m_bIsNationWarTarget;

	public Rect3D mapRect => m_mapRect;

	public Dictionary<int, ContinentMonsterArrange> monsterArranges => m_monsterArranges;

	public Dictionary<int, ContinentObjectArrange> objectArranges => m_objectArranges;

	public EliteMonsterCategory eliteMonsterCategory => m_eliteMonsterCategory;

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		m_nLocationId = Convert.ToInt32(dr["locationId"]);
		m_nId = Convert.ToInt32(dr["continentId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_sDescriptionKey = Convert.ToString(dr["descriptionKey"]);
		m_bIsNationTerritory = Convert.ToBoolean(dr["isNationTerritory"]);
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		m_bIsNationWarTarget = Convert.ToBoolean(dr["isNationWarTarget"]);
		m_mapRect.x = Convert.ToSingle(dr["x"]);
		m_mapRect.y = Convert.ToSingle(dr["y"]);
		m_mapRect.z = Convert.ToSingle(dr["z"]);
		m_mapRect.sizeX = Convert.ToSingle(dr["xSize"]);
		m_mapRect.sizeY = Convert.ToSingle(dr["ySize"]);
		m_mapRect.sizeZ = Convert.ToSingle(dr["zSize"]);
	}

	public void AddMonsterArrange(ContinentMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentException("monsterArrange");
		}
		if (monsterArrange.continent != null)
		{
			throw new Exception("해당 몬스터배치는 이미 대륙에 추가되어 있습니다.");
		}
		m_monsterArranges.Add(monsterArrange.no, monsterArrange);
		monsterArrange.continent = this;
	}

	public ContinentMonsterArrange GetMonsterArrange(int nArrangeNo)
	{
		if (!m_monsterArranges.TryGetValue(nArrangeNo, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddPortal(Portal portal)
	{
		if (portal == null)
		{
			throw new ArgumentNullException("portal");
		}
		if (portal.continent != null)
		{
			throw new Exception("이미 대륙에 추가된 포탈 입니다.");
		}
		m_portals.Add(portal.id, portal);
		portal.continent = this;
	}

	public Portal GetPortal(int nId)
	{
		if (!m_portals.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddNpc(Npc npc)
	{
		if (npc == null)
		{
			throw new ArgumentNullException("npc");
		}
		m_npcs.Add(npc.id, npc);
	}

	public void AddObjectArrange(ContinentObjectArrange arrange)
	{
		if (arrange == null)
		{
			throw new ArgumentNullException("obj");
		}
		if (arrange.continent != null)
		{
			throw new Exception("이미 대륙에 추가된 오브젝트배치 입니다.");
		}
		m_objectArranges.Add(arrange.no, arrange);
		arrange.continent = this;
	}

	public ContinentObjectArrange GetObjectArrange(int nArrangeNo)
	{
		if (!m_objectArranges.TryGetValue(nArrangeNo, out var value))
		{
			return null;
		}
		return value;
	}

	public void SetEliteMonsterCategory(EliteMonsterCategory category)
	{
		if (category == null)
		{
			throw new ArgumentNullException("category");
		}
		m_eliteMonsterCategory = category;
	}
}
