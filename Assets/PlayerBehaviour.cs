using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSettings
{
	public float velocity;
	public float angularVelocity;

	[Range(0f,15f)]
	public float gainDb;
	[Range(0f, 90f)]
	public float range;

	[Range(0f, 10f)]
	public float minDistance;
}

[System.Serializable]
public class TouchBehaviour
{
	public Vector2 initialPosition;
	public Vector2 direction;

	[Range(0f,1f)]
	public float initX, initY;

	private Vector2 screen;

	public void Start()
	{
		screen.x = Screen.width * initX;
		screen.y = Screen.height * initY;
	}

	public void Update()
	{
		if(Input.touchCount > 0)
		{
			Touch touch =  Input.touches[0];

			if(touch.phase == TouchPhase.Began)
			{
				initialPosition = touch.position;
			}
			else if(touch.phase == TouchPhase.Ended)
			{
				initialPosition = Vector2.zero;
				direction = Vector2.zero;
			}
			else if(touch.phase == TouchPhase.Moved)
			{
				Vector2 temp = touch.position - initialPosition;
				if(Mathf.Abs(temp.x) >= screen.x)
					direction.x = temp.x;
				if(Mathf.Abs(temp.y) >= screen.y)
					direction.y = temp.y;
			}
		}
	}
}

public class PlayerBehaviour : MonoBehaviour {

	public PlayerSettings playerSettings;
	public TouchBehaviour touchBehaviour;


	public Transform audioSurce;
	private Transform gps;
	private ResonanceAudioSource resonance;
	private float initDB;
	private float dir;


	private float x;
	private float gain;

	// Use this for initialization
	void Start () {
		touchBehaviour.Start();
		gps = transform.GetChild(0);
		resonance = audioSurce.GetComponent<ResonanceAudioSource>();
		initDB = resonance.gainDb;
		
	}
	
	// Update is called once per frame
	void Update () {
		touchBehaviour.Update();

		float ang = touchBehaviour.direction.x * playerSettings.angularVelocity;
		ang *= Time.deltaTime;
		transform.Rotate(0, ang, 0);

		float velocity = touchBehaviour.direction.y * playerSettings.velocity;
		velocity *= Time.deltaTime;
		transform.Translate(0, 0, velocity);

		gps.LookAt(audioSurce);
		dir = transform.eulerAngles.y - gps.eulerAngles.y;

		if(dir > -playerSettings.range && dir < playerSettings.range)
		{
			Vector3 vecDis = audioSurce.position - transform.position;
			float dis = Mathf.Sqrt((vecDis.x*vecDis.x) + (vecDis.y*vecDis.y) + (vecDis.z*vecDis.z));
			Debug.Log(dis);
			if(dis >= playerSettings.minDistance)
			{
				x = (Mathf.Abs(dir)*180)/playerSettings.range;
				gain = Mathf.Sin(Mathf.Deg2Rad*x) * playerSettings.gainDb;
				resonance.gainDb = initDB + gain;
			}
		}
		else
		{
			resonance.gainDb = initDB;
		}


	}
}
