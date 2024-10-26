extends Resource
class_name RoomData

@export var enemies: Array[Resource] = []
@export var items: Array[Resource] = []
@export var enemies_remaining: int = 0 # Initialize with the total number of enemies in this room
