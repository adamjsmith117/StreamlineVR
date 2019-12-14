using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;

public class SettingsManager : MonoBehaviour
{
  [SerializeField]
  private SteamVR_LaserPointer laserPointer;
  [SerializeField]
  private GameObject laserCube;
  [SerializeField]
  private GameObject settingsPanel;
  [SerializeField]
  private GameObject menuPanel;
  [SerializeField]
  private GameObject head;

  [SerializeField]
  private GameObject wayCont;
  [SerializeField]
  private GameObject timeCont;

  private HashSet<string> bNames = new HashSet<string>();
  private Color orig;

  private void Start()
  {
    bNames.Add("Exit");
    bNames.Add("Save");
    bNames.Add("Help");
    bNames.Add("AdvMov");
    bNames.Add("Interaction");
    bNames.Add("Tips");
    bNames.Add("Tools");
    bNames.Add("Settings");
    bNames.Add("Playback");
    bNames.Add("Transparency");
    bNames.Add("Color");
    bNames.Add("Waypoints");
    bNames.Add("HUD");
    bNames.Add("Movement");
    bNames.Add("Selection");
    bNames.Add("Return");
    bNames.Add("Resume");
  }

  public void EnterMenu()
  {
    menuPanel.SetActive(false);
    settingsPanel.SetActive(true);
    EnableLaser(true);

    wayCont.SetActive(false);
    timeCont.SetActive(false);

    //position the menu in front of the head 
    Quaternion hrotation = head.transform.rotation;
    Vector3 sdirection = new Vector3(0, -1.5f, 4);
    settingsPanel.transform.position = head.transform.position;
    settingsPanel.transform.Translate(hrotation * sdirection, Space.World);
    settingsPanel.transform.rotation = Quaternion.LookRotation(settingsPanel.transform.position - head.transform.position, Vector3.up);
  }

  public void ExitMenu()
  {
    EnableLaser(false);
    settingsPanel.SetActive(false);
    menuPanel.SetActive(true);
    wayCont.SetActive(true);
    timeCont.SetActive(true);
  }

  private void EnableLaser(bool state)
  {
    laserPointer.enabled = state;
    laserCube.SetActive(state);
    if (state)
    {
      laserPointer.PointerClick += PointerClick;
      laserPointer.PointerIn += PointerInside;
      laserPointer.PointerOut += PointerOutside;
    }
    else
    {
      laserPointer.PointerClick -= PointerClick;
      laserPointer.PointerIn -= PointerInside;
      laserPointer.PointerOut -= PointerOutside;
    }
  }

  private void PointerClick(object sender, PointerEventArgs e)
  {
    e.target.GetComponent<Button>().onClick.Invoke();
    Color color = new Color(0.1568626f, 0.5568628f, 0.9490196f, orig.a);
    e.target.gameObject.GetComponent<Renderer>().material.SetColor("_Color", color);
  }

  private void PointerInside(object sender, PointerEventArgs e)
  {
    //Color orig = e.target.gameObject.GetComponent<Renderer>().material.color;
    orig = e.target.gameObject.GetComponent<Renderer>().material.color;
    Color color;
    if(e.target.name == "Exit")
    {
      //red
      color = new Color(1f, 0f, 0f);
      color.a = orig.a;
      e.target.gameObject.GetComponent<Renderer>().material.SetColor("_Color", color);
    }
    else if (bNames.Contains(e.target.name))
    {
      //yellow
      color = new Color(1f,.631f,.098f);
      color.a = orig.a;
      e.target.gameObject.GetComponent<Renderer>().material.SetColor("_Color", color);
    }
  }

  private void PointerOutside(object sender, PointerEventArgs e)
  {
    if (bNames.Contains(e.target.name))
    {
      //Color orig = e.target.gameObject.GetComponent<Renderer>().material.color;
      //Color color = new Color(0.1568627f, 0.5568628f, 0.9490196f);
      //color.a = orig.a;
      //e.target.gameObject.GetComponent<Renderer>().material.SetColor("_Color", color);
      e.target.gameObject.GetComponent<Renderer>().material.SetColor("_Color", orig);
    }
  }
}
