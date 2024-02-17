using ApiFacer.DB;
using ConsoleFaiserScript.Classes;
using ConsoleFaiserScript.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YandexDisk.Client.Protocol;

namespace ConsoleFaiserScript.Requests
{
    internal class vkCloud
    {
        public static async Task Recognize(string imgPath, string yandexPath, ApiDB apiDB, int eventId)
        {
            string url = $"https://smarty.mail.ru/api/v1/persons/recognize?oauth_token={Config.vkCloud}&oauth_provider=mcs";

            string contentType = GetContentType(imgPath);

            // Check if the content type is supported
            if (contentType == null)
            {
                Console.WriteLine("Unsupported image format. Supported formats: jpeg, png, tiff");
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    byte[] fileBytes = File.ReadAllBytes(imgPath);
                    var fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.Add("Content-Type", contentType);
                    formData.Add(fileContent, "file", Path.GetFileName(imgPath));

                    string metaJson = "{\"space\": \"0\", \"create_new\": true, \"images\": [{ \"name\": \"file\" }]}";
                    formData.Add(new StringContent(metaJson), "meta");

                    HttpResponseMessage response = await client.PostAsync(url, formData);

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var root = JsonConvert.DeserializeObject<Root>(responseBody);

                    if (root.status == 200)
                    {
                        foreach (var obj in root.body.objects)
                        {
                            if (obj.persons != null)
                            {
                                foreach (var person in obj.persons)
                                {
                                    int personId = int.Parse(person.tag.Substring(6));
                                    await dbRequest.SaveUserImages(personId, yandexPath, imgPath, person, apiDB, eventId);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static async Task<List<string>> RecognizeUser(IFormFile imgPath, int eventId, ApiDB apiDB, IWebHostEnvironment _hostEnvironment)
        {
            string url = $"https://smarty.mail.ru/api/v1/persons/recognize?oauth_token={Config.vkCloud}&oauth_provider=mcs";

            string contentType = GetContentType(imgPath.FileName);

            if (contentType == null)
            {
                Console.WriteLine("Unsupported image format. Supported formats: jpeg, png, tiff");
                return new List<string>();
            }

            using (HttpClient client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    using (var stream = imgPath.OpenReadStream())
                    {
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.Add("Content-Type", contentType);
                        formData.Add(fileContent, "file", Path.GetFileName(imgPath.FileName));

                        string metaJson = "{\"space\": \"0\", \"create_new\": true, \"images\": [{ \"name\": \"file\" }]}";
                        formData.Add(new StringContent(metaJson), "meta");

                        HttpResponseMessage response = await client.PostAsync(url, formData);

                        string responseBody = await response.Content.ReadAsStringAsync();
                        var root = JsonConvert.DeserializeObject<Root>(responseBody);

                        if (root.status == 200)
                        {
                            foreach (var obj in root.body.objects)
                            {
                                if (obj.persons != null)
                                {
                                    int personId = int.Parse(obj.persons[0].tag.Substring(6));
                                    List<string> list = apiDB.UserImages
                                    .Where(x => x.userId == personId && x.eventId == eventId)
                                    .Select(n => n.localPath.StartsWith(_hostEnvironment.WebRootPath) ? n.localPath.Substring(_hostEnvironment.WebRootPath.Length) : n.localPath)
                                    .ToList();

                                    return list;
                                }
                                else
                                {
                                    return new List<string>();
                                }
                            }
                        }
                    }
                }
            }
            return new List<string>();
        }

        public static async Task ProcessFilesSequentiallyAsync(List<Resource> files, int index, string localFolder, ApiDB apiDB, int eventId)
        {
            if (index < files.Count)
            {
                Resource file = files[index];
                string localPath = Path.Combine(localFolder, file.Name);
                await Recognize(localPath, file.Path, apiDB, eventId);
                await ProcessFilesSequentiallyAsync(files, index + 1, localFolder, apiDB, eventId);
            }
        }

        private static string GetContentType(string filePath)
        {
            string extension = Path.GetExtension(filePath)?.ToLower();
            switch (extension)
            {
                case ".jpeg":
                case ".jpg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".tiff":
                case ".tif":
                    return "image/tiff";
                default:
                    return null;
            }
        }
    }
}
