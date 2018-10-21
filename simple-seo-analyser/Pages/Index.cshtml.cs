using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace simple_seo_analyser
{

    public class IndexModel : PageModel
    {

        [BindProperty]
        public String Input { get; set; }
        [BindProperty]
        public bool EnableURL { get; set; } = true;
        [BindProperty]
        public bool EnableKeywords { get; set; } = true;
        [BindProperty]
        public bool EnableWordCounts { get; set; } = true;

        public void OnGet()
        {
        }

        public void OnPost()
        {
            ViewData["Input"] = Input;
            Dictionary<string, int> result;

            try
            {
                HtmlNode node = GetHtmlNode(Input);

                if (EnableKeywords)
                {
                    String keywords = node
                    .SelectSingleNode(@"//meta[@name='keywords']/@content")
                    .GetAttributeValue("content", "No keywords found");
                    ViewData["Keywords"] = keywords;
                }

                if (EnableURL)
                {
                    HtmlNodeCollection urls = node.SelectNodes("//a[@href]");
                    ViewData["UrlCount"] = urls.Count;
                }

                if (EnableWordCounts)
                {
                    result = SEOAnalyze(node);
                    ViewData["Result"] = result;
                }
            }
            catch
            {
                ViewData["ErrorMsg"] = "Cannot process URL";
            }


        }

        public static HtmlNode GetHtmlNode(string file)
        {
            bool isValidUrl = Uri.IsWellFormedUriString(file, UriKind.RelativeOrAbsolute);

            HtmlNode documentNode;
            if (isValidUrl)
            {
                var web = new HtmlWeb();
                var htmlDoc = web.Load(file);
                documentNode = htmlDoc.DocumentNode;
            }
            else
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(file);
                documentNode = doc.DocumentNode;
            }

            return documentNode;
        }

        public static Dictionary<string, int> SEOAnalyze(HtmlNode documentNode)
        {
            var counts = new Dictionary<string, int>();
            var stopWords = new HashSet<string> { "a", "about", "above", "after", "again", "against", "all", "am", "an", "and", "any", "are", "as", "at", "be", "because", "been", "before", "being", "below", "between", "both", "but", "by", "could", "did", "do", "does", "doing", "down", "during", "each", "few", "for", "from", "further", "had", "has", "have", "having", "he", "he'd", "he'll", "he's", "her", "here", "here's", "hers", "herself", "him", "himself", "his", "how", "how's", "i", "i'd", "i'll", "i'm", "i've", "if", "in", "into", "is", "it", "it's", "its", "itself", "let's", "me", "more", "most", "my", "myself", "nor", "of", "on", "once", "only", "or", "other", "ought", "our", "ours", "ourselves", "out", "over", "own", "same", "she", "she'd", "she'll", "she's", "should", "so", "some", "such", "than", "that", "that's", "the", "their", "theirs", "them", "themselves", "then", "there", "there's", "these", "they", "they'd", "they'll", "they're", "they've", "this", "those", "through", "to", "too", "under", "until", "up", "very", "was", "we", "we'd", "we'll", "we're", "we've", "were", "what", "what's", "when", "when's", "where", "where's", "which", "while", "who", "who's", "whom", "why", "why's", "with", "would", "you", "you'd", "you'll", "you're", "you've", "your", "yours", "yourself", "yourselves" };

            HtmlNodeCollection body = documentNode.SelectNodes("//article//*[self::p or self::h1 or self::h2 or self::h3 or self::h4 or self::h5 or self::h6]");

            if (body == null)
            {
                body = documentNode.SelectNodes("//body//*[self::p or self::h1 or self::h2 or self::h3 or self::h4 or self::h5 or self::h6]");
            }

            if (body == null)
            {
                // unable to parse out anything):
                return counts;
            }

            StringBuilder b = new StringBuilder();
            foreach (var v in body)
            {
                b.Append(v.InnerText);
                b.Append(" ");
            }
            var contents = Regex.Replace(b.ToString(), "&#8217;", "'");
            contents = Regex.Replace(contents, @"(\&[a-z0-9#]*\;)", "");
            MatchCollection matches = Regex.Matches(contents, @"\b[\w\+'’]*\b");


            foreach (Match match in matches)
            {
                if (match.Value == "") continue;
                String word = match.Value.ToLower();
                if (!stopWords.Contains(word))
                {
                    if (counts.TryGetValue(word, out int value))
                    {
                        counts[word] = value + 1;
                    }
                    else
                    {
                        counts[word] = 1;
                    }
                }
            }

            //foreach (KeyValuePair<string, int> entry in counts)
            //{
            //    Console.WriteLine("{0}: {1}", entry.Key, entry.Value);
            //}

            return counts;
        }


    }
}
