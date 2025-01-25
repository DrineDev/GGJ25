using Godot;

public partial class GameBorder : Node
{
    // References to the border Area2D nodes
    private Area2D _leftBorder;
    private Area2D _rightBorder;
    private Area2D _bottomBorder;

    // Coordinate range
    private float _minX = -300f;
    private float _maxX = 300f;
    private float _minY = -500f;
    private float _maxY = 500f;

    public override void _Ready()
    {
        // Get references to the border Area2D nodes
        _leftBorder = GetNode<Area2D>("LeftBorder");
        _rightBorder = GetNode<Area2D>("RightBorder");
        _bottomBorder = GetNode<Area2D>("BottomBorder");

        // Debug: Check if nodes are found
        if (_leftBorder == null) GD.Print("LeftBorder not found!");
        if (_rightBorder == null) GD.Print("RightBorder not found!");
        if (_bottomBorder == null) GD.Print("BottomBorder not found!");

        // Connect the body_entered signal for each border
        if (_leftBorder != null)
            _leftBorder.BodyEntered += OnLeftBorderEntered;
        if (_rightBorder != null)
            _rightBorder.BodyEntered += OnRightBorderEntered;
        if (_bottomBorder != null)
            _bottomBorder.BodyEntered += OnBottomBorderEntered;
    }

    private void OnLeftBorderEntered(Node body)
    {
        if (body is Player player)
        {
            GD.Print("Player entered the left border. Teleporting to the right...");
            // Teleport the player to the right border
            TeleportPlayer(player, new Vector2(_maxX, player.GlobalPosition.Y));
        }
        else if (body is IEnemy enemy)
        {
            enemy.OnBorderEntered();
        }
    }

    private void OnRightBorderEntered(Node body)
    {
        if (body is Player player)
        {
            GD.Print("Player entered the right border. Teleporting to the left...");
            // Teleport the player to the left border
            TeleportPlayer(player, new Vector2(_minX, player.GlobalPosition.Y));
        }
        else if (body is IEnemy enemy)
        {
            enemy.OnBorderEntered();
        }
    }

    private void OnBottomBorderEntered(Node body)
    {
        if (body is Player player)
        {
            GD.Print("Player entered the bottom border. Restarting game...");
            player.Die();
        }
        else if (body is IEnemy enemy)
        {
            enemy.OnBorderEntered();
        }
    }

    private void TeleportPlayer(Player player, Vector2 targetPosition)
    {
        // Clamp the target position to the coordinate range
        targetPosition.X = Mathf.Clamp(targetPosition.X, _minX, _maxX);
        targetPosition.Y = Mathf.Clamp(targetPosition.Y, _minY, _maxY);

        // Teleport the player to the target position
        player.GlobalPosition = targetPosition;
        GD.Print("Player teleported to: ", targetPosition);
    }
}