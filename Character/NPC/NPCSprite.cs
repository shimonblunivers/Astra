using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class NPCSprite : Node2D
{
    [Export] public AnimatedSprite2D SkinNode { get; set; }
    [Export] public AnimatedSprite2D EyesNode { get; set; }
    [Export] public AnimatedSprite2D HairNode { get; set; }
    [Export] public AnimatedSprite2D TorsoNode { get; set; }
    [Export] public AnimatedSprite2D LegsNode { get; set; }
    [Export] public AnimatedSprite2D BootsNode { get; set; }
    [Export] public Timer TimerNode { get; set; }

    public Color[] Skin { get; private set; } = System.Array.Empty<Color>();

    public override void _Ready()
    {
        // Fallback to GetNode if not bound via Inspector
        SkinNode ??= GetNode<AnimatedSprite2D>("Skin");
        EyesNode ??= GetNode<AnimatedSprite2D>("Eyes");
        HairNode ??= GetNode<AnimatedSprite2D>("Hair");
        TorsoNode ??= GetNode<AnimatedSprite2D>("Torso");
        LegsNode ??= GetNode<AnimatedSprite2D>("Legs");
        BootsNode ??= GetNode<AnimatedSprite2D>("Boots");
        TimerNode ??= GetNode<Timer>("Timer");

        var random = new RandomNumberGenerator();
        random.Randomize();

        HairNode.Frame = random.RandiRange(0, 6);
        HairNode.FlipH = random.RandiRange(0, 1) == 0;

        SetSkin();

        EyesNode.Stop();
        TimerNode.WaitTime = random.RandfRange(0f, 2f);
        TimerNode.Start();

        StartEyesAnimationAsync();
    }

    private async void StartEyesAnimationAsync()
    {
        await ToSignal(TimerNode, Timer.SignalName.Timeout);
        EyesNode.Play("default");
    }

    public void SetSkin(
        Color? a = null,
        Color? b = null,
        Color? c = null,
        Color? d = null,
        Color? e = null)
    {
        Color skinColor = a ?? RandomColor();
        Color eyesColor = b ?? RandomColor();
        Color hairColor = c ?? RandomColor();
        Color torsoColor = d ?? RandomColor();
        Color legsColor = e ?? RandomColor();

        SkinNode.Modulate = skinColor;
        EyesNode.Modulate = eyesColor;
        HairNode.Modulate = hairColor;
        TorsoNode.Modulate = torsoColor;
        LegsNode.Modulate = legsColor;
        BootsNode.Modulate = Colors.Black;

        Skin = new Color[] { skinColor, eyesColor, hairColor, torsoColor, legsColor };
    }

    private static Color RandomColor()
    {
        var random = new RandomNumberGenerator();
        // Fixed: randf() generates uniform values in [0.0, 1.0] unlike randfn() which yields negative numbers
        return new Color(
            random.Randf() * 0.75f,
            random.Randf() * 0.75f,
            random.Randf() * 0.75f,
            1.0f
        );
    }
}