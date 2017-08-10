using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour {

	public event BombExploded OnBombExploded;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioClip TickingSound;

	[SerializeField]
	private AudioClip ExplosionSound;

	[SerializeField]
	private SoundManager soundManager;

	private float BombRange;
	private float FuseLength;

	public void SetBombRange(float range) 
	{
		BombRange = range + 0.5f;
	}

	public void SetFuseLength(float time)
	{
		FuseLength = time;
	}

	public void SetOff()
	{
		StartCoroutine ("WaitForDetonation");
	}

	// Use this for initialization
	void Start () 
	{
		soundManager = SoundManager.GetSoundManager ();
		soundManager.RegisterAudioSource (audioSource, gameObject.transform);

		audioSource.clip = TickingSound;
		soundManager.PlayAudioSource (gameObject.transform);
	}
	
	private IEnumerator WaitForDetonation()
	{
		yield return new WaitForSeconds (FuseLength);

		Detonate ();
		Debug.Log ("Boom");

		if (OnBombExploded != null) 
		{
			OnBombExploded (gameObject);
		}

		while (audioSource.isPlaying)
		{
			yield return new WaitForSeconds (0.2f);
		}						

		soundManager.DeregisterAudioSource (gameObject.transform);	
		Destroy (gameObject);
	}

	private void Detonate ()
	{
		audioSource.Stop();
		audioSource.clip = ExplosionSound;
		soundManager.PlayAudioSource (gameObject.transform);
		GetComponent<MeshRenderer> ().enabled = false;
		GetComponent<SphereCollider> ().enabled = false;

		RaycastHit hit;

		Vector3[] directions = new Vector3[8]
			{ 
				new Vector3(1, 0, 0),
				new Vector3(0, 0, 1),
				new Vector3(-1, 0, 0),
				new Vector3(0, 0, -1),
	
				new Vector3(1, 0, 1),
				new Vector3(1, 0, -1),
				new Vector3(-1, 0, 1),
				new Vector3(-1, 0, -1),
			};

		for (int i = 0; i < directions.Length; i++) 
		{
			if (Physics.Raycast (gameObject.transform.position, directions [i], out hit, BombRange)) 
			{
				DamagedByExplosion toDestroy = hit.transform.gameObject.GetComponent<DamagedByExplosion> ();

				if (toDestroy != null) 
				{
					toDestroy.HitByExplosion ();
				}
			}
		}
	}
}
