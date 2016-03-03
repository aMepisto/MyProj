
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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
        return string.Format("id [{0}], num[{1}], qty[{2}], " +
                             "SyncCnt [{3}], SyncComplete [{4}], " +
                             "SyncRewarded [{5}], SyncUpdated [{6}], Progress [{7}]", this.ID, this.Num, this.Qty,
                             this.SyncCnt, this.SyncComplete, this.SyncRewarded, this.SyncUpdated, this.Progress
                             );
    }
}

public class AchieveRow
{
	public AchieveRow()
	{
	}

	~AchieveRow()
	{
	}
}

public interface IAchievement
{
	void Init(List<DataClass> datas);

	void ApplySync ();

	DataClass GetCurrentItem ();

	string GetTitle ();

	string GetDescription ();
}

public abstract class MissionBase : IAchievement
{
	protected uint _groupID = 0u;

	protected List<DataClass> _datas = null; 

	//protected AchieveRow _row = null;

	public MissionBase(uint groupID)
	{
		this._groupID = groupID;
	}

	#region IAchievement implementation

	public void Init (List<DataClass> datas)
	{
		if (_datas == null) 
			_datas = new List<DataClass> ();
		_datas.AddRange (datas);
	}

	public abstract void ApplySync ();
	public abstract DataClass GetCurrentItem ();
	public abstract string GetTitle ();
	public abstract string GetDescription ();
	protected virtual bool InvalidDayTimeOver () { return true; }
	#endregion
}

public class Mission_Achievement : MissionBase
{
	public uint GroupID { get { return _groupID; } }

	public List<DataClass> datas { get { return _datas; } }

	public Mission_Achievement(uint groupID)
		:base(groupID)
	{
	}

	#region IAchievement implementation
	public override void ApplySync ()
	{
		// do Something;
	}

	public override DataClass GetCurrentItem ()
	{
		// do Search Current Item;
		return _datas[0];
	}

	public override string GetTitle ()
	{
		return string.Empty;
	}

	public override string GetDescription ()
	{
		return string.Empty;
	}

	#endregion
}

public class Mission_DailyMission : MissionBase
{
	public uint GroupID { get { return _groupID; } }

	public List<DataClass> datas { get { return _datas; } }

	public Mission_DailyMission(uint groupID)
		:base(groupID)
	{
	}

	#region IAchievement implementation
	public override void ApplySync ()
	{
		// do Something;
	}

	public override DataClass GetCurrentItem ()
	{
		// do Search Current Item;
		return _datas[0];
	}

	protected override bool InvalidDayTimeOver ()
	{
		// do something
		return true;
	}

	public override string GetTitle ()
	{
		return string.Empty;
	}

	public override string GetDescription ()
	{
		return string.Empty;
	}
	#endregion
}
	
public class ListSorting : MonoBehaviour
{
    #region Variables
	private List<Mission_Achievement> _achievement = new List<Mission_Achievement>();
	private List<Mission_DailyMission> _dailyMission = new List<Mission_DailyMission>();

    private List<KeyValuePair<int, DataClass>> _achieveList = new List<KeyValuePair<int, DataClass>>();
    #endregion Variables

    private void Awake() 
    {
		CreateDummyRowAchievementData ();
		CreateDummyRowDailyMissionData ();
		//ApplySync ();

		for (int i = 0; i < _achievement.Count; ++i) 
		{
			Debug.LogError (string.Format ("[Achievement GroupID {0}] => data Count {1}, \n GetCurrent : {2}", 
				_dailyMission[i].GroupID,
				_achievement[i].datas.Count, 
				_achievement[i].GetCurrentItem().ToString()));
		}

		for (int i = 0; i < _dailyMission.Count; ++i) 
		{
			Debug.LogError (string.Format ("[DailyMission GroupID {0}] => data Count {1}, \n GetCurrent : {2}", 
				_dailyMission[i].GroupID,
				_dailyMission[i].datas.Count, 
				_dailyMission[i].GetCurrentItem().ToString()));
		}
	}


	private void CreateDummyRowAchievementData ()
	{
		List<DataClass> rows1 = new List<DataClass> ();
		for (int i = 0; i < 3; ++i) 
		{
			var item = new DataClass (i, 0, (i + 1) * 2);
			rows1.Add(item);
		}

		List<DataClass> rows2 = new List<DataClass> ();
		for (int i = 3; i < 6; ++i) 
		{
			var item = new DataClass (i, 0, (i + 1) * 2);
			rows2.Add(item);
		}

		List<DataClass> rows3 = new List<DataClass> ();
		for (int i = 6; i < 10; ++i) 
		{
			var item = new DataClass (i, 0, (i + 1) * 2);
			rows3.Add(item);
		}

		var group1 = new Mission_Achievement (1u);
		group1.Init (rows1);
		_achievement.Add (group1);

		var group2 = new Mission_Achievement (2u);
		group2.Init (rows2);
		_achievement.Add (group2);

		var group3 = new Mission_Achievement (3u);
		group3.Init (rows3);
		_achievement.Add (group3);
	}

	private void CreateDummyRowDailyMissionData ()
	{
		List<DataClass> rows1 = new List<DataClass> ();
		for (int i = 0; i < 2; ++i) 
		{
			var item = new DataClass (i + 1000, 0, (i + 1));
			rows1.Add(item);
		}

		List<DataClass> rows2 = new List<DataClass> ();
		for (int i = 2; i < 4; ++i) 
		{
			var item = new DataClass (i + 1000, 0, (i + 1));
			rows2.Add(item);
		}

		List<DataClass> rows3 = new List<DataClass> ();
		for (int i = 4; i < 5; ++i) 
		{
			var item = new DataClass (i + 1000, 0, (i + 1));
			rows3.Add(item);
		}

		var group1 = new Mission_DailyMission (1u);
		group1.Init (rows1);
		_dailyMission.Add (group1);

		var group2 = new Mission_DailyMission (2u);
		group2.Init (rows2);
		_dailyMission.Add (group2);

		var group3 = new Mission_DailyMission (3u);
		group3.Init (rows3);
		_dailyMission.Add (group3);
	}

	/*
	 * 
	 	// TODO : only sort Test Method..
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

	// TODO : only sort Test Method..
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
	*/

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

}
