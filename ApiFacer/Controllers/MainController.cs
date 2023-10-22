using ApiFacer.Classes;
using ApiFacer.DB;
using ApiFacer.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using DlibDotNet;
using DlibDotNet.Dnn;
using DlibDotNet.Extensions;
using Emgu.CV.Features2D;
using System.Text;
using Newtonsoft.Json;
using System.Globalization;

namespace ApiFacer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainController : Controller
    {
        private readonly ApiDB dbContext;
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

        [NonAction]
        async Task<Images> ProcessImage(IFormFile file, string folderPath, int eventId, string eventPath, int authorId)
        {
            var filePath = Path.Combine(folderPath, file.FileName);
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            Images images = new Images()
            {
                path = "EventFolders/" + eventPath + "/" + file.FileName,
                eventId = eventId,
                authorId = authorId
            };

            return images;
        }

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

        [HttpPost]
        [Route("create_event")]
        public async Task<ActionResult> create_event(CreateEvent e)
        {
            var user = await Session(e.sessionkey);

            if (user == null)
            {
                return NotFound(new { message = "Вы не вошли в профиль", status = "err" });
            }

            // Checking if user has the permissions to create events
            if (user.id_role == 1)
            {
                string mainPath = Path.Combine(_hostEnvironment.WebRootPath, "EventFolders");
                string parentFolderPath = string.Empty;
                if (e.parentEventId.HasValue)
                {
                    var parentEvent = await dbContext.Events.FindAsync(e.parentEventId);
                    if (parentEvent == null)
                    {
                        return NotFound(new { message = "Родительского мероприятия не существует!", status = "err" });
                    }
                    parentFolderPath = Path.Combine(parentEvent.path);
                }

                string folderPath = Path.Combine(parentFolderPath, e.Name);
                string fullFolderPath = Path.Combine(mainPath, folderPath);

                Console.WriteLine(fullFolderPath);
                // Check if directory already exists
                if (!Directory.Exists(fullFolderPath))
                {
                    Directory.CreateDirectory(fullFolderPath);

                    // Create a new event
                    Events ev = new Events()
                    {
                        Name = e.Name,
                        ParentEventId = e.parentEventId,
                        path = folderPath,
                        authorId = user.userId,
                    };

                    // Add the event to the database
                    await dbContext.Events.AddAsync(ev);
                    await dbContext.SaveChangesAsync();
                    return Ok(new { message = "", status = "ok" });
                }
                else
                {
                    return NotFound(new { message = "Мероприятие с таким названием уже существует!", status = "err" });
                }
            }
            else
            {
                return NotFound(new { message = "Нет прав!", status = "err" });
            }

        }


        [HttpPost]
        [Route("get_events")]
        public async Task<ActionResult> get_events(SessionRequest session)
        {
            var login = await Session(session.sessionkey);

            if (login == null)
            {
                return NotFound(new { message = "Вы не вошли в профиль", status = "err" });
            }

            int authorId = login.userId;

            if (login.id_role == 1)
            {
                var ev = await dbContext.Events
               .Include(e => e.ParentEvent)
               .Include(e => e.Images)
               .ToListAsync();
                return Ok(new { message = "", status = "ok", events = ev });
            }
            else
            {
                var ev = await dbContext.Events
                .Where(e => e.authorId == authorId)
                .Include(e => e.ParentEvent)
                .Include(e => e.Images)
                .ToListAsync();
                return Ok(new { message = "", status = "ok", events = ev });
            }
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> LogOut(SessionRequest session)
        {
            var login = Session(session.sessionkey);

            if (login == null)
            {
                return NotFound(new { message = "Вы не вошли в профиль", status = "err" });
            }

            var log = dbContext.Logins.Where(x => x.sessionkey == session.sessionkey).FirstOrDefault();
            dbContext.Logins.Remove(log);
            await dbContext.SaveChangesAsync();

            return Ok(new { maessage = "Вы вышли!", status = "ok" });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login_with_session")]
        public async Task<ActionResult> login_with_session(SessionRequest session)
        {
            var user = await Session(session.sessionkey);

            if (user != null)
            {
                return Ok(new { session = user.sessionkey, role = user.id_role ,status = "ok" });
            }

            return NotFound(new { message = "Нет сессии!", status = "err" });
        }

        [HttpPost]
        [Route("add_photographer")]
        public async Task<ActionResult> add_photographer(AddUser u)
        {
            var user = await Session(u.sessionkey);

            if (user == null)
            {
                return NotFound(new { message = "Вы не вошли в профиль", status = "err" });
            }

            if (user.id_role == 1)
            {
                Users ev = new Users()
                {
                    login = u.login,
                    password = u.password,
                    first_name = u.first_name,
                    last_name = u.last_name,
                    surname = u.surname,
                    id_role = 2,
                };

                await dbContext.Users.AddAsync(ev);
                await dbContext.SaveChangesAsync();
                return Ok(new { message = "", status = "ok" });
            }
            else
            {
                return NotFound(new { message = "Нет прав!", status = "err" });
            }
        }

        //curl - X POST http://192.168.137.1:5001/api/Main/add_image_to_event/2 ^
        //-F "files=@C:\Users\kamil\Postman\files\OjZesovKQPI.jpg;type=image/jpeg" ^
        //-F "files=@C:\Users\kamil\Postman\files\xvAorCW2D_E.jpg;type=image/png" ^
        //-F "sessionkey=aa101797-a3cf-4460-8210-6f64a09c5199"

        [HttpPost]
        [Route("add_image_to_event/{eventId}")]
        public async Task<ActionResult> add_image_to_event([FromRoute] int eventId, [FromForm]ImageRequest s)
        {
            var user = await Session(s.sessionkey);

            if (user == null)
            {
                return NotFound(new { message = "Вы не вошли в профиль", status = "err" });
            }

            if (user.id_role == 2 || user.id_role == 1)
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

                foreach (var file in s.files)
                {
                    var supportedTypes = new[] { "image/jpg", "image/jpeg", "image/png" };

                    if (!supportedTypes.Contains(file.ContentType))
                    {
                        return BadRequest(new { message = "Неподдерживаемый тип файла. Только .jpg, .jpeg, .png файлы поддерживаются.", status = "err" });
                    }

                    Images image = await ProcessImage(file, folderPath, eventId, eventEntity.path,user.userId);
                    await dbContext.AddAsync(image);
                    List<Users> users =  await DetectAndSaveFace(Path.Combine(folderPath, file.FileName));

                    if (users.Count > 0)
                    {
                        foreach (Users u in users)
                        {
                            var userImage = new UserImages
                            {
                                ImageId = image.Id,
                                UserId = u.Id
                            };

                            await dbContext.UserImages.AddAsync(userImage);
                        }
                    }
                }

                await dbContext.SaveChangesAsync();

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

        [NonAction]
        public async Task<List<Users>> DetectAndSaveFace(string imagePath)
        {
            var usersFound = new List<Users>();
            double threshold = 0.6;

            var users = await dbContext.Users.ToListAsync();

            using (var fd = Dlib.GetFrontalFaceDetector())
            using (var sp = ShapePredictor.Deserialize("dlib/shape_predictor_68_face_landmarks.dat"))
            using (var net = DlibDotNet.Dnn.LossMetric.Deserialize("dlib/dlib_face_recognition_resnet_model_v1.dat"))
            {
                using (var img = Dlib.LoadImage<RgbPixel>(imagePath))
                {
                    var dets = fd.Operator(img);

                    foreach (var rect in dets)
                    {
                        var shape = sp.Detect(img, rect);
                        var faceChip = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                        var face = Dlib.ExtractImageChip<RgbPixel>(img, faceChip);
                        var matrix = new Matrix<RgbPixel>(face);
                        var faceDescriptor = net.Operator<RgbPixel>(matrix);

                        Users closestUser = null;
                        double minDistance = double.MaxValue;

                        var faceDescriptorArray = faceDescriptor.ToArray();
                        var faceDescriptorStr = string.Join(",", faceDescriptorArray.Select(p => p.ToString()));
                        var faceDescrip = faceDescriptorStr
                        .Split(',')
                        .Select(s =>
                        {
                            bool success = float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
                            if (success)
                                return result;
                            return 0;
                        })
                        .ToArray();
                        var faceDescriptorRestored1 = new DlibDotNet.Matrix<float>(1, faceDescrip.Length);

                        for (int i = 0; i < faceDescrip.Length; i++)
                        {
                            faceDescriptorRestored1[i] = faceDescrip[i];
                        }

                        foreach (var existingUser in users)
                        {
                            if (existingUser.FaceDescriptor != null)
                            {
                                var faceDescriptorArrayRestored = existingUser.FaceDescriptor.Split(',').Select(float.Parse).ToArray();
                                var faceDescriptorRestored = new DlibDotNet.Matrix<float>(1, faceDescriptorArrayRestored.Length);

                                for (int i = 0; i < faceDescriptorArrayRestored.Length; i++)
                                {
                                    faceDescriptorRestored[i] = faceDescriptorArrayRestored[i];
                                }

                                var distance = Dlib.Length(faceDescriptorRestored1 - faceDescriptorRestored);
                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                    closestUser = existingUser;
                                }
                            }
                        }

                        
                        if (minDistance > threshold)
                        {
                            var newUser = new Users
                            {
                                FaceDescriptor = faceDescriptorStr,
                                id_role = 3
                            };

                            dbContext.Users.Add(newUser);
                            await dbContext.SaveChangesAsync();

                            usersFound.Add(newUser);
                        }
                        else
                        {
                            usersFound.Add(closestUser);
                        }
                    }
                }
            }
            return usersFound;
        }


        [NonAction]
        private double CalculateDistance(Matrix<float> descriptor1, Matrix<float> descriptor2)
        {
            double sqsum = 0.0;
            for (int i = 0; i < descriptor1.Size; i++)
            {
                var diff = descriptor1[i] - descriptor2[i];
                sqsum += diff * diff;
            }
            return Math.Sqrt(sqsum);
        }
    }
}
;