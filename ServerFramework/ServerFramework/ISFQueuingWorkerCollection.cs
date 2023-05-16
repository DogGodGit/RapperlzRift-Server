namespace ServerFramework;

public interface ISFQueuingWorkerCollection
{
	void AddWork(object workerId, ISFRunnable work);
}
