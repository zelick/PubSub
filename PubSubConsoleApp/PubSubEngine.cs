using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubConsoleApp
{
    public class PubSubEngine
    {
        private readonly Dictionary<int, List<Handler>> subscribers = new();
        private readonly Dictionary<int, string> PublicationMap = new(); //ovde mi se nalaze sve neobradjene publikacije
        private readonly object _lock = new();

        public PubSubEngine() 
        {
            StartConsumingMessages();
        }
        private void StartConsumingMessages()
        {
            Thread thread = new Thread(() => { 
                //prolazi kroz neobradjene poruke i tamo gde postoji subscriber onda poziva registrovani delegat 
                //da li mi treba red za svakog subrscibera
            });
        }

        public void Subscribe(int topicId, Handler handler)
        {
            lock (_lock)
            {
                if (!subscribers.ContainsKey(topicId))
                {
                    subscribers[topicId] = new List<Handler>();
                }

                subscribers[topicId].Add(handler);
            }
        }

        public void Publish(int topicId, string message) 
        {
            List<Handler> subList;

            lock (_lock)
            {
                if (!subscribers.TryGetValue(topicId, out subList))
                    return;
            }

            //foreach (var subscriber in subList)
            //{
            //    subscriber
            //}
        }
    }
}
