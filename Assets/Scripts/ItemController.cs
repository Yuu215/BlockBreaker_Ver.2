using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    //�R���|�[�l���g�n
    private Rigidbody2D myRigidbody2D;
    private Transform target;              //���S�̃Q�[���I�u�W�F�N�g�̈ʒu
    private GameObject frameIcon;          //���A�C�R��
    private GameObject blizzardIcon;       //�X�A�C�R��
    private GameObject plasmaIcon;         //���A�C�R��
    private GameObject healingIcon;        //�񕜃A�C�R��
    private PlayerStatusManager.Strengthening itemType;
    private PlayerStatusManager _playerStatusManager;
    private SpriteRenderer _spriteRenderer;

    //�A�C�e���̑��x��
    [SerializeField] private float itemSpeed;
    [SerializeField] private float rotate;

    public Color color_1, color_2, color_3, color_4;


    private void Awake()
    {
        myRigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        this._spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

        //�A�C�R���擾
        this.frameIcon = this.gameObject.transform.Find("Frame").gameObject;
        this.blizzardIcon = this.gameObject.transform.Find("Blizzard").gameObject;
        this.plasmaIcon = this.gameObject.transform.Find("Plasma").gameObject;
        this.healingIcon = this.gameObject.transform.Find("Healing").gameObject;
    }

    void Start()
    {
        _playerStatusManager = GameObject.Find("Manager").GetComponent<PlayerStatusManager>();
        this.target = GameObject.Find("Haert").GetComponent<Transform>();
    }

    void Update()
    {
        //�u���b�N�̉�]�ƒ��S�ւ̈ړ�
        this.gameObject.transform.Rotate(new Vector3(0, 0, rotate) * Time.deltaTime);
        this.gameObject.transform.position = Vector3.MoveTowards(transform.position, target.position, itemSpeed * Time.deltaTime);
    }

    public void SetItem(BlockController.BlockType setItem)
    {
        ResetIcon();

        switch (setItem)
        {
            case BlockController.BlockType.Flame:
                this.itemType = PlayerStatusManager.Strengthening.Frame;
                this._spriteRenderer.color = color_1;
                this.frameIcon.SetActive(true);
                break;
            case BlockController.BlockType.Blizzard:
                this.itemType = PlayerStatusManager.Strengthening.Blizzard;
                this._spriteRenderer.color = color_2;
                this.blizzardIcon.SetActive(true);
                break;
            case BlockController.BlockType.Plasma:
                this.itemType = PlayerStatusManager.Strengthening.Plasma;
                this._spriteRenderer.color = color_3;
                this.plasmaIcon.SetActive(true);
                break;
            case BlockController.BlockType.Healing:
                this.itemType = PlayerStatusManager.Strengthening.Healing;
                this._spriteRenderer.color = color_4;
                this.healingIcon.SetActive(true);
                break;
            default:
                this.itemType = PlayerStatusManager.Strengthening.none;
                break;
        }
    }

        private void ResetIcon()
    {
        this.frameIcon.SetActive(false);
        this.blizzardIcon.SetActive(false);
        this.plasmaIcon.SetActive(false);
        this.healingIcon.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("�A�C�e�����ڐG����");
        if (collision.gameObject.tag == "Heart")
        {
            _playerStatusManager.SetPlayerBuffZone(itemType);
            this.gameObject.SetActive(false);
        }
        if (collision.gameObject.tag == "PlayerBall")
        {
            _playerStatusManager.SetPlayerBuffBall(itemType);
            this.gameObject.SetActive(false);
        }
    }
}