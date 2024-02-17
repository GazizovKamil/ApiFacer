using ConsoleFaiserScript.DB;
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
    internal class webDavApi
    {
        public static void GetPreview(Resource file, string localFolder)
        {
            try
            {
                var path = file.Path;
                if (path.StartsWith("disk:/"))
                {
                    path = path.Substring(6);
                }
                var request = (HttpWebRequest)WebRequest.Create("https://webdav.yandex.ru/" + path + "?preview&size=1920x");
                request.Method = "GET";
                request.Headers.Add("Authorization", "OAuth " + Config.oauthToken);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var fileStream = new FileStream(Path.Combine(localFolder, file.Name), FileMode.Create))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("https://webdav.yandex.ru/" + file.Path + "?preview&size=1920x");
                Console.WriteLine(ex);
            }
        }
    }
}
