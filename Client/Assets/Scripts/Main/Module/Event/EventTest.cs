using Main.EventTest;
using UnityEngine;
using Xicheng.events;

namespace Xicheng.module.events
{
    public class EventTest : MonoBehaviour
    {
        void Start()
        {
         
            var testEvent =RefPool.Acquire<TestEvent>();
       
            //GameEvent.AddListener(typeof(TestEvent), TestEventAction);
            GameEvent.AddListener<TestEvent>(TestEventAction);

            var eventGroup = EventGroup.Acquire();
            eventGroup.Subscribe<TestEvent>(TestEventAction);
            eventGroup.UnSubscribe(typeof(TestEvent),TestEventAction);
            eventGroup.Notify(new TestEvent());
            eventGroup.Clear();
       

            //GameEvent.RemoveListener(typeof(TestEvent), TestEventAction);
            //GameEvent.RemoveListener<TestEvent>( TestEventAction);

        }

        void OnGUI()
        {
            if (GUILayout.Button("EventTest"))
            {
                TestEvent evt = RefPool.Acquire<TestEvent>();
 
                evt.name = "wengpanfeng";
                GameEvent.Send(evt);
            }

            if (GUILayout.Button("RemoveEventTest"))
            {
                GameEvent.RemoveListener(typeof(TestEvent), TestEventAction);
            }
        }

        void TestEventAction(IEvent p)
        {
            if (p == null)
            {
                Debug.Log("[TestEventAction] param is nil~ ");
            }
            else
            {
                TestEvent _p = p as TestEvent;
                Debug.Log("[TestEventAction] p.name =  " + _p.name);
            }
        }
    }
}