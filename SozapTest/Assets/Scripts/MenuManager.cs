using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject _panelHUD;

    private long _secondsElapsed = 0;
    private Coroutine _timerCorutine;

    public void Init(int pCompletedLevelsCount)
    {
        _resetLevelButton.onClick.AddListener(() => OnResetLevelClick?.Invoke());
        _playButton.onClick.AddListener(() => OnPlayClick?.Invoke());
        _nextLevelButton.onClick.AddListener(()=> { 
            OnNextLevelClick?.Invoke();
            SetNextLevelButtonActive(false); 
        });
        _backToMenuButton.onClick.AddListener(()=> {
            StopTimer();
            ShowMainMenu();
        });
        _chooseLevelDropdown.onValueChanged.AddListener(onDropdownValueChanged);
        addLevelOptions(pCompletedLevelsCount);
    }

    public void HideMainMenu()
    {
        _panelMainMenu.SetActive(false);
        _panelHUD.SetActive(true);
        SetNextLevelButtonActive(false);
    }

    public void ShowMainMenu()
    {
        _panelMainMenu.SetActive(true);
        _panelHUD.SetActive(false);
    }

    public void SetNextLevelButtonActive(bool pState)
    {
        _nextLevelButton.gameObject.SetActive(pState);
    }

    public void ShowBestScoreInfo(BestScore pBestScore)
    {
        if(pBestScore != null)
        {
            _bestScoreText.text = "Best time is "+pBestScore.Minutes+":"+pBestScore.Seconds + " of a total " + pBestScore.TotalPlaysCount + " number of tries.";
        }
        else
        {
            _bestScoreText.text = "This level is not completed yet!";
        }
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
        StopTimer();
    }

    private void onDropdownValueChanged(int pIndex)
    {
        OnLevelSelect?.Invoke(pIndex + 1);
    }

    private void addLevelOptions(int pLevelCount)
    {
        _chooseLevelDropdown.ClearOptions();
        List<string> lDropOptions = new List<string>();
        for (int i = 0; i<=pLevelCount;i++)
        {
            lDropOptions.Add("Level "+ (i+1));
        }
        _chooseLevelDropdown.AddOptions(lDropOptions);
    }

    private IEnumerator RunTimer()
    {
        _secondsElapsed = 0;
        while (true)
        {
            _timerText.text = string.Format("{0:00}:{1:00}", _secondsElapsed / 60, _secondsElapsed % 60);
            yield return new WaitForSeconds(1f);
            _secondsElapsed++;
        }
    }
}
