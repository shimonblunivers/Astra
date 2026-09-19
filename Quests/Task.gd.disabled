## Task that the player can accept from an NPC
class_name Task
extends Resource

@export var id : int
@export var title : String
@export_multiline var description : String

@export var reward : int
@export var goal : Goal

@export var world_limit : int = -1

## If true, the NPC won't give this mission randomly, only if the previous task is completed
@export var is_followup_task : bool = false
## If true, the mission will be finished completely only when you get back to the NPC 
@export var return_to_npc : bool = true
## Required roles for NPC to give this mission
@export var required_role : NPC.Roles
## Roles that will be added to the NPC, if he gives this mission
@export var add_role_on_accept : NPC.Roles

@export var difficulty_multiplier : int = 1

var times_activated : int = 0
