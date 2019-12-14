using System.Collections.Generic;
using UnityEngine;

public class Triangulator : MonoBehaviour
{
  static private List<SerializableVector3> Coords; // coordinate values referenced by coordIndices
  static private List<int> originalCoordIndices;   // coordInidces of provided mesh
  static private List<int> modifiedCoordIndices;   // the new list of trinagulated coordIndices to be returned

  /** TriangulateMesh
   * 
   *  Summary: 
   *    Loops over all points in the mesh made from coords and coordIndices and converts all non-trinagle faces to triangles
   *    
   *  Parameters:
   *    List<SerializableVector3> coords - coordinate values referenced by coordIndices
   *    List<int> coordIndices - list of coord indices that defines the shapes making up the of mesh
   *    
   *  Returns:
   *    List<int> - modified list of coordIndices, now triangulated
   **/
  public static List<int> TriangulateMesh(List<SerializableVector3> coords, List<int> coordIndices)
  {
    Coords = coords; // save passed in coordinate info into our global field Coords
    originalCoordIndices = coordIndices;
    modifiedCoordIndices = new List<int>(); // initialize the list we will be returning

    List<int> currentPolygon = new List<int>();
    for (int idx = 0; idx < coordIndices.Count; idx++) // go over every polygon in the provided mesh
    {
      if (coordIndices[idx] != -1) // add non negative values to current polygon
      {
        currentPolygon.Add(coordIndices[idx]);
      }
      else
      {
        if (currentPolygon.Count > 3) // triangulate non-triangle faces
        {
          TriangluatePolygon(currentPolygon);
        }
        else // add the unmodified triangle to the updated list
        {
          for (int i = 0; i < currentPolygon.Count; i++)
          {
            modifiedCoordIndices.Add(currentPolygon[i]);
          }
        }

        currentPolygon.Clear(); // clear this list to read the next polygon in the mesh
      }
    }

    return modifiedCoordIndices; // return the triangulated mesh info
  }

  /** Triangulate Polygon
   * 
   * Summary:
   *  Takes a list of coord indices specifying a polygon (a single face) to be triangulated
   * 
   * Parameters:
   *  List<int> polygonIndices - list of coord indices making up a single face of the mesh
   *  
   * Effects:
   *  Updates the global modifiedCoordIndices field with indices of the triangulated polygon
   **/
  private static void TriangluatePolygon(List<int> polygonIndices)
  {
    while (polygonIndices.Count > 3)
    {
      List<int> earVerts = FindEar(polygonIndices); // find an ear

      foreach (int vert in earVerts)   // add the ear to the new trinagle list  
      {
        modifiedCoordIndices.Add(vert);
      }

      polygonIndices.Remove(earVerts[1]); // remove the middle earVert from the polygon
    }

    foreach (int vert in polygonIndices) // make a triangle from the remaining vertices
    {
      modifiedCoordIndices.Add(vert);
    }
  }

  /** FindEar
   * 
   *  Summary:
   *    Finds an ear in the provided polygon and returns the three vertices that make up the ear
   * 
   *  Parameter:
   *    List<int> polygonIndices - list of indices that make up the polygon being triangulated
   *  
   *  Returns:
   *    The vertices that make up the ear that was found
   **/
  private static List<int> FindEar(List<int> polygonIndices)
  {
    int i = 1; // start at 1 since we need to index i-1 in first iteration
    bool earNotFound = true;
    List<int> triangle = new List<int>();

    while (earNotFound) // while we haven't made a new triangle
    {
      triangle = new List<int> // see if this is a suitable ear to be clipped
      {
        polygonIndices[i - 1],
        polygonIndices[i],
        polygonIndices[i + 1]
      };

      if (IsConvex(triangle))
      {
        earNotFound = false; // this is a suitable ear, flag that we are done
      }
      if (earNotFound) // if this wasn't a suitable ear, iterate onto the next set of three points
      {
        i++;
      }
    }

    return triangle; // return the ear that is suitable for clipping
  }

  /** IsConvex
   * 
   *  Summary:
   *    Helper method to determine if the polygon passed in is convex
   * 
   *  Parameter:
   *    List<int> tri - triangle being evaluated
   *  
   *  Returns:
   *    bool - true if tri is convex, false otherwise
   **/
  private static bool IsConvex(List<int> tri)
  {
    bool got_negative = false;
    bool got_positive = false;
    int num_points = tri.Count;
    int B, C;
    for(int A = 0; A < num_points; A++)
    {
      B = (A + 1) % num_points;
      C = (B + 1) % num_points;

      float cross_product =
          CrossProductLength(
              Coords[tri[A]].x, Coords[tri[A]].y,
              Coords[tri[B]].x, Coords[tri[B]].y,
              Coords[tri[C]].x, Coords[tri[C]].y);
      if(cross_product < 0)
      {
        got_negative = true;
      }
      else if(cross_product > 0)
      {
        got_positive = true;
      }
      if(got_negative && got_positive)
        return false;
    }

    // If we got this far, the polygon is convex.
    return true;
  }

  /** CrossProductLength
   *  
   *  Summary:
   *    Calculates the length of the cross product between the two vectors created by points A,B, and C
   *    
   *  Parameters:
   *    Ax, Ay - the x & y coords of point A
   *    Bx, By - the x & y coords of point B (the shared point of the two resulting vectors)
   *    Cx, Cy - the x & y coords of point C
   */
  private static float CrossProductLength( float Ax, float Ay,
    float Bx, float By, float Cx, float Cy )
  {
    // Get the vectors' coordinates.
    float BAx = Ax - Bx;
    float BAy = Ay - By;
    float BCx = Cx - Bx;
    float BCy = Cy - By;

    // Calculate the Z coordinate of the cross product.
    return (BAx * BCy - BAy * BCx);
  }
}
