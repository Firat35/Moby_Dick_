using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Threading;

namespace Moby_Dick
{   
    class Program
    {      
        static void Main(string[] args)
        {            
           WebClient WC = new WebClient();
           // İndirme ilerlemesindeki tutarsızlığı gidermek için kullanılan değişken ve nesne
           long LastProgressUpdatePosition = -1;
           Object LockObject = new Object();

           WC.DownloadProgressChanged += (o, e) =>
            {
                //Dosya indirmede ilerlemedeki tutarsızlığı tek iş parçacığı ile önleme
                lock (LockObject)
                {
                    //Daha önceden yapılan ilerleme güncellenmesinin tekrar yapılmasını önler
                    if (LastProgressUpdatePosition > e.BytesReceived)
                        return;

                    Console.WriteLine("Downloaded {0} of {1} bytes. {2} % complete...", e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage);             
                    LastProgressUpdatePosition = e.BytesReceived;                  
                }
            };
            
            WC.DownloadFileCompleted += new AsyncCompletedEventHandler(WC_DownloadFileCompleted);

            //Asenkron işlemden dolayı(arka planda dosya inmesi ve aynı zamanda iniş yüzdesinin konsol ekranında gösterilmesi) 
            //dosyayı indirirken yeni kod satırına geçmesini önlemek için dosyanın inmesini bekletme
            var syncObject = new Object();
            lock (syncObject)
            {
                WC.DownloadFileAsync(new Uri("http://www.gutenberg.org/files/2701/2701-0.txt"), @"C:\MOBY\Moby_Dick.txt",syncObject);
                Monitor.Wait(syncObject);
            }
               
            //Belirtilen dizinde dosyayı okuyarak string türünde text değişkenine kaydet
            string filename = @"C:\MOBY\Moby_Dick.txt";
            string text = File.ReadAllText(filename);
           
            //Okunan kelimelerin hepsini küçük harfli kelimelere çevir
            text = text.ToLower();
            
            //Metinden gereksiz karakterlerin yerine boşluk koy
            Regex reg_exp = new Regex("[^a-zA-Z]");
            text = reg_exp.Replace(text, " ");

            // Metni kelimelere ayırıp liste oluşturma
            List<string> wordList = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
          
            // Kelimelerin sayısı ile birlikte bir liste oluşturmak için sözlük nesnesi oluşturma
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            // Listedeki kelimeleri teker teker sözlüğe kaydet sözlükte daha önce kayıtlıysa sayısını artır
            foreach (string word in wordList)
            {
                // Kelime 3 harftan az ise kelime olarak sayma
                if (word.Length >= 3)
                {
                    // Daha önceden kelime kaydedilmiş mi sözlüğe
                    if (dictionary.ContainsKey(word))
                    {
                        // Kaydedilmişse önceki kelimenin sayısını bir artır
                        dictionary[word]++;
                    }
                    else
                    {
                        // Kelime sözlükte kayıtlı değilse kayıt için sözlükteki değerine 1 değerini ata
                        dictionary[word] = 1;
                    }

                }//Kelime uzunluk kontrol sonu

            } // Listedeki kelimeleri tarama sonu

            //XML DOSYASINA KAYDETME
            var serializer = new DictionarySerializer<string, int>();
            serializer.Serialize("C://MOBY/moby_words.xml", dictionary);

            Console.WriteLine("Download edilen metindeki kelime sayısı XML dosyasına kaydedildi.");            
            Console.ReadKey();

        } // Main metodu sonu

        // İndirme tamamlandı bildirisi için kullanılan method
        static void WC_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null && !e.Cancelled)
            {
                Console.WriteLine("Download completed!");
            }
            lock (e.UserState)
            {
                //releases blocked thread
                Monitor.Pulse(e.UserState);
            }
        }

    } // Program sınıf sonu
   
    // Sözlüğün XML'e serialize işlemi için kullanılan sınıf
    public class DictionarySerializer<TKey, TValue>
    {
        [XmlType(TypeName = "word")]
        public class Item
        {
            [XmlAttribute("count")]
            public TValue Value;
            [XmlAttribute("text")]
            public TKey Key;           
        }

        private XmlSerializer _serializer = new XmlSerializer(typeof(Item[]), new XmlRootAttribute("Words"));
    
        public void Serialize(string filename, Dictionary<TKey, TValue> dictionary)
        {
            using (var writer = new StreamWriter(filename))
            {
                _serializer.Serialize(writer, dictionary.Select(p => new Item() { Key = p.Key, Value = p.Value }).ToArray());
            }
        }
    }
}
