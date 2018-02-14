using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Touch;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public enum GameState {
		Inactive,
		Started,
		Finished
	}

	[SerializeField] private InGameUi _inGameUi;
	[SerializeField] private GameOverUi _gameOverUi;
	
	private GameState _state;
	private ScoreManager _scoreManager;

	private void Awake() {
		_state = GameState.Inactive;
		_scoreManager = new ScoreManager(_inGameUi);
	}
	
	protected virtual void OnEnable() {
		LeanTouch.OnFingerDown += OnFingerDown;
	}

	protected virtual void OnDisable() {
		LeanTouch.OnFingerDown -= OnFingerDown;
	}
	
	private void OnFingerDown(LeanFinger finger) {
		if (_state == GameState.Inactive) {
			_state = GameState.Started;
		
			// TODO start your game
		}
	}

	
	
	public void GameFinished() {
		_state = GameState.Finished;
		
		DOTween.Sequence()
			.AppendCallback(() => _inGameUi.FadeOut(0.3f))
			.AppendInterval(0.4f)
			.AppendCallback(() => _gameOverUi.Show(_scoreManager.GetScore(), _scoreManager.GetBestScore()));
	}
	
	public void RestartGame() {
		SceneManager.LoadScene("Game");
	}

	public GameState GetCurrentState() {
		return _state;
	}
}
