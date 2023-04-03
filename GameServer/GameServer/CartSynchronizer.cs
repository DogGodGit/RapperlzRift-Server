using System;
using ServerFramework;

namespace GameServer;

public static class CartSynchronizer
{
	public static void Exec(CartInstance cartInst, ISFRunnable runnable)
	{
		if (cartInst == null)
		{
			throw new ArgumentNullException("cartInst");
		}
		if (runnable == null)
		{
			throw new ArgumentNullException("runnable");
		}
		Place place = null;
		do
		{
			lock (cartInst.syncObject)
			{
				place = cartInst.currentPlace;
			}
		}
		while (!Run_Place(cartInst, runnable, place));
	}

	private static bool Run_Place(CartInstance cartInst, ISFRunnable runnable, Place place)
	{
		if (place == null)
		{
			return Run_CartInstance(cartInst, runnable, place);
		}
		lock (place.syncObject)
		{
			return Run_CartInstance(cartInst, runnable, place);
		}
	}

	private static bool Run_CartInstance(CartInstance cartInst, ISFRunnable runnable, Place place)
	{
		lock (cartInst.syncObject)
		{
			return Run_Runnable(cartInst, runnable, place);
		}
	}

	private static bool Run_Runnable(CartInstance cartInst, ISFRunnable runnable, Place place)
	{
		if (place != cartInst.currentPlace)
		{
			return false;
		}
		runnable.Run();
		return true;
	}
}
