using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class VRActive : MonoBehaviour
{
  private void Start()
  {
    //DisableVR();
    //string[] devices = XRSettings.supportedDevices;
    //foreach (string device in devices)
    //{
    //  Debug.Log(device);
    //}
  }

  private IEnumerator LoadDevice(string newDevice, bool enable)
  {
    XRSettings.LoadDeviceByName(newDevice);
    yield return null;
    XRSettings.enabled = enable;
  }

  private void EnableVR()
  {
    StartCoroutine(LoadDevice("OpenVR", true));
  }

  private void DisableVR()
  {
    StartCoroutine(LoadDevice("", false));
  }
}
