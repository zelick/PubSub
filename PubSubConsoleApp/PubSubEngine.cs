using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubConsoleApp
{
    public class PubSubEngine
    {

        private readonly Dictionary<int, List<Subscriber>> subscribers = new();
        private readonly object _lock = new();
        public void Subscribe(int topicId, Subscriber subscriber)
        {
            lock (_lock)
            {
                if (!subscribers.ContainsKey(topicId))
                {
                    subscribers[topicId] = new List<Subscriber>();
                }

                subscribers[topicId].Add(subscriber);
            }
        }

        public void Publish(int topicId, string message) 
        {
            List<Subscriber> subList;

            lock (_lock)
            {
                if (!subscribers.TryGetValue(topicId, out subList))
                    return;
            }

           // Console.WriteLine($"[PubSubEngine] Tema {topicId}: \"{message}\" ({subscribers.Count} pretplatnika)");

            foreach (var subscriber in subList)
            {
                subscriber.ProduceMessage(message);
            }
        }
    }
}
