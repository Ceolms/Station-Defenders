using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSeeThrough : MonoBehaviour
{
    public Shader seeThroughShader;
    public Material material;
    public Shader originalShader;
    private GameObject wallCheckCamera;
    public bool isHidden;
    public GameObject obstacleObject;

    void Start()
    {
        originalShader = material.shader;
        wallCheckCamera = GameObject.Find("SeeThroughCamera");
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 fromPosition = wallCheckCamera.transform.position;
        Vector3 toPosition = this.transform.position;
        toPosition = new Vector3(toPosition.x, toPosition.y + 1.5f, toPosition.z);
        Vector3 direction = toPosition - fromPosition;

        Debug.DrawRay(wallCheckCamera.transform.position, direction);
        if (Physics.Raycast(wallCheckCamera.transform.position, direction, out hit))
        {
            if (hit.collider.gameObject.tag.Equals("Obstacle"))
            {
                Debug.Log(this.gameObject.name +" " + hit.collider.name);
                isHidden = true;
                material.shader = seeThroughShader;
                obstacleObject = hit.collider.gameObject;
            }
            else
            {
                isHidden = false;
                material.shader = originalShader;
            }
        }
    }

    void OnApplicationQuit()
    {
        material.shader = originalShader;
    }
}
