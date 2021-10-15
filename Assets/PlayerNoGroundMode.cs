using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PlayerNoGroundMode : Player
{
    public float timeBeforeEnablePhysics = 6;
    private void Start()
    {
        rig.isKinematic = true;
        StartCoroutine(TurnOffKinematicAfter());
    }

    IEnumerator TurnOffKinematicAfter()
    {
        yield return new WaitForSeconds(timeBeforeEnablePhysics);
        fallJump = false;
        rig.isKinematic = false;
        rig.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
    }
}
