using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 	Ripple Effect Notes
 *
 *	Step 1: Get closest 4 vertices to collider
 *	Step 2: Calculate initial levels for each step outwards (Less for each consecutive row)
 *	Step 3: On collision, Set each point to its start value
 *	Step 4: Decay on collision stay event
 *
 */

[ExecuteInEditMode]
public class TriggerScript : MonoBehaviour
{
	ParticleSystem ps;

	public Color32 ColorIn;
	public Color32 ColorOut;

	List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
	List<ParticleSystem.Particle> outside = new List<ParticleSystem.Particle>();

	void OnEnable()
	{
		ps = GetComponent<ParticleSystem>();
	}

	void OnParticleTrigger()
	{

		int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
		int numOutside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Outside, outside);

		for (int i = 0; i < numEnter; i++)
		{
			ParticleSystem.Particle p = enter[i];
			p.startColor = ColorIn;
			p.startSize = .5f;
			p.startSize--;
			enter[i] = p;
		}
			
		for (int i = 0; i < numOutside; i++)
		{
			ParticleSystem.Particle p = outside[i];
			p.startColor = ColorOut;
			//p.startSize = 0.5f;
			outside[i] = p;
		}
			
		ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
		ps.SetTriggerParticles(ParticleSystemTriggerEventType.Outside, outside);

	}
}
