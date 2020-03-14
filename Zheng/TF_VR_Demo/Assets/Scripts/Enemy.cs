using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    //public Collider playerCollider;
    public GameObject fireball;
    public float fireballSpeed = 5.0f;
    public RectTransform canvas;
    public RectTransform hpBar;
    public SkinnedMeshRenderer skinnedMeshRenderer;

    private int MaxHP = 100;
    private int currentHP;
    private NavMeshAgent agent;
    private Animator animator;
    private bool findFlag;
    private bool invokeFlag;
    private bool isAlive;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        findFlag = false;
        invokeFlag = false;
        currentHP = MaxHP;
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        Transform player = GameObject.Find("OVRPlayerController").transform;
        Collider playerCollider = GameObject.Find("OVRPlayerController").GetComponent<Collider>();

        RaycastHit hit;
        if(!findFlag)
        {
            if(Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, 12.0f)) 
            {
                //Debug.Log("find you");
                if(hit.transform == playerCollider.transform)
                {
                    agent.SetDestination(player.position);
                    findFlag = true;
                }
            }
            else if(currentHP < MaxHP)
            {
                agent.SetDestination(player.position);
                findFlag = true;
            }
        }
        else
        {
            if((transform.position - player.position).magnitude < 6.0f) 
            {
                agent.Stop();
                transform.LookAt(player.transform);
                if(!invokeFlag)
                {
                    invokeFlag = true;
                    InvokeRepeating("Attack", 1.5f, 4.0f);
                }
            }
            else 
            {
                CancelInvoke();
                invokeFlag = false;
                agent.SetDestination(player.position);
                agent.Resume();
            }
        }

        canvas.LookAt(player.transform);
    }

    void Attack()
    {
        Transform player = GameObject.Find("OVRPlayerController").transform;

        animator.SetTrigger("Projectile Attack 01");

        Vector3 shootingDirection = (player.position+ new Vector3(0, -0.5f, 0) - transform.position).normalized;

        GameObject newFireball = Instantiate(fireball, transform.position + new Vector3(0, 1, 0), transform.rotation);
        newFireball.tag = "EnemyFireball";
        FireballControl fireballControl = newFireball.GetComponent<FireballControl>();
        fireballControl.SetTarget(transform);
        fireballControl.SetVelocity(shootingDirection * fireballSpeed);

        Destroy(newFireball, 10.0f);
    }

    public void TakeDamage(int damage)
    {
        if(isAlive)
        {
            currentHP -= damage;

            if (currentHP <= 0)
            {
                hpBar.localScale = new Vector3(0f, 1f, 1f);
                isAlive = false;
                CancelInvoke();
                agent.Stop();
                animator.SetTrigger("Die");
                int leftEnemiesCount = GameObject.Find("GameManager").GetComponent<GameManager>().GetLeftEnemiesCount();
                GameObject.Find("DistanceGrabHandLeft").GetComponent<LeftHand>().SetLeftEnemiesText(leftEnemiesCount - 1);
                GameObject.Find("GameManager").GetComponent<GameManager>().SetLeftEnemiesCount(leftEnemiesCount - 1);
                Destroy(gameObject, 2.0f);
                enabled = false;
            }
            else
            {
                hpBar.localScale = new Vector3(currentHP/100.0f, 1f, 1f);
            }           
        }
    }
}
