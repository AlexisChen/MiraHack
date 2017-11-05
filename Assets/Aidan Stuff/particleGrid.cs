using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class particleGrid : MonoBehaviour {
	
	public int SizeX;
	public int SizeY;
	public float Spacing;
	int countMax;

	[Header("Testing")]
	public bool testRadius;
	public int CenterX = 0;
	public int CenterY = 0;
	public int maxRadius;
	public float peakHeight;
	public float IveGotThePower = 1.0f;

	ParticleSystem.MainModule newMain;
	ParticleSystem m_System;
	ParticleSystem.Particle[] m_Particles;
	GameObject sphere;
	void Start () {

		countMax = SizeX * SizeY;

		m_System = gameObject.GetComponent(typeof(ParticleSystem)) as ParticleSystem;

		newMain = m_System.main;
		newMain.maxParticles = countMax;

		m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];



	}
		
	void Update () {

		int numParticlesAlive = m_System.GetParticles(m_Particles);

		int counting = 0;

		for (int xpos = 0; xpos < SizeX; xpos++)
		{
			for (int ypos = 0; ypos < SizeY; ypos++)
			{

				m_Particles[counting].position = new Vector3( (ypos * Spacing)*1.0F-((SizeY/2.0F) * Spacing)+(Spacing/2) , 0 , (xpos * Spacing)*1.0F-((SizeX/2.0F) * Spacing)+(Spacing/2));
				m_Particles [counting].startColor = Color.white;

				m_System.SetParticles(m_Particles, numParticlesAlive);

				counting++;
			
			}
		}


		if (testRadius == true) {
			
			for (float i = 0.0f; i < maxRadius; i++) {

				for (float a = 0.0f; a < (Mathf.PI * 2); a += 0.1f) {
					int x = Mathf.RoundToInt (CenterX + (Spacing * i) * Mathf.Cos (a));
					int y = Mathf.RoundToInt (CenterY + (Spacing * i) * Mathf.Sin (a));

					if (x >= 0 && x < SizeX && y >= 0 && y < SizeY) {
						//Debug.Log (getParticleIndex (x, y));

						m_Particles [getParticleIndex (x, y)].startColor = new Color (1, 1, (i / maxRadius), 1);

						Vector3 pos =  m_Particles [getParticleIndex (x, y)].position;
						pos.y = Mathf.Pow(((i / maxRadius)*(-1 * peakHeight))+(peakHeight),IveGotThePower);
						m_Particles [getParticleIndex (x, y)].position = pos;



					}

					m_System.SetParticles (m_Particles, numParticlesAlive);
				}

			}
		}

	}

	int getParticleIndex(int x, int y){
		if ((x > SizeX || y > SizeY) && (x < 0 || y < 0)) {
			return -1;
		}else{
			return x + SizeX * y;
		}
	}


}