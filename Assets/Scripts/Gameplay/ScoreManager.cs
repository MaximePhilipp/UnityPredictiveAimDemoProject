using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager {

	private const string BestScoreKey = "ScoreManager.BestScoreKey";

	private readonly InGameUi _inGameUi;
	private int _score;
	private readonly int _bestScore;
	private int _currentBestScore;
	
	public ScoreManager(InGameUi inGameUi) {
		_inGameUi = inGameUi;

		_bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
		_currentBestScore = _bestScore;
		_score = 0;
		UpdateDisplay();
	}

	private void UpdateDisplay() {
		_inGameUi.UpdateDisplay(_score, _bestScore);
	}

	public void IncrementScore(int increment) {
		_score += increment;

		if (_score > _currentBestScore) {
			_currentBestScore = _score;
			PlayerPrefs.SetInt(BestScoreKey, _currentBestScore);
		}
		
		UpdateDisplay();
	}

	public void IncrementScore() {
		IncrementScore(1);
	}

	public int GetScore() {
		return _score;
	}

	public int GetBestScore() {
		return _bestScore;
	}
}
