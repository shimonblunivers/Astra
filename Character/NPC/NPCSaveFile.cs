using Godot;
using Godot.Collections;

[GlobalClass]
public partial class NPCSaveFile : Resource
{
	[Export] public string Nickname { get; set; } = string.Empty;
	[Export] public Vector2 Position { get; set; } = Vector2.Zero;
	[Export] public int Id { get; set; }
	[Export] public Array Roles { get; set; } = new Array();
	[Export] public Array Skin { get; set; } = new Array();
	[Export] public Array Hair { get; set; } = new Array();
	[Export] public int ShipId { get; set; }

	public static Array<NPCSaveFile> Save()
	{
		var files = new Array<NPCSaveFile>();

		foreach (var npc in NPC.Npcs)
		{
			var file = new NPCSaveFile();
			file.Nickname = npc.Nickname;
			file.Position = npc.Position;
			file.Id = npc.Id;

			if (npc.RoleList == null || npc.RoleList.Count == 0)
			{
				file.Roles = new Array { (int)NPC.Roles.Civilian };
			}
			else
			{
				var rolesArr = new Array();
				foreach (var role in npc.RoleList)
				{
					rolesArr.Add((int)role);
				}
				file.Roles = rolesArr;
			}

			// Skin array from sprites
			if (npc.Skin.VariantType == Variant.Type.Array)
			{
				file.Skin = npc.Skin.AsGodotArray();
			}

			// Hair [frame, flip_h] from hair node
			var hairNode = npc.GetNode("Sprite").GetNode<Sprite2D>("Hair");
			file.Hair = new Array { hairNode.Frame, hairNode.FlipH };

			if (npc.Ship != null)
			{
				file.ShipId = npc.Ship.Id;
			}

			files.Add(file);
		}

		return files;
	}

	public void Load()
	{
	}
}