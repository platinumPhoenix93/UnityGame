using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public float respawnDelay;
    public Player gamePlayer;

    private static CheckpointController[] FindGameObjectsWithTag(string v)
    {
        throw new NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        gamePlayer = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Respawn()
    {
        gamePlayer.gameObject.SetActive(false);
        gamePlayer.transform.position = gamePlayer.respawnPoint;
        gamePlayer.gameObject.SetActive(true);
    }
}
