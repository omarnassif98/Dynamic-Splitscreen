using UnityEngine;
using System;
using System.Collections;

public class cameraBehaviour : MonoBehaviour {

	bool onePlayer;

	public Transform player1;
	public Transform player2;

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

		if (player1 != null && player2 != null) {
			onePlayer = false;
		} else {
			onePlayer = true;
		}
		camera1 = gameObject;
		camera2 = new GameObject ();
		camera2.AddComponent<Camera> ();
		camera2.GetComponent<Camera> ().cullingMask = ~(1 << LayerMask.NameToLayer ("TransparentFX"));

		splitter = new GameObject();
		splitter.transform.parent = gameObject.transform;
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
	}
	float angle;
	void LateUpdate () {
		if (onePlayer == false) {
			float zDistance = player1.position.z - player2.transform.position.z;
			distance = Vector3.Distance (player1.position, player2.transform.position);


			if (player1.transform.position.x <= player2.transform.position.x) {
				angle = Mathf.Rad2Deg * Mathf.Acos (zDistance / distance);
			} else {
				angle = Mathf.Rad2Deg * Mathf.Asin (zDistance / distance) - 90;
			}


			splitter.transform.localEulerAngles = new Vector3 (0, 0, angle);

			Vector3 midPoint = new Vector3 ((player1.position.x + player2.position.x) / 2, (player1.position.y + player2.position.y) / 2, (player1.position.z + player2.position.z) / 2); 
			if (camera1.GetComponent<Camera> ().WorldToViewportPoint (player2.position).x > 1 - delta || camera1.GetComponent<Camera> ().WorldToViewportPoint (player2.position).x < 0 + delta || camera1.GetComponent<Camera> ().WorldToViewportPoint (player2.position).y > 1 - delta || camera1.GetComponent<Camera> ().WorldToViewportPoint (player2.position).y < 0 + delta || camera1.GetComponent<Camera> ().WorldToViewportPoint (player1.position).x > 1 - delta || camera1.GetComponent<Camera> ().WorldToViewportPoint (player1.position).x < 0 + delta || camera1.GetComponent<Camera> ().WorldToViewportPoint (player1.position).y > 1 - delta || camera1.GetComponent<Camera> ().WorldToViewportPoint (player1.position).y < 0 + delta) {
				oneView = false;
			}
			if (distance < 15) {
				oneView = true;
			}
			//Waits for the two cameras to split and then calcuates a midpoint relevant to the difference in position between the two cameras.
			if (oneView == false) {
				Vector3 offset = midPoint - player1.position; 
				offset.x = Mathf.Clamp (offset.x, -splitDistance, splitDistance);
				offset.y = Mathf.Clamp (offset.y, -splitDistance, splitDistance);
				offset.z = Mathf.Clamp (offset.z, -splitDistance, splitDistance);
				midPoint = player1.position + offset;

				Vector3 offset2 = midPoint - player2.position; 
				offset2.x = Mathf.Clamp (offset2.x, -splitDistance, splitDistance);
				offset2.y = Mathf.Clamp (offset2.y, -splitDistance, splitDistance);
				offset2.z = Mathf.Clamp (offset2.z, -splitDistance, splitDistance);
				Vector3 midPoint2 = player2.position - offset;


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
				if (splitter.activeSelf)
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
		} else {
			camera1.transform.position = Vector3.Lerp (camera1.transform.position, player1.position + margin, Time.deltaTime * 1.5f);
			Quaternion newRot = Quaternion.LookRotation (player1.position - camera1.transform.position);
			camera1.transform.rotation = Quaternion.Lerp (camera1.transform.rotation, newRot, Time.deltaTime * 1.5f);

		}
	}

	void OnGUI(){
		if (oneView == false && onePlayer == false) {
			GUIUtility.RotateAroundPivot (-angle, new Vector2 (Screen.width / 2, Screen.height / 2));
			GUI.DrawTexture (new Rect (Screen.width / 2, Screen.height / 2, 222222, 13), splitterGraphic);
			GUI.DrawTexture (new Rect (Screen.width / 2, Screen.height / 2, -222222, 13), splitterGraphic);
	}
	}
	}

