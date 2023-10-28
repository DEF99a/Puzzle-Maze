using UnityEngine;
using UnityEngine.Serialization;

public enum PositionTypeTile
{
    InTopRight,
    InTopLeft,
    InBotRight,
    InBotLeft,
    OutBot,
    OutLeft,
    OutRight,
    OutTop,
    OutTopRight,
    OutTopLeft,
    OutBotRight,
    OutBotLeft,
    Mid,

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
    OneForkLineBot,
    NONE
}

public class TileData
{
    public static void SetPositionType(int[,] arounds, int x, int y, int xMax, int yMax,
        out PositionTypeTile positionPositionTypeTile, out bool botRightOut, out bool botLeftOut
        , out bool topRightOut, out bool topLeftOut, out bool rightOut, out bool leftOut, out bool topOut,
        out bool botOut)
    {
        // mid
        var mid = arounds[x, y];
        var botLeft = x - 1 < xMax && y - 1 < yMax && x - 1 >= 0 && y - 1 >= 0 ? arounds[x - 1, y - 1] : 0;
        var botRight = x + 1 < xMax && y - 1 < yMax && x + 1 >= 0 && y - 1 >= 0 ? arounds[x + 1, y - 1] : 0;
        var bot = x < xMax && y - 1 < yMax && x >= 0 && y - 1 >= 0 ? arounds[x, y - 1] : 0;
        var left = x - 1 < xMax && y < yMax && x - 1 >= 0 && y >= 0 ? arounds[x - 1, y] : 0;
        var right = x + 1 < xMax && y < yMax && x + 1 >= 0 && y >= 0 ? arounds[x + 1, y] : 0;
        var topLeft = x - 1 < xMax && y + 1 < yMax && x - 1 >= 0 && y + 1 >= 0 ? arounds[x - 1, y + 1] : 0;
        var top = x < xMax && y + 1 < yMax && x >= 0 && y + 1 >= 0 ? arounds[x, y + 1] : 0;
        var topRight = x + 1 < xMax && y + 1 < yMax && x + 1 >= 0 && y + 1 >= 0 ? arounds[x + 1, y + 1] : 0;

        botRightOut = botRight == 1;
        botLeftOut = botLeft == 1;
        topRightOut = topRight == 1;
        topLeftOut = topLeft == 1;
        leftOut = left == 1;
        rightOut = right == 1;
        topOut = top == 1;
        botOut = bot == 1;

        var total = topLeft + top + topRight +
                    left + mid + right +
                    botLeft + bot + botRight;

        if (total == 9)
        {
            positionPositionTypeTile = PositionTypeTile.Mid;
            return;
        }

        if (total == 8)
        {
            positionPositionTypeTile =
                TileData.Equal_8(mid, bot, right, top, left, botLeft, botRight, topLeft, topRight);
            if (positionPositionTypeTile != PositionTypeTile.NONE)
                return;
        }

        if (total == 7)
        {
            positionPositionTypeTile =
                TileData.Equal_7(mid, bot, right, top, left, botLeft, botRight, topLeft, topRight);
            if (positionPositionTypeTile != PositionTypeTile.NONE)
                return;
        }

        if (total == 6)
        {
            positionPositionTypeTile =
                TileData.Equal_6(mid, bot, right, top, left, botLeft, botRight, topLeft, topRight);
            if (positionPositionTypeTile != PositionTypeTile.NONE)
                return;
        }


        if (total == 5)
        {
            positionPositionTypeTile =
                TileData.Equal_5(mid, bot, right, top, left, botLeft, botRight, topLeft, topRight);
            if (positionPositionTypeTile != PositionTypeTile.NONE)
                return;
        }

        if (total == 4)
        {
            positionPositionTypeTile =
                TileData.Equal_4(mid, bot, right, top, left, botLeft, botRight, topLeft, topRight);
            if (positionPositionTypeTile != PositionTypeTile.NONE)
                return;
        }

        if (total == 3)
        {
            positionPositionTypeTile =
                TileData.Equal_3(mid, bot, right, top, left, botLeft, botRight, topLeft, topRight);
            if (positionPositionTypeTile != PositionTypeTile.NONE)
                return;
        }

        if (total == 2)
        {
            positionPositionTypeTile =
                TileData.Equal_2(mid, bot, right, top, left, botLeft, botRight, topLeft, topRight);
            if (positionPositionTypeTile != PositionTypeTile.NONE)
                return;
        }

        if (total == 1)
        {
            positionPositionTypeTile = PositionTypeTile.OneMid;
            return;
        }

        positionPositionTypeTile = PositionTypeTile.NONE;
        Debug.LogError("Wrong Tile Water");
        return;
    }

    public static PositionTypeTile Equal_8(int mid, int bot, int right, int top, int left, int botLeft, int botRight,
        int topLeft,
        int topRight)
    {
        //---------
        if (topLeft == 0)
        {
            return PositionTypeTile.InBotRight;
        }

        if (topRight == 0)
        {
            return PositionTypeTile.InBotLeft;
        }

        if (botLeft == 0)
        {
            return PositionTypeTile.InTopRight;
        }

        if (botRight == 0)
        {
            return PositionTypeTile.InTopLeft;
        }

        //---------
        if (top == 0)
        {
            return PositionTypeTile.OutTop;
        }

        if (bot == 0)
        {
            return PositionTypeTile.OutBot;
        }

        if (right == 0)
        {
            return PositionTypeTile.OutRight;
        }

        if (left == 0)
        {
            return PositionTypeTile.OutLeft;
        }

        return PositionTypeTile.NONE;
    }

    public static PositionTypeTile Equal_7(int mid, int bot, int right, int top, int left, int botLeft, int botRight,
        int topLeft,
        int topRight)
    {
        if (botRight == 0 && topRight == 0)
        {
            return PositionTypeTile.OneCrossLineLeft;
        }

        if (topLeft == 0 && botLeft == 0)
        {
            return PositionTypeTile.OneCrossLineRight;
        }

        if (topLeft == 0 && topRight == 0)
        {
            return PositionTypeTile.OneCrossLineBot;
        }

        if (botLeft == 0 && botRight == 0)
        {
            return PositionTypeTile.OneCrossLineTop;
        }

        //------------
        if (bot == 0)
        {
            if (topLeft == 0)
            {
                return PositionTypeTile.OneHorizontalBotLeft;
            }

            if (topRight == 0)
            {
                return PositionTypeTile.OneHorizontalBotRight;
            }
        }

        if (left == 0)
        {
            if (topRight == 0)
            {
                return PositionTypeTile.OneVerticalTopLeft;
            }

            if (botRight == 0)
            {
                return PositionTypeTile.OneVerticalBotLeft;
            }
        }

        if (top == 0)
        {
            if (botLeft == 0)
            {
                return PositionTypeTile.OneHorizontalTopLeft;
            }

            if (botRight == 0)
            {
                return PositionTypeTile.OneHorizontalTopRight;
            }
        }

        if (right == 0)
        {
            if (botLeft == 0)
            {
                return PositionTypeTile.OneVerticalBotRight;
            }

            if (topLeft == 0)
            {
                return PositionTypeTile.OneVerticalTopRight;
            }
        }

//----------------
        if (top + right == 0 && topRight == 1)
        {
            return PositionTypeTile.OutTopRight;
        }

        if (bot + right == 0 && botRight == 1)
        {
            return PositionTypeTile.OutBotRight;
        }

        if (bot + left == 0 && botLeft == 1)
        {
            return PositionTypeTile.OutBotLeft;
        }

        if (top + left == 0 && topLeft == 1)
        {
            return PositionTypeTile.OutTopLeft;
        }

//--------------------
        if (top + bot == 0)
        {
            return PositionTypeTile.OneHorizontal;
        }

        if (right + left == 0)
        {
            return PositionTypeTile.OneVertical;
        }

        if (botLeft + topRight == 0)
        {
            return PositionTypeTile.OneTopLeftBotRight;
        }

        if (topLeft + botRight == 0)
        {
            return PositionTypeTile.OneBotLeftTopRight;
        }
//-------------

        if (top == 0 && (topLeft == 0 || topRight == 0))
        {
            return PositionTypeTile.OutTop;
        }

        if (bot == 0 && (botRight == 0 || botLeft == 0))
        {
            return PositionTypeTile.OutBot;
        }

        if (right == 0 && (topRight == 0 || botRight == 0))
        {
            return PositionTypeTile.OutRight;
        }

        if (left == 0 && (botLeft == 0 || topLeft == 0))
        {
            return PositionTypeTile.OutLeft;
        }

        return PositionTypeTile.NONE;
    }

    public static PositionTypeTile Equal_6(int mid, int bot, int right, int top, int left, int botLeft, int botRight,
        int topLeft,
        int topRight)
    {
        //-----------
        if (topLeft == 0 && top == 0 && topRight == 0)
        {
            return PositionTypeTile.OutTop;
        }

        if (botLeft == 0 && bot == 0 && botRight == 0)
        {
            return PositionTypeTile.OutBot;
        }

        if (topLeft == 0 && left == 0 && botLeft == 0)
        {
            return PositionTypeTile.OutLeft;
        }

        if (right == 0 && botRight == 0 && topRight == 0)
        {
            return PositionTypeTile.OutRight;
        }

//-------
        if (right == 0 && top == 0 && (topRight == 0 || botRight == 0 || topLeft == 0))
        {
            return PositionTypeTile.OutTopRight;
        }

        if (left == 0 && top == 0 && (topLeft == 0 || topRight == 0 || botLeft == 0))
        {
            return PositionTypeTile.OutTopLeft;
        }

        if (left == 0 && bot == 0 && (botLeft == 0 || topLeft == 0 || botRight == 0))
        {
            return PositionTypeTile.OutBotLeft;
        }

        if (right == 0 && bot == 0 && (botRight == 0 || topRight == 0 || botLeft == 0))
        {
            return PositionTypeTile.OutBotRight;
        }

        //-------
        if (right == 0 && top == 0 && botLeft == 0)
        {
            return PositionTypeTile.OneTopRight;
        }

        if (left == 0 && top == 0 && botRight == 0)
        {
            return PositionTypeTile.OneTopLeft;
        }

        if (left == 0 && bot == 0 && topRight == 0)
        {
            return PositionTypeTile.OneBotLeft;
        }

        if (right == 0 && bot == 0 && topLeft == 0)
        {
            return PositionTypeTile.OneBotRight;
        }


        //-------
        if (botLeft == 0 && botRight == 0 && topRight == 0)
        {
            return PositionTypeTile.OneCrossLineTopLeft;
        }

        if (topLeft == 0 && botRight == 0 && topRight == 0)
        {
            return PositionTypeTile.OneCrossLineBotLeft;
        }

        if (topLeft == 0 && botLeft == 0 && topRight == 0)
        {
            return PositionTypeTile.OneCrossLineBotRight;
        }

        if (topLeft == 0 && botLeft == 0 && botRight == 0)
        {
            return PositionTypeTile.OneCrossLineTopRight;
        }

        // ------
        if (top == 0 && right == 0 && bot == 0)
        {
            return PositionTypeTile.OneRight;
        }

        if (top == 0 && left == 0 && bot == 0)
        {
            return PositionTypeTile.OneLeft;
        }

        if (right == 0 && left == 0 && bot == 0)
        {
            return PositionTypeTile.OneBot;
        }

        if (right == 0 && left == 0 && top == 0)
        {
            return PositionTypeTile.OneTop;
        }

        // ---------
        if (bot == 0 && (botRight == 0 || botLeft == 0))
        {
            if (topLeft == 0)
            {
                return PositionTypeTile.OneHorizontalBotLeft;
            }

            if (topRight == 0)
            {
                return PositionTypeTile.OneHorizontalBotRight;
            }
        }

        if (left == 0 && (topLeft == 0 || botLeft == 0))
        {
            if (topRight == 0)
            {
                return PositionTypeTile.OneVerticalTopLeft;
            }

            if (botRight == 0)
            {
                return PositionTypeTile.OneVerticalBotLeft;
            }
        }

        if (top == 0 && (topLeft == 0 || topRight == 0))
        {
            if (botLeft == 0)
            {
                return PositionTypeTile.OneHorizontalTopLeft;
            }

            if (botRight == 0)
            {
                return PositionTypeTile.OneHorizontalTopRight;
            }
        }

        if (right == 0 && (topRight == 0 || botRight == 0))
        {
            if (botLeft == 0)
            {
                return PositionTypeTile.OneVerticalBotRight;
            }

            if (topLeft == 0)
            {
                return PositionTypeTile.OneVerticalTopRight;
            }
        }

        //--------
        if (top == 0 && botRight == 0 && botLeft == 0)
        {
            return PositionTypeTile.OneForkLineTop;
        }

        if (bot == 0 && topLeft == 0 && topRight == 0)
        {
            return PositionTypeTile.OneForkLineBot;
        }

        if (left == 0 && topRight == 0 && botRight == 0)
        {
            return PositionTypeTile.OneForkLineLeft;
        }

        if (right == 0 && topLeft == 0 && botLeft == 0)
        {
            return PositionTypeTile.OneForkLineRight;
        }

        //---------------
        if (right == 0 && left == 0 &&
            (topLeft == 0 || topRight == 0 ||
             botLeft == 0 || botRight == 0))
        {
            return PositionTypeTile.OneVertical;
        }

        if (top == 0 && bot == 0 &&
            (topLeft == 0 || topRight == 0 ||
             botLeft == 0 || botRight == 0))
        {
            return PositionTypeTile.OneHorizontal;
        }

        return PositionTypeTile.NONE;
    }

    public static PositionTypeTile Equal_5(int mid, int bot, int right, int top, int left, int botLeft, int botRight,
        int topLeft,
        int topRight)
    {
        //----------   

        if (bot == 0 && left == 0 && (botLeft == 0 && topLeft == 0 ||
                                      botLeft == 0 && botRight == 0 ||
                                      topLeft == 0 && botRight == 0))
        {
            return PositionTypeTile.OutBotLeft;
        }

        if (bot == 0 && right == 0 && (botRight == 0 && topRight == 0 ||
                                       botRight == 0 && botLeft == 0 ||
                                       topRight == 0 && botLeft == 0))
        {
            return PositionTypeTile.OutBotRight;
        }

        if (right == 0 && top == 0 && (topRight == 0 && topLeft == 0 ||
                                       topRight == 0 && botRight == 0 ||
                                       topLeft == 0 && botRight == 0))
        {
            return PositionTypeTile.OutTopRight;
        }

        if (left == 0 && top == 0 && (topLeft == 0 && topRight == 0 ||
                                      topLeft == 0 && botLeft == 0 ||
                                      topRight == 0 && botLeft == 0))
        {
            return PositionTypeTile.OutTopLeft;
        }

        //----------   

        if (topRight == 0 && botRight == 0 && topLeft == 0 && botLeft == 0)
        {
            return PositionTypeTile.OneCrossLine;
        }

        if (top == 0 && bot == 0 && left == 0 && right == 0)
        {
            return PositionTypeTile.OneMid;
        }
        //----------   

        if ((topLeft == 0 || botLeft == 0 || topRight == 0 || botRight == 0) &&
            top == 0 && bot == 0 && right == 0)
        {
            return PositionTypeTile.OneRight;
        }

        if ((topLeft == 0 || botLeft == 0 || topRight == 0 || botRight == 0) &&
            top == 0 && bot == 0 && left == 0)
        {
            return PositionTypeTile.OneLeft;
        }

        if ((topLeft == 0 || botLeft == 0 || topRight == 0 || botRight == 0) &&
            top == 0 && right == 0 && left == 0)
        {
            return PositionTypeTile.OneTop;
        }

        if ((topLeft == 0 || botLeft == 0 || topRight == 0 || botRight == 0) &&
            right == 0 && bot == 0 && left == 0)
        {
            return PositionTypeTile.OneBot;
        }

        ///-----------
        if (bot == 0 && botRight == 0 && botLeft == 0)
        {
            if (topLeft == 0)
            {
                return PositionTypeTile.OneHorizontalBotLeft;
            }

            if (topRight == 0)
            {
                return PositionTypeTile.OneHorizontalBotRight;
            }
        }

        if (left == 0 && topLeft == 0 && botLeft == 0)
        {
            if (topRight == 0)
            {
                return PositionTypeTile.OneVerticalTopLeft;
            }

            if (botRight == 0)
            {
                return PositionTypeTile.OneVerticalBotLeft;
            }
        }

        if (top == 0 && topLeft == 0 && topRight == 0)
        {
            if (botLeft == 0)
            {
                return PositionTypeTile.OneHorizontalTopLeft;
            }

            if (botRight == 0)
            {
                return PositionTypeTile.OneHorizontalTopRight;
            }
        }

        if (right == 0 && topRight == 0 && botRight == 0)
        {
            if (botLeft == 0)
            {
                return PositionTypeTile.OneVerticalBotRight;
            }

            if (topLeft == 0)
            {
                return PositionTypeTile.OneVerticalTopRight;
            }
        }

        //---------------

        if ((topLeft == 0 || topRight == 0 || botLeft == 0) && top == 0 && left == 0 && botRight == 0)
        {
            return PositionTypeTile.OneTopLeft;
        }

        if ((botLeft == 0 || topRight == 0 || botRight == 0) && bot == 0 && right == 0 && topLeft == 0)
        {
            return PositionTypeTile.OneBotRight;
        }

        if ((botLeft == 0 || botRight == 0 || topLeft == 0) && bot == 0 && left == 0 && topRight == 0)
        {
            return PositionTypeTile.OneBotLeft;
        }

        if ((topRight == 0 || topLeft == 0 || botRight == 0) && top == 0 && right == 0 && botLeft == 0)
        {
            return PositionTypeTile.OneTopRight;
        }

        //----------

        if (right == 0 && left == 0 &&
            (topLeft == 0 && topRight == 0 ||
             botLeft == 0 && botRight == 0 ||
             topLeft == 0 && botRight == 0 ||
             botLeft == 0 && topRight == 0 ||
             topLeft == 0 && botLeft == 0 ||
             topRight == 0 && botRight == 0))
        {
            return PositionTypeTile.OneVertical;
        }

        if (top == 0 && bot == 0 &&
            (topLeft == 0 && topRight == 0 ||
             botLeft == 0 && botRight == 0 ||
             topLeft == 0 && botRight == 0 ||
             botLeft == 0 && topRight == 0 ||
             topLeft == 0 && botLeft == 0 ||
             topRight == 0 && botRight == 0))
        {
            return PositionTypeTile.OneHorizontal;
        }
        // ---------------


        if (right == 0 && topLeft == 0 && botLeft == 0 && (botRight == 0 || topRight == 0))
        {
            return PositionTypeTile.OneForkLineRight;
        }

        if (left == 0 && topRight == 0 && botRight == 0 && (topLeft == 0 || botLeft == 0))
        {
            return PositionTypeTile.OneForkLineLeft;
        }

        if (top == 0 && botLeft == 0 && botRight == 0 && (topLeft == 0 || topRight == 0))
        {
            return PositionTypeTile.OneForkLineTop;
        }

        if (bot == 0 && topLeft == 0 && topRight == 0 && (botLeft == 0 || botRight == 0))
        {
            return PositionTypeTile.OneForkLineBot;
        }

        return PositionTypeTile.NONE;
    }

    public static PositionTypeTile Equal_4(int mid, int bot, int right, int top, int left, int botLeft, int botRight,
        int topLeft,
        int topRight)
    {
        if (mid == 1 && top == 1 && topRight == 1 && right == 1)
        {
            return PositionTypeTile.OutBotLeft;
        }

        if (mid == 1 && top == 1 && topLeft == 1 && left == 1)
        {
            return PositionTypeTile.OutBotRight;
        }

        if (mid == 1 && left == 1 && botLeft == 1 && bot == 1)
        {
            return PositionTypeTile.OutTopRight;
        }

        if (mid == 1 && right == 1 && bot == 1 && botRight == 1)
        {
            return PositionTypeTile.OutTopLeft;
        }
        // mid

        if (mid == 1 && (topRight == 1 && botRight == 1 && botLeft == 1 ||
                         topLeft == 1 && botRight == 1 && botLeft == 1 ||
                         topLeft == 1 && topRight == 1 && botLeft == 1 ||
                         topLeft == 1 && topRight == 1 && botRight == 1))

        {
            return PositionTypeTile.OneMid;
        }

        // ------------

        if (mid == 1 && left == 1 && (topRight == 1 && botLeft == 1 ||
                                      topRight == 1 && botRight == 1 ||
                                      topRight == 1 && topLeft == 1 ||
                                      botRight == 1 && botLeft == 1 ||
                                      botRight == 1 && topLeft == 1 ||
                                      botLeft == 1 && topLeft == 1))
        {
            return PositionTypeTile.OneRight;
        }

        if (mid == 1 && right == 1 && (topRight == 1 && botLeft == 1 ||
                                       topRight == 1 && botRight == 1 ||
                                       topRight == 1 && topLeft == 1 ||
                                       botRight == 1 && botLeft == 1 ||
                                       botRight == 1 && topLeft == 1 ||
                                       botLeft == 1 && topLeft == 1))
        {
            return PositionTypeTile.OneLeft;
        }

        if (mid == 1 && bot == 1 && (topRight == 1 && botLeft == 1 ||
                                     topRight == 1 && botRight == 1 ||
                                     topRight == 1 && topLeft == 1 ||
                                     botRight == 1 && botLeft == 1 ||
                                     botRight == 1 && topLeft == 1 ||
                                     botLeft == 1 && topLeft == 1))
        {
            return PositionTypeTile.OneTop;
        }

        if (mid == 1 && top == 1 && (topRight == 1 && botLeft == 1 ||
                                     topRight == 1 && botRight == 1 ||
                                     topRight == 1 && topLeft == 1 ||
                                     botRight == 1 && botLeft == 1 ||
                                     botRight == 1 && topLeft == 1 ||
                                     botLeft == 1 && topLeft == 1))

        {
            return PositionTypeTile.OneBot;
        }

        // ------------
        if (mid == 1 && top == 1 && bot == 1 && left == 1)
        {
            return PositionTypeTile.OneForkLineRight;
        }

        if (mid == 1 && top == 1 && bot == 1 && right == 1)
        {
            return PositionTypeTile.OneForkLineLeft;
        }

        if (mid == 1 && left == 1 && bot == 1 && right == 1)
        {
            return PositionTypeTile.OneForkLineTop;
        }

        if (mid == 1 && left == 1 && top == 1 && right == 1)
        {
            return PositionTypeTile.OneForkLineBot;
        }

//-------------
        if (mid == 1 && left == 1 && right == 1 && (topLeft == 1 || topRight == 1 || botLeft == 1 || botRight == 1))
        {
            return PositionTypeTile.OneHorizontal;
        }

        if (mid == 1 && top == 1 && bot == 1 && (topLeft == 1 || topRight == 1 || botLeft == 1 || botRight == 1))
        {
            return PositionTypeTile.OneVertical;
        }

        //---------------
        if (mid == 1 && left == 1 && bot == 1 && (topRight == 1 || topLeft == 1 || botRight == 1))
        {
            return PositionTypeTile.OneTopRight;
        }

        if (mid == 1 && left == 1 && top == 1 && (topRight == 1 || botRight == 1 || botLeft == 1))
        {
            return PositionTypeTile.OneBotRight;
        }

        if (mid == 1 && right == 1 && top == 1 && (topLeft == 1 || botRight == 1 || botLeft == 1))
        {
            return PositionTypeTile.OneBotLeft;
        }

        if (mid == 1 && right == 1 && bot == 1 && (topRight == 1 || topLeft == 1 || botLeft == 1))
        {
            return PositionTypeTile.OneTopLeft;
        }

        return PositionTypeTile.NONE;
    }

    public static PositionTypeTile Equal_3(int mid, int bot, int right, int top, int left, int botLeft, int botRight,
        int topLeft,
        int topRight)
    {
        if (mid == 1 && (topRight == 1 && botLeft == 1 ||
                         topLeft == 1 && botRight == 1 ||
                         topRight == 1 && topLeft == 1 ||
                         botRight == 1 && botLeft == 1 ||
                         topRight == 1 && botLeft == 1 ||
                         topLeft == 1 && botRight == 1 ||
                         topRight == 1 && botRight == 1 ||
                         topLeft == 1 && botLeft == 1))

        {
            return PositionTypeTile.OneMid;
        }

        // ------------

        if (mid == 1 && left == 1 && (topRight == 1 || botLeft == 1 ||
                                      topLeft == 1 || botRight == 1))
        {
            return PositionTypeTile.OneRight;
        }

        if (mid == 1 && right == 1 && (topRight == 1 || botLeft == 1 ||
                                       topLeft == 1 || botRight == 1))
        {
            return PositionTypeTile.OneLeft;
        }

        if (mid == 1 && bot == 1 && (topRight == 1 || botLeft == 1 ||
                                     topLeft == 1 || botRight == 1))
        {
            return PositionTypeTile.OneTop;
        }

        if (mid == 1 && top == 1 && (topRight == 1 || botLeft == 1 ||
                                     topLeft == 1 || botRight == 1))
        {
            return PositionTypeTile.OneBot;
        }

        // ------------
        if (mid == 1 && top == 1 && left == 1)
        {
            return PositionTypeTile.OneBotRight;
        }

        if (mid == 1 && top == 1 && right == 1)
        {
            return PositionTypeTile.OneBotLeft;
        }

        if (mid == 1 && bot == 1 && right == 1)
        {
            return PositionTypeTile.OneTopLeft;
        }

        if (mid == 1 && bot == 1 && left == 1)
        {
            return PositionTypeTile.OneTopRight;
        }

//-------------
        if (mid == 1 && left == 1 && right == 1)
        {
            return PositionTypeTile.OneHorizontal;
        }

        if (mid == 1 && top == 1 && bot == 1)
        {
            return PositionTypeTile.OneVertical;
        }

        return PositionTypeTile.NONE;
    }

    public static PositionTypeTile Equal_2(int mid, int bot, int right, int top, int left, int botLeft, int botRight,
        int topLeft,
        int topRight)
    {
        if (mid == 1 && (topRight == 1 || botLeft == 1 ||
                         topLeft == 1 || botRight == 1))

        {
            return PositionTypeTile.OneMid;
        }
        // ------------

        if (mid == 1 && left == 1)
        {
            return PositionTypeTile.OneRight;
        }

        if (mid == 1 && right == 1)
        {
            return PositionTypeTile.OneLeft;
        }

        if (mid == 1 && bot == 1)
        {
            return PositionTypeTile.OneTop;
        }

        if (mid == 1 && top == 1)
        {
            return PositionTypeTile.OneBot;
        }

        return PositionTypeTile.NONE;
    }
}


[System.Serializable]
public class WaterTile
{
    [SerializeField] private PositionTypeTile positionPositionTypeTile;
    [SerializeField] private Vector2 _position;

    public Vector2 Pos => _position;

    public PositionTypeTile PositionPositionTypeTile
    {
        get => positionPositionTypeTile;
        private set { positionPositionTypeTile = value; }
    }

    public void SetPosition(Vector3 position)
    {
        _position = position;
    }

    public void SetPositionType(int[,] arounds, int x, int y, int xMax, int yMax)
    {
        TileData.SetPositionType(arounds, x, y, xMax, yMax, out PositionTypeTile xx, out bool botRight,
            out bool botLeft, out bool topRight, out bool topLeft, out bool right, out bool left, out bool top,
            out bool bot);
        PositionPositionTypeTile = xx;
    }
}



[System.Serializable]
public class GroundBasic2Tile
{
    [SerializeField] private PositionTypeTile positionPositionTypeTile;
    [SerializeField] private Vector2 _position;

    public Vector2 Pos => _position;

    public PositionTypeTile PositionPositionTypeTile
    {
        get => positionPositionTypeTile;
        private set { positionPositionTypeTile = value; }
    }

    public void SetPosition(Vector3 position)
    {
        _position = position;
    }

    public void SetPositionType(int[,] arounds, int x, int y, int xMax, int yMax)
    {
        TileData.SetPositionType(arounds, x, y, xMax, yMax, out PositionTypeTile xx, out bool botRight,
            out bool botLeft, out bool topRight, out bool topLeft, out bool right, out bool left, out bool top,
            out bool bot);
        PositionPositionTypeTile = xx;
    }
}