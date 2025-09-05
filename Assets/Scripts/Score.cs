using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public int _score;
    [SerializeField] Text _text;
    [SerializeField] Animator _animator;
    float previousScore;
    void Start()
    {
        _score = 0;
    }

    void Update()
    {
        previousScore = Mathf.Lerp(previousScore, (float)_score, Time.deltaTime * 10);
        _text.text = (Mathf.RoundToInt(previousScore)).ToString();
    }

    public void UpdateScore(int s) 
    {
        _score += s;
        _animator.SetTrigger("Pop");
    }
}

