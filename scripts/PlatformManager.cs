using Godot;
using System;

public partial class PlatformManager : Node2D
{
	[Export] private Node2D _scene; // Parent node for instantiated groups
	[Export] public float _levelSpeed {get; set;} = 200f; // Speed at which groups move
	[Export] private PackedScene[] _groups; // Array of group scenes to instantiate
	[Export] private float _groupLength = 250f; // Spacing between groups

	private float _currentGroupProgress = 0; // Tracks movement progress

	public override void _Ready()
	{
		_currentGroupProgress = 0;

		// Initialize groups to fill the screen and make sure it scrolls infinitely
		int initialGroups = Mathf.CeilToInt(GetViewportRect().Size.Y / _groupLength) + 2; // Dynamically calculate based on screen height
		for (int i = 0; i < initialGroups; i++)
		{
			_InstantiateGroup(-(i + 1)); // Instantiate groups at negative y values
		}

		// Find the player node and connect to its PlayerDied signal
		Player player = GetNode<Player>("../Player"); // Adjust the path to your player node
		if (player != null) {
			player.PlayerDied += OnPlayerDied;
		} else {
			GD.PrintErr("Player node not found!");
		}
	}

	public override void _Process(double delta)
	{
		float progress = _levelSpeed * (float)delta;

		// Move all children of _scene downward
		foreach (Node child in _scene.GetChildren())
		{
			if (child is Node2D node2D)
			{
				node2D.GlobalPosition += new Vector2(0, progress);
			}
		}

		// Track progress for instantiating and removing groups
		_currentGroupProgress += progress;

		if (_currentGroupProgress >= _groupLength)
		{
			// Spawn a new group above the top of the screen
			Node2D lastGroup = _scene.GetChild(_scene.GetChildCount() - 1) as Node2D;
			if (lastGroup != null)
			{
				// Calculate the y-position for the new group
				float newY = lastGroup.GlobalPosition.Y - _groupLength;
				_InstantiateGroupAtY(newY);
			}

			// Remove the first group only if it is completely out of view
			Node2D firstGroup = _scene.GetChild(0) as Node2D;
			if (firstGroup != null && firstGroup.GlobalPosition.Y >= GetViewportRect().Size.Y)
			{
				firstGroup.QueueFree();
			}

			// Reset progress
			_currentGroupProgress -= _groupLength;
		}
	}

	private void _InstantiateGroup(int offset = 0, int type = -1)
	{
		int index = (type == -1) ? (int)(GD.Randi() % _groups.Length) : type;
		PackedScene selectedScene = _groups[index];

		if (selectedScene == null)
		{
			GD.PrintErr("Invalid PackedScene in _groups array!");
			return;
		}

		Node2D group = selectedScene.Instantiate<Node2D>();
		_scene.AddChild(group);

		// Position the group at the correct y-position
		group.GlobalPosition = new Vector2(0, offset * _groupLength);

		// Debug print
		GD.Print($"Instantiated group at offset {offset}, y = {offset * _groupLength}");
	}

	private void _InstantiateGroupAtY(float y, int type = -1)
	{
		int index = (type == -1) ? (int)(GD.Randi() % _groups.Length) : type;
		PackedScene selectedScene = _groups[index];

		if (selectedScene == null)
		{
			GD.PrintErr("Invalid PackedScene in _groups array!");
			return;
		}

		Node2D group = selectedScene.Instantiate<Node2D>();
		_scene.AddChild(group);

		// Position the group at the specified y-position
		group.GlobalPosition = new Vector2(0, y);

		// Debug print
		GD.Print($"Instantiated group at y = {y}");
	}

	private void OnPlayerDied() {
		GD.Print("Player died! Stopping platform movement...");
		_levelSpeed = 0;
	}
}
