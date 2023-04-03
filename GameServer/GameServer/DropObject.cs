using System;
using ClientCommon;

namespace GameServer;

public abstract class DropObject
{
	public const int kType_MainGear = 1;

	public const int kType_Item = 2;

	protected MonsterInstance m_source;

	public abstract int type { get; }

	public MonsterInstance source
	{
		get
		{
			return m_source;
		}
		set
		{
			m_source = value;
		}
	}

	public abstract PDDropObject ToPDDropObject();

	public static DropObject Create(DropObjectPoolEntry poolEntry)
	{
		if (poolEntry == null)
		{
			throw new ArgumentNullException("poolEntry");
		}
		return poolEntry.type switch
		{
			1 => new MainGearDropObject(poolEntry.mainGear, poolEntry.mainGearOwned, 0), 
			2 => new ItemDropObject(poolEntry.item, poolEntry.itemCount, poolEntry.itemOwned), 
			_ => null, 
		};
	}
}
