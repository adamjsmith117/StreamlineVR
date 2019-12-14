//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Demonstrates the use of the controller hint system
//
//=============================================================================

using UnityEngine;
using Valve.VR.Extras;

namespace Valve.VR.InteractionSystem.Sample
{
  //------------------------------------------------------------------------- 

  public class Button_push : MonoBehaviour
  {
    [SerializeField]
    private SteamVR_LaserPointer laserPointer;
    [SerializeField]
    private GameObject laserCube;
    [SerializeField]
    private GameObject settingsPanel;
    [SerializeField]
    private GameObject HelpPanel;

    public void HelpUp()
    {
      settingsPanel.SetActive(false);
      HelpPanel.SetActive(true);
    }

    public void HelpDown()
    {
      settingsPanel.SetActive(true);
      HelpPanel.SetActive(false);
    }

    private void EnableLaser(bool state)
    {
      laserPointer.enabled = state;
      laserCube.SetActive(state);
    }
  }
}
