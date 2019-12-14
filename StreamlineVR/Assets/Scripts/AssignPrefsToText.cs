using UnityEngine;
using UnityEngine.UI;

public class AssignPrefsToText : MonoBehaviour
{
  [SerializeField] private Text timestep;
  [SerializeField] private Text scale;
  [SerializeField] private Text coord;
  [SerializeField] private Toggle hudCoordToggle;
  [SerializeField] private Toggle hudRefToggle;
  [SerializeField] private Toggle hudPlaybackToggle;
  [SerializeField] private Toggle hudLegendToggle;

  // Start is called before the first frame update
  void Start()
  {
    if (PlayerPrefs.GetInt("initTimestep") == 0)
    {
      timestep.text = PlayerPrefs.GetInt("firstTimestep").ToString();
    }
    else
    {
      timestep.text = PlayerPrefs.GetInt("initTimestep").ToString();
    }

    scale.text = PlayerPrefs.GetInt("initScale").ToString();

    coord.text = "(" + PlayerPrefs.GetInt("initXCoord").ToString() + ", " +
                    PlayerPrefs.GetInt("initYCoord").ToString() + ", " +
                    PlayerPrefs.GetInt("initZCoord").ToString() + ")";

    if (PlayerPrefs.GetInt("hudCoordToggle") == 1)
    {
      hudCoordToggle.isOn = true;
    }
    else
    {
      hudCoordToggle.isOn = false;
    }

    if (PlayerPrefs.GetInt("hudRefAxisToggle") == 1)
    {
      hudRefToggle.isOn = true;
    }
    else
    {
      hudRefToggle.isOn = false;
    }

    if (PlayerPrefs.GetInt("hudPlaybackToggle") == 1)
    {
      hudPlaybackToggle.isOn = true;
    }
    else
    {
      hudPlaybackToggle.isOn = false;
    }

    if (PlayerPrefs.GetInt("hudLegendToggle") == 1)
    {
      hudLegendToggle.isOn = true;
    }
    else
    {
      hudLegendToggle.isOn = false;
    }
  }
}
