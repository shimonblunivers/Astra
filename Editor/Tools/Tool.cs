using Godot;

[GlobalClass]
public partial class Tool : Resource
{
    [Export] public string ToolName { get; set; } = string.Empty;
    [Export] public string Nickname { get; set; } = string.Empty;
    [Export] public Texture2D Texture { get; set; }

    [Export] public int Price { get; set; } = 100;

    [Export] public bool Rotatable { get; set; } = false;

    [Export] public bool Object { get; set; } = false;
    [Export] public string SpawnTileOnRemove { get; set; } = string.Empty;

    [Export] public int WorldLimit { get; set; } = -1;
    [Export] public Vector2I PlaceableOnAtlasChoords { get; set; } = new Vector2I(-1, -1);
    [Export] public int TerrainId { get; set; } = -1;
    [Export] public Vector2I AtlasCoords { get; set; } = new Vector2I(-1, -1);

    [Export] public bool Debug { get; set; } = false;

    public int NumberOfInstances { get; set; } = 0;

    public void Create()
    {
        if (string.IsNullOrEmpty(ToolName))
        {
            GD.PushWarning("Warning: Attempted to register Tool with an empty or null name.");
            return;
        }

        var shipEditor = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ShipEditor");
        if (shipEditor != null)
        {
            var tools = shipEditor.Get("tools").AsGodotDictionary();
            tools[ToolName] = this;
        }
    }
}