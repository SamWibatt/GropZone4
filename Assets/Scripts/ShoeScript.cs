using UnityEngine;
using System.Collections;

public class ShoeScript : MonoBehaviour {

	private Rigidbody2D rb;
	//private Rigidbody2D parentrb;
	private bool shoeFalling;
	private PlaneScript parentScript;
	private float parentVelocity;

	//make the shoe wrap around if it's gone off the right edge of the screen (see FixedUpdate)
	public static float screenEdge = 4.8f;	

	//shoe position relative to parent plane's transform
	public float ShoeOffsetX = 0.43f;
	public float ShoeOffsetY = -0.29f;

	//shoe drop sound
	public AudioClip DropNoise;

	//for mobiles, fire button collider
	public Collider2D fireCollider;


	// Use this for initialization
	// called when object is instantiated
	// ok, now the shoe is no longer a child of the plane by default.
	void Awake () {
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start () {
		//may need to do this not in awake bc gm's awake is where cloneplane is set up and
		//it needs to exist by now for this to work
		shoeFalling = false;
		transform.parent = GameMgr.instance.clonePlane.transform;		//hopework
		transform.position = new Vector2(transform.parent.position.x + ShoeOffsetX, transform.parent.position.y + ShoeOffsetY);
		parentScript = (PlaneScript)GetComponentInParent (typeof(PlaneScript));
		//print ("DEBUG: shoe parent script is named " + parentScript.name);
		//print ("DEBUG: parent transform after doing parent thing is " + transform.parent.position.x + ", " + transform.parent.position.y);
		//print ("DEBUG: shoe transform after doing parent thing is " + transform.position.x + ", " + transform.position.y);

		//HOW DO I GET FIRE BUTTON BOUNDS? Fish it out of gm script, I guess
		//this is so gross
		#if (UNITY_ANDROID || UNITY_IPHONE)
		foreach(GameObject rootobjy in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()) {
			if(rootobjy.name == "GM") {
				GameMgr gmgr = (GameMgr)rootobjy.GetComponent (typeof(GameMgr));
				fireCollider = gmgr.fireCollider;
				break;
			}
		}
		#endif
	}

	//FixedUpdate is more time-consistent than Update; let's use it to wrap the shoe around if it's off the screen.
	//might look a bit wonky because the plane doesn't wrap immediately, but wev
	void FixedUpdate() {
		if (shoeFalling == true && transform.position.x >= screenEdge) {
			transform.position = new Vector2 (-screenEdge, transform.position.y);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//if shoe is not falling and fire button pressed, unparent it so it no longer moves with the plane and
		//starts falling.
		//LATER might want to have it so it has the plane's forward velocity when it falls? Probably always want that.
		//how do I get parent velocity? Fetch a ref to the script object in Awake() - might want to do in a Start()? 
		//but it's expensive so don't do it lots - then grab out planeSpeed.
		//also make is so that you can't drop the shoe unless it's onscreen (within the bezel, which reaches from -screenEdge to
		//screenEdge in x.)
		bool dropRequested = false;

		//for mobiles, use the fire button on screen; else use actual fire button e.g. ctrl or A button on stick
		//but in editor, use the PC buttons because it can't do touch
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
					dropRequested = true;
					break;
				}
			}
		}
		#else
		if (Input.GetButtonDown ("Fire1")) {
			dropRequested = true;
		}
		#endif

		if (dropRequested && shoeFalling == false && 
			transform.position.x > -screenEdge && transform.position.x < screenEdge) { 
			//parentrb = GetComponentInParent<Rigidbody2D>();		//not supposed to do this every frame but didn't work to do it just once
			parentVelocity = parentScript.planeSpeed; //parentrb.velocity.x; didn't work - got 0
			//print ("parent velocity is " + parentVelocity);
			transform.parent = null;
			shoeFalling = true;
			rb.isKinematic = false;
			//see if this makes the shoe move in x at the speed of the plane - noop
			//or actually, it does, but the planespeed isn't meaningful to the physics engine.
			//CURRENTLY KLUDGED TO MULTIPLY BY 100 - WHAT I SHOULD DO IS USE THE PHYSICS ENGINE TO MOVE THE PLANE.
			//and now I've done that, let's swh
			rb.velocity = new Vector2(parentVelocity, 0.0f);

			//play falling noise
			SoundManager.instance.PlaySingle(DropNoise);
		}
	}

}
