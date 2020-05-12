using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var fundation = new XmlDocument();
            fundation.Load(@"c:\temp\fundation-request.xml");

            var fusion = new XmlDocument();
            fusion.Load(@"c:\temp\fusion-request.xml");

            var fundationNodes = fundation.SelectNodes("/Request")[0];

            var fusionNodes = fusion.SelectNodes("/Request")[0];

            var fundationNodesWithValue = new Dictionary<string, string>();
            var fundationNodesWithoutValue = new List<string>();

            var fusionNodesWithValue = new Dictionary<string, string>();
            var fusionNodesWithoutValue = new List<string>();

            foreach (XmlNode node in fundationNodes)
            {
                if(string.IsNullOrEmpty(node.InnerText))
                {
                    fundationNodesWithoutValue.Add(node.Name);
                }
                else
                {
                    fundationNodesWithValue.Add(node.Name, node.InnerText);
                }                
            }

            foreach (XmlNode node in fusionNodes)
            {
                if (string.IsNullOrEmpty(node.InnerText))
                {
                    fusionNodesWithoutValue.Add(node.Name);
                }
                else
                {
                    fusionNodesWithValue.Add(node.Name, node.InnerText);
                }
            }


            var bothWithoutValue = fusionNodesWithoutValue.Where(x => fundationNodesWithoutValue.Contains(x)).ToList();
            var withoutValueFusionVsFundation = fusionNodesWithoutValue.Where(x => !fundationNodesWithoutValue.Contains(x)).ToList();
            var withoutValueFundationVsFusion = fundationNodesWithoutValue.Where(x => !fusionNodesWithoutValue.Contains(x)).ToList();

            var html = "<html><header><style>table, tr, td {{ border: 1px solid black; }} </style></header><body><h1>Summary</h1>{0}</body></html>";

            var content1 = "<h3>No Value - Both </h3><table>{0}</table><br />";
            content1 += "<h3>No Value - Only Fusion </h3><table>{1}</table><br /> ";
            content1 += "<h3>No Value - Only Fundation </h3><table>{2}</table><br /> ";
            content1 += "<h3>Filled Both</h3><table>{3}</table><br /> ";
            content1 += "<h3>Filled Only Fundation</h3><table>{4}</table><br /> ";
            content1 += "<h3>Filled Only Fusion</h3><table>{5}</table><br /> ";

            var fieldsBothWithoutValue = "";            

            foreach (var field in bothWithoutValue)
            {
                fieldsBothWithoutValue += string.Format("<tr><td>{0}</td></tr>", field);
            }

            var fieldsFusionWithoutValue = "";

            foreach (var field in withoutValueFusionVsFundation)
            {
                fieldsFusionWithoutValue += string.Format("<tr><td>{0}</td></tr>", field);
            }

            var fieldsFundationWithoutValue = "";

            foreach (var field in withoutValueFundationVsFusion)
            {
                fieldsFundationWithoutValue += string.Format("<tr><td>{0}</td></tr>", field);
            }            


            var filledBoth = fundationNodesWithValue.Where(x => fusionNodesWithValue.ContainsKey(x.Key)).Select(x => new { Key = x.Key, Fusion = fusionNodesWithValue[x.Key], Fundation = x.Value }).ToList();

            var filledBothFields = string.Format("<tr><td><b>{0}</b></td><td><b>{1}</b></td><td><b>{2}</b></td></tr>", "Node Name", "Fusion", "Fundation");

            foreach (var field in filledBoth)
            {
                filledBothFields += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", field.Key, field.Fusion, field.Fundation);
            }


            var filleOnlyFundation = fundationNodesWithValue.Where(x => !fusionNodesWithValue.ContainsKey(x.Key)).ToList();

            var filleOnlyFundationFields = string.Format("<tr><td><b>{0}</b></td><td><b>{1}</b></td></tr>", "Node Name", "Fundation");

            foreach (var field in filleOnlyFundation)
            {
                filleOnlyFundationFields += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", field.Key, field.Value);
            }


            var filledOnlyFusion = fusionNodesWithValue.Where(x => !fundationNodesWithValue.ContainsKey(x.Key)).ToList();

            var filledOnlyFusionFields = string.Format("<tr><td><b>{0}</b></td><td><b>{1}</b></td></tr>", "Node Name", "Fundation");

            foreach (var field in filledOnlyFusion)
            {
                filledOnlyFusionFields += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", field.Key, field.Value);
            }


            content1 = string.Format(content1, fieldsBothWithoutValue, fieldsFusionWithoutValue, fieldsFundationWithoutValue, filledBothFields, filleOnlyFundationFields, filledOnlyFusionFields);

            html = string.Format(html, content1);


            File.WriteAllText("c:\\temp\\summary.html", html);
        }
    }
}
