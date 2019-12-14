using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PopulateSharingHelpText : MonoBehaviour
{
  [SerializeField]
  private Text helpText;

  // Start is called before the first frame update
  private void Start()
  {
    helpText.text = "Once you have saved a project, all of the files belonging to the project are saved in a single folder on your computer named the same as the project name. Your projects can be found in the following directory:\n" +
                "\n\t\t" + Path.Combine(Application.persistentDataPath, "Projects") + "\n" +
                "\nYou can compress any project folder located here to send to collaborators.";
  }
}
