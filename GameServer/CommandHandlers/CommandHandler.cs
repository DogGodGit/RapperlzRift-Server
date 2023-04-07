using System;
using ClientCommon;
using Photon.SocketServer;
using ServerFramework;

namespace GameServer.CommandHandlers;

public abstract class CommandHandler<TCommandBody, TResponseBody> : SFCommandHandler, ICommandHandler, ISFCommandHandler, ISFHandler, ISFRunnable where TCommandBody : CommandBody where TResponseBody : ResponseBody
{
    protected TCommandBody m_body = null;

    public ClientPeer clientPeer => (ClientPeer)m_sender;

    public ClientCommandName commandName => (ClientCommandName)m_snPacketName;

    protected virtual bool globalLockRequired => false;

    protected override void DeserializeBody()
    {
        m_body = Body.DeserializeRaw<TCommandBody>(m_rawBody);
    }

    public void SendResponse(short snResult, string sErrorMessage, TResponseBody body, SendParameters sendParameters)
    {
        SendResponse(snResult, sErrorMessage, Body.SerializeRaw(body), sendParameters);
    }

    public void SendResponseOK(TResponseBody body, SendParameters sendParameters)
    {
        SendResponse(0, null, body, sendParameters);
    }

    public void SendResponseOK(TResponseBody body)
    {
        SendResponseOK(body, default);
    }

    protected override void HandleCommand()
    {
        if (globalLockRequired)
        {
            lock (Global.syncObject)
            {
                InvokeHandleCommandInternal();
                return;
            }
        }
        InvokeHandleCommandInternal();
    }

    private void InvokeHandleCommandInternal()
    {
        ClientPeerSynchronizer.Exec(clientPeer, new SFAction(HandleCommandInternal));
    }

    protected abstract void HandleCommandInternal();

    protected void Reenter(ISFRunnable work)
    {
        if (work == null)
        {
            throw new ArgumentNullException("work");
        }
        clientPeer.AddWork(new SFAction<ISFRunnable>(ProcessReentry, work));
    }

    private void ProcessReentry(ISFRunnable work)
    {
        try
        {
            if (globalLockRequired)
            {
                lock (Global.syncObject)
                {
                    InvokeProcessReentryInternal(work);
                    return;
                }
            }
            InvokeProcessReentryInternal(work);
        }
        catch (CommandHandleException ex2)
        {
            SendResponse(ex2.result, ex2.Message, null);
            if (ex2.loggingEnabled)
            {
                LogError(null, ex2);
            }
        }
        catch (Exception ex)
        {
            SendResponse(1, ex.Message, null);
            LogError(null, ex);
        }
    }

    private void InvokeProcessReentryInternal(ISFRunnable work)
    {
        ClientPeerSynchronizer.Exec(clientPeer, new SFAction<ISFRunnable>(ProcessReentryInternal, work));
    }

    private void ProcessReentryInternal(ISFRunnable work)
    {
        OnReentry(work);
        work.Run();
    }

    protected virtual void OnReentry(ISFRunnable work)
    {
    }

    protected void RunWork(SFWork work)
    {
        work.finishHandler = WorkFinishHandler;
        work.Schedule();
    }

    private void WorkFinishHandler(SFWork work, Exception error)
    {
        Reenter(new SFAction<SFWork, Exception>(OnWorkFinished, work, error));
    }

    private void OnWorkFinished(SFWork work, Exception error)
    {
        if (error == null)
        {
            OnWork_Success(work);
        }
        else
        {
            OnWork_Error(work, error);
        }
    }

    protected virtual void OnWork_Success(SFWork work)
    {
    }

    protected virtual void OnWork_Error(SFWork work, Exception error)
    {
        short snResult = 1;
        bool bLoggingEnabled = true;
        if (error is CommandHandleException)
        {
            CommandHandleException cmdError = (CommandHandleException)error;
            snResult = ((CommandHandleException)error).result;
            bLoggingEnabled = cmdError.loggingEnabled;
        }
        SendResponse(snResult, error.Message, null);
        if (bLoggingEnabled)
        {
            LogError(null, error);
        }
    }
}
