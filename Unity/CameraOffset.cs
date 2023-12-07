using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Top-down camera offset script follows the average position of one or more transforms, with distance and height offsets.
/// </summary>
public class CameraOffset : MonoBehaviour
{
    #region Attributes
    [Header("Target Transforms")]
    [Tooltip("The primary target is the transform that the camera will base its forward vector on.")]
    [SerializeField] Transform primaryTarget;
    [Tooltip("Should the camera stay behind the primary target or the average position?")]
    [SerializeField] bool StayBehindPrimaryTarget;
    [Tooltip("The list of transforms that the camera will average and follow.")]
    public List<Transform> targetTransforms = new();
    [Header("Offset Values")]
    [Tooltip("The minimum distance the camera can be from the target")]
    [SerializeField] float minFollowDist;
    [Tooltip("The minimum height the camera can be")]
    [SerializeField] float minHeight;
    private float currentHeight; // tracking current height of camera
    [Tooltip("The maximum height the camera can be")]
    [SerializeField] float maxHeight;
    [Tooltip("The time it takes for the camera to reach its target position")]
    [SerializeField] float smoothTime;
    private Vector3 velocity = Vector3.zero; // Used for smoothing
    #endregion
    #region Operations
    private void Start()
    {
        if (!targetTransforms.Contains(primaryTarget)) targetTransforms.Add(primaryTarget); // If the target list does not include the primary target, add it
        transform.parent = null; // Unparent the camera from the player. Allows for packaging camera with another prefab
    }
    private void LateUpdate()
    {
        // Calculate the offset position
        Vector3 _avgPos = AvgPos(); // Calculate the average position of all targets
        Vector3 _movePos = _avgPos + (Vector3.up * currentHeight) - (WhereIsForward(_avgPos) * minFollowDist); // Apply offsets
        _movePos.y = Mathf.Clamp(currentHeight, minHeight, maxHeight); // Clamp the height
        // Perform movement and rotation
        transform.SetPositionAndRotation(Vector3.SmoothDamp(transform.position, _movePos, ref velocity, smoothTime), Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_avgPos - transform.position), smoothTime));
    }
    private Vector3 AvgPos() // Calculate the average position of all targets
    {
        Vector3 _avgPosition = Vector3.zero;
        float _greatestDist = 0; // The greatest distance between the primary target and any other target
        foreach (Transform _transform in targetTransforms) // Calculate the average position and the greatest distance
        {
            // Add the position to the average
            _avgPosition += _transform.position;
            // Calculate the greatest distance
            foreach (Transform _otherTransform in targetTransforms)
            {
                float _dist = Vector3.Distance(primaryTarget.position, _otherTransform.position); // Calculate the distance between the primary target and the other target
                if (_dist > _greatestDist) _greatestDist = _dist; // If the distance is greater than the current greatest distance, set the greatest distance to the distance
            }
        }
        currentHeight = _greatestDist; // Set the current height to the greatest distance
        return _avgPosition / targetTransforms.Count; // Return the average position
    }
    private Vector3 WhereIsForward(Vector3 _targetPoint) // Return the forward vector based on the primary target or the average position
    {
        if (StayBehindPrimaryTarget) return primaryTarget.forward; // If the camera should stay behind the primary target, return the primary target's forward vector
        else return _targetPoint; // Otherwise, return the average position
    }
    #endregion
}