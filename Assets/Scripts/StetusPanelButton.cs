using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StetusPanelButton : MonoBehaviour
{
    //ステータスパネルを開くor閉じる
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private PlayerStatusManager _playerStatusManager;
    [SerializeField] private Frame frame;

    public enum Frame
    { 
        Left,
        Middle,
        Right
    }


    private void Start()
    {

    }


    public void OpenPanel()
    {
        if (_gameManager.canGameStart)
        {
            _gameManager.GameStop();
            this.statusPanel.SetActive(true);
        }
    }

    public void ClosePanel()
    {
        _gameManager.GameMove();
        this.statusPanel.SetActive(false);
    }

    public void RedrawBoutton()
    {
        _playerStatusManager.ResetPowerUpSelect(false);
    }

    public void SkillExplanation()
    {
        _playerStatusManager.SetSkillExplanation(frame);
    }

    public void DecisionSkill()
    {
        _playerStatusManager.PowerUpDecision(frame);
    }
}
