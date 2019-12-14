using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;

public class SaveParams : MonoBehaviour
{
  [SerializeField]
  private InputField timestepData;
  [SerializeField]
  private InputField scaleData;
  [SerializeField]
  private InputField xCoordData;
  [SerializeField]
  private InputField yCoordData;
  [SerializeField]
  private InputField zCoordData;
  [SerializeField]
  private InputField projectName;
  [SerializeField]
  private CanvasGroup mainPanel;
  [SerializeField]
  private GameObject overwritePanel;
  [SerializeField]
  private Text overwriteMessage;

  private bool popupAnswered;
  private bool overwrite;

  private void Start()
  {
    popupAnswered = false;
    overwrite = false;
  }

  private bool ValidateTimestep()
  {
    string inputVal = timestepData.text;
    if (inputVal.Length == 0)
    {
      timestepData.image.color = new Color32(255, 100, 100, 255);
      return false;
    }
    else
    {
      string listOfTimeStepNums = PlayerPrefs.GetString("listOfTimeSteps");
      string[] timeStepNums = listOfTimeStepNums.Trim().Split(' ');

      if (Array.Exists(timeStepNums, element => element == inputVal) || inputVal == "0")
      {
        timestepData.image.color = Color.white;
        return true;
      }
      else
      {
        timestepData.image.color = new Color32(255, 100, 100, 255);
        return false;
      }
    }
  }

  private bool ValidateScale()
  {
    string inputVal = scaleData.text;
    if (inputVal.Length == 0)
    {
      scaleData.image.color = new Color32(255, 100, 100, 255);
      return false;
    }
    else
    {
      if (int.Parse(inputVal) <= 0)
      {
        scaleData.image.color = new Color32(255, 100, 100, 255);
        return false;
      }
      else
      {
        scaleData.image.color = Color.white;
        return true;
      }
    }
  }

  private bool ValidateXCoord()
  {
    string xCoord = xCoordData.text;

    if (xCoord.Length == 0)
    {
      xCoordData.image.color = new Color32(255, 100, 100, 255);
      return false;
    }
    else
    {
      xCoordData.image.color = Color.white;
      return true;
    }
  }

  private bool ValidateYCoord()
  {
    string yCoord = yCoordData.text;

    if (yCoord.Length == 0)
    {
      yCoordData.image.color = new Color32(255, 100, 100, 255);
      return false;
    }
    else
    {
      yCoordData.image.color = Color.white;
      return true;
    }
  }

  private bool ValidateZCoord()
  {
    string zCoord = zCoordData.text;

    if (zCoord.Length == 0)
    {
      zCoordData.image.color = new Color32(255, 100, 100, 255);
      return false;
    }
    else
    {
      zCoordData.image.color = Color.white;
      return true;
    }
  }

  private bool ValidateProjectName()
  {
    string projName = projectName.text;

    if(projName.Length == 0)
    {
      projectName.image.color = new Color32(255, 100, 100, 255);
      return false;
    }
    else
    {
      projectName.image.color = Color.white;
      return true;
    }
  }

  private void SaveTimestep()
  {
    PlayerPrefs.SetInt("initTimestep", int.Parse(timestepData.text));
  }

  private void SaveScale()
  {
    PlayerPrefs.SetInt("initScale", int.Parse(scaleData.text));
  }

  private void SaveXCoordinate()
  {
    PlayerPrefs.SetInt("initXCoord", int.Parse(xCoordData.text));
  }

  private void SaveYCoordinate()
  {
    PlayerPrefs.SetInt("initYCoord", int.Parse(yCoordData.text));
  }

  private void SaveZCoordinate()
  {
    PlayerPrefs.SetInt("initZCoord", int.Parse(zCoordData.text));
  }

  private void SaveProjectName()
  {
    PlayerPrefs.SetString("projectName", projectName.text);
  }

  public void ValidateParams()
  {
    if (ValidateTimestep() & ValidateScale() & ValidateXCoord() & ValidateYCoord() & ValidateZCoord() & ValidateProjectName())
    {
      if (ProjectExists())
      {
        StartCoroutine(StartOverwritePopup());
      }
      else
      {
        SaveTimestep();
        SaveScale();
        SaveXCoordinate();
        SaveYCoordinate();
        SaveZCoordinate();
        SaveProjectName();
        ChangeScene.SetScene(3);
      }
    }
  }

  private IEnumerator StartOverwritePopup()
  {
    ShowOverwritePopup();
    while (!popupAnswered)
    {
      yield return null;
    }
    if (overwrite)
    {
      SaveTimestep();
      SaveScale();
      SaveXCoordinate();
      SaveYCoordinate();
      SaveZCoordinate();
      SaveProjectName();
      ChangeScene.SetScene(3);
    }
  }

  private bool ProjectExists()
  {
    return Directory.Exists(Path.Combine(Application.persistentDataPath, "Projects", projectName.text));
  }

  public void PopupCancel()
  {
    overwrite = false;
    HideOverwritePopup();
  }

  public void PopupContinue()
  {
    overwrite = true;
    HideOverwritePopup();
  }

  private void ShowOverwritePopup()
  {
    DisableMainPanel();
    popupAnswered = false;
    overwritePanel.SetActive(true);
    overwriteMessage.text = "A Project Named '" + projectName.text + "' Already Exists. Continuing Will Overwrite All Existing Files In The '" + projectName.text + "' Project Folder. Continue?";
  }

  private void HideOverwritePopup()
  {
    popupAnswered = true;
    overwritePanel.SetActive(false);
    EnableMainPanel();
  }

  private void DisableMainPanel()
  {
    mainPanel.interactable = false;
  }

  private void EnableMainPanel()
  {
    mainPanel.interactable = true;
  }
}
