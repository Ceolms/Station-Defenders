using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    public int maxHealthPoint = 1000;
    private int currentHealth;
    private bool anim;
    private bool left = true;
    GameObject coreMesh;

    private float shakeDuration = 0f;
    private float shakeAmount = 0.02f;
    private float decreaseFactor = 0.5f;
    Vector3 originalPos;
    void Start()
    {
        currentHealth = maxHealthPoint;
        coreMesh = this.transform.GetChild(0).gameObject;
        originalPos = coreMesh.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth == 0) // if lifePoints <= 0
        {
            Debug.Log("Game Over");
            foreach(PlayerController p in GameManager.Instance.players)
            {
                p.uiManager.GameOverScreen();
            }
        }
        if(anim)
        {
            if (shakeDuration > 0)
            {
                coreMesh.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

                shakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                shakeDuration = 0f;
                coreMesh.transform.localPosition = originalPos;
                anim = false;
                anim = false;
                coreMesh.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.white;
                coreMesh.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(23, 127, 137, 69);
            }
        }
    }

    public void TakeDamage(int damages)
    {
        if(damages >0)
        {
            Debug.Log("Core take damage :" + damages);
            if (currentHealth > 0)
            {
                currentHealth -= damages;
                if (currentHealth < 0) currentHealth = 0;
                foreach (PlayerController p in GameManager.Instance.players)
                {
                    p.uiManager.SetCoreLifebarSize(currentHealth / 10);
                }
                if (!anim)
                {
                    anim = true;
                    coreMesh.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
                    coreMesh.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(255, 0, 0, 69);
                    shakeDuration = 0.4f;
                }
            }
        }
    }
}
