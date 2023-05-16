using System.Collections.Generic;
using System.Xml.Linq;

namespace ServerFramework;

public class SFTextFilterNode
{
	private char m_char;

	private bool m_bFinal;

	private Dictionary<char, SFTextFilterNode> m_children = new Dictionary<char, SFTextFilterNode>();

	public char value
	{
		get
		{
			return m_char;
		}
		set
		{
			m_char = value;
		}
	}

	public bool final
	{
		get
		{
			return m_bFinal;
		}
		set
		{
			m_bFinal = value;
		}
	}

	public SFTextFilterNode AddChild(char value)
	{
		value = char.ToLower(value);
		SFTextFilterNode result = null;
		if (m_children.TryGetValue(value, out result))
		{
			return result;
		}
		result = new SFTextFilterNode();
		result.value = value;
		m_children.Add(value, result);
		return result;
	}

	public SFTextFilterNode FindChild(char value)
	{
		SFTextFilterNode result = null;
		if (!m_children.TryGetValue(char.ToLower(value), out result))
		{
			return null;
		}
		return result;
	}

	public void ClearChildren()
	{
		m_children.Clear();
	}

	public XElement ToXml()
	{
		XElement xElement = new XElement("node");
		xElement.SetAttributeValue("value", (m_char == '\0') ? "" : m_char.ToString());
		foreach (SFTextFilterNode value in m_children.Values)
		{
			xElement.Add(value.ToXml());
		}
		return xElement;
	}
}
