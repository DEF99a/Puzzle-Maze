using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GroundBasic2 : MonoBehaviour
{
    [SerializeField] private GroundBasic2Tile _tile;
    [SerializeField] private SpriteRenderer _icon;
    [SerializeField] private SpriteRenderer _shadow;

    [SerializeField] private Sprite _outBot,
        _outRight,
        _outLeft,
        _outTop,
        _outTopLeft,
        _outTopRight,
        _outBotLeft,
        _outBotRight,
        _mid,
        _inTopLeft,
        _inTopRight,
        _inBotLeft,
        _inBotRight,
        OneMid,
        OneBot,
        OneLeft,
        OneRight,
        OneTop,
        OneBotLeft,
        OneBotRight,
        OneTopRight,
        OneTopLeft,
        OneVertical,
        OneHorizontal,
        OneHorizontalBotLeft,
        OneHorizontalBotRight,
        OneHorizontalTopRight,
        OneHorizontalTopLeft,
        OneVerticalBotLeft,
        OneVerticalBotRight,
        OneVerticalTopRight,
        OneVerticalTopLeft,
        OneCrossLine,
        OneCrossLineLeft,
        OneCrossLineRight,
        OneCrossLineTop,
        OneCrossLineBot,
        OneCrossLineTopLeft,
        OneCrossLineTopRight,
        OneCrossLineBotLeft,
        OneCrossLineBotRight,
        OneBotLeftTopRight,
        OneTopLeftBotRight,
        OneForkLineTop,
        OneForkLineLeft,
        OneForkLineRight,
        OneForkLineBot;

    //--------
    [SerializeField]
    private Sprite _shadowBotLeft,
        _shadowBotRight,
        _shadowMid,
        _shadowOneMid;

    public void Set(GroundBasic2Tile tile)
    {
        _tile = tile;
        _shadow.gameObject.SetActive(false);
        //_icon.sortingOrder = (int)(transform.position.y * -10);
        //_shadow.sortingOrder = _icon.sortingOrder + 1;
        var pos = _icon.transform.position;
        pos.z = pos.y / 1000;
        _icon.transform.position = pos;

        pos = _shadow.transform.position;
        pos.z = pos.y / 1000;
        _shadow.transform.position = pos;

        switch (tile.PositionPositionTypeTile)
        {
            case PositionTypeTile.Mid:
                _icon.sprite = _mid;
                break;
            case PositionTypeTile.OutTop:
                _icon.sprite = _outTop;
                break;
            case PositionTypeTile.OutBot:
                _icon.sprite = _outBot;
                _shadow.gameObject.SetActive(true);
                _shadow.sprite = _shadowMid;
                break;
            case PositionTypeTile.OutRight:
                _icon.sprite = _outRight;
                break;
            case PositionTypeTile.OutLeft:
                _icon.sprite = _outLeft;
                break;
            case PositionTypeTile.OutBotLeft:
                _icon.sprite = _outBotLeft;
                _shadow.gameObject.SetActive(true);
                _shadow.sprite = _shadowBotLeft;

                break;
            case PositionTypeTile.OutBotRight:
                _icon.sprite = _outBotRight;
                _shadow.gameObject.SetActive(true);
                _shadow.sprite = _shadowBotRight;

                break;
            case PositionTypeTile.OutTopLeft:
                _icon.sprite = _outTopLeft;
                break;
            case PositionTypeTile.OutTopRight:
                _icon.sprite = _outTopRight;
                break;
            case PositionTypeTile.InBotLeft:
                _icon.sprite = _inBotLeft;
                break;
            case PositionTypeTile.InBotRight:
                _icon.sprite = _inBotRight;
                break;
            case PositionTypeTile.InTopLeft:
                _icon.sprite = _inTopLeft;
                break;
            case PositionTypeTile.InTopRight:
                _icon.sprite = _inTopRight;
                break;


            case PositionTypeTile.OneBot:
                _icon.sprite = OneBot;
                _shadow.gameObject.SetActive(true);
                _shadow.sprite = _shadowOneMid;
                break;
            case PositionTypeTile.OneLeft:
                _icon.sprite = OneLeft;
                _shadow.gameObject.SetActive(true);
                _shadow.sprite = _shadowBotLeft;

                break;
            case PositionTypeTile.OneRight:
                _icon.sprite = OneRight;
                _shadow.gameObject.SetActive(true);
                _shadow.sprite = _shadowBotRight;
                break;
            case PositionTypeTile.OneTop:
                _icon.sprite = OneTop;
                break;
            case PositionTypeTile.OneMid:
                _icon.sprite = OneMid;
                _shadow.gameObject.SetActive(true);
                _shadow.sprite = _shadowOneMid;
                break;


            case PositionTypeTile.OneHorizontal:
                _icon.sprite = OneHorizontal;
 		_shadow.gameObject.SetActive(true);
                _shadow.sprite = _shadowMid;
                break;
            case PositionTypeTile.OneVertical:
                _icon.sprite = OneVertical;
                break;
            case PositionTypeTile.OneHorizontalBotLeft:
                _icon.sprite = OneHorizontalBotLeft;
                _shadow.gameObject.SetActive(true);
                _shadow.sprite = _shadowMid;
                break;
            case PositionTypeTile.OneVerticalBotLeft:
                _icon.sprite = OneVerticalBotLeft;
          

                break;
            case PositionTypeTile.OneHorizontalBotRight:
                _icon.sprite = OneHorizontalBotRight;
                _shadow.gameObject.SetActive(true);
                _shadow.sprite = _shadowMid;
                break;
            case PositionTypeTile.OneVerticalBotRight:
                _icon.sprite = OneVerticalBotRight;
                break;
            case PositionTypeTile.OneHorizontalTopLeft:
                _icon.sprite = OneHorizontalTopLeft;
                break;
            case PositionTypeTile.OneVerticalTopLeft:
                _icon.sprite = OneVerticalTopLeft;
                break;
            case PositionTypeTile.OneHorizontalTopRight:
                _icon.sprite = OneHorizontalTopRight;
                break;
            case PositionTypeTile.OneVerticalTopRight:
                _icon.sprite = OneVerticalTopRight;
                break;


            case PositionTypeTile.OneBotLeft:
                _icon.sprite = OneBotLeft;
                _shadow.gameObject.SetActive(true);
                _shadow.sprite = _shadowBotLeft;

                break;
            case PositionTypeTile.OneBotRight:
                _icon.sprite = OneBotRight;
                _shadow.gameObject.SetActive(true);
                _shadow.sprite = _shadowBotRight;
                break;
            case PositionTypeTile.OneTopLeft:
                _icon.sprite = OneTopLeft;
                break;
            case PositionTypeTile.OneTopRight:
                _icon.sprite = OneTopRight;
                break;
            case PositionTypeTile.OneBotLeftTopRight:
                _icon.sprite = OneBotLeftTopRight;
                break;
            case PositionTypeTile.OneTopLeftBotRight:
                _icon.sprite = OneTopLeftBotRight;
                break;


            case PositionTypeTile.OneCrossLine:
                _icon.sprite = OneCrossLine;
                break;
            case PositionTypeTile.OneCrossLineTop:
                _icon.sprite = OneCrossLineTop;
                break;
            case PositionTypeTile.OneCrossLineBot:
                _icon.sprite = OneCrossLineBot;
                break;
            case PositionTypeTile.OneCrossLineLeft:
                _icon.sprite = OneCrossLineLeft;
                break;
            case PositionTypeTile.OneCrossLineRight:
                _icon.sprite = OneCrossLineRight;
                break;
            case PositionTypeTile.OneCrossLineBotLeft:
                _icon.sprite = OneCrossLineBotLeft;
                break;
            case PositionTypeTile.OneCrossLineBotRight:
                _icon.sprite = OneCrossLineBotRight;
                break;
            case PositionTypeTile.OneCrossLineTopLeft:
                _icon.sprite = OneCrossLineTopLeft;
                break;
            case PositionTypeTile.OneCrossLineTopRight:
                _icon.sprite = OneCrossLineTopRight;
                break;


            case PositionTypeTile.OneForkLineBot:
                _icon.sprite = OneForkLineBot;
                _shadow.gameObject.SetActive(true);
                _shadow.sprite = _shadowMid;

                break;
            case PositionTypeTile.OneForkLineLeft:
                _icon.sprite = OneForkLineLeft;
                break;
            case PositionTypeTile.OneForkLineRight:
                _icon.sprite = OneForkLineRight;
                break;
            case PositionTypeTile.OneForkLineTop:
                _icon.sprite = OneForkLineTop;
                break;
        }

        // transform.position = water.Pos;
    }
}