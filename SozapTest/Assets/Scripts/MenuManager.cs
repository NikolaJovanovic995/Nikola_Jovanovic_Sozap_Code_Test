using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages all UI elements
/// </summary>
public class MenuManager : MonoBehaviour
{
    public event Action OnResetLevelClick;
    public event Action OnNextLevelClick;
    public event Action OnPlayClick;
    public event Action<int> OnLevelSelect;

    [SerializeField] private Dropdown _chooseLevelDropdown;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _resetLevelButton;
    [SerializeField] private Button _backToMenuButton;
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Text _timerText;
    [SerializeField] private Text _bestScoreText;
    [SerializeField] private GameObject _panelMainMenu;
    [SerializeField] private GameObject _arrowsHUDHolder;
    [SerializeField] private Button _moveButtonLeft;
    [SerializeField] private Button _moveButtonRight;
    [SerializeField] private Button _moveButtonUp;
    [SerializeField] private Button _moveButtonDown;

    private long _secondsElapsed = 0;
    private int _totalLevelCount = 0;
    private Coroutine _timerCorutine;
    private const string NO_BEST_SCORE_TEXT = "This level is not completed yet!";

    public void Init(int pCompletedLevelsCount, int pSelectedLevelIndex, int pTotalLevelsCount, bool pEnableHUDArrows)
    {
        _totalLevelCount = pTotalLevelsCount;
        _arrowsHUDHolder.SetActive(pEnableHUDArrows);
        if (pEnableHUDArrows)
        {
            _moveButtonLeft.onClick.AddListener(() => InputController.FireMoveEvent(Vector2Int.left));
            _moveButtonRight.onClick.AddListener(() => InputController.FireMoveEvent(Vector2Int.right));
            _moveButtonUp.onClick.AddListener(() => InputController.FireMoveEvent(Vector2Int.up));
            _moveButtonDown.onClick.AddListener(() => InputController.FireMoveEvent(Vector2Int.down));
        }

        _resetLevelButton.onClick.AddListener(() => OnResetLevelClick?.Invoke());
        _playButton.onClick.AddListener(() => OnPlayClick?.Invoke());
        _nextLevelButton.onClick.AddListener(onNextLevelClick);
        _backToMenuButton.onClick.AddListener(onBackToMenuClick);
        _chooseLevelDropdown.onValueChanged.AddListener(onDropdownValueChanged);
        RefreshChooseLevelDropdownOptions(pCompletedLevelsCount, pSelectedLevelIndex);
        ShowMainMenu();
    }

    public void RefreshChooseLevelDropdownOptions(int pCompletedLevelCount, int pSelectedLevelIndex)
    {
        _chooseLevelDropdown.ClearOptions();
        List<string> lDropOptions = new List<string>();
        int lLevelCount = Mathf.Clamp(pCompletedLevelCount + 1, 1, _totalLevelCount);
        for (int i = 1; i <= lLevelCount; i++)
        {
            lDropOptions.Add("Level " + i);
        }
        _chooseLevelDropdown.AddOptions(lDropOptions);
        _chooseLevelDropdown.value = pSelectedLevelIndex - 1;
        _chooseLevelDropdown.onValueChanged.Invoke(pSelectedLevelIndex - 1);
    }

    public void HideMainMenu()
    {
        _panelMainMenu.SetActive(false);
        SetNextLevelButtonActive(false);
    }

    public void ShowMainMenu()
    {
        _panelMainMenu.SetActive(true);
    }

    public void SetNextLevelButtonActive(bool pState)
    {
        _nextLevelButton.gameObject.SetActive(pState);
    }

    public void ShowLevelBestScoreInfo(LevelBestScore pBestScore)
    {
        _bestScoreText.text = pBestScore != null ? formatLevelBestScoreText(pBestScore) : NO_BEST_SCORE_TEXT;
    }

    public void StartTimer()
    {
        _timerCorutine = StartCoroutine(RunTimer());
    }

    public void StopTimer()
    {
        StopCoroutine(_timerCorutine);
    }

    public void StopTimerAndGetValues(out int pMinutes,out int pSeconds)
    {
        StopTimer();
        pMinutes = (int)(_secondsElapsed / 60);
        pSeconds = (int)(_secondsElapsed % 60);
    }

    private void onNextLevelClick()
    {
        OnNextLevelClick?.Invoke();
        SetNextLevelButtonActive(false);
    }

    private void onBackToMenuClick()
    {
        StopTimer();
        ShowMainMenu();
    }

    private void onDropdownValueChanged(int pIndex)
    {
        OnLevelSelect?.Invoke(pIndex + 1);
    }

    private IEnumerator RunTimer()
    {
        _secondsElapsed = 0;
        while (true)
        {
            _timerText.text = formatTimeText((int)_secondsElapsed / 60, (int)_secondsElapsed % 60);
            yield return new WaitForSeconds(1f);
            _secondsElapsed++;
        }
    }

    private string formatTimeText(int pMinutes, int pSeconds)
    {
        return string.Format("{0:00}:{1:00}", pMinutes, pSeconds);
    }

    private string formatLevelBestScoreText(LevelBestScore pBestScore)
    {
        return "Best time is " + formatTimeText(pBestScore.Minutes, pBestScore.Seconds) + " of the total " + pBestScore.TotalPlaysCount + " number of scores.";
    }
}
