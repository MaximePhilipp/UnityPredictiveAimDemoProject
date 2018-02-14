using UnityEngine;

namespace Misc {

	public class FPSDisplayer : MonoBehaviour {

		float deltaTime = 0.0f;

		private void Update() {
			deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		}

		private void OnGUI() {
			int w = Screen.width, h = Screen.height;

			GUIStyle style = new GUIStyle();

			Rect rect = new Rect(w * -0.02f, h * 0.96f, w, h * 0.02f);
			style.alignment = TextAnchor.LowerRight;
			style.fontSize = h * 2 / 100;
			style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
			float msec = deltaTime * 1000.0f;
			float fps = 1.0f / deltaTime;
			string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
			GUI.Label(rect, text, style);
		}

	}

}

