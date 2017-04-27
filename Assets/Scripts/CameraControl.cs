﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraControl : MonoBehaviour {


	Image titleText;
	Text instructionText;
	Text startText;
	public GameObject player;

	private Vector3 offset;

//	public float MoveSpeed = 10f;

	private Vector3 playerVelocity;

	public float minAngle;

	public float maxAngle;

	public float maxDistance;

	public float minDistance;

	public float cameraCorrectionSpeed;

	private bool begun;

	float arcPoint;

	public float verticalOffset;
	public float lookVerticalOffset;
	
	// Use this for initialization

	IEnumerator Start () 
	{
		blackCurtainControl blackCurtain = GameObject.Find("blackCurtain").GetComponent<blackCurtainControl>();
		Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Ground");
		Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Eel");
		instructionText = GameObject.Find("introText").GetComponent<Text>();
		titleText = GameObject.Find ("TitleImage").GetComponent<Image> ();
		startText = GameObject.Find ("startText").GetComponent<Text> ();
		startText.enabled = false;
		// Fade in player
	    yield return StartCoroutine(blackCurtain.FadeToColor(Color.clear, 1)); // should be active
		// Player visible
		yield return new WaitForSeconds(3);
		// Fade out screen
		yield return StartCoroutine(blackCurtain.FadeToColor(Color.black, 1)); 
		Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Ground");
		Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Eel");

		// Fade in Everything
		StartCoroutine(blackCurtain.FadeToColor(Color.clear, 3));
		StartCoroutine(FadeInTitleElements());

		for(float t = 0; t < 1; t += 0.5f*Time.deltaTime) {
			instructionText.color = Color.Lerp(Color.white, Color.clear, t); // should be from white
			yield return null;
		}

		StartCoroutine(FadeInTitleElements()); // should be out
		startText.enabled = true;
		begun = false;
		player = GameObject.Find ("PlayerShell");
		offset = transform.position - player.transform.position;
		while(!Input.anyKey) {
			yield return null;
		}
		StartCoroutine (FadeOutTitleElements ());
		StartCoroutine(GameStartAmplitude());
		StartCoroutine(MoveTowardsPlayer());
//		begun = true;
//		playerVelocity = new Vector3 (0, 0, 10);
	}

	void Update(){

	}

	void LateUpdate () 
	{ 
		if(begun) {

			Vector3 newPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
			if(Vector3.Distance(transform.position, newPos) > maxDistance){
				transform.position = Vector3.Slerp(transform.position, newPos, 0.01f);
			}
			else if(Vector3.Distance(transform.position, newPos) < minDistance){
				transform.position = Vector3.SlerpUnclamped(newPos, transform.position, 1.005f);
			}
/*			RaycastHit hit;
			Ray shootingRay = new Ray(transform.position, Vector3.down);
			if(Physics.Raycast(shootingRay, out hit, 100)) {
				Vector3 verticalVector = new Vector3(transform.position.x, hit.point.y, transform.position.z);
				if(GroundSinControl.CalculateSinPosition(transform.position) + 2.4f > transform.position.y) {
					transform.position = Vector3.SlerpUnclamped(verticalVector, transform.position, 1.015f);
				}
				else if(GroundSinControl.CalculateSinPosition(transform.position) + 2.0f < transform.position.y) {
					transform.position = Vector3.Slerp(transform.position, verticalVector, 0.009f);
				}
				else if(Vector3.Angle(Vector3.down, player.transform.position - transform.position) < minAngle) {
					transform.position = Vector3.Slerp(transform.position, verticalVector, cameraCorrectionSpeed);
				}
				else if(Vector3.Angle(Vector3.down, player.transform.position - transform.position) > maxAngle){
					transform.position = Vector3.SlerpUnclamped(verticalVector, transform.position, 1 + cameraCorrectionSpeed);
				}
			}*/
			Vector3 anchorPos = RotatePointAroundPivot(transform.position, player.transform.position + new Vector3(0, player.transform.position.y + 4, -2), 5 * Time.deltaTime);
			if(GroundSinControl.CalculateSinPosition(transform.position) + 2.4f > transform.position.y) {
					transform.position = Vector3.SlerpUnclamped(anchorPos, transform.position, 0.015f);
			}
			else if(Vector3.Angle(Vector3.down, player.transform.position - transform.position) < minAngle) {
				anchorPos = RotatePointAroundPivot(transform.position, player.transform.position + new Vector3(0, player.transform.position.y, -2), 5 * Time.deltaTime);
					transform.position = Vector3.Slerp(transform.position, anchorPos, 0.02f);
			}
			else if(Vector3.Distance(transform.position, player.transform.position) > 2) {
				transform.position = Vector3.Slerp(transform.position, anchorPos, 0.01f);
			} 
			else if(Vector3.Distance(transform.position, player.transform.position) < 1f || player.transform.position.y + verticalOffset > transform.position.y) {
				transform.position = Vector3.Slerp(anchorPos, transform.position, 0.01f);
			}

			Vector3 curRotation = player.transform.rotation.eulerAngles;
			transform.rotation = Quaternion.Euler(0, curRotation.y, 0);
			transform.LookAt(new Vector3(player.transform.position.x, player.transform.position.y + lookVerticalOffset, player.transform.position.z));
			//Debug.Log(Vector3.Angle(Vector3.down, player.transform.position - transform.position));
		}
	}

	IEnumerator FadeOutTitleElements() {
		Color startColorTitle = titleText.color;

		Color startColorInstruction = instructionText.color;

		for (float t = 0; t <= 1; t += 0.5f*Time.deltaTime) {
			titleText.color = Color.Lerp (startColorTitle, Color.clear, t);
			instructionText.color = Color.Lerp (startColorInstruction, Color.clear, t);
			yield return null;
		}

		Destroy (titleText.gameObject);
		Destroy (startText.gameObject);
	}

	IEnumerator FadeInTitleElements() {
		Color startColorTitle = titleText.color;
		Color endColorTitle = startColorTitle;
		endColorTitle.a = 0.5f;

/*		Color startColorInstruction = instructionText.color;
		Color endColorInstruction = startColorInstruction;
		endColorInstruction.a = 0;
*/
		for (float t = 0; t <= 1; t += 0.2f*Time.deltaTime) {
			titleText.color = Color.Lerp (startColorTitle, endColorTitle, t);
//			instructionText.color = Color.Lerp (startColorInstruction, endColorInstruction, t);
			yield return null;
		}
	}

	IEnumerator GameStartAmplitude() {
		float startAmplitude = GroundSinControl.amplitudeModifier;
		float lerpTime = 2;
		for(float t = 0; t <= lerpTime; t += Time.deltaTime) {
			GroundSinControl.amplitudeModifier = Mathf.SmoothStep(startAmplitude, 1, t / lerpTime); 
			yield return null;
		}
	}

	IEnumerator MoveTowardsPlayer() {

		float introLookVerticalOffset = 0;
		Vector3 newPos = new Vector3(player.transform.position.x, transform.position.y + 2.5f, player.transform.position.z);
		for(float t = 0; Vector3.Distance(player.transform.position, transform.position) > 4; t+= Time.deltaTime) {
			transform.position = Vector3.Lerp(transform.position, newPos, 0.01f);
			transform.LookAt(new Vector3(player.transform.position.x, player.transform.position.y + introLookVerticalOffset, player.transform.position.z));
			if(introLookVerticalOffset < lookVerticalOffset) {
				introLookVerticalOffset += Time.deltaTime;
			}
			yield return null;
		}
		Debug.Log("begun");
		begun = true;
	}

	Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle) {
		Vector2 offset = new Vector2(pivot.z, pivot.y);
		Vector2 offsetPoint = new Vector2(point.z, point.y) - offset;
		float cosVal = Mathf.Cos(angle * Mathf.Deg2Rad);
		float sinVal = Mathf.Sin(angle * Mathf.Deg2Rad);
		Vector2 rotatedPoint = new Vector2(offsetPoint.x * cosVal - offsetPoint.y * sinVal, offsetPoint.x * sinVal + offsetPoint.y * cosVal);
		rotatedPoint += offset;
		return new Vector3(point.x, rotatedPoint.y, rotatedPoint.x);
	}
}
