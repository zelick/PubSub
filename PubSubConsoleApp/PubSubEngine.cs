using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubConsoleApp
{
    public class PubSubEngine
    {
        private readonly Dictionary<int, List<Handler>> subscribersMap = new();
        private readonly Dictionary<int, BlockingCollection<string>> topicMessageQueues = new();

        public void Subscribe(int topic, Handler action)
        {
            lock (subscribersMap)
            {
                if (!topicMessageQueues.ContainsKey(topic))
                {
                    topicMessageQueues[topic] = new BlockingCollection<string>();
                }

                if (!subscribersMap.ContainsKey(topic))
                {
                    subscribersMap[topic] = new List<Handler>();
                }
                subscribersMap[topic].Add(action);

                var messageQueue = topicMessageQueues[topic];

                var thread = new Thread(() =>
                {
                    foreach (var message in messageQueue.GetConsumingEnumerable()) //GetConsumingEnumerable - ceka poruke, bez stalnog proveravanja 
                    {
                        foreach (var handler in subscribersMap[topic])
                        {
                            handler(message);
                        }
                    }
                });

                thread.IsBackground = true;
                thread.Start();
            }
        }
        public void Publish(int topic, string message)
        {
            lock (topicMessageQueues)
            {
                if (topicMessageQueues.TryGetValue(topic, out var queue))
                {
                    queue.Add(message); 
                }
            }
        }

    }
}
