﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Incognito.Data;
using Incognito.Models;
using Incognito.Models.MessageViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Incognito.Controllers
{
    public class PublicController : Controller
    {
        private UserContext _userContext;
        private MessageContext _messageContext;
        private UserManager<ApplicationUser> _userManager;

        public PublicController(
            UserContext userContext,
            MessageContext messageContext,
            UserManager<ApplicationUser> userManager)
        {
            _userContext = userContext;
            _messageContext = messageContext;
            _userManager = userManager;
        }

        [HttpGet]
        public ActionResult Index(string username)
        {
            var lookUpUser = username;
            var userFound = _userContext.Users
                .FirstOrDefault(u => u.UserName == lookUpUser);

            if (userFound != null)
            {
                var profile = _userContext.Profiles
                    .FirstOrDefault(p => p.UserId == userFound.Id);

                var userId = userFound.Id;
                var userName = $"{userFound.FirstName} {userFound.LastName}";
                ViewData["User"] = userName;
                ViewData["receiverId"] = userId;
                ViewData["Twitter"] = profile.Twitter;
                ViewData["Company"] = profile.CompanyName;
                ViewData["Profile"] = $"{userFound.FirstName[0]}{userFound.LastName[0]}";

                return View();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(MessageViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var senderId = _userManager.GetUserId(User);

                var message = new Message
                {
                    RecevierId =viewModel.ReceiverId,
                    Text = viewModel.Text,
                    SentTime = DateTime.UtcNow,
                };
                if (senderId == null)
                {
                    message.SenderId = "Not Provided";
                } else
                {
                    message.SenderId = senderId;
                }
                _messageContext.Add(message);
                await _messageContext.SaveChangesAsync();
                return RedirectToAction(nameof(Success));
            }
            return View(viewModel);
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
