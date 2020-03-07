using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace YahooFin
{
   class Program
   {
      public static async Task DownloadData(string symbol, long startDate, long endDate, string crumb, HttpClient httpClient)
      {
         var url = $"https://query1.finance.yahoo.com/v7/finance/download/{symbol}?period1={startDate}&period2={endDate}&interval=1d&events=history&crumb={crumb}";
         var response = await httpClient.GetAsync(url);

         string responseBody = await response.Content.ReadAsStringAsync(); ;
         Console.WriteLine(responseBody);
         //System.out.println(url);

         //request.addHeader("User-Agent", "Mozilla/5.0 (X11; U; Linux x86_64; en-US; rv:1.9.2.13) Gecko/20101206 Ubuntu/10.10 (maverick) Firefox/3.6.13");
         //try
         //{
         //   HttpResponse response = client.execute(request, context);
         //   System.out.println("Response Code : " + response.getStatusLine().getStatusCode());
         //   HttpEntity entity = response.getEntity();

         //   String reasonPhrase = response.getStatusLine().getReasonPhrase();
         //   int statusCode = response.getStatusLine().getStatusCode();

         //   System.out.println(String.format("statusCode: %d", statusCode));
         //   System.out.println(String.format("reasonPhrase: %s", reasonPhrase));

         //   if (entity != null)
         //   {
         //      BufferedInputStream bis = new BufferedInputStream(entity.getContent());
         //      BufferedOutputStream bos = new BufferedOutputStream(new FileOutputStream(new File(filename)));
         //      int inByte;
         //      while ((inByte = bis.read()) != -1)
         //         bos.write(inByte);
         //      bis.close();
         //      bos.close();
         //   }
         //   HttpClientUtils.closeQuietly(response);

         //}
         //catch (Exception ex)
         //{
         //   System.out.println("Exception");
         //   System.out.println(ex);
         //}
      }

      public static void Main(string[] args)
      {
         var httpClient = new HttpClient();
         httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36");
         var jsonObject = new JObject();

         var task = Task.Run(async () =>
         {
            var response = await httpClient.GetAsync("https://au.finance.yahoo.com/quote/API.AX/history?p=API.AX");
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync(); ;


            var start = responseBody.IndexOf("CrumbStore", StringComparison.Ordinal);
            start = responseBody.IndexOf("crumb", start, StringComparison.Ordinal);
            var end = responseBody.IndexOf('}', start);
            var result = responseBody.Substring(start - 1, end - start + 2).Replace("\"", "'")
               .Insert(0, "{");
            jsonObject = JObject.Parse(result);
            
         });

         task.Wait();
         var taskTwo = Task.Run(() => DownloadData("LNK.AX", 1551683555, 1583305955, jsonObject["crumb"].ToString(), httpClient));
         taskTwo.Wait();
      }


      //static void Main(string[] args)
      //{
      //   var request = WebRequest.Create("https://au.finance.yahoo.com/quote/API.AX/history?p=API.AX") as HttpWebRequest;
      //   string cookieHeader = "";
      //   String cookieValue = "";
      //   JObject jsonObject = new JObject();

      //   if (request != null)
      //   {
      //      var rawResponse = request.GetResponse();
      //      var response = (HttpWebResponse)rawResponse;

      //      cookieHeader = response.Headers[HttpResponseHeader.SetCookie];
      //      var results = cookieHeader.Split(";");
      //      cookieValue = results[0].Substring(2, results[0].Length - 3);


      //      string responseText = "";
      //      var encoding = ASCIIEncoding.ASCII;
      //      using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
      //      {
      //         responseText = reader.ReadToEnd();
      //      }

      //      var start = responseText.IndexOf("CrumbStore", StringComparison.Ordinal);
      //      start = responseText.IndexOf("crumb", start, StringComparison.Ordinal);
      //      var end = responseText.IndexOf('}', start);
      //      var result = responseText.Substring(start - 1, end - start + 2).Replace("\"", "'")
      //         .Insert(0, "{");
      //      jsonObject = JObject.Parse(result);
      //      Console.WriteLine(result);

      //      Console.WriteLine(jsonObject["crumb"]);

      //      Console.WriteLine(cookieValue);
      //   }

      //   var crumb = jsonObject["crumb"];
      //   request = WebRequest.Create(
      //      $"https://query1.finance.yahoo.com/v7/finance/download/API.AX?period1=1551683555&period2=1583305955&interval=1d&events=history&crumb={crumb}") as HttpWebRequest;
      //   if (request.CookieContainer == null)
      //   {
      //      request.CookieContainer = new CookieContainer();
      //   }

      //   var cookieToSet = new Cookie("B", cookieValue,"/",".yahoo.com");

      //   request.CookieContainer.Add(cookieToSet);
      //   var someResponse = (HttpWebResponse)request.GetResponse();
      //   Console.WriteLine(someResponse);

      //}
   }
}
