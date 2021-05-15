using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private MenuManager _menuMenager;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private LevelController _levelController;

    private DataManager _dataManager;


    public void Awake()
    {
        _dataManager = new DataManager();
        _dataManager.Init();
        _menuMenager.Init(_dataManager.CompletedLevelsCount);
        _menuMenager.OnLevelSelect += onLevelSelect;
        _menuMenager.OnPlayClick += onPlayClick;
        _menuMenager.OnResetLevelClick += onResetLevelClick;
        _menuMenager.OnNextLevelClick += onNextLevelClick;
        _playerController.Init();
        _levelController.OnLevelComplete += onLevelComplete;
        _menuMenager.ShowMainMenu();
    }

    private void onLevelSelect(int pIndex)
    {
        _menuMenager.ShowBestScoreInfo(_dataManager.ReadBestScoreData(pIndex));
    }

    private void onLevelComplete()
    {
        int lMinutes, lSeconds;
        _menuMenager.StopTimerAndGetValues(out lMinutes, out lSeconds);
        _menuMenager.SetNextLevelButtonActive(_dataManager.CheckIfNotLastLevel());
        _dataManager.LevelCompleted(lMinutes, lSeconds);
        InputController.ProcessPlayerMovementInput = false;
    }

    private void onPlayClick()
    {
        MapData lMapData = _dataManager.ReadSelectedLevelData();
        _menuMenager.HideMainMenu();
        playLevel(lMapData);
    }

    private void onResetLevelClick()
    {
        _menuMenager.StopTimer();
        _levelController.CreateLevel(_dataManager.SelectedLevelMapData);
        _menuMenager.StartTimer();
    }

    private void onNextLevelClick()
    {
        MapData lMapData = _dataManager.ReadNextLevelData();
        playLevel(lMapData);
    }

    private void setCameraSizeAndPosition(Vector2Int pMapDimensions)
    {
        _mainCamera.transform.position = new Vector3(pMapDimensions.x / 2f, pMapDimensions.y / 2f, -10f);
        _mainCamera.orthographicSize = pMapDimensions.y / 2f;
    }

    private void playLevel(MapData pMapData)
    {
        _levelController.CreateLevel(pMapData);
        _menuMenager.StartTimer();
        InputController.ProcessPlayerMovementInput = true;
        setCameraSizeAndPosition(pMapData.Dimensions);
    }
}
