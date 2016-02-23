using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DataClass
{
    public int ID;
    public int Num;
    public int Qty;

    public int SyncCnt { get; set; }
    public int SyncComplete { get; set; }
    public int SyncRewarded { get; set; }
    public int SyncUpdated { get; set; }
    private float _progress;
    public float Progress { get { return _progress; } }
    public int SyncSort
    {
        get
        {
            if (this.SyncComplete != 0 && this.SyncRewarded == 0) return 0;
            else if (0 < this.SyncCnt && this.SyncUpdated != 0 && this.SyncComplete == 0 && this.SyncRewarded == 0) return 1;
            else if (this.SyncCnt == 0 && this.SyncUpdated == 0 && this.SyncComplete == 0 && this.SyncRewarded == 0) return 2;
            else return 3;
        }
    }

    public DataClass(int id, int num, int qty)
    {
        this.ID = id;
        this.Num = num;
        this.Qty = qty;
    }

    public void SetSync(int cnt, int comp, int reward, int update)
    {
        SyncCnt = cnt;
        SyncComplete = comp;
        SyncRewarded = reward;
        SyncUpdated = update;

        _progress = (float)SyncCnt / this.Qty;
    }

    public override string ToString()
    {
        return string.Format("id [{0}], num[{1}], qty[{2}] \n" +
                             "SyncCnt [{3}], SyncComplete [{4}], " +
                             "SyncRewarded [{5}], SyncUpdated [{6}], Progress [{7}]", this.ID, this.Num, this.Qty,
                             this.SyncCnt, this.SyncComplete, this.SyncRewarded, this.SyncUpdated, this.Progress
                             );
    }
}

public class ListSorting : MonoBehaviour
{
    #region Mission Compare
    private static int MissionCompare(KeyValuePair<int, DataClass> x, KeyValuePair<int, DataClass> y)
    {
        // 조건 1 SyncSort 값 오름 차순 정렬.
        var nValue = x.Value.SyncSort.CompareTo(y.Value.SyncSort);
        if (nValue == 0)
        {
            // 조건 2 Progress값 내림 차순 정렬.
            nValue = x.Value.Progress.CompareTo(y.Value.Progress);
            if (nValue == 0)
            {
                // 조건 3 Key값 오름차순 정렬.
                return x.Key.CompareTo(y.Key);
            }

            return -nValue;
        }

        return nValue;
    }
    #endregion Mission Compare

    #region Variables
    private List<KeyValuePair<int, DataClass>> _achieveList = new List<KeyValuePair<int, DataClass>>();
    #endregion Variables

    private void Awake() 
    {
        CreateAchieve();

        try
        {
            SortList();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            throw;
        }
	}

    private void CreateAchieve()
    {
        var total = string.Empty;
        for (int i = 0; i < 20; ++i)
        {
            var item = new DataClass(i, i + 1, (i + 1) * 2);

            if (i == 0)
                item.SetSync(2, 1, 0, 1);
            else if (i == 1)
                item.SetSync(2, 0, 0, 1);
            else if (i == 5)
                item.SetSync(12, 1, 0, 1);
            else if (i == 6)
                item.SetSync(14, 1, 0, 1);
            else if (i == 7)
                item.SetSync(15, 0, 0, 1);
            else if (i == 8)
                item.SetSync(13, 0, 0, 1);
            else if (i == 14)
                item.SetSync(29, 0, 0, 1);
            else if (i == 15)
                item.SetSync(32, 1, 1, 1);
            else if (i == 18)
                item.SetSync(38, 1, 1, 1);
            else
                item.SetSync(0, 0, 0, 0);
            _achieveList.Add(new KeyValuePair<int, DataClass>(i, item));

            total += item.ToString() + "\n";
        }
        Debug.Log("one : \n" + total);
    }
    
    private void SortList()
    {
        _achieveList.Sort(MissionCompare);
        var total = string.Empty;
        foreach (var item in _achieveList)
        {
            total += item.ToString() + "\n";

        }
        Debug.Log("two : \n" + total);
    }
}
