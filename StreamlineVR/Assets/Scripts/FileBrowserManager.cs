using UnityEngine;
using System.Collections;
using SimpleFileBrowser;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class FileBrowserManager : MonoBehaviour
{
  [SerializeField]
  private Image selectedFilesBackground;
  [SerializeField]
  private RectTransform selectedFilesContainer;
  [SerializeField]
  private RectTransform oldPathContainer;
  [SerializeField]
  private RectTransform newPathContainer;
  [SerializeField]
  private Font gothicFont;
  [SerializeField]
  private Toggle folderSelectionModeToggle;
  [SerializeField]
  private Text errorText;
  [SerializeField]
  private CanvasGroup mainPanel;
  [SerializeField]
  private GameObject overwritePanel;
  [SerializeField]
  private GameObject quickLinkContainer;
  [SerializeField]
  private GameObject filesContainer;
  [SerializeField]
  private ScrollRect quickLinksScroll;
  [SerializeField]
  private ScrollRect filesScroll;
  [SerializeField]
  private ScrollRect selectedFilesScroll;
  [SerializeField]
  private Text overwriteMessage;

  private HashSet<string> selectedFilePaths;
  private List<int> timeStepsList;
  private string overwriteOldPath;
  private string overwriteNewPath;
  private bool popupAnswered;

  private void Start()
  {
    selectedFilePaths = new HashSet<string>();
    timeStepsList = new List<int>();
    LoadExistingSelectedFileInfo();
    ResetFilePlayerPrefs();
    FileBrowser.CustomLoadDialog(SuccessCallback, ClearSelectedFilesInfo, false, null, "Select Folder", "Select");
    FileBrowser.SetFilters(true, new FileBrowser.Filter("x3d", ".x3d"));
    FileBrowser.SetDefaultFilter(".x3d");
    //FileBrowser.SingleClickMode = true;
  }

  public void PopupKeep()
  {
    HideOverwritePopup();
  }

  public void PopupOverwrite()
  {
    selectedFilePaths.Remove(overwriteOldPath);
    selectedFilePaths.Add(overwriteNewPath);
    ClearSelectedFilesContainer();
    foreach (string selectedFilePath in selectedFilePaths)
    {
      AddToSelectedFilesContainer(selectedFilePath);
    }
    HideOverwritePopup();
  }

  private void ShowOverwritePopup(string timeStepNum)
  {
    DisableMainPanel();
    popupAnswered = false;
    overwritePanel.SetActive(true);
    ClearOldPathContainer();
    ClearNewPathContainer();
    overwriteMessage.text = "A Time Step " + timeStepNum + " File Is Already Selected. Overwrite Path?";
    AddToOldPathContainer(overwriteOldPath);
    AddToNewPathContainer(overwriteNewPath);
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
    FileBrowserQuickLink[] quickLinkScripts = quickLinkContainer.GetComponentsInChildren<FileBrowserQuickLink>();
    foreach (FileBrowserQuickLink quickLinkScript in quickLinkScripts)
    {
      quickLinkScript.enabled = false;
    }
    FileBrowserItem[] itemScripts = filesContainer.GetComponentsInChildren<FileBrowserItem>();
    foreach (FileBrowserItem itemScript in itemScripts)
    {
      itemScript.enabled = false;
    }
    quickLinksScroll.scrollSensitivity = 0;
    filesScroll.scrollSensitivity = 0;
    selectedFilesScroll.scrollSensitivity = 0;
  }

  private void EnableMainPanel()
  {
    mainPanel.interactable = true;
    FileBrowserQuickLink[] quickLinkScripts = quickLinkContainer.GetComponentsInChildren<FileBrowserQuickLink>();
    foreach (FileBrowserQuickLink quickLinkScript in quickLinkScripts)
    {
      quickLinkScript.enabled = true;
    }
    FileBrowserItem[] itemScripts = filesContainer.GetComponentsInChildren<FileBrowserItem>();
    foreach (FileBrowserItem itemScript in itemScripts)
    {
      itemScript.enabled = true;
    }
    quickLinksScroll.scrollSensitivity = 35;
    filesScroll.scrollSensitivity = 35;
    selectedFilesScroll.scrollSensitivity = 35;
  }

  private void ResetFilePlayerPrefs()
  {
    PlayerPrefs.SetString("userFilePaths", "");
    PlayerPrefs.SetInt("firstTimestep", 0);
    PlayerPrefs.SetInt("lastTimestep", 0);
    PlayerPrefs.SetInt("timeStepCount", 0);
    PlayerPrefs.SetString("listOfTimeSteps", "");
  }

  private void LoadExistingSelectedFileInfo()
  {
    string userFilePaths = PlayerPrefs.GetString("userFilePaths");
    if (userFilePaths != "")
    {
      string[] filePaths = userFilePaths.Remove(userFilePaths.Length - 1).Split(',');
      foreach (string filePath in filePaths)
      {
        selectedFilePaths.Add(filePath);
        AddToSelectedFilesContainer(filePath);
        AddToTimeStepsList(UnpackTimeStepNum(filePath));
      }
    }
  }

  public void FolderSelectionModeToggled()
  {
    if (folderSelectionModeToggle.isOn)
    {
      FileBrowser.ToggleFolderSelectionMode(true, null);
    }
    else
    {
      FileBrowser.ToggleFolderSelectionMode(false, ".x3d");
    }
  }

  private void ClearOldPathContainer()
  {
    foreach (Transform oldPath in oldPathContainer.transform)
    {
      Destroy(oldPath.gameObject);
    }
    oldPathContainer.sizeDelta = new Vector2(0, 0);
  }

  private void ClearNewPathContainer()
  {
    foreach (Transform newPath in newPathContainer.transform)
    {
      Destroy(newPath.gameObject);
    }
    newPathContainer.sizeDelta = new Vector2(0, 0);
  }

  private void ClearSelectedFilesContainer()
  {
    foreach (Transform selectedFile in selectedFilesContainer.transform)
    {
      Destroy(selectedFile.gameObject);
    }
    selectedFilesContainer.sizeDelta = new Vector2(0, 0);
  }

  private void ClearSelectedFilesInfo()
  {
    ClearSelectedFilesContainer();
    selectedFilePaths.Clear();
    timeStepsList.Clear();
  }

  private void AddToOldPathContainer(string path)
  {
    GameObject oldPath = new GameObject("OldPath");
    oldPath.transform.SetParent(oldPathContainer);
    RectTransform rectTransform = oldPath.AddComponent<RectTransform>();
    rectTransform.anchorMin = new Vector2(0, 1);
    rectTransform.anchorMax = new Vector2(0, 1);
    rectTransform.pivot = new Vector2(0, 1);
    rectTransform.localPosition = new Vector3(3, -oldPathContainer.rect.height, 0);
    rectTransform.localScale = new Vector3(1, 1, 1);
    Text text = oldPath.AddComponent<Text>();
    text.font = gothicFont;
    text.color = Color.black;
    text.fontSize = 25;
    text.text = path;
    rectTransform.sizeDelta = new Vector2(text.preferredWidth, 0);
    rectTransform.sizeDelta = new Vector2(text.preferredWidth + 7, text.preferredHeight);
    oldPathContainer.sizeDelta = new Vector2(Mathf.Max(oldPathContainer.rect.width, rectTransform.rect.width), oldPathContainer.rect.height + rectTransform.rect.height);
  }

  private void AddToNewPathContainer(string path)
  {
    GameObject newPath = new GameObject("SelectedFile");
    newPath.transform.SetParent(newPathContainer);
    RectTransform rectTransform = newPath.AddComponent<RectTransform>();
    rectTransform.anchorMin = new Vector2(0, 1);
    rectTransform.anchorMax = new Vector2(0, 1);
    rectTransform.pivot = new Vector2(0, 1);
    rectTransform.localPosition = new Vector3(3, -newPathContainer.rect.height, 0);
    rectTransform.localScale = new Vector3(1, 1, 1);
    Text text = newPath.AddComponent<Text>();
    text.font = gothicFont;
    text.color = Color.black;
    text.fontSize = 25;
    text.text = path;
    rectTransform.sizeDelta = new Vector2(text.preferredWidth, 0);
    rectTransform.sizeDelta = new Vector2(text.preferredWidth + 7, text.preferredHeight);
    newPathContainer.sizeDelta = new Vector2(Mathf.Max(newPathContainer.rect.width, rectTransform.rect.width), newPathContainer.rect.height + rectTransform.rect.height);
  }

  private void AddToSelectedFilesContainer(string path)
  {
    GameObject selectedFile = new GameObject("SelectedFile");
    selectedFile.transform.SetParent(selectedFilesContainer);
    RectTransform rectTransform = selectedFile.AddComponent<RectTransform>();
    rectTransform.anchorMin = new Vector2(0, 1);
    rectTransform.anchorMax = new Vector2(0, 1);
    rectTransform.pivot = new Vector2(0, 1);
    rectTransform.localPosition = new Vector3(3, -selectedFilesContainer.rect.height, 0);
    rectTransform.localScale = new Vector3(1, 1, 1);
    Text text = selectedFile.AddComponent<Text>();
    text.font = gothicFont;
    text.color = Color.black;
    text.fontSize = 25;
    text.text = path;
    rectTransform.sizeDelta = new Vector2(text.preferredWidth, 0);
    rectTransform.sizeDelta = new Vector2(text.preferredWidth + 7, text.preferredHeight);
    selectedFilesContainer.sizeDelta = new Vector2(Mathf.Max(selectedFilesContainer.rect.width, rectTransform.rect.width), selectedFilesContainer.rect.height + rectTransform.rect.height);
  }

  private string UnpackTimeStepNum(string path)
  {
    //unpack timestep num
    string timeStepNum = "";
    if (path.Contains("/"))
    {
      //linux
      timeStepNum = path.Substring(path.LastIndexOf('/')).Replace("/", "").Replace(".x3d", "").Replace("timestep", "");
    }
    else if (path.Contains("\\"))
    {
      //windows
      timeStepNum = path.Substring(path.LastIndexOf('\\')).Replace("\\", "").Replace(".x3d", "").Replace("timestep", "");
    }
    else
    {
      //bad path
      Debug.Log("Something went wrong! Bad path!");
    }
    return timeStepNum;
  }

  private string UnpackFileName(string path)
  {
    //unpack filename
    string fileName = "";
    if (path.Contains("/"))
    {
      //linux
      fileName = path.Substring(path.LastIndexOf('/')).Replace("/", "").Replace(".x3d", "");
    }
    else if (path.Contains("\\"))
    {
      //windows
      fileName = path.Substring(path.LastIndexOf('\\')).Replace("\\", "").Replace(".x3d", "");
    }
    else
    {
      //bad path
      Debug.Log("Something went wrong! Bad path!");
    }
    return fileName;
  }

  private bool AddToTimeStepsList(string timeStepNum)
  {
    int timeStepToAdd = int.Parse(timeStepNum);
    if (timeStepsList.Contains(timeStepToAdd))
    {
      return false;
    }
    timeStepsList.Add(timeStepToAdd);
    return true;
  }

  /// <summary>
  /// Callback function that is called when there is a successful file selection.
  /// </summary>
  /// <param name="path"></param>
  private void SuccessCallback(string path)
  {
    StartCoroutine(TryToAddFile(path));
  }

  private IEnumerator TryToAddFile(string path)
  {
    selectedFilesBackground.color = Color.white;
    errorText.text = "";
    Regex correctFileName = new Regex(@"timestep\d+");
    if (File.Exists(path))
    {
      if (selectedFilePaths.Contains(path))
      {
        //Debug.Log("PATH ALREADY SELECTED");
        //message? or do nothing?
        errorText.text = "Selected file(s) is already selected.";
      }
      else if (!correctFileName.IsMatch(UnpackFileName(path)))
      {
        //Debug.Log("IMPROPER FILE NAME");
        //message maybe?
        errorText.text = "Selected file(s) is improperly named.";
      }
      else
      {
        string timeStepNum = UnpackTimeStepNum(path);
        if (!AddToTimeStepsList(timeStepNum))
        {
          //get old path
          foreach (string selectedFilePath in selectedFilePaths)
          {
            string selectedFilePathNum = UnpackTimeStepNum(selectedFilePath);
            if (string.Compare(timeStepNum, selectedFilePathNum) == 0)
            {
              overwriteOldPath = selectedFilePath;
              break;
            }
          }
          overwriteNewPath = path;
          ShowOverwritePopup(timeStepNum);
          while (!popupAnswered)
          {
            yield return null;
          }
        }
        else
        {
          selectedFilePaths.Add(path);
          AddToSelectedFilesContainer(path);
        }
      }
    }
    else if (Directory.Exists(path))
    {
      string[] filePaths = Directory.GetFiles(path, @"*.x3d", SearchOption.TopDirectoryOnly);
      foreach (string filePath in filePaths)
      {
        if (selectedFilePaths.Contains(filePath))
        {
          //Debug.Log("PATH ALREADY SELECTED");
          //message? or do nothing?
          errorText.text = "At least one selected file is already selected.";
        }
        else if (!correctFileName.IsMatch(UnpackFileName(filePath)))
        {
          //Debug.Log("IMPROPER FILE NAME");
          //message maybe?
          errorText.text = "At least one selected file is improperly named.";
        }
        else
        {
          string timeStepNum = UnpackTimeStepNum(filePath);
          if (!AddToTimeStepsList(timeStepNum))
          {
            //get old path
            foreach (string selectedFilePath in selectedFilePaths)
            {
              string selectedFilePathNum = UnpackTimeStepNum(selectedFilePath);
              if (string.Compare(timeStepNum, selectedFilePathNum) == 0)
              {
                overwriteOldPath = selectedFilePath;
                break;
              }
            }
            overwriteNewPath = filePath;
            ShowOverwritePopup(timeStepNum);
            while (!popupAnswered)
            {
              yield return null;
            }
          }
          else
          {
            selectedFilePaths.Add(filePath);
            AddToSelectedFilesContainer(filePath);
          }
        }
      }
    }
  }

  public void ExtactFromSelectedFiles()
  {
    if (selectedFilePaths.Count > 0)
    {
      /* Save the path to each selected file in playerPrefs */
      string allPaths = "";
      foreach (string selectedFilePath in selectedFilePaths)
      {
        allPaths = allPaths + selectedFilePath + ",";
      }
      PlayerPrefs.SetString("userFilePaths", allPaths);

      //populate timestep playerPrefs info
      timeStepsList.Sort();
      PlayerPrefs.SetInt("firstTimestep", timeStepsList[0]);
      PlayerPrefs.SetInt("lastTimestep", timeStepsList[timeStepsList.Count - 1]);
      PlayerPrefs.SetInt("timeStepCount", timeStepsList.Count);
      foreach (int timeStep in timeStepsList)
      {
        string listOfTimeSteps = PlayerPrefs.GetString("listOfTimeSteps");
        PlayerPrefs.SetString("listOfTimeSteps", listOfTimeSteps + timeStep + " ");
      }

      //go to parameter screen
      ChangeScene.SetScene(2);
    }
    else
    {
      selectedFilesBackground.color = new Color32(255, 100, 100, 255);
      errorText.text = "No files selected. Please select at least one.";
    }
  }

  private IEnumerator ShowLoadDialogCoroutine()
  {
    // Show a load file dialog and wait for a response from user
    // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
    yield return FileBrowser.ShowLoadDialog(SuccessCallback, () => { Debug.Log("Canceled"); },
                                   true, null, "Select Folder", "Select");

    // Dialog is closed
    // Print whether a file is chosen (FileBrowser.Success)
    // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
    Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);
  }

}
