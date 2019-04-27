using UnityEditor;
using UnityEngine;

public class TileMapMenuItem {

	[MenuItem("GameObject/Tile Map")]
	public static void CreateTileMap() {
		GameObject o = new GameObject("TileMap");
		o.AddComponent<TileMap>();
	}
}
