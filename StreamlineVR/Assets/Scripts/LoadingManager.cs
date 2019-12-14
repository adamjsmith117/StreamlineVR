using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class LoadingManager : MonoBehaviour
{
  [SerializeField]
  private Text loadingText;
  [SerializeField]
  private Image progressBar;
  [SerializeField]
  private RectTransform activeBarBG;
  [SerializeField]
  private Text progressText;
  [SerializeField]
  private RectTransform activeBar;
  [SerializeField]
  private Material opaque;
  [SerializeField]
  private GameObject selectToggle;
  [SerializeField]
  private Font gameObjectInfoFont;

  private string projectFolderPath;
  private bool activeBarMovingRight;
  private bool fileParsed;
  private bool meshRendered;
  private Stopwatch stopWatch;
  private Vector3 totalMeshCenterTransform;
  private int totalObjects;
  private float maxMeshLength;
  private float maxMeshWidth;
  private float maxMeshHeight;
  private Dictionary<string, GameObject> timeStepGameObjects;
  private Dictionary<string, GameObject> gameObjects;

  //private readonly int parseUpdateCount = 10000;
  //private readonly int renderUpdateCount = 10000;
  //private readonly int lineSetUpdateCount = 100;

  private void Start()
  {
    projectFolderPath = "";
    gameObjects = new Dictionary<string, GameObject>();
    timeStepGameObjects = new Dictionary<string, GameObject>();
    totalMeshCenterTransform = Vector3.zero;
    totalObjects = 0;
    maxMeshLength = 0;
    maxMeshWidth = 0;
    maxMeshHeight = 0;
    activeBarMovingRight = true;
    fileParsed = false;
    meshRendered = false;
    stopWatch = new Stopwatch();
    stopWatch.Start();
    StartCoroutine(ParseAllFiles());
  }

  private IEnumerator ParseAllFiles()
  {
    projectFolderPath = Path.Combine(Application.persistentDataPath, "Projects", PlayerPrefs.GetString("projectName"));
    if (Directory.Exists(projectFolderPath))
    {
      Directory.Delete(projectFolderPath, true);
    }
    Directory.CreateDirectory(projectFolderPath);

    string pathsStrings = PlayerPrefs.GetString("userFilePaths");
    string[] paths = pathsStrings.Remove(pathsStrings.Length - 1).Split(',');
    int totalFiles = paths.Length;
    int filesParsed = 0;

    foreach (string path in paths)
    {
      //unpack file name
      string fileName = Path.GetFileNameWithoutExtension(path);

      progressBar.fillAmount = (float)filesParsed / (float)totalFiles;
      progressText.text = filesParsed + " / " + totalFiles;
      loadingText.text = "Parsing " + fileName + "...";
			yield return null;
      //run on multiple threads?
      while (!fileParsed)
      {
        yield return StartCoroutine(ParseFile(path));
      }
      filesParsed++;
      fileParsed = false;
    }

    StartCoroutine(LoadAndRenderAll());
  }

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
    SceneManager.UnloadSceneAsync(4);
  }

  /// <summary>
  /// Given a filename, reads and saves relevant info
  /// </summary>
  /// <param name="filePath"></param>
  private IEnumerator ParseFile(string filePath)
  {
    //UnityEngine.Debug.Log("File Path: " + filePath);
    //unpack file name
    string fileName = Path.GetFileNameWithoutExtension(filePath);
    //UnityEngine.Debug.Log("File Name: " + fileName);

    if (File.Exists(filePath))
    {
      StreamReader inputStream = new StreamReader(filePath);
      int meshNum = 0;
      string line;
      //int lineCounter = 0;
      while ((line = inputStream.ReadLine()) != null)
      {
        //uncomment after reading in colors
        //NEW UPDATE
        //lineCounter++;
        //if (lineCounter == parseUpdateCount)
        //{
        //  lineCounter = 0;
        //  yield return null;
        //}
        if (line.Contains("<Shape>"))
        {
          //***************************************
          //UnityEngine.Debug.Log("Found the starting shape tag");
          //***************************************
          bool isLineSet = false;

          //UnityEngine.Debug.Log("Attempting to create mesh " + meshNum + " binary");
          MeshInfo meshInfo = new MeshInfo();
          List<SerializableVector3> coordinates = new List<SerializableVector3>();
          List<SerializableVector3> colorValues = new List<SerializableVector3>();
          List<int> mapCoordinatesToStructure = new List<int>();
          List<int> mapColorValuesToCoordinates = new List<int>();

          //debugging
          //*************************************************
          //Dictionary<int, int> coordDictionary = new Dictionary<int, int>();
          ////Dictionary<int, int> colorDictionary = new Dictionary<int, int>();
          //int numCoords = 0;
          ////int numColors = 0;
          //int numCoordIndiceGroups = 0;
          ////int numColorIndiceGroups = 0;
          //int numCoordIndices = 0;
          ////int numColorIndices = 0;
          //int numCoordSets = 0;
          //*************************************************

          bool hasVertColors = false;

          while ((line = inputStream.ReadLine()) != null)
          {
            yield return null;
            if (line.Contains("<IndexedFaceSet"))
            {
              if(line.Contains("colorPerVertex=\"true\""))
              {
                hasVertColors = true;
              }
              else if (line.Contains("colorPerVertex=\"false\""))
              {
                hasVertColors = false;
              }
              else
              {
                UnityEngine.Debug.Log("No colorPerVertex tag");
              }
              //***************************************
              //UnityEngine.Debug.Log("Found the start of indexed face set");
              //***************************************
              //FINDING THE COORD INDICES
              //*************************************************
              while (!line.Contains("coordIndex=\""))
              {
                line = inputStream.ReadLine();
                yield return null;
              }
              //***************************************
              //UnityEngine.Debug.Log("Found the start of coord indices");
              //***************************************
              if (!line.Contains("-1"))//coordIndex data is on different line
              {
                line = inputStream.ReadLine();
                //lineCounter = 0;
                yield return null;
                while (!line.Contains("\""))
                {
                  string[] coordIndices = line.Trim().Split(' ');
                  foreach (string coordIndex in coordIndices)
                  {
                    mapCoordinatesToStructure.Add(int.Parse(coordIndex));
                  }
                  line = inputStream.ReadLine();
                  //NEW UPDATE
                  //lineCounter++;
                  //if (lineCounter == parseUpdateCount)
                  //{
                  //  lineCounter = 0;
                  //  yield return null;
                  //}
                  //debugging
                  //*************************************************
                  //int coordIndicesInLine = (coordIndices.Length - 1);
                  //numCoordIndices += coordIndicesInLine;
                  //numCoordIndiceGroups += 1;
                  //if (coordDictionary.ContainsKey(coordIndicesInLine))
                  //{
                  //  coordDictionary[coordIndicesInLine] += 1;
                  //}
                  //else
                  //{
                  //  coordDictionary[coordIndicesInLine] = 1;
                  //}
                  //*************************************************
                }
              }
              else//coordIndex data is on same line
              {
                int firstDoubleQuote = line.IndexOf("\"", StringComparison.Ordinal);
                int lastDoubleQuote = line.LastIndexOf("\"", StringComparison.Ordinal);
                string coordIndicesString = line.Substring(firstDoubleQuote + 1, lastDoubleQuote - (firstDoubleQuote + 1));
                string[] coordIndices = coordIndicesString.Trim().Split(' ');
                //debugging
                //*************************************************
                //int count = 0;
                //*************************************************
                foreach (string coordIndex in coordIndices)
                {
                  //debugging
                  //*************************************************
                  //if (!coordIndex.Equals("-1"))
                  //{
                  //  count++;
                  //}
                  //else
                  //{
                  //  int coordIndicesInLine = count;
                  //  numCoordIndices += coordIndicesInLine;
                  //  numCoordIndiceGroups += 1;
                  //  if (coordDictionary.ContainsKey(coordIndicesInLine))
                  //  {
                  //    coordDictionary[coordIndicesInLine] += 1;
                  //  }
                  //  else
                  //  {
                  //    coordDictionary[coordIndicesInLine] = 1;
                  //  }
                  //  count = 0;
                  //}
                  //*************************************************
                  mapCoordinatesToStructure.Add(int.Parse(coordIndex));
                }
              }
              //***************************************
              //UnityEngine.Debug.Log("Found the end of coord indices");
              //***************************************
              //*************************************************
              //FINDING THE COORDINATES
              //*************************************************
              while (!line.Contains("<Coordinate"))
              {
                line = inputStream.ReadLine();
                yield return null;
              }
              //***************************************
              //UnityEngine.Debug.Log("Found the start of coordinates");
              //***************************************
              if (!line.Contains("point=\""))//coord data on next immediate line
              {
                line = inputStream.ReadLine();
                yield return null;
                int firstDoubleQuote = line.IndexOf("\"", StringComparison.Ordinal);
                int lastDoubleQuote = line.LastIndexOf("\"", StringComparison.Ordinal);
                string coordsString = line.Substring(firstDoubleQuote + 1, lastDoubleQuote - (firstDoubleQuote + 1));
                string[] coords = coordsString.Trim().Split(' ');
                int coordsNum = coords.Length;
                for (int i = 0; i < coordsNum; i += 3)
                {
                  float x = (float)Convert.ToDouble(coords[i]);
                  float y = (float)Convert.ToDouble(coords[i + 1]);
                  float z = (float)Convert.ToDouble(coords[i + 2]);

                  coordinates.Add(new SerializableVector3(x, y, z));
                  //debugging
                  //*************************************************
                  //numCoords += 3;
                  //numCoordSets += 1;
                  //*************************************************
                }
              }
              else//coord data is on next LINES
              {
                line = inputStream.ReadLine();
                //lineCounter = 0;
                yield return null;
                while (!line.Contains("\""))
                {
                  string[] coordXYZ = line.Trim().Replace(",", "").Split(' ');

                  float x = (float)Convert.ToDouble(coordXYZ[0]);
                  float y = (float)Convert.ToDouble(coordXYZ[1]);
                  float z = (float)Convert.ToDouble(coordXYZ[2]);

                  coordinates.Add(new SerializableVector3(x, y, z));
                  line = inputStream.ReadLine();
                  //NEW UPDATE
                  //lineCounter++;
                  //if (lineCounter == parseUpdateCount)
                  //{
                  //  lineCounter = 0;
                  //  yield return null;
                  //}
                  //debugging
                  //*************************************************
                  //numCoords += 3;
                  //numCoordSets += 1;
                  //*************************************************
                }
              }
              //***************************************
              //UnityEngine.Debug.Log("Found the end of coordinates");
              //***************************************
              //*************************************************
              //finished face set
              //***************************************
              //UnityEngine.Debug.Log("Found the end of indexed face set");
              //***************************************
              //debugging
              //*************************************************
              //UnityEngine.Debug.Log("End of mesh");
              //UnityEngine.Debug.Log("Printing info for mesh " + meshNum);
              //UnityEngine.Debug.Log("Coord sets in mesh: " + numCoordSets);
              //UnityEngine.Debug.Log("Coords in mesh: " + numCoords);
              //Debug.Log("Colors in mesh: " + numColors);
              //UnityEngine.Debug.Log("Coord indices in mesh: " + numCoordIndices);
              //UnityEngine.Debug.Log("Coord indice groups in mesh: " + numCoordIndiceGroups);
              //string coordDictionaryOutput = "";
              //foreach (KeyValuePair<int, int> keyValuePair in coordDictionary)
              //{
              //  coordDictionaryOutput += string.Format("Coord Index Group Size = {0}, Count = {1}\n", keyValuePair.Key, keyValuePair.Value);
              //}
              //UnityEngine.Debug.Log(coordDictionaryOutput);
              //Debug.Log("Color indices in mesh: " + numColorIndices);
              //Debug.Log("Color indice groups in mesh: " + numColorIndiceGroups);
              //string colorDictionaryOutput = "";
              //foreach (KeyValuePair<int, int> keyValuePair in colorDictionary)
              //{
              //  colorDictionaryOutput += string.Format("Color Index Group Size = {0}, Count = {1}\n", keyValuePair.Key, keyValuePair.Value);
              //}
              //Debug.Log(colorDictionaryOutput);
              //*************************************************

              //********** COLOR *********************
              if(hasVertColors)
              {
                while (!line.Contains("<Color"))
                {
                  line = inputStream.ReadLine();
                }
                line = inputStream.ReadLine();
                while (!line.Contains("\""))
                {
                  string[] coordRGB = line.Trim().Replace(",", "").Split(' ');

                  float r = (float)Convert.ToDouble(coordRGB[0]);
                  float g = (float)Convert.ToDouble(coordRGB[1]);
                  float b = (float)Convert.ToDouble(coordRGB[2]);

                  colorValues.Add(new SerializableVector3(r, g, b));

                  line = inputStream.ReadLine();
                }
              }

              //UnityEngine.Debug.Log("Triangulator length of coordIndices prior: " + mapCoordinatesToStructure.Count);
              mapCoordinatesToStructure = Triangulator.TriangulateMesh(coordinates, mapCoordinatesToStructure);
              //UnityEngine.Debug.Log("Triangulator length of coordIndices after: " + mapCoordinatesToStructure.Count);

              //if not using triangulator
              //mapCoordinatesToStructure.RemoveAll(num => num == -1);

              meshInfo.MapCoordinatesToStructure = mapCoordinatesToStructure;
              meshInfo.Coordinates = coordinates;
              meshInfo.ColorValues = colorValues;
              //meshInfo.MapColorValuesToCoordinates = mapColorValuesToCoordinates; // Original color mapping, not set anywhere in code yet
              meshInfo.MapColorValuesToCoordinates = mapCoordinatesToStructure;
              meshInfo.IsLineSet = isLineSet;

              // now save mesh to binary file
              BinaryFormatter bf = new BinaryFormatter();
              string binaryFileName = PlayerPrefs.GetString("projectName") + "_" + fileName + "_" + meshNum + ".dat";

              //track binary file name
              string listOfBinaryFiles = PlayerPrefs.GetString("listOfBinaryFiles");
              PlayerPrefs.SetString("listOfBinaryFiles", listOfBinaryFiles + binaryFileName + " ");
              FileStream binaryFile = File.Create(Path.Combine(projectFolderPath, binaryFileName));

              bf.Serialize(binaryFile, meshInfo);
              binaryFile.Close();
              //UnityEngine.Debug.Log("Saved mesh " + meshNum + "!");
              meshNum += 1;
              break;
            }
            else if (line.Contains("<IndexedLineSet"))
            {
              if (line.Contains("colorPerVertex=\"true\""))
              {
                hasVertColors = true;
              }
              else if (line.Contains("colorPerVertex=\"false\""))
              {
                hasVertColors = false;
              }
              else
              {
                UnityEngine.Debug.Log("No colorPerVertex tag");
              }
              //***************************************
              //UnityEngine.Debug.Log("Found the start of indexed line set");
              //***************************************
              isLineSet = true;
              line = inputStream.ReadLine();
              //lineCounter = 0;
              yield return null;
              //***************************************
              //UnityEngine.Debug.Log("Found the start of coord indices");
              //***************************************
              while (!line.Contains("\""))
              {
                string[] coordIndices = line.Trim().Split(' ');
                foreach (string coordIndice in coordIndices)
                {
                  mapCoordinatesToStructure.Add(int.Parse(coordIndice));
                }
                line = inputStream.ReadLine();
                //NEW UPDATE
                //lineCounter++;
                //if (lineCounter == parseUpdateCount)
                //{
                //  lineCounter = 0;
                //  yield return null;
                //}
                //debugging
                //*************************************************
                //int coordIndicesInLine = (coordIndices.Length - 1);
                //numCoordIndices += coordIndicesInLine;
                //numCoordIndiceGroups += 1;
                //if (coordDictionary.ContainsKey(coordIndicesInLine))
                //{
                //  coordDictionary[coordIndicesInLine] += 1;
                //}
                //else
                //{
                //  coordDictionary[coordIndicesInLine] = 1;
                //}
                //*************************************************
              }
              //***************************************
              //UnityEngine.Debug.Log("Found the end of coord indices");
              //***************************************
              while (!line.Contains("<Coordinate"))
              {
                line = inputStream.ReadLine();
                yield return null;
              }
              //***************************************
              //UnityEngine.Debug.Log("Found the start of coordinates");
              //***************************************
              line = inputStream.ReadLine();
              //lineCounter = 0;
              yield return null;
              while (!line.Contains("\""))
              {
                string[] coordXYZ = line.Trim().Replace(",", "").Split(' ');

                float x = (float)Convert.ToDouble(coordXYZ[0]);
                float y = (float)Convert.ToDouble(coordXYZ[1]);
                float z = (float)Convert.ToDouble(coordXYZ[2]);

                coordinates.Add(new SerializableVector3(x, y, z));
                line = inputStream.ReadLine();
                //NEW UPDATE
                //lineCounter++;
                //if (lineCounter == parseUpdateCount)
                //{
                //  lineCounter = 0;
                //  yield return null;
                //}
                //debugging
                //*************************************************
                //numCoords += 3;
                //numCoordSets += 1;
                //*************************************************
              }
              //***************************************
              //UnityEngine.Debug.Log("Found the end of coordinates");
              //***************************************
              //finished line set
              //***************************************
              //UnityEngine.Debug.Log("Found the end of indexed line set");
              //***************************************
              //debugging
              //*************************************************
              //UnityEngine.Debug.Log("End of mesh");
              //UnityEngine.Debug.Log("Printing info for mesh " + meshNum);
              //UnityEngine.Debug.Log("Coord sets in mesh: " + numCoordSets);
              //UnityEngine.Debug.Log("Coords in mesh: " + numCoords);
              //Debug.Log("Colors in mesh: " + numColors);
              //UnityEngine.Debug.Log("Coord indices in mesh: " + numCoordIndices);
              //UnityEngine.Debug.Log("Coord indice groups in mesh: " + numCoordIndiceGroups);
              //string coordDictionaryOutput = "";
              //foreach (KeyValuePair<int, int> keyValuePair in coordDictionary)
              //{
              //  coordDictionaryOutput += string.Format("Coord Index Group Size = {0}, Count = {1}\n", keyValuePair.Key, keyValuePair.Value);
              //}
              //UnityEngine.Debug.Log(coordDictionaryOutput);
              //Debug.Log("Color indices in mesh: " + numColorIndices);
              //Debug.Log("Color indice groups in mesh: " + numColorIndiceGroups);
              //string colorDictionaryOutput = "";
              //foreach (KeyValuePair<int, int> keyValuePair in colorDictionary)
              //{
              //  colorDictionaryOutput += string.Format("Color Index Group Size = {0}, Count = {1}\n", keyValuePair.Key, keyValuePair.Value);
              //}
              //Debug.Log(colorDictionaryOutput);
              //*************************************************

              // **************** LINESET COLORS ***************************
             if(hasVertColors)
              {
                while (!line.Contains("<Color"))
                {
                  line = inputStream.ReadLine();
                }
                line = inputStream.ReadLine();
                while (!line.Contains("\""))
                {
                  string[] coordRGB = line.Trim().Replace(",", "").Split(' ');

                  float r = (float)Convert.ToDouble(coordRGB[0]);
                  float g = (float)Convert.ToDouble(coordRGB[1]);
                  float b = (float)Convert.ToDouble(coordRGB[2]);

                  colorValues.Add(new SerializableVector3(r, g, b));

                  line = inputStream.ReadLine();
                }
              }

              meshInfo.MapCoordinatesToStructure = mapCoordinatesToStructure;
              meshInfo.Coordinates = coordinates;
              meshInfo.ColorValues = colorValues;
              meshInfo.MapColorValuesToCoordinates = mapColorValuesToCoordinates;
              meshInfo.IsLineSet = isLineSet;

              // now save mesh to binary file
              BinaryFormatter bf = new BinaryFormatter();
              string binaryFileName = PlayerPrefs.GetString("projectName") + "_" + fileName + "_" + meshNum + ".dat";

              //track binary file name
              string listOfBinaryFiles = PlayerPrefs.GetString("listOfBinaryFiles");
              PlayerPrefs.SetString("listOfBinaryFiles", listOfBinaryFiles + binaryFileName + " ");
              FileStream binaryFile = File.Create(Path.Combine(projectFolderPath, binaryFileName));

              bf.Serialize(binaryFile, meshInfo);
              binaryFile.Close();
              //UnityEngine.Debug.Log("Saved mesh " + meshNum + "!");
              meshNum += 1;
              break;
            }
          }
        }
      }
      inputStream.Dispose();
      fileParsed = true;
    }
    else
    {
      //bad path
      UnityEngine.Debug.Log("A bad path was provided to the parser.");
    }
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
  
  //binary formatter async?

  private Tuple<MeshInfo, string> LoadMeshInfo(string fileName)
  {
    string binaryFilePath = Path.Combine(projectFolderPath, fileName);
    if (File.Exists(binaryFilePath))
    {
      BinaryFormatter bf = new BinaryFormatter();
      FileStream file = File.Open(binaryFilePath, FileMode.Open);
      MeshInfo meshInfo = (MeshInfo)bf.Deserialize(file);
      file.Close();
      return Tuple.Create(meshInfo, fileName.Replace(".dat", ""));
    }
    else
    {
      UnityEngine.Debug.Log("Binary file does not exist");
      return null;
    }
  }

  private IEnumerator RenderMeshInfo(MeshInfo meshInfo, string fileName)
  {
    string timeStepSubNum = fileName.Replace(PlayerPrefs.GetString("projectName") + "_timestep", "");
    string timeStepNum = timeStepSubNum.Substring(0,timeStepSubNum.IndexOf('_'));
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
        myMesh.colors = colors; // ************************************* Setting lineset mesh colors ***************************

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

        //create material for GameObject
        Material myMat = new Material(opaque);

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

        //scale GameObject
        int scale = PlayerPrefs.GetInt("initScale");
        myGameObject.transform.localScale = new Vector3(scale, scale, scale);

        // Add gameobject to dictionary of all gameobjects
        gameObjects.Add(fileName + "LineSet" + lineNum, myGameObject);

        //add GameObject to selection scroll view
        //RectTransform gameObjectContainer = GameObject.Find("Player/SteamVRObjects/RightHand/HandCanvas/Selection_pan/GameObjects/GameObjectViewport/GameObjectContainer").GetComponent<RectTransform>();

        //GameObject gameObjectInfo = new GameObject("GameObjectInfo");
        //gameObjectInfo.transform.SetParent(gameObjectContainer);
        //RectTransform infoRectTransform = gameObjectInfo.AddComponent<RectTransform>();
        //infoRectTransform.anchorMin = new Vector2(0, 1);
        //infoRectTransform.anchorMax = new Vector2(0, 1);
        //infoRectTransform.pivot = new Vector2(0, 1);
        //infoRectTransform.localPosition = new Vector3(0, -gameObjectContainer.rect.height, 0);
        //infoRectTransform.localScale = new Vector3(1, 1, 1);

        //GameObject gameObjectSelectToggle = Instantiate(selectToggle);
        //gameObjectSelectToggle.transform.SetParent(gameObjectInfo.transform);
        //RectTransform selectToggleRectTransform = gameObjectSelectToggle.GetComponent<RectTransform>();
        //selectToggleRectTransform.localPosition = new Vector3(10f, -10f, 0);

        //float infoWidthSum = selectToggleRectTransform.rect.width + 20f;
        //float infoHeight = selectToggleRectTransform.rect.height + 20f;

        //GameObject gameObjectName = new GameObject("GameObjectName");
        //gameObjectName.transform.SetParent(gameObjectInfo.transform);
        //RectTransform nameRectTransform = gameObjectName.AddComponent<RectTransform>();
        //nameRectTransform.anchorMin = new Vector2(0, 1);
        //nameRectTransform.anchorMax = new Vector2(0, 1);
        //nameRectTransform.pivot = new Vector2(0, 1);
        //nameRectTransform.localPosition = new Vector3(infoWidthSum, 0, 0);
        //nameRectTransform.localScale = new Vector3(1, 1, 1);
        //Text nameText = gameObjectName.AddComponent<Text>();
        //nameText.font = gameObjectInfoFont;
        //nameText.color = Color.black;
        //nameText.alignment = TextAnchor.MiddleLeft;
        //nameText.text = fileName + "LineSet" + lineNum;
        //nameRectTransform.sizeDelta = new Vector2(nameText.preferredWidth, infoHeight);
        //infoWidthSum += nameRectTransform.rect.width + 10f;

        //infoRectTransform.sizeDelta = new Vector2(infoWidthSum, infoHeight);
        //infoRectTransform.localRotation = Quaternion.Euler(0, 0, 0);

        //gameObjectContainer.sizeDelta = new Vector2(Mathf.Max(gameObjectContainer.rect.width, infoWidthSum), gameObjectContainer.rect.height + infoHeight);

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

      //create material for GameObjects
      Material myExteriorMat = new Material(opaque);
      Material myInteriorMat = new Material(opaque);


      //assign material to GameObjects
      meshExteriorRenderer.material = myExteriorMat;
      meshInteriorRenderer.material = myInteriorMat;

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

      //scale GameObjects
      int scale = PlayerPrefs.GetInt("initScale");
      myGameObjectExterior.transform.localScale = new Vector3(scale, scale, scale);
      myGameObjectInterior.transform.localScale = new Vector3(scale, scale, scale);

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
      //RectTransform gameObjectContainer = GameObject.Find("Player/SteamVRObjects/RightHand/HandCanvas/Selection_pan/GameObjects/GameObjectViewport/GameObjectContainer").GetComponent<RectTransform>();

      //GameObject exteriorGameObjectInfo = new GameObject("GameObjectInfo");
      //exteriorGameObjectInfo.transform.SetParent(gameObjectContainer);
      //RectTransform exteriorInfoRectTransform = exteriorGameObjectInfo.AddComponent<RectTransform>();
      //exteriorInfoRectTransform.anchorMin = new Vector2(0, 1);
      //exteriorInfoRectTransform.anchorMax = new Vector2(0, 1);
      //exteriorInfoRectTransform.pivot = new Vector2(0, 1);
      //exteriorInfoRectTransform.localPosition = new Vector3(0, -gameObjectContainer.rect.height, 0);
      //exteriorInfoRectTransform.localScale = new Vector3(1, 1, 1);

      //GameObject exteriorGameObjectSelectToggle = Instantiate(selectToggle);
      //exteriorGameObjectSelectToggle.transform.SetParent(exteriorGameObjectInfo.transform);
      //RectTransform exteriorSelectToggleRectTransform = exteriorGameObjectSelectToggle.GetComponent<RectTransform>();
      //exteriorSelectToggleRectTransform.localPosition = new Vector3(10f, -10f, 0);

      //float exteriorInfoWidthSum = exteriorSelectToggleRectTransform.rect.width + 20f;
      //float exteriorInfoHeight = exteriorSelectToggleRectTransform.rect.height + 20f;

      //GameObject exteriorGameObjectName = new GameObject("GameObjectName");
      //exteriorGameObjectName.transform.SetParent(exteriorGameObjectInfo.transform);
      //RectTransform exteriorNameRectTransform = exteriorGameObjectName.AddComponent<RectTransform>();
      //exteriorNameRectTransform.anchorMin = new Vector2(0, 1);
      //exteriorNameRectTransform.anchorMax = new Vector2(0, 1);
      //exteriorNameRectTransform.pivot = new Vector2(0, 1);
      //exteriorNameRectTransform.localPosition = new Vector3(exteriorInfoWidthSum, 0, 0);
      //exteriorNameRectTransform.localScale = new Vector3(1, 1, 1);
      //Text exteriorNameText = exteriorGameObjectName.AddComponent<Text>();
      //exteriorNameText.font = gameObjectInfoFont;
      //exteriorNameText.color = Color.black;
      //exteriorNameText.alignment = TextAnchor.MiddleLeft;
      //exteriorNameText.text = fileName + "Exterior";
      //exteriorNameRectTransform.sizeDelta = new Vector2(exteriorNameText.preferredWidth, exteriorInfoHeight);
      //exteriorInfoWidthSum += exteriorNameRectTransform.rect.width + 10f;

      //exteriorInfoRectTransform.sizeDelta = new Vector2(exteriorInfoWidthSum, exteriorInfoHeight);
      //exteriorInfoRectTransform.localRotation = Quaternion.Euler(0, 0, 0);

      //gameObjectContainer.sizeDelta = new Vector2(Mathf.Max(gameObjectContainer.rect.width, exteriorInfoWidthSum), gameObjectContainer.rect.height + exteriorInfoHeight);

      //GameObject interiorGameObjectInfo = new GameObject("GameObjectInfo");
      //interiorGameObjectInfo.transform.SetParent(gameObjectContainer);
      //RectTransform interiorInfoRectTransform = interiorGameObjectInfo.AddComponent<RectTransform>();
      //interiorInfoRectTransform.anchorMin = new Vector2(0, 1);
      //interiorInfoRectTransform.anchorMax = new Vector2(0, 1);
      //interiorInfoRectTransform.pivot = new Vector2(0, 1);
      //interiorInfoRectTransform.localPosition = new Vector3(0, -gameObjectContainer.rect.height, 0);
      //interiorInfoRectTransform.localScale = new Vector3(1, 1, 1);

      //GameObject interiorGameObjectSelectToggle = Instantiate(selectToggle);
      //interiorGameObjectSelectToggle.transform.SetParent(interiorGameObjectInfo.transform);
      //RectTransform interiorSelectToggleRectTransform = interiorGameObjectSelectToggle.GetComponent<RectTransform>();
      //interiorSelectToggleRectTransform.localPosition = new Vector3(10f, -10f, 0);

      //float interiorInfoWidthSum = interiorSelectToggleRectTransform.rect.width + 20f;
      //float interiorInfoHeight = interiorSelectToggleRectTransform.rect.height + 20f;

      //GameObject interiorGameObjectName = new GameObject("GameObjectName");
      //interiorGameObjectName.transform.SetParent(interiorGameObjectInfo.transform);
      //RectTransform interiorNameRectTransform = interiorGameObjectName.AddComponent<RectTransform>();
      //interiorNameRectTransform.anchorMin = new Vector2(0, 1);
      //interiorNameRectTransform.anchorMax = new Vector2(0, 1);
      //interiorNameRectTransform.pivot = new Vector2(0, 1);
      //interiorNameRectTransform.localPosition = new Vector3(interiorInfoWidthSum, 0, 0);
      //interiorNameRectTransform.localScale = new Vector3(1, 1, 1);
      //Text interiorNameText = interiorGameObjectName.AddComponent<Text>();
      //interiorNameText.font = gameObjectInfoFont;
      //interiorNameText.color = Color.black;
      //interiorNameText.alignment = TextAnchor.MiddleLeft;
      //interiorNameText.text = fileName + "Interior";
      //interiorNameRectTransform.sizeDelta = new Vector2(interiorNameText.preferredWidth, interiorInfoHeight);
      //interiorInfoWidthSum += interiorNameRectTransform.rect.width + 10f;

      //interiorInfoRectTransform.sizeDelta = new Vector2(interiorInfoWidthSum, interiorInfoHeight);
      //interiorInfoRectTransform.localRotation = Quaternion.Euler(0, 0, 0);

      //gameObjectContainer.sizeDelta = new Vector2(Mathf.Max(gameObjectContainer.rect.width, interiorInfoWidthSum), gameObjectContainer.rect.height + interiorInfoHeight);

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
