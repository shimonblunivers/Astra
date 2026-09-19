using Godot;

[GlobalClass]
public partial class EditorCamera : Camera2D
{
    public const float SPEED = 400.0f;
    public const float RUN_SPEED_MODIFIER = 800.0f;

    public const float ZOOM_SPEED = 0.1f;
    public const float MAX_ZOOM = 10.0f;
    public const float MIN_ZOOM = 0.2f;

    public const float LEFT_LIMIT = -350.0f;

    [Export] public bool Locked { get; set; } = false;

    public override void _PhysicsProcess(double delta)
    {
        if (Locked) return;

        float dt = (float)delta;
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        float running = Input.GetActionStrength("game_run");

        if (direction != Vector2.Zero)
        {
            Vector2 velocity = new Vector2(
                direction.X * (SPEED + RUN_SPEED_MODIFIER * running) * (1.0f / Zoom.X),
                direction.Y * (SPEED + RUN_SPEED_MODIFIER * running) * (1.0f / Zoom.Y)
            );

            var options = (Node)GetNode("/root/Options");
            bool devMode = options != null && (bool)options.Get("DEVELOPMENT_MODE");

            // Fixed: Check velocity.X < 0 so camera can still move right when at or beyond the left boundary
            if (!devMode && velocity.X < 0 && (Position.X + velocity.X * dt) < LEFT_LIMIT)
            {
                velocity.X = 0;
                Position = new Vector2(Mathf.Max(Position.X, LEFT_LIMIT), Position.Y);
            }

            Position += new Vector2(velocity.X * dt, velocity.Y * dt);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Locked) return;

        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            float zoomModifier = 0f;

            if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
            {
                zoomModifier = Zoom.X > 1.0f ? -1.0f : -0.25f;
            }
            else if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
            {
                zoomModifier = 1.0f;
            }

            if (zoomModifier == 0f) return;

            float difference = zoomModifier * ZOOM_SPEED;
            float newValue = Mathf.Clamp(difference + Zoom.X, MIN_ZOOM, MAX_ZOOM);
            Zoom = new Vector2(newValue, newValue);
        }
    }
}