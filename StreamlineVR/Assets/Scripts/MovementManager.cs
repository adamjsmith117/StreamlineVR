using UnityEngine;
using Valve.VR;

public class MovementManager : MonoBehaviour
{
  [SerializeField]
  private SteamVR_Action_Vector2 touchpadact;
  [SerializeField]
  private SteamVR_Action_Pose cpos;
  [SerializeField]
  private SteamVR_Action_Single speedact;
  [SerializeField]
  private SteamVR_Input_Sources leftHand;
  [SerializeField]
  private SteamVR_Input_Sources rightHand;
  [SerializeField]
  private int moveMult;

  private bool advmove;
  private SteamVR_Input_ActionSet_default set;
  private float speed;

  private void Start()
  {
    set = new SteamVR_Input_ActionSet_default();
    speed = 0.025f;
  }

  // Update is called once per frame
  private void Update()
  {
    //if move is pressed down go in direction of left controller
    if (set.move.GetState(leftHand))
    {
      //some tests to get used to how it all works
      /*
      if (set.move.GetStateDown(hand1))
      {
        print("touchpad pressed down");
      }
      if (set.move.GetStateUp(hand1))
      {
        print("touchpad pressed up");
      }
      if (set.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any))
      {
        print("trigger pressed down");
      }
      */
      Vector2 padval = set.touchpadact.GetAxis(leftHand);
      /*
      if (padval != Vector2.zero)
      {
        print("location" + padval);
      }
      */
      //get the location and rotation of the left controller
      Quaternion rot = cpos.GetLocalRotation(leftHand);
      Vector3 cont = cpos.GetLocalPosition(leftHand);
      //print("contr" + rot);

      //the value 0 to 1 from the trigger action
      float mm = set.speed.GetAxis(leftHand);

      //setting the speed multiplier with max of 3x original speed
      float multiplier = 1 + (mm * moveMult);
      float rate = speed * multiplier;

      //get the x and y from the touchpad and scale them by rate, may need to scale padval 0 to 1 values
      //x L R movement, y front back movement
      float horizontalPad = padval.x * rate;
      float forewardPad = padval.y * rate;

      //turn the advanced movement off
      if (!advmove)
      {
        horizontalPad = 0;
      }

      //do the translation
      //(x,y,x) x horizontal L R, y vertical, z foreward back
      Vector3 init = new Vector3(horizontalPad, 0f, forewardPad);
      //print(forewardPad);
      transform.Translate(rot * init);
      //print("moving to " + (rot * init));
    }
  }

  public void ToggleADV()
  {
    if (advmove)
    {
      advmove = false;
    }
    else
    {
      advmove = true;
    }
  }
}
