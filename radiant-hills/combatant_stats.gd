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

func equip(item:Resource):
	# Check which slot to equip the item based on it's slot type
	match item.slot_type:
		"ring":
			# Equip in Ring1 if empty, otherwise equip in Ring2
			if Ring1 == null:
				Ring1 = item
			elif Ring2 == null:
				Ring2 = item
			else:
				print("Both ring slots are occupied.")
		"chest":
			if Chest == null:
				Chest = item
			else:
				print("Chest slot is already occupied.")
		"hat":
			if Hat == null:
				Hat = item
			else:
				print("Hat slot is already occupied.")
		"staff":
			if Staff == null:
				Staff = item
			else:
				print("Staff slot is already occupied.")
		"boots":
			if Boots == null:
				Boots = item
			else:
				print("Boots slot is already occupied.")
		"spellbook":
			if Spellbook == null:
				Spellbook = item
			else:
				print("Spellbook slot is already occupied.")
	# Update stats after equipping the item
	set_stats()
