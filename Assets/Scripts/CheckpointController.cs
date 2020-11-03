using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{

    public Transform activationTrigger;
    public Animator animator;
    public LayerMask whatIsCheckpointTrigger;
    private bool isActive;
    public float activationRadius;
    public LevelManager levelManager;
    // Start is called before the first frame update

    private void Awake()
    {
        isActive = false;
    }

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        isActive = Physics2D.OverlapCircle(activationTrigger.position, activationRadius, whatIsCheckpointTrigger);
        animator.SetBool("isActive", isActive);
    }
}
