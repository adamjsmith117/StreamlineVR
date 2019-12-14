using UnityEngine;
using UnityEngine.UI;

namespace VRScrollView
{
  public class GameObjectInfoItem : CustomListItem
  {
    #region Variables
    [SerializeField]
    private Toggle toggle;
    [SerializeField]
    private Text nickName;
    [SerializeField]
    private Text id;
    [SerializeField]
    private Text objectName;
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
    public void SetInfo(bool toggle, string name, int id, string objectName)
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

      this.objectName.text = objectName;
      RectTransform objectNameRectTransform = this.objectName.gameObject.GetComponent<RectTransform>();
      objectNameRectTransform.localPosition = new Vector3(infoWidthSum, 0, 0);
      objectNameRectTransform.sizeDelta = new Vector2(this.objectName.preferredWidth, infoHeight);
      infoWidthSum += objectNameRectTransform.rect.width + spacing;

      ((RectTransform)transform).sizeDelta = new Vector2(infoWidthSum, infoHeight);
    }
    #endregion

    private void UpdateSelected(bool selected, int id)
    {
      SelectionManager.CustomGameObject customGameObject = SelectionManager.customGameObjects[(id - 1)];
      Outline outlineComponent = customGameObject.gameObject.GetComponent<Outline>();
      if (selected)
      {
        if (outlineComponent)
        {
          outlineComponent.enabled = true;
        }
        //add to current selected objects
        SelectionManager.currentSelectedGameObjects.Add(customGameObject.gameObject);
      }
      else
      {
        if (outlineComponent)
        {
          outlineComponent.enabled = false;
        }
        //remove from current selected objects
        SelectionManager.currentSelectedGameObjects.Remove(customGameObject.gameObject);
      }
      customGameObject.selected = selected;
      SelectionManager.customGameObjects[(id - 1)] = customGameObject;
      SelectionManager.toggleUpdated = true;
      //DEBUGGING
      //******************************************************************************************************
      //SelectionManager.CustomGameObject gameObject2 = SelectionManager.customGameObjects[(id - 1)];
      //Debug.Log("Should Change To: " + selected);
      //Debug.Log("ID: " + id);
      //Debug.Log("Changed To: " + gameObject2.selected);
    }
  }
}