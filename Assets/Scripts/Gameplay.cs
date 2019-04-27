using System;
using System.Collections;
using UnityEngine;

public class Gameplay : MonoBehaviour {
	private bool ableToMove = true; // Primarily used during battle
	private Actor playerActor;
	[Range(0, .9f)]
	public float battleChance = .3f;
	public Vector2 currentTile = Vector2.zero;

	public GameObject[] treasures;
	
	private WindowManager windowManager; // Keep track of window manager to open other windows
	private CombatWindow combatWindow; // Keep track of battle window, will be assigned by WindowManager
	private StatsWindow statsWindow; // Same thing as above

	// Lerp stuff
	public bool playerMoving;
	public float speed = 1f;
	private float moveTime;
	private Vector2 startPosition;
	private Vector2 endPosition;

	private void Start() {
		// Grab references
		windowManager = FindObjectOfType<WindowManager>();
		playerActor = gameObject.GetComponent<Actor>();
		playerActor.ResetHealth();

		// Set the player position at the beginning
		Vector2 newPos = Grid.grid[(int)currentTile.x, (int)currentTile.y].transform.position;
		gameObject.transform.position = newPos;

		// Open starting windows
		OpenMessageWindow("Find the castle!");
		OpenStatsWindow();
	}

	private void Update() {
		// Lerp stuff
		if (playerMoving) {

			// Calculate difference in time from the last frame
			moveTime += Time.deltaTime;
			if (moveTime > speed) {
				// Player has finished moving, needs to stop
				playerMoving = false;
				transform.position = endPosition;

				TileAction();
			}

			transform.position = Vector2.Lerp(startPosition, endPosition, moveTime / speed);
		}
	}

	// Method is ran after movement
	private void TileAction() {
		// Grab reference to current tile game object
		GameObject tile = Grid.grid[(int)currentTile.x, (int)currentTile.y];

		// If treasure, then add some money and change the tag
		if (tile.tag == "Treasure") {
			int money = UnityEngine.Random.Range(5, 51);
			playerActor.AddMoney(money);
			OpenMessageWindow("Player has gained " + money + " gold!");
			statsWindow.UpdateStats();
			tile.tag = "ClearTile";
		}
		else if (tile.tag == "Castle") {
			int money = UnityEngine.Random.Range(50, 151);
			playerActor.AddMoney(money);
			OpenMessageWindow("Player has gained " + money + " gold!");
			statsWindow.UpdateStats();
			StartCoroutine(won());
		}
		else {
			float chance = UnityEngine.Random.Range(0, 1f);
			if (chance < battleChance) {
				BeginCombat();
			}
		}
	}

	private IEnumerator won() {
		yield return new WaitForSeconds(2);

		// Move player
		Vector2 newPos = Grid.grid[2, 2].transform.position;
		gameObject.transform.position = newPos;
		currentTile = new Vector2(2, 2);

		// Add to player health
		int healthGained = Mathf.Max(playerActor.maxHealth, playerActor.maxHealth + 5);
		OpenMessageWindow("Player has gained " + healthGained + " HP.");
		playerActor.maxHealth = healthGained;

		// Reset treasure
		foreach (GameObject treasure in treasures) {
			treasure.tag = "Treasure";
		}

		yield return new WaitForSeconds(1);
		Camera.main.transform.position = new Vector3(80, 24, -100);

		playerActor.ResetHealth();
		statsWindow.UpdateStats();
	}

	public void Move(string direction) {
		// Track the tile to move to incase its not valid
		if (ableToMove) {
			GameObject nextTile;
			switch (direction) {
				case "Up":
					if (currentTile.y < Grid.gridY) {
						// Pull the tile from the grid and make sure its valid
						nextTile = Grid.grid[(int)currentTile.x, (int)currentTile.y + 1];
						if (nextTile.tag == "ClearTile" || nextTile.tag == "Treasure" || nextTile.tag == "Castle") {
							// Clear to move
							startPosition = Grid.grid[(int)currentTile.x, (int)currentTile.y].transform.position;
							endPosition = nextTile.transform.position;
							currentTile = new Vector2(currentTile.x, currentTile.y + 1);
							moveTime = 0;
							playerMoving = true;
						}
					}
					break;
				case "Down":
					if (currentTile.y > 0) {
						// Pull the tile from the grid and make sure its valid
						nextTile = Grid.grid[(int)currentTile.x, (int)currentTile.y - 1];
						if (nextTile.tag == "ClearTile" || nextTile.tag == "Treasure" || nextTile.tag == "Castle") {
							// Clear to move
							startPosition = Grid.grid[(int)currentTile.x, (int)currentTile.y].transform.position;
							endPosition = nextTile.transform.position;
							currentTile = new Vector2(currentTile.x, currentTile.y - 1);
							moveTime = 0;
							playerMoving = true;
						}
					}
					break;
				case "Right":
					if (currentTile.x < Grid.gridX) {
						// Pull the tile from the grid and make sure its valid
						nextTile = Grid.grid[(int)currentTile.x + 1, (int)currentTile.y];
						if (nextTile.tag == "ClearTile" || nextTile.tag == "Treasure" || nextTile.tag == "Castle") {
							// Clear to move
							startPosition = Grid.grid[(int)currentTile.x, (int)currentTile.y].transform.position;
							endPosition = nextTile.transform.position;
							currentTile = new Vector2(currentTile.x + 1, currentTile.y);
							moveTime = 0;
							playerMoving = true;
						}
					}
					break;
				case "Left":
					if (currentTile.x > 0) {
						// Pull the tile from the grid and make sure its valid
						nextTile = Grid.grid[(int)currentTile.x - 1, (int)currentTile.y];
						if (nextTile.tag == "ClearTile" || nextTile.tag == "Treasure" || nextTile.tag == "Castle") {
							// Clear to move
							startPosition = Grid.grid[(int)currentTile.x, (int)currentTile.y].transform.position;
							endPosition = nextTile.transform.position;
							currentTile = new Vector2(currentTile.x - 1, currentTile.y);
							moveTime = 0;
							playerMoving = true;
						}
					}
					break;
			} 
		}
	}

	public void BeginCombat() {
		// TODO display message before combat opens
		// Grab reference to the combat window and make player busy
		combatWindow = windowManager.Open((int)Windows.CombatWindow - 1, false) as CombatWindow;
		combatWindow.endCombatCallback += CombatOver;
		combatWindow.BeginCombat(playerActor);
		isPlayerBusy(true);
	}

	public void EndCombat() {
		combatWindow.Close();
		isPlayerBusy(false);
	}

	private void CombatOver(bool playerWin) {
		EndCombat();

		// Player died
		if (!playerWin) {
			// End Game
			StartCoroutine(Exit());
		}
	}

	private IEnumerator Exit() {
		OpenMessageWindow("Player has died.");

		yield return new WaitForSeconds(2);
		windowManager.Open((int)Windows.GameOverWindow - 1);
	}

	private void isPlayerBusy(bool value) {
		ableToMove = !value;
		Camera.main.GetComponent<PanCamera>().enabled = !value;
	}

	private MessageWindow OpenMessageWindow(string message) {
		MessageWindow window = windowManager.Open((int)Windows.MessageWindow - 1, false) as MessageWindow;
		window.text = message;
		return window;
	}

	private StatsWindow OpenStatsWindow() {
		statsWindow = windowManager.Open((int)Windows.StatsWindow - 1, false) as StatsWindow;
		statsWindow.player = gameObject.GetComponent<Actor>();
		statsWindow.UpdateStats();
		return statsWindow;
	}

	private void moveplayer() {
		gameObject.transform.position = Grid.grid[(int)currentTile.x, (int)currentTile.y].transform.position;
	}
}
