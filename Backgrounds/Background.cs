using Godot;

[GlobalClass]
public partial class Background : Node2D
{
    public override void _Process(double delta)
    {
        if (World.Instance == null || Player.MainPlayer?.Camera == null) return;

        // Fix: GetDistanceFromCenter returns a Vector2 offset, not a float
        Vector2 distance = World.Instance.GetDistanceFromCenter(GlobalPosition);
        float zoom = Mathf.Max(0.001f, Player.MainPlayer.Camera.Zoom.X);

        Vector2 halfExtents = new Vector2(2048f / zoom, 2048f / zoom) / 2f;
        Vector2 rectSize = new Vector2(2048f + 2048f / zoom, 2048f + 2048f / zoom);
        Rect2 region = new Rect2(-halfExtents, rectSize);

        foreach (Node child in GetChildren())
        {
            if (child is Sprite2D sprite)
            {
                if (sprite.Material is ShaderMaterial shaderMat)
                {
                    shaderMat.SetShaderParameter("background_position", distance);
                }

                sprite.RegionRect = region;
            }
        }

        GlobalRotation = 0.0f;
    }
}