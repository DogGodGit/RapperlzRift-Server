using System;
using System.Collections.Generic;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace ServerFramework;

public abstract class SFPeerImpl
{
	protected Guid m_id = Guid.Empty;

	protected ISFPeer m_peer;

	protected SFDynamicWorker m_worker = new SFDynamicWorker();

	public Guid id => m_id;

	public int workCount => m_worker.workCount;

	public SFPeerImpl(ISFPeer peer)
	{
		if (peer == null)
		{
			throw new ArgumentNullException("peer");
		}
		m_id = Guid.NewGuid();
		m_peer = peer;
		m_worker.isAsyncErrorLogging = true;
	}

	public void AddWork(ISFRunnable work)
	{
		m_worker.EnqueueWork(work);
	}

	public void StartWorker()
	{
		m_worker.Start();
	}

	public void StopWorker(bool bClearQueue)
	{
		m_worker.Stop(bClearQueue);
	}

	public SendResult SendResponse(short snResult, string sErrorMessage, short snCommandName, long lnPacketId, byte[] body, SendParameters sendParameters)
	{
		return m_peer.SendOperationResponse(SFUtil.CreateOperationResponse(snResult, sErrorMessage, snCommandName, lnPacketId, body), sendParameters);
	}

	public SendResult SendEvent(short snEventName, byte[] body, SendParameters sendParameters)
	{
		return m_peer.SendEvent(SFUtil.CreateEventData(snEventName, body), sendParameters);
	}

	public void OnCommand(short snCommandName, long lnPacketId, byte[] body, SendParameters sendParameters)
	{
		ISFCommandHandler iSFCommandHandler = m_peer.commandHandlerFactory.CreateHandler(snCommandName);
		if (iSFCommandHandler == null)
		{
			throw new Exception($"'{snCommandName}' 커맨드 핸들러가 존재하지 않습니다.");
		}
		iSFCommandHandler.Init(m_peer, snCommandName, lnPacketId, body, sendParameters);
		m_peer.AddWork(iSFCommandHandler);
	}

	public void OnEvent(short snEventName, byte[] body, SendParameters sendParameters)
	{
		ISFEventHandler iSFEventHandler = m_peer.eventHandlerFactory.CreateHandler(snEventName);
		if (iSFEventHandler == null)
		{
			throw new Exception($"'{snEventName}' 이벤트 핸들러가 존재하지 않습니다.");
		}
		iSFEventHandler.Init(m_peer, snEventName, body, sendParameters);
		m_peer.AddWork(iSFEventHandler);
	}

	public abstract void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters);

	protected void HandleCommand(Dictionary<byte, object> parameters, SendParameters sendParameters)
	{
		short snCommandName = 0;
		long lnPacketId = 0L;
		byte[] array = null;
		try
		{
			snCommandName = (short)parameters[0];
			lnPacketId = (long)parameters[1];
			array = (byte[])parameters[2];
			m_peer.OnCommand(snCommandName, lnPacketId, array, sendParameters);
		}
		catch (SFCommandHandleException ex)
		{
			m_peer.SendResponse(ex.result, ex.Message, snCommandName, lnPacketId, null, sendParameters);
			if (ex.loggingEnabled)
			{
				SFLogUtil.Error(GetType(), null, ex);
			}
		}
		catch (Exception ex2)
		{
			m_peer.SendResponse(1, ex2.Message, snCommandName, lnPacketId, null, sendParameters);
			SFLogUtil.Error(GetType(), null, ex2);
		}
	}

	protected void HandleEvent(Dictionary<byte, object> parameters, SendParameters sendParameters)
	{
		try
		{
			short snEventName = (short)parameters[0];
			byte[] body = (byte[])parameters[1];
			m_peer.OnEvent(snEventName, body, sendParameters);
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	public virtual void OnDisconnected(DisconnectReason reasonCode, string sReasonDetail)
	{
		StopWorker(bClearQueue: true);
	}
}
