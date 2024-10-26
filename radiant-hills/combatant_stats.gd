extends Sprite2D


# Basic Stats
var health = 100
var attack_power = 10
var is_attacking = false

# Equipment Slots
@export var Ring1: Resource
@export var Ring2: Resource
@export var Chest: Resource
@export var Hat: Resource
@export var Staff: Resource
@export var Boots: Resource
@export var Spellbook: Resource

# Update Stats based on equipped items
func set_stats():
	# Base Stats
	health = 100
	attack_power = 10
	
	# Apply modifiers based off of equipment slots
	if Ring1: apply_item_modifiers(Ring1)
	if Ring2: apply_item_modifiers(Ring2)
	if Chest: apply_item_modifiers(Chest)
	if Hat: apply_item_modifiers(Hat)
	if Staff: apply_item_modifiers(Staff)
	if Boots: apply_item_modifiers(Boots)
	if Spellbook: apply_item_modifiers(Spellbook)

	print ("Updated player stats with equipment: Health - " + str(health) + ", Attack Power - " +str(attack_power))

# Applies individual item modifiers.
func apply_item_modifiers(item):
	if item.has("health_bonus"):
		health += item.health_bonus
	if item.has("attack_bonus"):
		attack_power += item.attack_bonus
