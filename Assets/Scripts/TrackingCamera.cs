using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// camera that keeps all targets on screen and zooms to fit
[RequireComponent(typeof(Camera))]
public class TrackingCamera : MonoBehaviour
{
    Camera cam;
    public List<Transform> targets;
    float desiredOrtho;
    Vector2 desiredPos;
    public float zoomSmoothing = 1;
    public float panSmoothing = 1;
    public float padding = 4;
    public float minOrtho = 5;
    public float maxOrtho = 7.5f;
    float zoomVelocity;
    Vector2 panVelocity;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (targets.Count == 0)
            return;
        desiredOrtho = GetDesiredOrtho();
        desiredPos = GetDesiredPosition();
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, desiredOrtho, ref zoomVelocity, zoomSmoothing);
        Vector2 newPos = Vector2.SmoothDamp(transform.position, desiredPos, ref panVelocity, panSmoothing);
        cam.transform.position = new Vector3(newPos.x, newPos.y, cam.transform.position.z);
    }

    public void Snap() {
        if (targets.Count == 0)
            return;
        cam.orthographicSize = GetDesiredOrtho();
        Vector2 newPos = GetDesiredPosition();
        cam.transform.position = new Vector3(newPos.x, newPos.y, cam.transform.position.z);
        zoomVelocity = 0;
        panVelocity = Vector2.zero;
    }

    float GetDesiredOrtho()
    {
        if (targets.Count == 0)
            throw new System.Exception("No targets to track");
        Vector4 minMax = GetMinMax();
        float orthoByY = (minMax.w - minMax.y) / 2;
        float orthoByX = (minMax.z - minMax.x) / cam.aspect / 2;
        float ortho = Mathf.Max(orthoByX, orthoByY);
        ortho = Mathf.Max(ortho, minOrtho);
        ortho = Mathf.Min(ortho, maxOrtho);
        return ortho;
    }

    Vector4 GetMinMax()
    {
        float minX = targets[0].position.x,
                minY = targets[0].position.y,
                maxX = targets[0].position.x,
                maxY = targets[0].position.y;
        foreach (Transform t in targets)
        {
            minX = Mathf.Min(minX, t.position.x);
            minY = Mathf.Min(minY, t.position.y);
            maxX = Mathf.Max(maxX, t.position.x);
            maxY = Mathf.Max(maxY, t.position.y);
        }
        return new Vector4(minX - padding, minY - padding, maxX + padding, maxY + padding);
    }

   

    Vector2 GetDesiredPosition()
    {
        if (targets.Count == 0)
            throw new System.Exception("No targets to track");
        Vector4 minMax = GetMinMax();
        float x = Mathf.Lerp(minMax.x, minMax.z, 0.5f);
        float y = Mathf.Lerp(minMax.y, minMax.w, 0.5f);
        return new Vector2(x, y);
    }
    // call this to make sure a target doesn't go outside the camera's bounds when zoomed all the way out
    public Vector2 ClampInsideCamera(Vector2 pos, float padding = 0.5f)
    {
        if (targets.Count == 0)
            throw new System.Exception("No targets to track");
        // only clamp if we're at max zoom
        if (desiredOrtho != maxOrtho)
            return pos;
        float w = desiredOrtho * cam.aspect - padding;
        float h = desiredOrtho - padding;
        pos.x = Mathf.Clamp(pos.x, desiredPos.x - w, desiredPos.x + w);
        pos.y = Mathf.Clamp(pos.y, desiredPos.y - h, desiredPos.y + h);
        return pos;
    }

    public void AddCameraTarget(Transform target)
    {
        if (targets.Contains(target))
            throw new System.Exception("Camera is already following target");
        targets.Add(target);
    }

    public void RemoveCameraTarget(Transform target)
    {
        if (!targets.Contains(target))
            throw new System.Exception("Camera is not following target");
        targets.Remove(target);
    }
}
