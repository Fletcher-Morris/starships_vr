using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVisibleSkins : MonoBehaviour
{
	public LayerMask[] masks;
	public int currentMask = 0;
	public Camera myCam;
	public Camera viewCam;

	void start(){
		myCam.cullingMask = masks [0];
	}

	public void NextMask(){
		if (currentMask < masks.Length - 1) {
			currentMask++;
		} else {
			currentMask = 0;
		}

		myCam.cullingMask = masks [currentMask];
		viewCam.cullingMask = masks [currentMask];
	}

	public void PrevMask(){
		if (currentMask > 0) {
			currentMask--;
		} else {
			currentMask = masks.Length - 1;
		}

		myCam.cullingMask = masks [currentMask];
		viewCam.cullingMask = masks [currentMask];
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			NextMask ();
		}
	}
}