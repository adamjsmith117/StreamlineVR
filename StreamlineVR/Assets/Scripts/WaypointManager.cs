using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;
using VRScrollView;

public class WaypointManager : MonoBehaviour, ICustomListViewAdapter
{
  [SerializeField]
  private Transform player;
  [SerializeField]
  private Transform waypointParent;
  [SerializeField]
  private RectTransform waypointContainer;
  [SerializeField]
  private SteamVR_LaserPointer laserPointer;
  [SerializeField]
  private GameObject laserCube;
  [SerializeField]
  private GameObject laserModeDropdown;
  [SerializeField]
  private GameObject wayPanel;
  [SerializeField]
  private GameObject menuPanel;
  [SerializeField]
  private WaypointInfoItem waypointInfoPrefab;
  [SerializeField]
  private CustomRecycledListView listView;

  private bool useLaserPointer;
  private int laserMode;
  public static List<Waypoint> waypoints;
  private readonly Color32[] colors = { new Color32(255, 0, 0, 255), new Color32(255, 165, 0, 255), new Color32(255, 255, 0, 255), new Color32(0, 128, 0, 255), new Color32(0, 0, 255, 255), new Color32(75, 0, 130, 255), new Color32(238, 130, 238, 255), new Color32(255, 105, 180, 255), new Color32(244, 164, 96, 255), new Color32(112, 128, 144, 255) };

  private void Start()
  {
    ItemHeight = 40f;
    waypoints = new List<Waypoint>();
    useLaserPointer = true;
    laserMode = 0;
    listView.SetAdapter(this);
    PopulateWaypoints();
  }

  public void EnterMenu()
  {
    menuPanel.SetActive(false);
    wayPanel.SetActive(true);
    if (useLaserPointer)
    {
      EnableLaser(true);
      laserModeDropdown.SetActive(true);
      if (laserMode == 0)
      {
        laserPointer.PointerClick += CreateNewWaypointUsingLaser;
      }
      else if (laserMode == 1)
      {
        laserPointer.PointerClick += DeleteExistingWaypointUsingLaser;
      }
    }
  }

  public void ExitMenu()
  {
    if (useLaserPointer)
    {
      if (laserMode == 0)
      {
        laserPointer.PointerClick -= CreateNewWaypointUsingLaser;
      }
      else if (laserMode == 1)
      {
        laserPointer.PointerClick -= DeleteExistingWaypointUsingLaser;
      }
      laserModeDropdown.SetActive(false);
      EnableLaser(false);
    }
    wayPanel.SetActive(false);
    menuPanel.SetActive(true);
    SaveWaypoints();
  }

  #region Interface Methods
  public int Count { get { return waypoints.Count; } }
  public float ItemHeight { get; private set; }

  public CustomListItem CreateItem()
  {
    WaypointInfoItem item = (WaypointInfoItem)Instantiate(waypointInfoPrefab, waypointContainer, false);
    return item;
  }

  public void SetItemContent(CustomListItem item)
  {
    WaypointInfoItem waypoint = (WaypointInfoItem)item;
    Waypoint waypointInfo = waypoints[item.Position];

    waypoint.SetInfo(waypointInfo.selected, waypointInfo.nickName, waypointInfo.id, waypointInfo.coordinates, waypointInfo.color, player);
  }
  #endregion

  private void EnableLaser(bool state)
  {
    laserPointer.enabled = state;
    laserCube.SetActive(state);
  }

  public void UseLaserPointerChanged(Toggle useLaserPointerToggle)
  {
    useLaserPointer = useLaserPointerToggle.isOn;
    if (useLaserPointer)
    {
      //show laser and mode selector
      EnableLaser(true);
      laserModeDropdown.SetActive(true);
      //sub event based off current laser mode
      if (laserMode == 0)
      {
        laserPointer.PointerClick += CreateNewWaypointUsingLaser;
      }
      else if (laserMode == 1)
      {
        laserPointer.PointerClick += DeleteExistingWaypointUsingLaser;
      }
    }
    else
    {
      //hide laser and mode selector. unsub events
      EnableLaser(false);
      laserModeDropdown.SetActive(false);
      //unsub event based off current laser mode
      if (laserMode == 0)
      {
        laserPointer.PointerClick -= CreateNewWaypointUsingLaser;
      }
      else if (laserMode == 1)
      {
        laserPointer.PointerClick -= DeleteExistingWaypointUsingLaser;
      }
    }
  }

  public void LaserModeChanged()
  {
    //switch subbed events based off new laser mode
    laserMode = laserModeDropdown.GetComponent<Dropdown>().value;
    if (laserMode == 0)
    {
      laserPointer.PointerClick -= DeleteExistingWaypointUsingLaser;
      laserPointer.PointerClick += CreateNewWaypointUsingLaser;
    }
    else if (laserMode == 1)
    {
      laserPointer.PointerClick -= CreateNewWaypointUsingLaser;
      laserPointer.PointerClick += DeleteExistingWaypointUsingLaser;
    }
  }

  public void CreateNewWaypoint()
  {
    //should only be called while not using laser pointer
    Vector3 coordinates = player.position;
    string nickName = "TestNickName";
    int id = waypoints.Count + 1;
    Color32 color = colors[Random.Range(0, colors.Length - 1)];
    CreateWaypoint(false, nickName, id, coordinates, color);
  }

  private void CreateNewWaypointUsingLaser(object sender, PointerEventArgs e)
  {
    float distToObject = e.distance;
    if (distToObject < 100f)
    {
      Ray raycast = new Ray(laserPointer.transform.position, laserPointer.transform.forward);
      Vector3 coordinates = raycast.GetPoint(distToObject - 2);
      string nickName = "TestNickName";
      int id = waypoints.Count + 1;
      Color32 color = colors[Random.Range(0, colors.Length - 1)];
      CreateWaypoint(false, nickName, id, coordinates, color);
    }
  }

  private void CreateWaypoint(bool selected, string nickName, int id, Vector3 coordinates, Color32 color)
  {
    GameObject waypoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    waypoint.name = "waypoint" + id;
    waypoint.transform.SetParent(waypointParent);
    waypoint.transform.position = coordinates;
    waypoint.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
    waypoint.GetComponent<MeshRenderer>().material.SetColor("_Color", color);

    Waypoint newWaypoint = new Waypoint
    {
      selected = selected,
      nickName = nickName,
      id = id,
      coordinates = coordinates,
      color = color,
      waypoint = waypoint
    };
    waypoints.Add(newWaypoint);

    listView.UpdateList();
  }

  public void DeleteExistingWaypoint()
  {
    for (int i = waypoints.Count - 1; i >= 0; i--)
    {
      if (waypoints[i].selected)
      {
        TryToDeleteWaypoint(waypoints[i].waypoint);
      }
    }
    listView.UpdateList();
  }

  private void DeleteExistingWaypointUsingLaser(object sender, PointerEventArgs e)
  {
    TryToDeleteWaypoint(e.target.gameObject);
    listView.UpdateList();
  }

  private void TryToDeleteWaypoint(GameObject gameObjectToTry)
  {
    string currentGameObjectName = gameObjectToTry.name;
    Regex correctWaypointName = new Regex(@"waypoint\d+");
    if (correctWaypointName.IsMatch(currentGameObjectName))
    {
      int index = int.Parse(currentGameObjectName.Replace("waypoint", "")) - 1;

      //remove game object from scene
      Destroy(gameObjectToTry);

      waypoints.RemoveAt(index);
      //update other waypoint ids
      for (int i = index; i < waypoints.Count; i++)
      {
        int id = i + 1;
        waypoints[i].waypoint.name = "waypoint" + id.ToString();
        Waypoint updatedWaypoint = waypoints[i];
        updatedWaypoint.id = id;
        waypoints[i] = updatedWaypoint;
      }
    }
  }

  private void PopulateWaypoints()
  {
    string waypointInfo = PlayerPrefs.GetString("waypointInfo");
    if (waypointInfo.Length != 0)
    {
      string[] waypointsArray = waypointInfo.Remove(waypointInfo.Length - 1).Split(',');
      foreach (string singleWaypointInfo in waypointsArray)
      {
        string[] singleWaypointInfoArray = singleWaypointInfo.Split(' ');
        bool selected = bool.Parse(singleWaypointInfoArray[0]);
        string nickName = singleWaypointInfoArray[1];
        int id = int.Parse(singleWaypointInfoArray[2]);
        Vector3 coordinates = new Vector3(float.Parse(singleWaypointInfoArray[3]), float.Parse(singleWaypointInfoArray[4]), float.Parse(singleWaypointInfoArray[5]));
        Color32 color = new Color32(byte.Parse(singleWaypointInfoArray[6]), byte.Parse(singleWaypointInfoArray[7]), byte.Parse(singleWaypointInfoArray[8]), byte.Parse(singleWaypointInfoArray[9]));
        CreateWaypoint(selected, nickName, id, coordinates, color);
      }
    }
  }

  private void SaveWaypoints()
  {
    string waypointInfo = "";
    foreach (Waypoint waypoint in waypoints)
    {
      Vector3 coordinates = waypoint.coordinates;
      Color32 color = waypoint.color;
      waypointInfo += waypoint.selected + " " + waypoint.nickName + " " + waypoint.id.ToString() + " " + coordinates.x.ToString() + " " + coordinates.y.ToString() + " " + coordinates.z.ToString() + " " + color.r.ToString() + " " + color.g.ToString() + " " + color.b.ToString() + " " + color.a.ToString() + ",";
    }
    PlayerPrefs.SetString("waypointInfo", waypointInfo);
  }

  public struct Waypoint
  {
    public bool selected;
    public string nickName;
    public int id;
    public Vector3 coordinates;
    public Color color;
    public GameObject waypoint;
  }
}
