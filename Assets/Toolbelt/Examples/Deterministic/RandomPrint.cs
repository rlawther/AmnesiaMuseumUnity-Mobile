using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Toolbelt {
	namespace DeterministicExample {

		/***
		 * Simple demonstration of using a System.Random object with a seed.
		 * If running over a cluster, the number on the text boxes should be the same
		 * across all machines.
		 */
		public class RandomPrint : MonoBehaviour 
		{
			public int randomSeed = 77;
			public List<TextMesh> textMeshes;
			System.Random randomGen;

			// Use this for initialization
			void Start () {
				randomGen = new System.Random(randomSeed);
			}
			
			// Update is called once per frame
			void Update () {
				int frame = UnityEngine.Time.frameCount;

				if (frame % 100 == 0) {
					double ranNum = randomGen.NextDouble() * 100.0;
					string ranNumText = ranNum.ToString("F1");

					foreach (TextMesh tm in textMeshes) {
						tm.text = ranNumText;
					}
				}
			}
		}
	}
}