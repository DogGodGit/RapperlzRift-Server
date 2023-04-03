using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class Sector
{
	private Place m_place;

	private int m_nRow;

	private int m_nColumn;

	private Dictionary<Guid, Hero> m_heroes = new Dictionary<Guid, Hero>();

	private Dictionary<long, MonsterInstance> m_monsterInsts = new Dictionary<long, MonsterInstance>();

	private Dictionary<long, CartInstance> m_cartInsts = new Dictionary<long, CartInstance>();

	private Dictionary<long, ContinentObjectInstance> m_objectInsts = new Dictionary<long, ContinentObjectInstance>();

	public Place place => m_place;

	public int row => m_nRow;

	public int column => m_nColumn;

	public Dictionary<Guid, Hero> heroes => m_heroes;

	public Dictionary<long, MonsterInstance> monsterInsts => m_monsterInsts;

	public Dictionary<long, CartInstance> cartInsts => m_cartInsts;

	public Dictionary<long, ContinentObjectInstance> objectInsts => m_objectInsts;

	public Sector(Place place, int nRow, int nColumn)
	{
		m_place = place;
		m_nRow = nRow;
		m_nColumn = nColumn;
	}

	public bool IsInterestSector(int nRow, int nColumn)
	{
		if (nRow >= m_nRow - 1 && nRow <= m_nRow + 1 && nColumn >= m_nColumn - 1)
		{
			return nColumn <= m_nColumn + 1;
		}
		return false;
	}

	public void AddUnit(Unit unit)
	{
		if (unit == null)
		{
			throw new ArgumentException("unit");
		}
		switch (unit.type)
		{
		case UnitType.Hero:
			AddHero((Hero)unit);
			break;
		case UnitType.MonsterInstance:
			AddMonster((MonsterInstance)unit);
			break;
		case UnitType.CartInstance:
			AddCartInstance((CartInstance)unit);
			break;
		default:
			throw new Exception("해당 타입의 유닛을 추가할 할 수 없습니다.");
		}
	}

	public void RemoveUnit(Unit unit)
	{
		if (unit != null)
		{
			switch (unit.type)
			{
			case UnitType.Hero:
				RemoveHero(((Hero)unit).id);
				break;
			case UnitType.MonsterInstance:
				RemoveMonster(((MonsterInstance)unit).instanceId);
				break;
			case UnitType.CartInstance:
				RemoveCartInstance(((CartInstance)unit).instanceId);
				break;
			default:
				throw new Exception("해당 타입의 유닛을 삭제할 할 수 없습니다.");
			}
		}
	}

	public void AddHero(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentException("hero");
		}
		m_heroes.Add(hero.id, hero);
	}

	public void RemoveHero(Guid id)
	{
		m_heroes.Remove(id);
	}

	public void GetHeroIds(ICollection<Guid> buffer, Guid heroIdToExclude)
	{
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.id != heroIdToExclude && !hero.isRidingCart)
			{
				buffer.Add(hero.id);
			}
		}
	}

	public void GetPDHeroes(ICollection<PDHero> buffer, Guid heroIdToExclude, DateTimeOffset currentTime)
	{
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.id != heroIdToExclude && !hero.isRidingCart)
			{
				lock (hero.syncObject)
				{
					buffer.Add(hero.ToPDHero(currentTime));
				}
			}
		}
	}

	public void GetClientPeers(ICollection<ClientPeer> buffer, Guid heroIdToExclude)
	{
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.isReal && hero.id != heroIdToExclude)
			{
				buffer.Add(hero.account.peer);
			}
		}
	}

	public void AddMonster(MonsterInstance monsterInst)
	{
		if (monsterInst == null)
		{
			throw new ArgumentException("monsterInst");
		}
		m_monsterInsts.Add(monsterInst.instanceId, monsterInst);
	}

	public void RemoveMonster(long lnInstanceId)
	{
		m_monsterInsts.Remove(lnInstanceId);
	}

	public void GetMonsters(ICollection<MonsterInstance> buffer)
	{
		foreach (MonsterInstance monsterInst in m_monsterInsts.Values)
		{
			buffer.Add(monsterInst);
		}
	}

	public void GetMonsterInstanceIds(ICollection<long> buffer)
	{
		foreach (MonsterInstance monsterInst in m_monsterInsts.Values)
		{
			buffer.Add(monsterInst.instanceId);
		}
	}

	public void GetPDMonsterInstances<T>(ICollection<T> buffer, DateTimeOffset currentTime) where T : PDMonsterInstance
	{
		foreach (MonsterInstance monsterInst in m_monsterInsts.Values)
		{
			buffer.Add((T)monsterInst.ToPDMonsterInstance(currentTime));
		}
	}

	public void AddContinentObject(ContinentObjectInstance objectInst)
	{
		if (objectInst == null)
		{
			throw new ArgumentNullException("objectInst");
		}
		m_objectInsts.Add(objectInst.instanceId, objectInst);
	}

	public void RemoveContinentObject(long lnInstanceId)
	{
		m_objectInsts.Remove(lnInstanceId);
	}

	public void GetContinentObjectInstanceIds(ICollection<long> buffer)
	{
		foreach (ContinentObjectInstance objectInst in m_objectInsts.Values)
		{
			buffer.Add(objectInst.instanceId);
		}
	}

	public void GetPDContinentObjectInstances(ICollection<PDContinentObjectInstance> buffer)
	{
		foreach (ContinentObjectInstance objectInst in m_objectInsts.Values)
		{
			buffer.Add(objectInst.ToPDContinentObjectInstance());
		}
	}

	public void AddCartInstance(CartInstance cartInst)
	{
		if (cartInst == null)
		{
			throw new ArgumentException("cartInst");
		}
		m_cartInsts.Add(cartInst.instanceId, cartInst);
	}

	public void RemoveCartInstance(long lnInstanceId)
	{
		m_cartInsts.Remove(lnInstanceId);
	}

	public void GetCartInstanceIds(ICollection<long> buffer, long lnInstanceIdToExclude)
	{
		foreach (CartInstance cartInst in m_cartInsts.Values)
		{
			if (cartInst.instanceId != lnInstanceIdToExclude)
			{
				buffer.Add(cartInst.instanceId);
			}
		}
	}

	public void GetPDCartInstances(ICollection<PDCartInstance> buffer, long lnInstanceIdToExclude, DateTimeOffset currentTime)
	{
		foreach (CartInstance cartInst in m_cartInsts.Values)
		{
			if (cartInst.instanceId != lnInstanceIdToExclude)
			{
				lock (cartInst.syncObject)
				{
					buffer.Add(cartInst.ToPDCartInstance(currentTime));
				}
			}
		}
	}

	public static void GetClientPeers(IEnumerable<Sector> sectors, Guid heroIdToExclude, ICollection<ClientPeer> buffer)
	{
		foreach (Sector sector in sectors)
		{
			sector.GetClientPeers(buffer, heroIdToExclude);
		}
	}

	public static List<ClientPeer> GetClientPeers(IEnumerable<Sector> sectors, Guid heroIdToExclude)
	{
		List<ClientPeer> results = new List<ClientPeer>();
		GetClientPeers(sectors, heroIdToExclude, results);
		return results;
	}

	public static List<PDHero> GetPDHeroes(IEnumerable<Sector> sectors, Guid heroIdToExclude, DateTimeOffset currentTime)
	{
		List<PDHero> results = new List<PDHero>();
		foreach (Sector sector in sectors)
		{
			sector.GetPDHeroes(results, heroIdToExclude, currentTime);
		}
		return results;
	}

	public static List<Guid> GetHeroIds(IEnumerable<Sector> sectors, Guid heroIdToExclude)
	{
		List<Guid> results = new List<Guid>();
		foreach (Sector sector in sectors)
		{
			sector.GetHeroIds(results, heroIdToExclude);
		}
		return results;
	}

	public static List<T> GetPDMonsterInstances<T>(IEnumerable<Sector> sectors, DateTimeOffset currentTime) where T : PDMonsterInstance
	{
		List<T> results = new List<T>();
		foreach (Sector sector in sectors)
		{
			sector.GetPDMonsterInstances(results, currentTime);
		}
		return results;
	}

	public static List<long> GetMonsterInstanceIds(IEnumerable<Sector> sectors)
	{
		List<long> results = new List<long>();
		foreach (Sector sector in sectors)
		{
			sector.GetMonsterInstanceIds(results);
		}
		return results;
	}

	public static List<PDContinentObjectInstance> GetPDContinentObjectInstances(IEnumerable<Sector> sectors)
	{
		List<PDContinentObjectInstance> results = new List<PDContinentObjectInstance>();
		foreach (Sector sector in sectors)
		{
			sector.GetPDContinentObjectInstances(results);
		}
		return results;
	}

	public static List<long> GetContinentObjectInstanceIds(IEnumerable<Sector> sectors)
	{
		List<long> results = new List<long>();
		foreach (Sector sector in sectors)
		{
			sector.GetContinentObjectInstanceIds(results);
		}
		return results;
	}

	public static List<PDCartInstance> GetPDCartInstances(IEnumerable<Sector> sectors, long lnInstanceIdToExclude, DateTimeOffset currentTime)
	{
		List<PDCartInstance> results = new List<PDCartInstance>();
		foreach (Sector sector in sectors)
		{
			sector.GetPDCartInstances(results, lnInstanceIdToExclude, currentTime);
		}
		return results;
	}

	public static List<long> GetCartInstanceIds(IEnumerable<Sector> sectors, long lnInstanceIdToExclude)
	{
		List<long> results = new List<long>();
		foreach (Sector sector in sectors)
		{
			sector.GetCartInstanceIds(results, lnInstanceIdToExclude);
		}
		return results;
	}
}
