using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Top-down camera offset script follows the average position of one or more transforms, with a minimum distance and height offset.
/// </summary>
public class CameraOffset : MonoBehaviour
{
    #region Attributes
    [Header("Target Transforms")]
    [Tooltip("The primary target is the transform that the camera will base its forward vector on.")]
    [SerializeField] Transform primaryTarget;
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
    Vector3 velocity = Vector3.zero; // Used for smoothing
    #endregion
    #region Operations
    private void Start()
    {
        if (!targetTransforms.Contains(primaryTarget)) targetTransforms.Add(primaryTarget); // If the target list does not include the primary target, add it
        transform.parent = null; // Unparent the camera from the player. Allows for packaging camera with another prefab
    }
    private void LateUpdate()
    {
        Vector3 _avgPos = AvgPos();
        // Calculate the target position and offset and clamp the height
        Vector3 _targetPos = _avgPos + (Vector3.up * minHeight) - (primaryTarget.forward * minFollowDist);
        _targetPos.y = Mathf.Clamp(currentHeight, minHeight, maxHeight);
        // Perform movement and rotation
        transform.SetPositionAndRotation(Vector3.SmoothDamp(transform.position, _targetPos, ref velocity, smoothTime), Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_avgPos - transform.position), smoothTime));
    }
    private Vector3 AvgPos()
    {
        Vector3 _avgPosition = Vector3.zero; // The average position of all targets
        float _greatestDist = 0; // The greatest distance between the primary target and any other target
        foreach (Transform _transform in targetTransforms)
        {
            _avgPosition += _transform.position;
            foreach (Transform _otherTransform in targetTransforms)
            {
                float _dist = Vector3.Distance(primaryTarget.position, _otherTransform.position);
                if (_dist > _greatestDist) _greatestDist = _dist;
            }
        }
        currentHeight = _greatestDist;
        return _avgPosition / targetTransforms.Count;
    }
    #endregion
}