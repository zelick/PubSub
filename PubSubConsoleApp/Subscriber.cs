using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubConsoleApp
{
    public class Subscriber
    {
        
        public string Name { get; set; }
        public Handler Handler { get; set; }
        public readonly Queue<string> Channel = new Queue<string>();
        public readonly object _lock = new();
        private readonly AutoResetEvent messageAvailable = new(false); // signal za novu poruku

        public Subscriber(string name, Handler handler) {
            Name = name;
            Handler = handler;
            StartConsumingMessages();
        }


        public void ProduceMessage(string message)
        {
            lock (_lock)
            {
                Channel.Enqueue(message);
                messageAvailable.Set(); // signalizuj da je stigla nova poruka
            }
        }

        private void StartConsumingMessages()
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    messageAvailable.WaitOne(); // ceka dok ne stigne signal
 
                    string message;

                    lock (_lock)
                    {
                        if (Channel.Count == 0)
                            continue; //Ako nema poruka, ceka

                        message = Channel.Dequeue();
                    }
                    Console.WriteLine("Poruka poslata za: " + this.Name);
                    Handler(message);
                }
            });

            thread.IsBackground = true; 
            thread.Start();
        }

    }
}
