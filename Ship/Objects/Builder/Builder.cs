using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class Builder : ShipPart
{
    [Export] public AudioStreamPlayer2D Sound { get; set; }
    [Export] public Marker2D SpawnPoint { get; set; }
    [Export] public Connector ConnectorInstance { get; set; }
    [Export] public Area2D InteractArea { get; set; }

    public List<Connector> ConnectorsInRange { get; } = new List<Connector>();

    public override void _Ready()
    {
        base._Ready();

        Sound ??= GetNodeOrNull<AudioStreamPlayer2D>("Sound");
        SpawnPoint ??= GetNodeOrNull<Marker2D>("SpawnPoint");
        ConnectorInstance ??= GetNodeOrNull<Connector>("Connector");
        InteractArea ??= GetNodeOrNull<Area2D>("Area2D");
    }

    public void PlaySound()
    {
        Sound?.Play();
    }

    public Vector2 GetSpawnPosition()
    {
        return SpawnPoint != null ? SpawnPoint.GlobalPosition : GlobalPosition;
    }

    public float GetShipRotation()
    {
        return (float)GlobalRotation;
    }

    public void OnAreaEntered(Area2D area)
    {
        Connector connector = ((Node)area) as Connector ?? area?.GetParent() as Connector;
        if (connector != null && !ConnectorsInRange.Contains(connector))
        {
            ConnectorsInRange.Add(connector);
        }
    }

    public void OnAreaExited(Area2D area)
    {
        Connector connector = ((Node)area) as Connector ?? area?.GetParent() as Connector;
        if (connector != null)
        {
            ConnectorsInRange.Remove(connector);
        }
    }
}