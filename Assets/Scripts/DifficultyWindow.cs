using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyWindow : Window {
	// This class doesn't currently interact with the keyboard

	// Unity doesn't give any methods to help this class act as a list or group
	public ToggleGroup toggleGroup;
	public Toggle[] toggles;

	public int difficulty;

	public override void Open() {
		difficulty = PlayerPrefs.GetInt("Difficulty", 1);
		toggles = toggleGroup.GetComponentsInChildren<Toggle>();
		base.Open();
	}

	public void Save() {
		Debug.Log("Difficulty: " + difficulty);
		NextWindow();
	}

	private void Update() {
		// Change toggle difficulty as needed
		for (int i = 0; i < toggles.Length; i++) {
			if (toggles[i].isOn) {
				difficulty = i;
				return;
			}
		}

		// Watch for select button (usually return or space)
		if (Input.GetButtonDown("Submit")) {
			Save();
		}
	}

	private void Start() {
		toggles = toggleGroup.GetComponentsInChildren<Toggle>();
	}
}
