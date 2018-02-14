using System.Collections.Generic;
using Misc;
using UnityEditor;
using UnityEngine;

namespace AimPrediction {
	public class AimPredictionManager : MonoBehaviour {

		[Tooltip("The maximum amount of dots to display for the aim.")]
		[SerializeField] private int _dotsToDisplay;
		[Tooltip("The higher the value, the bigger the aim will be.")]
		[SerializeField] private float _predictionTime;

		[SerializeField] private bool _hideAtStartup;
		[SerializeField] private bool _stopPredictionOnFirstCollision;
		
		// TODO 
		[SerializeField] private SpriteRenderer _dotPrefab;
		
		
		
		private int _layerMaskId;
		private List<SpriteRenderer> _dots;
		private float _shooterMaxVelocity;
		private float _objectGravityScale;
		
		
		
		
		
		// PUBLIC API :

		public void Init(float shooterMaxVelocity, float objectGravityScale) {
			_shooterMaxVelocity = shooterMaxVelocity;
			_objectGravityScale = objectGravityScale;
		}
		
		public void Show() {
			if(!gameObject.activeSelf)
				gameObject.SetActive(true);
		}

		public void Hide() {
			if(gameObject.activeSelf)
				gameObject.SetActive(false);
		}

		public void UpdateDisplay(Vector2 aimVector, float strengthPercent) {
			transform.rotation = Quaternion.Euler(0f, 0f, VectorUtils.AngleBetweenVector2(Vector2.zero, aimVector));
			PredictTrajectory(aimVector * (_shooterMaxVelocity * strengthPercent), _predictionTime / _dotsToDisplay);
		}
		
		
		
		
		
		
		
		
		
		// PRIVATE API :
		
		private void Awake() {
			_layerMaskId = LayerMask.GetMask ("Default");
			
			Transform container = null;
			Transform child;
			for(int i = 0 ; i < transform.childCount ; i++) {
				child = transform.GetChild(i);
				if (child.name.ToLower().Contains("container")) {
					container = child.transform;
					break;
				}
			}
			
			_dots = new List<SpriteRenderer>();
			for (int i = 0; i < _dotsToDisplay; i++) {
				SpriteRenderer dot = Instantiate(_dotPrefab, Vector3.zero, Quaternion.identity, container);
				_dots.Add(dot);
			}
			
			if(_hideAtStartup)
				gameObject.SetActive(false);
		}


		private void PredictTrajectory(Vector3 startVelocity, float timestep) {
			Vector3 start = transform.position;
			bool collider = false;
			int k = 1;
			float coeff = 0.5f;

			for (int i = 0; i < _dots.Count; i++) {
				float t = timestep * k;
				float tgravity = timestep * (i + 1);
				Transform currentTransform = _dots[i].transform;

				Vector3 pos = PredictTrajectoryAtTime(start, startVelocity, t, tgravity, coeff);
			 
				if (i > 0 && !collider) {
					RaycastHit2D hit;
					Transform previousTransform = _dots[i - 1].transform;
					
					Vector2 startPt = new Vector2(previousTransform.position.x, previousTransform.position.y);
					Vector2 end = new Vector2(pos.x,pos.y);
					hit = Physics2D.Linecast (startPt, end, _layerMaskId);
					if(hit.collider != null){
						if (!_stopPredictionOnFirstCollision) {
							startVelocity = Vector3.Reflect(startVelocity, hit.normal);
							start = pos = hit.point;
							collider = true;
							k = 1;
							currentTransform.position = pos;
						}
						_dots [i].gameObject.SetActive(false);
						coeff = 0.75f;
						continue;
					}
				}
				_dots[i].gameObject.SetActive(true);
				currentTransform.position = pos;
				k++;
			}
		}
		
		private Vector3 PredictTrajectoryAtTime (Vector3 start, Vector3 startVelocity, float time, float timeGravity, float coeff) {
			return start + startVelocity * time + Physics.gravity * _objectGravityScale * time * timeGravity * coeff;
		}
	}
}