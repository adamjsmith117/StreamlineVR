using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlaybackManager : MonoBehaviour
{
  [SerializeField]
  private Image playForwardButton;
  [SerializeField]
  private Image playBackwardButton;
  [SerializeField]
  private Button loopButton;
  [SerializeField]
  private Sprite pauseSprite;
  [SerializeField]
  private Sprite playForwardSprite;
  [SerializeField]
  private Sprite playBackwardSprite;
  [SerializeField]
  private Slider timeStepMarker;
  [SerializeField]
  private Text currentTimeStepText;
  [SerializeField]
  private Text currentTimeStepIndexText;
  [SerializeField]
  private Text timeStepCountText;
  [SerializeField]
  private Text playDelayText;
  [SerializeField]
  private Text hudCurrentTimeStepText;
  [SerializeField]
  private Text hudCurrentTimeStepIndexText;
  [SerializeField]
  private Text hudTimeStepCountText;
  [SerializeField]
  private GameObject playPanel;
  [SerializeField]
  private GameObject menuPanel;
  [SerializeField]
  private Text hudCurrentSelectedGameObjectName;
  [SerializeField]
  private Button scaleButton;
  [SerializeField]
  private Button transButton;
  [SerializeField]
  private Button colorButton;
  [SerializeField]
  private Button wayButton;
  [SerializeField]
  private Button selectButton;
  [SerializeField]
  private Button tAndRButton;
  [SerializeField]
  private Button settButton;

  private static Dictionary<string, GameObject> timeStepGameObjects;
  private List<string> timeSteps;
  public static string currentTimeStep;
  private int currentTimeStepIndex;
  private int timeStepCount;
  private int playDelay;
  private float playDelayInSeconds;
  private bool doLoop;
  private bool playingForward;
  private bool playingBackward;

  private void Start()
  {
    timeStepGameObjects = new Dictionary<string, GameObject>();
    timeSteps = new List<string>();
    string listOfTimeStepNums = PlayerPrefs.GetString("listOfTimeSteps");
    string[] timeStepNums = listOfTimeStepNums.Trim().Split(' ');
    foreach(string timeStepNum in timeStepNums)
    {
      timeSteps.Add(timeStepNum);
    }
    int initTimestep = PlayerPrefs.GetInt("initTimestep");
    if(initTimestep == 0)
    {
      currentTimeStep = PlayerPrefs.GetInt("firstTimestep").ToString();
    }
    else
    {
      currentTimeStep = initTimestep.ToString();
    }
    currentTimeStepText.text = currentTimeStep;
    hudCurrentTimeStepText.text = currentTimeStep;
    currentTimeStepIndex = timeSteps.IndexOf(currentTimeStep);
    currentTimeStepIndexText.text = (currentTimeStepIndex + 1).ToString();
    hudCurrentTimeStepIndexText.text = (currentTimeStepIndex + 1).ToString();
    timeStepCount = PlayerPrefs.GetInt("timeStepCount");
    timeStepCountText.text = timeStepCount.ToString();
    hudTimeStepCountText.text = timeStepCount.ToString();
    UpdateTimeStepMarker();
    playDelay = 5;
    playDelayText.text = "1x";
    playDelayInSeconds = 0.5f;
    doLoop = true;
  }

  public static void SetTimeStepGameObjects(Dictionary<string, GameObject> dictionary)
  {
    timeStepGameObjects = dictionary;
    foreach (KeyValuePair<string, GameObject> pair in timeStepGameObjects)
    {
      if (pair.Key != "TimeStep" + currentTimeStep)
      {
        pair.Value.SetActive(false);
      }
    }
  }

  public void EnterMenu()
  {
    menuPanel.SetActive(false);
    playPanel.SetActive(true);
  }

  public void ExitMenu()
  {
    playPanel.SetActive(false);
    if (playingForward || playingBackward)
    {
      //disable tool buttons
      scaleButton.interactable = false;
      transButton.interactable = false;
      colorButton.interactable = false;
      wayButton.interactable = false;
      selectButton.interactable = false;
      tAndRButton.interactable = false;
      settButton.interactable = false;
    }
    else
    {
      //enable tool buttons
      scaleButton.interactable = true;
      transButton.interactable = true;
      colorButton.interactable = true;
      wayButton.interactable = true;
      selectButton.interactable = true;
      tAndRButton.interactable = true;
      settButton.interactable = true;
    }
    menuPanel.SetActive(true);
  }

  public void ToggleLooping()
  {
    ColorBlock colors = loopButton.colors;
    if(doLoop)
    {
      colors.normalColor = Color.white;
    }
    else
    {
      colors.normalColor = new Color(0.1568628f, 0.5568628f, 0.9490196f, 1f);
    }
    loopButton.colors = colors;
    doLoop = !doLoop;
  }

  public void IncreasePlaySpeed()
  {
    if(playDelay > 1)
    {
      playDelay -= 1;
      playDelayText.text = (5f / playDelay).ToString("G2") + "x";
      playDelayInSeconds = playDelay / 10f;
    }
  }

  public void DecreasePlaySpeed()
  {
    if(playDelay < 10)
    {
      playDelay += 1;
      playDelayText.text = (5f / playDelay).ToString("G2") + "x";
      playDelayInSeconds = playDelay / 10f;
    }
  }

  public void ToggleForwardPlay()
  {
    if(playingBackward)
    {
      StopCoroutine("PlayBackward");
      playingBackward = false;
      playBackwardButton.sprite = playBackwardSprite;
    }
    if(!playingForward)
    {
      StartCoroutine("PlayForward");
      playingForward = true;
      playForwardButton.sprite = pauseSprite;
    }
    else
    {
      StopCoroutine("PlayForward");
      playingForward = false;
      playForwardButton.sprite = playForwardSprite;
    }
  }

  private IEnumerator PlayForward()
  {
    while(true)
    {
      NextTimeStep();
      yield return new WaitForSecondsRealtime(playDelayInSeconds);
    }
  }

  public void ToggleBackwardPlay()
  {
    if(playingForward)
    {
      StopCoroutine("PlayForward");
      playingForward = false;
      playForwardButton.sprite = playForwardSprite;
    }
    if(!playingBackward)
    {
      StartCoroutine("PlayBackward");
      playingBackward = true;
      playBackwardButton.sprite = pauseSprite;
    }
    else
    {
      StopCoroutine("PlayBackward");
      playingBackward = false;
      playBackwardButton.sprite = playBackwardSprite;
    }
  }

  private IEnumerator PlayBackward()
  {
    while(true)
    {
      PrevTimeStep();
      yield return new WaitForSecondsRealtime(playDelayInSeconds);
    }
  }

  public void FirstTimeStep()
  {
    string firstTimeStep = PlayerPrefs.GetInt("firstTimestep").ToString();
    if(string.Compare(currentTimeStep, firstTimeStep) != 0)
    {
      TimeStepToHide(currentTimeStep);
      TimeStepToShow(firstTimeStep);
      UpdateTimeStepMarker();
    }
  }

  public void LastTimeStep()
  {
    string lastTimeStep = PlayerPrefs.GetInt("lastTimestep").ToString();
    if(string.Compare(currentTimeStep, lastTimeStep) != 0)
    {
      TimeStepToHide(currentTimeStep);
      TimeStepToShow(lastTimeStep);
      UpdateTimeStepMarker();
    }
  }

  public void NextTimeStep()
  {
    for(int i = 0; i < timeStepCount; i++)
    {
      if(string.Compare(timeSteps[i], currentTimeStep) == 0)
      {
        TimeStepToHide(timeSteps[i]);
        string newTimeStep;
        if(i == (timeStepCount - 1))
        {
          if(doLoop)
          {
            newTimeStep = timeSteps[0];
            QueueManager.ExhaustQueue("timestep" + newTimeStep);
          }
          else
          {
            newTimeStep = currentTimeStep;
          }
        }
        else
        {
          newTimeStep = timeSteps[i + 1];
          QueueManager.ExhaustQueue("timestep" + newTimeStep);
        }
        TimeStepToShow(newTimeStep);
        UpdateTimeStepMarker();
        break;
      }
    }
  }

  public void PrevTimeStep()
  {
    for(int i = 0; i < timeStepCount; i++)
    {
      if(string.Compare(timeSteps[i], currentTimeStep) == 0)
      {
        TimeStepToHide(timeSteps[i]);
        string newTimeStep;
        if(i == 0)
        {
          if(doLoop)
          {
            newTimeStep = timeSteps[timeStepCount - 1];
            QueueManager.ExhaustQueue("timestep" + newTimeStep);
          }
          else
          {
            newTimeStep = currentTimeStep;
          }
        }
        else
        {
          newTimeStep = timeSteps[i - 1];
          QueueManager.ExhaustQueue("timestep" + newTimeStep);
        }
        TimeStepToShow(newTimeStep);
        UpdateTimeStepMarker();
        break;
      }
    }
  }

  private void UpdateTimeStepMarker()
  {
    //deactivate selection outline on current selected game object
    if (SelectionManager.currentSelectedGameObjects != null && SelectionManager.currentSelectedGameObjects.Count > 0)
    {
      foreach (GameObject currentSelectedGameObject in SelectionManager.currentSelectedGameObjects)
      {
        //update toggle on selection menu
        SelectionManager.CustomGameObject customGameObject = SelectionManager.customGameObjects[SelectionManager.gameObjectIds[currentSelectedGameObject.name] - 1];
        customGameObject.selected = false;
        SelectionManager.customGameObjects[SelectionManager.gameObjectIds[currentSelectedGameObject.name] - 1] = customGameObject;
      }
      SelectionManager.currentSelectedGameObjects.Clear();
      hudCurrentSelectedGameObjectName.text = "None";
    }
    if (timeStepCount > 1)
    {
      timeStepMarker.value = (float)currentTimeStepIndex / (timeStepCount - 1);
    }
    else
    {
      timeStepMarker.value = 1;
    }
  }

  private void TimeStepToHide( string timeStepToHide )
  {
    //new hiding time steps
    timeStepGameObjects["TimeStep" + timeStepToHide].SetActive(false);
  }

  private void TimeStepToShow( string timeStepToShow )
  {
    //new showing time steps
    timeStepGameObjects["TimeStep" + timeStepToShow].SetActive(true);
    currentTimeStep = timeStepToShow;
    currentTimeStepText.text = currentTimeStep;
    hudCurrentTimeStepText.text = currentTimeStep;
    currentTimeStepIndex = timeSteps.IndexOf(currentTimeStep);
    currentTimeStepIndexText.text = (currentTimeStepIndex + 1).ToString();
    hudCurrentTimeStepIndexText.text = (currentTimeStepIndex + 1).ToString();
  }
}
