using Godot;

public partial class Enemy : CharacterBody2D
{
	// Constants for enemy behavior
	public const float Speed = 100.0f; // Speed when chasing the player
	public const float WanderSpeed = 50.0f; // Speed when wandering
	public const float WaitTime = 3.0f; // Time to wait before leaving

	// Property to track if the enemy has entered the border
	public bool HasEnteredBorder { get; set; } = false;

	// Private fields
	private Player _player; // Reference to the player
	private Vector2 _wanderDirection; // Direction for wandering
	private float _waitTimer = 0; // Timer for waiting

	public override void _Ready()
	{
		// Find the player node in the scene
		_player = GetNode<Player>("/root/Game/Player"); // Adjust the path to your player node

		if (_player == null)
		{
			GD.PrintErr("Player node not found!");
		}
		else
		{
			GD.Print("Player node found successfully.");
		}

		// Start wandering
		SetWanderDirection();

		// Connect the body_entered signal for the Killzone
		var killzone = GetNode<Area2D>("Killzone");
		if (killzone != null)
		{
			killzone.BodyEntered += OnBodyEntered;
		}
		else
		{
			GD.PrintErr("Killzone not found!");
		}
	}

	public override void _Process(double delta)
	{
		if (_player == null) return;

		if (_player.IsInStealthMode)
		{
			// Player is in stealth mode, wander or wait
			if (_waitTimer > 0)
			{
				_waitTimer -= (float)delta; // Decrease the wait timer
				GD.Print("Enemy is waiting...");
			}
			else
			{
				Wander((float)delta); // Wander around
			}
		}
		else
		{
			// Player is visible, chase the player
			ChasePlayer((float)delta);
		}
	}

	private void ChasePlayer(float delta)
	{
		// Calculate the direction to the player
		Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();

		// Move the enemy toward the player
		Velocity = direction * Speed;
		MoveAndSlide();
	}

	private void Wander(float delta)
	{
		// Move in the wander direction
		Velocity = _wanderDirection * WanderSpeed;
		MoveAndSlide();

		// Change direction randomly
		if (GD.Randf() < 0.01f) // 1% chance to change direction each frame
		{
			SetWanderDirection();
		}
	}

	private void SetWanderDirection()
	{
		// Set a random wander direction
		_wanderDirection = new Vector2(GD.Randf() - 0.5f, GD.Randf() - 0.5f).Normalized();
	}

	public void OnPlayerHidden()
	{
		// Called when the player enters stealth mode
		_waitTimer = WaitTime; // Start the wait timer
		GD.Print("Enemy lost track of the player!");
	}

	private void OnBodyEntered(Node body)
	{
		GD.Print("Body entered the enemy's hitbox!"); // Debug print

		// Check if the body that entered the hitbox is the player
		if (body is Player player)
		{
			GD.Print("Enemy hit the player!");
			player.TakeDamage(1); // Reduce the player's HP by 1
			QueueFree(); // Despawn the enemy
		}
	}

	public void OnBorderEntered()
	{
		GD.Print("Enemy entered the border!");

		// Check if the enemy has entered the border before
		if (HasEnteredBorder)
		{
			// Despawn the enemy if it has entered the border before
			GD.Print("Enemy has entered the border before. Despawning...");
			QueueFree();
		}
		else
		{
			// Mark the enemy as having entered the border
			HasEnteredBorder = true;
			GD.Print("Enemy entered the border for the first time.");
		}
	}
}
