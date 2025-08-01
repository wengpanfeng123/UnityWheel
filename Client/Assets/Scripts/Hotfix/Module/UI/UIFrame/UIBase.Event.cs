using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Xicheng.module.ui;

namespace Xicheng.UI
{
    
    public partial class UIBase
    {
        public Dictionary<UI_Event, UI_Event.BaseEventDelegate> _baseEventDic;
        private Dictionary<UI_Event, UI_Event.PointerEventDelegate> _pointerEventDic;
        private Dictionary<UI_Event, UI_Event.AxisEventDelegate> _axisEventDic;
        
        private void Execute(EventTriggerType id, BaseEventData eventData)
        {
            // for (int i = 0; i < triggers.Count; ++i)
            // {
            //     var ent = triggers[i];
            //     if (ent.eventID == id && ent.callback != null)
            //         ent.callback.Invoke(eventData);
            // }
        }
        
        
        /// <summary>
        /// Called by the EventSystem when the pointer enters the object associated with this EventTrigger.
        /// </summary>
        public virtual void RegisterPointerEnter(GameObject go,UI_Event.PointerEventDelegate pEvent)
        {
            //Execute(EventTriggerType.PointerEnter, pEvent);
            var uiEvent = UI_Event.Get(go);
            _pointerEventDic.TryAdd(uiEvent, pEvent);
            uiEvent.onPointerEnter = pEvent;
        }

        /// <summary>
        /// Called by the EventSystem when the pointer exits the object associated with this EventTrigger.
        /// </summary>
        public virtual void RegisterPointerExit(GameObject go,UI_Event.PointerEventDelegate pEvent)
        {
            //Execute(EventTriggerType.PointerExit, eventData);
            var uiEvent = UI_Event.Get(go);
            _pointerEventDic.TryAdd(uiEvent, pEvent);
            uiEvent.onPointerExit = pEvent;
        }

        /// <summary>
        /// Called by the EventSystem every time the pointer is moved during dragging.
        /// </summary>
        public virtual void RegisterOnDrag(GameObject go,UI_Event.PointerEventDelegate pEvent)
        {
            //Execute(EventTriggerType.Drag, eventData);
            var uiEvent = UI_Event.Get(go);
            _pointerEventDic.TryAdd(uiEvent, pEvent);
            uiEvent.onDrag = pEvent;
        }

        /// <summary>
        /// Called by the EventSystem when an object accepts a drop.
        /// </summary>
        public virtual void RegisterOnDrop(GameObject go,UI_Event.PointerEventDelegate pEvent)
        {
            //Execute(EventTriggerType.Drop, eventData);
            var uiEvent = UI_Event.Get(go);
            _pointerEventDic.TryAdd(uiEvent, pEvent);
            uiEvent.onDrop = pEvent;
        }

        /// <summary>
        /// Called by the EventSystem when a PointerDown event occurs.
        /// </summary>
        public virtual void RegisterPointerDown(GameObject go,UI_Event.PointerEventDelegate pEvent)
        {
            //Execute(EventTriggerType.PointerDown, eventData);
            var uiEvent = UI_Event.Get(go);
            _pointerEventDic.TryAdd(uiEvent, pEvent);
            uiEvent.onPointerDown = pEvent;
        }

        /// <summary>
        /// Called by the EventSystem when a PointerUp event occurs.
        /// </summary>
        public virtual void RegisterPointerUp(GameObject go,UI_Event.PointerEventDelegate pEvent)
        {
            //Execute(EventTriggerType.PointerUp, eventData);
            var uiEvent = UI_Event.Get(go);
            _pointerEventDic.TryAdd(uiEvent, pEvent);
            uiEvent.onPointerUp = pEvent;
        }

        /// <summary>
        /// Called by the EventSystem when a Click event occurs.
        /// </summary>
        public virtual void RegisterClick(Button go,UI_Event.PointerEventDelegate pEvent)
        {
            // Execute(EventTriggerType.PointerClick, eventData);
            var uiEvent = UI_Event.Get(go.gameObject);
            _pointerEventDic.TryAdd(uiEvent, pEvent);
            uiEvent.onPointerClick = pEvent;
        }

        /// <summary>
        /// Called by the EventSystem when a Select event occurs.
        /// </summary>
        public virtual void RegisterOnSelect(GameObject go,UI_Event.BaseEventDelegate bEvent)
        {
            // Execute(EventTriggerType.Select, eventData);
            var uiEvent = UI_Event.Get(go);
            _baseEventDic.TryAdd(uiEvent, bEvent);
            uiEvent.onSelect = bEvent;
        }

        /// <summary>
        /// Called by the EventSystem when a new object is being selected.
        /// </summary>
        public virtual void RegisterOnDeselect(GameObject go,UI_Event.BaseEventDelegate bEvent)
        {
            //Execute(EventTriggerType.Deselect, eventData);
            var uiEvent = UI_Event.Get(go);
            _baseEventDic.TryAdd(uiEvent, bEvent);
            uiEvent.onDeselect = bEvent;
        }

        /// <summary>
        /// Called by the EventSystem when a new Scroll event occurs.
        /// </summary>
        public virtual void RegisterOnScroll(GameObject go,UI_Event.PointerEventDelegate pEvent)
        {
            // Execute(EventTriggerType.Scroll, eventData);
            var uiEvent = UI_Event.Get(go);
            _pointerEventDic.TryAdd(uiEvent, pEvent);
            uiEvent.onScroll = pEvent;
        }

        /// <summary>
        /// Called by the EventSystem when a Move event occurs.
        /// </summary>
        public virtual void RegisterOnMove(GameObject go,UI_Event.AxisEventDelegate aEvent)
        {
            //Execute(EventTriggerType.Move, eventData);
            var uiEvent = UI_Event.Get(go);
            _axisEventDic.TryAdd(uiEvent, aEvent);
            uiEvent.onMove = aEvent;
        }

        /// <summary>
        /// Called by the EventSystem when the object associated with this EventTrigger is updated.
        /// </summary>
        public virtual void RegisterOnUpdateSelected(GameObject go,UI_Event.BaseEventDelegate bEvent)
        {
            // Execute(EventTriggerType.UpdateSelected, eventData);
            var uiEvent = UI_Event.Get(go);
            _baseEventDic.TryAdd(uiEvent, bEvent);
            uiEvent.onUpdateSelected = bEvent;
        }

        /// <summary>
        /// Called by the EventSystem when a drag has been found, but before it is valid to begin the drag.
        /// </summary>
        public virtual void RegisterOnInitializePotentialDrag(GameObject go,UI_Event.PointerEventDelegate pEvent)
        {
            // Execute(EventTriggerType.InitializePotentialDrag, eventData);
            var uiEvent = UI_Event.Get(go);
            _pointerEventDic.TryAdd(uiEvent, pEvent);
            uiEvent.onInitializedPotentialDrag = pEvent;
        }

        /// <summary>
        /// Called before a drag is started.
        /// </summary>
        public virtual void RegisterOnBeginDrag(GameObject go,UI_Event.PointerEventDelegate pEvent)
        {
            // Execute(EventTriggerType.BeginDrag, eventData);
            var uiEvent = UI_Event.Get(go);
            _pointerEventDic.TryAdd(uiEvent, pEvent);
            uiEvent.onBeginDrag = pEvent;
        }

        /// <summary>
        /// Called by the EventSystem once dragging ends.
        /// </summary>
        public virtual void RegisterOnEndDrag(GameObject go,UI_Event.PointerEventDelegate pEvent)
        {
            // Execute(EventTriggerType.EndDrag, eventData);
            var uiEvent = UI_Event.Get(go);
            _pointerEventDic.TryAdd(uiEvent, pEvent);
            uiEvent.onEndDrag = pEvent;
        }

        /// <summary>
        /// Called by the EventSystem when a Submit event occurs.
        /// </summary>
        public virtual void RegisterOnSubmit(GameObject go,UI_Event.BaseEventDelegate bEvent)
        {
            var uiEvent = UI_Event.Get(go);
            _baseEventDic.TryAdd(uiEvent, bEvent);
            uiEvent.onSubmit = bEvent;
        }

        /// <summary>
        /// Called by the EventSystem when a Cancel event occurs.
        /// </summary>
        public virtual void RegisterOnCancel(GameObject go,UI_Event.BaseEventDelegate bEvent)
        {
            var uiEvent = UI_Event.Get(go);
            _baseEventDic.TryAdd(uiEvent, bEvent);
            uiEvent.onCancel = bEvent;
        }
        
        

        private void ClearEvent()
        {
            if (_pointerEventDic != null)
            {
                foreach (var kv in _pointerEventDic)
                {
                    kv.Key.Clear();
                }
                _pointerEventDic.Clear();
            }

            //事件清空
            if (_pointerEventDic != null)
            {
                foreach (var kv in _baseEventDic)
                {
                    kv.Key.Clear();
                }
                _pointerEventDic.Clear();
            }

         
            if (_axisEventDic != null)
            {
                foreach (var kv in _axisEventDic)
                {
                    kv.Key.Clear();
                }
                _axisEventDic.Clear();
            }
        }
    }
}