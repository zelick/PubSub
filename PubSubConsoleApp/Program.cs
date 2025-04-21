using System;
using System.Collections.Generic;
using System.Threading;

namespace PubSubConsoleApp
{
    public delegate void Handler(string message); 
    class Program
    {
        public static void WriteMessage(string message)
        {
            Console.WriteLine($"[Handler 1] Primljena poruka: {message}");
            Thread.Sleep(2000); // simulacija sporog handlera
        }

        public static void FastHandler(string message)
        {
            Console.WriteLine($"[Handler 2] Brza obrada poruke: {message}");
        }

        public static void AnotherHandler(string message)
        {
            Console.WriteLine($"[Handler 3] Još jedan handler dobio poruku: {message}");
        }
        static void Main()
        {
            //var engine = new PubSubEngine();

            var pubSubManager = new PubSubManager();

            pubSubManager.Subscribe(1, WriteMessage);
            pubSubManager.Subscribe(1, FastHandler);
            pubSubManager.Subscribe(1, AnotherHandler);

            pubSubManager.Publish(1, "Poruka #1");
            pubSubManager.Publish(1, "Poruka #2");
            pubSubManager.Publish(1, "Poruka #3");

            Console.WriteLine("Poruke su poslate. Obrada..");
            Console.ReadKey();
        }
    }
}
