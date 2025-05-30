using Main.EventTest;
using UnityEngine;
using xicheng.events;
namespace xicheng.module.events
{
    public class EventTest : MonoBehaviour
    {
        void Start()
        {
            
            var testEvent =RefPool.Acquire<TestEvent>();
            
            //GameEvent.AddListener(typeof(TestEvent), TestEventAction);
            GameEvent.AddListener<TestEvent>(TestEventAction);
            
         
            
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