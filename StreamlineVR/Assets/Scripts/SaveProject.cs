using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Valve.VR;

public class SaveProject : MonoBehaviour
{
  [SerializeField]
  private MeshRenderer saveButton;
  [SerializeField]
  private SteamVR_Input_Sources rightHand;
  [SerializeField]
  private SteamVR_Input_Sources leftHand;

  private SteamVR_Action_Vibration haptics;

  private void Start()
  {
    haptics = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic");
  }

  /**
   *  Saves the current session's player preferences to a .pf file.
   */
  public void Save()
  {
    ExhaustAllQueues();
    SaveGameObjectInfo();

    // Create ProjectData object with all of the current PlayerPrefs
    ProjectData data = new ProjectData();

    string path = Path.Combine(Application.persistentDataPath, "Projects", PlayerPrefs.GetString("projectName"), PlayerPrefs.GetString("projectName") + ".pf");
    if (File.Exists(path))
    {
      Debug.Log("Overwriting " + path);
    }

    // Prepare the file we will write to
    BinaryFormatter bf = new BinaryFormatter();
    FileStream file = new FileStream(path, FileMode.Create);

    // Save data
    bf.Serialize(file, data);
    file.Close();

    // Report where the file was saved
    Debug.Log("Saved to " + path);

    // Change save button color
    saveButton.material.color = Color.green;

    // Do haptics so user knows it saved
    haptics.Execute(0, 1f, 0.005f, 1f, rightHand);
    haptics.Execute(0, 1f, 0.005f, 1f, leftHand);
    // Not working as expected
    //haptics.Execute(5f, 1f, 0.005f, 1f, rightHand);
    //haptics.Execute(5f, 1f, 0.005f, 1f, leftHand);
  }

  /**
   *  Helper function to exhaust all pending changes before saving project
   */ 
  private void ExhaustAllQueues()
  {
    string timestepNames = PlayerPrefs.GetString("listOfTimeSteps");
    string[] names = timestepNames.Trim(' ').Split(' ');
    foreach(string timestepNumber in names)
    {
      QueueManager.ExhaustQueue("timestep" + timestepNumber);
    }
  }

  private void SaveGameObjectInfo()
  {
    string colors = "";
    string transforms = "";
    foreach(GameObject go in SelectionManager.gameObjects.Values)
    {
      Color c = go.GetComponent<MeshRenderer>().material.color;
      string color = ColorUtility.ToHtmlStringRGBA(c);
      string val1 = go.name + " " + color + ", ";
      colors += val1;

      Transform trans = go.GetComponent<Transform>();

      string contents = trans.localScale + " " + trans.localPosition + " " + trans.localRotation;
      string val2 = go.name + " " + contents + ";";
      transforms += val2;
    }
    PlayerPrefs.SetString("listOfGameObjectColors", colors);
    PlayerPrefs.SetString("listOfGameObjectTransforms", transforms);
  }
}