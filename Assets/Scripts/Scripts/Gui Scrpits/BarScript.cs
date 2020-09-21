using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarScript : MonoBehaviour {
[SerializeField]
    private Text valueText;
    [SerializeField]
    private float lerpSpeed;
    private float fillAmount;
    [SerializeField]
    private Image content;

    public float MaxValue { get; set; }

    // Use this for initialization
    void Start () {
      
	}
	public float Value
    {
        set
        {
            
           string[] temp = valueText.text.Split(':');
           valueText.text =  temp[0] + ": " + value;
            fillAmount = Map(value,MaxValue);
            temp[1] = temp[0] + ": " + value;
        }
    }
	// Update is called once per frame
	void Update () {
        Handlebar();
	}
    private void Handlebar()
    {
        if (fillAmount != content.fillAmount)
        {
            content.fillAmount = Mathf.Lerp(content.fillAmount, fillAmount, Time.deltaTime * lerpSpeed);
        }
        else
        {

        }
    }
            private float Map(float value, float inMax)
    {
        return value / inMax;
    }
}
