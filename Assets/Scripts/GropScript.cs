using UnityEngine;
using System.Collections;

public class GropScript : MonoBehaviour {

	public int row;			// 0 = top fast rightward row, 1 = middle middle leftward row, 2 = bottom slow rightward row
	public int which;		// 0 = first grop 1 = second grop
	public float speed;		// amount by which to change x per frame. Negative for leftward
	public bool hidden;
	public bool hit;		// true if the grop has been hit by a shoe
	public Rigidbody2D rb;

	public float leftEdge = -6.0f;		// furthest left a leftgoing grop should go
	public float rightEdge = 6.0f;		// mutatis mutandis, rightgoing


	public static int[] rowPoints = { 1, 2, 4 };		// 1 for row 0, 2 for row 1, 4 for row 2
	public static float[] rowSpeed = { 3.0f, -2.0f, 1.0f };  
	//note that xform y for blue row is -1.177, second is -2.077, third is -2.977
	public static float[] rowY = { -1.177f, -2.077f, -2.977f }; 
	public static float gropSpacing = -6.0f;			//how far apart the grops are in absolute terms

	//squish noise and different pitches at which to play it by row
	public AudioClip SplatNoise;
	public static float[] rowPitch = { 1.0f, 0.75f, 0.5f };

	//normal and squashed graphics
	public Material normalMaterial;
	public Material gloppyMaterial;

	// Use this for initialization1
	void Awake () {
		//start out invisible and uncollideable until everything is settled down
		//I don't think I really need to
		//Hide ();
	}

	public void InitSetRowNRank(int whichRow, int whichGrop) {

		// grop starts unhit
		hit = false;

		//Set initial position by row.
		//if row is 1, start off the right edge, else start off the left edge.
		row = whichRow;
		which = whichGrop;
		float x = (row == 1)?rightEdge:leftEdge;

		if (whichGrop == 1) {
			//each row has 2 grops, 0 and 1, and they're gropSpacing apart (rows 0 and 2, second grop is [currently]
			//3 units left of grop 0, row 1 it's 3 units right [negate gropSpacing.])
			x += (row == 1) ? -gropSpacing : gropSpacing;
		}

		if (row == 1) {
			//middle row, flip texture around
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		} 

		//then set position. 
		transform.position = new Vector2 (x, rowY[row]);
		//print ("InitSetRowNRank whichRow " + whichRow + " whichGrop " + whichGrop + " at " + 
		//	transform.position.x + ", " + transform.position.y);
	}

	public void SetInMotion() {
		//also moving along
		rb = GetComponent<Rigidbody2D>();
		rb.velocity = new Vector2 (rowSpeed[row], 0.0f);

		//and make officially visible
		Restore();
	}

	//squished grop has squish graphics and can't be collided with
	public void Squash() {
		//GetComponent<MeshRenderer> ().enabled = false;
		GetComponent<MeshRenderer> ().sharedMaterial = gloppyMaterial;
		GetComponent<BoxCollider2D> ().enabled = false;
		hidden = true;
	}

	//normal grop has normal graphics and CAN be collided with
	public void Restore() {
		//GetComponent<MeshRenderer> ().enabled = true;
		GetComponent<MeshRenderer> ().sharedMaterial = normalMaterial;
		GetComponent<BoxCollider2D> ().enabled = true;
		hidden = false;
	}

	//FixedUpdate is more regular than update - per update of physics engine, or something
	void FixedUpdate () {
		// row 1, have the grop blink back to the right when it reaches the left; others opposite.
		if (row == 1) {
			if (transform.position.x < leftEdge) {
				transform.position = new Vector2 (rightEdge, transform.position.y);

				//restore grop if squished
				if (hit) {
					Restore ();
					hit = false;
				}
			}
		} else {
			if (transform.position.x > rightEdge) {
				transform.position = new Vector2 (leftEdge, transform.position.y);

				//restore grop if squished
				if (hit) {
					Restore ();
					hit = false;
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		//ok, now the shoe is not the only collider in the mobile versions - so bail if the grop is colliding with something not
		//named shoe (or QuadShoePrefab(Clone), wev.)
		if(col.gameObject.name != "QuadShoePrefab(Clone)") return;
		//print ("OW! THE SHOE!");

		//play splatter noise
		SoundManager.instance.PlaySingleAtPitch(SplatNoise,rowPitch[row]);

		//this shouldn't happen if the grop is already "hit" - it'll be hidden
		//or rather uncollideable and in gloppy state
		Squash();
		hit = true;
		GameMgr.instance.addScore (rowPoints [row]);
		GameMgr.instance.resetShoeGrop ();
	}

}
