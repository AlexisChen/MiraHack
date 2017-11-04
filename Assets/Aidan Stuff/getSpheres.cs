using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getSpheres : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject[] spheres = GameObject.FindGameObjectsWithTag("sphere");
		Cloth bassCarpet = gameObject.GetComponent(typeof(Cloth)) as Cloth;
		List<ClothSphereColliderPair> sphereColliders = new List<ClothSphereColliderPair>();


		foreach (GameObject sphere in spheres)
		{	

			sphereColliders.Add(new ClothSphereColliderPair(sphere.GetComponent(typeof(SphereCollider)) as SphereCollider , null));

		}

		bassCarpet.sphereColliders = sphereColliders.ToArray();
	}

	// Update is called once per frame
	void Update () {

	}
}
