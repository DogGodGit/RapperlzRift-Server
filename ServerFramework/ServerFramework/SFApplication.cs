using System.IO;
using System.Threading;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net;
using log4net.Config;
using Photon.SocketServer;

namespace ServerFramework;

public abstract class SFApplication : ApplicationBase
{
	protected SFStaticWorker m_logWorker = new SFStaticWorker();

	protected SFQueuingWorkManager m_queuingWorkManager = new SFQueuingWorkManager();

	protected SFStandaloneWorker m_standaloneWorker = new SFStandaloneWorker();

	protected override void Setup()
	{
		Initialize();
		OnInitialized();
	}

	protected virtual void Initialize()
	{
		ThreadPool.GetMinThreads(out var _, out var completionPortThreads);
		ThreadPool.GetMaxThreads(out var workerThreads2, out var _);
		ThreadPool.SetMinThreads(workerThreads2, completionPortThreads);
		GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(base.ApplicationRootPath, "log");
		GlobalContext.Properties["LogFileName"] = base.PhotonInstanceName + "." + base.ApplicationName;
		ExitGames.Logging.LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
		XmlConfigurator.ConfigureAndWatch((FileInfo)(object)new FileInfo(Path.Combine(base.BinaryPath, "log4net.config")));
		StartLogWorker();
		StartStandaloneWorker();
		InitQueuingWorkManager();
	}

	protected abstract void InitQueuingWorkManager();

	protected virtual void OnInitialized()
	{
	}

	private void StartLogWorker()
	{
		m_logWorker.isAsyncErrorLogging = false;
		m_logWorker.Start();
	}

	private void StopLogWorker()
	{
		m_logWorker.StopAndWaitFinish(bClearQueue: false);
		SFLogUtil.Info(((object)this).GetType(), "LogWorker finished.", null, bStackTrace: false, bAsync: false);
	}

	public void AddLogWork(ISFRunnable work)
	{
		m_logWorker.EnqueueWork(work);
	}

	public void AddQueuingWork(SFMainQueuingWork work)
	{
		m_queuingWorkManager.AddWork(work);
	}

	private void StopQueuingWorkManager()
	{
		m_queuingWorkManager.StopAndWaitFinish();
		SFLogUtil.Info(((object)this).GetType(), "QueuingWorkManager finished.");
	}

	public void AddStandaloneWork(ISFRunnable work)
	{
		m_standaloneWorker.AddWork(work);
	}

	private void StartStandaloneWorker()
	{
		m_standaloneWorker.isAsyncErrorLogging = true;
		m_standaloneWorker.Start();
	}

	private void StopStandaloneWorker()
	{
		m_standaloneWorker.StopAndWaitFinish();
		SFLogUtil.Info(((object)this).GetType(), "StandaloneWorker finished.");
	}

	protected override void TearDown()
	{
		SFLogUtil.Info(((object)this).GetType(), "TearDown started.");
		OnTearDown();
		StopQueuingWorkManager();
		StopStandaloneWorker();
		StopLogWorker();
		SFLogUtil.Info(((object)this).GetType(), "TearDown finished.", null, bStackTrace: false, bAsync: false);
	}

	protected virtual void OnTearDown()
	{
	}
}
