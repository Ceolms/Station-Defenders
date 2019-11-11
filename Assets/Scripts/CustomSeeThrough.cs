using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSeeThrough : MonoBehaviour
{
    public Shader seeThroughShader;
    public Material material;
    private Shader originalShader;
    private Camera wallCheckCamera;

    void Start()
    {
        wallCheckCamera = GameObject.Find("SeeThroughCamera").GetComponent<Camera>();
        originalShader = material.shader;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 fromPosition = wallCheckCamera.transform.position;
        Vector3 toPosition = this.transform.position;
        Vector3 direction = toPosition - fromPosition;

        if (Physics.Raycast(wallCheckCamera.transform.position, direction, out hit))
        {
            if (hit.collider.gameObject.tag.Equals("Obstacle"))
            {
                material.shader = seeThroughShader;
            }
            else material.shader = originalShader;
        }
    }
}
