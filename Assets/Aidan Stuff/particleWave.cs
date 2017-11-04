using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class particleWave : MonoBehaviour
{	
	ParticleSystem m_System;
	ParticleSystem.Particle[] m_Particles;
	public float multiplier = 0.01f;
	public float speed = 10.0f;
	public float phase = 1.0f;

	/* public Color colorTop;
	public Color colorBottom; */

	private void LateUpdate()
	{
		InitializeIfNeeded();


		int numParticlesAlive = m_System.GetParticles(m_Particles);


		for (int i = 0; i < numParticlesAlive; i++)
		{
			
			m_Particles[i].position = new Vector3 (m_Particles[i].position.x, m_Particles[i].position.y * Mathf.Sin((Time.time * speed) + (i*phase))*multiplier, m_Particles[i].position.z);
			//m_Particles[i].startColor = Color.Lerp(colorBottom, colorTop, Mathf.Sin((Time.time * speed) + (i*phase)));
		}


		m_System.SetParticles(m_Particles, numParticlesAlive);


	}

	void InitializeIfNeeded()
	{
		if (m_System == null)
			m_System = GetComponent<ParticleSystem>();

		if (m_Particles == null || m_Particles.Length < m_System.maxParticles)
			m_Particles = new ParticleSystem.Particle[m_System.maxParticles];
	}
}