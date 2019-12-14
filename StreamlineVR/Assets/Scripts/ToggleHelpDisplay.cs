using UnityEngine;
using UnityEngine.UI;

public class ToggleHelpDisplay : MonoBehaviour
{
  [SerializeField]
  private GameObject WhatIs;
  [SerializeField]
  private GameObject NewProject;
  [SerializeField]
  private GameObject LoadProject;
  [SerializeField]
  private GameObject SaveProject;
  [SerializeField]
  private GameObject Exiting;
  [SerializeField]
  private GameObject Sharing;
  [SerializeField]
  private GameObject TopLayer;
  [SerializeField]
  private Text WhatIsText;
  [SerializeField]
  private Text NewProjectText;
  [SerializeField]
  private Text SaveProjectText;
  [SerializeField]
  private Text LoadProjectText;
  [SerializeField]
  private Text ExitingText;
  [SerializeField]
  private Text SharingText;

  public void SetWhatIsActive()
  {
    WhatIs.SetActive(true);
    NewProject.SetActive(false);
    LoadProject.SetActive(false);
    SaveProject.SetActive(false);
    Exiting.SetActive(false);
    Sharing.SetActive(false);
    TopLayer.SetActive(false);
  }

  public void SetNewProjectActive()
  {
    WhatIs.SetActive(false);
    NewProject.SetActive(true);
    LoadProject.SetActive(false);
    SaveProject.SetActive(false);
    Exiting.SetActive(false);
    Sharing.SetActive(false);
    TopLayer.SetActive(false);
  }

  public void SetLoadProjectActive()
  {
    WhatIs.SetActive(false);
    NewProject.SetActive(false);
    LoadProject.SetActive(true);
    SaveProject.SetActive(false);
    Exiting.SetActive(false);
    Sharing.SetActive(false);
    TopLayer.SetActive(false);
  }

  public void SetSaveProjectActive()
  {
    WhatIs.SetActive(false);
    NewProject.SetActive(false);
    LoadProject.SetActive(false);
    SaveProject.SetActive(true);
    Exiting.SetActive(false);
    Sharing.SetActive(false);
    TopLayer.SetActive(false);
  }

  public void SetExitingActive()
  {
    WhatIs.SetActive(false);
    NewProject.SetActive(false);
    LoadProject.SetActive(false);
    SaveProject.SetActive(false);
    Exiting.SetActive(true);
    Sharing.SetActive(false);
    TopLayer.SetActive(false);
  }

  public void SetSharingActive()
  {
    WhatIs.SetActive(false);
    NewProject.SetActive(false);
    LoadProject.SetActive(false);
    SaveProject.SetActive(false);
    Exiting.SetActive(false);
    Sharing.SetActive(true);
    TopLayer.SetActive(false);
  }

  public void SetTopLayerActive()
  {
    WhatIs.SetActive(false);
    NewProject.SetActive(false);
    LoadProject.SetActive(false);
    SaveProject.SetActive(false);
    Exiting.SetActive(false);
    Sharing.SetActive(false);
    TopLayer.SetActive(true);
    SetTextBlack();
  }

  private void SetTextBlack()
  {
    WhatIsText.color = Color.black;
    NewProjectText.color = Color.black;
    SaveProjectText.color = Color.black;
    LoadProjectText.color = Color.black;
    ExitingText.color = Color.black;
    SharingText.color = Color.black;
  }
}
