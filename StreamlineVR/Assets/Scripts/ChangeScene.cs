using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
  public void SwitchScene(int idx)
  {
    SetScene(idx);
  }

  public static void SetScene(int sceneNumber)
  {
    SceneManager.LoadScene(sceneNumber);
  }

  public void ExitApp()
  {
    Application.Quit();
  }

  public void ExitVR()
  {
    string projectFolderPath = Path.Combine(Application.persistentDataPath, "Projects", PlayerPrefs.GetString("projectName"));
    string path = Path.Combine(projectFolderPath, PlayerPrefs.GetString("projectName") + ".pf");
    if (!File.Exists(path))
    {
      Debug.Log("Project Not Saved. Removing Project Files And Folder...");
      Directory.Delete(projectFolderPath, true);
    }
    Destroy(GameObject.Find("Player"));
    SetScene(0);
  }
}
