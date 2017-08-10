using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void BombExploded(GameObject bomb);

public class BombSpawnerScript : MonoBehaviour {

	public bool CanSpawn { get; private set; }
	public int BombsLeft { get { return (CanSpawn)? MaxBombsAtTheSameTime - ActiveBombs.Count : -1; } }

	[SerializeField]
	private GameObject BombPrefab;

	[SerializeField]
	private float FuseLength;

	[SerializeField]
	private float BombRange;

	[SerializeField]
	private float BombCooldown;

	[SerializeField]
	private int MaxBombsAtTheSameTime;

	private List<GameObject> ActiveBombs;
	private bool NoPlaceToSpawnBomb;
	private float cooldown;

	private InGameUIScript UIControls;

	// Use this for initialization
	void Start () 
	{
		ActiveBombs = new List<GameObject> ();

		NoPlaceToSpawnBomb = false;
		cooldown = 0f;

		UIControls = GameObject.Find("UI Canvas").GetComponent<InGameUIScript> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		CanSpawn = 
			!NoPlaceToSpawnBomb &&
			ActiveBombs.Count < MaxBombsAtTheSameTime &&
			cooldown <= 0;

		if (cooldown > 0) 
		{
			cooldown -= Time.deltaTime;
		}

		if (CanSpawn && Input.GetAxis ("Fire1") != 0) {
			SpawnBomb ();
		} /*else if (!CanSpawn) {
			if (!NoPlaceToSpawnBomb)
				Debug.Log ("No place");

			if (ActiveBombs.Count >= MaxBombsAtTheSameTime)
				Debug.Log ("No bombs left");

			if (cooldown > 0)
				Debug.Log ("Cooldown");
			
		}*/

		UIControls.UpdateCounter (BombsLeft);
	}

	void OnTriggerEnter(Collider collision)
	{
		NoPlaceToSpawnBomb = true;
	}

	void OnTriggerExit(Collider collision)
	{
		NoPlaceToSpawnBomb = false;
	}

	private void CleanupBombList(GameObject bomb)
	{
		if (ActiveBombs != null) 
		{			
			if (!ActiveBombs.Remove (bomb)) {
			} else 
			{
				
			}
		}
	}

	private void SpawnBomb()
	{
		GameObject bomb = Instantiate (BombPrefab, transform.position, new Quaternion ());

		bomb.GetComponent<BombScript>().SetBombRange (BombRange);
		bomb.GetComponent<BombScript>().SetFuseLength (FuseLength);
		bomb.GetComponent<BombScript>().OnBombExploded += CleanupBombList;

		bomb.GetComponent<BombScript>().SetOff ();

		ActiveBombs.Add (bomb);

		cooldown += BombCooldown;
	}
}
