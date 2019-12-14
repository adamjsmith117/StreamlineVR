using UnityEngine;

namespace VRScrollView
{
  [RequireComponent(typeof(RectTransform))]
  public class CustomListItem : MonoBehaviour
  {
    public object Tag { get; set; }
    public int Position { get; set; }

    private ICustomListViewAdapter adapter;

    internal void SetAdapter(ICustomListViewAdapter listView)
    {
      this.adapter = listView;
    }
  }
}
