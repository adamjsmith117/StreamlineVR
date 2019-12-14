using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class TipController : MonoBehaviour
{

  [SerializeField]
  private GameObject rcanvas;
  [SerializeField]
  private GameObject lcanvas;
  [SerializeField]
  private GameObject tcanvas;
  private SteamVR_Input_ActionSet_default actionSet;
  private bool on;

  private void Start()
  {
    actionSet = new SteamVR_Input_ActionSet_default();
    on = true;
    actionSet.move.onStateDown += GoMove;
    actionSet.move.onStateUp += StopMove;
    //hide right pad
    rcanvas.transform.GetChild(1).gameObject.SetActive(false);
    rcanvas.transform.GetChild(4).gameObject.SetActive(false);
    //hide right grip
    rcanvas.transform.GetChild(6).gameObject.SetActive(false);
    rcanvas.transform.GetChild(7).gameObject.SetActive(false);
    //hide left grip
    lcanvas.transform.GetChild(6).gameObject.SetActive(false);
    lcanvas.transform.GetChild(7).gameObject.SetActive(false);
  }

  public void TipsOff()
  {
    rcanvas.SetActive(false);
    lcanvas.SetActive(false);
    tcanvas.SetActive(false);
    actionSet.move.onStateDown -= GoMove;
    actionSet.move.onStateUp -= StopMove;
    on = false;
  }

  public void ToggleTips()
  {
    if (!on)
    {//turn on
      rcanvas.SetActive(true);
      lcanvas.SetActive(true);
      tcanvas.SetActive(true);
      actionSet.move.onStateDown += GoMove;
      actionSet.move.onStateUp += StopMove;
      on = true;
    }
    else if (on)
    {//turn off
      rcanvas.SetActive(false);
      lcanvas.SetActive(false);
      tcanvas.SetActive(false);
      actionSet.move.onStateDown -= GoMove;
      actionSet.move.onStateUp -= StopMove;
      on = false;
    }
  }

  //clears the tooltip text, used when pressing the back button and other places
  public void ClearTooltip()
  {
    Text txt = tcanvas.GetComponentInChildren<Text>();
    if (txt != null)
      txt.text = "";
  }

  //change the button hint to reflect movement mode
  public void GoMove(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
  {
    Text txt = lcanvas.transform.GetChild(0).gameObject.GetComponent<Text>();
    txt.text = "Go Faster";
  }
  //change the button hints back to normal
  public void StopMove(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
  {
    Text txt = lcanvas.transform.GetChild(0).gameObject.GetComponent<Text>();
    txt.text = "Interact";
  }

  //translation tool
  public void TmenuEnter()
  {
    //tip for the pad
    rcanvas.transform.GetChild(1).gameObject.SetActive(true);
    rcanvas.transform.GetChild(4).gameObject.SetActive(true);
    //trigger
    Text txt = rcanvas.transform.GetChild(0).gameObject.GetComponent<Text>();
    txt.text = "Grab";
  }
  //translation tool exit
  public void TmenuExit()
  {
    rcanvas.transform.GetChild(1).gameObject.SetActive(false);
    rcanvas.transform.GetChild(4).gameObject.SetActive(false);
    Text txt = rcanvas.transform.GetChild(0).gameObject.GetComponent<Text>();
    txt.text = "Interact";
  }

  //waypoints tool menu
  public void WmenuEnter()
  {
    //right trigger text
    Text txt = rcanvas.transform.GetChild(0).gameObject.GetComponent<Text>();
    txt.text = "Place";
  }

  //selection tool
  public void SmenuEnter()
  {
    //right trigger text
    Text txt = rcanvas.transform.GetChild(0).gameObject.GetComponent<Text>();
    txt.text = "Select";
  }

  public void ToolMenuExit()
  {
    Text txt = rcanvas.transform.GetChild(0).gameObject.GetComponent<Text>();
    txt.text = "Interact";
  }

  //functions specific to the scale menu hints
  public void ScaleMenuEnter()
  {
    //right trigger text
    Text txt = rcanvas.transform.GetChild(0).gameObject.GetComponent<Text>();
    txt.text = "Select";

    //show right grip
    rcanvas.transform.GetChild(6).gameObject.SetActive(true);
    rcanvas.transform.GetChild(7).gameObject.SetActive(true);
    //show left grip
    lcanvas.transform.GetChild(6).gameObject.SetActive(true);
    lcanvas.transform.GetChild(7).gameObject.SetActive(true);
  }
  public void ScaleMenuExit()
  {
    Text txt = rcanvas.transform.GetChild(0).gameObject.GetComponent<Text>();
    txt.text = "Interact";

    //hide right grip
    rcanvas.transform.GetChild(6).gameObject.SetActive(false);
    rcanvas.transform.GetChild(7).gameObject.SetActive(false);
    //hide left grip
    lcanvas.transform.GetChild(6).gameObject.SetActive(false);
    lcanvas.transform.GetChild(7).gameObject.SetActive(false);

  }
}
