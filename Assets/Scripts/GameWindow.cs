using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWindow : Window {

	public MapManager maps;

	public override void Open() {
		base.Open();
		maps.EnableMap();
		Camera.main.GetComponent<PanCamera>().enabled = true;
	}

	public override void Close() {
		base.Close();
		maps.DisableMap();
		Camera.main.GetComponent<PanCamera>().enabled = false;
	}
}
