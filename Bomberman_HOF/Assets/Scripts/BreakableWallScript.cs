using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWallScript : MonoBehaviour, DamagedByExplosion {

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private SoundManager soundManager;

	public void HitByExplosion()
	{
		//gameObject.SetActive (false);
		gameObject.GetComponent<MeshRenderer>().enabled = false;	

		StartCoroutine ("HandleWallDestroyed");
	}

	// Use this for initialization
	void Start () 
	{
		soundManager = SoundManager.GetSoundManager ();
		soundManager.RegisterAudioSource (audioSource, gameObject.transform);

		//StartCoroutine ("TestSoundSource");
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
		
	// korutyna testowa
	private IEnumerator TestSoundSource()
	{
		yield return new WaitForSeconds (Random.Range (3f, 20f));
		HitByExplosion ();
	}

	private IEnumerator HandleWallDestroyed()
	{		
		soundManager.PlayAudioSource (gameObject.transform);
		soundManager.DeregisterAudioSource (gameObject.transform);

		while (audioSource.isPlaying) 
		{
			yield return new WaitForSeconds (0.3f);
		}			

		Destroy (gameObject);
	}
}
