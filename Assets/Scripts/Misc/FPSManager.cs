using UnityEngine;

namespace Misc {

	public class FPSManager : MonoBehaviour {

		private void Awake() {
			Application.targetFrameRate = 60;
		}
	}

}