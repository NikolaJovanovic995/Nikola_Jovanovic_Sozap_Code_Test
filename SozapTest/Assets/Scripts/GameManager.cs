using UnityEngine;

/// <summary>
/// Main manager that controlls all components
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private MenuManager _menuMenager;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private LevelController _levelController;

    private DataManager _dataManager;

    public void Awake()
    {
        bool lIsMobilePlatform = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
        InputController.ProcessKeyboardInput = !lIsMobilePlatform;
        AudioManager.Instance.Play("ThemeSong");
        _dataManager = new DataManager();
        _dataManager.Init();
        _menuMenager.OnLevelSelect += onLevelSelect;
        _menuMenager.Init(_dataManager.CompletedLevelCount, _dataManager.SelectedLevelIndex, _dataManager.TotalLevelCount, lIsMobilePlatform);
        _menuMenager.OnPlayClick += onPlayClick;
        _menuMenager.OnResetLevelClick += onResetLevelClick;
        _menuMenager.OnNextLevelClick += onNextLevelClick;
        _playerController.Init(_levelController);
        _levelController.OnLevelComplete += onLevelComplete;
    }

    private void onLevelSelect(int pIndex)
    {
        _menuMenager.ShowLevelBestScoreInfo(_dataManager.ReadLevelBestScoreData(pIndex));
    }

    private void onLevelComplete()
    {
        _menuMenager.StopTimerAndGetValues(out int lMinutes, out int lSeconds);
        _menuMenager.SetNextLevelButtonActive(_dataManager.CheckIfNotLastLevel());
        _dataManager.UpdateLevelBestScoreData(lMinutes, lSeconds);
        InputController.ProcessPlayerMovementInput = false;
        _menuMenager.RefreshChooseLevelDropdownOptions(_dataManager.CompletedLevelCount, _dataManager.SelectedLevelIndex);
    }

    private void onPlayClick()
    {
        playLevel(_dataManager.ReadSelectedLevelData());
        _menuMenager.HideMainMenu();
    }

    private void onResetLevelClick()
    {
        _menuMenager.StopTimer();
        _levelController.CreateLevel(_dataManager.SelectedLevelMapData);
        _menuMenager.StartTimer();
    }

    private void onNextLevelClick()
    {
        playLevel(_dataManager.ReadNextLevelData());
    }

    private void setCameraSizeAndPosition(Vector2Int pMapDimensions)
    {
        _mainCamera.transform.position = new Vector3(pMapDimensions.x / 2f, pMapDimensions.y / 2f, -10f);
        _mainCamera.orthographicSize = pMapDimensions.y / 2f +1;
    }

    private void playLevel(MapData pMapData)
    {
        _levelController.CreateLevel(pMapData);
        _menuMenager.StartTimer();
        InputController.ProcessPlayerMovementInput = true;
        setCameraSizeAndPosition(pMapData.Dimensions);
    }
}
