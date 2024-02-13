using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectMenuController : MonoBehaviour
{
    [SerializeField] GameObject arrow;
    [SerializeField] Color color_1, color_2;
    [SerializeField] Text text;
    [SerializeField] StartSound _startSound;
    [SerializeField] TitleManager _titleManager;

    void Start()
    {
         text.color = color_2;
    }

    void Update()
    {
        
    }

    public void PassingMouse()
    {
        arrow.SetActive(true);
        text.color = color_1;
        _startSound.MenuSound();
    }

    public void LeaveMouse()
    {
        arrow.SetActive(false);
        text.color = color_2;
    }

    public void GameSceneMove()
    {
        _titleManager.GameSceneLoading();
    }

    

}
