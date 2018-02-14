using UnityEngine;

namespace Misc {

	public class DontDestroyOnLoad : MonoBehaviour {

		private void Awake() {
			DontDestroyOnLoad(this.gameObject);
		}

	}

}