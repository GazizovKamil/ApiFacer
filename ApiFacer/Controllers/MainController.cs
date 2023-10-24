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
using System.Collections.Immutable;

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

        //curl -X POST http://192.168.137.177:5001/api/Main/add_image_to_event/1 ^
        //-F "files=@C:\Users\Камиль\Desktop\xvAorCW2D_E.jpg;type=image/jpeg" ^
        //-F "files=@C:\Users\Камиль\Desktop\OjZesovKQPI.jpg;type=image/png" ^
        //-F "sessionkey=fc33fab9-9f36-468b-aca7-e536ed8554c3"


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
                    await dbContext.Images.AddAsync(image);
                    await dbContext.SaveChangesAsync();

                    List<People> users = await DetectAndSaveFace(Path.Combine(folderPath, file.FileName));

                    if (users.Count > 0)
                    {
                        foreach (People u in users)
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


        //curl -X POST http://192.168.137.177:5001/api/Main/add_image_to_event/1 ^
        //-F "files=@C:\Users\Камиль\Desktop\xvAorCW2D_E.jpg;type=image/jpeg" ^
        //-F "files=@C:\Users\Камиль\Desktop\OjZesovKQPI.jpg;type=image/png" ^
        //-F "sessionkey=489e6243-47d4-417d-ad20-98a260655e10"

        [NonAction]
        public async Task<List<People>> DetectAndSaveFace(string imagepath)
        {
            //Console.WriteLine(imagePath);
            var usersFound = new List<People>();
            double threshold = 0.6;

            var users = await dbContext.People.ToListAsync();

            using (var fd = Dlib.GetFrontalFaceDetector())
            using (var sp = ShapePredictor.Deserialize("dlib/shape_predictor_68_face_landmarks.dat"))
            using (var net = DlibDotNet.Dnn.LossMetric.Deserialize("dlib/dlib_face_recognition_resnet_model_v1.dat"))
            {
                using (var img = Dlib.LoadImage<RgbPixel>(imagepath))
                {
                    var dets = fd.Operator(img);

                    foreach (var rect in dets)
                    {
                        var shape = sp.Detect(img, rect);
                        var faceChip = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                        var face = Dlib.ExtractImageChip<RgbPixel>(img, faceChip);
                        var matrix = new Matrix<RgbPixel>(face);
                        var faceDescriptor = net.Operator(matrix);
                        
                        People closestUser = null;
                        double minDistance = double.MaxValue;

                        var faceDescriptorArray = faceDescriptor.ToArray();

                        foreach (var existingUser in users)
                        {
                            var peop = Mongo.GetDescript(existingUser.Id);
                            if (peop != null && peop.descriptor != null)
                            {
                                float[] faceEmbedding = peop.descriptor;
                                Matrix<float> embeddingMatrix = new Matrix<float>(1, faceEmbedding.Length);

                                // Populate the matrix with the values from the face embedding array
                                for (int i = 0; i < faceEmbedding.Length; i++)
                                {
                                    embeddingMatrix[0, i] = faceEmbedding[i];
                                }

                                var distance = Dlib.Length(faceDescriptor.SingleOrDefault() - embeddingMatrix);
                                if (distance < minDistance)
                                {
                                    Console.WriteLine(distance);
                                    minDistance = distance;
                                    closestUser = existingUser;
                                }

                            }
                        }


                        if (minDistance > threshold)
                        {
                            var newUser = new People();

                            dbContext.People.Add(newUser);
                            await dbContext.SaveChangesAsync();

                            PeopleDescriptor people = new PeopleDescriptor()
                            {
                                descriptor = faceDescriptor.SingleOrDefault().ToArray(),
                                people_id = newUser.Id
                            };

                            Mongo.AddToDB(people);

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
    }
}
