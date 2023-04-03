namespace GameServer;

public struct Rect3D
{
	public float x;

	public float y;

	public float z;

	public float sizeX;

	public float sizeY;

	public float sizeZ;

	public static readonly Rect3D zero = new Rect3D(0f, 0f, 0f, 0f, 0f, 0f);

	public float xMin
	{
		get
		{
			return x;
		}
		set
		{
			sizeX += x - value;
			x = value;
		}
	}

	public float xMax
	{
		get
		{
			return x + sizeX;
		}
		set
		{
			sizeX = value - x;
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
			sizeY += y - value;
			y = value;
		}
	}

	public float yMax
	{
		get
		{
			return y + sizeY;
		}
		set
		{
			sizeY = value - y;
		}
	}

	public float zMin
	{
		get
		{
			return z;
		}
		set
		{
			sizeZ += z - value;
			z = value;
		}
	}

	public float zMax
	{
		get
		{
			return z + sizeZ;
		}
		set
		{
			sizeZ = value - z;
		}
	}

	public Rect3D(float fX, float fY, float fZ, float fSizeX, float fSizeY, float fSizeZ)
	{
		x = fX;
		y = fY;
		z = fZ;
		sizeX = fSizeX;
		sizeY = fSizeY;
		sizeZ = fSizeX;
	}

	public bool Contains(float fX, float fY, float fZ)
	{
		if (fX >= x && fX < x + sizeX && fY >= y && fY < y + sizeY && fZ >= z)
		{
			return fZ < z + sizeZ;
		}
		return false;
	}

	public bool ContainsCircle(float fCenterX, float fCenterZ, float fRadius)
	{
		if (fCenterX - fRadius >= x && fCenterX + fRadius < x + sizeX && fCenterZ - fRadius >= z)
		{
			return fCenterZ + fRadius < z + sizeZ;
		}
		return false;
	}
}
