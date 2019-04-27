using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAnimation : MonoBehaviour {

	private RectTransform target; // Whatever is being shaken
	private float duration;
	private float strength; // How strong the shaking is
	private float elapsedTime = 0;
	private Vector2 originalPosition;

	// Begin shaking for time given
	public void Shake(RectTransform target, float duration = 1, float strength = 0) {
		this.target = target;
		originalPosition = target.anchoredPosition;
		this.duration = duration;
		this.strength = strength;
		elapsedTime = 0;
	}

	// Immediately stop and reset target
	public void Stop() {
		target.anchoredPosition = originalPosition;
		target = null;
	}

	private void Update() {
		if (target != null) {
			// Give a random movement
			if (elapsedTime < duration) {
				float rumbleX = Random.Range(-strength, strength) + originalPosition.x;
				float rumbleY = Random.Range(-strength, strength) + originalPosition.y;

				// Set the position
				target.anchoredPosition = new Vector2(rumbleX, rumbleY);
				elapsedTime += Time.deltaTime;
			}
			else {
				Stop();
			}
		}
	}
}
