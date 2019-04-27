using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour {

	[HideInInspector] // Use WindowManagerEditor to deal with Windows
	public Window[] windows;
	public int currentWindow;
	public int startingWindow;

	public Window GetWindow(int index) {
		return windows[index];
	}

	private void ToggleVisibility(int index, bool closeAllOpen = true) {
		int total = windows.Length;

		if (closeAllOpen) {
			// Go through each window incase any windows were left open
			for (int i = 0; i < total; i++) {
				Window window = windows[i];
				// If we come accross an open window, close it
				if (window.gameObject.activeSelf) {
					window.Close();
				}
			}
		}

		GetWindow(index).Open();

	}

	public Window Open(int index, bool closeAllOpen = true) {
		// Invalid index
		if (index < 0 || index >= windows.Length) {
			return null;
		}

		// Set the current window to the given index and open it
		currentWindow = index;
		ToggleVisibility(currentWindow, closeAllOpen);

		return GetWindow(currentWindow);
	}

	private void Start() {
		Open(startingWindow);
		Window.manager = this;
	}
}
