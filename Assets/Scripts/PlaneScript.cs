
using UnityEngine;
using System.Collections;


public class PlaneScript: MonoBehaviour {

	//some nice constants - for now, make them non-static so can change in inspector...?
	//THESE ARE CURRENTLY FOR NON-PHYSICS ENGINE - USE PHYSICS ENGINE
	public static float planeSpeedInit = 2.3f;			//plane speed at game start
	public static float planeSpeed20 = 3.2f;				//speed after 20 points
	public static float planeSpeed30 = 4.3f;				//speed after 30 points
	public static float planeMaxX = 5.5f;
	public float planeY = 2.7f;

	public float planeSpeed;

	private Rigidbody2D rb;

	//plane position - I think move from -5.5 to 5.5 (planeMaxX) then wrap around. Plane can't drop a shoe when
	//the shoe is offscreen - think re: how to do that.
	//private Vector3 planePos;

	// Use this for initialization
	// try awake instead of start because this needs to happen quick
	void Awake () {
		//plane goes faster at 20 points and 30 points,so we want planeSpeed to be variable.
		SetSpeed(planeSpeedInit);
		//see if this makes it not start too low
		transform.position = new Vector2(-planeMaxX,planeY);
	}

	public void SetSpeed(float nuspeed) {
		//let's try using the physics engine to move this thing
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = new Vector2 (nuspeed, 0.0f);
		planeSpeed = nuspeed;
	}
	
	// Update is called once per frame
	// let's use FixedUpdate because it's timing based and frames aren't. Should be smoother or at least the hitches look better
	void FixedUpdate () {
		//plane moves at constant speed - part of the charm of this oldie.
		//although the speed changes at 20 and 30 points, we'll figure that out later.
		//here, we recognize that the plane only moves rightward, so the only bounds check we need is on the + end.
		//float xPos = transform.position.x + planeSpeed;
		if (transform.position.x > planeMaxX) {
			transform.position = new Vector2(-planeMaxX,planeY);
		}
		//planePos.x = xPos;
		//transform.position = planePos;
	}
}
