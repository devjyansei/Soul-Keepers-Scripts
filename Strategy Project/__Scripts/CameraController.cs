using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoSingleton<CameraController>
{
    private float panSpeedHolder;
    public float panSpeed = 5f;
    public float panBorderThickness = 10f;
    public Vector2Int panLimit;

    public float scrollSpeed = 7f;
    public float minY = 3f;
    public float maxY = 20f;

    private bool isPanActive=true;

    public Camera mainGameCam;
   // public Camera uiCam;

    private void Start()
    {
        panSpeedHolder = panSpeed;
        EnablePan();

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePan();
        }

        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        


        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * 100f *  Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);

        transform.position = pos;

    }
    public void EnablePan()
    {
        panSpeed = panSpeedHolder;
        isPanActive = true;
    }
    public void DisablePan()
    {
        panSpeed = 0f;
        isPanActive = false;
    }
    public void TogglePan()
    {
        if (isPanActive)
        {
            DisablePan();
            
        }
        else
        {
            EnablePan();
            
        }
    }
    public void SetPanSped(float value)
    {
        panSpeed = value;
    }
    public float GetPanSpeed()
    {
        return panSpeed;
    }

    public void TranslateCamera(Vector3 startPosition, Vector3 targetPosition, float duration)
    {
        StartCoroutine(MoveCamera(startPosition, targetPosition, duration));
    }

    private IEnumerator MoveCamera(Vector3 startPosition, Vector3 targetPosition, float duration)
    {
        float timer = 0f;
        Vector3 originalPosition = transform.position;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
    }
}
