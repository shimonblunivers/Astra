using Godot;

public partial class Background : Node2D
{
    public override void _Process(double delta)
    {
        float distance = World.Instance.GetDistanceFromCenter(GlobalPosition);
        float zoom = Player.MainPlayer.Camera.Zoom.X;

        foreach (Node child in GetChildren())
        {
            Sprite2D sprite = (Sprite2D)child;
            sprite.Material.Set("shader_parameter/background_position", distance);
            sprite.RegionRect = new Rect2(
                -new Vector2(2048f / zoom, 2048f / zoom) / 2f,
                new Vector2(2048f + 2048f / zoom, 2048f + 2048f / zoom)
            );
        }

        GlobalRotation = 0.0f;
    }
}
