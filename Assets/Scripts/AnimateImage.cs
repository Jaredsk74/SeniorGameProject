using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateImage : MonoBehaviour {

	public Sprite startImage;
	public Sprite endImage;
	public float animateTime = .25f; // How long it takes before the sprite changes
	private float elapsedTime = 0;
	private Image image; // Grab the image component of the gameobject

	// Use this for initialization
	void Start () {
		image = gameObject.GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		elapsedTime += Time.deltaTime;
		if (elapsedTime > animateTime) {
			// Switch the image to the other image
			if (image.sprite == startImage) {
				image.sprite = endImage;
			}
			else {
				image.sprite = startImage;
			}
			elapsedTime = 0;
		}
	}
}
