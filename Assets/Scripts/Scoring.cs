using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Scoring : MonoBehaviour
{
    public int plusScore;
    public int minusScore;
    private SphereCollider plusTop, plusBottom, minusTop, minusBottom;
    private float plusTime, minusTime;
    public TMP_Text plusScoreText;
    public TMP_Text minusScoreText;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject == minusBottom.gameObject && minusTime + 3 > Time.time) {
            plusScore += 2;
        }
        if (other.gameObject == plusBottom.gameObject && plusTime + 3 > Time.time) {
            minusScore += 2;
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject == minusTop.gameObject) {
            minusTime = Time.time;
        }
        if (other.gameObject == plusTop.gameObject) {
            plusTime = Time.time;
        }
    }
    void Start()
    {
        plusTop = GameObject.Find("PlusTopCollider").GetComponent<SphereCollider>();
        plusBottom = GameObject.Find("PlusBottomCollider").GetComponent<SphereCollider>();
        minusTop = GameObject.Find("MinusTopCollider").GetComponent<SphereCollider>();
        minusBottom = GameObject.Find("MinusBottomCollider").GetComponent<SphereCollider>();
        plusScore = 0;
        minusScore = 0;
        plusTime = -10;
        minusTime = -10;
    }
    void Update()
    {
        plusScoreText.text = "Player 1: " + plusScore.ToString();
        minusScoreText.text = "Player 2: " + minusScore.ToString();
    }
}