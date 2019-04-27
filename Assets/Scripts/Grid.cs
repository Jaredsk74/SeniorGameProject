using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

	private TileMap map;
	public static GameObject[,] grid;
	public static int gridX;
	public static int gridY;

	void Start () {
		// Grab reference
		map = FindObjectOfType<TileMap>();

		grid = new GameObject[(int) map.mapSize.x, (int) map.mapSize.y];

		// Setup an array to track the grid
		// It is setup like and X, Y coordinate system
		// 0,0 is the bottom left
		for (int i = 0; i < grid.GetLength(0); i++) {
			for (int j = 0; j < grid.GetLength(1); j++) {
				// Debug.Log("Grid (" + j + ", " + i + ")");
				grid[j, i] = GameObject.Find("Tile_" + j + "," + i);
			}
		}

		gridX = grid.GetLength(1);
		gridY = grid.GetLength(0);
	}
}
