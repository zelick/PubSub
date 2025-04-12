using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubConsoleApp
{
    public class PubSubEngine
    {
        // na primer int - topic id( muzika, sport, fotografija) 
        // ana se pretplati na fotografije i muziku 
        // kristina se pretplati na sport 
        // milica se pretplati na sve 
        //neko objavi poruku na temu Muzika, svi koji su pretplaceni dobijaju tu poruku 

        //List<Action<string>> -  lista korisnika, odnosno funckija koja reaguje kada dodje poruka
        // vise ljudi moze da se pretplati na temu - one to many model ili ne?
        private readonly Dictionary<int, List<Action<string>>> _subscriptions = new();
        private readonly object _lock = new(); // Za sinhronizaciju

       //   topicId -  na koju temu se korisnik pretplacuje 
       //  Action<string> handler - funkcija koja ce se pozvati kad poruka stigne 
        public void Subscribe(int topicId, Action<string> handler)
        {
            lock (_lock)
            {
                if (!_subscriptions.ContainsKey(topicId))
                    _subscriptions[topicId] = new List<Action<string>>();

                _subscriptions[topicId].Add(handler); //lista korisnika tj. njihova funckija koja se pozvati kad dodje poruka 
            }
        }

        // Objavi poruku na datu temu → sve funkcije koje su pretplaćene će biti pozvane
        public void Publish(int topicId, string message) //na koju temu se pretplacuje korisnik
        {
            List<Action<string>> subscribers;

            lock (_lock)
            {
                if (!_subscriptions.TryGetValue(topicId, out subscribers))
                    return;
                //subscribers = new List<Action<string>>(subscribers); 
            }

            Console.WriteLine($"[PubSubEngine] Šaljem poruku za temu {topicId}: \"{message}\" ({subscribers.Count} pretplatnika)");

            foreach (var handler in subscribers)
            {
                // Pokreni handler u zasebnoj niti (kao da šaljemo notifikaciju korisnicima)
                new Thread(() => handler(message)).Start();
            }
        }
    }
}
