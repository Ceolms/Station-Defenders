using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    public int maxHealthPoint = 1000;
    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealthPoint;
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
    }

    public void TakeDamage(int damages)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damages;
            if (currentHealth < 0) currentHealth = 0;
            foreach (PlayerController p in GameManager.Instance.players)
            {
                p.uiManager.SetCoreLifebarSize(currentHealth);
            }
        } 
    }
}
