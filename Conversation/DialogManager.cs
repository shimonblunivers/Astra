using Godot;
using Godot.Collections;

/// <summary>
/// Global singleton that manages all dialogs in the game.
/// </summary>
public partial class DialogManager : Node
{
    private readonly PackedScene _textBoxScene = GD.Load<PackedScene>("res://Conversation/TextBox.tscn");

    private Array _dialogLines = new Array();
    private int _currentLineIndex = 0;

    private Node _textBox;
    private Vector2 _textBoxPosition;

    private bool _isDialogActive = false;
    private bool _canAdvanceLine = false;

    [Signal]
    public delegate void DialogFinishedEventHandler();

    public override void _Ready()
    {
        // parent is accessed at runtime via GetParent()
    }

    /// <summary>
    /// <param name="position">The offset of the text box from the parent node.</param>
    /// <param name="lines">The lines of the dialog.</param>
    /// </summary>
    public void StartDialog(Vector2 position, Array lines)
    {
        if (_isDialogActive)
        {
            GD.PrintErr("Warning: Dialog is already active");
            return;
        }

        _dialogLines = lines;
        _textBoxPosition = position;
        ShowTextBox();

        _isDialogActive = true;
    }

    private void ShowTextBox()
    {
        if (_dialogLines.Count == 0)
        {
            GD.PrintErr("Warning: Attempted to show text box with dialog_lines empty");
            return;
        }

        if (_dialogLines[_currentLineIndex].VariantType == Variant.Type.Int)
        {
            var task = QuestManager.GetTask(_dialogLines[_currentLineIndex].AsInt32());
            if (task != null)
            {
                Quest quest = new Quest();
                // Equivalent to: Quest.new(task.id, parent, -1)
                // Adjust according to your Quest constructor/init method
                quest.Init(task.Id, GetParent(), -1);
                Advance();
                return;
            }
            else
            {
                GD.PrintErr("Warning: Task " + _dialogLines[_currentLineIndex].ToString() + " not found in QuestManager.tasks");
                _dialogLines = new Array { " . . . " };
                _currentLineIndex = 0;
            }
        }

        _textBox = _textBoxScene.Instantiate();
        (_textBox as TextBox).FinishedDisplaying += OnTextBoxFinishedDisplaying;
        AddChild(_textBox);
        (_textBox as Node2D).Position = _textBoxPosition;

        (_textBox as TextBox).DisplayText(_dialogLines[_currentLineIndex].AsString());

        _canAdvanceLine = false;
    }

    private void OnTextBoxFinishedDisplaying()
    {
        _canAdvanceLine = true;
    }

    /// <summary>
    /// Advances the dialog to the next line.
    /// </summary>
    public void Advance()
    {
        if (_isDialogActive)
        {
            _textBox?.QueueFree();

            _currentLineIndex++;

            if (_currentLineIndex >= _dialogLines.Count)
            {
                EmitSignal(SignalName.DialogFinished);
                _isDialogActive = false;
                _currentLineIndex = 0;
                return;
            }

            ShowTextBox();
        }
        else
        {
            GD.PrintErr("Warning: Attempted to advance dialog when dialog is not active");
        }
    }

    public void EndDialog()
    {
        if (!_isDialogActive) return;

        _textBox?.QueueFree();
        EmitSignal(SignalName.DialogFinished);
        _isDialogActive = false;
        _currentLineIndex = 0;
    }
}
