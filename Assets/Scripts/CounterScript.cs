using UnityEngine;
using System.Collections;

public class CounterScript : MonoBehaviour {

	public Transform onesDigit;
	public Transform tensDigit;

	// handy list of sprites to use for digits s.t. digitSprites[n|n=0-9] = image for numeral n
	public Sprite[] digitAndStickerSprites;

	private int value;

	// Use this for initialization
	// doing Awake because setsticker was getting called from GM before this ran
	void Awake () {
		// create the digitSprites array by using sprite with name "Digit_n" for entry n
		//digitSprites = new Sprite[10];
		//for (int j = 0; j < 10; j++) {
		//	digitSprites [j] = Resources.Load("Digit_" +j, typeof(Sprite)) as Sprite;
		//}

		//from http://forum.unity3d.com/threads/mini-tutorial-on-changing-sprite-on-runtime.212619/
		//I had to create a Resources folder and put the sprites folder with the number/sticker sprites in there.
		//swh.
		//if it do nae work, look into http://docs.unity3d.com/ScriptReference/AssetDatabase.LoadAssetAtPath.html - THAT MAY ONLY WORK IN EDITOR...???
		//BUT IT DID WORK!
		digitAndStickerSprites = Resources.LoadAll<Sprite> ("Sprites/NumberNStickerSprites");

		//get handles on our sprite objects' renderers so we can change the digits - might need to get the renderer otf?
		onesDigit = transform.FindChild("OnesDigit");
		tensDigit = transform.FindChild ("TensDigit");

		//and init our counter to blank - not sure this is needed, but wev
		SetValue (-1);
	}

	//THIS IS A HORRIBLE HARDCODE THING THAT ASSUMES THE STICKERS ARE JUST WHATEVER SPRITES ARE AFTER THE FIRST TEN IN digitAndStickerSprites
	//so pass in 0 for score, 1 for shoes.
	public void setSticker(int newsticker, float xoff, float yoff) {
		Transform kid = transform.FindChild ("Sticker");
		kid.GetComponent<SpriteRenderer> ().sprite = digitAndStickerSprites[10+newsticker];
		kid.transform.position = new Vector3(transform.position.x + xoff,transform.position.y + yoff);
	}

	//set value from 00 to 99. Special case -1, just show sticker
	public void SetValue(int nuval) {
		// just dump out if new value isn't a positive 2-digit number or -1
		if (nuval < -1 || nuval > 99)
			return;

		if (nuval == -1) {
			//blank - just use null, I think, don't shut off the renderer
			onesDigit.GetComponent<SpriteRenderer> ().sprite = null;
			tensDigit.GetComponent<SpriteRenderer> ().sprite = null;
		} else {
			value = nuval;

			//and cue up the sprites!
			onesDigit.GetComponent<SpriteRenderer> ().sprite = digitAndStickerSprites [value % 10];
			tensDigit.GetComponent<SpriteRenderer> ().sprite = digitAndStickerSprites [value / 10];
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
