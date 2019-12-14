using UnityEngine;

[System.Serializable]
public class ProjectData
{
  public string projectName;
  public string listOfTimeSteps;
  public string listOfGameObjectNames;
  public string listOfBinaryFiles;
  public string userFilePaths;
  public int initTimestep;
  public int initXCoord;
  public int initYCoord;
  public int initZCoord;
  public int hudCoordToggle;
  public int hudRefAxisToggle;
  public int hudPlaybackToggle;
  public int hudLegendToggle;
  public int firstTimestep;
  public int lastTimestep;
  public int initScale;
  public int timeStepCount;
  public string listOfGameObjectColors; 
  public string waypointInfo;
  public string listOfGameObjectTransforms;

  /** ProjectData constructor 
   *  Creates ProjectData object from the current session's PlayerPrefs
   **/
  public ProjectData()
  {
    projectName = PlayerPrefs.GetString("projectName");
    listOfTimeSteps = PlayerPrefs.GetString("listOfTimeSteps");
    listOfGameObjectNames = PlayerPrefs.GetString("listOfGameObjectNames");
    listOfBinaryFiles = PlayerPrefs.GetString("listOfBinaryFiles");
    userFilePaths = PlayerPrefs.GetString("userFilePaths");
    initTimestep = PlayerPrefs.GetInt("initTimestep");
    initXCoord = PlayerPrefs.GetInt("initXCoord");
    initYCoord = PlayerPrefs.GetInt("initYCoord");
    initZCoord = PlayerPrefs.GetInt("initZCoord");
    hudCoordToggle = PlayerPrefs.GetInt("hudCoordToggle");
    hudRefAxisToggle = PlayerPrefs.GetInt("hudRefAxisToggle");
    hudPlaybackToggle = PlayerPrefs.GetInt("hudPlaybackToggle");
    hudLegendToggle = PlayerPrefs.GetInt("hudLegendToggle");
    firstTimestep = PlayerPrefs.GetInt("firstTimestep");
    lastTimestep = PlayerPrefs.GetInt("lastTimestep");
    initScale = PlayerPrefs.GetInt("initScale");
    timeStepCount = PlayerPrefs.GetInt("timeStepCount");
    listOfGameObjectColors = PlayerPrefs.GetString("listOfGameObjectColors"); 
    waypointInfo = PlayerPrefs.GetString("waypointInfo");
    listOfGameObjectTransforms = PlayerPrefs.GetString("listOfGameObjectTransforms");
  }
}
