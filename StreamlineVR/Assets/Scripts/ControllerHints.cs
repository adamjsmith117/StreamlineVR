using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

//-------------------------------------------------------------------------
public class ControllerHints : MonoBehaviour
{
  public Hand righthand;
  public Hand lefthand;
  private Coroutine buttonHintCoroutine;
  private Coroutine textHintCoroutine;


  public void Showallthings()
  {
    ISteamVR_Action_In action1 = SteamVR_Input.actionsIn[9];
    ISteamVR_Action_In action2 = SteamVR_Input.actionsIn[10];
    ISteamVR_Action_In action3 = SteamVR_Input.actionsIn[0];
    ISteamVR_Action_In action4 = SteamVR_Input.actionsIn[11];
    ISteamVR_Action_In action5 = SteamVR_Input.actionsIn[14];
    action1.GetActive(lefthand.handType);
    ControllerButtonHints.ShowTextHint(lefthand, action3, "Test interact ui", false);
    //ControllerButtonHints.HideButtonHint(lefthand, action3);
    ControllerButtonHints.ShowTextHint(lefthand, action4, "Test toggle menu");
    //ControllerButtonHints.HideButtonHint(lefthand, action4);
    ControllerButtonHints.ShowTextHint(lefthand, action2, "Test move", false);
    //ControllerButtonHints.HideButtonHint(lefthand, action2);
    //ControllerButtonHints.ShowTextHint(lefthand, action5, "Test tgrab", false);
    //ControllerButtonHints.HideAllButtonHints(hand);
  }
  
}

