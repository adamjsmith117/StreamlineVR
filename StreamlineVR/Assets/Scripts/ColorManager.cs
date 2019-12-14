using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;

public class ColorManager : MonoBehaviour
{
  [SerializeField]
  private SteamVR_LaserPointer laserPointer;
  [SerializeField]
  private GameObject laserCube;
  [SerializeField]
  private Image colorSelected;
  [SerializeField]
  private GameObject colorPanel;
  [SerializeField]
  private GameObject menuPanel;
  [SerializeField]
  private Text hudCurrentSelectedGameObjectName;
  [SerializeField]
  private GameObject normalPalette;
  [SerializeField]
  private GameObject colorBlindPalette;
  [SerializeField]
  private Material opaqueVert;
  [SerializeField]
  private Material transparentVert;
  [SerializeField]
  private Material opaque;
  [SerializeField]
  private Material transparent;
  [SerializeField]
  private Text toggleVertColors;
  [SerializeField]
  private Dropdown toolEffectDropdown;

  private static bool currentSelectedGameObjectUsingVertColors;
  private bool usingNormalPalette;
  private bool useLaserPointer;
  private bool allTimeSteps;
  private int toolEffect;
  private Color currentSelectedColor;
  private List<GameObject> currentSelectedGameObjects;
  private List<GameObject> currentSelectedGameObjectPairs;
  private List<GameObject> gameObjectsToEffect;

  private void Start()
  {
    currentSelectedGameObjectUsingVertColors = false;
    usingNormalPalette = true;
    colorBlindPalette.SetActive(false);
    useLaserPointer = true;
    allTimeSteps = true;
    toolEffect = 1;
    currentSelectedColor = Color.white;
    currentSelectedGameObjects = new List<GameObject>();
    currentSelectedGameObjectPairs = new List<GameObject>();
    gameObjectsToEffect = new List<GameObject>();
  }

  public void EnterMenu()
  {
    menuPanel.SetActive(false);
    colorPanel.SetActive(true);
    if(useLaserPointer)
    {
      EnableLaser(true);
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
      if (currentSelectedGameObjects[0].GetComponent<MeshRenderer>().material.name.Contains("Vert"))
      {
        toggleVertColors.text = "Stop Using Imported Vert Colors";
        currentSelectedGameObjectUsingVertColors = true;
      }
      else
      {
        toggleVertColors.text = "Use Imported Vert Colors";
        currentSelectedGameObjectUsingVertColors = false;
      }
      UpdateToolEffect();
    }
    else
    {
      //update UI and tool info
      toggleVertColors.text = "Use Imported Vert Colors";
      currentSelectedGameObjectUsingVertColors = false;
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
    }
    colorPanel.SetActive(false);
    menuPanel.SetActive(true);
  }

  private void EnableLaser( bool state )
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

  public void SwitchColorPalettes()
  {
    if (usingNormalPalette)
    {
      normalPalette.SetActive(false);
      colorBlindPalette.SetActive(true);
      usingNormalPalette = false;
    }
    else
    {
      colorBlindPalette.SetActive(false);
      normalPalette.SetActive(true);
      usingNormalPalette = true;
    }
  }

  public void UseLaserPointerChanged( Toggle useLaserPointerToggle )
  {
    if(useLaserPointerToggle.isOn)
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

  public void ChangeSelectedColor( Button selectColorButton )
  {
    currentSelectedColor = selectColorButton.colors.normalColor;
    colorSelected.color = currentSelectedColor;
  }

  private void UpdateSelectedGameObjectsUsingLaser( object sender, PointerEventArgs e )
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
      if (currentSelectedGameObjects[0].GetComponent<MeshRenderer>().material.name.Contains("Vert"))
      {
        toggleVertColors.text = "Stop Using Imported Vert Colors";
        currentSelectedGameObjectUsingVertColors = true;
      }
      else
      {
        toggleVertColors.text = "Use Imported Vert Colors";
        currentSelectedGameObjectUsingVertColors = false;
      }
      UpdateToolEffect();
    }
    else
    {
      hudCurrentSelectedGameObjectName.text = "None";
      toggleVertColors.text = "Use Imported Vert Colors";
      currentSelectedGameObjectUsingVertColors = false;
      gameObjectsToEffect.Clear();
    }
  }

  public void ColorCurrentSelectedGameObject()
  {
    if (currentSelectedGameObjects.Count > 0)
    {
      foreach (GameObject gameObject in gameObjectsToEffect)
      {
        ColorGameObject(gameObject);
        if (allTimeSteps)
        {
          string fullMeshName = gameObject.name;
          string timestepName = fullMeshName.Split('_')[1];
          QueueManager.PushChange(fullMeshName.Substring(fullMeshName.LastIndexOf('_') + 1), "color " + ColorUtility.ToHtmlStringRGBA(currentSelectedColor), timestepName);
        }
      }
    }
  }

  private void ColorGameObject(GameObject gameObject)
  {
    Color currentSelectedColorWithAlpha = currentSelectedColor;
    float originalAlpha = gameObject.GetComponent<MeshRenderer>().material.color.a;
    if (originalAlpha == 1f)
    {
      Material oldMaterial = gameObject.GetComponent<MeshRenderer>().material;
      gameObject.GetComponent<MeshRenderer>().material = new Material(opaque);
      Destroy(oldMaterial);
    }
    else
    {
      Material oldMaterial = gameObject.GetComponent<MeshRenderer>().material;
      gameObject.GetComponent<MeshRenderer>().material = new Material(transparent);
      Destroy(oldMaterial);
    }
    currentSelectedColorWithAlpha.a = originalAlpha;
    gameObject.GetComponent<MeshRenderer>().material.color = currentSelectedColorWithAlpha;
  }

  public void ToggleVertColors()
  {
    if (currentSelectedGameObjects.Count > 0)
    {
      if (!currentSelectedGameObjectUsingVertColors)
      {
        foreach (GameObject gameObject in gameObjectsToEffect)
        {
          VertColorOn(gameObject);
          if (allTimeSteps)
          {
            string fullMeshName = gameObject.name;
            string timestepName = fullMeshName.Split('_')[1];
            QueueManager.PushChange(fullMeshName.Substring(fullMeshName.LastIndexOf('_') + 1), "material 0", timestepName);
          }
        }

        toggleVertColors.text = "Stop Using Imported Vert Colors";
        currentSelectedGameObjectUsingVertColors = true;
      }
      else
      {
        foreach (GameObject gameObject in gameObjectsToEffect)
        {
          VertColorOff(gameObject);
          if (allTimeSteps)
          {
            string fullMeshName = gameObject.name;
            string timestepName = fullMeshName.Split('_')[1];
            QueueManager.PushChange(fullMeshName.Substring(fullMeshName.LastIndexOf('_') + 1), "material 1", timestepName);
          }
        }

        toggleVertColors.text = "Use Imported Vert Colors";
        currentSelectedGameObjectUsingVertColors = false;
      }
    }
  }

  private void VertColorOn(GameObject gameObject)
  {
    // using opaqueVert material as a container to save original RGBA since opaqueVert does not use it's RGBA for rendering (saved like this so we can 'untoggle' back to original color and alpha)
    Color originalColor = gameObject.GetComponent<Renderer>().material.color;
    if (originalColor.a == 1f)
    {
      Material oldMaterial = gameObject.GetComponent<MeshRenderer>().material;
      gameObject.GetComponent<Renderer>().material = new Material(opaqueVert);
      Destroy(oldMaterial);
    }
    else
    {
      Material oldMaterial = gameObject.GetComponent<MeshRenderer>().material;
      gameObject.GetComponent<Renderer>().material = new Material(transparentVert);
      Destroy(oldMaterial);
    }
    gameObject.GetComponent<Renderer>().material.color = originalColor;
  }

  private void VertColorOff(GameObject gameObject)
  {
    Color originalColor = gameObject.GetComponent<Renderer>().material.color;
    if (originalColor.a == 1f)
    {
      Material oldMaterial = gameObject.GetComponent<MeshRenderer>().material;
      gameObject.GetComponent<Renderer>().material = new Material(opaque);
      Destroy(oldMaterial);
    }
    else
    {
      Material oldMaterial = gameObject.GetComponent<MeshRenderer>().material;
      gameObject.GetComponent<Renderer>().material = new Material(transparent);
      Destroy(oldMaterial);
    }
    gameObject.GetComponent<Renderer>().material.color = originalColor;
  }
}