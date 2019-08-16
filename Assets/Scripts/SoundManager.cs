//nicked from https://unity3d.com/learn/tutorials/projects/2d-roguelike/audio
using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour 
{
	public static AudioSource efxSource;                   //Drag a reference to the audio source which will play the sound effects.
	public static AudioSource musicSource;                 //Drag a reference to the audio source which will play the music.
	public static SoundManager instance = null;     //Allows other scripts to call functions from SoundManager.             
	public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
	public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.
	
	
	void Awake ()
	{
		//Check if there is already an instance of SoundManager
		if (instance == null) {
			//if not, set it to this.
			instance = this;
			//and create our music and effects sources.
			//I think this will make it so that those survive scene loads.
			//do I need to do settings on 'em? Do the 2d thing just in case.
			//Could do this with a prefab, I guess
			//This works, but is gross. I should write a better sound manager for real projects.
			SoundManager.efxSource = gameObject.AddComponent<AudioSource>();
			SoundManager.efxSource.spatialBlend = 0.0f;		//set to 2d
			SoundManager.musicSource = gameObject.AddComponent<AudioSource>();
			SoundManager.musicSource.spatialBlend = 0.0f;		//set to 2d
		//If instance already exists:
		} else if (instance != this)
			//Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
			Destroy (gameObject);
		
		//Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
		DontDestroyOnLoad (gameObject);
	}
	
	
	//Used to play single sound clips.
	public void PlaySingle(AudioClip clip)
	{
		//Set the clip of our efxSource audio source to the clip passed in as a parameter.
		SoundManager.efxSource.clip = clip;
		
		//and set pitch to normal
		SoundManager.efxSource.pitch = 1.0f;

		//Play the clip.
		SoundManager.efxSource.Play ();
	}

	//Used to play single sound clips.
	public void PlaySingleAtPitch(AudioClip clip, float pitch)
	{
		//Set the clip of our efxSource audio source to the clip passed in as a parameter.
		SoundManager.efxSource.clip = clip;

		//and set pitch
		SoundManager.efxSource.pitch = pitch;

		//Play the clip.
		SoundManager.efxSource.Play ();
	}

	
	//RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
	public void RandomizeSfx (params AudioClip[] clips)
	{
		//Generate a random number between 0 and the length of our array of clips passed in.
		int randomIndex = Random.Range(0, clips.Length);
		
		//Choose a random pitch to play back our clip at between our high and low pitch ranges.
		float randomPitch = Random.Range(lowPitchRange, highPitchRange);
		
		//Set the pitch of the audio source to the randomly chosen pitch.
		efxSource.pitch = randomPitch;
		
		//Set the clip to the clip at our randomly chosen index.
		efxSource.clip = clips[randomIndex];
		
		//Play the clip.
		efxSource.Play();
	}
}
