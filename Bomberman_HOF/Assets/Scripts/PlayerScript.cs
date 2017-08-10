using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour, DamagedByExplosion {

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private SoundManager soundManager;

	[SerializeField]
	private float CharacterSpeed;

	private InGameUIScript UIControls;
	private CharacterController controller;

	public bool Alive { get; private set; }

	public void HitByExplosion()
	{
		//gameObject.SetActive (false);
		StartCoroutine("KillPlayer");
	}

	// Use this for initialization
	void Start () 
	{		
		Alive = true;

		audioSource = GetComponent<AudioSource> ();

		soundManager = SoundManager.GetSoundManager ();
		soundManager.RegisterPlayer (audioSource, gameObject.transform);

		UIControls = GameObject.Find("UI Canvas").GetComponent<InGameUIScript> ();
		controller = GetComponent<CharacterController> ();

		if (controller == null) 
		{
			Debug.LogWarning ("Controller");
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{		
		if (!Alive)
			return;

		Vector3 movement = CharacterSpeed * Time.deltaTime *
			new Vector3 (
				Input.GetAxis ("Horizontal"),
				0,
				Input.GetAxis ("Vertical"));

		movement = transform.TransformDirection (movement);

		controller.Move (movement);	

		transform.Rotate (0, Input.GetAxis ("Mouse X") * 180f * Time.deltaTime, 0);

		if (Input.GetAxis ("Cancel") != 0) 
		{
			HitByExplosion ();
		}
	}		

	void OnCollisionEnter(Collision collider)
	{
		if (collider.gameObject.tag.Equals ("Enemy") && Alive) 
		{
			Debug.Log ("HitEnemy");

			HitByExplosion ();
		}
	}

	private IEnumerator KillPlayer()
	{
		Debug.Log ("YOU DIED");

		//soundManager.StopEverySound ();
		//soundManager.PlayAudioSource (gameObject.transform);	

		Alive = false;

		audioSource.time = 0.33f;
		audioSource.volume = SoundManager.SoundVolume.Value;
		audioSource.Play ();

		UIControls.PlayerDied ();
		GetComponent<Animator> ().SetTrigger ("PlayerDied");

		while (audioSource.isPlaying) 
		{
			yield return new WaitForSeconds (5.0f);
		}

		SceneManager.LoadScene ("Main menu");
	}
}
