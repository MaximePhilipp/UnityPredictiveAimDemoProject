using System;
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
		[SerializeField] private bool _stopPredictionOnFirstCollision;
		
		// TODO 
		[SerializeField] private SpriteRenderer _dotPrefab;
		
		
		
		private int _layerMaskId;
		private bool _isInit;
		private List<SpriteRenderer> _dots;
		private float _shooterMaxVelocity;
		private Rigidbody2D _objectBody;
		
		
		
		
		
		// PUBLIC API :

		/// <summary>
		/// Sets the basic information needed for the aim prediction. Call this before anything else!
		/// </summary>
		/// <param name="shooterMaxVelocity">The maximum force magnitude applied to your object by the shooter.</param>
		/// <param name="objectGravityScale">The rigidbody of your object's prefab. The component will extract the needed physics information.</param>
		public void Init(float shooterMaxVelocity, Rigidbody2D objectRigidBody) {
			_shooterMaxVelocity = shooterMaxVelocity;
			_objectBody = objectRigidBody;
			_isInit = true;
		}
		
		/// <summary>
		/// Displays the aim prediction if it wasn't already the case. Does nothing otherwise.
		/// </summary>
		public void Show() {
			if(!gameObject.activeSelf)
				gameObject.SetActive(true);
		}

		/// <summary>
		/// Hides the aim prediction if it wasn't already the case. Does nothing otherwise.
		/// </summary>
		public void Hide() {
			if(gameObject.activeSelf)
				gameObject.SetActive(false);
		}

		/// <summary>
		/// Updates the aim prediction with the current aim information. Usually this method is called on Update but you
		/// can call this less often if you want a more jittery display.
		/// </summary>
		/// <param name="aimVector">The normalized vector of the current aim.</param>
		/// <param name="strengthPercent">The strength percentage between 0.0 and 1.0.</param>
		/// <exception cref="Exception"></exception>
		public void UpdateDisplay(Vector2 aimVector, float strengthPercent) {
			if(!_isInit)
				throw new Exception("Trying to update the aim prediction before initializing the component! Try calling Init() first.");
			
			transform.rotation = Quaternion.Euler(0f, 0f, VectorUtils.AngleBetweenVector2(Vector2.zero, aimVector));
			PredictTrajectory(aimVector * (_shooterMaxVelocity * strengthPercent));
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

			Hide();
		}


		private void PredictTrajectory(Vector3 startVelocity) {
			List<Vector2> predictedPositions = Plot(_objectBody, transform.position, startVelocity);

			for (int i = 0; i < _dots.Count; i++) {
				if (i > predictedPositions.Count - 1 && _dots[i].gameObject.activeSelf) {
					_dots[i].gameObject.SetActive(false);
					return;
				}
				
				_dots[i].transform.position = predictedPositions[i];
				if(!_dots[i].gameObject.activeSelf)
					_dots[i].gameObject.SetActive(true);
			}
			
			
			
			/*
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
					if(hit.collider != null) {
						if (!_stopPredictionOnFirstCollision) {
							startVelocity = Vector3.Reflect(startVelocity, hit.normal);
							start = pos = hit.point;
							collider = true;
							k = 1;
							currentTransform.position = pos;
						}
						_dots [i].gameObject.SetActive(false);
						coeff = 0.9f;
						continue;
					}
				}
				_dots[i].gameObject.SetActive(true);
				currentTransform.position = pos;
				k++;
			}*/
		}
		
		private Vector3 PredictTrajectoryAtTime (Vector3 start, Vector3 startVelocity, float time, float timeGravity, float coeff) {
			return start + startVelocity * time + Physics.gravity * _objectBody.gravityScale * time * timeGravity * coeff;
		}
		
		
		
		
		// New prediction method
		
		public List<Vector2> Plot(Rigidbody2D body, Vector2 pos, Vector2 velocity) {
			float timeStep = Mathf.Max( _predictionTime / (_dotsToDisplay * 1f), Time.fixedDeltaTime / Physics2D.velocityIterations);
			List<Vector2> results = new List<Vector2>();
 
			Vector2 gravityAccel = Physics2D.gravity * body.gravityScale * timeStep * timeStep;
			float drag = 1f - timeStep * body.drag;
			Vector2 moveStep = velocity * timeStep;

			float elapsedTime = 0f;
			while(elapsedTime < _predictionTime) {
				moveStep += gravityAccel;
				moveStep *= drag;
				pos += moveStep;
				results.Add(pos);

				elapsedTime += timeStep;
			}
 
			return results;
		}
	}
}