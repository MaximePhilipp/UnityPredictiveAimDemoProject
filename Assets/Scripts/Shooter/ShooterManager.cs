using System.Collections;
using System.Collections.Generic;
using AimPrediction;
using Lean.Touch;
using UnityEngine;

namespace Shooter {
	public class ShooterManager : MonoBehaviour {

		private readonly float MaxStrengthDist = Screen.width / 6f;
		
		[SerializeField] private GameObject _objectToThrow;
		[SerializeField] private AimPredictionManager _aimPrediction;
		[SerializeField] private float _shooterMaxStrength;

		private LeanFinger _currentFinger;
		
		
		
		
		
		// Lifecycle : 
		
		private void Awake() {
			_aimPrediction.Init(_shooterMaxStrength, _objectToThrow.GetComponent<Rigidbody2D>());
		}
		
		protected virtual void OnEnable() {
			LeanTouch.OnFingerDown += OnFingerDown;
			LeanTouch.OnFingerUp += OnFingerUp;
		}

		protected virtual void OnDisable() {
			LeanTouch.OnFingerDown -= OnFingerDown;
			LeanTouch.OnFingerUp -= OnFingerUp;
		}
		
		private void Update() {
			if (_currentFinger != null) {
				float strengthPercent = GetStrengthPercent();
				
				if (strengthPercent < 0.2f) {
					_aimPrediction.Hide();
					return;
				}
				
				_aimPrediction.Show();
				_aimPrediction.UpdateDisplay(GetAngleOfCurrentFinger(), GetStrengthPercent());
			}
		}
		
		
		

		
		// Touch Handling :
		
		private void OnFingerDown(LeanFinger finger) {
			if (_currentFinger == null) {
				_currentFinger = finger;
				_aimPrediction.Show();
			}
		}

		private void OnFingerUp(LeanFinger finger) {
			if (_currentFinger != null) {
				Vector3 angle = GetAngleOfCurrentFinger();
				float strengthPercent = GetStrengthPercent();

				_currentFinger = null;
				_aimPrediction.Hide();
				
				// Aborts the shoot if the strength is too low
				if (strengthPercent < 0.2f) 				
					return;

				// Instantiates the object and shoot!
				GameObject obj = Instantiate(_objectToThrow, transform.position, Quaternion.identity, null);
				obj.GetComponent<Rigidbody2D>().AddForce(angle * _shooterMaxStrength * strengthPercent, ForceMode2D.Impulse);
			}
		}
		
		private Vector3 GetAngleOfCurrentFinger() {
			return Vector3.Normalize(_currentFinger.StartScreenPosition - _currentFinger.ScreenPosition);
		}
		
		private float GetStrengthPercent() {
			if (_currentFinger == null)
				return 0f;
			return Mathf.Min(1f, Vector3.Distance(_currentFinger.ScreenPosition, _currentFinger.StartScreenPosition) / MaxStrengthDist);
		}

	}
}
