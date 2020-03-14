using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHand : MonoBehaviour
{
    public GameObject crosshair;
    public Transform magicCircle;
    public Transform Camera;
    public GameObject fireball;
    public GameObject iceball;
    public float fireballSpeed = 5.0f;
    public AudioSource fireSound;
    public int mode;//1:fire     2:ice

    private GameObject newCrosshair;
    private float previousShootTime;
    private float nowShootTime;

    // Start is called before the first frame update
    void Start()
    {
        newCrosshair =  Instantiate(crosshair, Vector3.zero, Quaternion.identity);
        previousShootTime = 0;
        mode = 1;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        int layerMask;
        layerMask = 1<<11 | 1<<13;
        if(Physics.Raycast(transform.position, transform.forward, out hit,  Mathf.Infinity, layerMask))
        {
            newCrosshair.transform.position = hit.point;
            newCrosshair.transform.LookAt(Camera);
        }
        
        magicCircle.Rotate(0, 0, Time.deltaTime * 130f);
    }

    public void ModeSwitch()
    {
        if(mode == 1) 
        {
            mode = 2;
            magicCircle.GetComponent<SpriteRenderer>().sprite = Resources.Load("MagicCircleIce", typeof(Sprite)) as Sprite;
        }
        else 
        {
            mode = 1;
            magicCircle.GetComponent<SpriteRenderer>().sprite = Resources.Load("MagicCircleFire", typeof(Sprite)) as Sprite;
        }
    }

    public void Cast()
    {
        if(mode == 1)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().SendSerialMessage("v");
        }
        else if(mode == 2)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().SendSerialMessage("x");
        }
    }

    public void Shoot(float temp)
    {
        nowShootTime = Time.time;

        if(nowShootTime - previousShootTime >= 2.0f) //allow to shoot
        {
            previousShootTime = nowShootTime;

            Vector3 shootingDirection = (newCrosshair.transform.position+ new Vector3(0, 0, 0) - transform.position).normalized;

            if(mode == 1)
            {
                GameObject newFireball = Instantiate(fireball, transform.position + new Vector3(0, 0, 0), transform.rotation);

                if(!GameObject.Find("GameManager").GetComponent<GameManager>().isCoolingDown && temp > GameObject.Find("GameManager").GetComponent<GameManager>().GetRightTemp())
                {
                    newFireball.tag = "PlayerFireball";
                    fireSound.volume = 1.0f;
                    GameObject.Find("GameManager").GetComponent<GameManager>().CoolDown();
                }
                else
                {
                    newFireball.tag = "PlayerFireballSmall";

                    Vector3 fireballSmallScale = new Vector3(0.2f, 0.2f, 0.2f);
                    newFireball.transform.localScale =  fireballSmallScale;
                    newFireball.transform.GetChild(0).localScale = fireballSmallScale;
                    newFireball.transform.GetChild(1).localScale = fireballSmallScale;
                    newFireball.transform.GetChild(2).localScale = fireballSmallScale;
                    newFireball.transform.GetChild(3).localScale = fireballSmallScale;

                    fireSound.volume = 0.5f;
                }

                FireballControl fireballControl = newFireball.GetComponent<FireballControl>();
                fireballControl.SetTarget(transform);
                fireballControl.SetVelocity(shootingDirection * fireballSpeed);

                fireSound.Play();

                Destroy(newFireball, 10.0f); 
            }
            else if(mode == 2)
            {
                GameObject newFireball = Instantiate(iceball, transform.position + new Vector3(0, 0, 0), transform.rotation);

                if(!GameObject.Find("GameManager").GetComponent<GameManager>().isCoolingDown && temp < GameObject.Find("GameManager").GetComponent<GameManager>().GetIceTemp())
                {
                    newFireball.tag = "PlayerFireball";
                    fireSound.volume = 1.0f;
                    GameObject.Find("GameManager").GetComponent<GameManager>().CoolDown();
                }
                else
                {
                    newFireball.tag = "PlayerFireballSmall";

                    Vector3 fireballSmallScale = new Vector3(0.2f, 0.2f, 0.2f);
                    newFireball.transform.localScale =  fireballSmallScale;
                    newFireball.transform.GetChild(0).localScale = fireballSmallScale;
                    newFireball.transform.GetChild(1).localScale = fireballSmallScale;

                    fireSound.volume = 0.5f;
                }

                FireballControl fireballControl = newFireball.GetComponent<FireballControl>();
                fireballControl.SetTarget(transform);
                fireballControl.SetVelocity(shootingDirection * fireballSpeed);

                fireSound.Play();

                Destroy(newFireball, 10.0f); 
            }      
        }
    }
}
