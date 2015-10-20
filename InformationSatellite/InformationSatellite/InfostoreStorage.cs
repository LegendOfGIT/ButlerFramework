using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using System.Xml.Linq;

using Butler.Shared;

namespace Infosatellite
{
    public class InfostoreStorage : Storage
    {
        private Uri InfostoreEndpoint { get; set; }

        public InfostoreStorage ()
	    {
            string sGet = WebConfigurationManager.AppSettings["Infostore_StoreURL"];
            InfostoreEndpoint = string.IsNullOrEmpty(sGet) ? null : new Uri(sGet); 
	    }

        public override void Store(KeyValuePair<string, Dictionary<string, List<string>>> kvpInfos, XDocument xdTemplate)
        {
            List<InfoItemType> items = new List<InfoItemType>();

            if (kvpInfos.Value != null)
            {
                InfoItemType item = new InfoItemType { Id = kvpInfos.Key.Replace("'", "´") ?? string.Empty };
                string infotyp = string.Empty;
                if (kvpInfos.Value != null){
                    List<InfoDetailType> details = new List<InfoDetailType>();
                    foreach (KeyValuePair<string, List<string>> kvpInfoDetail in kvpInfos.Value)
                    {
                        string infoid = kvpInfoDetail.Key;
                        if(infoid.Equals("Infotyp"))
                            infotyp = kvpInfoDetail.Value[0];
                        else
                            foreach (string wert in kvpInfoDetail.Value)
                                details.Add(new InfoDetailType { Id = infoid, Wert = wert });
                    }
                    item.Details = details.ToArray();
                }

                item.Typ = infotyp;

                items.Add(item);
            }

            if (InfostoreEndpoint != null)
            {
                string json = new JavaScriptSerializer().Serialize(items);

                HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(InfostoreEndpoint.AbsoluteUri); request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version11;
                request.Method = "POST";


                // turn our request string into a byte stream
                byte[] postBytes = Encoding.UTF8.GetBytes(json);

                // this is important - make sure you specify type this way
                request.ContentType = "application/json; charset=UTF-8";
                request.Accept = "application/json";
                request.ContentLength = postBytes.Length;
                Stream requestStream = request.GetRequestStream();

                // now send it
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();
            }
        }
    }
}