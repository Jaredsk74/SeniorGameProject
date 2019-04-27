using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : Window {

	public Text leftStatsLabel;
	public Text leftStatsValue;
	public Text rightStatsLabel;
	public Text rightStatsValue;
	public Text value;
	// This delay is the delay between each stat being displayed to build up to the total score display
	public float statsDelay = 1f;
	public int numOfStats = 6;
	public int statsPerColumn = 3;

	private int statRow = 0;
	//private float elapsedTime = 0;

	// Method to set stats
	private void UpdateStatText(Text label, Text value) {
		// Set the label
		label.text += "Stat " + statRow + "\n";
		// Give the value a random number and pad it with 4 spaces.
		value.text += Random.Range(0, 1000).ToString("D4") + "\n";
	}

	private void ShowNextStat() {
		// Check if we have reached the end of the stats
		if (statRow > numOfStats - 1) {
			// Set the total score and pad to 10
			value.CrossFadeAlpha(1f, statsDelay, false);
			value.text = Random.Range(0, 1000000000).ToString("D10");
			// Use -1 as a signal that the stats are done
			statRow = -1;
		}
		// Set left stat
		else if (statRow < statsPerColumn) {
			// Fade in animation
			if (statRow == 0) {
				leftStatsLabel.CrossFadeAlpha(1f, (statsDelay * statsPerColumn) * 1.2f, false);
				leftStatsValue.CrossFadeAlpha(1f, (statsDelay * statsPerColumn) * 1.2f, false);
			}

			UpdateStatText(leftStatsLabel, leftStatsValue);
			// Next row
			statRow++;
		}
		// Set right stat
		else {
			// Fade in animation
			if (statRow == statsPerColumn) {
				rightStatsLabel.CrossFadeAlpha(1f, statsDelay * statsPerColumn, false);
				rightStatsValue.CrossFadeAlpha(1f, statsDelay * statsPerColumn, false);
			}
			UpdateStatText(rightStatsLabel, rightStatsValue);
			// Next row
			statRow++;
		}
	}

	public void ClearText() {
		// Set the text invisible
		leftStatsLabel.CrossFadeAlpha(0, 0, false);
		leftStatsValue.CrossFadeAlpha(0, 0, false);
		rightStatsLabel.CrossFadeAlpha(0, 0, false);
		rightStatsValue.CrossFadeAlpha(0, 0, false);
		value.CrossFadeAlpha(0, 0, false);
		leftStatsLabel.text = "";
		leftStatsValue.text = "";
		rightStatsLabel.text = "";
		rightStatsValue.text = "";
		value.text = "0000000000";
	}

	public void Next() {
		NextWindow();
	}

	public override void Open() {
		// Reset text
		ClearText();
		base.Open();
	}

	public override void Close() {
		base.Close();
		// Reset row
		statRow = 0;
	}

	//private void Update() {
	//	// Get current elapsed time
	//	elapsedTime += Time.deltaTime;

	//	// Hit delay, time to display the next stat
	//	if (elapsedTime > statsDelay && statRow != -1) {
	//		ShowNextStat();
	//		elapsedTime = 0;
	//	}
	//}
}
