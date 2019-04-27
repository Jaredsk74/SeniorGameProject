using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindow : Window {

	public float closeDelay = 2f;
	private float elapsedTime;
	private bool windowClosing;
	private Text textObject; // Actual object
	
	// Text to be set
	public string text {
		set {
			textObject.text = value;
		}
	}

	protected override void Awake() {
		// Set text upon awake
		textObject = GetComponentInChildren<Text>();

		base.Awake();
	}

	public override void Open() {
		base.Open();
		// Prepare to close window everytime it opens
		windowClosing = true;
		elapsedTime = 0;
	}

	private void Update() {
		if (windowClosing) {
			elapsedTime += Time.deltaTime;
			if (elapsedTime > closeDelay) {
				windowClosing = false;
				Close();
			}
		}
	}
}
