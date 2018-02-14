using UnityEngine;
using UnityEngine.SceneManagement;

namespace Misc {
	public class ForceBootstrap : MonoBehaviour {
		private void Awake() {
			if (FindObjectOfType<Bootstrap>() == null)
				SceneManager.LoadScene(0, LoadSceneMode.Additive);
		}
	}
}