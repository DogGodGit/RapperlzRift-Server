using Photon.SocketServer;

namespace ServerFramework;

public abstract class SFStandaloneWork : SFWork
{
	public override void Schedule()
	{
		((SFApplication)ApplicationBase.Instance).AddStandaloneWork(this);
	}
}
