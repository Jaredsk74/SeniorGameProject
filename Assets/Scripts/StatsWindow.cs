using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StatsWindow : Window {

	public Actor player;
	public Text values;

	// Build stats panel values
	public void UpdateStats() {
		// Check for null
		if (player != null && values != null) {
			StringBuilder sb = new StringBuilder();

			// D2 makes sure a single digit is 2 digits in space
			sb.Append(player.health.ToString("D2"));
			sb.Append("/");
			sb.AppendLine(player.maxHealth.ToString("D2"));
			sb.Append(player.money.ToString("D5")); // D5: 5 spaces

			values.text = sb.ToString();
		}
	}
}
