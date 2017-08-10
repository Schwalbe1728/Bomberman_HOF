using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

	public static float? MusicVolume { get; private set; }
	public static float? SoundVolume { get; private set;}
	public static bool? ProximityEffect { get; private set; }

	[SerializeField]
	private AudioSource MusicTrack;

	[SerializeField]
	private float MaxSoundDistance;

	private Transform PlayerPosition;

	private Dictionary<Transform, AudioSource> RegisteredSoundSources;

	public static SoundManager GetSoundManager()
	{
		SoundManager result = null;
		GameObject temp = GameObject.Find ("Sound Manager Object");

		if (temp != null) 
		{
			result = temp.GetComponent<SoundManager> ();
		}

		return result;
	}

	public void SetMusicVolume(float newVol)
	{
		MusicVolume = newVol;

		MusicTrack.Pause ();
		MusicTrack.volume = MusicVolume.Value;
		MusicTrack.UnPause ();
	}

	public void SetSoundVolume(float newVol)
	{
		SoundVolume = newVol;
	}

	public void ToggleProximityEffect(bool value)
	{
		ProximityEffect = value;
	}

	public void RegisterAudioSource(AudioSource source, Transform position)
	{
		if (RegisteredSoundSources == null) 
		{
			RegisteredSoundSources = new Dictionary<Transform, AudioSource> ();
			Debug.LogWarning ("Nowy słownik");
		}

		RegisteredSoundSources.Add (position, source);
	}

	public void RegisterPlayer(AudioSource source, Transform position)
	{
		RegisterAudioSource (source, position);
		PlayerPosition = position;
	}

	public void PlayAudioSource(Transform key)
	{
		float blend = SoundVolume.Value *
						((ProximityEffect.Value)?
							CalculateProximityEffect(key) :
							1);

		RaycastHit hit;

		if (Physics.Raycast (key.position, PlayerPosition.position - key.position, out hit)) 
		{
			if (!hit.transform.gameObject.tag.Equals ("Player")) 
			{
				blend /= 2.75f;
			}
		}			

		if (RegisteredSoundSources.ContainsKey (key)) 
		{
			AudioSource tempSource = RegisteredSoundSources [key];

			tempSource.pitch = Random.Range (0.9f, 1.1f);

			tempSource.PlayOneShot (tempSource.clip, blend);
		}
		else 
		{
			Debug.LogWarning ("SoundManager.PlayAudioSource: Somethings wrong");
		}
	}

	public void DeregisterAudioSource(Transform deregistered)
	{
		if (RegisteredSoundSources.ContainsKey (deregistered)) 
		{
			RegisteredSoundSources.Remove (deregistered);
		}
		else 
		{
			Debug.LogWarning ("SoundManager.DeregisterAudioSource: Somethings wrong");
		}
	}		

	public void PlayMusic()
	{
		if (MusicTrack != null) 
		{
			MusicTrack.volume = MusicVolume.Value;
			MusicTrack.Play ();
		}
	}

	public void StopEverySound()
	{
		if (MusicTrack != null) 
		{
			MusicTrack.Stop ();
		}

		foreach (KeyValuePair<Transform, AudioSource> source in RegisteredSoundSources) 
		{
			source.Value.Stop ();
		}

		//RegisteredSoundSources.Clear ();
	}

	// Use this for initialization
	void Awake () 
	{		
		// first creation
		if (MusicVolume == null || !MusicVolume.HasValue) 
		{
			MusicVolume = 0.5f;
		}

		if (SoundVolume == null || !SoundVolume.HasValue) 
		{
			SoundVolume = 0.5f;
		}

		if (ProximityEffect == null || !ProximityEffect.HasValue) 
		{
			ProximityEffect = true;
		}

		if (PlayerPosition == null) 
		{
			GameObject player = GameObject.Find ("Player");

			if (player != null) 
			{
				PlayerPosition = player.transform;
			}
		}

		if (RegisteredSoundSources == null) 
		{
			RegisteredSoundSources = new Dictionary<Transform, AudioSource> ();
		}

		PlayMusic ();
	}

	private float CalculateProximityEffect(Transform source)
	{
		return Mathf.Max (MaxSoundDistance - Vector3.Distance (source.position, PlayerPosition.position), 0) / MaxSoundDistance;
	}
}
