using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class Tooltip : MonoBehaviour
{

  [SerializeField] private string message;

  private void OnHandHoverBegin(Hand hand)
  {
    Text txt = hand.GetComponentInChildren<Text>();
    if(txt != null)
      txt.text = message;
  }

  private void OnHandHoverEnd(Hand hand)
  {
    Text txt = hand.GetComponentInChildren<Text>();
    if(txt != null)
      txt.text = "";
  }
}
