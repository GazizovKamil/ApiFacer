﻿using ApiFacer.Classes;
using ApiFacer.DB;
using ApiFacer.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using System.Globalization;
using System.Collections.Immutable;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using ConsoleFaiserScript.Requests;
using System.Security.AccessControl;
using YandexDisk.Client.Protocol;
using YandexDisk.Client;
using ConsoleFaiserScript.DB;
using YandexDisk.Client.Http;

namespace ApiFacer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainController : Controller
    {
        public readonly ApiDB dbContext;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MainController(ApiDB dbContext, IWebHostEnvironment hostEnvironment)
        {
            this.dbContext = dbContext;
            this._hostEnvironment = hostEnvironment;
        }

        [NonAction]
        public async Task<Logins> Session(string sessionkey)
        {
            var user = dbContext.Logins.Where(x => x.sessionkey.Equals(sessionkey)).FirstOrDefault();

            if (user == null)
            {
                return null;
            }
            else
            {
                user.ipAdress = HttpContext.Connection.RemoteIpAddress.ToString();
                await dbContext.SaveChangesAsync();
                return user;
            }
        }

        //[NonAction]
        //async Task<Images> ProcessImage(IFormFile file, string folderPath, int eventId, string eventPath, int authorId)
        //{
        //    var filePath = Path.Combine(folderPath, file.FileName);

        //    using (var stream = System.IO.File.Create(filePath))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    Images images = new Images()
        //    {
        //        path = @"EventFolders\" + eventPath + @"\" + file.FileName,
        //        eventId = eventId,
        //        authorId = authorId
        //    };

        //    return images;
        //}

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> login(Login user)
        {
            var login = dbContext.Users.Where(a => a.login.Equals(user.login) && a.password.Equals(user.password)).FirstOrDefault();

            if (login != null)
            {
                var session = Guid.NewGuid().ToString();
                var check_session = dbContext.Logins.Where(x => x.ipAdress == HttpContext.Connection.RemoteIpAddress.ToString()).FirstOrDefault();
                if (check_session != null)
                {
                    check_session.sessionkey = session;
                    check_session.ipAdress = HttpContext.Connection.RemoteIpAddress.ToString();
                }
                else
                {
                    Logins log = new Logins()
                    {
                        sessionkey = session,
                        userId = login.Id,
                        id_role = login.id_role,
                        ipAdress = HttpContext.Connection.RemoteIpAddress.ToString()
                    };

                    await dbContext.Logins.AddAsync(log);
                }

                await dbContext.SaveChangesAsync();

                return Ok(new { message = "Вход выполнен", status = "ok", session = $"{session}", role = login.id_role });
            }
            return NotFound(new { message = "Нет такого пользователя!", status = "err" });
        }

        //[HttpPost]
        //[Route("create_event")]
        //public async Task<ActionResult> create_event(CreateEvent e)
        //{
        //    var user = await Session(e.sessionkey);

        //    if (user == null)
        //    {
        //        return NotFound(new { message = "Вы не вошли в профиль", status = "err" });
        //    }

        //    // Checking if user has the permissions to create events
        //    if (user.id_role == 1)
        //    {
        //        string mainPath = Path.Combine(_hostEnvironment.WebRootPath, "EventFolders");
        //        string parentFolderPath = string.Empty;
        //        if (e.parentEventId.HasValue)
        //        {
        //            var parentEvent = await dbContext.Events.FindAsync(e.parentEventId);
        //            if (parentEvent == null)
        //            {
        //                return NotFound(new { message = "Родительского мероприятия не существует!", status = "err" });
        //            }
        //            parentFolderPath = Path.Combine(parentEvent.path);
        //        }

        //        string folderPath = Path.Combine(parentFolderPath, e.Name);
        //        string fullFolderPath = Path.Combine(mainPath, folderPath);

        //        Console.WriteLine(fullFolderPath);
        //        // Check if directory already exists
        //        if (!Directory.Exists(fullFolderPath))
        //        {
        //            Directory.CreateDirectory(fullFolderPath);

        //            // Create a new event
        //            Events ev = new Events()
        //            {
        //                Name = e.Name,
        //                ParentEventId = e.parentEventId,
        //                path = folderPath,
        //                authorId = user.userId,
        //            };

        //            // Add the event to the database
        //            await dbContext.Events.AddAsync(ev);
        //            await dbContext.SaveChangesAsync();
        //            return Ok(new { message = "", status = "ok" });
        //        }
        //        else
        //        {
        //            return NotFound(new { message = "Мероприятие с таким названием уже существует!", status = "err" });
        //        }
        //    }
        //    else
        //    {
        //        return NotFound(new { message = "Нет прав!", status = "err" });
        //    }
        //}

        //[HttpPost]
        //[Route("get_events")]
        //public async Task<ActionResult> get_events(SessionRequest session)
        //{
        //    var login = await Session(session.sessionkey);

        //    if (login == null)
        //    {
        //        return NotFound(new { message = "Вы не вошли в профиль", status = "err" });
        //    }

        //    int authorId = login.userId;

        //    if (login.id_role == 1)
        //    {
        //        var ev = await dbContext.Events
        //       .Include(e => e.ParentEvent)
        //       .Include(e => e.Images)
        //       .ToListAsync();
        //        return Ok(new { message = "", status = "ok", events = ev });
        //    }
        //    else
        //    {
        //        var ev = await dbContext.Events
        //        .Where(e => e.authorId == authorId)
        //        .Include(e => e.ParentEvent)
        //        .Include(e => e.Images)
        //        .ToListAsync();
        //        return Ok(new { message = "", status = "ok", events = ev });
        //    }
        //}

        //[HttpPost]
        //[Route("get_user_name")]
        //public async Task<ActionResult> get_user_name(SessionRequest session)
        //{
        //    var login = await Session(session.sessionkey);

        //    if (login == null)
        //    {
        //        return NotFound(new { message = "Вы не вошли в профиль", status = "err" });
        //    }

        //    var user = await dbContext.Users.FirstOrDefaultAsync(x=> x.Id == login.userId);

        //    var fullName = $"{user.surname} {user.first_name} {user.last_name}".Trim();

        //    return Ok(new { status = "ok", fullName = fullName });
        //}

        [HttpGet]
        [Route("get_all_events")]
        public async Task<ActionResult> get_all_events()
        {
            var events = await dbContext.Events
                                .Select(e => new { e.Id, e.Name })
                                .ToListAsync();

            return Ok(new { status = "ok", events = events });
        }

        //[HttpPost]
        //[Route("logout")]
        //public async Task<IActionResult> logout(SessionRequest session)
        //{
        //    var login = Session(session.sessionkey);

        //    if (login == null)
        //    {
        //        return NotFound(new { message = "Вы не вошли в профиль", status = "err" });
        //    }

        //    var log = dbContext.Logins.Where(x => x.sessionkey == session.sessionkey).FirstOrDefault();
        //    dbContext.Logins.Remove(log);
        //    await dbContext.SaveChangesAsync();

        //    return Ok(new { maessage = "Вы вышли!", status = "ok" });
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[Route("login_with_session")]
        //public async Task<ActionResult> login_with_session(SessionRequest session)
        //{
        //    var user = await Session(session.sessionkey);

        //    if (user != null)
        //    {
        //        return Ok(new { session = user.sessionkey, role = user.id_role ,status = "ok" });
        //    }

        //    return NotFound(new { message = "Нет сессии!", status = "err" });
        //}

        //[HttpPost]
        //[Route("add_photographer")]
        //public async Task<ActionResult> add_photographer(AddUser u)
        //{
        //    var user = await Session(u.sessionkey);

        //    if (user == null)
        //    {
        //        return NotFound(new { message = "Вы не вошли в профиль", status = "err" });
        //    }

        //    if (user.id_role == 1)
        //    {
        //        Users ev = new Users()
        //        {
        //            login = u.login,
        //            password = u.password,
        //            first_name = u.first_name,
        //            last_name = u.last_name,
        //            surname = u.surname,
        //            id_role = 2,
        //        };

        //        await dbContext.Users.AddAsync(ev);
        //        await dbContext.SaveChangesAsync();
        //        return Ok(new { message = "", status = "ok" });
        //    }
        //    else
        //    {
        //        return NotFound(new { message = "Нет прав!", status = "err" });
        //    }
        //}

        //curl -X POST http://192.168.137.1:5001/api/Main/add_image_to_event/1 ^
        //-F "files=@C:\Users\kamil\Postman\files\xvAorCW2D_E.jpg;type=image/jpeg" ^
        //-F "files=@C:\Users\kamil\Postman\files\OjZesovKQPI.jpg;type=image/png" ^
        //-F "sessionkey=f28f8165-7a4c-4515-904a-5ba0852c8e56"


        [HttpPost]
        [Route("add_image_to_event/{eventId}")]
        public async Task<ActionResult> add_image_to_event([FromRoute] int eventId, [FromForm] ImageRequest s)
        {
            IDiskApi diskApi = new DiskHttpApi(Config.oauthToken);

            var user = await Session(s.sessionkey);

            if (user == null)
            {
                return NotFound(new { message = "Вы не вошли в профиль", status = "err" });
            }

            if (user.id_role == 1)
            {
                string mainPath = Path.Combine(_hostEnvironment.WebRootPath, "EventFolders");
                var eventEntity = await dbContext.Events.FindAsync(eventId);

                if (eventEntity == null)
                {
                    return NotFound(new { message = "Мероприятие не найдено!", status = "err" });
                }

                string folderPath = Path.Combine(mainPath, eventEntity.path);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                int limit = 1000;
                int offset = 0;

                IEnumerable<Resource> allFilesInFolder = Enumerable.Empty<Resource>();
                Resource fooResourceDescription = await diskApi.MetaInfo.GetInfoAsync(new ResourceRequest
                {
                    Path = s.yandexPath,
                    Limit = limit,
                    Offset = offset,
                }, CancellationToken.None);
                offset += limit;
                allFilesInFolder = allFilesInFolder.Concat(fooResourceDescription.Embedded.Items.Where(item => item.Type == YandexDisk.Client.Protocol.ResourceType.File));

                await TraverseFoldersAsync(s.yandexPath);

                IEnumerable<Task> downloadingTasks =
                allFilesInFolder.Select(async file =>
                {
                    webDavApi.GetPreview(file, file.Path);
                });

                await Task.WhenAll(downloadingTasks);

                await Task.Run(async () =>
                {
                    await vkCloud.ProcessFilesSequentiallyAsync(allFilesInFolder.ToList(), 0, folderPath, dbContext, eventId);
                });

                async Task TraverseFoldersAsync(string folderPath)
                {
                    int limit = 1000;
                    int offset = 0;

                    do
                    {
                        Resource fooResourceDescription = await diskApi.MetaInfo.GetInfoAsync(new ResourceRequest
                        {
                            Path = folderPath,
                            Limit = limit,
                            Offset = offset,
                        }, CancellationToken.None);

                        allFilesInFolder = allFilesInFolder.Concat(fooResourceDescription.Embedded.Items.Where(item => item.Type == YandexDisk.Client.Protocol.ResourceType.File));

                        foreach (var item in fooResourceDescription.Embedded.Items)
                        {
                            if (item.Type == YandexDisk.Client.Protocol.ResourceType.Dir)
                            {
                                await TraverseFoldersAsync(item.Path);
                            }
                        }

                        offset += limit;
                    } while (fooResourceDescription.Embedded.Items.Count == limit);
                }

                return Ok(new { message = "Файлы успешно загружены!", status = "ok" });
            }
            else
            {
                return NotFound(new { message = "Нет прав!", status = "err" });
            }
        }


        [HttpGet]
        [Route("images/{*filepath}")]
        public async Task<IActionResult> GetImage([FromRoute] string filepath)
        {
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath + "/" + filepath);

            if (!System.IO.File.Exists(imagePath))
                return NotFound();

            var image = System.IO.File.OpenRead(imagePath);
            return File(image, "image/jpeg");
        }        

        //[NonAction]
        //async Task CallPythonScriptAsync(string imagePath, string path)
        //{
        //    using (Process process = new Process())
        //    {
        //        process.StartInfo.UseShellExecute = false;
        //        process.StartInfo.FileName = "python";
        //        process.StartInfo.Arguments = $"./Scripts/face_detection.py \"{imagePath}\" \"{path}\"";
        //        process.Start();

        //        await process.WaitForExitAsync();
        //    }
        //}

        //[NonAction]
        //private async Task<List<string>> search_by_face(string imagePath, string path, int event_id)
        //{
        //    List<string> matches = new List<string>();

        //    using (Process process = new Process())
        //    {
        //        process.StartInfo.UseShellExecute = false;
        //        process.StartInfo.RedirectStandardOutput = true;  // Enable to read output
        //        process.StartInfo.FileName = "python";
        //        process.StartInfo.Arguments = $"./Scripts/get_images.py \"{imagePath}\" \"{path}\" \"{event_id}\"";
        //        process.Start();

        //        // Read the output.
        //        string result = process.StandardOutput.ReadToEnd();
        //        process.WaitForExit();

        //        matches = JsonConvert.DeserializeObject<List<string>>(result);
        //        // Now "matches" contains user IDs or other data you decided to output from Python
        //    }
        //    return matches;
        //}

        //curl -X POST -H "Content-Type: multipart/form-data" -F "file=@C:\Users\kamil\OneDrive\Рабочий стол\photo_2023-02-21_15-48-15.jpg" http://192.168.137.1:5001/api/Main/search_face/1

        //[HttpPost]
        //[Route("search_face/{event_id}")]
        //public async Task<ActionResult> search_face([FromRoute] int event_id, [FromForm] SearchRequest s)
        //{
        //    string mainPath = Path.Combine(_hostEnvironment.WebRootPath, "EventFolders");

        //    var events = await dbContext.Events.FindAsync(event_id); 

        //    if (events == null)
        //    {
        //        return BadRequest(new { message = "Нет такого мероприятия.", status = "err" });
        //    }
        //    //Console.WriteLine(Path.Combine(mainPath, eventEntity.path));
        //    string folderPath = Path.Combine(mainPath, "faces");

        //    if (!Directory.Exists(folderPath))
        //    {
        //        Directory.CreateDirectory(folderPath);
        //    }

        //    var supportedTypes = new[] { "image/jpg", "image/jpeg", "image/png" };

        //    if (!supportedTypes.Contains(s.file.ContentType))
        //    {
        //        return BadRequest(new { message = "Неподдерживаемый тип файла. Только .jpg, .jpeg, .png файлы поддерживаются.", status = "err" });
        //    }

        //    string filePath = Path.Combine(folderPath, s.file.FileName);

        //    using (var stream = System.IO.File.Create(filePath))
        //    {
        //        await s.file.CopyToAsync(stream);
        //    }
        //    string fileNameFromEventFolders = Path.Combine("EventFolders\\faces", s.file.FileName);

        //    List<string> matches = await Task.Run(() => search_by_face(Path.Combine(folderPath, s.file.FileName), fileNameFromEventFolders, event_id));

        //    return Ok(new { message = "Файлы успешно загружены!", status = "ok", matches = matches });
        //}

        [HttpPost]
        [Route("search_face/{event_id}")]
        public async Task<ActionResult> search_face([FromRoute] int event_id, [FromForm] SearchRequest s)
        {
            string mainPath = Path.Combine(_hostEnvironment.WebRootPath, "EventFolders");

            var events = await dbContext.Events.FindAsync(event_id);

            if (events == null)
            {
                return BadRequest(new { message = "Нет такого мероприятия.", status = "err" });
            }
            string folderPath = Path.Combine(mainPath, "faces");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var supportedTypes = new[] { "image/jpg", "image/jpeg", "image/png" };

            if (!supportedTypes.Contains(s.file.ContentType))
            {
                return BadRequest(new { message = "Неподдерживаемый тип файла. Только .jpg, .jpeg, .png файлы поддерживаются.", status = "err" });
            }

            string filePath = Path.Combine(folderPath, s.file.FileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await s.file.CopyToAsync(stream);
            }
            string fileNameFromEventFolders = Path.Combine("EventFolders\\faces", s.file.FileName);

            List<string> matches = await vkCloud.RecognizeUser(s.file, event_id, dbContext, _hostEnvironment);

            return Ok(new { message = "Файлы успешно загружены!", status = "ok", matches = matches });
        }
    }
}
