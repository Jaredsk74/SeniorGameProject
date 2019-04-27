using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {

	public int health;
	public int maxHealth;
	public int money;
	public int attackMin = 1;
	public int attackMax = 1;

	public bool alive {
		get { return health > 0; }
	}

	public void DecreaseHealth (int amount) {
		// Return whichever is greater, 0 or the health minus amount
		health = Mathf.Max(health - amount, 0);
	}

	public void RestoreHealth (int amount) {
		// Return whichever value is lesser to prevent going over max
		health = Mathf.Min(health + amount, maxHealth);
	}

	public void ResetHealth() {
		health = maxHealth;
	}

	public void AddMoney(int value) {
		money += value;
	}
}
