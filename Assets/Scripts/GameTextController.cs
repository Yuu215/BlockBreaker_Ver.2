using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTextController : MonoBehaviour
{
    private float _dispaytime = 0.15f;

    private float _timer;
    private Text _txt;
    private Vector2 pos;
    public Color color_1, color_2, color_3;

    private void Awake()
    {
        _txt = this.gameObject.GetComponent<Text>();
    }

    public void SetDamageText(int damage, Vector2 pos , string color)
    {
        this.pos = pos;
        this.transform.position = pos;
        this._timer = 0;
        switch (color)
        {
            case "Frame":
                _txt.color = color_1;
                break;
            case "Normal":
                _txt.color = color_2;
                break;
            case "Plasma":
                _txt.color = color_3;
                break;
        }
            
        _txt.text = damage.ToString();
    }

    public void SetEXPText(int exp, Vector2 pos)
    {
        this.pos = pos;
        this.transform.position = pos;
        this._timer = 0;
        _txt.color = color_2;
        _txt.text = "EXP\n" + "+" + exp.ToString();


    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < _dispaytime)
        {
            pos.y += 0.01f;
            this.gameObject.transform.position = pos;
        }
        if (_timer > _dispaytime)
        {
            Invoke(nameof(SetActiveFalse), 1f);
        }
    }

    void SetActiveFalse()
    {
        this.gameObject.SetActive(false);
    }
}
