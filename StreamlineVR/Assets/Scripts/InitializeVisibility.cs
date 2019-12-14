using UnityEngine;
using Valve.VR.Extras;

public class InitializeVisibility : MonoBehaviour
{
	[SerializeField]
  private Transform player;
  [SerializeField]
  private SteamVR_LaserPointer laserPointer;
  [SerializeField]
  private GameObject laserCube;
  [SerializeField]
  private GameObject handCanvas;
  [SerializeField]
  private GameObject playPanel;
  [SerializeField]
  private GameObject transparencyPanel;
  [SerializeField]
  private GameObject colorPanel;
  [SerializeField]
  private GameObject waypointPanel;
  [SerializeField]
  private GameObject translationPanel;
  [SerializeField]
  private GameObject selectPanel;
  [SerializeField]
  private GameObject movableHUD;
  [SerializeField]
  private GameObject cameraContainer;
  [SerializeField]
  private GameObject settingsCanvas;
  [SerializeField]
  private GameObject scalePanel;

  private void Start()
	{
		player.position = new Vector3(PlayerPrefs.GetInt("initXCoord"), PlayerPrefs.GetInt("initYCoord"), PlayerPrefs.GetInt("initZCoord"));
    EnableLaser(false);
    handCanvas.SetActive(false);
    playPanel.SetActive(false);
    transparencyPanel.SetActive(false);
    colorPanel.SetActive(false);
    waypointPanel.SetActive(false);
    translationPanel.SetActive(false);
    selectPanel.SetActive(false);
    movableHUD.SetActive(false);
    cameraContainer.SetActive(false);
    settingsCanvas.SetActive(false);
    scalePanel.SetActive(false);
  }

  private void EnableLaser(bool state)
  {
    laserPointer.enabled = state;
    laserCube.SetActive(state);
  }
}
