using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Swipe_HUD : MonoBehaviour
{
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] Transform content;
    float scrollPos;
    int selectedIndex;


    void Update()
    {
        float[] pos = new float[content.childCount];
        float distance = 1f / (pos.Length - 1f);
        for (int i = 0; i < pos.Length; i++) pos[i] = distance * i;

        if (Input.GetMouseButton(0))
        {
            scrollPos = scrollBar.value;
        }
        else
        {
            for (int i = 0; i < pos.Length; i++)
            {
                if (scrollPos < pos[i] + (distance / 2) && (scrollPos > pos[i] - (distance / 2)))
                {
                    selectedIndex = i;
                    scrollBar.value = Mathf.Lerp(scrollBar.value, pos[i], .1f);
                }
            }
        }

        //for (int i = 0; i < pos.Length; i++)
        //{
        //    if (scrollPos < pos[i] + (distance / 2) && (scrollPos > pos[i] - (distance / 2)))
        //    {
        //        Transform buttonOBJ = content.GetChild(i);
        //        buttonOBJ.localScale = Vector2.Lerp(buttonOBJ.localScale, new Vector2(1f, 1f), .1f);

        //        for (int a = 0; a < pos.Length; a++) 
        //        {
        //            buttonOBJ = content.GetChild(a);
        //            if (a != i)
        //            {
        //                float distanceFromSelected = Mathf.Abs(a - i);
        //                float newSize = 1f - distanceFromSelected * .25f;
        //                buttonOBJ.localScale = Vector2.Lerp(buttonOBJ.localScale, new Vector2(newSize, newSize), .1f);
        //            }
        //        }
        //    }
        //}
    }
}
