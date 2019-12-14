using UnityEngine;
using System.Collections;
using SimpleFileBrowser;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LoadProject : MonoBehaviour
{

  public void OpenDialogue()
  {
    string projectsPath = Path.Combine(Application.persistentDataPath, "Projects");
    Debug.Log(projectsPath);
    if (!Directory.Exists(projectsPath))
    {
      Directory.CreateDirectory(projectsPath);
    }

    // Set filters (optional)
    // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
    // if all the dialogs will be using the same filters
    //FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"), new FileBrowser.Filter("Text Files", ".txt", ".pdf"));
    FileBrowser.SetFilters(false, new FileBrowser.Filter("Projects", ".pf"));

    // Set default filter that is selected when the dialog is shown (optional)
    // Returns true if the default filter is set successfully
    // In this case, set Images filter as the default filter
    //FileBrowser.SetDefaultFilter(".jpg");
    FileBrowser.SetDefaultFilter(".pf");

    // Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
    // Note that when you use this function, .lnk and .tmp extensions will no longer be
    // excluded unless you explicitly add them as parameters to the function
    //FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

    // Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
    // It is sufficient to add a quick link just once
    // Name: Users
    // Path: C:\Users
    // Icon: default (folder icon)
    //FileBrowser.AddQuickLink("Users", "C:\\Users", null);

    // Show a save file dialog 
    // onSuccess event: not registered (which means this dialog is pretty useless)
    // onCancel event: not registered
    // Save file/folder: file, Initial path: "C:\", Title: "Save As", submit button text: "Save"
    // FileBrowser.ShowSaveDialog( null, null, false, "C:\\", "Save As", "Save" );

    // Show a select folder dialog 
    // onSuccess event: print the selected folder's path
    // onCancel event: print "Canceled"
    // Load file/folder: folder, Initial path: default (Documents), Title: "Select Folder", submit button text: "Select"
    //FileBrowser.ShowLoadDialog( (path) => { Debug.Log( "Selected: " + path ); }, 
    //                                () => { Debug.Log( "Canceled" ); }, 
    //                                true, null, "Select Folder", "Select" );

    // Coroutine example
    StartCoroutine(ShowLoadDialogCoroutine());
  }

  private IEnumerator ShowLoadDialogCoroutine()
  {
    // Show a load file dialog and wait for a response from user
    // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
    yield return FileBrowser.WaitForLoadDialog(false, Path.Combine(Application.persistentDataPath, "Projects"), "Load Project", "Load");

    // Dialog is closed
    // Print whether a file is chosen (FileBrowser.Success)
    // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
    Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);

    if (FileBrowser.Success)
    {
      LoadSuccessful(FileBrowser.Result);
    }
  }

  /**
   *  Success Callback when a file is successfully loaded from Load-Dialogue
   */ 
  private void LoadSuccessful(string path)
  {
    Debug.Log("Selected: " + path);
    Load(path);
    ChangeScene.SetScene(6);
  }

  private void Load(string path)
  {
    if (File.Exists(path))
    {
      BinaryFormatter bf = new BinaryFormatter();
      FileStream file = new FileStream(path, FileMode.Open);
      ProjectData data = bf.Deserialize(file) as ProjectData;
      file.Close();
      Debug.Log("Loaded " + path);
      PopulatePlayerPrefs(data);
    }
    else
    {
      Debug.LogError("Save file not found in: " + path);
    }
  }

  /**
   *  Helper function to populate player prefs based on loaded file
   */
  private void PopulatePlayerPrefs( ProjectData data )
  {
    PlayerPrefs.SetString("projectName", data.projectName);
    PlayerPrefs.SetInt("firstTimestep", data.firstTimestep);
    PlayerPrefs.SetInt("hudCoordToggle", data.hudCoordToggle);
    PlayerPrefs.SetInt("hudLegendToggle", data.hudLegendToggle);
    PlayerPrefs.SetInt("hudPlaybackToggle", data.hudPlaybackToggle);
    PlayerPrefs.SetInt("hudRefAxisToggle", data.hudRefAxisToggle);
    PlayerPrefs.SetInt("initScale", data.initScale);
    PlayerPrefs.SetInt("initXCoord", data.initXCoord);
    PlayerPrefs.SetInt("initYCoord", data.initYCoord);
    PlayerPrefs.SetInt("initZCoord", data.initZCoord);
    PlayerPrefs.SetInt("lastTimestep", data.lastTimestep);
    PlayerPrefs.SetString("listOfBinaryFiles", data.listOfBinaryFiles);
    PlayerPrefs.SetString("listOfGameObjectNames", data.listOfGameObjectNames);
    PlayerPrefs.SetString("listOfTimeSteps", data.listOfTimeSteps);
    PlayerPrefs.SetInt("timeStepCount", data.timeStepCount);
    PlayerPrefs.SetString("listOfGameObjectColors", data.listOfGameObjectColors); 
    PlayerPrefs.SetString("waypointInfo", data.waypointInfo);
    PlayerPrefs.SetString("listOfGameObjectTransforms", data.listOfGameObjectTransforms);
  }

}