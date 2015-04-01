using UnityEngine;
using System.Collections;

public class CiccioMoveController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private float lastTime;

	// Use this for initialization
	void Start () {
        this.agent = this.GetComponent<NavMeshAgent>();
        this.animator = this.GetComponent<Animator>();
        lastTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () 
    {

        this.animator.SetBool("Move", this.agent.velocity.magnitude > 0);
       

        if (Time.time > lastTime + 4)
        {
            this.agent.destination = new Vector3(Random.Range(-4, 4), 0, Random.Range(-4, 4));
            lastTime = Time.time;

        }
	}
}
