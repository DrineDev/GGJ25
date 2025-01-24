using Godot;

public partial class GameBorder : Node {
    // References to the border Area2D nodes
    private Area2D _topBorder;
    private Area2D _bottomBorder;
    private Area2D _leftBorder;
    private Area2D _rightBorder;

    public override void _Ready() {
        // Get references to the border Area2D nodes
        _topBorder = GetNode<Area2D>("TopBorder");
        _bottomBorder = GetNode<Area2D>("BottomBorder");
        _leftBorder = GetNode<Area2D>("LeftBorder");
        _rightBorder = GetNode<Area2D>("RightBorder");

        // Connect the body_entered signal for each border
        _topBorder.BodyEntered += OnBorderEntered;
        _bottomBorder.BodyEntered += OnBorderEntered;
        _leftBorder.BodyEntered += OnBorderEntered;
        _rightBorder.BodyEntered += OnBorderEntered; }

    private void OnBorderEntered(Node body) {
        // Handle objects entering the border
        if (body is Player player) {
            GD.Print("Player entered the border!");
            // Reset player position or trigger an event
            ResetPlayerPosition(player); }
        else if (body is Enemy enemy) {
            GD.Print("Enemy entered the border!");

            // Check if the enemy has entered the border before
            if (enemy.HasEnteredBorder) {
                // Despawn the enemy if it has entered the border before
                GD.Print("Enemy has entered the border before. Despawning...");
                enemy.QueueFree();
            } else {
                // Mark the enemy as having entered the border
                enemy.HasEnteredBorder = true;
                GD.Print("Enemy entered the border for the first time.");
            }
        }
    }
    private void ResetPlayerPosition(Player player) {
        // Flip the player's X position to the opposite side and add a small offset
        float offset = 10f; // Adjust this value as needed
        player.GlobalPosition = new Vector2(-player.GlobalPosition.X + Mathf.Sign(player.GlobalPosition.X) * offset, player.GlobalPosition.Y);
    }
    private void ResetEnemyPosition(Enemy enemy) {
        // Example: Reset enemy position or destroy it
        enemy.QueueFree(); // Destroy the enemy
    }
}