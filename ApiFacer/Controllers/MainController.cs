using ApiFacer.Classes;
using ApiFacer.DB;
using ApiFacer.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFacer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MainController : Controller
    {
        private readonly ApiDB dbContext;

        public MainController(ApiDB dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> login(Login user)
        {
            var login = dbContext.Users.Where(a => a.login.Equals(user.login) && a.password.Equals(user.password)).FirstOrDefault();

            if (login != null)
            {
                return Ok(new { message = "Вход выполнен", status = "ok" });
            }
            return NotFound(new { message = "Нет такого пользователя!", status = "err" });
        }

        [HttpPost]
        [Route("create_event")]
        public async Task<ActionResult> create_event(CreateEvent e)
        {
            Events ev = new Events()
            {
                Name = e.Name,
            };

            await dbContext.Events.AddAsync(ev);
            await dbContext.SaveChangesAsync();

            return Ok(new { message = "", status = "ok" });
        }

        [HttpGet]
        [Route("get_events")]
        public async Task<ActionResult> get_events()
        {
            var ev = await dbContext.Events.ToListAsync();

            return Ok(new { message = "", status = "ok", events = ev });
        }
    }
}
;