namespace GameServer;

public struct Rect
{
	public float x;

	public float y;

	public float width;

	public float height;

	public static readonly Rect zero = new Rect(0f, 0f, 0f, 0f);

	public float xMin
	{
		get
		{
			return x;
		}
		set
		{
			width += x - value;
			x = value;
		}
	}

	public float xMax
	{
		get
		{
			return x + width;
		}
		set
		{
			width = value - x;
		}
	}

	public float yMin
	{
		get
		{
			return y;
		}
		set
		{
			height += y - value;
			y = value;
		}
	}

	public float yMax
	{
		get
		{
			return y + height;
		}
		set
		{
			height = value - y;
		}
	}

	public Rect(float fX, float fY, float fWidth, float fHeight)
	{
		x = fX;
		y = fY;
		width = fWidth;
		height = fHeight;
	}

	public bool Contains(float fX, float fY)
	{
		if (fX >= x && fX < x + width && fY >= y)
		{
			return fY < y + height;
		}
		return false;
	}

	public bool ContainsCircle(float fCenterX, float fCenterY, float fRadius)
	{
		if (fCenterX - fRadius >= x && fCenterX + fRadius < x + width && fCenterY - fRadius >= y)
		{
			return fCenterY + fRadius < y + height;
		}
		return false;
	}
}
