using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Thruster : ShipPart
{
    private dynamic N(string path) => GetNode(path);



    public Sprite2D sprite;
    // onready: sprite = GetNode<Sprite2D>("Sprite2D");
    public GPUParticles2D jet_particles;
    // onready: jet_particles = GetNode<GPUParticles2D>("JetParticles");
    public GPUParticles2D left_sidejet_particles;
    // onready: left_sidejet_particles = GetNode<GPUParticles2D>("LeftSideJetParticles");
    public GPUParticles2D right_sidejet_particles;
    // onready: right_sidejet_particles = GetNode<GPUParticles2D>("RightSideJetParticles");

    public AudioStreamPlayer2D jet_sound;
    // onready: jet_sound = GetNode<AudioStreamPlayer2D>("Sounds/Jet");
    public AudioStreamPlayer2D sidejet_sound;
    // onready: sidejet_sound = GetNode<AudioStreamPlayer2D>("Sounds/SideJet");

    public int layer = 0;

    public float power;

    public int direction;

    public bool running = false;

    public blocked := false;

    public Array blocked_sides = [false, false] # LEFT RIGHT;

    // # Required empty space in tiles to function
    public int _required_space = 4;



    public void init(dynamic _ship, Vector2I _coords, float _durability = 150, float _mass = 5, int _direction = 0, int _power = 1000)
    {
    super(_ship, _coords, _durability, _mass);
    direction = _direction;
    power = _power;

    ship.thrusters[direction].append(this);
    CallDeferred("get_blocked_sides");

    }

    public void set_status(bool status)
    {
    running = status;
    jet_particles.emitting = running;
    jet_sound.Playing = status;

    }

    public void side_thrusters(float dir)
    {
    bool emitting = false;
    if (!blocked_sides[0])
    {
        }
    left_sidejet_particles.emitting = dir < 0;
    if (dir < 0)
    {
        }
    emitting = true;
    if (!blocked_sides[1])
    {
        }
    right_sidejet_particles.emitting = dir > 0;
    if (dir > 0)
    {
        }
    emitting = true;
    if (sidejet_sound.Playing != emitting)
    {
        }
    sidejet_sound.Playing = emitting;

    }

    public void get_blocked_sides()
    {
    Array _blocked_sides = [false, false, false, false] # LEFT UP RIGHT DOWN;

    _blocked_sides[1] = ship.get_tile(tilemap_coords + Vector2I(-1, 0)) != null;
    _blocked_sides[2] = ship.get_tile(tilemap_coords + Vector2I(0, -1)) != null;
    _blocked_sides[3] = ship.get_tile(tilemap_coords + Vector2I(1, 0)) != null;
    _blocked_sides[0] = ship.get_tile(tilemap_coords + Vector2I(0, 1)) != null;

    if (direction == 0)
    {
        }
    blocked_sides = [_blocked_sides[0], _blocked_sides[2]];
    }
    else if (direction == 1)
    {
        }
    blocked_sides = [_blocked_sides[1], _blocked_sides[3]];
    }
    else if (direction == 2)
    {
        }
    blocked_sides = [_blocked_sides[2], _blocked_sides[0]];
    }
    else if (direction == 3)
    {
        }
    blocked_sides = [_blocked_sides[3], _blocked_sides[1]];

    Array blocked_cells = new Array();

    if (direction == 0)
    {
        }
    for (int x = 1; x < 1 + _required_space; x++)
    {
        }
    blocked_cells.append(Vector2I(x, 0));
    }
    else if (direction == 1)
    {
        }
    for (int y = 1; y < 1 + _required_space; y++)
    {
        }
    blocked_cells.append(Vector2I(0, y));
    }
    else if (direction == 2)
    {
        }
    for (int x = -_required_space; x < 0; x++)
    {
        }
    blocked_cells.append(Vector2I(x, 0));
    }
    else if (direction == 3)
    {
        }
    for (int y = -_required_space; y < 0; y++)
    {
        }
    blocked_cells.append(Vector2I(0, y));

    foreach (var cell in blocked_cells)
    {
        }
    if (ship.get_tile(tilemap_coords + cell) != null)
    {
        }
    blocked = true;

    if (!blocked)
    {
        }
    ship.thrust_power[direction] += power;

    set_status(false);


    }

}