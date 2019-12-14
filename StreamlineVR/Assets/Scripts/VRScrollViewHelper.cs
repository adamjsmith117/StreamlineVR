using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class VRScrollViewHelper : MonoBehaviour
{
  [SerializeField]
  private LinearMapping vert;
  [SerializeField]
  private LinearMapping hori;
  [SerializeField]
  private Scrollbar vv;
  [SerializeField]
  private Scrollbar hh;

  private float prevv = 0;
  // Update is called once per frame
  private void Update()
  {
    if (Mathf.Abs(prevv - vert.value) > 0.004)
    {
      vv.value = 1 - vert.value;
      hh.value = hori.value;
      prevv = vert.value;
    }
    else if(vert.value == 1 || vert.value == 0 )
    {
      vv.value = 1 - vert.value;
      hh.value = hori.value;
      prevv = vert.value;
    }
    else
    {
      hh.value = hori.value;
    }
  }
}
