using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Wall : ShipPart
{
    private dynamic N(string path) => GetNode(path);



    public Sprite2D sprite;
    // onready: sprite = GetNode<Sprite2D>("Sprite2D");
    public LightOccluder2D light_occluder;
    // onready: light_occluder = GetNode<LightOccluder2D>("LightOccluder2D");
    public ProgressBar hp;
    // onready: hp = GetNode<ProgressBar>("HP");
    public AnimatedSprite2D cracks;
    // onready: cracks = GetNode<AnimatedSprite2D>("Cracks");
    public Button button;
    // onready: button = GetNode<Button>("Button");

    public static dynamic debris_scene = GD.Load<PackedScene>("res://Ship/Walls/Debris/Debris.tscn");

    public int layer = 0;


    public void init(dynamic _ship, Vector2I _coords, float _durability = 100, float _mass = 4)
    {
    super(_ship, _coords, _durability, _mass);

    }

    public override void _Ready()
    {
    hp.MaxValue = durability_max;
    hp.Value = durability_max;

    }

    public void set_texture(dynamic texture)
    {
    sprite.Texture = texture;

    }

    public void _on_button_pressed()
    {
    if (Options.DEVELOPMENT_MODE)
    {
        }
    damage(25);

    }

    public void damage(float dmg)
    {

    durability_current -= dmg;

    button.TooltipText = str(Mathf.Snapped(durability_current / durability_max, 0.01)) + "%";

    switch (durability_current / durability_max)
    {
        }
    case 1:
        }
    cracks.Frame = 0;
    case 0.75:
        }
    cracks.Frame = 1;
    case 0.5:
        }
    cracks.Frame = 2;
    case 0.25:
        }
    cracks.Frame = 3;

    hp.Value = durability_current;
    if ((durability_current <= 0))
    {
        }
    destroy();


    }

    public void destroy()
    {
    _debris_object := debris_scene.Instantiate();

    ship.destroyed_walls.append(tilemap_coords);

    _debris_object.init(ship, tilemap_coords);
    _debris_object.Position = position;

    get_parent().add_child.CallDeferred(_debris_object);
    dynamic _pos = ship.wall_tile_map.local_to_map(position);
    ship.wall_tile_map.set_cells_terrain_connect([_pos] , 0, -1, false);

    remove();
    }

}