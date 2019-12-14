using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;
using Valve.VR;
using System.Collections.Generic;

public class ScaleManager : MonoBehaviour
{
  [SerializeField]
  private SteamVR_LaserPointer laserPointer;
  [SerializeField]
  private GameObject laserCube;
  [SerializeField]
  private GameObject scalePanel;
  [SerializeField]
  private GameObject menuPanel;
  [SerializeField]
  private Text hudCurrentSelectedGameObjectName;
  [SerializeField]
  private GameObject hand1;
  [SerializeField]
  private GameObject hand2;
  [SerializeField]
  private Text currentScaleText;
  [SerializeField]
  private Text scaleFactorText;
  [SerializeField]
  private Dropdown toolEffectDropdown;

  private int scaleFactor;
  private SteamVR_Input_ActionSet_default actionSet;
  private bool doUpdateLoop;
  private float initial_distance;
  private bool useLaserPointer;
  private bool allTimeSteps;
  private int toolEffect;
  private List<GameObject> currentSelectedGameObjects;
  private List<GameObject> currentSelectedGameObjectPairs;
  private List<GameObject> gameObjectsToEffect;
  private List<Vector3> initialScales;
  private static float initialScale;
  private static Vector3 meshCenter;

  private void Start()
  {
    scaleFactor = 10;
    useLaserPointer = true;
    allTimeSteps = true;
    toolEffect = 1;
    currentSelectedGameObjects = new List<GameObject>();
    currentSelectedGameObjectPairs = new List<GameObject>();
    gameObjectsToEffect = new List<GameObject>();
    initialScales = new List<Vector3>();
    actionSet = new SteamVR_Input_ActionSet_default();
    doUpdateLoop = false;
    initial_distance = 0;
    initialScale = 0;
    meshCenter = Vector3.zero;
  }

  public void EnterMenu()
  {
    menuPanel.SetActive(false);
    scalePanel.SetActive(true);
    if (useLaserPointer)
    {
      EnableLaser(true);
      EnableScale(true);
    }
    currentSelectedGameObjects = SelectionManager.currentSelectedGameObjects;
    if (currentSelectedGameObjects.Count > 0)
    {
      foreach (GameObject currentSelectedGameObject in currentSelectedGameObjects)
      {
        //activate selection outline on current selected game object
        Outline outlineComponent = currentSelectedGameObject.GetComponent<Outline>();
        if (outlineComponent)
        {
          outlineComponent.enabled = true;
        }
        //get current selected game object pair
        if (currentSelectedGameObject.name.Contains("Exterior"))
        {
          currentSelectedGameObjectPairs.Add(SelectionManager.gameObjects[currentSelectedGameObject.name.Replace("Exterior", "Interior")]);
        }
        else
        {
          currentSelectedGameObjectPairs.Add(SelectionManager.gameObjects[currentSelectedGameObject.name.Replace("Interior", "Exterior")]);
        }
      }
      //update UI and tool info
      currentScaleText.text = currentSelectedGameObjects[0].transform.localScale.z.ToString("0.00");
      UpdateToolEffect();
    }
    else
    {
      currentScaleText.text = "---";
    }
  }

  public void ExitMenu()
  {
    if (currentSelectedGameObjects.Count > 0)
    {
      foreach (GameObject currentSelectedGameObject in currentSelectedGameObjects)
      {
        //deactivate selection outline on current selected game object
        Outline outlineComponent = currentSelectedGameObject.GetComponent<Outline>();
        if (outlineComponent)
        {
          outlineComponent.enabled = false;
        }
      }
    }
    SelectionManager.currentSelectedGameObjects = currentSelectedGameObjects;
    currentSelectedGameObjectPairs.Clear();
    if (useLaserPointer)
    {
      EnableLaser(false);
      EnableScale(false);
    }
    scalePanel.SetActive(false);
    menuPanel.SetActive(true);
  }

  public static void SetMeshCenter(Vector3 center)
  {
    meshCenter = new Vector3(center.x, center.y, center.z);
  }

  public static void SetInitialScale(float scale)
  {
    initialScale = scale;
  }

  private void EnableScale(bool state)
  {
    if (state)
    {
      actionSet.GoScale.onStateUp += StopLoop;
      actionSet.GoScale.onStateDown += StartLoop;
    }
    else
    {
      actionSet.GoScale.onStateUp -= StopLoop;
      actionSet.GoScale.onStateDown -= StartLoop;
    }
  }

  private void EnableLaser(bool state)
  {
    laserPointer.enabled = state;
    laserCube.SetActive(state);
    if (state)
    {
      laserPointer.PointerClick += UpdateSelectedGameObjectsUsingLaser;
    }
    else
    {
      laserPointer.PointerClick -= UpdateSelectedGameObjectsUsingLaser;
    }
  }

  public void ResetScale()
  {
    if (currentSelectedGameObjects.Count > 0)
    {
      foreach (GameObject gameObject in gameObjectsToEffect)
      {
        ResetScaleGameObject(gameObject);
        if (allTimeSteps)
        {
          string fullMeshName = gameObject.name;
          string timestepName = fullMeshName.Split('_')[1];
          QueueManager.PushChange(fullMeshName.Substring(fullMeshName.LastIndexOf('_') + 1), "scale 0 " + initialScale, timestepName);
        }
      }
      currentScaleText.text = currentSelectedGameObjects[0].transform.localScale.z.ToString("0.00");
    }
  }

  private void ResetScaleGameObject(GameObject gameObject)
  {
    // Get the initial geometric center of the mesh in world space
    Vector3 initialGeometricCenter = gameObject.transform.TransformPoint(meshCenter);
    gameObject.transform.localScale = new Vector3(initialScale, initialScale, initialScale);
    // Get the post geometric center of the mesh in world space
    Vector3 postGeometricCenter = gameObject.transform.TransformPoint(meshCenter);
    // Calculate the distance the object "drifts" and correct it by subtracting that compensation vector
    Vector3 compensationVector = postGeometricCenter - initialGeometricCenter;
    gameObject.transform.position = gameObject.transform.position - compensationVector;
  }

  public void IncreaseScaleFactor()
  {
    if (scaleFactor < 200)
    {
      scaleFactor += 10;
      scaleFactorText.text = scaleFactor + "%";
    }
  }

  public void DecreaseScaleFactor()
  {
    if (scaleFactor > 10)
    {
      scaleFactor -= 10;
      scaleFactorText.text = scaleFactor + "%";
    }
  }

  public void UpScale()
  {
    if (currentSelectedGameObjects.Count > 0)
    {
      foreach (GameObject gameObject in gameObjectsToEffect)
      {
        UpScaleGameObject(gameObject);
        if (allTimeSteps)
        {
          string fullMeshName = gameObject.name;
          string timestepName = fullMeshName.Split('_')[1];
          QueueManager.PushChange(fullMeshName.Substring(fullMeshName.LastIndexOf('_') + 1), "scale 1 " + (scaleFactor / 100f), timestepName);
        }
      }
      currentScaleText.text = currentSelectedGameObjects[0].transform.localScale.z.ToString("0.00");
    }
  }

  private void UpScaleGameObject(GameObject gameObject)
  {
    // Get the initial geometric center of the mesh in world space
    Vector3 initialGeometricCenter = gameObject.transform.TransformPoint(meshCenter);
    gameObject.transform.localScale += gameObject.transform.localScale * (scaleFactor / 100f);
    // Get the post geometric center of the mesh in world space
    Vector3 postGeometricCenter = gameObject.transform.TransformPoint(meshCenter);
    // Calculate the distance the object "drifts" and correct it by subtracting that compensation vector
    Vector3 compensationVector = postGeometricCenter - initialGeometricCenter;
    gameObject.transform.position = gameObject.transform.position - compensationVector;
  }

  public void DownScale()
  {
    if (currentSelectedGameObjects.Count > 0)
    {
      foreach (GameObject gameObject in gameObjectsToEffect)
      {
        DownScaleGameObject(gameObject);
        if (allTimeSteps)
        {
          string fullMeshName = gameObject.name;
          string timestepName = fullMeshName.Split('_')[1];
          QueueManager.PushChange(fullMeshName.Substring(fullMeshName.LastIndexOf('_') + 1), "scale 2 " + (scaleFactor / 100f), timestepName);
        }
      }
      currentScaleText.text = currentSelectedGameObjects[0].transform.localScale.z.ToString("0.00");
    }
  }

  private void DownScaleGameObject(GameObject gameObject)
  {
    // Get the initial geometric center of the mesh in world space
    Vector3 initialGeometricCenter = gameObject.transform.TransformPoint(meshCenter);
    gameObject.transform.localScale -= gameObject.transform.localScale * (scaleFactor / 100f);
    // Get the post geometric center of the mesh in world space
    Vector3 postGeometricCenter = gameObject.transform.TransformPoint(meshCenter);
    // Calculate the distance the object "drifts" and correct it by subtracting that compensation vector
    Vector3 compensationVector = postGeometricCenter - initialGeometricCenter;
    gameObject.transform.position = gameObject.transform.position - compensationVector;
  }

  private void StartLoop(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
  {
    if (currentSelectedGameObjects.Count > 0)
    {
      doUpdateLoop = true;
      initial_distance = Vector3.Distance(hand1.transform.position, hand2.transform.position);

      foreach (GameObject gameObject in gameObjectsToEffect)
      {
        initialScales.Add(gameObject.transform.localScale);
      }
    }
  }

  private void StopLoop(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
  {
    if (currentSelectedGameObjects.Count > 0)
    {
      doUpdateLoop = false;

      if (allTimeSteps)
      {
        foreach (GameObject gameObject in gameObjectsToEffect)
        {
          //apply to all time steps
          string fullMeshName = gameObject.name;
          string timestepName = fullMeshName.Split('_')[1];
          QueueManager.PushChange(fullMeshName.Substring(fullMeshName.LastIndexOf('_') + 1), "scale 0 " + gameObject.transform.localScale.z, timestepName);
        }
      }

      initialScales.Clear();
    }
  }

  private void Update()
  {
    if (doUpdateLoop)
      UpdateScale();
  }

  private void UpdateScale()
  {
    //scale the object scaled by distance between the hands
    float current_distance = Vector3.Distance(hand1.transform.position, hand2.transform.position);
    float scale = current_distance / initial_distance;

    int index = 0;
    foreach (GameObject gameObject in gameObjectsToEffect)
    {
      // Get the initial geometric center of the mesh in world space
      Vector3 initialGeometricCenter = gameObject.transform.TransformPoint(meshCenter);
      Vector3 initial_scale = initialScales[index];
      float modx = scale * initial_scale.x;
      float mody = scale * initial_scale.y;
      float modz = scale * initial_scale.z;
      gameObject.transform.localScale = new Vector3(modx, mody, modz);
      // Get the post geometric center of the mesh in world space
      Vector3 postGeometricCenter = gameObject.transform.TransformPoint(meshCenter);
      // Calculate the distance the object "drifts" and correct it by subtracting that compensation vector
      Vector3 compensationVector = postGeometricCenter - initialGeometricCenter;
      gameObject.transform.position = gameObject.transform.position - compensationVector;
      index++;
    }

    currentScaleText.text = currentSelectedGameObjects[0].transform.localScale.z.ToString("N2");
  }

  public void ToolEffectChanged()
  {
    toolEffect = toolEffectDropdown.value;
    UpdateToolEffect();
  }

  private void UpdateToolEffect()
  {
    if (currentSelectedGameObjects.Count > 0)
    {
      gameObjectsToEffect.Clear();
      if (toolEffect == 2)
      {
        Transform parent = currentSelectedGameObjects[0].transform.parent;
        foreach (Transform child in parent)
        {
          GameObject gameObject = child.gameObject;
          gameObjectsToEffect.Add(gameObject);
        }
      }
      else
      {
        foreach (GameObject currentSelectedGameObject in currentSelectedGameObjects)
        {
          gameObjectsToEffect.Add(currentSelectedGameObject);
        }

        if (toolEffect == 1)
        {
          foreach (GameObject currentSelectedGameObjectPair in currentSelectedGameObjectPairs)
          {
            gameObjectsToEffect.Add(currentSelectedGameObjectPair);
          }
        }
      }
    }
  }

  public void AllTimeStepsChanged(Toggle allTimeStepsToggle)
  {
    allTimeSteps = allTimeStepsToggle.isOn;
  }

  public void UseLaserPointerChanged(Toggle useLaserPointerToggle)
  {
    useLaserPointer = useLaserPointerToggle.isOn;
    if (useLaserPointer)
    {
      //show laser
      EnableLaser(true);
    }
    else
    {
      //hide laser
      EnableLaser(false);
    }
  }

  private void UpdateSelectedGameObjectsUsingLaser(object sender, PointerEventArgs e)
  {
    SelectionManager.GameObjectSelect(e.target.gameObject);
    currentSelectedGameObjects = SelectionManager.currentSelectedGameObjects;
    currentSelectedGameObjectPairs.Clear();
    foreach (GameObject currentSelectedGameObject in currentSelectedGameObjects)
    {
      //get current selected game object pair
      if (currentSelectedGameObject.name.Contains("Exterior"))
      {
        currentSelectedGameObjectPairs.Add(SelectionManager.gameObjects[currentSelectedGameObject.name.Replace("Exterior", "Interior")]);
      }
      else
      {
        currentSelectedGameObjectPairs.Add(SelectionManager.gameObjects[currentSelectedGameObject.name.Replace("Interior", "Exterior")]);
      }
    }
    //update UI and tool info
    if (currentSelectedGameObjects.Count > 0)
    {
      hudCurrentSelectedGameObjectName.text = currentSelectedGameObjects[0].name;
      currentScaleText.text = currentSelectedGameObjects[0].transform.localScale.z.ToString("0.00");
      UpdateToolEffect();
    }
    else
    {
      hudCurrentSelectedGameObjectName.text = "None";
      currentScaleText.text = "---";
      gameObjectsToEffect.Clear();
    }
  }
}
