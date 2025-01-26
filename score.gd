extends Label

func _process(delta):
	if ScoringSystem.game_active:  # Use the modified property name
		self.text = str(ScoringSystem.score)

func IncrementScore():
	if ScoringSystem.game_active:  # Use the modified property name
		ScoringSystem.score += 1	

func _on_timer_timeout() -> void:
	IncrementScore()
