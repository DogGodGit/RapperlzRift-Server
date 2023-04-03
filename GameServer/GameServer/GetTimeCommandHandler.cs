using System;
using ClientCommon;

namespace GameServer;

public class GetTimeCommandHandler : CommandHandler<GetTimeCommandBody, GetTimeResponseBody>
{
	protected override void HandleCommandInternal()
	{
		GetTimeResponseBody resBody = new GetTimeResponseBody();
		resBody.time = (DateTimeOffset)DateTimeUtil.currentTime;
		SendResponseOK(resBody);
	}
}
