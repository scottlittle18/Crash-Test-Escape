using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is attached to the player's ShoveTrigger child object and is used to inact the proper functions for shoving people and objects
/// </summary>
public class PlayerShoveTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "ImmobileDummy")
        {
            // Allows the immobile test dummy to fall over which gives the player a chance to jump on it
            collision.gameObject.GetComponent<Rigidbody2D>().freezeRotation = false;

            // This makes it so the player moves more accurately with the dummy when standing still on it and the conveyor is on
            collision.gameObject.tag = "MovingPlatforms";
        }
    }
}
