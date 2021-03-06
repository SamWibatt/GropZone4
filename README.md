
# MIGRATING FROM GITHUB TO GITLAB at https://gitlab.com/SamWibatt/gropzone4 - please go to that repository unless you're already there

# GropZone4
Small Unity project about dropping shoes on grasshoppers from a great height, 1974-style!

* Originally written in mid-2016, this project now loads and runs in Unity 2019.2.1f1 on Ubuntu, at least. I haven't yet tried exporting standalone versions. 

* The big change I had to make was to comment out obsolete platform cases in  Assets/Editor/CrossPlatformInput/CrossPlatformInputInitialize.cs - what, nobody wants a Blackberry version anymore? That file may need to be repaired.

* The DocsNMocks directory contains a design notes file of my stream of consciousness discovery of Unity in the course of writing this little thing. Code comments do similar.

----
## About

Informal writeup from the Facebook post where I premiered this -

So! Grop Zone 4. A video game in which you squash grasshoppers by dropping shoes from an airplane.

![In all its glory](images/screenshot.jpg)

**Pictured: PC Version**

This video-bagatelle is a super simple 70s-style game I wrote, based on a real 70s game I didn't write, to begin familiarizing myself with the Unity game development system (http://unity3d.com/). A couple folks requested copies and have had a good time with the game, so here I will put up links for anyone else wanting to give it a go.

There are versions for Windows, Mac OS X, Linux and Android.

iPhone / iOS is possible to create with Unity but the supplementary software is much harder to get hold of than the Android equivalent. Plus it only runs on Macs, of which I have nary a one. Apologies if that's your platform of choice (Perhaps one of my iOS-dev-capable friends could lend a hand, if there's interest.)

## Notes on gameplay:
- The plane speeds up once you've scored 20 points, and again when you've scored 30.
- If you press Exit during the game (esc key on computers, onscreen exit button on Android), it returns to the menu.
- If you press Exit in the menu, it exits the app.

My best score to date is 50 points out of 60 possible. Check the totally non-photoshopped screenshot!

## Versions

### Windows - (used to have a link to dl)

Download and unzip this file somewhere. Double-click the .exe to play. There is a dialog box offering you a choice of screen resolutions and graphics quality - it's such a simple thing that highest res / best quality ("fantastic") should work on any modern machine.

The ctrl key acts as the "fire" button, and esc is the "exit" button.

### Mac - (used to have a link to dl)

Same as for Windows, I believe. One of my friends has successfully run it.

### Linux - (used to have a link to dl)

X86 architecture. Same instructions as Windows and Mac. I've tried it on Linux Mint / Cinnamon and it worked fine, but only filled the screen if running in the monitor's native resolution; otherwise, the monitor resolution stayed at native and the game appeared in the lower left corner taking up whatever little fraction of the screen you chose for a resolution. YMMV.

### Android

**Should** run on any modern droid. Not quite sure how any individual tablet / phone will handle this; installation is not as friendly as using the Play Store. I downloaded the .apk file linked here and then used a file manager to open it. There was a warning dialog about installing stuff from unknown sources (I promise no malware) and an option to go the the settings menu and install an untrusted app just this once.
The big difference is that there are FIRE and EXIT buttons onscreen that take the places, respectively, of the ctrl and esc buttons on the Mac / PC. (and on devices like my tablet that have a stubbier-than-16:9 display aspect ratio they're cut off a bit)

Also: this runs great on my phone (Samsung Galaxy Alpha), lovely and smooth, but juddery and rocky on my tablet (Samsung Galaxy Tab 4 8.0)

----

The game that inspired this - and inspired me to learn how video games worked, back in the mid-seventies, was "Drop Zone 4" by National Entertainment and Meadows Games. Here's the sales flyer for it: [Image by Dphower at arcade-museum.com](http://flyers.arcade-museum.com/?page=flyer&db=videodb&id=2560&image=1) reproduced here and I hope that's ok

![Drop Zone 4 flyer](images/DropZone4-Dphower.jpg)

---
