using UnityEngine;
using System.Collections;

public class MenuGM : MonoBehaviour {

	//singleton game manager
	public static MenuGM instance = null;

	//Counters for score and shoes
	public GameObject CounterPrefab;
	public GameObject counterScore;
	public GameObject counterShoes;
	private CounterScript counterScoreScriptInstance;
	private CounterScript counterShoesScriptInstance;

	//on mobiles, allow for the explicit buttons
	//would do conditional compile but it looks like that might get confused and forget the prefab setting when
	//I change platforms - but I may just have forgotten to save the scene. Anyway, let's just make it always there
	public GameObject DroidButtonsPrefab;
	public GameObject droidButtonsClone = null;
	//public Bounds fireButtonBounds;
	//public Bounds exitButtonBounds;
	public Collider2D fireCollider;
	public Collider2D exitCollider;

	// Use this for initialization
	void Awake () {
		//enforce MenuGM singleton status.
		if (instance == null) {
			instance = this;			//if there's no game mgr yet, this is the one
		}
		else if (instance != this)
			Destroy (gameObject);		//if there is, and it's not us, scrub.

	  	//do setup
		//kludgy way to show counter stickers with no counter - they come up blank digits
		counterShoes = (GameObject)Instantiate(CounterPrefab, new Vector3(transform.position.x -3.4f, transform.position.y + 0.7f, transform.position.z - 0.2f), Quaternion.identity);
		counterShoesScriptInstance = (CounterScript)counterShoes.GetComponent (typeof(CounterScript));
		counterShoesScriptInstance.setSticker (1, 0.599f, -0.886f);

		counterScore = (GameObject)Instantiate(CounterPrefab, new Vector3(transform.position.x -3.4f, transform.position.y + 2.0f, transform.position.z - 0.2f), Quaternion.identity);
		counterScoreScriptInstance = (CounterScript)counterScore.GetComponent (typeof(CounterScript));
		counterScoreScriptInstance.setSticker (0, -0.028f, -0.609f);

		//if on mobile, set up explicit buttons for fire and exit
		#if (UNITY_ANDROID || UNITY_IPHONE)
		droidButtonsClone = (GameObject)Instantiate(DroidButtonsPrefab, new Vector3(0.0f, 0.0f, -1.0f), Quaternion.identity);
		//fireButtonBounds = droidButtonsClone.transform.Find("Fire").gameObject.GetComponent<SpriteRenderer>().bounds;
		//exitButtonBounds = droidButtonsClone.transform.Find("Exit").gameObject.GetComponent<SpriteRenderer>().bounds;
		exitCollider = droidButtonsClone.transform.Find("Exit").gameObject.GetComponent<Collider2D>();
		fireCollider = droidButtonsClone.transform.Find("Fire").gameObject.GetComponent<Collider2D>();
		#endif
	}
	
	// Update is called once per frame
	void Update () {
		//on mobiles, use the explicit onscreen buttons - in editor, use PC keys
		#if (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE))
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) //touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
			{
				/* old nonworky
				//time for some PtInRect!
				//assuming that firebuttonbounds and exitbuttonbounds have the same z
				Vector3 touchPoint = new Vector3(touch.position.x, touch.position.y, fireButtonBounds.center.z );

				//check fire before exit; if player touches both simultaneously, assume fire. Shoe code will catch that
				if(fireButtonBounds.Contains(touchPoint)) {
					Invoke ("StartGame", 0.0f);
					break;
				} else if(exitButtonBounds.Contains(touchPoint)) {
					Application.Quit();
				}
				*/
				//from http://answers.unity3d.com/questions/652927/touch-input-on-sprites.html, this does work.
				Vector3 wp = Camera.main.ScreenToWorldPoint(touch.position);
				Vector2 touchPos = new Vector2(wp.x, wp.y);
				//check fire before exit; if player touches both simultaneously, assume fire. Shoe code will catch that
				//hm. actually, this only guards against the SAME touch being in both fire and exit - and if an earlier touch
				//in the list is on exit, it'll bail. Not too worried but be aware of this issue for games where I do care.
				if (fireCollider == Physics2D.OverlapPoint(touchPos))
				{
					Invoke ("StartGame", 0.0f);
					break;
				}
				else if (exitCollider == Physics2D.OverlapPoint(touchPos))
				{
					Application.Quit();
				}

			}
		}
		#else
		if (Input.GetButtonDown ("Fire1")) {
			//start game!
			Invoke ("StartGame", 0.0f);
		} else if (Input.GetButtonDown ("Cancel")) {
			//if Esc is pressed - not sure what does this on a joystick or droid - quit the game.
			//apparently does nothing in editor or web player.
			Application.Quit ();
		}
		#endif
	}

	void StartGame() {
		//I'm assuming this destroys everything including Instance - is it so? I don't think so. See if this helps
		instance = null;
		UnityEngine.SceneManagement.SceneManager.LoadScene ("MainScene");
	}
}
