using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Text/EffectTextTemplate")]
public class EffectTextTemplate : ScriptableObject {

    public string m_text = "<sprite=1> SAMPLE TEXT!!";
    public Color m_baseColor = Color.white;
    public Color m_outlineColor = Color.white;
}
