using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;
using VRScrollView;

public class SelectionManager : MonoBehaviour, ICustomListViewAdapter
{
  [SerializeField]
  private SteamVR_LaserPointer laserPointer;
  [SerializeField]
  private GameObject laserCube;
  [SerializeField]
  private GameObject selectPanel;
  [SerializeField]
  private GameObject menuPanel;
  [SerializeField]
  private Text currentSelectedGameObjectName;
  [SerializeField]
  private Text hudCurrentSelectedGameObjectName;
  [SerializeField]
  private GameObjectInfoItem gameObjectInfoPrefab;
  [SerializeField]
  private CustomRecycledListView listView;
  [SerializeField]
  private RectTransform gameObjectContainer;

  public static List<GameObject> currentSelectedGameObjects;
  private bool useLaserPointer;
  public static List<CustomGameObject> customGameObjects;
  // Dictionary containing all gameObjects loaded into project
  public static Dictionary<string, GameObject> gameObjects;
  public static Dictionary<string, int> gameObjectIds;
  public static bool toggleUpdated;

  private void Start()
  {
    ItemHeight = 40f;
    customGameObjects = new List<CustomGameObject>();
    gameObjects = new Dictionary<string, GameObject>();
    gameObjectIds = new Dictionary<string, int>();
    currentSelectedGameObjects = new List<GameObject>();
    currentSelectedGameObjectName.text = "None";
    useLaserPointer = true;
    toggleUpdated = false;
    listView.SetAdapter(this);
  }

  public void EnterMenu()
  {
    menuPanel.SetActive(false);
    selectPanel.SetActive(true);
    if (useLaserPointer)
    {
      EnableLaser(true);
    }
    if (currentSelectedGameObjects.Count > 0)
    {
      foreach (GameObject currentSelectedGameObject in currentSelectedGameObjects)
      {
        CustomGameObject customGameObject = customGameObjects[gameObjectIds[currentSelectedGameObject.name] - 1];
        customGameObject.selected = true;
        customGameObjects[gameObjectIds[currentSelectedGameObject.name] - 1] = customGameObject;
        //activate selection outline on current selected game object
        Outline outlineComponent = currentSelectedGameObject.GetComponent<Outline>();
        if (outlineComponent)
        {
          outlineComponent.enabled = true;
        }
      }
      currentSelectedGameObjectName.text = currentSelectedGameObjects[0].name;
    }
    else
    {
      currentSelectedGameObjectName.text = "None";
    }
    listView.UpdateList();
  }

  public void ExitMenu()
  {
    //deactivate selection outline on current selected game object
    if (currentSelectedGameObjects.Count > 0)
    {
      foreach (GameObject currentSelectedGameObject in currentSelectedGameObjects)
      {
        Outline outlineComponent = currentSelectedGameObject.GetComponent<Outline>();
        if (outlineComponent)
        {
          outlineComponent.enabled = false;
        }
      }
    }
    if (useLaserPointer)
    {
      EnableLaser(false);
    }
    selectPanel.SetActive(false);
    menuPanel.SetActive(true);
  }

  #region Interface Methods
  public int Count { get { return customGameObjects.Count; } }
  public float ItemHeight { get; private set; }

  public CustomListItem CreateItem()
  {
    GameObjectInfoItem item = Instantiate(gameObjectInfoPrefab, gameObjectContainer, false);
    return item;
  }

  public void SetItemContent(CustomListItem item)
  {
    GameObjectInfoItem gameObject = (GameObjectInfoItem)item;
    CustomGameObject gameObjectInfo = customGameObjects[item.Position];
    gameObject.SetInfo(gameObjectInfo.selected, gameObjectInfo.nickName, gameObjectInfo.id, gameObjectInfo.objectName);
  }
  #endregion

  // Method used to poplate our list of all gameobjects
  public static void PopulateGameObjects(Dictionary<string, GameObject> dictionary)
  {
    gameObjects = dictionary;
    int idNum = 1;
    foreach (KeyValuePair<string, GameObject> pair in gameObjects)
    {
      //PlaybackManager.currentTimeStep;
      CustomGameObject customGameObject = new CustomGameObject
      {
        selected = false,
        nickName = "TestNickName",
        id = idNum,
        objectName = pair.Key,
        gameObject = pair.Value
      };
      customGameObjects.Add(customGameObject);
      gameObjectIds.Add(pair.Key, idNum);
      idNum++;
    }
  }

  private void EnableLaser(bool state)
  {
    laserPointer.enabled = state;
    laserCube.SetActive(state);
    if (state)
    {
      laserPointer.PointerClick += GameObjectSelectUsingLaser;
    }
    else
    {
      laserPointer.PointerClick -= GameObjectSelectUsingLaser;
    }
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

  private void Update()
  {
    if (toggleUpdated)
    {
      if (currentSelectedGameObjects.Count == 0)
      {
        currentSelectedGameObjectName.text = "None";
        hudCurrentSelectedGameObjectName.text = "None";
      }
      else
      {
        currentSelectedGameObjectName.text = currentSelectedGameObjects[0].name;
        hudCurrentSelectedGameObjectName.text = currentSelectedGameObjects[0].name;
      }
      toggleUpdated = false;
    }
  }

  public void DeselectSelectedGameObjects()
  {
    if (currentSelectedGameObjects.Count > 0)
    {
      foreach (GameObject currentSelectedGameObject in currentSelectedGameObjects)
      {
        //update toggle on selection menu
        CustomGameObject customGameObject = customGameObjects[gameObjectIds[currentSelectedGameObject.name] - 1];
        customGameObject.selected = false;
        customGameObjects[gameObjectIds[currentSelectedGameObject.name] - 1] = customGameObject;
        //remove object outline
        Outline outlineComponent = currentSelectedGameObject.GetComponent<Outline>();
        if (outlineComponent)
        {
          outlineComponent.enabled = false;
        }
      }
      //remove all from selected game objects
      currentSelectedGameObjects.Clear();
      listView.UpdateList();
      currentSelectedGameObjectName.text = "None";
      hudCurrentSelectedGameObjectName.text = "None";
    }
  }

  private void GameObjectSelectUsingLaser(object sender, PointerEventArgs e)
  {
    if (currentSelectedGameObjects.Count > 0)
    {
      bool deselected = false;
      for (int i = currentSelectedGameObjects.Count - 1; i >= 0; i--)
      {
        GameObject currentSelectedGameObject = currentSelectedGameObjects[i];
        if (ReferenceEquals(e.target.gameObject, currentSelectedGameObject))
        {
          //update toggle on selection menu
          CustomGameObject customGameObject = customGameObjects[gameObjectIds[currentSelectedGameObject.name] - 1];
          customGameObject.selected = false;
          customGameObjects[gameObjectIds[currentSelectedGameObject.name] - 1] = customGameObject;
          //remove object outline
          currentSelectedGameObject.GetComponent<Outline>().enabled = false;
          //remove from selected game objects
          currentSelectedGameObjects.RemoveAt(i);
          deselected = true;
          break;
        }
      }
      if (!deselected)
      {
        GameObject newSelectedGameObject = e.target.gameObject;
        //update toggle on selection menu
        CustomGameObject customGameObject = customGameObjects[gameObjectIds[newSelectedGameObject.name] - 1];
        customGameObject.selected = true;
        customGameObjects[gameObjectIds[newSelectedGameObject.name] - 1] = customGameObject;
        //outline object
        newSelectedGameObject.GetComponent<Outline>().enabled = true;
        //add to selected game objects
        currentSelectedGameObjects.Add(newSelectedGameObject);
      }
      listView.UpdateList();
      //update UI and tool info
      if (currentSelectedGameObjects.Count == 0)
      {
        currentSelectedGameObjectName.text = "None";
        hudCurrentSelectedGameObjectName.text = "None";
      }
      else
      {
        currentSelectedGameObjectName.text = currentSelectedGameObjects[0].name;
        hudCurrentSelectedGameObjectName.text = currentSelectedGameObjects[0].name;
      }
    }
    else
    {
      GameObject newSelectedGameObject = e.target.gameObject;
      //update toggle on selection menu
      CustomGameObject customGameObject = customGameObjects[gameObjectIds[newSelectedGameObject.name] - 1];
      customGameObject.selected = true;
      customGameObjects[gameObjectIds[newSelectedGameObject.name] - 1] = customGameObject;
      //outline object
      newSelectedGameObject.GetComponent<Outline>().enabled = true;
      //add to selected game objects
      currentSelectedGameObjects.Add(newSelectedGameObject);
      listView.UpdateList();
      //update UI and tool info
      currentSelectedGameObjectName.text = currentSelectedGameObjects[0].name;
      hudCurrentSelectedGameObjectName.text = currentSelectedGameObjects[0].name;
    }
  }

  public static void GameObjectSelect(GameObject newGameObject)
  {
    if (currentSelectedGameObjects.Count > 0)
    {
      bool deselected = false;
      for (int i = currentSelectedGameObjects.Count - 1; i >= 0; i--)
      {
        GameObject currentSelectedGameObject = currentSelectedGameObjects[i];
        if (ReferenceEquals(newGameObject, currentSelectedGameObject))
        {
          //update toggle on selection menu
          CustomGameObject customGameObject = customGameObjects[gameObjectIds[currentSelectedGameObject.name] - 1];
          customGameObject.selected = false;
          customGameObjects[gameObjectIds[currentSelectedGameObject.name] - 1] = customGameObject;
          //remove object outline
          currentSelectedGameObject.GetComponent<Outline>().enabled = false;
          //remove from selected game objects
          currentSelectedGameObjects.RemoveAt(i);
          deselected = true;
          break;
        }
      }
      if (!deselected)
      {
        //update toggle on selection menu
        CustomGameObject customGameObject = customGameObjects[gameObjectIds[newGameObject.name] - 1];
        customGameObject.selected = true;
        customGameObjects[gameObjectIds[newGameObject.name] - 1] = customGameObject;
        //outline object
        newGameObject.GetComponent<Outline>().enabled = true;
        //add to selected game objects
        currentSelectedGameObjects.Add(newGameObject);
      }
    }
    else
    {
      //update toggle on selection menu
      CustomGameObject customGameObject = customGameObjects[gameObjectIds[newGameObject.name] - 1];
      customGameObject.selected = true;
      customGameObjects[gameObjectIds[newGameObject.name] - 1] = customGameObject;
      //outline object
      newGameObject.GetComponent<Outline>().enabled = true;
      //add to selected game objects
      currentSelectedGameObjects.Add(newGameObject);
    }
  }

  public struct CustomGameObject
  {
    public bool selected;
    public string nickName;
    public int id;
    public string objectName;
    public GameObject gameObject;
  }
}
