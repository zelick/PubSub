using System;
using System.Threading;

namespace PubSubConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new PubSubEngine();

            // Pretplate korisnika
            engine.Subscribe(1, msg => Console.WriteLine($"[Ana] Poruka: {msg}"));
            engine.Subscribe(1, msg => Console.WriteLine($"[Kristina] Poruka: {msg}"));
            engine.Subscribe(2, msg => Console.WriteLine($"[Milica] Poruka: {msg}"));

            // Objavi poruku na temu 1 (Fotografije)
            Console.WriteLine("Objavljujem novu poruku na temu 1 (Fotografije)...");
            engine.Publish(1, "Nova pesma od izvođača X!");

            // Objavi poruku na temu 2 (Sport)
            Console.WriteLine("Objavljujem novu poruku na temu 2 (Sport)...");
            engine.Publish(2, "Fudbalski tim Y je pobedio!");

            // Objavi poruku na temu 3 (niko nije pretplaćen, neće se ništa desiti)
            Console.WriteLine("Objavljujem na temu 3 (nepostojeća pretplata)...");
            engine.Publish(3, "Ovo niko neće videti");

            // Sačekaj malo da se niti završe
            Thread.Sleep(1000);
        }
    }
}
