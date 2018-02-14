using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class InGameUi : MonoBehaviour {

		[SerializeField] private Text _scoreText;
		[SerializeField] private Text _bestScoreText;
		[SerializeField] private CanvasGroup _canvasGroup;

		public void UpdateDisplay(int score, int bestScore) {
			_scoreText.text = score + "";
			_bestScoreText.text = bestScore + "";
		}

		public void FadeIn(float duration) {
			_canvasGroup.DOFade(1f, duration);
		}
		
		public void FadeOut(float duration) {
			_canvasGroup.DOFade(0f, duration);
		}
	}
}