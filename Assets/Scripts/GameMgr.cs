using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour {

	//status!
	public int bombs = 15;
	public int score = 0;

	// not sure I need these
	//public GameObject gropsPrefab;
	//public GameObject cloneGrops;
	public GameObject PlanePrefab; 
	public GameObject clonePlane;
	public GameObject ShoePrefab;
	public GameObject cloneShoe;
//	private ShoeScript ShoeScriptInstance;		//to invoke the shoe reset? ick

	//let's try handling the grops as plain old game objects.
	//they'll get a tag or a number for which row they belong to, and that
	//will determine their direction, speed, and point value.
	//note that xform y for blue row is -1.177, second is -2.077, third is -2.977 - will encode in 
	//grop
	public GameObject gropPrefab;
	public GameObject[] grops;

	//Counters for score and shoes
	public GameObject CounterPrefab;
	public GameObject counterScore;
	public GameObject counterShoes;
	private CounterScript counterScoreScriptInstance;
	private CounterScript counterShoesScriptInstance;

	//singleton game manager
	public static GameMgr instance = null;

	//other statics kept here for convenience
	public static float ShoeResetDelay = 1.0f;

	//on mobiles, allow for the explicit buttons
	public GameObject DroidButtonsPrefab;
	public GameObject droidButtonsClone;
	public Collider2D fireCollider;
	public Collider2D exitCollider;

	// Use this for initialization
	void Awake () {
		//enforce GameMgr singleton status.
		if (instance == null) {
			//print ("YAY I am the isntance!");
			instance = this;			//if there's no game mgr yet, this is the one
		}
		else if (instance != this)
			Destroy (gameObject);		//if there is, and it's not us, scrub.

		//Set GameMgr to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
		//YANKED FROM SOUNDMANAGER- DO I NEED THIS? Probably not - in the vid it shows for soundmanager that it
		//lets a looping song keep going during scene reloads (new levels.)
		//ARE THESE SINGLETONS GAME-WIDE OR SCENE-ONLY?
		//DontDestroyOnLoad (gameObject);

		//if we get this far, time to set up our stuff.
		//IS THIS THE RIGHT PLACE TO PUT THIS?
		Setup();
	}

	public void Setup() {
		//instantiate the plane at its initial position. GameMgr's xform should be 0,0,0
		//using clonePlane here gives us a handle on it and we can destroy it and stuff.
		//might want that for an attract mode or something - we'll see how to handle multiple scenes, I guess
		clonePlane = Instantiate(PlanePrefab, transform.position, Quaternion.identity) as GameObject;
		//cloneGrops = Instantiate (gropsPrefab, transform.position, Quaternion.identity) as GameObject;
		//then add a shoe ready to drop.
		makeNewShoe ();

		//get shoe script here?
		//ShoeScriptInstance = (ShoeScript)cloneShoe.GetComponent(typeof(ShoeScript));
		//print ("DEBUG: shoe script is named " + ShoeScriptInstance.name);

		//create the grops: 3 rows, 2 grops each; GropScript InitSetRowNRank knows how to position them and all
		grops = new GameObject[6];

		for (int row = 0; row < 3; row++) {
			for (int whichgrop = 0; whichgrop < 2; whichgrop++) {
				grops [(2 * row) + whichgrop] = (GameObject)Instantiate (gropPrefab, transform.position, Quaternion.identity);
				GropScript gs = (GropScript)grops [(2 * row) + whichgrop].GetComponent(typeof(GropScript));
				gs.InitSetRowNRank (row, whichgrop);

				//I guess set in motion here and see what happens - this should all run fast enough that it's not a problem
				//i.e. no calls to fixedupdate in the meantime
				gs.SetInMotion();
			}
		}

		//create the score and shoe counters.
		//Mocked it up for score and shoes, and the score one has a top level transform of -4.4, 2 
		//Tens is 0,0 relative to it - I think I'll get rid of that level of hierarchy and just put the digit sprites directly in
		//Ones is 0.65, 0 relative to it
		//Sticker is -0.028, -0.609
		//Shoes main xform is -4.4, 0.7
		//Sticker .599, -.886
		//these main xforms are a bit wrong - maybe the prefab is center-pivot even though it doesn't look like it if I drag one into the Hierarchy? knocking 1 off x looks good

		//ok, for some reason only the first one of these works, whichever one comes first.
		//sticker is always score.
		//aha, problem was that I needed to do the setup (sprite load) in CounterScript in Awake bc this is called at Awake-time, and so if sprites were loaded
		//in start() they were uninitialized when this happened.
		counterShoes = (GameObject)Instantiate(CounterPrefab, new Vector3(transform.position.x -3.4f, transform.position.y + 0.7f, transform.position.z - 0.2f), Quaternion.identity);
		counterShoesScriptInstance = (CounterScript)counterShoes.GetComponent (typeof(CounterScript));
		counterShoesScriptInstance.setSticker (1, 0.599f, -0.886f);
		counterShoesScriptInstance.SetValue (bombs);

		counterScore = (GameObject)Instantiate(CounterPrefab, new Vector3(transform.position.x -3.4f, transform.position.y + 2.0f, transform.position.z - 0.2f), Quaternion.identity);
		counterScoreScriptInstance = (CounterScript)counterScore.GetComponent (typeof(CounterScript));
		counterScoreScriptInstance.setSticker (0, -0.028f, -0.609f);
		counterScoreScriptInstance.SetValue (score);

		//if on mobile, set up explicit buttons for fire and exit
		#if (UNITY_ANDROID || UNITY_IPHONE)
		droidButtonsClone = (GameObject)Instantiate(DroidButtonsPrefab, new Vector3(0.0f, 0.0f, -1.0f), Quaternion.identity);
		exitCollider = droidButtonsClone.transform.Find("Exit").gameObject.GetComponent<Collider2D>();
		fireCollider = droidButtonsClone.transform.Find("Fire").gameObject.GetComponent<Collider2D>();
		#endif
	}

	//called once per frame
	void Update() {
		//check for exit. On mobile platforms, use the explicit exit button.
		//on other platforms (and in editor), use cancel (Esc on PC, B button on X360, etc)
		bool exitRequested = false;
		#if (!UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE))
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) //touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
			{
				//time for some PtInRect!
				//from http://answers.unity3d.com/questions/652927/touch-input-on-sprites.html, this does work.
				Vector3 wp = Camera.main.ScreenToWorldPoint(touch.position);
				Vector2 touchPos = new Vector2(wp.x, wp.y);
				//check fire before exit; if player touches both simultaneously, assume fire. Shoe code will catch that
				//hm. actually, this only guards against the SAME touch being in both fire and exit - and if an earlier touch
				//in the list is on exit, it'll bail. Not too worried but be aware of this issue for games where I do care.
				if (fireCollider == Physics2D.OverlapPoint(touchPos))
				{
					//shoe script will handle; just skip any possible break
					break;
				}
				else if (exitCollider == Physics2D.OverlapPoint(touchPos))
				{
					exitRequested = true;
				}
			}
		}
		#else
		if (Input.GetButtonDown ("Cancel"))
		{
		exitRequested = true;
		}
		#endif

		if (exitRequested) {
			//should come up with some kind of effect
			if (cloneShoe != null) {
				Destroy (cloneShoe);
			}

			//drop to menu pretty quickly
			Invoke ("Reset", 0.5f);
		}
	}

	void Reset()
	{
		//this doesn't destroy this object, does it? swh if we set instance = null
		instance = null;
		UnityEngine.SceneManagement.SceneManager.LoadScene ("MenuScene");
	}

	void DecrementBombs () {
		bombs--;
		//print ("Now you have " + bombs + " bombs! I  mean shoes.");
		// HERE UPDATE DISPLAY STUFF
		counterShoesScriptInstance.SetValue(bombs);
		// don't call checkGameOver because it'll actually stop the game.
	}

	public void makeNewShoe() {
		cloneShoe = Instantiate (ShoePrefab, transform.position, Quaternion.identity) as GameObject;
	}

	public void resetShoeFloor() {
		//called when shoe has hit the floor - make the pish sound, wait a tick, pop back up to the plane.
		//this is gross, but I think the best way to do this is for the shoe to handle its own reparenting and such.
		//USEFUL TO DO HERE BECAUSE CAN DECREMENT NUMBER OF BOMBS and check for game over.
		//print("ResetShoeFloor!");
		Destroy (cloneShoe);
		DecrementBombs();
		//game is over when all bombs are dropped
		if (bombs < 1) {
			//Do game over stuff incl destroy the shoe? If we destroy the plane, does the shoe go away too?
			//but in here we actually stop the game. So don't call it until everything else is done.
			//I think all we need to do to make the game non-interactable is to destroy the shoe.
			//actually instead we just don't re-create it if bombs < 1
			Invoke ("Reset", 5.0f);
		} else {
			//still rolling? then wait a bit and set up a new shoe.
			Invoke ("makeNewShoe", ShoeResetDelay);
		}
	}

	public void resetShoeGrop() {
		//called when shoe has hit a grop - make the boom sound, add score, wait a tick, pop back up to the plane.
		//may need different routines for different types of grop? Also need to change plane speed at 20 and 30 points.
		//DON'T FORGET TO DECK THE NUMBER OF BOMBS.
		//actually not sure we need a different routine
		resetShoeFloor();
	}

	public void addScore(int points) {
		//here add to the score - and check to see if the plane needs to speed up.
		int newscore = score + points;
		//this is gross. But wev.
		if (score < 20 && newscore >= 20) {
			PlaneScript planeScriptInstance = (PlaneScript)clonePlane.GetComponent (typeof(PlaneScript));
			planeScriptInstance.SetSpeed(PlaneScript.planeSpeed20);
		} else if (score < 30 && newscore >= 30) {
			PlaneScript planeScriptInstance = (PlaneScript)clonePlane.GetComponent (typeof(PlaneScript));
			planeScriptInstance.SetSpeed(PlaneScript.planeSpeed30);
		}
		score = newscore;

		//UPDATE DISPLAY
		counterScoreScriptInstance.SetValue(score);
	}
}
