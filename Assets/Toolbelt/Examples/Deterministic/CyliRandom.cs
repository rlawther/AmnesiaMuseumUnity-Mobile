using UnityEngine;
using System.Collections;

namespace Toolbelt {
	namespace DeterministicExample {

		public class CyliRandom : MonoBehaviour 
		{
			public int randomSeed = 22;
			public bool dontUseRandomSeed = false;

			System.Random randomGen;
			

			// Use this for initialization
			void Start () {
				if (!dontUseRandomSeed) {
					this.randomGen = new System.Random(randomSeed);
				} else {
					this.randomGen = new System.Random();
				}

				StartCoroutine(this.RandomColour());
			}
			
			IEnumerator RandomColour() 
			{
				while (true) {
					System.Random r = this.randomGen;
					this.renderer.material.color = new Color((float)r.NextDouble(),
					                                         (float)r.NextDouble(),
					                                         (float)r.NextDouble());
					yield return new WaitForSeconds(2.0f);
				}
			}
		}
	}
}