using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

  private Text buttonText;
  private Color hoverColor = new Color(1f, 0.7333f, 0.3412f);

  private void Start()
  {
    buttonText = GetComponentInChildren<Text>();
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    buttonText.color = hoverColor;
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    buttonText.color = Color.black;
  }
}