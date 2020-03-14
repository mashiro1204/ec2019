using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Text hpText;
    public AudioSource footstepSound;

    private int MaxHP = 100;
    private int currentHP;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = MaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 joystickAxis = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.LTouch);
        if(joystickAxis.magnitude > 0.8f && !footstepSound.isPlaying)
        {
            //Debug.Log("dsfsdfsf");
            footstepSound.Play();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            hpText.text = "HP:"+ 0 + "%";
            GameObject.Find("GameManager").GetComponent<GameManager>().GameOver();
        }
        else
        {
            hpText.text = "HP:"+ currentHP + "%";
        }
    }
}
