using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

namespace VRScrollView
{
  public class WaypointInfoItem : CustomListItem
  {
    #region Variables
    [SerializeField]
    private Toggle toggle;
    [SerializeField]
    private Text nickName;
    [SerializeField]
    private Text id;
    [SerializeField]
    private Text coords;
    [SerializeField]
    private Image color;
    [SerializeField]
    private Button teleport;
    #endregion

    #region Properties
    private RectTransform m_transform;
    public RectTransform TransformComponent
    {
      get
      {
        if (m_transform == null)
          m_transform = (RectTransform)transform;

        return m_transform;
      }
    }
    #endregion

    #region Initialization Functions
    public void SetInfo(bool toggle, string name, int id, Vector3 coords, Color color, Transform player)
    {
      float infoWidthSum = 40f;
      float infoHeight = 40f;
      float spacing = 10f;

      this.toggle.onValueChanged.RemoveAllListeners();
      this.toggle.isOn = toggle;
      this.toggle.onValueChanged.AddListener(delegate { UpdateSelected(this.toggle.isOn, id); });

      nickName.text = name;
      RectTransform nameRectTransform = nickName.gameObject.GetComponent<RectTransform>();
      nameRectTransform.localPosition = new Vector3(infoWidthSum, 0, 0);
      nameRectTransform.sizeDelta = new Vector2(nickName.preferredWidth, infoHeight);
      infoWidthSum += nameRectTransform.rect.width + spacing;

      this.id.text = id.ToString();
      RectTransform idRectTransform = this.id.gameObject.GetComponent<RectTransform>();
      idRectTransform.localPosition = new Vector3(infoWidthSum, 0, 0);
      idRectTransform.sizeDelta = new Vector2(this.id.preferredWidth, infoHeight);
      infoWidthSum += idRectTransform.rect.width + spacing;

      this.coords.text = coords.ToString();
      RectTransform coordsRectTransform = this.coords.gameObject.GetComponent<RectTransform>();
      coordsRectTransform.localPosition = new Vector3(infoWidthSum, 0, 0);
      coordsRectTransform.sizeDelta = new Vector2(this.coords.preferredWidth, infoHeight);
      infoWidthSum += coordsRectTransform.rect.width + spacing;

      this.color.color = color;
      RectTransform colorRectTransform = this.color.gameObject.GetComponent<RectTransform>();
      colorRectTransform.localPosition = new Vector3(infoWidthSum, 0, 0);
      infoWidthSum += colorRectTransform.rect.width + spacing;

      ColorBlock colorBlock = this.teleport.colors;
      colorBlock.highlightedColor = color;
      this.teleport.colors = colorBlock;
      RectTransform teleportRectTransform = this.teleport.gameObject.GetComponent<RectTransform>();
      teleportRectTransform.localPosition = new Vector3(infoWidthSum, -spacing, 0);
      UIElement teleportUIElement = this.teleport.gameObject.GetComponent<UIElement>();
      teleportUIElement.onHandClick = new CustomEvents.UnityEventHand();
      teleportUIElement.onHandClick.AddListener(delegate { TeleportPlayer(player, coords); });
      infoWidthSum += teleportRectTransform.rect.width + spacing;

      ((RectTransform)transform).sizeDelta = new Vector2(infoWidthSum, infoHeight);
    }
    #endregion

    private void TeleportPlayer(Transform player, Vector3 coordinates)
    {
      player.position = coordinates;
      //Debug.Log("Teleport Used");
    }

    private void UpdateSelected(bool selected, int id)
    {
      WaypointManager.Waypoint waypoint = WaypointManager.waypoints[(id - 1)];
      waypoint.selected = selected;
      WaypointManager.waypoints[(id - 1)] = waypoint;
      //WaypointManager.Waypoint waypoint2 = WaypointManager.waypoints[(id - 1)];
      //Debug.Log("Should Change To: " + selected);
      //Debug.Log("ID: " + id);
      //Debug.Log("Changed To: " + waypoint2.selected);
    }
  }
}