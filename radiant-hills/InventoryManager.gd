extends Node
class_name InventoryManager

@export var player_character: Node
var inventory: Array = []
const INVENTORY_SIZE = 28  # Fixed number of slots

func _ready():
	# Initialize inventory with nulls
	inventory = Array()  # Create an empty array
	for i in range(INVENTORY_SIZE):
		inventory.append(null)  # Fill the array with nulls (empty slots)
	
	# Example: Load an item and add it to the first available slot
	var ring_of_power = load("res://TestRingofPower.tres") as EquipmentData
	if ring_of_power:
		add_item(ring_of_power)  # Add the item to the inventory
	else:
		print("Failed to load the resource.")

# Function to add an item to the inventory
func add_item(item: EquipmentData):
	# Find the first available slot
	for i in range(INVENTORY_SIZE):
		if inventory[i] == null:  # Check for empty slot
			inventory[i] = item  # Add item to the first empty slot
			update_inventory_display()  # Update the UI
			return  # Exit once the item is added
	print("No available slots in inventory.")

# Function to update the GridContainer with item buttons
func update_inventory_display():
	var grid_container = $GridContainer  # Ensure this path is correct

	# Clear existing buttons
	for child in grid_container.get_children():
		child.queue_free()  # Free each child node

	# Create buttons for each filled slot in the inventory
	for i in range(INVENTORY_SIZE):
		if inventory[i] != null:  # Check if slot is filled
			var button = Button.new()
			button.text = inventory[i].item_name  # Display item name
			# button.icon = inventory[i].icon  # Uncomment if using item icons
			button.pressed.connect(self._on_item_button_pressed.bind(inventory[i]))

			grid_container.add_child(button)  # Adds a button to the GridContainer

func _on_item_button_pressed(item: EquipmentData):
	print("Equipped item: " + item.item_name)
	# Call function to equip item
	if player_character:
		player_character.equip(item)  # Calls the equip function on the player character
	else:
		print("Player character is not assigned.")
