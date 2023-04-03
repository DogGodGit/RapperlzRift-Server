using ClientCommon;

namespace GameServer;

public interface IWarehouseObject
{
	int warehouseObjectType { get; }

	WarehouseSlot warehouseSlot { get; set; }

	PDWarehouseObject ToPDWarehouseObject();
}
