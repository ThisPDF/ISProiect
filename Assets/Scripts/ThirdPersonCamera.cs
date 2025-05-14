using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    
    [Header("Camera Settings")]
    public float distance = 5.0f;
    public float height = 2.0f;
    public float smoothSpeed = 10.0f;
    
    [Header("Camera Constraints")]
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;
    public float minHeight = 1.0f;
    public float maxHeight = 5.0f;
    
    [Header("Camera Collision")]
    public bool enableCollision = true;
    public float collisionRadius = 0.3f;
    public LayerMask collisionLayers;
    
    private Vector3 targetPosition;
    private float currentDistance;
    
    void Start()
    {
        // Find player if target is not assigned
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogError("No player found for camera to follow!");
            }
        }
        
        currentDistance = distance;
        
        // Position the camera initially
        if (target != null)
        {
            transform.position = CalculateCameraPosition();
            transform.LookAt(target.position + Vector3.up * height * 0.5f);
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Calculate the desired camera position
        targetPosition = CalculateCameraPosition();
        
        // Handle camera collision
        if (enableCollision)
        {
            HandleCameraCollision();
        }
        
        // Smoothly move the camera
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        
        // Look at the target
        transform.LookAt(target.position + Vector3.up * height * 0.5f);
    }
    
    Vector3 CalculateCameraPosition()
    {
        // Calculate the position behind the target
        Vector3 desiredPosition = target.position - target.forward * distance + Vector3.up * height;
        return desiredPosition;
    }
    
    void HandleCameraCollision()
    {
        // Cast a ray from the target to the desired camera position
        RaycastHit hit;
        Vector3 direction = targetPosition - (target.position + Vector3.up * height * 0.5f);
        float targetDistance = direction.magnitude;
        
        if (Physics.SphereCast(target.position + Vector3.up * height * 0.5f, collisionRadius, direction.normalized, out hit, targetDistance, collisionLayers))
        {
            // If there's an obstacle, adjust the camera distance
            float distanceToObstacle = hit.distance;
            currentDistance = Mathf.Clamp(distanceToObstacle, minDistance, distance);
            
            // Recalculate the target position with the new distance
            targetPosition = target.position - target.forward * currentDistance + Vector3.up * height;
        }
        else
        {
            // No obstacle, gradually return to the desired distance
            currentDistance = Mathf.Lerp(currentDistance, distance, Time.deltaTime * smoothSpeed);
        }
    }
    
    // Allow zooming with mouse wheel
    public void AdjustDistance(float scrollInput)
    {
        distance = Mathf.Clamp(distance - scrollInput, minDistance, maxDistance);
    }
    
    // Draw camera visualization in the editor
    void OnDrawGizmosSelected()
    {
        if (target == null) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(target.position + Vector3.up * height * 0.5f, CalculateCameraPosition());
        Gizmos.DrawWireSphere(CalculateCameraPosition(), collisionRadius);
    }
}