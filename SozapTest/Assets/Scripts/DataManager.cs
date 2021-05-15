using System;
using System.IO;
using UnityEngine;

public class DataManager
{
    private const int TOTAL_LEVEL_COUNT = 4;
    private int _completedLevelsCount = 0;
    private int _selectedLevelIndex = 1;
    private BestScore _selectedLevelBestScore;
    private MapData _selectedLevelMapData;

    public int CompletedLevelsCount => _completedLevelsCount;
    public int TotalLevelCount => TOTAL_LEVEL_COUNT;
    public MapData SelectedLevelMapData => _selectedLevelMapData;

    public void Init()
    {
       if(PlayerPrefs.HasKey("CompletedLevelsCount"))
       {
            _completedLevelsCount = PlayerPrefs.GetInt("CompletedLevelsCount");
       }
    }

    private string getLocalFilePath(int pLevelNumber)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return Application.persistentDataPath + "/" + "Level_" + pLevelNumber + ".json";
        }
        else
        {
            return Application.dataPath + "/" + "Level_" + pLevelNumber + ".json";
        }
    }

    public MapData ReadSelectedLevelData()
    {
        string lJsonString = File.ReadAllText("Assets/Resources/LevelData/Level_"+ _selectedLevelIndex +".json");
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

    public void LevelCompleted(int pMinutes, int pSeconds)
    {
        if(_selectedLevelBestScore != null)
        {
            _selectedLevelBestScore.TotalPlaysCount++;
            if(pMinutes *60 + pSeconds < _selectedLevelBestScore.Minutes * 60 + _selectedLevelBestScore.Seconds)
            {
                _selectedLevelBestScore.Minutes = pMinutes;
                _selectedLevelBestScore.Seconds = pSeconds;
            }
        }
        else
        {
            _selectedLevelBestScore = new BestScore(pMinutes, pSeconds, 1);
            _completedLevelsCount++;
            PlayerPrefs.SetInt("CompletedLevelsCount", _completedLevelsCount);
        }
        File.WriteAllText(getLocalFilePath(_selectedLevelIndex) , JsonUtility.ToJson(_selectedLevelBestScore));
    }

    public BestScore ReadBestScoreData(int pIndex)
    {
        _selectedLevelIndex = pIndex;
        _selectedLevelBestScore = null;
        string lFilePath = getLocalFilePath(pIndex);
        if (File.Exists(lFilePath))
        {
            string lJsonString = File.ReadAllText(lFilePath);

            _selectedLevelBestScore = JsonUtility.FromJson<BestScore>(lJsonString);
        }
        return _selectedLevelBestScore;
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
    BOX_HOLDER_WITH_BOX = 6
}

[Serializable]
public class BestScore
{
    public int Minutes;
    public int Seconds;
    public int TotalPlaysCount;

    public BestScore(int pMinutes, int pSeconds, int pTotalPlaysCount)
    {
        Minutes= pMinutes;
        Seconds= pSeconds;
        TotalPlaysCount= pTotalPlaysCount;
    }
}