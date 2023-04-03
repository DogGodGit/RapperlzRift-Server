using System;
using ClientCommon;

namespace GameServer;

public class NationWarHistoryCommandHandler : InGameCommandHandler<NationWarHistoryCommandBody, NationWarHistoryResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		Cache cache = Cache.instance;
		DateTime currentDate = DateTimeUtil.currentTime.Date;
		NationWarManager nationWarManager = cache.nationWarManager;
		NationWarHistoryResponseBody resBody = new NationWarHistoryResponseBody();
		resBody.histories = nationWarManager.GetPDNationWarHistories(currentDate).ToArray();
		SendResponseOK(resBody);
	}
}
