using UnityEngine;

public class TileMap : MonoBehaviour {

	public Vector2 mapSize = new Vector2(10, 10);
	public Texture2D tileset;
	public Vector2 tileSize = new Vector2(0, 0);
	public Vector2 tilePadding = new Vector2(0, 0);
	public Object[] sprites;
	public Vector2 gridSize = new Vector2(0, 0);
	public float pixelsPerUnit = 100;
	public int tileId = 0;
	public GameObject tiles;

	// Getter for sprite from brush
	public Sprite tileBrush {
		get { return sprites[tileId] as Sprite; }
	}

	// Gizmo is a tool to assist in placing objects. Gizmos are not visible in-game.
	private void OnDrawGizmosSelected() {
		var position = transform.position;

		// Check if a tileset was loaded
		if (tileset != null) {

			// Drawing tiles
			Gizmos.color = Color.grey;

			int currentRow = 0;
			int currentColumn = 0;
			var maxColumns = mapSize.x;
			var totalTiles = mapSize.x * mapSize.y;
			var tile = new Vector3(tileSize.x / pixelsPerUnit, tileSize.y / pixelsPerUnit);
			var offset = new Vector2(tile.x / 2, tile.y / 2);

			for (int i = 0; i < totalTiles; i++) {

				// Grab current column and then calculate the x and y of the center of the tile
				currentColumn = (int)(i % maxColumns);
				var tileCenterX = (currentColumn * tile.x) + offset.x + position.x;
				var tileCenterY = (currentRow * tile.y) + offset.y + position.y;

				Gizmos.DrawWireCube(new Vector2(tileCenterX, tileCenterY), tile);

				// If at end of column, then move onto next row
				if (currentColumn == maxColumns - 1) {
					currentRow++;
				}
			}

			// Set border color
			Gizmos.color = Color.white;

			// Gizmos are drawn from the center out. Calculate a new center that positions the gizmo as quadrant 1.
			var centerX = position.x + (gridSize.x / 2);
			var centerY = position.y + (gridSize.y / 2);

			// Draw border
			Gizmos.DrawWireCube(new Vector2(centerX, centerY), gridSize);
		}
	}
}
