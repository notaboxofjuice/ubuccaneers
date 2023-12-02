using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Camera offset script follows the average position of one or more transforms, with a minimum distance and height offset.
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
    private void FixedUpdate()
    {
        // Calculate the target position and offset and clamp the height
        Vector3 targetPos = avgPos() + (Vector3.up * minHeight) - (primaryTarget.forward * minFollowDist);
        targetPos.y = Mathf.Clamp(targetPos.y, minHeight, maxHeight);
        // Perform movement and rotation
        transform.SetPositionAndRotation(Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime), Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(primaryTarget.position - transform.position), smoothTime));
    }
    private Vector3 avgPos()
    {
        Vector3 _avgPosition = Vector3.zero;
        foreach (Transform _transform in targetTransforms)
        {
            _avgPosition += _transform.position;
        }
        return _avgPosition / targetTransforms.Count;
    }
#endregion
}