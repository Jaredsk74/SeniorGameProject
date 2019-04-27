using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// Links up with Tile Map
[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor {

	public TileMap map;
	public TileBrush brush;
	private Vector3 mouseHitPosition;
	private float row;
	private float column;

	private bool mouseOnMap {
		// Check that mouse is within map grid
		get { return (mouseHitPosition.x > 0 && mouseHitPosition.x < map.gridSize.x) && (mouseHitPosition.y > 0 && mouseHitPosition.y < map.gridSize.y); }
	}

	public override void OnInspectorGUI() {
		// Type of layout in the custom inspector
		EditorGUILayout.BeginVertical();
		// Grab mapsize from inspector
		// Keep reference to old size to see if it changed or not
		var oldSize = map.mapSize;
		map.mapSize = EditorGUILayout.Vector2Field("Map Size", map.mapSize);

		// If values did change, then update the map grid and border
		if (map.mapSize != oldSize) {
			UpdateCalculations();
		}

		// Grab tileset from inspector
		// Keep reference to old tileset to see if it changed or not
		var oldTileset = map.tileset;
		map.tileset = (Texture2D) EditorGUILayout.ObjectField("Tileset", map.tileset, typeof(Texture2D), false);

		// If values did change then update tileset setup tile painting
		if (map.tileset != oldTileset) {
			UpdateCalculations();
			map.tileId = 1; // 0 being the entire tileset
			CreateBrush();
		}

		// Display message if tileset isn't assigned yet
		if (map.tileset == null) {
			EditorGUILayout.HelpBox("No tileset has been selected.", MessageType.Warning); 
		}
		else {
			// Display tileset data and update brush
			EditorGUILayout.LabelField("Tile Size", map.tileSize.x + "x" + map.tileSize.y);
			map.tilePadding = EditorGUILayout.Vector2Field("Tile Padding", map.tilePadding);
			EditorGUILayout.LabelField("Grid Size in Units", map.gridSize.x + "x" + map.gridSize.y);
			EditorGUILayout.LabelField("Pixels Per Unit", map.pixelsPerUnit.ToString());
			UpdateBrush(map.tileBrush);

			// Add a button for clearing the map. Button returns true when pressed.
			if (GUILayout.Button("Clear Map")) {
				if (EditorUtility.DisplayDialog("Clear Tile Map", "Are you sure?", "Clear", "Cancel")) {
					ClearMap();
				}
			}

			// Add a button to sort tile objects by name
			if (GUILayout.Button("Sort Tiles")) {
				SortTiles();
			}
		}

		EditorGUILayout.EndVertical();
	}

	// OnEnable is called everytime the inspector is displayed
	private void OnEnable() {
		// TODO: Remove any extra instances of TileBrush or Tiles
		// Make sure inspector loads up map instance
		map = target as TileMap;
		// Change the view tool so that user doesn't accidentally rotate or scale tiles while placing
		Tools.current = Tool.View;

		// Check if the tiles object has been created or not
		if (map.tiles == null) {
			// Create a tiles object to hold tiles that are placed
			GameObject obj = new GameObject("Tiles");
			obj.transform.SetParent(map.transform);
			obj.transform.position = Vector3.zero;
			map.tiles = obj;
		}

		if (map.tileset != null) {
			UpdateCalculations();
			CreateBrush();
		}
	}

	// Called when clicking off the Tile Map
	private void OnDisable() {
		// Destroy brush(es)
		foreach (TileBrush brush in FindObjectsOfType<TileBrush>()) {
			DestroyImmediate(brush.gameObject);
		}
	}

	private void OnSceneGUI() {
		if (brush != null) {
			UpdateMousePosition ();
			MoveBrush();

			// Check if the map exists and the mouse is on it
			if (map.tileset != null && mouseOnMap) {
				if (Event.current.shift) {
					DrawTile();
				}
				else if (Event.current.control) {
					DestroyTile();
				}
			}
		}
	}

	// Refresh TileMap assets and calculations based on those assets
	private void UpdateCalculations() {
		// Grab a reference to the tileset sprites
		var path = AssetDatabase.GetAssetPath(map.tileset);
		map.sprites = AssetDatabase.LoadAllAssetsAtPath(path);

		if (map.sprites.Length <= 1) {
			EditorGUILayout.HelpBox("Invalid tileset selected. Did you remember to slice your tileset?", MessageType.Error);
		}
		else {
			// Grab the first sprite and get its size. First element in array is the whole texture so grab second.
			var sprite = (Sprite)map.sprites[1];
			var width = sprite.textureRect.width;
			var height = sprite.textureRect.height;

			map.tileSize = new Vector2(width, height);

			// Set the grid size to actual number of Unity units
			map.pixelsPerUnit = sprite.pixelsPerUnit;
			map.gridSize = new Vector2((width / map.pixelsPerUnit) * map.mapSize.x, (height / map.pixelsPerUnit) * map.mapSize.y);
		}
	}

	private void CreateBrush() {
		// Make sure there isn't already an existing tile brush
		if (brush == null) {
			// Grab reference to map's tile brush and make sure its not null
			var sprite = map.tileBrush;
			if (sprite != null) {
				// Create a new object
				GameObject obj = new GameObject("TileBrush");
				// Nest the brush inside the tile map object
				obj.transform.SetParent(map.transform);

				// Attach the game object and finish setting up the brush
				brush = obj.AddComponent<TileBrush>();
				brush.tile = obj.AddComponent<SpriteRenderer>();
				// Make sure the brush stays above placed tiles
				brush.tile.sortingOrder = 1000;

				// Calculate brush size using the tileset pixels per unit.
				brush.brushSize = new Vector2(sprite.textureRect.width / map.pixelsPerUnit,
												sprite.textureRect.height / map.pixelsPerUnit);
				brush.UpdateBrush(sprite);
				// Set the brush to origin position
				MoveBrush(map.transform.position.x, map.transform.position.y);
			} 
		}
	}

	// Sets the brush sprite to the tile sprite
	public void UpdateBrush(Sprite sprite) {
		if (brush != null) {
			brush.UpdateBrush(sprite);
		}
	}

	private void UpdateMousePosition() {
		// Used to track the scene collision
		var plane = new Plane(map.transform.TransformDirection(Vector3.forward), Vector3.zero);
		var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		var hit = Vector3.zero;
		float distance = 0;

		// Test if there is a collision between the mouse point and the scene
		if (plane.Raycast(ray, out distance)) {
			hit = ray.origin + ray.direction.normalized * distance;
		}

		// Convert that point to usable data
		mouseHitPosition = map.transform.InverseTransformPoint(hit);
	}

	// Move brush based on mouse position on Tile Map
	private void MoveBrush() {
		// TODO: Make brush follow mouse even while mouse is outside the Tile Map 
		// Make sure mouse is in actual map before updating
		if (mouseOnMap) {
			// Calculate tilesize
			var tileSize = map.tileSize.x / map.pixelsPerUnit;

			// Calculate brush position
			var x = Mathf.Floor(mouseHitPosition.x / tileSize) * tileSize;
			var y = Mathf.Floor(mouseHitPosition.y / tileSize) * tileSize;

			x += map.transform.position.x + tileSize / 2;
			y += map.transform.position.y + tileSize / 2;

			row = x / tileSize;
			column = y / tileSize;

			// Give the brush an id
			var id = (int)((column * map.mapSize.x) + row);
			brush.tileId = id;

			brush.transform.position = new Vector3(x, y, map.transform.position.z);
		}
	}

	// Move brush based on given x and y coordinates
	private void MoveBrush(float x, float y) {
		// Calculate tilesize
		var tileSize = map.tileSize.x / map.pixelsPerUnit;

		// Calculate brush position
		x += map.transform.position.x + tileSize / 2;
		y += map.transform.position.y + tileSize / 2;

		// Calculate row and column for generating an id
		row = x / tileSize;
		column = y / tileSize;

		// Give the brush an id
		var id = (int)((column * map.mapSize.x) + row);
		brush.tileId = id;

		// Set the position
		brush.transform.position = new Vector3(x, y, map.transform.position.z);
	}

	private void DrawTile() {
		// Generate a name for the tile object
		string tileName = brush.tileId.ToString();
		tileName = (row - .5).ToString() + "," + (column - .5).ToString();

		// Calculate position of tile
		var x = brush.transform.position.x;
		var y = brush.transform.position.y;

		// Check if the tile being placed already exists
		GameObject tile = GameObject.Find(map.name + "/Tiles/Tile_" + tileName);
		if (tile == null) {

			// If not then create it
			tile = new GameObject("Tile_" + tileName);

			// Set parent object to the Tiles group
			tile.transform.SetParent(map.tiles.transform);
			tile.transform.position = new Vector3(x, y, 0);

			// Add the sprite renderer
			tile.AddComponent<SpriteRenderer>();
		}

		// Set the tile to the sprite of the brush
		tile.GetComponent<SpriteRenderer>().sprite = brush.tile.sprite;
	}

	private void DestroyTile() {
		// Check if a tile exists to be destroyed
		string tileName = brush.tileId.ToString();
		GameObject tile = GameObject.Find(map.name + "/Tiles/Tile_" + tileName);

		// If it does exist then destroy it
		if (tile != null) {
			DestroyImmediate(tile);
		}
	}

	private void ClearMap () {
		for (int i = 0; i < map.tiles.transform.childCount; i++) {
			DestroyImmediate(map.tiles.transform.GetChild(i).gameObject);
			// The child count will continue to decrease so its important that i stays at 0
			i--;
		}
	}

	private void SortTiles () {
		// Grab the tiles group
		GameObject tiles = GameObject.Find(map.name + "/Tiles");
		
		// Create a list of the names
		List<string> tileList = new List<string>();
		foreach (Transform go in tiles.transform.GetComponentsInChildren<Transform>()) {
			tileList.Add(go.name);
		}

		// Remove first element (which will be Tiles) and sort
		tileList.RemoveAt(0);
		tileList.Sort();

		// Move each object to last position by name
		foreach (string name in tileList) {
			GameObject go = GameObject.Find(tiles.name + "/" + name);
			go.transform.SetAsLastSibling();
		}
	}
}
