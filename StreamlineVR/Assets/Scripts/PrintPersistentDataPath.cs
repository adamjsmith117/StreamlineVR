using UnityEngine;
using System.IO;

public class PrintPersistentDataPath : MonoBehaviour
{
  // Use this for initialization
  private void Start()
  {
    DirectoryInfo test = new DirectoryInfo(Application.persistentDataPath);
    Debug.Log(test.ToString());
  }
}
