using System;
using System.Collections.Generic;
using Misc;
using UnityEditor;
using UnityEngine;

namespace AimPrediction {
	public class AimPredictionManager : MonoBehaviour {

		public enum AimDisplayType {
			Dots,
			Line
		}

		[SerializeField] private AimDisplayType _displayType;
		[Tooltip("The steps for the prediction ; the larger the smoother it will be!")]
		[SerializeField] private int _predictionsSteps;
		[Tooltip("Defines how much time ahead should be predicted.")]
		[SerializeField] private float _predictionTime;
		[Tooltip("The maximum of bounces the prediction will plot. 0 means no bounce previsualization.")]
		[SerializeField] private int _maxCollisionsToStopPrediction;
		
		[SerializeField] private SpriteRenderer _dotPrefab;
		[SerializeField] private LineRenderer _linePrefab;
		
		
		
		private int _layerMaskId;
		private bool _isInit;
		private List<SpriteRenderer> _dots;
		private List<LineRenderer> _lines;
		private float _shooterMaxVelocity;
		private Rigidbody2D _objectBody;
		private float _bodyBounciness;
		
		
		
		
		
		// PUBLIC API :

		/// <summary>
		/// Sets the basic information needed for the aim prediction. Call this before anything else!
		/// </summary>
		/// <param name="shooterMaxVelocity">The maximum force magnitude applied to your object by the shooter.</param>
		/// <param name="objectRigidBody">The rigidbody of your object's prefab. The component will extract the needed physics information.</param>
		public void Init(float shooterMaxVelocity, Rigidbody2D objectRigidBody) {
			_shooterMaxVelocity = shooterMaxVelocity;
			_objectBody = objectRigidBody;

			if (_objectBody.sharedMaterial != null)
				_bodyBounciness = _objectBody.sharedMaterial.bounciness;
			else if (_objectBody.GetComponent<Collider2D>())
				_bodyBounciness = _objectBody.GetComponent<Collider2D>().bounciness;
			else
				_bodyBounciness = 1f;
			
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
		/// Updates the aim prediction with the current aim information. Usually this method should be called on Update but you
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

			if (_displayType == AimDisplayType.Dots) {
				_dots = new List<SpriteRenderer>();
				for (int i = 0; i < _predictionsSteps; i++) {
					SpriteRenderer dot = Instantiate(_dotPrefab, Vector3.zero, Quaternion.identity, container);
					_dots.Add(dot);
				}
			}
			else if (_displayType == AimDisplayType.Line) {
				_lines = new List<LineRenderer>();
				for(int i = 0 ; i < _predictionsSteps - 1 ; i++)
					_lines.Add(Instantiate(_linePrefab, Vector3.zero, Quaternion.identity, container));
			}

			Hide();
		}


		private void PredictTrajectory(Vector3 startVelocity) {
			List<Vector2> predictedPositions = Plot(_objectBody, transform.position, startVelocity);

			if (_displayType == AimDisplayType.Dots) {
				for (int i = 0; i < _dots.Count; i++) {
					if (i > predictedPositions.Count - 1) {
						if (_dots[i].gameObject.activeSelf)
							_dots[i].gameObject.SetActive(false);
						continue;
					}

					_dots[i].transform.position = predictedPositions[i];
					if (!_dots[i].gameObject.activeSelf)
						_dots[i].gameObject.SetActive(true);
				}
			}
			else if (_displayType == AimDisplayType.Line) {
				for (int i = 0; i < _lines.Count; i++) {
					if (i > predictedPositions.Count - 2) {
						if(_lines[i].gameObject.activeSelf)
							_lines[i].gameObject.SetActive(false);
						continue;
					}
					
					_lines[i].SetPosition(0, predictedPositions[i]);
					_lines[i].SetPosition(1, predictedPositions[i + 1]);
					if(!_lines[i].gameObject.activeSelf)
						_lines[i].gameObject.SetActive(true);
				}
			}
		}
		
		public List<Vector2> Plot(Rigidbody2D body, Vector2 pos, Vector2 velocity) {
			float timeStep = Mathf.Max( _predictionTime / (_predictionsSteps * 1f), Time.fixedDeltaTime / Physics2D.velocityIterations);
			List<Vector2> results = new List<Vector2>();
 
			Vector2 gravityAccel = Physics2D.gravity * body.gravityScale * timeStep * timeStep;
			float drag = 1f - timeStep * body.drag;
			Vector2 moveStep = velocity * timeStep;

			bool hasCollided = false;
			float elapsedTime = 0f;

			int collisionsCount = 0;
			while(elapsedTime < _predictionTime) {
				moveStep += gravityAccel;
				moveStep *= drag;
				pos += moveStep;
				results.Add(pos);

				elapsedTime += timeStep;

				if (results.Count > 1) {
					if (hasCollided) {
						hasCollided = false;
						continue;
					}
					
					RaycastHit2D hit;
					Vector2 previousPosition = results[results.Count - 2];
					Vector2 currentPosition = results[results.Count - 1];

					hit = Physics2D.Linecast(previousPosition, currentPosition, _layerMaskId);
					if (hit.collider != null) {
						collisionsCount++;
						hasCollided = true;

						if (collisionsCount > _maxCollisionsToStopPrediction) {
							results.RemoveAt(results.Count - 1);
							break;
						}

						moveStep = Vector3.Reflect(moveStep * _bodyBounciness, hit.normal);
						pos = hit.point + moveStep * 0.1f;
						results[results.Count - 1] = pos;
					}
				}
			}
 
			return results;
		}
	}
}