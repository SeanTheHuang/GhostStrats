﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Text/EffectTextTemplate")]
public class EffectTextTemplate : ScriptableObject {

    public string m_iconText = "<sprite=1>";
    public string m_followUpText = " ME SEAN";
    public Material m_textMaterial;
}
