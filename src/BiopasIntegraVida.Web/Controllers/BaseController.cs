﻿using Microsoft.AspNetCore.Mvc;

namespace BiopasIntegraVida.Web.Controllers
{
    public class BaseController : Controller
    {
        public IActionResult CustomErrorPage()
        {
            Response.StatusCode = 500;
            return View("Views/LandingPages/erro.cshtml");
        }
    }
}
