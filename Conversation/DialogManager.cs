using Godot;
using Godot.Collections;
using System;
using Array = Godot.Collections.Array;

[GlobalClass]
public partial class DialogManager : Node
{
    [Signal] public delegate void DialogFinishedEventHandler();

    private readonly PackedScene _textBoxScene = GD.Load<PackedScene>("res://Conversation/TextBox.tscn");

    private Node _parent;
    private Array _dialogLines = new Array();
    private int _currentLineIndex = 0;

    public Node2D TextBox { get; private set; }
    public Vector2 TextBoxPosition { get; set; }

    public bool IsDialogActive { get; private set; } = false;
    public bool CanAdvanceLine { get; private set; } = false;

    public override void _Ready()
    {
        _parent = GetParent();
    }

    /// <summary>
    /// Starts a dialog sequence.
    /// </summary>
    /// <param name="position">The offset position of the text box relative to this node.</param>
    /// <param name="lines">The list/array of dialog lines or task IDs.</param>
    public void StartDialog(Vector2 position, Array lines)
    {
        if (IsDialogActive)
        {
            GD.PushWarning("Warning: Dialog is already active");
            return;
        }

        _dialogLines = lines ?? new Array();
        _currentLineIndex = 0;
        TextBoxPosition = position;
        IsDialogActive = true;

        ShowTextBox();
    }

    private void ShowTextBox()
    {
        if (_dialogLines == null || _dialogLines.Count == 0)
        {
            GD.PushWarning("Warning: Attempted to show text box with dialog_lines empty");
            return;
        }

        Variant currentLine = _dialogLines[_currentLineIndex];

        // If line is an integer, it is a Quest/Task ID
        if (currentLine.VariantType == Variant.Type.Int)
        {
            int taskId = (int)currentLine;
            var questManager = (Node)GetNode("/root/QuestManager");
            var task = (GodotObject)questManager.Call("get_task", taskId);

            if (task != null)
            {
                // Instantiate new Quest instance: Quest.new(task.id, parent, -1)
                var questScript = GD.Load<GDScript>("res://Quests/Quest.gd"); // or C# Quest class
                if (questScript != null)
                {
                    questScript.New((int)task.Get("id"), _parent, -1);
                }
                Advance();
                return;
            }
            else
            {
                // Fixed: Captured missing task ID before resetting dialog lines
                GD.PushWarning($"Warning: Task {taskId} not found in QuestManager.tasks");
                _dialogLines = new Array { " . . . " };
                _currentLineIndex = 0;
                currentLine = _dialogLines[0];
            }
        }

        TextBox = _textBoxScene.Instantiate<Node2D>();
        TextBox.Connect("finished_displaying", Callable.From(OnTextBoxFinishedDisplaying));
        AddChild(TextBox);
        TextBox.Position = TextBoxPosition;

        TextBox.Call("display_text", currentLine.AsString());
        CanAdvanceLine = false;
    }

    private void OnTextBoxFinishedDisplaying()
    {
        CanAdvanceLine = true;
    }

    /// <summary>
    /// Advances the dialog to the next line.
    /// </summary>
    public void Advance()
    {
        if (IsDialogActive)
        {
            // Fixed: Safe check to prevent crash if TextBox wasn't instantiated or already freed
            if (GodotObject.IsInstanceValid(TextBox))
            {
                TextBox.QueueFree();
                TextBox = null;
            }

            _currentLineIndex++;

            if (_currentLineIndex >= _dialogLines.Count)
            {
                EmitSignal(SignalName.DialogFinished);
                IsDialogActive = false;
                _currentLineIndex = 0;
                return;
            }

            ShowTextBox();
        }
        else
        {
            GD.PushWarning("Warning: Attempted to advance dialog when dialog is not active");
        }
    }

    /// <summary>
    /// Aborts and ends the ongoing dialog immediately.
    /// </summary>
    public void EndDialog()
    {
        if (!IsDialogActive) return;

        if (GodotObject.IsInstanceValid(TextBox))
        {
            TextBox.QueueFree();
            TextBox = null;
        }

        EmitSignal(SignalName.DialogFinished);
        IsDialogActive = false;
        _currentLineIndex = 0;
    }
}