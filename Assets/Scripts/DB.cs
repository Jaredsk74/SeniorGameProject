using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;

public class DB : MonoBehaviour {

	public Actor Player;
	public Actor Bat;
	public Actor Snake;
	public Actor Ghost;
	public Actor Snail;


	private void Start() {
		string conn = "URI=file:" +	
			System.IO.Path.Combine(Application.streamingAssetsPath, "stats.db");
		//string conn = "URI=file:" + Application.dataPath + "/stats.db"; //Path to database.
		IDbConnection dbconn;
		dbconn = new SqliteConnection(conn);
		dbconn.Open(); //Open connection to the database.
		IDbCommand dbcmd = dbconn.CreateCommand();
		string sqlQuery = "SELECT * " + "FROM stats";
		dbcmd.CommandText = sqlQuery;
		IDataReader reader = dbcmd.ExecuteReader();
		while (reader.Read()) {
			string name = reader.GetString(0);
			int max_hp = reader.GetInt32(1);
			int start_gold = reader.GetInt32(2);
			int min_atk = reader.GetInt32(3);
			int max_atk = reader.GetInt32(4);

			switch (name) {
				case "Player":
					Player.maxHealth = max_hp;
					Player.money = start_gold;
					Player.attackMin = min_atk;
					Player.attackMax = max_atk;
					break;
				case "Bat":
					Bat.maxHealth = max_hp;
					Bat.money = start_gold;
					Bat.attackMin = min_atk;
					Bat.attackMax = max_atk;
					break;
				case "Snake":
					Snake.maxHealth = max_hp;
					Snake.money = start_gold;
					Snake.attackMin = min_atk;
					Snake.attackMax = max_atk;
					break;
				case "Ghost":
					Ghost.maxHealth = max_hp;
					Ghost.money = start_gold;
					Ghost.attackMin = min_atk;
					Ghost.attackMax = max_atk;
					break;
				case "Snail":
					Snail.maxHealth = max_hp;
					Snail.money = start_gold;
					Snail.attackMin = min_atk;
					Snail.attackMax = max_atk;
					break;
				default:
					Debug.Log("Missing name");
					break;
			}
		}
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		dbconn.Close();
		dbconn = null;
	}
}
