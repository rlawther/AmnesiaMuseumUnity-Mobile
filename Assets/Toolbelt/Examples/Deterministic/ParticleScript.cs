using UnityEngine;
using System.Collections;

namespace Toolbelt {
	namespace DeterministicExample {

		public class ParticleScript : MonoBehaviour {

			public int randomSeed = 77;

			void Start () 
			{
				this.particleSystem.randomSeed = (uint)this.randomSeed;
				this.particleSystem.Simulate(0);
				this.particleSystem.Play();


			}
		}
	}

}
