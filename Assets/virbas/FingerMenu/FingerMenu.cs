using System;

using UnityEngine;

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

// using Apprentice.Helpers;


namespace Apprentice.Components
{
  public class FingerMenu : MonoBehaviour
  {
    [SerializeField] private Transform thumbTipLight;

    [SerializeField]
    private GameObject buttonsContainer;

    private float flatHandThreshold = 60.0f; // Set to 60 if you want to debug in editor
    private float facingCameraTrackingThreshold = 60.0f;


    //
    //public Handedness trackedHand  {get; public set;} = Handedness.Right;

    [SerializeField]
    private Handedness trackedHand = Handedness.Right;

    public Handedness TrackedHand => trackedHand;

    protected void Update()
    {
      if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, this.trackedHand, out MixedRealityPose palmPose))
      {
        if (IsPalmFacingHead(palmPose))
        {
          EnableInteraction();
          UpdateButtons();
          return;
        }
      }
      DisableInteraction();
    }

    private void EnableInteraction()
    {
      if (!buttonsContainer.gameObject.activeSelf)
      {
        buttonsContainer.SetActive(true);
        //FingerPointer.Enable();
      }
    }

    private void DisableInteraction()
    {
      if (buttonsContainer.gameObject.activeSelf)
      {
        buttonsContainer.SetActive(false);
        // FingerPointer.Disable();
      }
    }

    private void UpdateButtons()
    {
      MixedRealityPose thumbTip;

      if (HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, this.trackedHand, out thumbTip))
      {
        thumbTipLight.position = thumbTip.Position;
      }
    }

    private bool IsPalmFacingHead(MixedRealityPose palmPose)
    {
      float palmCameraAngle = Vector3.Angle(palmPose.Up, Camera.main.transform.forward);

      // Check if the triangle's normal formed from the palm, to index, to ring finger tip roughly matches the palm normal.
      MixedRealityPose indexTipPose, ringTipPose;

      if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, trackedHand, out indexTipPose) &&
          HandJointUtils.TryGetJointPose(TrackedHandJoint.RingTip, trackedHand, out ringTipPose))
      {
        var handNormal = Vector3.Cross(indexTipPose.Position - palmPose.Position,
                                       ringTipPose.Position - indexTipPose.Position).normalized;
        handNormal *= (trackedHand == Handedness.Right) ? 1.0f : -1.0f;


        if (Vector3.Angle(palmPose.Up, handNormal) > flatHandThreshold)
        {
          return false;
        }
      }

      return palmCameraAngle < facingCameraTrackingThreshold;
    }
  }
}
