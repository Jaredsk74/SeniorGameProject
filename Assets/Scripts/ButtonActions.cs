using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour {
	

	public Gameplay player;

	void Update () {
		// Don't allow any actions if a player is currently moving
		if (!player.playerMoving) {
			if (Input.GetKeyDown(KeyCode.UpArrow)) {
				player.Move("Up");
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow)) {
				player.Move("Down");
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow)) {
				player.Move("Right");
			}
			else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				player.Move("Left");
			}
			else if (Input.GetKeyDown(KeyCode.C)) {
				player.BeginCombat();
			}
			else if (Input.GetKeyDown(KeyCode.E)) {
				player.EndCombat();
			}
		}
	}
}
