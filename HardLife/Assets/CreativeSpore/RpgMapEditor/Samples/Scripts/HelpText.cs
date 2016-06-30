﻿using UnityEngine;
using System.Collections;

namespace CreativeSpore
{

    public class HelpText : MonoBehaviour 
    {
        public GameObject TextObj;
        public float DistanceFromPlayerToAppear = 1f;

        private Renderer m_helpTextRenderer;
        private PlayerController m_player;

	    // Use this for initialization
	    void Start () 
        {
            m_player = FindObjectOfType<PlayerController>();
            m_helpTextRenderer = TextObj.GetComponent<Renderer>();
	    }
	
	    void Update () 
        {
            bool isPlayerCloseEnough = Vector2.Distance(transform.position, m_player.transform.position) <= DistanceFromPlayerToAppear;
            m_helpTextRenderer.enabled = isPlayerCloseEnough;
            if (isPlayerCloseEnough)
            {
                Color textColor = m_helpTextRenderer.material.color;
                textColor.a = Mathf.Clamp(0.2f + Mathf.Abs(Mathf.Sin(0.05f * Time.frameCount)), 0f, 1f);
                m_helpTextRenderer.material.color = textColor;                
            }	
	    }
    }
}