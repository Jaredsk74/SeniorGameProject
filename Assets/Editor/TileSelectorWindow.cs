using UnityEditor;
using UnityEngine;

public class TileSelectorWindow : EditorWindow {

	// Setup menu options for scaling
	public enum Scale {
		ScaleWithWindow,
		_1x,
		_2x,
		_3x,
		_4x,
		_5x
	}

	private Scale scale;
	Vector2 currentTileSelected = Vector2.zero;

	// Keeps track of scroll bar position
	public Vector2 scrollPosition = Vector2.zero;

	[MenuItem("Window/Tile Selector")]
	public static void OpenTileSelectionWindow() {
		var window = EditorWindow.GetWindow(typeof(TileSelectorWindow));
		var title = new GUIContent("Tile Selector");
		window.titleContent = title;
	}

	private void OnGUI() {
		// Only show tiles if a tilemap object is selected
		// First check if not null
		if (Selection.activeGameObject != null) {
			var selection = Selection.activeGameObject.GetComponent<TileMap>();
			// Check if the gameobject has a tilemap component
			if (selection != null) {
				var tileset = selection.tileset;
				if (tileset != null) {

					// Prepare menu listing the scale options
					scale = (Scale) EditorGUILayout.EnumPopup("Zoom", scale);

					// Add an offset to give the menu a little extra room
					var offset = new Vector2(0, 25);

					// Grab scale option
					float newScale = (int)scale;

					// Set scaleWithWindow
					bool scaleWithWindow = false;
					if ((int)scale == 0) {
						scaleWithWindow = true;
					}

					// Calculate proper ratio if scaleWithWindow
					if (scaleWithWindow) {
						var xRatio = (position.width - offset.x) / tileset.width;
						var yRatio = (position.height - offset.y) / tileset.height;

						if (xRatio > yRatio) {
							newScale = yRatio;
						}
						else {
							newScale = xRatio;
						}
					}

					// Calculate new tileset scale
					var newTilesetSize = new Vector2(tileset.width, tileset.height) * newScale;

					// Calculate window size and size of content. If content is larger than window then add scroll bars.
					var windowSize = new Rect(0, 0, position.width, position.height);
					var contentSize = new Rect(0, 0, newTilesetSize.x + offset.x, newTilesetSize.y + offset.y);

					// Create ScrollView
					scrollPosition = GUI.BeginScrollView(windowSize, scrollPosition, contentSize);
					// Draw tileset in window and close ScrollView
					GUI.DrawTexture(new Rect(offset.x, offset.y, newTilesetSize.x, newTilesetSize.y), tileset);

					// Calculate new tileset, tile size and grid size
					var tileSize = selection.tileSize * newScale;
					// Account for padding
					tileSize.x += selection.tilePadding.x * newScale;
					tileSize.y += selection.tilePadding.y * newScale;
					var gridSize = new Vector2(newTilesetSize.x / tileSize.x, newTilesetSize.y / tileSize.y);

					// Now set the current selected tile position
					var selectedTilePosition = new Vector2(tileSize.x * currentTileSelected.x + offset.x,
						tileSize.y * currentTileSelected.y + offset.y);

					// ---------- Creating tile highlight ----------
					// Create a texture to apply to box border and later to inner box
					var highlightTexture = new Texture2D(1, 1);
					highlightTexture.SetPixel(0, 0, Color.black);
					highlightTexture.Apply();

					// Set the box border texture to a style using the GUI Skin class which provides a nice template
					var style = new GUIStyle(GUI.skin.customStyles[0]);
					style.normal.background = highlightTexture;

					// Draw border
					var borderX = tileSize.x * .05f;
					var borderY = tileSize.y * .05f;
					GUI.Box(new Rect(selectedTilePosition.x, selectedTilePosition.y, borderX, tileSize.y), "", style);
					GUI.Box(new Rect(selectedTilePosition.x, selectedTilePosition.y, tileSize.x, borderY), "", style);
					GUI.Box(new Rect(selectedTilePosition.x + (tileSize.x-borderX), selectedTilePosition.y, borderX, tileSize.y), "", style);
					GUI.Box(new Rect(selectedTilePosition.x, selectedTilePosition.y + (tileSize.y-borderY), tileSize.x - borderX, borderY), "", style);

					// Do it again but for the inner box
					highlightTexture = new Texture2D(1, 1);
					highlightTexture.SetPixel(0, 0, new Color(1, 1, 1, .25f));
					highlightTexture.Apply();
					style = new GUIStyle(GUI.skin.customStyles[0]);
					style.normal.background = highlightTexture;
					GUI.Box(new Rect(selectedTilePosition.x + borderX, selectedTilePosition.y + borderY, 
										tileSize.x - borderX * 2, tileSize.y - borderY * 2), "", style);
					// ---------------------------------------------

					// Check the event system to see if the mouse is being pressed
					var clickEvent = Event.current;
					Vector2 mousePosition = new Vector2(clickEvent.mousePosition.x - offset.x, clickEvent.mousePosition.y - offset.y);
					if (clickEvent.type == EventType.mouseDown && clickEvent.button == 0) {
						// Snap to grid to grab new grid position
						currentTileSelected.x = Mathf.Floor((mousePosition.x) / tileSize.x);
						currentTileSelected.y = Mathf.Floor((mousePosition.y) / tileSize.y);

						// Make sure selected tile is within grid.
						if (currentTileSelected.x > gridSize.x - 1) {
							currentTileSelected.x = gridSize.x - 1;
						}
						if (currentTileSelected.y > gridSize.y - 1) {
							currentTileSelected.y = gridSize.y - 1;
						}

						// Give the tile an ID to track which is selected
						selection.tileId = (int) (currentTileSelected.x + (currentTileSelected.y * gridSize.x) + 1);
						//Debug.Log(selection.tileId);

						Repaint();
					}

					GUI.EndScrollView();
				}
			}
		}
	}
}