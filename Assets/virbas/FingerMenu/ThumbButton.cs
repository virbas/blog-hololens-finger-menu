using System.Collections;

using UnityEngine;

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Apprentice.Components
{
  public class ThumbButton : MonoBehaviour
  {
    [SerializeField] UnityEngine.Events.UnityEvent onClick;
    
    [SerializeField] TrackedHandJoint trackedJoint;
    [SerializeField] float clickThreshold = 1f;

#if UNITY_EDITOR // for testing in the editor. Set it to 0.1 o trigger index action as soon as the hand is visible
    private float thumbClickDistanceThreshold = 0.1f;
#else
      private float thumbClickDistanceThreshold = 0.03f;
#endif

    private enum State
    {
      NONE,
      TOUCHED,
      RELEASING,
    }

    private State state = State.NONE;

    // Coroutine to trigger State after clickThreshold passed
    private Coroutine pressCoroutine;

    private Handedness trackedHand = Handedness.Right;

    void Start()
    {
      trackedHand = GetComponentInParent<FingerMenu>().TrackedHand;
    }

    // Update is called once per frame
    void Update()
    {
      bool found = false;
      MixedRealityPose trackedPose, thumbTip;

      // check if tracked joint is available for the tracked hand
      if (HandJointUtils.TryGetJointPose(trackedJoint, this.trackedHand, out trackedPose))
      {
        transform.position = trackedPose.Position;
        transform.rotation = trackedPose.Rotation;
        transform.Rotate(Vector3.right, 90);

        // Check if thumb tip is available for the tracked hand
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, this.trackedHand, out thumbTip))
        {
          found = true;

          // check if thumbtip is touching the tracked joint
          if (Vector3.Distance(thumbTip.Position, transform.position) < thumbClickDistanceThreshold)
          {
            if (state == State.NONE)
            {
              if (pressCoroutine != null)
              {
                StopCoroutine(pressCoroutine);
              }
              pressCoroutine = StartCoroutine(BeginPress());
            }
          }
          else
          {
            if (state == State.TOUCHED)
            {
              if (pressCoroutine != null)
              {
                StopCoroutine(pressCoroutine);
              }
              pressCoroutine = StartCoroutine(BeginRelease());
            }
          }
        }
      }

      if (!found)
      {
        if (pressCoroutine != null)
        {
          StopCoroutine(pressCoroutine);
        }
        state = State.NONE;
      }
    }

    private IEnumerator BeginPress()
    {
      state = State.TOUCHED;
      yield return new WaitForSeconds(clickThreshold);
      RiseClicked();
    }

    private IEnumerator BeginRelease()
    {
      state = State.RELEASING;
      yield return new WaitForSeconds(clickThreshold);
      state = State.NONE;
    }

    private void RiseClicked()
    {
      onClick.Invoke();
    }

    void OnDisable()
    {
      if (pressCoroutine != null)
      {
        StopCoroutine(pressCoroutine);
        pressCoroutine = null;
        state = State.NONE;
      }
    }
  }
}