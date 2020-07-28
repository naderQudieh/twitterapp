using Microsoft.Extensions.Configuration;
using NeoSmart.Unicode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Tweetinvi;
using Tweetinvi.Models;

namespace ConsoleApp3
{

    class Program
    {
        static public Queue<ITweet> queue = new Queue<ITweet>();
        static void Main(string[] args)
        {
            IConfiguration _config = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json", true, true)
                 .Build();
            string _credsConsumerKey = _config.GetSection("TwitterCredentials")["ConsumerKey"];
            string _credsConsumerSecret = _config.GetSection("TwitterCredentials")["ConsumerSecret"];
            string _credsAccessToken = _config.GetSection("TwitterCredentials")["AccessToken"];
            string _credsAccessTokenSecret = _config.GetSection("TwitterCredentials")["AccessSecret"];
            CreateSampleStream(_config);
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
        public static void CreateSampleStream(IConfiguration _config)
        { 
            Tweetinvi.Models.TwitterCredentials twitterCredentials = new Tweetinvi.Models.TwitterCredentials();
            twitterCredentials.AccessToken = _config.GetSection("TwitterCredentials")["AccessToken"];
            twitterCredentials.AccessTokenSecret = _config.GetSection("TwitterCredentials")["AccessSecret"];
            twitterCredentials.ConsumerKey = _config.GetSection("TwitterCredentials")["ConsumerKey"];
            twitterCredentials.ConsumerSecret = _config.GetSection("TwitterCredentials")["ConsumerSecret"];
            var fs = Tweetinvi.Stream.CreateSampleStream(twitterCredentials);
           // fs.AddTweetLanguageFilter(LanguageFilter.English); 
            fs.TweetReceived += (sender, args) => {
                Debug.WriteLine("Tweet Received");
                GetTweet(args.Tweet);
            };
            fs.StreamStopped += (sender, args) =>
            {
                var lastException = ExceptionHandler.GetLastException();
                if (lastException != null)
                {
                    Debug.WriteLine("An error occurred");
                    ExceptionHandler.ClearLoggedExceptions();
                }
                Debug.WriteLine("Stream Stopped at " + DateTime.Now.ToLongTimeString());
            };
            fs.WarningFallingBehindDetected += (sender, args) =>
            {
                Debug.WriteLine("Falling behind: {0}", args.WarningMessage);
            };
            fs.LimitReached += (sender, args) =>
            {
                Debug.WriteLine("Limit reached: {0}", args.NumberOfTweetsNotReceived);
            };
            fs.DisconnectMessageReceived += (sender, args) =>
            {
                Debug.WriteLine("Disconnect: {0}", args.DisconnectMessage);
            };

            fs.StartStreamAsync();




            //stream.StartStreamAsync();
        }

        public static void CreateFilteredStream()
        {
            var stream = Tweetinvi.Stream.CreateFilteredStream();
            stream.MatchingTweetReceived += (sender, args) => {
                GetTweet(args.Tweet);
            }; 
            // stream.AddTrack("SCP.Net");
            stream.StartStreamMatchingAnyConditionAsync();
        }
        public static void GetTweet(ITweet tweet)
        {
            Console.WriteLine(tweet.Text);
            bool isYes = Emoji.IsEmoji(tweet.Text, 20);
            if (isYes)
            {
                Debug.WriteLine("Limit reached: {0}", isYes);
            };
            queue.Enqueue(tweet);
            Console.WriteLine("•	Total number of tweets received  : {0}", queue.Count);
            if (queue.Count > 1000)
            {
                Debug.WriteLine("Limit reached: {0}", 1000);
            };
        }
        //private static bool  isEmoji(String message)
        //{
        //    return Regex.Matches(message,"(?:[\uD83C\uDF00-\uD83D\uDDFF]|[\uD83E\uDD00-\uD83E\uDDFF]|" +
        //            "[\uD83D\uDE00-\uD83D\uDE4F]|[\uD83D\uDE80-\uD83D\uDEFF]|" +
        //            "[\u2600-\u26FF]\uFE0F?|[\u2700-\u27BF]\uFE0F?|\u24C2\uFE0F?|" +
        //            "[\uD83C\uDDE6-\uD83C\uDDFF]{1,2}|" +
        //            "[\uD83C\uDD70\uD83C\uDD71\uD83C\uDD7E\uD83C\uDD7F\uD83C\uDD8E\uD83C\uDD91-\uD83C\uDD9A]\uFE0F?|" +
        //            "[\u0023\u002A\u0030-\u0039]\uFE0F?\u20E3|[\u2194-\u2199\u21A9-\u21AA]\uFE0F?|[\u2B05-\u2B07\u2B1B\u2B1C\u2B50\u2B55]\uFE0F?|" +
        //            "[\u2934\u2935]\uFE0F?|[\u3030\u303D]\uFE0F?|[\u3297\u3299]\uFE0F?|" +
        //            "[\uD83C\uDE01\uD83C\uDE02\uD83C\uDE1A\uD83C\uDE2F\uD83C\uDE32-\uD83C\uDE3A\uD83C\uDE50\uD83C\uDE51]\uFE0F?|" +
        //            "[\u203C\u2049]\uFE0F?|[\u25AA\u25AB\u25B6\u25C0\u25FB-\u25FE]\uFE0F?|" +
        //            "[\u00A9\u00AE]\uFE0F?|[\u2122\u2139]\uFE0F?|\uD83C\uDC04\uFE0F?|\uD83C\uDCCF\uFE0F?|" +
        //            "[\u231A\u231B\u2328\u23CF\u23E9-\u23F3\u23F8-\u23FA]\uFE0F?)+");
        //}


    }
}
