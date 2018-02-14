using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimDisplayerManager : MonoBehaviour {

	/*[SerializeField] private SpriteRenderer _dot1;
	[SerializeField] private SpriteRenderer _dot2;
	[SerializeField] private SpriteRenderer _dot3;
	[SerializeField] private SpriteRenderer _dot4;
	[SerializeField] private SpriteRenderer _dot5;

	[SerializeField] private Color _activatedDotColor;
	[SerializeField] private Color _deactivatedDotColor;

	[SerializeField] private AimInformationManager _aimInformation;

	[SerializeField] private ArrowManager _knife;

	private List<Transform> _points;
	private int _layerMaskId;
	private PlayerManager _player;
	private Rigidbody2D _knifePhysics;

	private void Awake() {
		_layerMaskId = LayerMask.GetMask ("Default");
		_points = new List<Transform>();
		
		_points.Add(_dot1.transform);
		_points.Add(_dot2.transform);
		_points.Add(_dot3.transform);
		_points.Add(_dot4.transform);
		_points.Add(_dot5.transform);

		_player = FindObjectOfType<PlayerManager>();
		_knifePhysics = _knife.GetComponent<Rigidbody2D>();
	}
	
	public void ShowAim() {
		gameObject.SetActive(true);
		_aimInformation.Show();
	}

	public void HideAim() {
		gameObject.SetActive(false);
		_aimInformation.Hide();
	}

	public void UpdateDisplay(float eulerAngle, Vector3 angle,  float percentStrength) {
		if(percentStrength < 0.2f && gameObject.activeSelf) 
			HideAim();
		else if(percentStrength > 0.2f && !gameObject.activeSelf)
			ShowAim();


		if (!gameObject.activeSelf)
			return;
		
		transform.rotation = Quaternion.Euler(0f, 0f, eulerAngle);

		_dot1.color = percentStrength >= 0.2f ? _activatedDotColor : _deactivatedDotColor;
		_dot2.color = percentStrength >= 0.4f ? _activatedDotColor : _deactivatedDotColor;
		_dot3.color = percentStrength >= 0.6f ? _activatedDotColor : _deactivatedDotColor;
		_dot4.color = percentStrength >= 0.8f ? _activatedDotColor : _deactivatedDotColor;
		_dot5.color = percentStrength >= 1f ? _activatedDotColor : _deactivatedDotColor;
		
		_aimInformation.UpdateInformation(Mathf.RoundToInt(percentStrength * 100f), Mathf.RoundToInt(eulerAngle));
		
		ManagePoints(angle, percentStrength);
	}
	
	private void ManagePoints(Vector3 angle, float strengthPercent){
		PlotTrajectory (transform.position, _player.GetShootVelocity(angle, strengthPercent), strengthPercent, 0.075f, 1f);
	}


	private Vector3 PlotTrajectoryAtTime (Vector3 start, Vector3 startVelocity, float time, float timeGravity, float coeff) {
		return start + startVelocity * time + Physics.gravity * _knifePhysics.gravityScale *time * timeGravity * coeff;
	}


	private void PlotTrajectory (Vector3 start, Vector3 startVelocity, float strengthPercent, float timestep, float maxTime) {

		bool collider = false;
		int offset = 0;
		int k = 1;
		float coeff = 0.5f + (1f - strengthPercent) * 0.5f;

		for (int i = 0; i < _points.Count; i++) {
			float t = timestep*(k);
			float tgravity = timestep*(i+1);

			Vector3 pos = PlotTrajectoryAtTime (start, startVelocity, t, tgravity, coeff);
			 
			if (i > 0 && !collider) {
				RaycastHit2D hit;  
				Vector2 startPt = new Vector2(_points [i-1].position.x, _points [i-1].position.y);
				Vector2 end = new Vector2(pos.x,pos.y);
				hit = Physics2D.Linecast (startPt, end, _layerMaskId);
				if(hit.collider != null){
					/*startVelocity = Vector3.Reflect(startVelocity, hit.normal);
					start = pos = hit.point;
					collider = true;
					k = 1;
					_points [i].position = pos;
					_points [i].gameObject.SetActive (false);
					coeff = 0.75f;
					continue;
				}
			}
			_points [i].gameObject.SetActive (true);
			_points [i].position = pos;
			k++;
		}
	}*/
}
