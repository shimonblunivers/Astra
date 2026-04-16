using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class ButtonIndicator : Node2D
{
    private dynamic N(string path) => GetNode(path);



    public InteractableShipPart interactable_ship_part;


    public void init(InteractableShipPart _interactable_ship_part)
    {
    interactable_ship_part = _interactable_ship_part;
    N("Button").Text = InputMap.action_get_events("game_use")[0].as_text();
    }

}