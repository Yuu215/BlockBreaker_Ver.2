using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    //<������������>
    //�ϐ�
    [SerializeField] private LineRenderer _rend;     //���C�������_���[
    [SerializeField] private Camera       _cam;      //�J����
    [SerializeField] private GameObject   paddle;    //�p�h��
    private Vector2 startLinePos;      //�n�_�̍��W
    private Vector2 endLinePos;        //�I�_�̍��W

    private int posCount = 0;
    private float interval = 0.1f;

    private bool CanLineDraw = false;   //���C���������邩�ǂ���

    void Start()
    {
        CanLineDraw = true; //���u��
    }

    public void CanLine(bool x)
    {
        if (x)
            this.CanLineDraw = true;
        if (!x)
            this.CanLineDraw = false;
    }

    void Update()
    {
        //�}�E�X�̃|�f�B�V����
        Vector2 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);

        if(this.CanLineDraw)
        {
            if (Input.GetMouseButtonDown(0))
                this.startLinePos = mousePos;
            if (Input.GetMouseButton(0))
                SetPosition(mousePos);
            if (Input.GetMouseButtonUp(0))
            {
                this.endLinePos = mousePos;
                posCount = 0;
                GeneratePaddle();
            }

        }
    }

    private void SetPosition(Vector2 pos)
    {
        if (!PosCheck(pos)) return;

        posCount++;
        _rend.positionCount = posCount;
        _rend.SetPosition(posCount - 1, pos);
    }

    private bool PosCheck(Vector2 pos)
    {
        if (posCount == 0) return true;

        //�O��̃|�C���g�ƈ��ȏ㗣�ꂽ��V���Ƀ|�C���g�𐶐�
        float distance = Vector2.Distance(_rend.GetPosition(posCount - 1), pos);
        return (distance > interval);
    }

    private void GeneratePaddle()
    {
        _rend.positionCount = posCount;   �@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@   �@//�����폜
        paddle.gameObject.SetActive(true);                                             //�p�h�����A�N�e�B�u�ɂ���

        float distance    = Vector2.Distance(startLinePos, endLinePos);                //�n�_�ƏI�_�̋��������߂�
        Vector2 midpoint  = Vector2.Lerp(startLinePos, endLinePos, 0.5f);              //���_�����߂�
        Vector2 direction = endLinePos - startLinePos;
        float angle       = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;     //�p�x�����߂�

       /* Debug.Log("�n�_�́A" + startLinePos + "�B�I�_�́A" + endLinePos);
        Debug.Log("�n�_�ƏI�_�̋����́A" + distance);
        Debug.Log("���_�́A" + midpoint);
        Debug.Log("�p�x�́A" + angle); */

        if (distance <= 0.5f)                                                          //�����Z������ꍇ�̓p�h����ύX���Ȃ�
        {
            paddle.gameObject.SetActive(false);
            return;
        }
        if (0.5f < distance && distance <= 1f)
            this.paddle.transform.localScale = new Vector2(0.15f, 1f);               �@//�p�h���̒�����ύX(�Œ�1�̒�����)
        else
            this.paddle.transform.localScale = new Vector2(0.15f, distance);           //�p�h���̒�����ύX

        this.paddle.transform.position = new Vector2(midpoint.x, midpoint.y);          //�p�h���̈ʒu��ύX
        this.paddle.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);        //�p�h���̊p�x��ύX
    } 

    public void DeletePaddle()
    {
        paddle.gameObject.SetActive(false);
    }
}
