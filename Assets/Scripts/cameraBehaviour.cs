using UnityEngine;
using System;
using System.Collections;

public class cameraBehaviour : MonoBehaviour {

	public Transform player1, player2; 

	public float splitDistance = 5;

	private GameObject camera1;
	private GameObject camera2;
	public Texture2D splitterGraphic;

	private GameObject split;
	private GameObject splitter;
	float distance;
	bool oneView;
	[SerializeField]
	[Range(0,1)]
	float delta;
	[SerializeField]
	Vector3 margin;
	[SerializeField]
	Vector3 margin2;

	void Start () {
		//Setting up cameras
		camera1 = gameObject;
		camera2 = new GameObject ();
		camera2.AddComponent<Camera> ();
		
		/*
		The splitter is essentially a square infront of the
		camera that splits the screen in two with a dynamic axis
		if the two player objects are far enough from one another.
		The square has a simple stencil shader attached to it
		that simply skips rendering the area occupied by it.
		Effectively, the area in question is 'transparent'. As such
		camera 2, which completely ignores the square is
		rendered in that space. 
		*/
		splitter = new GameObject();
		splitter.transform.parent = transform;
		splitter.transform.localPosition = Vector3.forward;
		splitter.SetActive (false);
		split = GameObject.CreatePrimitive (PrimitiveType.Quad);
		split.transform.parent = splitter.transform;
		split.transform.localPosition = new Vector3 (0,-3.5f,0);
		split.transform.localScale = new Vector3 (7, 7, 4);
		split.transform.localEulerAngles = Vector3.zero;

		Material tempMat = new Material (Shader.Find ("Unlit/Color"));
		Material tempMat2 = new Material (Shader.Find ("Omar/Splitscreen"));
		split.GetComponent<Renderer>().material = tempMat2;
		split.layer = LayerMask.NameToLayer ("TransparentFX");
		camera2.GetComponent<Camera> ().cullingMask = ~(1 << LayerMask.NameToLayer ("TransparentFX"));
	}
	float angle;
	void LateUpdate () {
			
			float zDistance = player1.position.z - player2.transform.position.z;
			distance = Vector3.Distance (player1.position, player2.transform.position);
			if (distance < 15) {
				oneView = true;
			}else{
				oneView = false;
			}
			
			


			/*
			The camera focuses on the midpoint of the two players if they are 
			close enough to be shown in the same frame. If not, the cameras point at offsets in
			the direction of the other player so that the two players occupy the appropriate
			portions of the bisected screen.
			*/
			Vector3 midPoint = new Vector3 ((player1.position.x + player2.position.x) / 2, (player1.position.y + player2.position.y) / 2, (player1.position.z + player2.position.z) / 2); 
			if (oneView == false) {
				//decides what angle to bisect the screen
				//Since it is always a portion of camera 1 which is masked,
				//the calculations to find the angle are different whether player one
				//is to the left or right of player 2
				if (player1.transform.position.x <= player2.transform.position.x) {
					angle = Mathf.Rad2Deg * Mathf.Acos (zDistance / distance);
				} else {
					angle = Mathf.Rad2Deg * Mathf.Asin (zDistance / distance) - 90;
				}

				splitter.transform.localEulerAngles = new Vector3 (0, 0, angle);
				Vector3 offset = midPoint - player1.position; 
				offset = offset.normalized * splitDistance;
				midPoint = player1.position + offset;

				Vector3 offset2 = midPoint - player2.position; 
				offset2 = offset2.normalized * splitDistance;
				Vector3 midPoint2 = player2.position - offset;

				//If it's the first frame rendering in two views, snap the cameras in place
				//otherwise have them smoothly glide to their destinations
				if (splitter.activeSelf == false) {
					splitter.SetActive (true);
					camera2.SetActive (true);
					camera2.transform.position = camera1.transform.position;
					camera2.transform.rotation = camera1.transform.rotation;
				} else {
					camera2.transform.position = Vector3.Lerp (camera2.transform.position, midPoint2 + margin2, Time.deltaTime * 1.5f);
					Quaternion newRot2 = Quaternion.LookRotation (midPoint2 - camera2.transform.position);
					camera2.transform.rotation = Quaternion.Lerp (camera2.transform.rotation, newRot2, Time.deltaTime * 1.5f);
				}

			} else {	
				splitter.SetActive (false);
				camera2.SetActive (false);
			}

			if (oneView == true) {
				camera1.transform.position = Vector3.Lerp (camera1.transform.position, midPoint + margin, Time.deltaTime * 1.5f);
			} else {
				camera1.transform.position = Vector3.Lerp (camera1.transform.position, midPoint + margin2, Time.deltaTime * 1.5f);
			}
			Quaternion newRot = Quaternion.LookRotation (midPoint - camera1.transform.position);
			camera1.transform.rotation = Quaternion.Lerp (camera1.transform.rotation, newRot, Time.deltaTime * 1.5f);
	}

	void OnGUI(){
		//draws a bar between the axis bisecting the screen
		if (oneView == false) {
			GUIUtility.RotateAroundPivot (-angle, new Vector2 (Screen.width / 2, Screen.height / 2));
			GUI.DrawTexture (new Rect (Screen.width / 2, Screen.height / 2, 222222, 13), splitterGraphic);
			GUI.DrawTexture (new Rect (Screen.width / 2, Screen.height / 2, -222222, 13), splitterGraphic);
	}
	}
	}

