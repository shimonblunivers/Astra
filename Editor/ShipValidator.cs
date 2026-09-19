using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class ShipValidator : RefCounted
{
    public static int Layer { get; set; } = 0;
    public static Vector2I FillAtlas { get; set; } = new Vector2I(0, 1);

    public static readonly HashSet<string> Walls = new HashSet<string>
    {
        "wall", "door", "thruster", "connector"
    };

    public static readonly HashSet<string> Floors = new HashSet<string>
    {
        "floor"
    };

    public static void AutofillFloor(TileMapLayer tilemap)
    {
        if (tilemap == null) return;
        var usedCells = tilemap.GetUsedCells();
        if (usedCells.Count == 0) return;

        Vector2I bottomLeft = FindBottomLeftEdge(usedCells);
        Vector2I topRight = FindTopRightEdge(usedCells);

        // Vector4I: X = left, Y = right, Z = top, W = bottom
        Vector4I limits = new Vector4I(bottomLeft.X - 1, topRight.X + 1, topRight.Y - 1, bottomLeft.Y + 1);

        Bucket(tilemap, limits);
        InvertFloorTiles(tilemap, limits);
    }

    public static bool CheckValidity(TileMapLayer tilemap)
    {
        if (tilemap == null) return false;
        var usedCells = tilemap.GetUsedCells();
        if (usedCells.Count == 0) return false;

        Vector2I bottomLeft = FindBottomLeftEdge(usedCells);
        Vector2I topRight = FindTopRightEdge(usedCells);

        Vector4I limits = new Vector4I(bottomLeft.X, topRight.X, topRight.Y, bottomLeft.Y);
        return Validate(tilemap, limits, usedCells);
    }

    private static Vector2I FindBottomLeftEdge(Godot.Collections.Array<Vector2I> usedCells)
    {
        Vector2I edge = usedCells[0];
        foreach (Vector2I cell in usedCells)
        {
            if (cell.X < edge.X) edge.X = cell.X;
            if (cell.Y > edge.Y) edge.Y = cell.Y;
        }
        return edge;
    }

    private static Vector2I FindTopRightEdge(Godot.Collections.Array<Vector2I> usedCells)
    {
        Vector2I edge = usedCells[0];
        foreach (Vector2I cell in usedCells)
        {
            if (cell.X > edge.X) edge.X = cell.X;
            if (cell.Y < edge.Y) edge.Y = cell.Y;
        }
        return edge;
    }

    private static void Bucket(TileMapLayer tilemap, Vector4I limits)
    {
        var visited = new HashSet<Vector2I>();
        var queue = new Queue<Vector2I>();

        Vector2I startPoint = new Vector2I(limits.X, limits.W);
        queue.Enqueue(startPoint);
        visited.Add(startPoint);

        while (queue.Count > 0)
        {
            Vector2I current = queue.Dequeue();

            if (!IsWall(tilemap, current))
            {
                ShipEditor.SellTile(tilemap, current, true, true);
                tilemap.SetCell(current, 0, FillAtlas);

                foreach (Vector2I neighbor in GetSurroundingCells(current, limits))
                {
                    if (visited.Add(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
    }

    public static bool IsWall(TileMapLayer tilemap, Vector2I coords)
    {
        string type = GetTileType(tilemap, coords, Layer);
        return Walls.Contains(type);
    }

    public static string GetTileType(TileMapLayer tilemap, Vector2I coords, int layer = 0)
    {
        if (tilemap?.TileSet == null) return string.Empty;

        int sourceId = tilemap.GetCellSourceId(coords);
        if (sourceId == -1) return string.Empty;

        Vector2I atlasCoords = tilemap.GetCellAtlasCoords(coords);
        if (atlasCoords == new Vector2I(-1, -1)) return string.Empty;

        var source = tilemap.TileSet.GetSource(sourceId) as TileSetAtlasSource;
        if (source == null) return string.Empty;

        TileData tileData = source.GetTileData(atlasCoords, 0);
        if (tileData == null) return string.Empty;

        Variant customData = tileData.GetCustomData("type");
        return customData.VariantType != Variant.Type.Nil ? customData.AsString() : string.Empty;
    }

    private static List<Vector2I> GetSurroundingCells(Vector2I coords, Vector4I limits)
    {
        var cells = new List<Vector2I>(4);

        if (coords.X - 1 >= limits.X) cells.Add(coords - new Vector2I(1, 0));
        if (coords.X + 1 <= limits.Y) cells.Add(coords + new Vector2I(1, 0));
        if (coords.Y - 1 >= limits.Z) cells.Add(coords - new Vector2I(0, 1));
        if (coords.Y + 1 <= limits.W) cells.Add(coords + new Vector2I(0, 1));

        return cells;
    }

    private static void InvertFloorTiles(TileMapLayer tilemap, Vector4I limits, Vector2I? atlasCoordsFill = null)
    {
        Vector2I fill = atlasCoordsFill ?? new Vector2I(0, 0);

        for (int x = limits.X; x <= limits.Y; x++)
        {
            for (int y = limits.Z; y <= limits.W; y++)
            {
                Vector2I cell = new Vector2I(x, y);
                Vector2I cellAtlas = tilemap.GetCellAtlasCoords(cell);

                if (cellAtlas == new Vector2I(-1, -1))
                {
                    tilemap.SetCell(cell, 0, fill);
                }
                else if (cellAtlas == FillAtlas)
                {
                    tilemap.SetCell(cell, 0, new Vector2I(-1, -1));
                }
            }
        }
    }

    private static bool Validate(TileMapLayer tilemap, Vector4I limits, Godot.Collections.Array<Vector2I> usedCells)
    {
        var visited = new HashSet<Vector2I>();
        var queue = new Queue<Vector2I>();
        var connectedTiles = new HashSet<Vector2I>();

        bool hasCore = false;
        bool hasConnector = false;

        foreach (Vector2I cell in usedCells)
        {
            string type = GetTileType(tilemap, cell);
            if (type == "core") hasCore = true;
            if (type == "connector")
            {
                hasConnector = true;
                connectedTiles.Add(cell);
                visited.Add(cell);

                foreach (Vector2I neighbor in GetSurroundingCells(cell, limits))
                {
                    if (visited.Add(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        if (!hasCore || !hasConnector) return false;

        while (queue.Count > 0)
        {
            Vector2I current = queue.Dequeue();
            string currentType = GetTileType(tilemap, current);

            if (Walls.Contains(currentType) || Floors.Contains(currentType))
            {
                connectedTiles.Add(current);
                foreach (Vector2I neighbor in GetSurroundingCells(current, limits))
                {
                    if (visited.Add(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        for (int x = limits.X; x <= limits.Y; x++)
        {
            for (int y = limits.Z; y <= limits.W; y++)
            {
                Vector2I point = new Vector2I(x, y);
                string pointType = GetTileType(tilemap, point);

                if (Walls.Contains(pointType) || Floors.Contains(pointType))
                {
                    if (!connectedTiles.Contains(point))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}