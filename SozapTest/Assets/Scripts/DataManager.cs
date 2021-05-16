using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Manages saving and loading of game data, keeps data for currently selected level, total number of levels, and number of finished levels
/// </summary>
public class DataManager
{
    public int SelectedLevelIndex => _selectedLevelIndex;
    public int CompletedLevelCount => _completedLevelCount;
    public int TotalLevelCount => TOTAL_LEVEL_COUNT;
    public MapData SelectedLevelMapData => _selectedLevelMapData;

    private const int TOTAL_LEVEL_COUNT = 4;
    private const string COMPLETED_LEVEL_KEY = "CompletedLevelCount";
    private string _localFilePath;
    private int _completedLevelCount = 0;
    private int _selectedLevelIndex = 1;
    private LevelBestScore _selectedLevelBestScore;
    private MapData _selectedLevelMapData;

    public void Init()
    {
       if(PlayerPrefs.HasKey(COMPLETED_LEVEL_KEY))
       {
            _completedLevelCount = PlayerPrefs.GetInt(COMPLETED_LEVEL_KEY);
       }
       _localFilePath = Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : Application.dataPath;
    }

    public MapData ReadSelectedLevelData()
    {
        string lJsonString = Resources.Load<TextAsset>("LevelData/Level_" + _selectedLevelIndex).text;
        return _selectedLevelMapData = JsonUtility.FromJson<MapData>(lJsonString);
    }

    public MapData ReadNextLevelData()
    {
        _selectedLevelIndex++;
        return ReadSelectedLevelData();
    }

    public bool CheckIfNotLastLevel()
    {
        return _selectedLevelIndex < TOTAL_LEVEL_COUNT;
    }

    public void UpdateLevelBestScoreData(int pMinutes, int pSeconds)
    {
        if(_selectedLevelBestScore != null)
        {
            _selectedLevelBestScore.TotalPlaysCount++;
            if(pMinutes * 60 + pSeconds < _selectedLevelBestScore.Minutes * 60 + _selectedLevelBestScore.Seconds)
            {
                _selectedLevelBestScore.Minutes = pMinutes;
                _selectedLevelBestScore.Seconds = pSeconds;
            }
        }
        else
        {
            _selectedLevelBestScore = new LevelBestScore(pMinutes, pSeconds, 1);
            _completedLevelCount++;
            PlayerPrefs.SetInt(COMPLETED_LEVEL_KEY, _completedLevelCount);
        }
        File.WriteAllText(getBestScoreLevelLocalFilePath(_selectedLevelIndex) , JsonUtility.ToJson(_selectedLevelBestScore));
    }

    public LevelBestScore ReadLevelBestScoreData(int pIndex)
    {
        _selectedLevelIndex = pIndex;
        _selectedLevelBestScore = null;
        string lFilePath = getBestScoreLevelLocalFilePath(_selectedLevelIndex);
        if (File.Exists(lFilePath))
        {
            string lJsonString = File.ReadAllText(lFilePath);
            _selectedLevelBestScore = JsonUtility.FromJson<LevelBestScore>(lJsonString);
        }
        return _selectedLevelBestScore;
    }

    private string getBestScoreLevelLocalFilePath(int pLevelNumber)
    {
        return _localFilePath + "/BestScoreLevel_" + pLevelNumber + ".json";
    }
}

[Serializable]
public class MapData
{
    public Vector2Int Dimensions;
    public Vector2Int PlayerPosition;
    public MapElementType[] MapElements;
}

[Serializable]
public enum MapElementType
{
    EMPTY = 0,
    WALL = 1,
    GRASS = 2,
    BOX = 3,
    BOX_HOLDER = 4,
    PLAYER = 5,
    BOX_HOLDER_AND_BOX = 6,
    BOX_HOLDER_AND_PLAYER = 7
}

[Serializable]
public class LevelBestScore
{
    public int Minutes;
    public int Seconds;
    public int TotalPlaysCount;

    public LevelBestScore(int pMinutes, int pSeconds, int pTotalPlaysCount)
    {
        Minutes= pMinutes;
        Seconds= pSeconds;
        TotalPlaysCount= pTotalPlaysCount;
    }
}