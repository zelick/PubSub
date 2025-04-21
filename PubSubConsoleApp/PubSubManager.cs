using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubConsoleApp
{
    //pomocna klasa
    public class Publication
    {
        public int TopicId { get; }
        public string Message { get; } = string.Empty;
        public Publication(int topicId, string messages) 
        { 
            TopicId = topicId;
            Message = messages;
        }   
    }
    public class PubSubManager
    {
        private Dictionary<int, List<Handler>> subscribersByTopicId = new Dictionary<int, List<Handler>>();
        private readonly Queue<Publication> publicationQueue = new(); // koristi red za obradu a ne mapu, da bi bio neki redosled
        private readonly Dictionary<Handler, Queue<string>> handlerQueues = new(); // red za svakog subscribera
        private readonly object _lock = new();
        private readonly AutoResetEvent messageEvent = new(false); 

        public PubSubManager() 
        {
            StartConsumingMessages();
        }

        public void StartConsumingMessages()
        {
            //proveri da li postoji subscriber na temu -  da li je lista Handler prazna 
            //prodji kroz neobradjene publikacije i tamo gde postoji subscriber (gde lista nije prazna) - pozovi odgvoarajuce handlere
            
            Thread publicationThread = new(() =>
            {
                while(true)
                {
                    Publication publication;  //boze sacuvaj, mozda ima bolji nacin
                    
                    lock(_lock)
                    {
                        //ako je prazno 
                        if(publicationQueue.Count ==  0)
                        {
                            //break;
                            //trebalo bi nit da udje u stanje cekanja, dok se ne promeni stanje

                            // Nema poruka – pusti lock i čekaj da stigne neka
                            Monitor.Exit(_lock);
                            messageEvent.WaitOne();
                            Monitor.Enter(_lock);
                            continue;
                        }
                        //ako nije prazna 
                        publication = publicationQueue.Dequeue();
                    }
                    
                    //Objava neobradjenih publikacija
                    List<Handler> handlers;
                    lock (_lock)
                    {
                        //ako postoji subscriber tj. lista handlera nije prazna
                        if (!subscribersByTopicId.TryGetValue(publication.TopicId, out handlers))
                        {
                            continue;
                        }
                    }
                        //ako postoje za svakog subsribera kreiaraj nit i pozovi handler 
                        //svaka nit ima svoj red, slicaj kad nije online ili je mreza losa
                        foreach(Handler handler in handlers)
                        {
                            lock (_lock)
                            {
                                if (!handlerQueues.ContainsKey(handler))
                                {
                                    handlerQueues[handler] = new Queue<string>();
                                    StartHandlerThread(handler);//pokreni nit za nekog subscribera
                                }

                                handlerQueues[handler].Enqueue(publication.Message);
                            }
                        }
                    }
                });

            publicationThread.IsBackground = true;
            publicationThread.Start();
        }


        private void StartHandlerThread(Handler handler)
        {
            Thread handlerThread = new(() =>
            {
                while (true)
                {
                    string message = null;

                    lock (_lock)
                    {
                        if (handlerQueues.TryGetValue(handler, out var queue) && queue.Count > 0)
                        {
                            message = queue.Dequeue();
                        }
                    }

                    if (message != null)
                    {
                        try
                        {
                            handler(message); // izvrsava delegat
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Nije se izvrsio delegat greska");
                        }
                    }
                    else
                    {
                        Thread.Sleep(100); // cekaj malo ako nema poruka -- kao da nit radi stalno i proverava da li je red prazan :)
                    }
                }
            });

            handlerThread.IsBackground = true;
            handlerThread.Start();
        }


        public void Subscribe(int topicId, Handler handler)
        {
            //pretplacuje se neko na temu
            //dodaj ovo u mapu subscribers by topic id 
            lock (_lock)
            {
                if (!subscribersByTopicId.ContainsKey(topicId))
                {
                    subscribersByTopicId[topicId] = new List<Handler>();
                }

                if (!subscribersByTopicId[topicId].Contains(handler))
                {
                    subscribersByTopicId[topicId].Add(handler);
                }
            }
        }

        public void Publish(int topicId, string message)
        {
            //objavi se tema, smesti u red neorbadjenih poruka
            var publication = new Publication(topicId, message);
            lock (_lock)
            {
                publicationQueue.Enqueue(publication);
            }

            messageEvent.Set(); // obavestava publicationThread da ima novih neobradjenih poruka
        }
    }
}
