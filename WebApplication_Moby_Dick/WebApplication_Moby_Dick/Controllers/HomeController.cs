using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication_Moby_Dick.Models;
using System.Xml;

namespace WebApplication_Moby_Dick.Controllers
{
    [ResponseCache(Duration = 600)]
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            
            // xml okuma işlemleri
            var kelime_listesi = new liste();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(@"C:\MOBY\moby_words.xml");

            XmlNode words = xmldoc.SelectSingleNode("Words");
            XmlNodeList wordList = words.SelectNodes("word");

            //Xml den alınan verileri listeye aktarma
            foreach (XmlNode node in wordList)
            {
                var word = new words();
                word.text = node.Attributes.GetNamedItem("text").Value;
                word.count = Convert.ToInt16(node.Attributes.GetNamedItem("count").Value);
                kelime_listesi.kelimeler.Add(word);
            }

            
            // Listeyi count a göre azalan sırada düzenleme
            var sortedList = kelime_listesi.kelimeler.OrderByDescending( a => a.count);

            // Düzenlenen listenin ilk 10 kelimesini yeni listeye aktarma
            var yeni_liste = new liste();
            int sayac = 1;
            foreach ( var kelime in sortedList)
             {
                yeni_liste.kelimeler.Add(kelime);

                 sayac++;
                 if (sayac > 10)
                 {
                     break;
                 }

             }
            
             return View("../Word/List/top_10_max_words", yeni_liste);


         }   //index sonu
        

    } //homecontroller sonu
}
