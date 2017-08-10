using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterScript : MonoBehaviour, DamagedByExplosion {

	public bool Alive { get; private set; }

	[SerializeField]
	private AudioClip MonsterAliveClip;

	[SerializeField]
	private AudioClip DeathClip;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private SoundManager soundManager;

	[SerializeField]
	private float CharacterSpeed;

	[SerializeField]
	private float EnemyMakesSoundDelay;

	[SerializeField]
	private float EnemyChecksForPlayerDelay;

	[SerializeField]
	private Transform PlayerPosition;

	private NavMeshAgent agent;

	private Vector3 LastKnownPlayerPosition;

	public void HitByExplosion()
	{
		//gameObject.SetActive (false);
		StartCoroutine("Kill");
	}

	// Use this for initialization
	void Start () 
	{
		Alive = true;

		audioSource = GetComponent<AudioSource> ();
		audioSource.clip = MonsterAliveClip;

		soundManager = SoundManager.GetSoundManager ();
		soundManager.RegisterAudioSource (audioSource, gameObject.transform);

		PlayerPosition = GameObject.Find ("Player").GetComponent<Transform> ();

		agent = GetComponent<NavMeshAgent> ();
		agent.speed = CharacterSpeed;

		StartCoroutine ("WanderAround");
		StartCoroutine ("MakeSounds");
	}		

	private IEnumerator WanderAround()
	{
		RaycastHit hit;

		yield return new WaitForSeconds (3f);

		while (Alive) 
		{
			if (Physics.Raycast (transform.position, PlayerPosition.transform.position - transform.position, out hit)) 
			{
				if (hit.transform.gameObject.tag.Equals ("Player")) 
				{
					//hit.collision.gameObject.GetComponent<PlayerScript> ().HitByExplosion ();
					LastKnownPlayerPosition = hit.point;
				} 
				else 
				{
					LastKnownPlayerPosition = 
						new Vector3 (Random.Range (-15, 15), 0, Random.Range (-15, 15));
				}
			}

			agent.SetDestination (LastKnownPlayerPosition);

			yield return new WaitForSeconds(EnemyChecksForPlayerDelay * Random.Range(0.9f, 1.1f));
		}
	}

	private IEnumerator MakeSounds()
	{
		while (Alive) 
		{
			yield return new WaitForSeconds (EnemyMakesSoundDelay * Random.Range (0.75f, 1.25f));

			if(Alive) soundManager.PlayAudioSource (gameObject.transform);
			/*
			while (audioSource.isPlaying) 
			{
				yield return new WaitForSeconds (0.5f);
			}*/
		}


	}

	private IEnumerator Kill()
	{
		Alive = false;

		audioSource.clip = DeathClip;
		soundManager.PlayAudioSource (gameObject.transform);
		GetComponent<MeshRenderer> ().enabled = false;

		// musiałem dodać te dwie linijki - Rigidbody zabijało gracza gdy wróg już umarł, Collider nie pozwalał wyminąć "trupa"
		Destroy(GetComponent<Rigidbody> ());
		Destroy(GetComponent<CapsuleCollider> ());

		while (audioSource.isPlaying) 
		{
			yield return new WaitForSeconds (0.3f);
		}

		Destroy (gameObject);
	}
}
