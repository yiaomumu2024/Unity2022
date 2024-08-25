using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BagItem : BaseUI, ItemBase<BagItemInfo>
{
    private TextMeshProUGUI countTxt;
    private BagItemInfo dataInfo;

    protected override void OnInit()
    {
        countTxt = GetComponentInChildren<TextMeshProUGUI>("numText");
    }

    public void SetInfo(BagItemInfo info)
    {
        dataInfo = info;
        countTxt.text = dataInfo.id + "";
    }
}
