using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
  public void Start()
  {
    PlayerPrefs.SetString("listOfTimeSteps", "");
    PlayerPrefs.SetString("listOfGameObjectNames", "");
    PlayerPrefs.SetString("listOfBinaryFiles", "");
    PlayerPrefs.SetString("userFilePaths", "");
    PlayerPrefs.SetInt("initTimestep", 0);
    PlayerPrefs.SetInt("initXCoord", 0);
    PlayerPrefs.SetInt("initYCoord", 0);
    PlayerPrefs.SetInt("initZCoord", 0);
    PlayerPrefs.SetInt("hudCoordToggle", 0);
    PlayerPrefs.SetInt("hudRefAxisToggle", 0);
    PlayerPrefs.SetInt("hudPlaybackToggle", 0);
    PlayerPrefs.SetInt("hudLegendToggle", 0);
    PlayerPrefs.SetInt("firstTimestep", 0);
    PlayerPrefs.SetInt("lastTimestep", 0);
    PlayerPrefs.SetInt("timeStepCount", 0);
    PlayerPrefs.SetInt("initScale", 1);
    PlayerPrefs.SetString("projectName", "");
    PlayerPrefs.SetString("waypointInfo", "");
    PlayerPrefs.SetString("listOfGameObjectColors", "");
    PlayerPrefs.SetString("listOfGameObjectTransforms", "");
    PlayerPrefs.SetFloat("shiftedX", 0);
    PlayerPrefs.SetFloat("shiftedY", 0);
    PlayerPrefs.SetFloat("shiftedZ", 0);
    PlayerPrefs.SetFloat("MaxMeshLength", 0);
    PlayerPrefs.SetFloat("MaxMeshWidth", 0);
    PlayerPrefs.SetFloat("MaxMeshHeight", 0);
  }

  public void PrintTimeSteps()
  {
    Debug.Log("Timestep Numbers: " + PlayerPrefs.GetString("listOfTimeSteps"));
  }

  public void PrintGameObjectNames()
  {
    Debug.Log("Game Object Names: " + PlayerPrefs.GetString("listOfGameObjectNames"));
  }

  public void PrintBinaryFiles()
  {
    Debug.Log("Binary Files: " + PlayerPrefs.GetString("listOfBinaryFiles"));
  }

  public void PrintUserFilePaths()
  {
    Debug.Log("User File Paths: " + PlayerPrefs.GetString("userFilePaths"));
  }

  public void PrintInitTimestep()
  {
    Debug.Log("Init Timestep: " + PlayerPrefs.GetInt("initTimestep"));
  }

  public void PrintInitXCoord()
  {
    Debug.Log("Init X Coord: " + PlayerPrefs.GetInt("initXCoord"));
  }

  public void PrintInitYCoord()
  {
    Debug.Log("Init Y Coord: " + PlayerPrefs.GetInt("initYCoord"));
  }

  public void PrintInitZCoord()
  {
    Debug.Log("Init Z Coord: " + PlayerPrefs.GetInt("initZCoord"));
  }

  public void PrintHUDCoordToggle()
  {
    Debug.Log("HUD Coord: " + PlayerPrefs.GetInt("hudCoordToggle"));
  }

  public void PrintHUDRefAxisToggle()
  {
    Debug.Log("HUD Ref Axis: " + PlayerPrefs.GetInt("hudRefAxisToggle"));
  }

  public void PrintHUDPlaybackToggle()
  {
    Debug.Log("HUD Playback: " + PlayerPrefs.GetInt("hudPlaybackToggle"));
  }

  public void PrintHUDLegendToggle()
  {
    Debug.Log("HUD Legend: " + PlayerPrefs.GetInt("hudLegendToggle"));
  }

  public void PrintFirstTimestep()
  {
    Debug.Log("First Timestep: " + PlayerPrefs.GetInt("firstTimestep"));
  }

  public void PrintLastTimestep()
  {
    Debug.Log("Last Timestep: " + PlayerPrefs.GetInt("lastTimestep"));
  }

  public void PrintTimestepCount()
  {
    Debug.Log("Timestep count: " + PlayerPrefs.GetInt("timeStepCount"));
  }

  public void PrintInitScale()
  {
    Debug.Log("Init Scale: " + PlayerPrefs.GetInt("initScale"));
  }

  public void PrintInitInfo()
  {
    PrintInitTimestep();
    PrintInitScale();
    PrintInitXCoord();
    PrintInitYCoord();
    PrintInitZCoord();
  }

  public void PrintHUDInfo()
  {
    PrintHUDCoordToggle();
    PrintHUDRefAxisToggle();
    PrintHUDPlaybackToggle();
    PrintHUDLegendToggle();
  }

  public void PrintTimestepInfo()
  {
    PrintTimeSteps();
    PrintFirstTimestep();
    PrintLastTimestep();
    PrintTimestepCount();
  }

  public void PrintAll()
  {
    PrintTimestepInfo();
    PrintGameObjectNames();
    PrintBinaryFiles();
    PrintUserFilePaths();
    PrintInitInfo();
    PrintHUDInfo();
  }

  public static void PrintAllInfo()
  {
    Debug.Log("Timestep Numbers: " + PlayerPrefs.GetString("listOfTimeSteps"));
    Debug.Log("First Timestep: " + PlayerPrefs.GetInt("firstTimestep"));
    Debug.Log("Last Timestep: " + PlayerPrefs.GetInt("lastTimestep"));
    Debug.Log("Timestep count: " + PlayerPrefs.GetInt("timeStepCount"));
    Debug.Log("Game Object Names: " + PlayerPrefs.GetString("listOfGameObjectNames"));
    Debug.Log("Binary Files: " + PlayerPrefs.GetString("listOfBinaryFiles"));
    Debug.Log("User File Paths: " + PlayerPrefs.GetString("userFilePaths"));
    Debug.Log("Init Timestep: " + PlayerPrefs.GetInt("initTimestep"));
    Debug.Log("Init Scale: " + PlayerPrefs.GetInt("initScale"));
    Debug.Log("Init X Coord: " + PlayerPrefs.GetInt("initXCoord"));
    Debug.Log("Init Y Coord: " + PlayerPrefs.GetInt("initYCoord"));
    Debug.Log("Init Z Coord: " + PlayerPrefs.GetInt("initZCoord"));
    Debug.Log("HUD Coord: " + PlayerPrefs.GetInt("hudCoordToggle"));
    Debug.Log("HUD Ref Axis: " + PlayerPrefs.GetInt("hudRefAxisToggle"));
    Debug.Log("HUD Playback: " + PlayerPrefs.GetInt("hudPlaybackToggle"));
    Debug.Log("HUD Legend: " + PlayerPrefs.GetInt("hudLegendToggle"));
  }

  public void SaveData()
  {
    PlayerPrefs.Save();
  }
}
