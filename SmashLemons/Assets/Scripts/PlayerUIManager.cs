using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    public TextMeshProUGUI damage;
    public TextMeshProUGUI life;
    public Gradient healthGradient;
    public Gradient lifeGradient;
    private int damageTaken;

    private void Start() {
        setDamage(0f);
        setLives(3);
    }

    public void setDamage(float value){
        damageTaken = (int)value;
        damage.text = damageTaken.ToString() + "%";
        damage.color = healthGradient.Evaluate(Mathf.Min((float)damageTaken / 200f, 1f));
    }

    public void setLives(int lives){
        life.text = lives.ToString();
        life.color = lifeGradient.Evaluate(Mathf.Min((float)(lives-1) / 3f, 1f));
    }
}
