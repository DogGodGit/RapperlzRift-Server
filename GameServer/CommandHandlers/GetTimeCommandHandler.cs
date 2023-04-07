using System;
using ClientCommon;

namespace GameServer.CommandHandlers;

public class GetTimeCommandHandler : CommandHandler<GetTimeCommandBody, GetTimeResponseBody>
{
    protected override void HandleCommandInternal()
    {
        GetTimeResponseBody resBody = new GetTimeResponseBody();
        resBody.time = DateTimeUtil.currentTime;
        SendResponseOK(resBody);
    }
}
