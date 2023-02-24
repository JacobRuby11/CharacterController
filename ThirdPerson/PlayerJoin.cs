using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoin : MonoBehaviour
{
    public Camera cam;
    FollowObjects follower;
    PlayerInputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        follower = cam.GetComponent<FollowObjects>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
            
    }

    void OnPlayerJoined(PlayerInput playerInput){
        Debug.Log(playerInput.devices[0]);
        follower.objects.Add(playerInput.gameObject.transform);
    
    }
}
