using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveOnClick : MonoBehaviour {
//	Animator anim;
//	int runHash = Animator.StringToHash("Run");
//	int stopHash = Animator.StringToHash("Stop");
//	int idleHash = Animator.StringToHash("Base Layer.Idle");
//	int moveHash = Animator.StringToHash("Base Layer.bodyMove");
//

	//for emitting effects:
	public float factor = 1.0f;
	public float timeConstraint = 1.0f;
	public float speed = 2.5f;
	public float upDownSpeed = 1.5f;
	public float amplitude = 3f;
	public float rotationSpeed = 2.5f;
	public float timeDuration = 1.0f;
	public float timeOffset;

	private float currTime;
	private bool move = true;



	Vector3 origin;
	// Use this for initialization
	void Start () {
//		anim = GetComponent<Animator>();
		origin = transform.position;
		currTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		//updating the position: 
		if (currTime < timeDuration) {
			currTime += Time.deltaTime;
			float time = currTime + timeOffset;
			float theta = 2 * Mathf.PI * time;

			float height = amplitude * Mathf.Sin (time * Mathf.PI * upDownSpeed);
			float x = Mathf.Cos (theta * speed) * (height + amplitude) * factor;
			float z = Mathf.Sin (theta * speed) * (height + amplitude) * factor;
			//		transform.position.Set(x, height, z);
			Vector3 tempPos = origin;
			tempPos.y += height + amplitude;
			tempPos.x += x;
			tempPos.z += z;

			transform.position = tempPos;
			transform.Rotate (Vector3.up, theta * rotationSpeed);
		} else {
			transform.position = origin;
		}

		if(Input.GetMouseButtonDown(0))
		{
			currTime = 0;

			//updating the animation effect. 
//			AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
//			if(stateInfo.nameHash == idleHash)
//			{
//				anim.SetTrigger(runHash);
//				anim.ResetTrigger(stopHash);
//			}else if(stateInfo.nameHash == moveHash)
//			{
//				anim.SetTrigger(stopHash);
//				anim.ResetTrigger(runHash);
//			}

		}
	}
}
