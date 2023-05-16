using System.Xml.Linq;

namespace ServerFramework;

public class SFTextFilter
{
	private SFTextFilterNode m_root = new SFTextFilterNode();

	public void AddElement(string sElement)
	{
		if (string.IsNullOrEmpty(sElement))
		{
			return;
		}
		SFTextFilterNode sFTextFilterNode = m_root;
		char[] array = sElement.ToCharArray();
		foreach (char value in array)
		{
			sFTextFilterNode = sFTextFilterNode.AddChild(value);
			if (sFTextFilterNode.final)
			{
				break;
			}
		}
		sFTextFilterNode.final = true;
		sFTextFilterNode.ClearChildren();
	}

	private bool Search(char[] chars, int nStartIndex, out int nResultStartIndex, out int nResultEndIndex)
	{
		nResultStartIndex = -1;
		nResultEndIndex = -1;
		int num = -1;
		SFTextFilterNode sFTextFilterNode = m_root;
		for (int i = nStartIndex; i < chars.Length; i++)
		{
			if (num == -1)
			{
				num = i;
			}
			sFTextFilterNode = sFTextFilterNode.FindChild(chars[i]);
			if (sFTextFilterNode == null)
			{
				i = num;
				num = -1;
				sFTextFilterNode = m_root;
			}
			else if (sFTextFilterNode.final)
			{
				nResultStartIndex = num;
				nResultEndIndex = i;
				return true;
			}
		}
		return false;
	}

	public string Filter(string sTargetText, char replacement)
	{
		if (string.IsNullOrEmpty(sTargetText))
		{
			return sTargetText;
		}
		char[] array = sTargetText.ToCharArray();
		int nStartIndex = 0;
		int nResultStartIndex;
		int nResultEndIndex;
		while (Search(array, nStartIndex, out nResultStartIndex, out nResultEndIndex))
		{
			for (int i = nResultStartIndex; i <= nResultEndIndex; i++)
			{
				array[i] = replacement;
			}
			nStartIndex = nResultEndIndex + 1;
		}
		return new string(array);
	}

	public bool ContainsElement(string sTargetText)
	{
		if (string.IsNullOrEmpty(sTargetText))
		{
			return false;
		}
		int nResultStartIndex;
		int nResultEndIndex;
		return Search(sTargetText.ToCharArray(), 0, out nResultStartIndex, out nResultEndIndex);
	}

	public XElement ToXml()
	{
		return m_root.ToXml();
	}
}
