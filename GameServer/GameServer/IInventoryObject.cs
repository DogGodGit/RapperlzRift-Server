using ClientCommon;

namespace GameServer;

public interface IInventoryObject
{
	int inventoryObjectType { get; }

	InventorySlot inventorySlot { get; set; }

	bool saleable { get; }

	PDInventoryObject ToPDInventoryObject();
}
