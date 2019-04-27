using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartWindow : Window {

	public Button resumeButton;

	// Override Open to modify default buttons
	public override void Open() {
		// Check if the Continue button should or shouldn't be enabled
		bool canResume = false;
		resumeButton.gameObject.SetActive(canResume);
		Navigation nav = new Navigation();
		Button dummy = firstSelected.GetComponent<Button>();

		// Change the default navigation to resume instead of new game
		if (resumeButton.gameObject.activeSelf) {
			nav.mode = Navigation.Mode.Explicit;
			nav.selectOnDown = resumeButton;
			nav.selectOnUp = resumeButton;
			dummy.navigation = nav;
		}

		// Change the navigation to account for the resume
		else {
			Button newGame = dummy.navigation.selectOnDown.gameObject.GetComponent<Button>();
			Button options = newGame.navigation.selectOnDown.gameObject.GetComponent<Button>();

			// Changing new game navigation
			nav.mode = Navigation.Mode.Explicit;
			nav.selectOnDown = options;
			nav.selectOnUp = options;
			newGame.navigation = nav;

			// Changing options navigation
			nav.mode = Navigation.Mode.Explicit;
			nav.selectOnDown = newGame;
			nav.selectOnUp = newGame;
			options.navigation = nav;
		}

		base.Open();
	}

	public void NewGame() {
		NextWindow();
	}
	public void Resume() {
		Debug.Log("Resume");
	}
	public void Options() {
		Debug.Log("Options");
	}
}
