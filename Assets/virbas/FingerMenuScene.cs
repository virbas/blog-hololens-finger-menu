using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

using Apprentice.Components;

//  

public class FingerMenuScene : MonoBehaviour
{
  public void SpawnObjectFromFinger(ThumbButton sender )
  {
    var handness = sender.GetComponentInParent<FingerMenu>().TrackedHand;
    if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, handness, out MixedRealityPose palmPose))
    {
      var clone = GameObject.Instantiate(sender.transform.Find("Icon"));
      clone.transform.position = palmPose.Position;
      clone.localScale = Vector3.one / 4.0f;

      clone.transform.LookAt(Camera.main.transform);
      clone.transform.Rotate(Vector3.up, 180, Space.Self);
    }
  }
}
