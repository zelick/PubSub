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

        private readonly Dictionary<int, List<Handler>> subscribersMap = new Dictionary<int, List<Handler>>(); //kad zatreba
        private readonly Dictionary<int, List<BlockingCollection<string>>> topicMessageQueues = new Dictionary<int, List<BlockingCollection<string>>>();

        public void Subscribe(int topic, Handler action)
        {
            var queue = new BlockingCollection<string>();

            if (!topicMessageQueues.ContainsKey(topic))
            {
                topicMessageQueues[topic] = new List<BlockingCollection<string>>();
            }
            topicMessageQueues[topic].Add(queue);

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
