using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubConsoleApp
{
    //Svaki subscriber ima svoju nit koja obradjuje poruke iz svog reda
    //Ako je mreza sporija za nekoga ili je nema
    public class Subscriber
    { 
        public string Name { get; set; }
        public readonly Action<string> Handler;
        public readonly Queue<string> Channel = new Queue<string>();
        public readonly object _lock = new();
        private readonly AutoResetEvent messageAvailable = new(false); // signal za novu poruku

        public Subscriber(string name, Action<string> handler) 
        {
            Name = name;
            Handler = handler;
            StartConsumingMessages();
        }

        //poruke stuzu u red
        // ako ovaj subscriber ne radi treunto ili je spor - poruke cekaju u redu
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

                    Handler(message);
                }
            });

            thread.IsBackground = true; 
            thread.Start();
        }

    }
}
