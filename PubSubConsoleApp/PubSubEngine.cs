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

        private readonly Dictionary<int, Handler> subscribersMap = new Dictionary<int, Handler>(); //kad zatreba
        private readonly Dictionary<int, List<BlockingCollection<string>>> topicMessageQueues = new Dictionary<int, List<BlockingCollection<string>>>();
        private readonly object topicLock = new object();

        public void Subscribe(int topic, Handler action)
        {
            var queue = new BlockingCollection<string>();
            lock (topicLock)
            {
                if (!topicMessageQueues.ContainsKey(topic))
                {
                    topicMessageQueues[topic] = new List<BlockingCollection<string>>();
                }
                topicMessageQueues[topic].Add(queue);
            }
            ThreadPool.QueueUserWorkItem( ac => //metoda ce biti izvrsena kada se nit oslobodi
            {
                foreach (var message in queue.GetConsumingEnumerable())
                {
                    action(message); 
                }
            });
        }
        public void Publish(int topic, string message)
        {
            lock (topicLock)
            {
                if (topicMessageQueues.ContainsKey(topic))
                {
                    foreach (var queue in topicMessageQueues[topic])
                    {
                        queue.Add(message);
                    }
                }
            }
        }

    }
}
