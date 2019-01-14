using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PitfallScript : MonoBehaviour
{

    public GameObject jumpSpheres;

    public SimpleCharacterControl player;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.Spawn();
            jumpSpheres.SetActive(true);
        }

        
    }
}
