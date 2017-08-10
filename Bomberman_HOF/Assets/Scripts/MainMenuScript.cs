using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour {

	[SerializeField]
	private SoundManager soundManager;

	[SerializeField]
	private GameObject MainMenuPanel;

	[SerializeField]
	private GameObject OptionsPanel;

	public void NewGameButtonPressed(string newGameLevel)
	{
		soundManager.StopEverySound ();

		SceneManager.LoadScene (newGameLevel);
	}

	public void OptionsButtonPressed()
	{
		MainMenuPanel.SetActive (false);
		OptionsPanel.SetActive (true);
	}

	public void BackToMenuButtonPressed()
	{
		MainMenuPanel.SetActive (true);
		OptionsPanel.SetActive (false);
	}

	public void ExitButtonPressed()
	{
		soundManager.StopEverySound ();

		Application.Quit ();
	}

	void Start()
	{
		OptionsPanel.SetActive (false);
	}
}
