using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.Extras;

public class TransformationManager : MonoBehaviour
{
  [SerializeField]
  private SteamVR_LaserPointer laserPointer;
  [SerializeField]
  private GameObject laserCube;
  [SerializeField]
  private GameObject translationPanel;
  [SerializeField]
  private GameObject menuPanel;
  [SerializeField]
  private Transform rightHandTransform;
  [SerializeField]
  private Text currentSelectedGameObjectName;
  [SerializeField]
  private Text currentSelectedGameObjectPosition;
  [SerializeField]
  private Text currentSelectedGameObjectRotation;
  [SerializeField]
  private Text hudCurrentSelectedGameObjectName;
  [SerializeField]
  private Dropdown toolEffectDropdown;
  [SerializeField]
  private SteamVR_Input_Sources rightHand;
  [SerializeField]
  private SteamVR_Input_Sources leftHand;

  private GameObject currentHoveredGameObject;
  private List<GameObject> currentSelectedGameObjects;
  private List<GameObject> currentSelectedGameObjectPairs;
  private List<GameObject> gameObjectsToEffect;
  private List<Vector3> initialEulerAngles;
  private bool useLaserPointer;
  private bool allTimeSteps;
  private bool lockRotation;
  private int toolEffect;
  private SteamVR_Input_ActionSet_default actionSet;
  private Transform originalParent;
  private bool movingCurrentSelectedGameObject;
  private static Vector3 meshCenter;

  private void Start()
  {
    useLaserPointer = true;
    allTimeSteps = true;
    lockRotation = true;
    toolEffect = 1;
    currentHoveredGameObject = null;
    currentSelectedGameObjects = new List<GameObject>();
    currentSelectedGameObjectPairs = new List<GameObject>();
    gameObjectsToEffect = new List<GameObject>();
    initialEulerAngles = new List<Vector3>();
    movingCurrentSelectedGameObject = false;
    originalParent = null;
    actionSet = new SteamVR_Input_ActionSet_default();
    meshCenter = Vector3.zero;
  }

  public void EnterMenu()
  {
    menuPanel.SetActive(false);
    translationPanel.SetActive(true);
    if (useLaserPointer)
    {
      EnableLaserAndGrab(true);
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
      Vector3 currentPosition = currentSelectedGameObjects[0].transform.TransformPoint(meshCenter);
      Vector3 currentRotation = currentSelectedGameObjects[0].transform.eulerAngles;
      currentSelectedGameObjectName.text = currentSelectedGameObjects[0].name;
      currentSelectedGameObjectPosition.text = "Position\nX:   " + currentPosition.x.ToString("0.00") + "\nY:   " + currentPosition.y.ToString("0.00") + "\nZ:   " + currentPosition.z.ToString("0.00");
      currentSelectedGameObjectRotation.text = "Rotation\nX:   " + currentRotation.x.ToString("0.00") + "\nY:   " + currentRotation.y.ToString("0.00") + "\nZ:   " + currentRotation.z.ToString("0.00");
      UpdateToolEffect();
    }
    else
    {
      currentSelectedGameObjectName.text = "None";
      currentSelectedGameObjectPosition.text = "Position\nX:   ---\nY:   ---\nZ:   ---";
      currentSelectedGameObjectRotation.text = "Rotation\nX:   ---\nY:   ---\nZ:   ---";
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
      EnableLaserAndGrab(false);
    }
    translationPanel.SetActive(false);
    menuPanel.SetActive(true);
  }

  private void EnableLaserAndGrab(bool state)
  {
    laserPointer.enabled = state;
    laserCube.SetActive(state);
    if (state)
    {
      laserPointer.PointerIn += GrabObjectUsingLaser;
      laserPointer.PointerOut += ResetHovered;
      actionSet.Tgrab.onStateDown += Pickup;
      actionSet.Tgrab.onStateUp += LetGo;
    }
    else
    {
      laserPointer.PointerIn -= GrabObjectUsingLaser;
      laserPointer.PointerOut -= ResetHovered;
      actionSet.Tgrab.onStateDown -= Pickup;
      actionSet.Tgrab.onStateUp -= LetGo;
    }
  }

  public void UseLaserPointerChanged(Toggle useLaserPointerToggle)
  {
    useLaserPointer = useLaserPointerToggle.isOn;
    if (useLaserPointer)
    {
      //show laser
      EnableLaserAndGrab(true);
    }
    else
    {
      //hide laser
      EnableLaserAndGrab(false);
    }
  }

  public static void SetMeshCenter(Vector3 center)
  {
    meshCenter = new Vector3(center.x, center.y, center.z);
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

  public void LockRotationChanged(Toggle lockRotationToggle)
  {
    lockRotation = lockRotationToggle.isOn;
  }

  private void Update()
  {
    if (movingCurrentSelectedGameObject)
    {
      if (lockRotation)
      {
        int index = 0;
        foreach (GameObject gameObject in gameObjectsToEffect)
        {
          Vector3 originalRotation = initialEulerAngles[index];
          //get original rotation angles
          float x = originalRotation.x;
          float y = originalRotation.y;
          float z = originalRotation.z;
          //get the difference from the original
          x -= gameObject.transform.eulerAngles.x;
          y -= gameObject.transform.eulerAngles.y;
          z -= gameObject.transform.eulerAngles.z;
          Vector3 gameObjectCenter = gameObject.transform.TransformPoint(meshCenter);
          //apply the reverse rotation
          gameObject.transform.RotateAround(gameObjectCenter, new Vector3(0, 0, 1), z);
          gameObject.transform.RotateAround(gameObjectCenter, new Vector3(0, 1, 0), y);
          gameObject.transform.RotateAround(gameObjectCenter, new Vector3(1, 0, 0), x);
          index++;
        }
      }
      Vector3 currentPosition = currentSelectedGameObjects[0].transform.TransformPoint(meshCenter);
      Vector3 currentRotation = currentSelectedGameObjects[0].transform.eulerAngles;
      currentSelectedGameObjectPosition.text = "Position\nX:   " + currentPosition.x.ToString("0.00") + "\nY:   " + currentPosition.y.ToString("0.00") + "\nZ:   " + currentPosition.z.ToString("0.00");
      currentSelectedGameObjectRotation.text = "Rotation\nX:   " + currentRotation.x.ToString("0.00") + "\nY:   " + currentRotation.y.ToString("0.00") + "\nZ:   " + currentRotation.z.ToString("0.00");
    }
    if (currentSelectedGameObjects.Count > 0 && translationPanel.activeSelf == true)
    {
      //rotation in x and y with touchpad with a set speed
      Vector2 rot = actionSet.touchpadact.GetAxis(SteamVR_Input_Sources.RightHand);
      if (rot != Vector2.zero)
      {
        if (lockRotation)
        {
          //send haptic feedback
          actionSet.Haptic.Execute(0, 0.005f, 0.005f, 1f, rightHand);
          actionSet.Haptic.Execute(0, 0.005f, 0.005f, 1f, leftHand);
        }
        else
        {
          Vector3 go = new Vector3(rot.y, -rot.x, 0);
          go = rightHandTransform.TransformVector(go);

          foreach (GameObject gameObject in gameObjectsToEffect)
          {
            gameObject.transform.RotateAround(gameObject.transform.TransformPoint(meshCenter), go, 30 * Time.deltaTime * go.magnitude);
          }

          Vector3 currentPosition = currentSelectedGameObjects[0].transform.TransformPoint(meshCenter);
          Vector3 currentRotation = currentSelectedGameObjects[0].transform.eulerAngles;
          currentSelectedGameObjectPosition.text = "Position\nX:   " + currentPosition.x.ToString("0.00") + "\nY:   " + currentPosition.y.ToString("0.00") + "\nZ:   " + currentPosition.z.ToString("0.00");
          currentSelectedGameObjectRotation.text = "Rotation\nX:   " + currentRotation.x.ToString("0.00") + "\nY:   " + currentRotation.y.ToString("0.00") + "\nZ:   " + currentRotation.z.ToString("0.00");
        }
      }
    }
  }

  private void ResetPosition(GameObject gameObject)
  {
    gameObject.transform.position = -1 * meshCenter * gameObject.transform.localScale.z;
  }

  private void ResetRotation(GameObject gameObject)
  {
    gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
  }

  public void ResetPositionAndRotation()
  {
    if (currentSelectedGameObjects.Count > 0)
    {
      foreach (GameObject gameObject in gameObjectsToEffect)
      {
        ResetPosition(gameObject);
        ResetRotation(gameObject);
        if (allTimeSteps)
        {
          string fullMeshName = gameObject.name;
          string timestepName = fullMeshName.Split('_')[1];
          QueueManager.PushChange(fullMeshName.Substring(fullMeshName.LastIndexOf('_') + 1), "transform 1", timestepName);
        }
      }

      Vector3 currentPosition = currentSelectedGameObjects[0].transform.TransformPoint(meshCenter);
      Vector3 currentRotation = currentSelectedGameObjects[0].transform.eulerAngles;
      currentSelectedGameObjectPosition.text = "Position\nX:   " + currentPosition.x.ToString("0.00") + "\nY:   " + currentPosition.y.ToString("0.00") + "\nZ:   " + currentPosition.z.ToString("0.00");
      currentSelectedGameObjectRotation.text = "Rotation\nX:   " + currentRotation.x.ToString("0.00") + "\nY:   " + currentRotation.y.ToString("0.00") + "\nZ:   " + currentRotation.z.ToString("0.00");
    }
  }

  private void GrabObjectUsingLaser(object sender, PointerEventArgs e)
  {
    currentHoveredGameObject = e.target.gameObject;
  }

  private void ResetHovered(object sender, PointerEventArgs e)
  {
    currentHoveredGameObject = null;
  }

  private void LetGo(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
  {
    if (movingCurrentSelectedGameObject)
    {
      foreach (GameObject gameObject in gameObjectsToEffect)
      {
        gameObject.transform.SetParent(originalParent);
        if (allTimeSteps)
        {
          string fullMeshName = gameObject.name;
          string timestepName = fullMeshName.Split('_')[1];
          //Debug.Log("CurrentSelectedGameObject transform.rotation: " + currentSelectedGameObject.transform.rotation);
          //Debug.Log("CurrentSelectedGameObject transform.localRotation: " + currentSelectedGameObject.transform.localRotation);
          //Debug.Log("CurrentSelectedGameObject transform.localEulerAngles: " + currentSelectedGameObject.transform.localEulerAngles);
          //Debug.Log("CurrentSelectedGameObject transform.eulerAngles: " + currentSelectedGameObject.transform.eulerAngles);
          //Debug.Log("CurrentSelectedGameObject transform.position: " + currentSelectedGameObject.transform.position);
          //Debug.Log("CurrentSelectedGameObject transform.localPosition: " + currentSelectedGameObject.transform.localPosition);
          //Debug.Log("CurrentSelectedGameObject transform.localRotation.eulerAngles: " + currentSelectedGameObject.transform.localRotation.eulerAngles);
          //Debug.Log("CurrentSelectedGameObject Quaternion.Euler(transform.localRotation.eulerAngles): " + Quaternion.Euler(currentSelectedGameObject.transform.localRotation.eulerAngles));
          //Debug.Log("Quaternion.Euler(transform.localRotation.eulerAngles) - x: " + Quaternion.Euler(currentSelectedGameObject.transform.localRotation.eulerAngles).x);
          //Debug.Log("Quaternion.Euler(transform.localRotation.eulerAngles) - y: " + Quaternion.Euler(currentSelectedGameObject.transform.localRotation.eulerAngles).y);
          //Debug.Log("Quaternion.Euler(transform.localRotation.eulerAngles) - z: " + Quaternion.Euler(currentSelectedGameObject.transform.localRotation.eulerAngles).z);
          //Debug.Log("Quaternion.Euler(transform.localRotation.eulerAngles) - w: " + Quaternion.Euler(currentSelectedGameObject.transform.localRotation.eulerAngles).w);
          string posVal = "(" + gameObject.transform.localPosition.x + ", " + gameObject.transform.localPosition.y + ", " + gameObject.transform.localPosition.z + ")";
          string rotVal = "(" + gameObject.transform.localRotation.x + ", " + gameObject.transform.localRotation.y + ", " + gameObject.transform.localRotation.z + ", " + gameObject.transform.localRotation.w + ")";
          QueueManager.PushChange(fullMeshName.Substring(fullMeshName.LastIndexOf('_') + 1), "transform 0 " + posVal + " " + rotVal, timestepName);
        }
      }

      movingCurrentSelectedGameObject = false;
      initialEulerAngles.Clear();
    }
  }

  private void Pickup(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
  {
    if (currentHoveredGameObject)
    {
      SelectionManager.GameObjectSelect(currentHoveredGameObject);
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
        currentSelectedGameObjectName.text = currentSelectedGameObjects[0].name;
        UpdateToolEffect();

        //set up to start moving
        originalParent = currentSelectedGameObjects[0].transform.parent;
        foreach (GameObject gameObject in gameObjectsToEffect)
        {
          initialEulerAngles.Add(gameObject.transform.eulerAngles);
          //set to child of right hand
          gameObject.transform.SetParent(rightHandTransform);
        }

        movingCurrentSelectedGameObject = true;
      }
      else
      {
        hudCurrentSelectedGameObjectName.text = "None";
        currentSelectedGameObjectName.text = "None";
        currentSelectedGameObjectPosition.text = "Position\nX:   ---\nY:   ---\nZ:   ---";
        currentSelectedGameObjectRotation.text = "Rotation\nX:   ---\nY:   ---\nZ:   ---";
        gameObjectsToEffect.Clear();
      }
    }
  }
}
