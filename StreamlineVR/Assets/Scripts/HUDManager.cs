using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class HUDManager : MonoBehaviour
{
  [SerializeField]
  private Text playerPositionText;
  [SerializeField]
  private Transform player;
  [SerializeField]
  private Transform axis;
  [SerializeField]
  private GameObject movableHUD;
  [SerializeField]
  private GameObject cameraContainer;
  [SerializeField]
  private Camera topViewCamera;
  [SerializeField]
  private Camera frontViewCamera;
  [SerializeField]
  private Camera sideViewCamera;
  [SerializeField]
  private Transform mainViewCamera;
  [SerializeField]
  private Transform playerIcon;
  [SerializeField]
  private SteamVR_Input_Sources leftHand;

  public static bool miniMapCamerasSet;
  private static bool updateValues;
  private SteamVR_Input_ActionSet_default set;
  private void Start()
  {
    miniMapCamerasSet = false;
    updateValues = false;
    set = new SteamVR_Input_ActionSet_default();
  }

  public void EnterMenu()
  {
    if (!miniMapCamerasSet)
    {
      SetMiniMapCameras();
      miniMapCamerasSet = true;
    }
    updateValues = true;
  }

  public void ExitMenu()
  {
    updateValues = false;
  }

  private void Update()
  {
    //toggle HUD
    if (set.toggle_menu.GetStateDown(leftHand) && set.toggle_menu.GetChanged(leftHand))
    {
      if (movableHUD.activeSelf)
      {
        movableHUD.SetActive(false);
        cameraContainer.SetActive(false);
        ExitMenu();
      }
      else
      {
        movableHUD.SetActive(true);
        cameraContainer.SetActive(true);
        EnterMenu();
      }
    }
    if (updateValues)
    {
      Vector3 playerPosition = player.position;
      playerPositionText.text = "X:   " + playerPosition.x.ToString("0.00") + "\n" + "Y:   " + playerPosition.y.ToString("0.00") + "\n" + "Z:   " + playerPosition.z.ToString("0.00");
      axis.rotation = Quaternion.identity;
    }
  }

  private void SetMiniMapCameras()
  {
    float maxMeshWidth = PlayerPrefs.GetFloat("MaxMeshWidth");
    float maxMeshHeight = PlayerPrefs.GetFloat("MaxMeshHeight");
    float maxMeshLength = PlayerPrefs.GetFloat("MaxMeshLength");
    float maxMeshDiagonal = Mathf.Sqrt(Mathf.Pow(maxMeshWidth, 2) + Mathf.Pow(maxMeshLength, 2));
    float yAngle = Mathf.Atan2(maxMeshWidth, maxMeshLength) * Mathf.Rad2Deg;
    float xAngle = Mathf.Atan2(maxMeshHeight, maxMeshDiagonal) * Mathf.Rad2Deg;
    int scale = PlayerPrefs.GetInt("initScale");
    float xTransform = maxMeshWidth * scale;
    float yTransform = maxMeshHeight * scale;
    float zTransform = maxMeshLength * scale;
    float iconScale = Mathf.Min(xTransform, yTransform, zTransform) * 0.1f;
    float maxTransform = Mathf.Max(xTransform, yTransform, zTransform);

    playerIcon.localScale = new Vector3(iconScale, iconScale, iconScale);

    topViewCamera.transform.localPosition = new Vector3(0, maxTransform * (5 / 4), 0);

    frontViewCamera.transform.localPosition = new Vector3(0, 0, -maxTransform * (5 / 4));

    sideViewCamera.transform.localPosition = new Vector3(maxTransform * (5 / 4), 0, 0);

    mainViewCamera.localPosition = new Vector3(xTransform, yTransform, -zTransform);
    mainViewCamera.localEulerAngles = new Vector3(xAngle, -yAngle, 0);
  }
}
