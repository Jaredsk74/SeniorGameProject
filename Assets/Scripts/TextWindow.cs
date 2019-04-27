using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextWindow : Window {

	public Text input;
	public int maxChars = 16;

	private float elapsedTime = 0;
	private float cursorDelay = .5f;
	private bool blink;
	private string userInput = "";

	private void Update() {
		string text = userInput;

		if (text.Length < maxChars) {
			text += "_";

			// If blinking, remove the last character
			if (blink) {
				text = text.Remove(text.Length - 1);
			}
		}

		input.text = text;

		// Check how long it's been since the last blink
		elapsedTime += Time.deltaTime;
		if (elapsedTime > cursorDelay) {
			elapsedTime = 0;
			blink = !blink;
		}
	}

	public void Accept() {
		NextWindow();
	}

	public void Cancel() {
		PreviousWindow();
	}
}
