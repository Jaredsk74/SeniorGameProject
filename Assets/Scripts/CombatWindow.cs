using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CombatWindow : Window {
	// Delegate stuff
	public delegate void EndCombat(bool playerWin);
	public EndCombat endCombatCallback;

	public GameObject[] enemies; // Each type of enemy
	public GameObject buttons; // Actions
	public Text label;
	public bool playerTurn = true;

	// Limits the editor to this range
	[Range(0, .9f)]
	public float runChance = .3f;
	public float critChance = .1f;

	// Hit animation
	public RectTransform windowRect;
	private RectTransform enemyRect;
	private HitAnimation shake;

	// Track actors
	private Actor player;
	private Actor enemy;

	protected override void Awake() {
		// Grab reference
		shake = GetComponent<HitAnimation>();

		base.Awake();
	}

	public override void Open() {
		base.Open();

		// Disable all enemies by default
		foreach (GameObject enemy in enemies) {
			enemy.SetActive(false);
		}

		buttons.SetActive(false);
	}

	// Setup enemy
	public void SetEnemy() {
		// Get a random enemy and enable
		int r = Random.Range(0, enemies.Length);
		enemies[r].SetActive(true);

		// Grab reference to the correct Actor and reset health
		enemy = enemies[r].GetComponent<Actor>();
		enemy.ResetHealth();

		enemyRect = enemy.GetComponent<RectTransform>();
		UpdateEnemyStats();
	}

	public void BeginCombat(Actor player) {
		this.player = player;
		SetEnemy();

		playerTurn = Random.Range(0, 1f) >= .5;
		if (playerTurn) {
			OpenMessageWindow("Incoming Battle!");
		}
		else {
			OpenMessageWindow("Enemy Attacks First!");
		}


		// Prepare next action
		StartCoroutine(NextAction());
	}

	private void OpenMessageWindow(string message) {
		// Grab reference to the message window
		MessageWindow msgWindow = manager.Open((int)Windows.MessageWindow - 1, false) as MessageWindow;
		msgWindow.text = message; // Set the text
	}

	//	public void Action(int selection, Actor actor, Actor receiver) {
	public void Action(Actor actor, Actor receiver, RectTransform target, float duration, float strength) {
		// Perform action
		string message = Attack(actor, receiver, target, duration, strength);

		OpenMessageWindow(message);
		buttons.SetActive(false);

		// Update enemy and player info
		UpdateEnemyStats();
		UpdatePlayerStats();

		// Prepare next action
		StartCoroutine(NextAction());
	}

	// Called for player actions
	public void PlayerAction(int selection) {
		if (selection == 0) { // 0 = Attack
			Action(player, enemy, enemyRect, .5f, 2);
			//shake.Shake(enemyRect, .5f, 2);
		}
		else {
			StartCoroutine(Run());
		}
		playerTurn = false;
	}

	// Called for enemy actions
	public void EnemyAction() {
		Action(enemy, player, windowRect, .85f, 2);
		playerTurn = true;
		//shake.Shake(windowRect, .85f, 2);
	}

	private IEnumerator NextAction() {
		// 2 seconds is a nice delay to display the status of the action
		yield return new WaitForSeconds(2);

		// Check if any of the actors are dead
		if (!player.alive || !enemy.alive) {
			StartCoroutine(CombatOver());
		}
		else {
			// If its the player turn, activate buttons to allow for an action
			if (playerTurn) {
				buttons.SetActive(true);
				OnFocus();
			}
			// Else, let the enemy attack
			else {
				EnemyAction();
			}
		}
	}

	private void UpdateEnemyStats() {
		label.text = enemy.name + "\nHP " + enemy.health.ToString("D2");
	}

	private string Attack(Actor actor, Actor receiver, RectTransform target, float duration, float strength) {
		int power = Random.Range(actor.attackMin, actor.attackMax);

		// Build message to return
		StringBuilder sb = new StringBuilder();

		// Check for critical
		if (power > 0 && Random.Range(0, 1f) <= critChance) {
			sb.Append("Critical hit! ");
			power *= 2;
		}

		receiver.DecreaseHealth(power);

		sb.Append(actor.name);
		sb.Append(" attacks ");
		sb.Append(receiver.name);
		sb.Append(". ");

		if (power > 0) {
			sb.Append(receiver.name);
			sb.Append(" loses ");
			sb.Append(power);
			sb.Append(" HP.");
			shake.Shake(target, duration, strength);
		}
		else {
			sb.Append(actor.name);
			sb.Append(" missed!");
		}


		return sb.ToString();
	}

	private void UpdatePlayerStats() {
		// Grab the stats window and update stats
		(manager.GetWindow((int)Windows.StatsWindow - 1) as StatsWindow).UpdateStats();
	}

	// Called for running
	private IEnumerator Run() {
		// Hide buttons
		buttons.SetActive(false);

		float chance = Random.Range(0, 1f);

		// Check if the player can run
		if (chance < runChance) {
			Debug.Log(chance);
			OpenMessageWindow("Run!");
			yield return new WaitForSeconds(2);

			// Call the delegate
			if (endCombatCallback != null) {
				endCombatCallback(player.alive);
			}
		}
		// Can't run
		else {
			OpenMessageWindow("Unable to run.");
			StartCoroutine(NextAction());
		}
	}

	// Delegate stuff
	private IEnumerator CombatOver() {
		// If player alive, then player won, else enemy won
		string message = (player.alive ? player.name : enemy.name) + " has defeated their opponent.";

		// Gold drop
		int gold = Random.Range(0, enemy.money);
		if (gold > 0 && player.alive) {
			message += " Gained " + gold + " gold.";
			player.money += gold;
			UpdatePlayerStats();
		}

		// Make sure other messages have disappeared
		yield return new WaitForSeconds(2);

		// Call the delegate
		if (endCombatCallback != null) {
			endCombatCallback(player.alive);
		}

		OpenMessageWindow(message);
	}
}
