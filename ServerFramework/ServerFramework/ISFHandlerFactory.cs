namespace ServerFramework;

public interface ISFHandlerFactory<TKey, out THandler> where THandler : ISFHandler
{
	THandler CreateHandler(TKey key);
}
