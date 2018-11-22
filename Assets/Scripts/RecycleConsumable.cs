﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleConsumable : MonoBehaviour
{
    public Parallaxer parallax;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {

            parallax.OnCollisionWithPlayer();
        }
    }
}
