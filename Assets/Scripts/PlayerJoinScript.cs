using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJoinScript : MonoBehaviour
{
    public GameObject textHint;
    public GameObject playerPreview;

    // Start is called before the first frame update

    public void Join()
    {
        textHint.GetComponent<Text>().text = "Ready !";
        textHint.GetComponent<Text>().color = Color.yellow;
        playerPreview.SetActive(true);
        playerPreview.GetComponent<Animator>().SetTrigger("Salute");
    }
    public void Quit()
    {
        textHint.GetComponent<Text>().text = "Press Start to Join !";
        textHint.GetComponent<Text>().color = Color.white;
        playerPreview.SetActive(false);
    }
}
