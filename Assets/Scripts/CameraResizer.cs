using UnityEngine;

public class CameraResizer : MonoBehaviour {

	public static float pixelsToUnits = 1f;
	public static float scale = 1f;

	// Resolution that the camera will adapt to
	public Vector2 nativeResolution = new Vector2(240, 160);

	// Awake is called before Start()
	void Awake () {
		var camera = GetComponent<Camera>();

		if (camera.orthographic) {
			// Get device screen height
			scale = Screen.height / nativeResolution.y;
			pixelsToUnits *= scale;

			// Set camera to new resolution
			camera.orthographicSize = (Screen.height / 2f) / pixelsToUnits;
		}
	}
}
