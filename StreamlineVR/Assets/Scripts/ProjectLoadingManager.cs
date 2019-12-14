using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Diagnostics;
using UnityEngine.XR;

public class ProjectLoadingManager : MonoBehaviour
{
  [SerializeField]
  private Text loadingText;
  [SerializeField]
  private Image progressBar;
  [SerializeField]
  private Text progressText;
  [SerializeField]
  private RectTransform activeBar;
  [SerializeField]
  private RectTransform activeBarBG;
  [SerializeField]
  private Material opaque;
  [SerializeField]
  private Material transparent;

  private bool activeBarMovingRight;
  private bool meshRendered;
  private Stopwatch stopWatch;
  private Vector3 totalMeshCenterTransform;
  private int totalObjects;
  private float maxMeshLength;
  private float maxMeshWidth;
  private float maxMeshHeight;
  private Dictionary<string, string> savedColors;
  private Dictionary<string, string> transforms;
  private Dictionary<string, GameObject> timeStepGameObjects;
  private Dictionary<string, GameObject> gameObjects;

  //private readonly int parseUpdateCount = 10000;
  //private readonly int renderUpdateCount = 10000;
  //private readonly int lineSetUpdateCount = 100;

  private void Start()
  {
    //save to playerprefs/project info?
    totalMeshCenterTransform = Vector3.zero;
    totalObjects = 0;
    maxMeshLength = 0;
    maxMeshWidth = 0;
    maxMeshHeight = 0;

    gameObjects = new Dictionary<string, GameObject>();
    timeStepGameObjects = new Dictionary<string, GameObject>();
    activeBarMovingRight = true;
    meshRendered = false;
    stopWatch = new Stopwatch();
    stopWatch.Start();

    // create dictionary of gameobject names to colors
    savedColors = new Dictionary<string, string>();
    string listOfGameObjectColors = PlayerPrefs.GetString("listOfGameObjectColors");
    string[] elements = listOfGameObjectColors.Split(',');

    foreach(string element in elements)
    {
      string entry = element.Trim(' ');
      string[] vals = entry.Split(' ');

      if(vals.Length == 2 && !savedColors.ContainsKey(vals[0])) // *** Second bit of logic is to prevent duplicate timestep info from being added to dictionary when it already exists ***
      {
        savedColors.Add(vals[0], vals[1]);
      }
    }

    // *************** Create dictionary of gameobject names to transform values **********************
    transforms = new Dictionary<string, string>();
    string listOfGameObjectTransforms = PlayerPrefs.GetString("listOfGameObjectTransforms");
    elements = listOfGameObjectTransforms.Split(';');

    char[] symbols = { ' ', '(', ')', ',' };
    foreach(string element in elements)
    {
      string entry = element.Replace("(", "");
      entry = entry.Replace(")", "");
      entry = entry.Replace(",", "");
      string[] vals = entry.Split(' ');

      // Add transform info to dictionary of transforms, make a check to ensure duplicates aren't added like in color case above
      if(vals.Length == 11 && !transforms.ContainsKey(vals[0]))
      {
        string contents = vals[1] + ", " + vals[2] + ", " + vals[3] + ", " + vals[4] + ", " + vals[5] + ", " + vals[6]
        + ", " + vals[7] + ", " + vals[8] + ", " + vals[9] + ", " + vals[10];
        transforms.Add(vals[0], contents);
      }
    }

    StartCoroutine(LoadAndRenderAll());
  }

  private void Update()
  {
    if (activeBarMovingRight)
    {
      if ((activeBar.localPosition.x + (activeBar.rect.width / 2)) >= (activeBarBG.rect.width / 2))
      {
        activeBarMovingRight = false;
        activeBar.Translate(-2, 0, 0);
      }
      else
      {
        activeBar.Translate(2, 0, 0);
      }
    }
    else
    {
      if ((activeBar.localPosition.x - (activeBar.rect.width / 2)) <= -(activeBarBG.rect.width / 2))
      {
        activeBarMovingRight = true;
        activeBar.Translate(2, 0, 0);
      }
      else
      {
        activeBar.Translate(-2, 0, 0);
      }
    }
  }

  /**
   * Reads from the PlayerPref specified binary files, creates gameobjects, renders them, and switches to VR 
   */
  private IEnumerator LoadAndRenderAll()
  {
    //EnableVR();
    AsyncOperation vrScene = SceneManager.LoadSceneAsync(5, LoadSceneMode.Additive);
    //wait for vr scene to finish loading
    while (!vrScene.isDone)
    {
      yield return null;
    }
    SceneManager.SetActiveScene(SceneManager.GetSceneByName("VR"));

    //find the initial time step num once
    string initialTimeStepNum;
    int initTimeStep = PlayerPrefs.GetInt("initTimestep");
    if (initTimeStep == 0)
    {
      initialTimeStepNum = PlayerPrefs.GetInt("firstTimestep").ToString();
    }
    else
    {
      initialTimeStepNum = initTimeStep.ToString();
    }

    //get transform of time step container to make children
    Transform timeStepContainer = GameObject.Find("TimeStepContainer").GetComponent<Transform>();

    //create parent game object for each time step
    string listOfTimeStepNums = PlayerPrefs.GetString("listOfTimeSteps");
    string[] timeStepNums = listOfTimeStepNums.Trim().Split(' ');
    foreach (string timeStepNum in timeStepNums)
    {
      GameObject timeStepComponents = new GameObject("TimeStep" + timeStepNum);
      timeStepComponents.transform.SetParent(timeStepContainer);
      timeStepGameObjects.Add(timeStepComponents.name, timeStepComponents);
    }

    //setup for loading and rendering from binaries
    string listOfBinaryFiles = PlayerPrefs.GetString("listOfBinaryFiles");
    string[] binaryFiles = listOfBinaryFiles.Remove(listOfBinaryFiles.Length - 1).Split(' ');
    int totalFiles = binaryFiles.Length;
    int filesLoadedAndRendered = 0;

    //load and render from each binary
    foreach (string binaryFile in binaryFiles)
    {
      progressBar.fillAmount = (float)filesLoadedAndRendered / (float)totalFiles;
      progressText.text = filesLoadedAndRendered + " / " + totalFiles;
      loadingText.text = "Loading And Rendering From " + binaryFile + "...";
      yield return null;
      Tuple<MeshInfo, string> meshInfo = LoadMeshInfo(binaryFile);
      yield return null;
      while (!meshRendered)
      {
        yield return StartCoroutine(RenderMeshInfo(meshInfo.Item1, meshInfo.Item2));
      }
      filesLoadedAndRendered++;
      meshRendered = false;
    }

    //attempt to center GameObjects
    int scale = PlayerPrefs.GetInt("initScale");
    Vector3 shiftedPosition = totalMeshCenterTransform / totalObjects;
    timeStepContainer.position = -1 * shiftedPosition * scale;
    PlayerPrefs.SetFloat("shiftedX", shiftedPosition.x);
    PlayerPrefs.SetFloat("shiftedY", shiftedPosition.y);
    PlayerPrefs.SetFloat("shiftedZ", shiftedPosition.z);

    PlayerPrefs.SetFloat("MaxMeshLength", maxMeshLength);
    PlayerPrefs.SetFloat("MaxMeshWidth", maxMeshWidth);
    PlayerPrefs.SetFloat("MaxMeshHeight", maxMeshHeight);

    PlaybackManager.SetTimeStepGameObjects(timeStepGameObjects);
    SelectionManager.PopulateGameObjects(gameObjects);
    ScaleManager.SetInitialScale(scale);
    ScaleManager.SetMeshCenter(shiftedPosition);
    TransformationManager.SetMeshCenter(shiftedPosition);
    QueueManager.SetMeshCenter(shiftedPosition);

    SwitchToVRScene();
  }

  private void SwitchToVRScene()
  {
    stopWatch.Stop();
    // Get the elapsed time as a TimeSpan value.
    TimeSpan ts = stopWatch.Elapsed;
    // Format and display the TimeSpan value.
    string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
    UnityEngine.Debug.Log("Total New Project Parsing And Rendering Time: " + elapsedTime);
    SceneManager.UnloadSceneAsync(6);// Switch to VRScene, unload current scene
  }

  private Tuple<MeshInfo, string> LoadMeshInfo(string fileName)
  {
    BinaryFormatter bf = new BinaryFormatter();
    FileStream file = File.Open(Path.Combine(Application.persistentDataPath, "Projects", PlayerPrefs.GetString("projectName"), fileName), FileMode.Open);
    MeshInfo meshInfo = (MeshInfo)bf.Deserialize(file);
    file.Close();
    return Tuple.Create(meshInfo, fileName.Replace(".dat", ""));
  }

  private IEnumerator RenderMeshInfo(MeshInfo meshInfo, string fileName)
  {
    string timeStepSubNum = fileName.Replace(PlayerPrefs.GetString("projectName") + "_timestep", "");
    string timeStepNum = timeStepSubNum.Substring(0, timeStepSubNum.IndexOf('_'));
    Transform timeStepComponentsTransform = timeStepGameObjects["TimeStep" + timeStepNum].transform;
    if (meshInfo.IsLineSet)
    {
      //get coordinates for mesh
      List<SerializableVector3> deserializedCoordinates = meshInfo.Coordinates;
      int coordsCount = deserializedCoordinates.Count;
      Vector3[] coords = new Vector3[coordsCount];
      //yield return null;
      for (int i = 0; i < coordsCount; i++)
      {
        coords[i] = deserializedCoordinates[i];
        //NEW UPDATE
        //if ((i % renderUpdateCount) == 0)
        //{
        //  yield return null;
        //}
      }
      yield return null;

      //map indices for mesh
      List<int> coordIndices = meshInfo.MapCoordinatesToStructure;
      List<int[]> indicesForEachLine = new List<int[]>();
      List<int> lineIndices = new List<int>();
      //int coordIndiceCount = 0;
      //yield return null;
      foreach (int coordIndice in coordIndices)
      {
        if (coordIndice == -1)
        {
          indicesForEachLine.Add(lineIndices.ToArray());
          lineIndices.Clear();
        }
        else
        {
          lineIndices.Add(coordIndice);
        }
        //NEW UPDATE
        //coordIndiceCount++;
        //if ((coordIndiceCount % renderUpdateCount) == 0)
        //{
        //  yield return null;
        //}
      }
      yield return null;

      // Vertex Colors ****************************************************************
      //get colors for mesh
      List<SerializableVector3> deserializedColors = meshInfo.ColorValues;
      int colorsCount = deserializedColors.Count;
      Color[] colors = new Color[colorsCount];
      //yield return null;
      for (int i = 0; i < colorsCount; i++)
      {
        Vector3 rgb = deserializedColors[i];
        colors[i] = new Color(rgb.x, rgb.y, rgb.z);
        //NEW UPDATE
        //if ((i % renderUpdateCount) == 0)
        //{
        //  yield return null;
        //}
      }
      yield return null;


      int lineNum = 0;
      //int indiceGroup = 0;
      foreach (int[] indicesInLine in indicesForEachLine)
      {
        //create mesh to combine
        Mesh myMesh = new Mesh
        {
          //allow mesh variables to hold more data
          indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };

        //assign veritces to mesh
        myMesh.vertices = coords;

        // assign vertex colors to mesh
        myMesh.colors = colors;

        // set material color to the saved color *******************************************************************************************************
        string name = fileName + "LineSet" + lineNum;
        string color = savedColors[name];
        ColorUtility.TryParseHtmlString("#" + color, out Color c);

        //create material for GameObject
        Material myMat;
        if (c.a == 1)
        {
          myMat = new Material(opaque);
        }
        else
        {
          myMat = new Material(transparent);
        }
        myMat.color = c;

        //int indiceCount = indicesInLine.Length;
        //int[] indices = new int[indiceCount];
        //for (int i = 0; i < indiceCount; i++)
        //{
        //  indices[i] = indicesInLine[i];
        //  //NEW UPDATE
        //  //if ((i % lineSetUpdateCount) == 0)
        //  //{
        //  //  yield return null;
        //  //}
        //}

        //assign indices to mesh
        myMesh.SetIndices(indicesInLine, MeshTopology.Lines, 0);

        //track GameObject name
        string listOfGameObjectNames = PlayerPrefs.GetString("listOfGameObjectNames");
        PlayerPrefs.SetString("listOfGameObjectNames", listOfGameObjectNames + fileName + "LineSet" + lineNum + " ");

        //create GameObject
        GameObject myGameObject = new GameObject(fileName + "LineSet" + lineNum);

        //make it a child of its component object
        myGameObject.transform.SetParent(timeStepComponentsTransform);

        //add mesh components
        MeshFilter meshFilter = myGameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = myGameObject.AddComponent<MeshRenderer>();

        //assign material to GameObject
        meshRenderer.material = myMat;

        //assign mesh to GameObject
        meshFilter.mesh = myMesh;

        //add mesh center offset and tally GameObject
        Bounds meshBounds = meshFilter.mesh.bounds;
        Vector3 meshSize = meshBounds.size;
        maxMeshLength = Mathf.Max(maxMeshLength, meshSize.z);
        maxMeshWidth = Mathf.Max(maxMeshWidth, meshSize.x);
        maxMeshHeight = Mathf.Max(maxMeshHeight, meshSize.y);
        totalMeshCenterTransform += meshBounds.center;
        totalObjects++;

        // ******************* Set transformation values ***************************
        string value = transforms[name];
        string[] contents = value.Split(',');
        float scale = float.Parse(contents[0]);

        float posX = float.Parse(contents[3]);
        float posY = float.Parse(contents[4]);
        float posZ = float.Parse(contents[5]);

        float rotX = float.Parse(contents[6]);
        float rotY = float.Parse(contents[7]);
        float rotZ = float.Parse(contents[8]);
        float rotW = float.Parse(contents[9]);

        myGameObject.transform.localScale = new Vector3(scale, scale, scale);
        myGameObject.transform.localPosition = new Vector3(posX, posY, posZ);
        myGameObject.transform.localRotation = new Quaternion(rotX, rotY, rotZ, rotW);

        // Add gameobject to dictionary of all gameobjects
        gameObjects.Add(fileName + "LineSet" + lineNum, myGameObject);

        //add GameObjects to selection scroll view

        lineNum += 1;
        //NEW UPDATE
        //indiceGroup++;
        //if ((indiceGroup % lineSetUpdateCount) == 0)
        //{
        //  yield return null;
        //}
      }
    }
    else
    {
      //get coordinates for meshes
      List<SerializableVector3> deserializedCoordinates = meshInfo.Coordinates;
      int coordsCount = deserializedCoordinates.Count;
      Vector3[] coords = new Vector3[coordsCount];
      //yield return null;
      for (int i = 0; i < coordsCount; i++)
      {
        coords[i] = deserializedCoordinates[i];
        //NEW UPDATE
        //if ((i % renderUpdateCount) == 0)
        //{
        //  yield return null;
        //}
      }
      yield return null;

      //map indices for meshes
      List<int> coordIndices = meshInfo.MapCoordinatesToStructure;
      int indiceCount = coordIndices.Count;
      int[] indices = new int[indiceCount];
      //yield return null;
      for (int i = 0; i < indiceCount; i++)
      {
        indices[i] = coordIndices[i];
        //NEW UPDATE
        //if ((i % renderUpdateCount) == 0)
        //{
        //  yield return null;
        //}
      }
      yield return null;

      //get colors for meshes ***********************************************************
      List<SerializableVector3> deserializedColors = meshInfo.ColorValues;
      int colorCount = deserializedColors.Count;
      Color[] colors = new Color[colorCount];
      //yield return null;
      for (int i = 0; i < colorCount; i++)
      {
        Vector3 rgb = deserializedColors[i];
        colors[i] = new Color(rgb.x, rgb.y, rgb.z);
        //NEW UPDATE
        //if ((i % renderUpdateCount) == 0)
        //{
        //  yield return null;
        //}
      }
      yield return null;

      //create meshes for GameObjects
      Mesh myMeshExterior = new Mesh
      {
        //allow mesh variables to hold more data
        indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
      };
      Mesh myMeshInterior = new Mesh
      {
        //allow mesh variables to hold more data
        indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
      };

      //assign veritces to meshes
      myMeshExterior.vertices = coords;
      myMeshInterior.vertices = coords;

      // assign colors to meshes ******************************************************
      myMeshExterior.colors = colors;
      myMeshInterior.colors = colors;

      //assign indices to meshes
      myMeshExterior.triangles = indices;
      Array.Reverse(indices);
      myMeshInterior.triangles = indices;

      //track GameObject names
      string listOfGameObjectNames = PlayerPrefs.GetString("listOfGameObjectNames");
      PlayerPrefs.SetString("listOfGameObjectNames", listOfGameObjectNames + fileName + "Exterior " + fileName + "Interior ");

      //create GameObjects
      GameObject myGameObjectExterior = new GameObject(fileName + "Exterior");
      GameObject myGameObjectInterior = new GameObject(fileName + "Interior");

      //make them a child of their component object
      myGameObjectExterior.transform.SetParent(timeStepComponentsTransform);
      myGameObjectInterior.transform.SetParent(timeStepComponentsTransform);

      //add mesh components
      MeshFilter meshExteriorFilter = myGameObjectExterior.AddComponent<MeshFilter>();
      MeshRenderer meshExteriorRenderer = myGameObjectExterior.AddComponent<MeshRenderer>();
      MeshCollider meshExteriorCollider = myGameObjectExterior.AddComponent<MeshCollider>();
      MeshFilter meshInteriorFilter = myGameObjectInterior.AddComponent<MeshFilter>();
      MeshRenderer meshInteriorRenderer = myGameObjectInterior.AddComponent<MeshRenderer>();
      MeshCollider meshInteriorCollider = myGameObjectInterior.AddComponent<MeshCollider>();

      //assign material to GameObjects with saved colors *****************************************************************************

      // Exterior
      string name = fileName + "Exterior";
      string color = savedColors[name];
      ColorUtility.TryParseHtmlString("#" + color, out Color exteriorColor);
      Material myMatExterior;
      if (exteriorColor.a == 1)
      {
        myMatExterior = new Material(opaque);
      }
      else
      {
        myMatExterior = new Material(transparent);
      }
      meshExteriorRenderer.material = myMatExterior;
      meshExteriorRenderer.material.color = exteriorColor;

      // Interior
      name = fileName + "Interior";
      color = savedColors[name];
      ColorUtility.TryParseHtmlString("#" + color, out Color interiorColor);
      Material myMatInterior;
      if (interiorColor.a == 1)
      {
        myMatInterior = new Material(opaque);
      }
      else
      {
        myMatInterior = new Material(transparent);
      }
      meshInteriorRenderer.material = myMatInterior;
      meshInteriorRenderer.material.color = interiorColor;

      //assign meshes to GameObjects
      meshExteriorFilter.mesh = myMeshExterior;
      meshInteriorFilter.mesh = myMeshInterior;

      //calculate normals for shading
      meshExteriorFilter.mesh.RecalculateNormals();
      meshInteriorFilter.mesh.RecalculateNormals();

      //add mesh center offsets and tally GameObjects
      Bounds meshExteriorBounds = meshExteriorFilter.mesh.bounds;
      Bounds meshInteriorBounds = meshInteriorFilter.mesh.bounds;
      Vector3 meshExteriorSize = meshExteriorBounds.size;
      Vector3 meshInteriorSize = meshInteriorBounds.size;
      maxMeshLength = Mathf.Max(maxMeshLength, meshExteriorSize.z, meshInteriorSize.z);
      maxMeshWidth = Mathf.Max(maxMeshWidth, meshExteriorSize.x, meshInteriorSize.x);
      maxMeshHeight = Mathf.Max(maxMeshHeight, meshExteriorSize.y, meshInteriorSize.y);
      totalMeshCenterTransform += meshExteriorBounds.center;
      totalMeshCenterTransform += meshInteriorBounds.center;
      totalObjects += 2;

      //assign meshes to GameObject colliders
      meshExteriorCollider.sharedMesh = myMeshExterior;
      meshInteriorCollider.sharedMesh = myMeshInterior;

      // ******************* Set transformation values ***************************
      name = fileName + "Exterior";
      string value = transforms[name];
      string[] contents = value.Split(',');
      float scale = float.Parse(contents[0].Trim(' '));

      float posX = float.Parse(contents[3].Trim(' '));
      float posY = float.Parse(contents[4].Trim(' '));
      float posZ = float.Parse(contents[5].Trim(' '));

      float rotX = float.Parse(contents[6].Trim(' '));
      float rotY = float.Parse(contents[7].Trim(' '));
      float rotZ = float.Parse(contents[8].Trim(' '));
      float rotW = float.Parse(contents[9].Trim(' '));

      myGameObjectExterior.transform.localScale = new Vector3(scale, scale, scale);
      myGameObjectExterior.transform.localPosition = new Vector3(posX, posY, posZ);
      myGameObjectExterior.transform.localRotation = new Quaternion(rotX, rotY, rotZ, rotW);


      name = fileName + "Interior";
      value = transforms[name];
      contents = value.Split(',');
      scale = float.Parse(contents[0]);

      posX = float.Parse(contents[3]);
      posY = float.Parse(contents[4]);
      posZ = float.Parse(contents[5]);

      rotX = float.Parse(contents[6]);
      rotY = float.Parse(contents[7]);
      rotZ = float.Parse(contents[8]);
      rotW = float.Parse(contents[9]);

      myGameObjectInterior.transform.localScale = new Vector3(scale, scale, scale);
      myGameObjectInterior.transform.localPosition = new Vector3(posX, posY, posZ);
      myGameObjectInterior.transform.localRotation = new Quaternion(rotX, rotY, rotZ, rotW);


      //TODO: ASSIGN WIDTH BASED ON SCALE
      //add outline component to GameObjects
      Outline outlineExterior = myGameObjectExterior.AddComponent<Outline>();
      outlineExterior.OutlineMode = Outline.Mode.OutlineAll;
      outlineExterior.OutlineColor = new Color(1f, 0.7333333f, 0.3411765f, 1f);
      outlineExterior.OutlineWidth = 10f;
      outlineExterior.enabled = false;
      Outline outlineInterior = myGameObjectInterior.AddComponent<Outline>();
      outlineInterior.OutlineMode = Outline.Mode.OutlineAll;
      outlineInterior.OutlineColor = new Color(1f, 0.7333333f, 0.3411765f, 1f);
      outlineInterior.OutlineWidth = 10f;
      outlineInterior.enabled = false;

      // Add gameobject to dictionary of all timesteps
      gameObjects.Add(fileName + "Exterior", myGameObjectExterior);
      gameObjects.Add(fileName + "Interior", myGameObjectInterior);

      //add GameObjects to selection scroll view

      //USER CHOICE?
      //optimizes the mesh for GPU access
      //MeshUtility.Optimize(myMesh);
    }
    meshRendered = true;
  }

  //private IEnumerator LoadDevice(string newDevice, bool enable)
  //{
  //  XRSettings.LoadDeviceByName(newDevice);
  //  yield return null;
  //  XRSettings.enabled = enable;
  //}

  //private void EnableVR()
  //{
  //  StartCoroutine(LoadDevice("OpenVR", true));
  //}
}
