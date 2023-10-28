using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Scripts.Helper;
using UnityEngine.UI;
using System.Linq;
using Assets.Scripts.Common;

public class PanelMap : MonoBehaviour
{
    public static PanelMap instance;

    [SerializeField] private RectTransform pages;
    [SerializeField] private int maxPage = 3;

    [SerializeField] private GameObject levelItemViewPf;
    [SerializeField] private GameObject levelPagePf;
    [SerializeField] private PageSwiper pageSwiper;
    [SerializeField] private TextMeshProUGUI totalStarTxt;

    private int curLevel;
    private List<LevelEndGameInfo> levelEndGameInfos;
    private List<int> showedPageIndexs;

    public void OnEnable()
    {
        instance = this;

        showedPageIndexs = new List<int>();
        levelEndGameInfos = ES3.Load(StringDefine.LevelEndGameInfos, new List<LevelEndGameInfo>());

        pageSwiper.totalPages = maxPage;
        curLevel = ES3.Load(StringDefine.CurrentLevel, Config.FirstLevel);
        var index = curLevel / 25 + 1;
        pageSwiper.currentPage = index;
        pageSwiper.gameObject.SetActive(false);

        pageSwiper.Show();

        while (pages.childCount > 0)
        {
            ObjectPool.instance.Disable(pages.GetChild(0).gameObject);
        }

        for (int k = 0; k < maxPage; k++)
        {
            var go = ObjectPool.instance.GetGameObject(levelPagePf, Vector3.zero, Quaternion.identity);
            var rt = go.GetComponent<RectTransform>();
            go.transform.SetParent(pages);
            go.transform.localScale = new Vector3(1, 1, 1);

            rt.offsetMin = new Vector2(Screen.width * k, rt.offsetMin.y);
            rt.offsetMax = new Vector2(Screen.width * k, rt.offsetMax.y - 100);
        }

        for (int k = pageSwiper.currentPage - 1; k < pageSwiper.currentPage + 1; k++)
        {
            var id = k + 1;
            if (id < 1 || showedPageIndexs.Contains(id)) continue;

            showedPageIndexs.Add(id);

            Show(id);
        }
        gameObject.SetActive(true);
    }

    private void Show(int pageIndex)
    {
        var container = pages.GetChild(pageIndex - 1).GetChild(0);

        while (container.childCount > 0)
        {
            ObjectPool.instance.Disable(container.GetChild(0).gameObject);
        }

        var min = (pageIndex - 1) * 25 + 1;
        var max = pageIndex * 25;

        int maxLevel = ES3.Load(StringDefine.CurrentLevel, 1);

        var totalStar = 0;
        var maxStar = 0;
        for (int k = min; k <= max; k++)
        {
            var go = ObjectPool.instance.GetGameObject(levelItemViewPf, Vector3.zero, Quaternion.identity);
            var itemView = go.GetComponent<LevelItemView>();

            var levelInfo = levelEndGameInfos.FirstOrDefault(l => l.levelAmount == k);

            itemView.Setup(k, k <= curLevel, levelInfo != null ? levelInfo.starAmount : 0);

            go.transform.SetParent(container);
            go.transform.localScale = new Vector3(1, 1, 1);

            if ( pageIndex == pageSwiper.currentPage && k <= maxLevel)
            {
                if(levelInfo != null) totalStar += levelInfo.starAmount;

                //Debug.Log("levelInfo.starAmoun:" + levelInfo.starAmount);
                maxStar += 3;
            }
        }
        //Debug.Log("totalStar:" + totalStar);
        if (pageIndex == pageSwiper.currentPage)
            totalStarTxt.text = totalStar + "/" + maxStar;
    }

    public void OnPageChange(int pageIndex)
    {
        for (int k = pageIndex - 1; k < pageIndex + 1; k++)
        {
            var id = k + 1;
            if (id < 1 || showedPageIndexs.Contains(id)) continue;
            showedPageIndexs.Add(id);
            Show(id);
        }
    }

    public void OnClickPrev()
    {
        if (pageSwiper.currentPage > 1)
        {
            pageSwiper.MovePage(-1);
        }
    }

    public void OnClickNext()
    {
        if (pageSwiper.currentPage < pageSwiper.totalPages)
        {
            pageSwiper.MovePage(1);
        }
    }
}
