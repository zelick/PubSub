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
            Thread.Sleep(1000);
            Console.WriteLine($"[Handler 1] Primljena poruka 1: {message}"); 
        }

        public static void FastHandler(string message)
        {
            //Thread.Sleep(2000);
            Console.WriteLine($"[Handler 2] Primljena poruka 2: {message}");
        }

        public static void AnotherHandler(string message)
        {
            Console.WriteLine($"[Handler 3] Primljena poruka 3: {message}");
        }
        static void Main()
        {
            //var engine = new PubSubEngine();

            var engine = new PubSubEngine();

            engine.Subscribe(2, WriteMessage);
            engine.Subscribe(1, FastHandler);
            engine.Subscribe(1, AnotherHandler);

            engine.Publish(1, "Poruka #1");
            engine.Publish(1, "Poruka #2");
            engine.Publish(1, "Poruka #3");

            Console.WriteLine("Poruke su poslate. OBRADA..");
            Console.ReadKey();
        }
    }
}
