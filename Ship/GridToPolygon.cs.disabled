using System.Linq;
using Godot;

public static class GridToPolygon
{
	public static Polygon2D GridToPolygon2D(Vector2[] grid)
	{
		var polygon = new Polygon2D();

		for (int i = 0; i < grid.Length; i++)
		{
			var point = grid[i];
			polygon.Polygon.Append(point);
		}
		return polygon;
	}
}