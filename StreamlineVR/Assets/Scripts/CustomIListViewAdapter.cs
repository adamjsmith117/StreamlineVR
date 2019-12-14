namespace VRScrollView
{
  public interface ICustomListViewAdapter
  {
    int Count { get; }
    float ItemHeight { get; }

    CustomListItem CreateItem();

    void SetItemContent(CustomListItem item);
  }
}
