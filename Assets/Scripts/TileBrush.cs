using UnityEngine;

public class TileBrush : MonoBehaviour {

	public Vector2 brushSize = Vector2.zero;
	public int tileId = 0;
	public SpriteRenderer tile;

	// Drawing a border of the brush
	private void OnDrawGizmosSelected() {
		// Color must be nicely visible against dark grey
		Gizmos.color = Color.black;
		Gizmos.DrawWireCube(transform.position, brushSize);
	}

	public void UpdateBrush(Sprite sprite) {
		tile.sprite = sprite;
	}
}
