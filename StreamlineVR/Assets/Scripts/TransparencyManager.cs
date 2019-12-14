using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;
using Valve.VR.Extras;
using System.Collections.Generic;
//using Unity.Jobs;
//using Unity.Collections;

public class TransparencyManager : MonoBehaviour
{
  [SerializeField]
  private SteamVR_LaserPointer laserPointer;
  [SerializeField]
  private GameObject laserCube;
  [SerializeField]
  private LinearMapping transLinearMap;
  [SerializeField]
  private Text levelBox;
  [SerializeField]
  private Material transparent;
  [SerializeField]
  private Material opaque;
  [SerializeField]
  private Material transparentVert;
  [SerializeField]
  private Material opaqueVert;
  [SerializeField]
  private GameObject transPanel;
  [SerializeField]
  private GameObject menuPanel;
  [SerializeField]
  private Text hudCurrentSelectedGameObjectName;
  [SerializeField]
  private GameObject handle;
  [SerializeField]
  private Dropdown toolEffectDropdown;

  private float prevValue;
  private bool doUpdateLoop;
  private bool useLaserPointer;
  private bool allTimeSteps;
  private int toolEffect;
  private List<GameObject> currentSelectedGameObjects;
  private List<GameObject> currentSelectedGameObjectPairs;
  private List<GameObject> gameObjectsToEffect;

  private void Start()
  {
    prevValue = 1f;
    doUpdateLoop = false;
    useLaserPointer = true;
    allTimeSteps = true;
    toolEffect = 1;
    currentSelectedGameObjects = new List<GameObject>();
    currentSelectedGameObjectPairs = new List<GameObject>();
    gameObjectsToEffect = new List<GameObject>();
  }

  public void EnterMenu()
  {
    menuPanel.SetActive(false);
    transPanel.SetActive(true);
    if (useLaserPointer)
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
      //set linear drive and level box to match selected object alpha
      float alpha = currentSelectedGameObjects[0].GetComponent<MeshRenderer>().material.color.a;
      levelBox.text = ((int)(alpha * 100)).ToString() + "%";
      transLinearMap.value = alpha;
      handle.transform.localPosition = new Vector3(0, 0.2f, 0.1f + (0.65f * alpha));
      UpdateToolEffect();
    }
    else
    {
      levelBox.text = "---%";
      transLinearMap.value = 1f;
      handle.transform.localPosition = new Vector3(0, 0.2f, 0.75f);
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
    transPanel.SetActive(false);
    menuPanel.SetActive(true);
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

  private void UpdateTransparency()
  {
    int percentInt = (int)(transLinearMap.value * 100);
    float value = (float)(percentInt / 100.0);

    if (Mathf.Abs(value - prevValue) < 0.009)
      return;

    levelBox.text = percentInt.ToString() + "%";

    foreach (GameObject gameObject in gameObjectsToEffect)
    {
      Material initialMaterial = gameObject.GetComponent<MeshRenderer>().material;
      Material newMaterial;
      bool usingVertMaterial = initialMaterial.name.Contains("Vert");

      if (value == 1f)
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

      Color[] meshColors = gameObject.GetComponent<MeshFilter>().mesh.colors;
      for (int i = 0; i < meshColors.Length; i++)
      {
        Color newColor = meshColors[i];
        newColor.a = value;
        meshColors[i] = newColor;
      }

      //NativeArray<Color> colorArray = new NativeArray<Color>(meshColors.Length, Allocator.TempJob);
      //for (int i = 0; i < meshColors.Length; i++)
      //{
      //  colorArray[i] = meshColors[i];
      //}
      //ColorParallelJob colorParallelJob = new ColorParallelJob
      //{
      //  colorArray = colorArray
      //};
      //JobHandle jobHandle = colorParallelJob.Schedule(meshColors.Length, 100);
      //jobHandle.Complete();
      //for (int i = 0; i < meshColors.Length; i++)
      //{
      //  meshColors[i] = colorArray[i];
      //}
      //colorArray.Dispose();

      gameObject.GetComponent<MeshFilter>().mesh.colors = meshColors;
      Color originalColor = initialMaterial.color;
      newMaterial.color = new Color(originalColor.r, originalColor.g, originalColor.b, value);
      gameObject.GetComponent<MeshRenderer>().material = newMaterial;
      Destroy(initialMaterial);
    }

    prevValue = value;
  }

  //private struct ColorParallelJob : IJobParallelFor
  //{
  //  public NativeArray<Color> colorArray;
  //  public float alpha;

  //  public void Execute(int index)
  //  {
  //    Color newColor = colorArray[index];
  //    newColor.a = alpha;
  //    colorArray[index] = newColor;
  //  }
  //}

  public void StartUpdatingLoop()
  {
    if (currentSelectedGameObjects.Count > 0)
    {
      doUpdateLoop = true;
    }
  }

  public void StopUpdatingLoop()
  {
    if (currentSelectedGameObjects.Count > 0)
    {
      doUpdateLoop = false;

      if (allTimeSteps)
      {
        int temp = (int)(transLinearMap.value * 100);
        float value = (float)(temp / 100.0);

        foreach (GameObject gameObject in gameObjectsToEffect)
        {
          //apply to all time steps
          string fullMeshName = gameObject.name;
          string timestepName = fullMeshName.Split('_')[1];
          QueueManager.PushChange(fullMeshName.Substring(fullMeshName.LastIndexOf('_') + 1), "transparency " + value, timestepName);
        }
      }
    }
  }

  private void Update()
  {
    if (doUpdateLoop)
      UpdateTransparency();
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
      //set linear drive and level box to match selected object alpha
      float alpha = currentSelectedGameObjects[0].GetComponent<MeshRenderer>().material.color.a;
      levelBox.text = ((int)(alpha * 100)).ToString() + "%";
      transLinearMap.value = alpha;
      handle.transform.localPosition = new Vector3(0, 0.2f, 0.1f + (0.65f * alpha));
      UpdateToolEffect();
    }
    else
    {
      hudCurrentSelectedGameObjectName.text = "None";
      levelBox.text = "---%";
      transLinearMap.value = 1f;
      handle.transform.localPosition = new Vector3(0, 0.2f, 0.75f);
      gameObjectsToEffect.Clear();
    }
  }
}
