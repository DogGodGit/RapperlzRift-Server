using ClientCommon;

namespace GameServer;

public struct Vector3
{
	public float x;

	public float y;

	public float z;

	public static readonly Vector3 zero = new Vector3(0f, 0f, 0f);

	public static readonly Vector3 forward = new Vector3(0f, 0f, 1f);

	public Vector3(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Vector3))
		{
			return false;
		}
		return Equals((Vector3)obj);
	}

	public bool Equals(Vector3 other)
	{
		return this == other;
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
	}

	public void Set(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public override string ToString()
	{
		return $"({x},{y},{z})";
	}

	public static implicit operator PDVector3(Vector3 src)
	{
		return new PDVector3(src.x, src.y, src.z);
	}

	public static implicit operator Vector3(PDVector3 src)
	{
		return new Vector3(src.x, src.y, src.z);
	}

	public static Vector3 operator +(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static Vector3 operator -(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static bool operator ==(Vector3 a, Vector3 b)
	{
		if (a.x == b.x && a.y == b.y)
		{
			return a.z == b.z;
		}
		return false;
	}

	public static bool operator !=(Vector3 a, Vector3 b)
	{
		if (a.x == b.x && a.y == b.y)
		{
			return a.z != b.z;
		}
		return true;
	}
}
