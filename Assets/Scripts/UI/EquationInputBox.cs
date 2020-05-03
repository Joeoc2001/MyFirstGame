﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EquationInputBox : EquationProvider
{
    InputField field;

    private void Awake()
    {
        field = GetComponent<InputField>();
        field.onEndEdit.AddListener(s => UpdateEquation(s));
    }

    private void UpdateEquation(string s)
    {
        Equation equation = Parser.Parse(s);
        SetEquation(equation.GetSimplified());
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateEquation(field.text);
    }
}