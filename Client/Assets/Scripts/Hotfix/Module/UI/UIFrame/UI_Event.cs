using UnityEngine;
using UnityEngine.EventSystems;

namespace Xicheng.module.ui
{
    
 using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI事件类
/// </summary>
public class UI_Event : EventTrigger
{
    protected const float CLICK_INTERVAL_TIME = 0.2f; //点击间隔时间
    protected const float CLICK_INTERVAL_POS = 2;

    //三个基础类型数据-委托
    public delegate void PointerEventDelegate(PointerEventData eventData,UI_Event ev);
    public delegate void BaseEventDelegate(BaseEventData eventData,UI_Event ev);
    public delegate void AxisEventDelegate(AxisEventData eventData,UI_Event ev);
    
    public Dictionary<string ,object> ArgsDic = new Dictionary<string, object>();

    public BaseEventDelegate onDeselect = null;
    public BaseEventDelegate onSubmit = null;
    public BaseEventDelegate onSelect = null;
    public BaseEventDelegate onCancel = null;
    public BaseEventDelegate onUpdateSelected = null;
   
    public PointerEventDelegate onBeginDrag = null;
    public PointerEventDelegate onDrag = null;
    public PointerEventDelegate onEndDrag = null;
    public PointerEventDelegate onDrop = null;
    public PointerEventDelegate onPointerClick = null;
    public PointerEventDelegate onPointerDown = null;
    public PointerEventDelegate onPointerEnter = null;
    public PointerEventDelegate onPointerExit = null;
    public PointerEventDelegate onPointerUp = null;
    public PointerEventDelegate onScroll = null;
    public PointerEventDelegate onInitializedPotentialDrag = null;
    
    public AxisEventDelegate onMove = null;

 

    //设置参数
    public void SetData(string key ,object val)
    {
        ArgsDic[key] = val;
    }
    
    //获取参数
    public T GetData<T>(string key)
    {
        if (ArgsDic.TryGetValue(key, out var value))
        {
            return (T) value;
        }

        return default(T);
    }

    public static UI_Event Get(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("Error param");
            return null;
        }

        UI_Event listener = go.GetComponent<UI_Event>();
        if (listener == null)
        {
            listener = go.AddComponent<UI_Event>();
        }

        return listener;
    }

    public void Clear()
    {
        onDeselect = null;
        onSubmit = null;
        onSelect = null;
        onCancel = null;
        onUpdateSelected = null;
        onBeginDrag = null;
        onDrag = null;
        onEndDrag = null;
        onDrop = null;
        onPointerClick = null;
        onPointerDown = null;
        onPointerEnter = null;
        onPointerExit = null;
        onPointerUp = null;
        onScroll = null;
        onInitializedPotentialDrag = null;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        onBeginDrag?.Invoke(eventData, this);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        onDrag?.Invoke(eventData, this);
    }

    public override void OnCancel(BaseEventData eventData)
    {
        base.OnCancel(eventData);
        onCancel?.Invoke(eventData, this);
    }
    
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        onEndDrag?.Invoke(eventData, this);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        onDrag?.Invoke(eventData,this);
    }


    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        onPointerClick?.Invoke(eventData,this);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        onPointerUp?.Invoke(eventData,this);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        onPointerDown?.Invoke(eventData,this);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        onPointerEnter?.Invoke(eventData,this);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        onPointerExit?.Invoke(eventData,this);
    }

    public override void OnScroll(PointerEventData eventData)
    {
        base.OnScroll(eventData);
        onScroll?.Invoke(eventData,this);
    }


    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        onSubmit?.Invoke(eventData,this);
    }

    public override void OnMove(AxisEventData eventData)
    {
        base.OnMove(eventData);
        onMove?.Invoke(eventData,this);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        onSelect?.Invoke(eventData,this);
    }

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        base.OnInitializePotentialDrag(eventData);
        onInitializedPotentialDrag?.Invoke(eventData,this);
    }

    public override void OnUpdateSelected(BaseEventData eventData)
    {
        base.OnUpdateSelected(eventData);
        onUpdateSelected?.Invoke(eventData,this);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        onDeselect?.Invoke(eventData,this);
    }
}

}