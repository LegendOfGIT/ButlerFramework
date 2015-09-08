using System.Collections.Generic;
using System.IO;
using System.Text;

using Data.Mining;
using Data.Mining.Web;

namespace ConsoleApplication1
{
    class Program
    {
        static List<MiningCommand> ContextCommandset = new List<MiningCommand>();
        static Dictionary<string, IEnumerable<string>> ContextDictionary = new Dictionary<string, IEnumerable<string>>();

        static void Main(string[] args)
        {
            var encoding = Encoding.GetEncoding(1252);

            var index = default(int);
            var crawlingmap = @"../../crawling/twitter.crawl";
        
            var lines = default(string[]);
            if (File.Exists(crawlingmap))
            {
                lines = File.ReadAllLines(crawlingmap);
            }

            var sourceUri = string.Empty;
            foreach (var line in lines)
            {
                if(line.StartsWith("Source"))
                {
                    sourceUri = line.Split('=')[1];
                    break;
                }
                index++;
            }
            for (int i = index + 1; i <= lines.Length; i++)
            {
                if (lines[i] != string.Empty)
                {
                    index = i;
                    break;
                }
            }

            var compiler = new MiningCompiler();
            var commandset = compiler.ParseCommandset(lines, index);
            ContextDictionary[MiningUtilityConstants.CurrentUri] = new[] { sourceUri };

            var miningutility = new MiningUtility {
                ContextCommandset = compiler.ContextCommandset,
                ContextDictionary = ContextDictionary
            };            
            miningutility.Mining(commandset);

            sourceUri = sourceUri;



            //while (!string.IsNullOrEmpty(sourceUri)) {
            //    var content = WebUtility.GetWebsiteContent(sourceUri, encoding);
            
            //    var result = new Dictionary<string, string>(); 
            //    var query = new CQ(content);

            //    var doctors = query[".teilnehmer"];
            //    foreach (var doctor in doctors)
            //    {
            //        var doctordom = new CQ(doctor.InnerHTML);
                    
            //        //  Name
            //        var name = doctordom["span[itemprop='name']"].Text();
            //        var communication = doctordom[".hidden-xs"];
            //        //  Telefonnummer
            //        var telefonnumber = communication[".nummer"].First().Text();
            //        //  E-Mail
            //        var email = communication[".email span.text"].First().Text();
            //    }

            //    var uri = query[".gs_seite_vor_wrapper a"].Select(wrapper => wrapper["href"]).FirstOrDefault();
            //    sourceUri = uri == null ? null : uri.ToString();
            //}

            //Console.ReadLine();
        }
    }
}