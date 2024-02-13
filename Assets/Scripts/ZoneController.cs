using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ZoneController : MonoBehaviour
{
    [SerializeField] private Vector2 maximumScale;
    [SerializeField] private Vector2 minimumScale;
    [SerializeField] private float duration = 1.0f;

    private Tweener scaleTweener;

    [SerializeField] private Color _color1, _color2, _color3;

    [SerializeField] private PlayerStatusManager _playerStatusManager;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        this._spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        
    }

    public void ExpansionZone(string type)
    {
        scaleTweener.Kill();
        this.gameObject.transform.localScale = new Vector3(0f, 0f, 1f);

        switch (type)
        {
            case "Frame":
                this._spriteRenderer.color = _color1;
                break;
            case "Blizzard":
                this._spriteRenderer.color = _color2;
                break;
            case "Plasma":
                this._spriteRenderer.color = _color3;
                break;
            default:;
                break;
        }


        scaleTweener = transform.DOScale(maximumScale, duration)
            .SetEase(Ease.OutBack);
    }

    public void ReductionZone()
    {
        this.gameObject.transform.localScale = new Vector3(0f, 0f, 1f);
    }
}
