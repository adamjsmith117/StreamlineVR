using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
  private static Vector3 meshCenter;
  private static Material opaque;
  private static Material transparent;
  private static Material opaqueVert;
  private static Material transparentVert; 
  // List of timestep values (i.e. timestep2, timestep5, timestep8, timestep11)
  public static List<string> timestepNames;

  // Dictionary of queues containing tuples of pending changes
  // Dictionary<timestepName, Queue<Tuple<submeshName, pendingChange>>>
  public static Dictionary<string, List<System.Tuple<string, string>>> queueDictionary;

  /**
   * Runs once on start. Initializes Dictionary of timestep-change queues.
   */
  private void Start()
  {
    meshCenter = Vector3.zero;
    opaque = Resources.Load("opaque") as Material;
    transparent = Resources.Load("transparent") as Material;
    opaqueVert = Resources.Load("opaqueVert") as Material;
    transparentVert = Resources.Load("transparentVert") as Material;
    timestepNames = new List<string>();
    // Creates the queue dictionary and populates it with the queues
    queueDictionary = new Dictionary<string, List<System.Tuple<string, string>>>();

    string steps = PlayerPrefs.GetString("listOfTimeSteps");
    steps = steps.Trim();
    string[] timesteps = steps.Split(' ');

    for(int i = 0; i < timesteps.Length; i++)
    {
      string timestepName = "timestep" + timesteps[i];
      timestepNames.Add(timestepName);

      List<System.Tuple<string, string>> queue = new List<System.Tuple<string, string>>();
      queueDictionary.Add(timestepName, queue);
    }
  }

  /**
   * Method that applies all pending changes in the provided timestep's queue
   * Parameters:
   *     - timestepName: the name of the timestep we want to apply all pending changes to (i.e. "timestep3")
   */
  public static void ExhaustQueue( string timestepName )
  {
    // Get the queue for the specified timestep
    if (queueDictionary[timestepName].Count > 0)
    {
      List<System.Tuple<string, string>> q = queueDictionary[timestepName];
      int count = q.Count;
      List<int> indicesToRemove = new List<int>();
      // dequeue and apply pending changes for each tuple in queue
      for (int i = 0; i < count; i++)
      {
        // Get the current change to apply that we are iterating over
        System.Tuple<string, string> change = q[i];
        indicesToRemove.Add(i);
        string[] changeValues = change.Item2.Split(' ');
        string subMeshName = PlayerPrefs.GetString("projectName") + "_" + timestepName + "_" + change.Item1;

        if (SelectionManager.gameObjects.ContainsKey(subMeshName))
        {
          GameObject gObj = SelectionManager.gameObjects[subMeshName];

          if (changeValues[0] == "color")
          {
            ColorUtility.TryParseHtmlString("#" + changeValues[1], out Color colorArg);
            float originalAlpha = gObj.GetComponent<MeshRenderer>().material.color.a;
            if (originalAlpha == 1f)
            {
              Material oldMaterial = gObj.GetComponent<MeshRenderer>().material;
              gObj.GetComponent<MeshRenderer>().material = new Material(opaque);
              Destroy(oldMaterial);
            }
            else
            {
              Material oldMaterial = gObj.GetComponent<MeshRenderer>().material;
              gObj.GetComponent<MeshRenderer>().material = new Material(transparent);
              Destroy(oldMaterial);
            }
            colorArg.a = originalAlpha;
            gObj.GetComponent<MeshRenderer>().material.color = colorArg;
          }
          else if (changeValues[0] == "transparency")
          {
            float alpha = float.Parse(changeValues[1]);
            Material initialMaterial = gObj.GetComponent<MeshRenderer>().material;
            Material newMaterial;
            bool usingVertMaterial = initialMaterial.name.Contains("Vert");

            if (alpha == 1f)
            {
              //opaque or opaqueVert
              if (usingVertMaterial)
              {
                newMaterial = new Material(opaqueVert);
              }
              else
              {
                newMaterial = new Material(opaque);
              }
            }
            else
            {
              //transparent or transparentVert
              if (usingVertMaterial)
              {
                newMaterial = new Material(transparentVert);
              }
              else
              {
                newMaterial = new Material(transparent);
              }
            }

            Color[] meshColors = gObj.GetComponent<MeshFilter>().mesh.colors;
            for (int k = 0; k < meshColors.Length; k++)
            {
              Color newColor = meshColors[k];
              newColor.a = alpha;
              meshColors[k] = newColor;
            }
            gObj.GetComponent<MeshFilter>().mesh.colors = meshColors;
            Color originalColor = initialMaterial.color;
            newMaterial.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            gObj.GetComponent<MeshRenderer>().material = newMaterial;
            Destroy(initialMaterial);
          }
          else if (changeValues[0] == "transform")
          {
            if (changeValues[1] == "0")
            {
              char[] charsToTrim = { ' ', ',', '(', ')' };

              string trans1 = changeValues[1].Trim(charsToTrim);
              string trans2 = changeValues[2].Trim(charsToTrim);
              string trans3 = changeValues[3].Trim(charsToTrim);

              string rot1 = changeValues[4].Trim(charsToTrim);
              string rot2 = changeValues[5].Trim(charsToTrim);
              string rot3 = changeValues[6].Trim(charsToTrim);
              string rot4 = changeValues[7].Trim(charsToTrim);

              Vector3 pos = new Vector3(float.Parse(trans1), float.Parse(trans2), float.Parse(trans3));
              gObj.transform.localPosition = pos;

              Quaternion quat = new Quaternion
              {
                x = float.Parse(rot1),
                y = float.Parse(rot2),
                z = float.Parse(rot3),
                w = float.Parse(rot4)
              };
              gObj.transform.localRotation = quat;
            }
            else if (changeValues[1] == "1")
            {
              gObj.transform.position = -1 * meshCenter * gObj.transform.localScale.z;
              gObj.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
          }
          else if (changeValues[0] == "material")
          {
            if (string.Compare(changeValues[1], "0") == 0)
            {
              Color originalColor = gObj.GetComponent<Renderer>().material.color;
              if (originalColor.a == 1f)
              {
                Material oldMaterial = gObj.GetComponent<MeshRenderer>().material;
                gObj.GetComponent<Renderer>().material = new Material(opaqueVert);
                Destroy(oldMaterial);
              }
              else
              {
                Material oldMaterial = gObj.GetComponent<MeshRenderer>().material;
                gObj.GetComponent<Renderer>().material = new Material(transparentVert);
                Destroy(oldMaterial);
              }
              gObj.GetComponent<Renderer>().material.color = originalColor;
            }
            else if (string.Compare(changeValues[1], "1") == 0)
            {
              Color originalColor = gObj.GetComponent<Renderer>().material.color;
              if (originalColor.a == 1f)
              {
                Material oldMaterial = gObj.GetComponent<MeshRenderer>().material;
                gObj.GetComponent<Renderer>().material = new Material(opaque);
                Destroy(oldMaterial);
              }
              else
              {
                Material oldMaterial = gObj.GetComponent<MeshRenderer>().material;
                gObj.GetComponent<Renderer>().material = new Material(transparent);
                Destroy(oldMaterial);
              }
              gObj.GetComponent<Renderer>().material.color = originalColor;
            }
          }
          else if (changeValues[0] == "scale")
          {
            if (changeValues[1] == "0")
            {
              float scale = float.Parse(changeValues[2]);
              // Get the initial geometric center of the mesh in world space
              Vector3 initialGeometricCenter = gObj.transform.TransformPoint(meshCenter);
              gObj.transform.localScale = new Vector3(scale, scale, scale);
              // Get the post geometric center of the mesh in world space
              Vector3 postGeometricCenter = gObj.transform.TransformPoint(meshCenter);
              // Calculate the distance the object "drifts" and correct it by subtracting that compensation vector
              Vector3 compensationVector = postGeometricCenter - initialGeometricCenter;
              gObj.transform.position = gObj.transform.position - compensationVector;
            }
            else if (changeValues[1] == "1")
            {
              float scaleFactor = float.Parse(changeValues[2]);
              // Get the initial geometric center of the mesh in world space
              Vector3 initialGeometricCenter = gObj.transform.TransformPoint(meshCenter);
              gObj.transform.localScale += gObj.transform.localScale * scaleFactor;
              // Get the post geometric center of the mesh in world space
              Vector3 postGeometricCenter = gObj.transform.TransformPoint(meshCenter);
              // Calculate the distance the object "drifts" and correct it by subtracting that compensation vector
              Vector3 compensationVector = postGeometricCenter - initialGeometricCenter;
              gObj.transform.position = gObj.transform.position - compensationVector;
            }
            else if (changeValues[1] == "2")
            {
              float scaleFactor = float.Parse(changeValues[2]);
              // Get the initial geometric center of the mesh in world space
              Vector3 initialGeometricCenter = gObj.transform.TransformPoint(meshCenter);
              gObj.transform.localScale -= gObj.transform.localScale * scaleFactor;
              // Get the post geometric center of the mesh in world space
              Vector3 postGeometricCenter = gObj.transform.TransformPoint(meshCenter);
              // Calculate the distance the object "drifts" and correct it by subtracting that compensation vector
              Vector3 compensationVector = postGeometricCenter - initialGeometricCenter;
              gObj.transform.position = gObj.transform.position - compensationVector;
            }
          }
        }
      }
      indicesToRemove.Sort();
      indicesToRemove.Reverse();
      foreach(int index in indicesToRemove)
      {
        queueDictionary[timestepName].RemoveAt(index);
      }
    }
    else
    {
      return;
    }
  }

  public static void SetMeshCenter(Vector3 center)
  {
    meshCenter = new Vector3(center.x, center.y, center.z);
  }

  /**
   * Method that pushes the specified change to all timesteps other than the one specified
   * Parameters:
   *    - submeshName: the name of the specific submesh the change should be applied to
   *    - pendingChange: string already formatted as necessary to be added to all other timestep's queues. 
   */
  public static void PushChange( string submeshName, string pendingChange, string fromTimestep )
  {
    // Remove redundant pending changes on the same submesh w/ the same type of change
    foreach(string timestep in timestepNames)
    {
      if(timestep != fromTimestep)
      {
        List<System.Tuple<string, string>> thisQueue = queueDictionary[timestep];
        int count = thisQueue.Count;
        for (int i = count - 1; i >= 0; i--)
        {
          System.Tuple<string, string> tuple = thisQueue[i];
          if (tuple.Item1 == submeshName)
          {
            string changeType = tuple.Item2.Split(' ')[0];
            string newChangeType = pendingChange.Split(' ')[0];
            if (changeType == newChangeType)
            {
              if (changeType == "material")
              {
                if (tuple.Item2.Split(' ')[1] == pendingChange.Split(' ')[1])
                {
                  thisQueue.RemoveAt(i);
                }
              }
              else if (changeType == "scale")
              {
                if (tuple.Item2.Split(' ')[1] == pendingChange.Split(' ')[1])
                {
                  thisQueue.RemoveAt(i);
                }
              }
              else if (changeType == "transform")
              {
                if (tuple.Item2.Split(' ')[1] == pendingChange.Split(' ')[1])
                {
                  thisQueue.RemoveAt(i);
                }
              }
              else
              {
                thisQueue.RemoveAt(i);
              }
            }
          }
        }

        System.Tuple<string, string> tup = new System.Tuple<string, string>(submeshName, pendingChange);
        queueDictionary[timestep].Add(tup);
      }
    }
  }
}
