using UnityEngine;
using System.Collections;

public class Floorhit : MonoBehaviour {

	public AudioClip PishNoise;

	void Start() {
		//print ("Floorhit start!");
	}

	//called when the shoe has fallen past all the grops and hit the offscreen kill quad.
	//non-trigger collision... this isn't getting called either. NEED TO USE THE 2D VERSION
	//IF USING 2D COLLIDERS
	//all this has to do is reset the shoe.
	//void OnCollisionEnter2D ()
	//{
	//	//print ("OW! THE SHOE! non-trigger");
	//	GameMgr.instance.resetShoeFloor ();
	//}

	//called when the shoe has fallen past all the grops and hit the offscreen kill quad.
	//all this has to do is reset the shoe.
	void OnTriggerEnter2D (Collider2D col)
	{
		//play the pish noise
		SoundManager.instance.PlaySingle(PishNoise);

		//reset the shoe
		GameMgr.instance.resetShoeFloor ();
	}

}
