using UnityEngine;
using UnityEngine.UI;

public class PopulateFields : MonoBehaviour
{
  [SerializeField]
  private InputField timestepData;
  [SerializeField]
  private InputField scaleData;
  [SerializeField]
  private InputField xData;
  [SerializeField]
  private InputField yData;
  [SerializeField]
  private InputField zData;
  [SerializeField]
  private Toggle hudCoordToggle;
  [SerializeField]
  private Toggle hudRefToggle;
  [SerializeField]
  private Toggle hudPlaybackToggle;
  [SerializeField]
  private Toggle hudLegendToggle;
  [SerializeField]
  private InputField projectName;

  void Start()
  {
    timestepData.text = PlayerPrefs.GetInt("initTimestep").ToString();
    scaleData.text = PlayerPrefs.GetInt("initScale").ToString();
    xData.text = PlayerPrefs.GetInt("initXCoord").ToString();
    yData.text = PlayerPrefs.GetInt("initYCoord").ToString();
    zData.text = PlayerPrefs.GetInt("initZCoord").ToString();
    projectName.text = PlayerPrefs.GetString("projectName");

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
