using UnityEngine;
using UnityEngine.UI;

public class PopulateSelectedFiles : MonoBehaviour
{
  [SerializeField]
  private RectTransform selectedFilesContainer;
  [SerializeField]
  private Font selectedFilesFont;
  [SerializeField]
  private Text timeStepCount;

  void Start()
  {
    string pathsStrings = PlayerPrefs.GetString("userFilePaths");
    string[] paths = pathsStrings.Remove(pathsStrings.Length - 1).Split(',');
    timeStepCount.text = paths.Length.ToString();

    foreach (string path in paths)
    {
      AddToSelectedFilesContainer(path);
    }
  }

  private void AddToSelectedFilesContainer(string path)
  {
    GameObject selectedFile = new GameObject("SelectedFile");
    selectedFile.transform.SetParent(selectedFilesContainer);
    RectTransform rectTransform = selectedFile.AddComponent<RectTransform>();
    rectTransform.anchorMin = new Vector2(0, 1);
    rectTransform.anchorMax = new Vector2(0, 1);
    rectTransform.pivot = new Vector2(0, 1);
    rectTransform.localPosition = new Vector3(3, -selectedFilesContainer.rect.height, 0);
    rectTransform.localScale = new Vector3(1, 1, 1);
    Text text = selectedFile.AddComponent<Text>();
    text.font = selectedFilesFont;
    text.color = Color.black;
    text.fontSize = 25;
    text.text = path;
    rectTransform.sizeDelta = new Vector2(text.preferredWidth, 0);
    rectTransform.sizeDelta = new Vector2(text.preferredWidth + 7, text.preferredHeight);
    selectedFilesContainer.sizeDelta = new Vector2(Mathf.Max(selectedFilesContainer.rect.width, rectTransform.rect.width), selectedFilesContainer.rect.height + rectTransform.rect.height);
  }
}
