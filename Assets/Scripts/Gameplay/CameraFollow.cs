using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform followTransform;
    public BoxCollider2D mapBounds;

    private float xMin, xMax, yMin, yMax;
    private float camY,camX;
    private float camOrthsize;
    private float cameraRatio;
    private Camera mainCam;

    private void Start()
    {
        CalculateBounds(this.mapBounds);

    }
    // Update is called once per frame
    void Update()
    {
        camY = Mathf.Clamp(followTransform.position.y, yMin + camOrthsize, yMax - camOrthsize);
        camX = Mathf.Clamp(followTransform.position.x, xMin + cameraRatio, xMax - cameraRatio);
        this.transform.position = new Vector3(camX, camY, this.transform.position.z);

        
    }
    /**
     * Change boundaries of camara.
     * 
     * @param bounds new bounds
     */
    public void ChangeBounds(BoxCollider2D bounds)
    {
        this.mapBounds = bounds;
        CalculateBounds(this.mapBounds);
    }
    /**
     * Calculate new boundaries of camera.
     * 
     * @param mapBounds bounds of map
     */
    void CalculateBounds(BoxCollider2D mapBounds)
    {
        xMin = mapBounds.bounds.min.x;
        xMax = mapBounds.bounds.max.x;
        yMin = mapBounds.bounds.min.y;
        yMax = mapBounds.bounds.max.y;
        mainCam = GetComponent<Camera>();
        camOrthsize = mainCam.orthographicSize + 0.5f;
        cameraRatio = (mainCam.aspect * 2f * mainCam.orthographicSize) / 2.0f + 0.5f;
    }
}