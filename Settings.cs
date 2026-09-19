using Godot;

[GlobalClass]
public partial class Settings : CanvasLayer
{
    [Export] public Slider SoundSlider { get; set; }
    [Export] public Slider SoundtrackSlider { get; set; }
    [Export] public Button BackButton { get; set; }
    [Export] public AudioStreamPlayer SfxTestSound { get; set; }

    private int _sfxBusIndex = -1;
    private int _musicBusIndex = -1;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        SoundSlider ??= GetNodeOrNull<Slider>("MarginContainer/VBoxContainer/SoundSlider");
        SoundtrackSlider ??= GetNodeOrNull<Slider>("MarginContainer/VBoxContainer/SoundtrackSlider");
        BackButton ??= GetNodeOrNull<Button>("MarginContainer/VBoxContainer/Back")
                      ?? GetNodeOrNull<Button>("MarginContainer/VBoxContainer/BackButton")
                      ?? GetNodeOrNull<Button>("Back");
        SfxTestSound ??= GetNodeOrNull<AudioStreamPlayer>("SFXTestSound");

        _sfxBusIndex = AudioServer.GetBusIndex("SFX");
        _musicBusIndex = AudioServer.GetBusIndex("Music");

        if (BackButton != null)
        {
            BackButton.Pressed += OnBackPressed;
        }

        if (SoundSlider != null && _sfxBusIndex >= 0)
        {
            float sfxDb = AudioServer.GetBusVolumeDb(_sfxBusIndex);
            SoundSlider.Value = (sfxDb + 30f) / 36f * 100f;
            SoundSlider.DragEnded += OnSoundSliderDragEnded;
        }

        if (SoundtrackSlider != null && _musicBusIndex >= 0)
        {
            float musicDb = AudioServer.GetBusVolumeDb(_musicBusIndex);
            SoundtrackSlider.Value = (musicDb + 30f) / 36f * 100f;
            SoundtrackSlider.ValueChanged += OnSoundtrackSliderValueChanged;
        }
    }

    public void OnSoundSliderDragEnded(bool valueChanged)
    {
        if (SoundSlider == null || _sfxBusIndex < 0) return;

        float targetDb = -30f + (36f * (float)SoundSlider.Value / 100f);
        AudioServer.SetBusVolumeDb(_sfxBusIndex, targetDb);

        bool shouldMute = targetDb <= -29f;
        AudioServer.SetBusMute(_sfxBusIndex, shouldMute);

        if (SfxTestSound != null && !shouldMute)
        {
            SfxTestSound.Play();
        }
    }

    public void OnSoundtrackSliderValueChanged(double value)
    {
        if (_musicBusIndex < 0) return;

        float targetDb = -30f + (36f * (float)value / 100f);
        AudioServer.SetBusVolumeDb(_musicBusIndex, targetDb);

        AudioServer.SetBusMute(_musicBusIndex, targetDb <= -29f);
    }

    public void OnBackPressed()
    {
        if (GodotObject.IsInstanceValid(Menu.Instance))
        {
            Menu.Instance.Visible = true;
        }

        QueueFree();
    }
}