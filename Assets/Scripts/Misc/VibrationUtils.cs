using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;

namespace Misc {
	public static class VibrationUtils {

		private static readonly string VibrationEnabledKey = "VibrationUtils.VibrationEnabled";
		
		private static float lastVibration = 0;
		private static float timeBetweenVibrations = 0.2f;
		

		public static void Vibrate() {
			Vibrate(false);
		}
		
		public static void Vibrate(bool ignoreVibrationCooldown) {
			if (!VibrationEnabled())
				return;
			
			if (ignoreVibrationCooldown || Time.unscaledTime - lastVibration > timeBetweenVibrations) {
				lastVibration = Time.unscaledTime;
				Debug.Log("Device generation : " + ((int) Device.generation));
				if((int) Device.generation > 30)
					HapticFeedback.DoHaptic(HapticFeedback.HapticForce.Medium);
				else
					Handheld.Vibrate();
			}
		}

		public static bool VibrationEnabled() {
			return PlayerPrefs.GetInt(VibrationEnabledKey, 1) == 1;
		}

		public static void ToggleVibrationEnabled() {
			PlayerPrefs.SetInt(VibrationEnabledKey, (PlayerPrefs.GetInt(VibrationEnabledKey, 1) + 1) % 2);
			
			if(VibrationEnabled())
				Vibrate(true);
		}
	}
}