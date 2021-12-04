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
        float cloack = 0;
        while(cloack < timeBeforeEnablePhysics)
        {
            if (Input.anyKeyDown) break;
            cloack += Time.deltaTime;
            yield return null;
        }
        fallJump = false;
        rig.isKinematic = false;
        rig.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
    }
}
