extends Sprite2D

var EnemyName
var health
var attack_power
var is_attacking = false

func set_stats(enemy_data: Resource):
	EnemyName = enemy_data.name
	health = enemy_data.health
	attack_power = enemy_data.attack_power
