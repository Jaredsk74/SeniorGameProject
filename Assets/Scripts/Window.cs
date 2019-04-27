using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Base class of all windows
public class Window : MonoBehaviour {

	// Look into Dependency Injection and Singletons
	// Give each window a connection to the WindowManager
	public static WindowManager manager;

	// Reference to the first selected item
	public GameObject firstSelected;
	public Windows nextWindow;
	public Windows prevWindow;

	// Find the EventSystem
	protected EventSystem eventSystem {
		get { return GameObject.Find("EventSystem").GetComponent<EventSystem>(); }
	}

	// Set selected item
	public virtual void OnFocus() {
		eventSystem.SetSelectedGameObject(firstSelected);
	}

	// Toggle whether window is active or not
	protected virtual void Display(bool value) {
		gameObject.SetActive(value);
	}

	// Open the selected window
	public virtual void Open() {
		Display(true);
		OnFocus();
	}

	// Close the selected window
	public virtual void Close() {
		Display(false);
	}

	// Keeps windows closed by default unless overridden with Start or extended in a child class
	protected virtual void Awake() {
		Close();
	}

	public void NextWindow() {
		manager.Open((int) nextWindow - 1);
	}

	public void PreviousWindow() {
		manager.Open((int)prevWindow - 1);
	}
}
