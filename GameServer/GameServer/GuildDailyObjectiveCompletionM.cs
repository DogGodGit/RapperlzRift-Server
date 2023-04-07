using System;
using System.Data;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class GuildDailyObjectiveCompletionMember
{
	private Guild m_guild;

	private Guid m_id = Guid.Empty;

	private string m_sName;

	public Guid id => m_id;

	public string name => m_sName;

	public GuildDailyObjectiveCompletionMember(Guild guild)
		: this(guild, Guid.Empty, null)
	{
	}

	public GuildDailyObjectiveCompletionMember(Guild guild, Guid id, string sName)
	{
		m_guild = guild;
		m_id = id;
		m_sName = sName;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["heroId"];
		m_sName = Convert.ToString(dr["name"]);
	}

	public PDGuildDailyObjectiveCompletionMember ToPDGuildDailyObjectiveCompletionMember()
	{
		PDGuildDailyObjectiveCompletionMember inst = new PDGuildDailyObjectiveCompletionMember();
		inst.id = (Guid)m_id;
		inst.name = m_sName;
		return inst;
	}
}
public class GuildDailyObjectiveCompletionMemberListCommandHandler : InGameCommandHandler<GuildDailyObjectiveCompletionMemberListCommandBody, GuildDailyObjectiveCompletionMemberListResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	private GuildMember m_myGuildMember;

	private Guild m_myGuild;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_myGuildMember = m_myHero.guildMember;
		if (m_myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어있지 않습니다.");
		}
		m_myGuild = m_myGuildMember.guild;
		GuildDailyObjectiveCompletionMemberListResponseBody resBody = new GuildDailyObjectiveCompletionMemberListResponseBody();
		resBody.completionMembers = m_myGuild.GetPDGuildDailyObjectiveCompletionMembers().ToArray();
		SendResponseOK(resBody);
	}
}
