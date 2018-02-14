using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class GameOverUi : MonoBehaviour {

		[SerializeField] private Text _scoreText;
		[SerializeField] private Text _bestScoreText;
		[SerializeField] private CanvasGroup _canvasGroup;

		private GameManager _game;
		private bool _shown;

		private void Awake() {
			_game = FindObjectOfType<GameManager>();
			
			if(!_shown)
				gameObject.SetActive(false);
		}

		public void Show(int score, int bestScore) {
			_shown = true;
			
			_scoreText.text = score + "";
			_bestScoreText.text = bestScore + "";
			_canvasGroup.alpha = 0f;
			gameObject.SetActive(true);

			_canvasGroup.DOFade(1f, 0.3f);

			Debug.Log("Game over shown.");
		}

		public void OnTapTocontinueTriggered() {
			_game.RestartGame();
		}
	}
}
