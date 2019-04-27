using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

	public Gameplay player;
	// Reference each map
	public TileMap easyMap;
	//public TileMap normalMap;
	//public TileMap hardMap;

	private void Awake() {

		DisableMap();
	}

	public void EnableMap() {
		//switch (difficulty) {
		//	case 0:
		//		easyMap.enabled = true;
		//		break;
		//	case 1:
		//		normalMap.enabled = true;
		//		break;
		//	case 2:
		//		hardMap.enabled = true;
		//		break;
		//	default:
		//		normalMap.enabled = true;
		//		break;
		//}
		Debug.Log("Enable map");
		easyMap.gameObject.SetActive(true);
		player.gameObject.SetActive(true);
	}

	public void DisableMap() {
		easyMap.gameObject.SetActive(false);
		player.gameObject.SetActive(false);
		//normalMap.enabled = false;
		//hardMap.enabled = false;
	}
}
