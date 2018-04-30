﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Assignment2.Models;
using Assignment2.ViewModels;


namespace Assignment2.Controllers
{
    public class AccountController : Controller
    {
        VisitorLogContext db = new VisitorLogContext();

        // GET: Account
        public ActionResult Index()
        {
            return View(db.Activities);
        }

        [HttpGet] //prepare data for the view
        public ActionResult Login()
        {
            int userIDnum = db.Users.Max(x => x.UserID); 

            for(int y = 0; y < userIDnum; y++)
            {
                var log = db.Users.SingleOrDefault(x => x.LoggedIn == true && x.UserID == y);

                if(log != null)
                {
                    log.LoggedIn = false;
                    db.SaveChanges();
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel user)
        {
            
            Activity Act = new Activity();

            User savedUser = db.Users.SingleOrDefault(m => m.Email == user.UserName && m.Password == user.Password);

            if (savedUser != null) //to check validity of the input
            {
                Act.ActivityName = user.UserName; //store the logged name
                Act.ActivityDate = DateTime.Now; //store current date and time
                Act.IpAddress = Request.UserHostAddress;// store user's IP adress


                db.Activities.Add(Act); //adding new user with name
              
                savedUser.LoggedIn = true;
                db.SaveChanges(); //saving new info in the database

                Session["TempUser"] = savedUser;

                return View("Index", db.Activities); //returning view and database
            }
            else
            {
                ModelState.Clear(); //to delete the input
                ModelState.AddModelError("Error", "Sorry. " +
                    "Check the database for login and password"); //display the error
                return View("Login");
             
            }

        }


        private User SessionUser() //storing temp user in session
        {
            if (Session["tempUser"] == null)
            {
                Session["tempUser"] = new User();
            }
            return (User)Session["tempUser"];
        }

        private void DeleteTempUser()
        {
            Session.Remove("tempUser");
        }
    }
}