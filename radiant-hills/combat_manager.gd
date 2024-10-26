extends Node2D

# Defines combatants
var combatant_1
var combatant_2

# Defines labels
var combatant1_health_label
var combatant2_health_label

# Establishes attack delay base of 0
var time_since_last_attack = 0
var current_attacker
var current_defender

# Used to start/end combat.
var battle_active = false

# Loads your room data, containing a list of enemy types.
var room_data = preload("res://RoomData_1.tres")  # Adjust based on room desired

# Message tracking
var message_printed = {
	"winner": false,
	"enemies_remaining": false
}

# Called when the node enters the scene tree for the first time.
func _ready():
	randomize()

	# Initialize combatants and labels
	combatant_1 = $Combatant1
	combatant_2 = $Combatant2
	combatant1_health_label = $Combatant1/Combatant1HealthLabel
	combatant2_health_label = $Combatant2/Combatant2HealthLabel

	# Initialize enemies_remaining based on room data
	room_data.enemies_remaining = room_data.enemies.size()
	start_new_battle()

# Selects a random enemy type for combatant_2 from room data
func start_new_battle():
	if room_data.enemies_remaining > 0:
		var enemy_resource = select_random_enemy()
		if enemy_resource:
			combatant_2.set_stats(enemy_resource)  # Sets combatant_2's stats
			room_data.enemies_remaining -= 1
			current_attacker = combatant_1
			current_defender = combatant_2

			# Reset message states
			message_printed["winner"] = false
			message_printed["enemies_remaining"] = false

			# Initial HP display
			update_health_display()
			battle_active = true
			time_since_last_attack = 0

			# Start the attack timer for the player's first attack
			$AttackTimer.start()  # Start the timer to wait before the first attack
		else:
			print("No enemy data found.")
	else:
		print("No more enemies in this room!")

func _on_AttackTimer_timeout():
	attack(current_attacker, current_defender)  # Player attacks first after the timer

func select_random_enemy():
	if room_data.enemies.size() > 0:
		var enemy_data = room_data.enemies[randi() % room_data.enemies.size()]
		return enemy_data
	else:
		print("No enemy data loaded.")
		return null

func _process(delta):
	if battle_active:
		time_since_last_attack += delta  # Increment time since last attack
		if time_since_last_attack >= 2:  # Checks for if 2 seconds has passed
			attack(current_attacker, current_defender)
			time_since_last_attack = 0  # Resets attack timer

			# Swap Attackers
			var temp = current_attacker
			current_attacker = current_defender
			current_defender = temp

			# Check for opponent health
			if current_defender.health <= 0:
				end_battle(current_attacker)

func attack(attacker, defender):
	# Generate random attack power within a range (20% of attack power)
	var min_attack = attacker.attack_power * 0.8  # 80% attack power minimum
	var max_attack = attacker.attack_power * 1.2  # 120% attack power maximum
	var random_attack_power = lerp(min_attack, max_attack, randf())
	
	# Round up to the nearest whole number
	random_attack_power = ceil(random_attack_power)
	
	# Apply Damage
	defender.health -= random_attack_power
	print(defender.name + " took " + str(random_attack_power) + " damage and now has " + str(defender.health) + " health.")
	
	# After Damage is applied, this updates the health labels in the Scene
	update_health_display()
	
	# Checks if the defender's health is zero or less
	if defender.health <= 0:
		end_battle(attacker)  # The attacker is the winner if the defender's health drops to zero

func update_health_display():
	# Update the text of each label to show current health
	combatant1_health_label.text = "Health: " + str(combatant_1.health)
	combatant2_health_label.text = "Health: " + str(combatant_2.health)

func end_battle(winner):
	if not message_printed["winner"]:
		print("Battle over! The winner is: " + winner.name)
		message_printed["winner"] = true
	
	battle_active = false  # Stop the battle loop

	# Stop the attack timer
	$AttackTimer.stop()

	if room_data.enemies_remaining > 0:
		if not message_printed["enemies_remaining"]:
			print("Enemies remaining: " + str(room_data.enemies_remaining))
			message_printed["enemies_remaining"] = true
		
		await(get_tree().create_timer(3.0).timeout)
		start_new_battle()  # Start a new battle after a delay
