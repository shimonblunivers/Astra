class_name Dialogs

static var conversations = {
	"greeting": [
		"Hey!",
		"Hello there!",
		"Yo!",
		"Ahoy!",
		"Hiya!",
		"Greetings!",
		"Howdy do!",
		"Hello there.",
		"Hi!",
		"Have a nice day.",
		"How are you?",
		"How's it going?",
		"Welcome back!",
		"Hey there, astronaut!",
		"Saluton, spaceman!",
		"Enjoy your stay!",
		"What do you need?",
		"Can I help you with something?",
		"Seen anything suspicious?",
		"Seen anything unusual?",
		"What do you think about space politics?",
		"What's the best part of space in your opinion?",
	],

	"mission": { # Mission categories
		0: [ # Mission index and its content
			"Hey, I need something brought back..",
			"I dropped a chip somewhere that had my powidl tea recipe on it..",
			"Could you bring it to me?",
			0, # Mission index to start
			"Thank you! I can't wait to make my tea again.",
		],
		1: [
			"Hey!",
			"You look like a traveler.. I need something..",
			"I lost a chip that had family photos from several generations.",
			"Careless, right? .. Anyway, could you bring it back? There's a reward in it for you!",
			1,
			"Thank you!",
		],
		2: [
			"Hey there!",
			"I need your help. Our station picked up a signal from an unknown source..",
			"Could you find out who or what is sending it?",
			"If you meet anyone nearby, check them out - alien, explorer, or even a lost satellite.",
			"Once you find the source, come back. Of course, I won't forget to reward you!",
			2,
			"I'll be waiting for your return!",
		],
		3: [
			"Yo, I need help with something..",
			"I can see you've got the courage to help a crew in distress.",
			"The situation's bad. Engines are down and vital systems are heavily damaged.",
			"I need the tool KL94 of size 10. Spare parts might be on our sister ship, but beware - size 10 tends to go missing.",
			"You'll need steady hands. Come back once you have it!",
			3,
		],

		4: [
			"Yo.",
			"I'm in deep trouble, someone stole my encrypted disk with ultra-classified data.",
			"That disk belongs to a wealthy officer who tasked me with finding it.",
			"Will you help me?",
			"Find my brother. He'll tell you how to proceed, but no one else can know.",
			"If the wrong people find out, we're done for.",
			"And the reward? We've got plenty of energy units to pay you well.",
			"So, what do you say?",
			4,
			"Thanks a lot!",
		],
		5: [
			"Hey.",
			"I lost my vape.",
			"I won't last many more days without it.",
			"Could you be so kind and bring it back?",
			5,
			"Thanks.",
		],
		6: [
			"Hey savior! It's me!",
			"I'll never forget what you did for me..",
			"I lost something again..",
			"This time it's a pack of candy..",
			"Would you save me again..?",
			6,
			"Thanks!",
		],
		7: [
			"Hey, I need your help!",
			"Our navigation systems were disrupted by a magnetic storm and we can't determine our position.",
			"Could you find a spare compass or some device that can help us navigate?",
			"It's urgent. Without it, we're blind in space.",
			7,
			"Come back with it ASAP, there's a reward waiting!",
		],
		# id : [
		#	"text",
		# ],
	},

	"mission_finished": {
		-1: [ # Default response
			"Thank you!",
		],
		4: [ # Mission index this responds to
			"Hey. You must be the one who's supposed to help us find the disk.",
			"My brother told me about you. Hope you're not scared - it won't be easy.",
			4001, # Follow-up mission index
		],
		4001: [
			"Thank you so much!",
			"You have no idea what this means to us.",
			"We're forever in your debt.",
		],
		5: [
			"You're my savior!",
			"Thanks!",
			"You can.. go now..",
			". . . . .",
			"You're still here?",
			"Well.. I lost something else..",
			"Could you bring me the gum I left on the ship next door?",
			5001,
			"Thanks again!",
		],
		5001: [
			"You actually found them!",
			"Thanks again! You're my double savior.",
		],
		6: [
			"Awesome!",
			"I knew I could count on you!",
		]
	},
}

## [param dialog_type]: key from [member conversations]. [br]
## [return]: random phrase from the given dialog type. [br]
static func random_phrase(dialog_type: String) -> String:
	if (!conversations.has(dialog_type)): return ""
	var random := RandomNumberGenerator.new()
	return conversations[dialog_type][random.randi_range(0, conversations[dialog_type].size() - 1)]


## [param roles]: array of [member NPC.Roles] that the task should have. [br]
## [param can_return_empty_task]: whether the function has chance to return -1. [br]
## [return]: random task ID that is not blocked in [member QuestManager.active_quests]. [br]
## Returns -2 if no task is available. [br]
static func random_task_id(roles := [], can_return_empty_task := false) -> int:
	var random := RandomNumberGenerator.new()
	if can_return_empty_task && random.randi_range(0, 3) == 0: return -1
	
	var usable_tasks = []
	for task_key in QuestManager.tasks.keys():
		var task = QuestManager.tasks[task_key]
		if !task.is_followup_task && (task.required_role in roles || task.required_role == NPC.Roles.NONE):
			if (task.times_activated < task.world_limit || task.world_limit < 0):
				if !(task.id in QuestManager.active_quests):
					usable_tasks.append(task)
	
	if usable_tasks.size() == 0:
		if (!can_return_empty_task): print_debug("Warning: No tasks available for roles: " + str(roles))
		return -2

	return usable_tasks.pick_random().id