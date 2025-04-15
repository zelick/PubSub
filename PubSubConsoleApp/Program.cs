using System;
using System.Collections.Generic;
using System.Threading;

namespace PubSubConsoleApp
{
    class Program
    {
        static void Main()
        {
            var pubSub = new PubSubEngine();
            var subscriber1 = new Subscriber("Ana", message =>
            {
                Console.WriteLine($"[Ana] Primljena poruka: {message}");
                Thread.Sleep(1000); // simulacija sporijeg korisnika
            });

            var subscriber2 = new Subscriber("Kristina", message =>
            {
                Console.WriteLine($"[Kristina] Primljena poruka: {message}");
            });

            var subscriber3 = new Subscriber("Milica", message =>
            {
                Console.WriteLine($"[Milica] Poruka obrađena: {message}");
                Thread.Sleep(2000); // još sporiji subscriber
            });

            pubSub.Subscribe(1, subscriber1);
            pubSub.Subscribe(1, subscriber2);
            pubSub.Subscribe(1, subscriber3);

            pubSub.Publish(1, "Publish poruka 1");
            pubSub.Publish(1, "Publish poruka 2");
            pubSub.Publish(1, "Publish poruka 3");

            Console.ReadKey(); // zadržava aplikaciju da se pozadinske niti izvrse
        }
    }
}
