using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class FriendApplication
{
	private long m_lnNo;

	private Hero m_applicant;

	private Hero m_target;

	public static readonly SFSynchronizedLongFactory noFactory = new SFSynchronizedLongFactory();

	public long no => m_lnNo;

	public Hero applicant => m_applicant;

	public Hero target => m_target;

	public FriendApplication(Hero applicant, Hero target)
	{
		if (applicant == null)
		{
			throw new ArgumentNullException("applicant");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		m_lnNo = noFactory.NewValue();
		m_applicant = applicant;
		m_target = target;
	}

	public PDFriendApplication ToPDFriendApplication()
	{
		PDFriendApplication inst = new PDFriendApplication();
		inst.no = m_lnNo;
		inst.applicantId = (Guid)m_applicant.id;
		inst.applicantName = m_applicant.name;
		inst.applicantNationId = m_applicant.nationId;
		inst.targetId = (Guid)m_target.id;
		inst.targetName = m_target.name;
		inst.targetNationId = m_target.nationId;
		return inst;
	}
}
