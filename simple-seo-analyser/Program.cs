using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Text;

namespace simple_seo_analyser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var stopWords = new HashSet<string> { "a", "about", "above", "after", "again", "against", "all", "am", "an", "and", "any", "are", "as", "at", "be", "because", "been", "before", "being", "below", "between", "both", "but", "by", "could", "did", "do", "does", "doing", "down", "during", "each", "few", "for", "from", "further", "had", "has", "have", "having", "he", "he'd", "he'll", "he's", "her", "here", "here's", "hers", "herself", "him", "himself", "his", "how", "how's", "i", "i'd", "i'll", "i'm", "i've", "if", "in", "into", "is", "it", "it's", "its", "itself", "let's", "me", "more", "most", "my", "myself", "nor", "of", "on", "once", "only", "or", "other", "ought", "our", "ours", "ourselves", "out", "over", "own", "same", "she", "she'd", "she'll", "she's", "should", "so", "some", "such", "than", "that", "that's", "the", "their", "theirs", "them", "themselves", "then", "there", "there's", "these", "they", "they'd", "they'll", "they're", "they've", "this", "those", "through", "to", "too", "under", "until", "up", "very", "was", "we", "we'd", "we'll", "we're", "we've", "were", "what", "what's", "when", "when's", "where", "where's", "which", "while", "who", "who's", "whom", "why", "why's", "with", "would", "you", "you'd", "you'll", "you're", "you've", "your", "yours", "yourself", "yourselves" };
            var doc = new HtmlDocument();
            doc.Load("test.html");
            HtmlNodeCollection body = doc.DocumentNode.SelectNodes("//article//*[self::p or self::h1 or self::h2 or self::h3 or self::h4 or self::h5 or self::h6]");
            StringBuilder b = new StringBuilder();
            foreach (var v in body)
            {
                b.Append(v.InnerText);
                b.Append(" ");
            }
            var contents = Regex.Replace(b.ToString(), "&#8217;", "'");
            contents = Regex.Replace(contents, @"(\&[a-z0-9#]*\;)", "");
            MatchCollection matches = Regex.Matches(contents, @"\b[\w\+'’]*\b");

            var counts = new Dictionary<string, int>();
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

            foreach (KeyValuePair<string, int> entry in counts)
            {
                Console.WriteLine("{0}: {1}", entry.Key, entry.Value);
            }
            //CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
