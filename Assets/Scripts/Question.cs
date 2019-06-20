using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Question
{
    private const int SIZE = 4;

    [SerializeField] private string questionText;
    [SerializeField] private string[] answers = new string[SIZE];

    public string QuestionText => questionText;
    public string[] Answers => answers;

    private void OnValidate()
    {
        if (answers.Length != SIZE)
        {
            Debug.LogWarning("Don't change the 'answers' field's array size!");
            Array.Resize(ref answers, SIZE);
        }
    }
}
