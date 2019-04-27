using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanCamera : MonoBehaviour {

	public float mouseSensitivity = 4f;
	private Vector3 startingPosition;
	private bool moving;
	//private GameController gc;

	//private void Start() {
	//	gc = FindObjectOfType<GameController>();
	//}

	private void Update() {
		// If right button has been pressed but is now released
		if (Input.GetMouseButtonUp(1) && moving) {
			moving = false;
		}
	}

	private void FixedUpdate() {
		// Check if the right mouse button is down
		if (Input.GetMouseButtonDown(1)) {
			startingPosition = Input.mousePosition;
			moving = true;
		}

		// Finally move the camera to the mouse position
		if (moving) {
			Vector3 position = Camera.main.ScreenToViewportPoint(Input.mousePosition - startingPosition);
			Vector3 newPosition = new Vector3(position.x * mouseSensitivity, position.y * mouseSensitivity, 0);
			transform.Translate(newPosition, Space.Self);
		}
		//else if (gc.currentPlayer != null) {
		//	GameObject target = gc.currentPlayer.gameObject;
		//	Vector3 position = target.transform.position;
		//	position.z = Camera.main.transform.position.z;

		//	Camera.main.transform.position = position;
		//}
	}
}
