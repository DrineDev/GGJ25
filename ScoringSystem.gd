extends Node

var score = 1
var game_active = true 

func reset_score():
	score = 1
	game_active = true

func stop_scoring():
	game_active = false
