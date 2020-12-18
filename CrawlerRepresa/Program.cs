using CsvHelper;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace CrawlerRepresa
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("/*********************************************************************************/\n" +
                              "/******************************* Represa Crawler ********************************/\n" +
                              "/********************************************************************************/");

            DateTime ultimaExecucao = new DateTime(2020, 01, 01);
            var dataCaptura = String.Empty;

            try
            {
                for (; ; )
                {
                    try
                    {
                        var lstBacias = GetDetalheBacia();
                        
                        if (lstBacias != null)
                        {
                            var elemento = lstBacias.Find(e => e.Reservatorio == "JURUMIRIM");

                            if (DateTime.Compare(DateTime.Now, ultimaExecucao) > 0 && dataCaptura != elemento.Data.ToString())
                            {
                                dataCaptura = elemento.Data.ToString();

                                writeFile(elemento);
                                ultimaExecucao = DateTime.Now;
                                Thread.Sleep(120000);
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Data: " + DateTime.Now.ToString() + " Erro: " + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Data: " + DateTime.Now.ToString() + " Erro: " + e.Message);
            }
        }

        private static void writeFile(Bacias bacia)
        {
            string subPath = @"C:\Users\Public\bacias";

            bool exists = System.IO.Directory.Exists(subPath);

            if (!exists)
                System.IO.Directory.CreateDirectory(subPath);

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(subPath + @"\baciaJurumirim.txt", true))
            {
                string text = DateTime.Now.ToString() + ";" + bacia.Data.ToString() + ";" + bacia.Reservatorio.ToString() + ";" + bacia.ReservatorioValorUtil.Replace(".", ",").ToString();
                file.WriteLine(text);
                file.Dispose();
                file.Close();
            }
        }

        static string GetHtml()
        {
            var url = "http://tr.ons.org.br/Content/Get/SituacaoDosReservatorios";
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);

            return response.Content;
        }

        static List<Bacias> GetDetalheBacia()
        {
            var lstBacias = new List<Bacias>();
            var htmlNode = GetHtml();

            lstBacias = JsonConvert.DeserializeObject<List<Bacias>>(htmlNode);

            return lstBacias;
        }
    }

    public class Bacias
    {
        public string Data { get; set; }
        public string Subsistema { get; set; }
        public string Bacia { get; set; }
        public string Reservatorio { get; set; }
        public string ReservatorioMax { get; set; }
        public string ReservatorioEARVerificadaMWMes { get; set; }
        public string ReservatorioEARVerificadaPorcentagem { get; set; }
        public string ReservatorioValorUtil { get; set; }
        public string ReservatorioPorcentagem { get; set; }
        public string BaciaMax { get; set; }
        public string BaciaEARVerificadaMWMes { get; set; }
        public string BaciaEARVerificadaPorcentagem { get; set; }
        public string BaciaPorcentagem { get; set; }
        public string SubsistemaMax { get; set; }
        public string SubsistemaEARVerificadaMWMes { get; set; }
        public string SubsistemaValorUtil { get; set; }
        public string SINMax { get; set; }
        public string SINEARVerificadaMWMes { get; set; }
        public string SINEARPorcentagem { get; set; }
    }
}
