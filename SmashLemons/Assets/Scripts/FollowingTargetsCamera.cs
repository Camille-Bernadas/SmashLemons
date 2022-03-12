using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class FollowingTargetsCamera : MonoBehaviour
{
    public List<Transform> targets;
    public Transform bottomLeft;
    public Transform topRight;
    public Vector3 offset = new Vector3(0f, 1f, -6f);
    public float smoothTime = 0.25f;
    
    public float maxZoom = -3f;
    public float minZoom = -12f;
    public float zoomLimiter = 30f;

    private Vector3 velocity;
    private Camera cam;
    private void Start() {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if(targets.Count == 0){
            return;
        }
        Move();
        Zoom();

    }
    void Move(){
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }
    void Zoom(){
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        offset = new Vector3(offset.x, offset.y, newZoom);

    }

    float GetGreatestDistance(){
        if (targets.Count == 1) {
            return 0f;
        }
        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++) {
            float xValue = Mathf.Max(bottomLeft.position.x, Mathf.Min(targets[i].position.x, topRight.position.x));
            float yValue = Mathf.Max(bottomLeft.position.y, Mathf.Min(targets[i].position.y, topRight.position.y));
        
            Vector3 position = new Vector3(xValue, yValue, 0f);
            bounds.Encapsulate(position);
        }
        return bounds.size.x;
    }

    Vector3 GetCenterPoint(){
        if(targets.Count == 1){
            return targets[0].position;
        }
        float xBase = Mathf.Max(bottomLeft.position.x, Mathf.Min(targets[0].position.x, topRight.position.x));
        float yBase = Mathf.Max(bottomLeft.position.y, Mathf.Min(targets[0].position.y, topRight.position.y));

        Vector3 basePosition = new Vector3(xBase, yBase, 0f);
        Bounds bounds = new Bounds(basePosition, Vector3.zero);
        for (int i = 0; i < targets.Count; i++){
            float xValue = Mathf.Max(bottomLeft.position.x, Mathf.Min(targets[i].position.x, topRight.position.x));
            float yValue = Mathf.Max(bottomLeft.position.y, Mathf.Min(targets[i].position.y, topRight.position.y));

            Vector3 position = new Vector3(xValue, yValue, 0f);
            bounds.Encapsulate(position);
        }
        return bounds.center;
    }
}
