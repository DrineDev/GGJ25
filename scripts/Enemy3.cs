using Godot;

public partial class Enemy3 : BaseEnemy
{
    private bool _isMovingRight; // Tracks if the enemy is moving to the right
    private bool _hasEnteredScreen = false; // Tracks if the enemy has entered the screen

    [Export] private float _spawnYOffset = 100f; // Adjust this to control the Y position of the spawn

    public override void _Ready()
    {
        base._Ready(); // Call the base class's _Ready method

        // Determine the initial direction based on spawn position
        _isMovingRight = GlobalPosition.X < GetViewportRect().Size.X / 2;

        // Adjust spawn position to ensure it's within the screen bounds horizontally
        Rect2 viewportRect = GetViewportRect();
        float spawnX = _isMovingRight ? -100 : viewportRect.Size.X + 100; // Spawn slightly outside the screen
        float spawnY = viewportRect.Size.Y - _spawnYOffset; // Spawn at a lower Y position
        GlobalPosition = new Vector2(spawnX, spawnY);

        GD.Print($"Enemy3 spawned at {GlobalPosition}. Moving {(_isMovingRight ? "right" : "left")}.");
    }

    public override void _Process(double delta)
    {
        // Move horizontally and check if it's out of bounds
        MoveHorizontally((float)delta);
        CheckOutOfBounds();
    }

    private void MoveHorizontally(float delta)
    {
        // Move in a straight horizontal line
        Velocity = new Vector2(_isMovingRight ? Speed : -Speed, 0);
        MoveAndSlide();
    }

    private void CheckOutOfBounds()
    {
        // Get the viewport boundaries
        Rect2 viewportRect = GetViewportRect();

        // Check if the enemy has entered the screen
        if (!_hasEnteredScreen)
        {
            if (GlobalPosition.X >= 0 && GlobalPosition.X <= viewportRect.Size.X)
            {
                _hasEnteredScreen = true;
                GD.Print("Enemy3 has entered the screen.");
            }
        }

        // Only despawn if the enemy has entered the screen and is now out of bounds
        if (_hasEnteredScreen && (GlobalPosition.X < 0 || GlobalPosition.X > viewportRect.Size.X))
        {
            GD.Print("Enemy3 is out of bounds. Despawning...");
            QueueFree(); // Despawn the enemy
        }
    }

    protected override void UpdateSpriteOrientation()
    {
        if (_sprite == null) return;

        // Flip the sprite based on movement direction
        _sprite.FlipH = !_isMovingRight;

        // Play the walk animation
        _sprite.Play("walk");
    }
}