namespace GameServer;

public struct MatrixPoint
{
	public int row;

	public int column;

	public static readonly MatrixPoint zero = new MatrixPoint(0, 0);

	public MatrixPoint(int nRow, int nColumn)
	{
		row = nRow;
		column = nColumn;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is MatrixPoint))
		{
			return false;
		}
		return Equals((MatrixPoint)obj);
	}

	public bool Equals(MatrixPoint other)
	{
		return this == other;
	}

	public override int GetHashCode()
	{
		return row.GetHashCode() ^ column.GetHashCode();
	}

	public void Set(int nRow, int nColumn)
	{
		row = nRow;
		column = nColumn;
	}

	public override string ToString()
	{
		return $"({row},{column})";
	}

	public static MatrixPoint operator +(MatrixPoint a, MatrixPoint b)
	{
		return new MatrixPoint(a.row + b.row, a.column + b.column);
	}

	public static MatrixPoint operator -(MatrixPoint a, MatrixPoint b)
	{
		return new MatrixPoint(a.row - b.row, a.column - b.column);
	}

	public static bool operator ==(MatrixPoint a, MatrixPoint b)
	{
		if (a.row == b.row)
		{
			return a.column == b.column;
		}
		return false;
	}

	public static bool operator !=(MatrixPoint a, MatrixPoint b)
	{
		if (a.row == b.row)
		{
			return a.column != b.column;
		}
		return true;
	}
}
