# combatant_2.gd
extends Sprite2D

# Combatant stats
var EnemyName = "Unknown"
var health = 100
var attack_power = 10
var is_attacking = false

# Sets combatant's stats based on the provided enemy data resource
func set_stats(enemy_data: Resource):
	if enemy_data:
		EnemyName = enemy_data.name
		health = enemy_data.health
		attack_power = enemy_data.attack_power
		print(EnemyName + " has entered the battle with " + str(health) + " health and " + str(attack_power) + " attack power.")
	else:
		print("Failed to load enemy data resource.")
