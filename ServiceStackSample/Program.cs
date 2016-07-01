using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using ServiceStack.Text;

namespace ServiceStackSample
{
    class Program
    {
        static string _address = "http://demos.telerik.com/kendo-ui/service/Products?callback=";
        static void Main(string[] args)
        {
            // Create an HttpClient instance
            HttpClient client = new HttpClient();
            List<Product> result;

            // Send a request asynchronously continue when complete
            client.GetAsync(_address).ContinueWith(
                (requestTask) =>
                {
                    // Get HTTP response from completed task.
                    HttpResponseMessage response = requestTask.Result;

                    // Check that response was successful or throw exception
                    response.EnsureSuccessStatusCode();

                    // Read response asynchronously as JsonValue and write out top facts for each country
                    response.Content.ReadAsStreamAsync().ContinueWith(
                        (readTask) =>
                        {
                            using (Stream s = readTask.Result)
                            using (TextWriter sr = new StreamWriter("out.csv"))
                            {
                                result = JsonSerializer.DeserializeFromStream<List<Product>>(s);
                                CsvSerializer.SerializeToWriter<List<Product>>(result, sr);
                            }
                            foreach (Product product in result)
                            {
                                Console.WriteLine(product.ProductName);
                            }
                        });
                });

            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }
    }

    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public bool Discontinued { get; set; }
    }
}
