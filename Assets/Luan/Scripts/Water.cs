using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private WaterTile _water;
    [SerializeField] private SpriteRenderer _icon;

    [SerializeField] public Sprite _outBot,
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

    public void Set(WaterTile water)
    {
        _water = water;
        switch (water.PositionPositionTypeTile)
        {
            case PositionTypeTile.Mid:
                _icon.sprite = _mid;
                break;
            case PositionTypeTile.OutTop:
                _icon.sprite = _outTop;
                break;
            case PositionTypeTile.OutBot:
                _icon.sprite = _outBot;
                break;
            case PositionTypeTile.OutRight:
                _icon.sprite = _outRight;
                break;
            case PositionTypeTile.OutLeft:
                _icon.sprite = _outLeft;
                break;
            case PositionTypeTile.OutBotLeft:
                _icon.sprite = _outBotLeft;
                break;
            case PositionTypeTile.OutBotRight:
                _icon.sprite = _outBotRight;
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
                break;
            case PositionTypeTile.OneLeft:
                _icon.sprite = OneLeft;
                break;
            case PositionTypeTile.OneRight:
                _icon.sprite = OneRight;
                break;
            case PositionTypeTile.OneTop:
                _icon.sprite = OneTop;
                break;
            case PositionTypeTile.OneMid:
                _icon.sprite = OneMid;
                break;


            case PositionTypeTile.OneHorizontal:
                _icon.sprite = OneHorizontal;
                break;
            case PositionTypeTile.OneVertical:
                _icon.sprite = OneVertical;
                break;
            case PositionTypeTile.OneHorizontalBotLeft:
                _icon.sprite = OneHorizontalBotLeft;
                break;
            case PositionTypeTile.OneVerticalBotLeft:
                _icon.sprite = OneVerticalBotLeft;
                break;
            case PositionTypeTile.OneHorizontalBotRight:
                _icon.sprite = OneHorizontalBotRight;
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
                break;
            case PositionTypeTile.OneBotRight:
                _icon.sprite = OneBotRight;
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