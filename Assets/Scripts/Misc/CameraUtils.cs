using UnityEngine;

namespace Misc {
	public class CameraUtils {

		private static Camera _mainCamera;

		public static Bounds GetCameraBounds() {
			if(_mainCamera == null)
				_mainCamera = Camera.main;
			
			float screenAspect = (float)Screen.width / (float)Screen.height;
			float cameraHeight = _mainCamera.orthographicSize * 2;
			Bounds bounds = new Bounds(
				_mainCamera.transform.position,
				new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
			return bounds;
		}
	}
}