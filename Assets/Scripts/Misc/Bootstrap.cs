using UnityEngine;
using UnityEngine.SceneManagement;

namespace Misc {
	public class Bootstrap : MonoBehaviour {
		private void Awake() {
			if (SceneManager.sceneCount == 1)
				SceneManager.LoadScene(1);
		}
	}
}