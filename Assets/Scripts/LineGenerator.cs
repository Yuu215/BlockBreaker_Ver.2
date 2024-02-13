using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    //<線を引く処理>
    //変数
    [SerializeField] private LineRenderer _rend;     //ラインレンダラー
    [SerializeField] private Camera       _cam;      //カメラ
    [SerializeField] private GameObject   paddle;    //パドル
    private Vector2 startLinePos;      //始点の座標
    private Vector2 endLinePos;        //終点の座標

    private int posCount = 0;
    private float interval = 0.1f;

    private bool CanLineDraw = false;   //ラインが引けるかどうか

    void Start()
    {
        CanLineDraw = true; //仮置き
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
        //マウスのポディション
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

        //前回のポイントと一定以上離れたら新たにポイントを生成
        float distance = Vector2.Distance(_rend.GetPosition(posCount - 1), pos);
        return (distance > interval);
    }

    private void GeneratePaddle()
    {
        _rend.positionCount = posCount;   　　　　　　　　　　　　　　　　　　　　   　//線を削除
        paddle.gameObject.SetActive(true);                                             //パドルをアクティブにする

        float distance    = Vector2.Distance(startLinePos, endLinePos);                //始点と終点の距離を求める
        Vector2 midpoint  = Vector2.Lerp(startLinePos, endLinePos, 0.5f);              //中点を求める
        Vector2 direction = endLinePos - startLinePos;
        float angle       = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;     //角度を求める

       /* Debug.Log("始点は、" + startLinePos + "。終点は、" + endLinePos);
        Debug.Log("始点と終点の距離は、" + distance);
        Debug.Log("中点は、" + midpoint);
        Debug.Log("角度は、" + angle); */

        if (distance <= 0.5f)                                                          //線が短すぎる場合はパドルを変更しない
        {
            paddle.gameObject.SetActive(false);
            return;
        }
        if (0.5f < distance && distance <= 1f)
            this.paddle.transform.localScale = new Vector2(0.15f, 1f);               　//パドルの長さを変更(最低1の長さに)
        else
            this.paddle.transform.localScale = new Vector2(0.15f, distance);           //パドルの長さを変更

        this.paddle.transform.position = new Vector2(midpoint.x, midpoint.y);          //パドルの位置を変更
        this.paddle.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);        //パドルの角度を変更
    } 

    public void DeletePaddle()
    {
        paddle.gameObject.SetActive(false);
    }
}
