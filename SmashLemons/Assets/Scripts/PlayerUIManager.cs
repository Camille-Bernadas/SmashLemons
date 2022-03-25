using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    public TextMeshProUGUI damage;
    public Gradient healthGradient;
    private int damageTaken;

    private void Start() {
        setDamage(0f);
    }

    public void setDamage(float value){
        damageTaken = (int)value;
        damage.text = damageTaken.ToString() + "%";
        damage.color = healthGradient.Evaluate(Mathf.Min((float)damageTaken / 200f, 1f));
    }
}
