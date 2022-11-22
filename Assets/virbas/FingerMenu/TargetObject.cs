using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Apprentice.Components;
public class TargetObject : MonoBehaviour
{
  public void ChangeColor(ThumbButton button)
  {
    var material = button.transform.Find("Geometry").GetComponent<MeshRenderer>().material;
    Debug.Log("Changing color " + material.color);
    GetComponent<MeshRenderer>().material.color = material.color;
  }

  public void ChangeGeometry(ThumbButton button)
  {
    var mesh = button.transform.Find("Geometry").GetComponent<MeshFilter>().mesh;
    GetComponent<MeshFilter>().mesh = mesh;
  }

  public void ChangeGeometry(Mesh mesh)
  {
    GetComponent<MeshFilter>().mesh = mesh;
  }
}
