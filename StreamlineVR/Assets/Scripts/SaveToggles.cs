using UnityEngine;
using UnityEngine.UI;

public class SaveToggles : MonoBehaviour
{
  [SerializeField] private Toggle coordToggle;
  [SerializeField] private Toggle refToggle;
  [SerializeField] private Toggle playbackToggle;
  [SerializeField] private Toggle legendToggle;

  private void SaveCoordToggle()
  {
    int i = (coordToggle.isOn) ? 1 : 0;              // make i = 1 if b==true, i = 0
    PlayerPrefs.SetInt("hudCoordToggle", i);
  }

  private void SaveRefAxisToggle()
  {
    int i = (refToggle.isOn) ? 1 : 0;              // make i = 1 if b==true, i = 0
    PlayerPrefs.SetInt("hudRefAxisToggle", i);
  }

  private void SavePlaybackToggle()
  {
    int i = (playbackToggle.isOn) ? 1 : 0;              // make i = 1 if b==true, i = 0
    PlayerPrefs.SetInt("hudPlaybackToggle", i);
  }

  private void SaveLegendToggle()
  {
    int i = (legendToggle.isOn) ? 1 : 0;              // make i = 1 if b==true, i = 0
    PlayerPrefs.SetInt("hudLegendToggle", i);
  }

  public void SaveAll()
  {
    SaveCoordToggle();
    SaveRefAxisToggle();
    SavePlaybackToggle();
    SaveLegendToggle();
  }

}
