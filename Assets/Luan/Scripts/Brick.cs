using System;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.Serialization;

public class Brick : MonoBehaviour
{
    [SerializeField] private Transform _mainSprite;

    //
    [SerializeField] private Sprite _outBot_Spr;
    [SerializeField] private Sprite _outBot_Shadow_Spr;

    [SerializeField] private Sprite _outRight_Spr;
    [SerializeField] private Sprite _outRight_Shadow_Spr;

    [SerializeField] private Sprite _outLeft_Spr;
    [SerializeField] private Sprite _outLeft_Shadow_Spr;

    [SerializeField] private Sprite _outTop_Spr;
    [SerializeField] private Sprite _outTop_Shadow_Spr;
    //

    [SerializeField] private Sprite _outBotLeft_Spr;
    [SerializeField] private Sprite _outBotLeft_Shadow_Spr;

    [SerializeField] private Sprite _outTopRight_Spr;
    [SerializeField] private Sprite _outTopRight_Shadow_Spr;

    [SerializeField] private Sprite _outTopLeft_Spr;
    [SerializeField] private Sprite _outTopLeft_Shadow_Spr;

    [SerializeField] private Sprite _outBotRight_Spr;
    [SerializeField] private Sprite _outBotRight_Shadow_Spr;
    //

    [SerializeField] private Sprite _mid_Spr;
    [SerializeField] private Sprite _mid_Shadow_Spr;

//
    [SerializeField] private Sprite _inBotLeft_Spr;
    [SerializeField] private Sprite _inBotLeft_Shadow_Spr;

    [SerializeField] private Sprite _inBotRight_Spr;
    [SerializeField] private Sprite _inBotRight_Shadow_Spr;

    [SerializeField] private Sprite _inTopLeft_Spr;
    [SerializeField] private Sprite _inTopLeft_Shadow_Spr;

    [SerializeField] private Sprite _inTopRight_Spr;

    [SerializeField] private Sprite _inTopRight_Shadow_Spr;

    //
    [SerializeField] private Sprite _oneMid_Spr;
    [SerializeField] private Sprite _oneMid_Shadow_Spr;

    //
    [SerializeField] private Sprite _oneBot_Spr;
    [SerializeField] private Sprite _oneBot_Shadow_Spr;

    [SerializeField] private Sprite _oneRight_Spr;
    [SerializeField] private Sprite _oneRight_Shadow_Spr;

    [SerializeField] private Sprite _oneLeft_Spr;
    [SerializeField] private Sprite _oneLeft_Shadow_Spr;

    [SerializeField] private Sprite _oneTop_Spr;
    [SerializeField] private Sprite _oneTop_Shadow_Spr;

    //
    [SerializeField] private Sprite _oneBotLeft_Spr;
    [SerializeField] private Sprite _oneBotLeft_Shadow_Spr;

    [SerializeField] private Sprite _oneBotRight_Spr;
    [SerializeField] private Sprite _oneBotRight_Shadow_Spr;

    [SerializeField] private Sprite _oneTopLeft_Spr;
    [SerializeField] private Sprite _oneTopLeft_Shadow_Spr;

    [SerializeField] private Sprite _oneTopRight_Spr;
    [SerializeField] private Sprite _oneTopRight_Shadow_Spr;

    //
    [SerializeField] private Sprite _oneVertical_Spr;
    [SerializeField] private Sprite _oneVertical_Shadow_Spr;

    [SerializeField] private Sprite _oneHorizontal_Spr;
    [SerializeField] private Sprite _oneHorizontal_Shadow_Spr;

    //
    [SerializeField] private Sprite _oneVerticalBotLeft_Spr;
    [SerializeField] private Sprite _oneVerticalBotLeft_Shadow_Spr;

    [SerializeField] private Sprite _oneVerticalTopLeft_Spr;
    [SerializeField] private Sprite _oneVerticalTopLeft_Shadow_Spr;

    [SerializeField] private Sprite _oneVerticalBotRight_Spr;
    [SerializeField] private Sprite _oneVerticalBotRight_Shadow_Spr;

    [SerializeField] private Sprite _oneVerticalTopRight_Spr;
    [SerializeField] private Sprite _oneVerticalTopRight_Shadow_Spr;

    //
    [SerializeField] private Sprite _oneHorizontalBotRight_Spr;
    [SerializeField] private Sprite _oneHorizontalBotRight_Shadow_Spr;

    [SerializeField] private Sprite _oneHorizontalTopRight_Spr;
    [SerializeField] private Sprite _oneHorizontalTopRight_Shadow_Spr;

    [SerializeField] private Sprite _oneHorizontalBotLeft_Spr;
    [SerializeField] private Sprite _oneHorizontalBotLeft_Shadow_Spr;

    [SerializeField] private Sprite _oneHorizontalTopLeft_Spr;
    [SerializeField] private Sprite _oneHorizontalTopLeft_Shadow_Spr;
    //

    [SerializeField] private Sprite _oneCrossLine_Spr;
    [SerializeField] private Sprite _oneCrossLine_Shadow_Spr;

    [SerializeField] private Sprite _oneCrossLineRight_Spr;
    [SerializeField] private Sprite _oneCrossLineRight_Shadow_Spr;

    [SerializeField] private Sprite _oneCrossLineLeft_Spr;
    [SerializeField] private Sprite _oneCrossLineLeft_Shadow_Spr;

    [SerializeField] private Sprite _oneCrossLineTop_Spr;
    [SerializeField] private Sprite _oneCrossLineTop_Shadow_Spr;

    [SerializeField] private Sprite _oneCrossLineBot_Spr;
    [SerializeField] private Sprite _oneCrossLineBot_Shadow_Spr;
    //

    [SerializeField] private Sprite _oneCrossLineTopLeft_Spr;
    [SerializeField] private Sprite _oneCrossLineTopLeft_Shadow_Spr;

    [SerializeField] private Sprite _oneCrossLineTopRight_Spr;
    [SerializeField] private Sprite _oneCrossLineTopRight_Shadow_Spr;


    [SerializeField] private Sprite _oneCrossLineBotLeft_Spr;
    [SerializeField] private Sprite _oneCrossLineBotLeft_Shadow_Spr;

    [SerializeField] private Sprite _oneCrossLineBotRight_Spr;
    [SerializeField] private Sprite _oneCrossLineBotRight_Shadow_Spr;

//
    [SerializeField] private Sprite _oneBotLeftTopRight_Spr;
    [SerializeField] private Sprite _oneBotLeftTopRight_Shadow_Spr;

    [SerializeField] private Sprite _oneTopLeftBotRight_Spr;
    [SerializeField] private Sprite _oneTopLeftBotRight_Shadow_Spr;

//
    [SerializeField] private Sprite _oneForkLineTop_Spr;
    [SerializeField] private Sprite _oneForkLineTop_Shadow_Spr;

    [SerializeField] private Sprite _oneForkLineBot_Spr;
    [SerializeField] private Sprite _oneForkLineBot_Shadow_Spr;

    [SerializeField] private Sprite _oneForkLineLeft_Spr;
    [SerializeField] private Sprite _oneForkLineLeft_Shadow_Spr;

    [SerializeField] private Sprite _oneForkLineRight_Spr;
    [SerializeField] private Sprite _oneForkLineRight_Shadow_Spr;

    [Header("SPRITE SHADOW CHANGE BY BESIDE")] [SerializeField]
    private Sprite _outLeftSprite;

    [SerializeField] private Sprite _oneHorizontalSprite,
        _inBotRightSprite,
        _inBotLeftSprite,
        _oneVerticalTopRightSprite,
        _verticalTopRightSprite,
        _outRightSprite,
        _oneVerticalSprite,
        _oneBotLeftTopRightSprite;

    [SerializeField] private Sprite _verticalBotLeftSprite;
    [SerializeField] private Sprite _oneVerticalBotRightSprite;
    [SerializeField] private Sprite _oneTopLeftBotRightSprite;
    private MapManager _mapManager;
    private BrickTile _brick;

    [Header("Amount HEALTH")] [SerializeField]
    private int _healthDefault = 100;

    public int _curHealth;


    private void Awake()
    {
        _curHealth = _healthDefault;
      /*  this.ObserveEveryValueChanged(_ => _curHealth).Subscribe(_ =>
        {
            Debug.Log("Change _current Health " +transform.name +" = "+ _  );
        });*/
    }

    private void OnDisable()
    {
        DestroyBrick();
    }

    void DestroyBrick()
    {
     //   _mapManager.CrackBrick(_brick);
        _brick.cbUpdateNewPostionType -= UpdateNewPostionType;
    }

    public void Set(BrickTile brick, MapManager mapManager = null)
    {
        _mapManager = mapManager;
        _brick = brick;
        _brick.cbUpdateNewPostionType += UpdateNewPostionType;
        SetBrick(brick.PositionPositionTypeTile);
    }

    private void UpdateNewPostionType(BrickTile brick)
    {
//        Awake();
        _brick.PositionPositionTypeTile = brick.PositionPositionTypeTile;
        SetBrick(brick.PositionPositionTypeTile);
    }

    void SetBrick(PositionTypeTile PositionPositionTypeTile)
    {
        var pos = transform.position;

        switch (PositionPositionTypeTile)
        {
            case PositionTypeTile.Mid:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _mid_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _mid_Shadow_Spr;

                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OutTop:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _outTop_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _outTop_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(true);
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (int) ((pos.y - 0.5f) * -10);
                break;
            case PositionTypeTile.OutBot:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _outBot_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _outBot_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OutRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _outRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _outRight_Shadow_Spr;
                if (_brick.BotRight)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _inBotLeftSprite;
                }
                else
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _outRightSprite;
                }

                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OutLeft:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _outLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _outLeft_Shadow_Spr;
                if (_brick.Botleft)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _inBotRightSprite;
                }
                else
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _outLeftSprite;
                }

                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OutBotLeft:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _outBotLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _outBotLeft_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OutBotRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _outBotRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _outBotRight_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OutTopLeft:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _outTopLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _outTopLeft_Shadow_Spr;
                if (_brick.Botleft)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _inBotRightSprite;
                }
                else
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _outLeftSprite;
                }

                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(true);
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (int) ((pos.y - 0.5f) * -10);
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                break;
            case PositionTypeTile.OutTopRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _outTopRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _outTopRight_Shadow_Spr;

                if (_brick.BotRight)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _inBotLeftSprite;
                }
                else
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _outRightSprite;
                }

                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (int) ((pos.y - 0.5f) * -10);

                _mainSprite.GetChild(0).gameObject.SetActive(true);
                break;
            case PositionTypeTile.InBotLeft:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _inBotLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _inBotLeft_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.InBotRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _inBotRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _inBotRight_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.InTopLeft:

                _mainSprite.GetComponent<SpriteRenderer>().sprite = _inTopLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _inTopLeft_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.InTopRight:

                _mainSprite.GetComponent<SpriteRenderer>().sprite = _inTopRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _inTopRight_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (int) ((pos.y - 0.5f) * -10);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;


            case PositionTypeTile.OneBot:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneBot_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneBot_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneLeft:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneLeft_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(true);
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (int) ((pos.y - .5f) * -10);
                break;
            case PositionTypeTile.OneRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneRight_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(true);
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (int) ((pos.y - 0.5f) * -10);
                break;
            case PositionTypeTile.OneTop:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneTop_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneTop_Shadow_Spr;
                if (!_brick.Botleft && !_brick.BotRight)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneVerticalSprite;
                }

                if (_brick.Botleft && !_brick.BotRight)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneVerticalTopRightSprite;
                }

                if (!_brick.Botleft && _brick.BotRight)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _verticalTopRightSprite;
                }

                if (_brick.Botleft && _brick.BotRight)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneHorizontalSprite;
                }

                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(true);
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (int) ((pos.y - 0.5f) * -10);
                break;
            case PositionTypeTile.OneMid:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneMid_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneMid_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(true);
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (int) ((pos.y - 0.5f) * -10);
                break;


            case PositionTypeTile.OneHorizontal:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneHorizontal_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneHorizontal_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(true);
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (int) ((pos.y - 0.5f) * -10);
                break;
            case PositionTypeTile.OneVertical:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneVertical_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneVertical_Shadow_Spr;
                if (!_brick.Botleft && !_brick.BotRight)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneVerticalSprite;
                }

                if (_brick.Botleft && !_brick.BotRight)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneVerticalTopRightSprite;
                }

                if (!_brick.Botleft && _brick.BotRight)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _verticalTopRightSprite;
                }

                if (_brick.Botleft && _brick.BotRight)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneHorizontalSprite;
                }

                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneHorizontalBotLeft:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneHorizontalBotLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneHorizontalBotLeft_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneVerticalBotLeft:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneVerticalBotLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneVerticalBotLeft_Shadow_Spr;
                if (!_brick.Botleft)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _verticalBotLeftSprite;
                }
                else
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneBotLeftTopRightSprite;
                }


                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneHorizontalBotRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneHorizontalBotRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneHorizontalBotRight_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneVerticalBotRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneVerticalBotRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneVerticalBotRight_Shadow_Spr;
                if (_brick.BotRight)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneTopLeftBotRightSprite;
                }
                else
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneVerticalBotRightSprite;
                }

                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneHorizontalTopLeft:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneHorizontalTopLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneHorizontalTopLeft_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(true);
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder =
                    (int) ((pos.y - 0.5f) * -10);
                break;
            case PositionTypeTile.OneVerticalTopLeft:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneVerticalTopLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneVerticalTopLeft_Shadow_Spr;
                if (_brick.Botleft)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _inBotRightSprite;
                }
                else
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _outLeftSprite;
                }

                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneHorizontalTopRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneHorizontalTopRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneHorizontalTopRight_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(true);
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder =
                    (int) ((pos.y - 0.5f) * -10);
                break;
            case PositionTypeTile.OneVerticalTopRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneVerticalTopRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneVerticalTopRight_Shadow_Spr;

                if (_brick.BotRight)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _inBotLeftSprite;
                }
                else
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _outRightSprite;
                }

                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;


            case PositionTypeTile.OneBotLeft:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneBotLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneBotLeft_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneBotRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneBotRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneBotRight_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneTopLeft:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneTopLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneTopLeft_Shadow_Spr;

                if (_brick.Botleft)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneBotLeftTopRightSprite;
                }
                else
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _verticalBotLeftSprite;
                }

                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(true);
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (int) ((pos.y - 0.5f) * -10);
                break;
            case PositionTypeTile.OneTopRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneTopRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneTopRight_Shadow_Spr;
                if (_brick.BotRight)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneTopLeftBotRightSprite;
                }
                else
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneVerticalBotRightSprite;
                }

                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(true);
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (int) ((pos.y - 0.5f) * -10);
                break;
            case PositionTypeTile.OneBotLeftTopRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneBotLeftTopRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneBotLeftTopRight_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneTopLeftBotRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneTopLeftBotRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneTopLeftBotRight_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;


            case PositionTypeTile.OneCrossLine:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneCrossLine_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneCrossLine_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneCrossLineTop:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneCrossLineTop_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneCrossLineTop_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneCrossLineBot:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneCrossLineBot_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneCrossLineBot_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneCrossLineLeft:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneCrossLineLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneCrossLineLeft_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneCrossLineRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneCrossLineRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneCrossLineRight_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneCrossLineBotLeft:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneCrossLineBotLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneCrossLineBotLeft_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneCrossLineBotRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneCrossLineBotRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneCrossLineBotRight_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneCrossLineTopLeft:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneCrossLineTopLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneCrossLineTopLeft_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneCrossLineTopRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneCrossLineTopRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneCrossLineTopRight_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;

            case PositionTypeTile.OneForkLineBot:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneForkLineBot_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneForkLineBot_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneForkLineLeft:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneForkLineLeft_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneForkLineLeft_Shadow_Spr;
                if (_brick.Botleft)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneBotLeftTopRightSprite;
                }
                else
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _verticalBotLeftSprite;
                }

                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneForkLineRight:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneForkLineRight_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneForkLineRight_Shadow_Spr;
                if (_brick.BotRight)
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneTopLeftBotRightSprite;
                }
                else
                {
                    _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneVerticalBotRightSprite;
                }

                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(false);
                break;
            case PositionTypeTile.OneForkLineTop:
                _mainSprite.GetComponent<SpriteRenderer>().sprite = _oneForkLineTop_Spr;
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sprite = _oneForkLineTop_Shadow_Spr;
                _mainSprite.GetComponent<SpriteRenderer>().sortingOrder = (int) (pos.y * -10);
                _mainSprite.gameObject.SetActive(true);
                _mainSprite.GetChild(0).gameObject.SetActive(true);
                _mainSprite.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = (int) ((pos.y - 0.5f) * -10);
                break;
        }
    }

}