using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bassCarpet : MonoBehaviour {

	public GameObject carpetItem;

	public int SizeX;
	public int SizeY;
	public float Spacing;

	public int[] listTest;

	void Start () {

		for (int xpos = 0; xpos < SizeX; xpos++)
		{
			for (int ypos = 0; ypos < SizeY; ypos++)
			{
				Instantiate(carpetItem, new Vector3((ypos * Spacing)*1.0F-((SizeY/2.0F) * Spacing), 0, (xpos * Spacing)*1.0F-((SizeX/2.0F) * Spacing)), Quaternion.identity);
			}
		}
	}
	

	void Update () {
		
	}
}
