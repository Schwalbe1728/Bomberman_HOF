using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameUIScript : MonoBehaviour {

	[SerializeField]
	private GameObject MonsterManager;

	private Image DeathFlash;
	private Text BombCounter;
	private Text MonstersLeft;
	private Text YouWin;

	private float flashLeft;

	private float flashingMonstersLeft;
	private float flashesPerSecond;

	private bool gameWon;

	public void UpdateCounter(int left)
	{
		BombCounter.text = (left > -1)? left.ToString () : "X";
	}

	public void PlayerDied()
	{
		StartCoroutine ("DisplayFlash");
	}

	void Start()
	{
		flashLeft = 4f;
		flashingMonstersLeft = 0f;
		flashesPerSecond = 0.05f;

		DeathFlash = GameObject.Find ("DeathFlash").GetComponent<Image>();
		BombCounter = GameObject.Find ("Bombs Left Counter").GetComponent<Text>();
		MonstersLeft = GameObject.Find ("Monsters Left Counter").GetComponent<Text> ();
		YouWin = GameObject.Find ("You Win").GetComponent<Text> ();

		StartCoroutine ("UpdateNumberOfMonsters");
	}

	private IEnumerator DisplayFlash()
	{
		while (flashLeft > 0) 
		{
			DeathFlash.color = new Color(DeathFlash.color.r, DeathFlash.color.g, DeathFlash.color.b, (0.45f * flashLeft / 4));
			flashLeft -= 0.11f;

			yield return new WaitForSeconds (0.1f);
		}
	}

	private IEnumerator UpdateNumberOfMonsters()
	{
		while (!gameWon) 
		{
			MonstersLeft.text = "Monsters Left: " + MonsterManager.transform.childCount;

			gameWon = MonsterManager.transform.childCount == 0;

			flashingMonstersLeft += flashesPerSecond * 90 * Time.deltaTime;
			MonstersLeft.color = new Color (1f, 1f, 1f, Mathf.Sin (flashingMonstersLeft));

			yield return new WaitForSeconds (Time.deltaTime);
		}

		StartCoroutine ("FinishGame");
	}

	private IEnumerator FinishGame()
	{
		BombCounter.enabled = false;
		MonstersLeft.enabled = false;

		while (YouWin.color.a < 0.98f) 
		{
			yield return new WaitForSeconds (Time.deltaTime);

			YouWin.color = new Color (1f, 1f, 1f, YouWin.color.a + 0.33f * Time.deltaTime);
		}

		yield return new WaitForSeconds (4f);

		SceneManager.LoadScene ("Main menu");
	}
}
