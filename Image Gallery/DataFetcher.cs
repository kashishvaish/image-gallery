using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Reflection;

namespace Image_Gallery
{
    // Class to fetch the data from server, parse it and provide the data back to the application
    class DataFetcher
    {
        async Task<string> GetDatafromService(string searchstring) 
        { 
            string readText = null; 
            try 
            { 
                String url = @" https://imagefetcherapi.azurewebsites.net/api/fetch_images?query=" + searchstring + "&max_count=5"; 
                using (HttpClient c = new HttpClient()) 
                { 
                    readText = await c.GetStringAsync(url); 
                } 
            } 
            catch 
            {
                var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                var path = Path.Combine(outPutDirectory, "Data\\sampleData.json");
                string rel_path = new Uri(path).LocalPath;
                readText = File.ReadAllText(rel_path); 
            } 
            return readText; 
        }
        public async Task<List<ImageItem>> GetImageData(string search) 
        { 
            string data = await GetDatafromService(search); 
            return JsonConvert.DeserializeObject<List<ImageItem>>(data); 
        }
    }
}
