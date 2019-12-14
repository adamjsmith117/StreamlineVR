using UnityEngine;
using Valve.VR;

public class HandCanvasManager : MonoBehaviour
{
  [SerializeField]
  private SteamVR_Input_Sources rightHand;
  [SerializeField]
  private GameObject handCanvas;

  private SteamVR_Input_ActionSet_default set;

  // Start is called before the first frame update
  private void Start()
  {
    set = new SteamVR_Input_ActionSet_default();
  }

  // Update is called once per frame
  private void Update()
  {
    //toggle menu
    if (set.toggle_menu.GetStateDown(rightHand) && set.toggle_menu.GetChanged(rightHand))
    {
      if (handCanvas.activeSelf)
      {
        handCanvas.SetActive(false);
      }
      else
      {
        handCanvas.SetActive(true);
      }
    }
  }
}
