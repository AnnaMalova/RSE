﻿using RSE.Core.Helpers;
using RSE.Core.Interfaces;
using RSE.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSE.Core
{
    public class Repository : IRepository
    {
        Context context = new Context();

        public IEnumerable<Exercise> Exercises { get { return context.Exercises;  } }
        public IEnumerable<Variant> Variants { get { return context.Variants; } }
        public IEnumerable<Teacher> Teachers { get { return context.Teachers; } }

        private User _authorizedUser;

        public object PasswordHelpers { get; private set; }

        public User GetAuthorizedUser()
        {
            return _authorizedUser;
        }

        public void SaveUserInfo(string email, string name)
        {
            _authorizedUser.Email = email;
            _authorizedUser.Name = name;
            context.SaveChanges();
        }
        public List<int> WrongAnswers (int answer, int numbOfTask,  Variant variant)
        {
            List<int> WrongAnswers = new List<int>();
            int CountOfWrongAnswers;

            Exercise task = context.Exercises.SingleOrDefault(x => x.Variant == variant && x.Number == numbOfTask && x.Answer == answer);
            if ( task.Answer != answer)
            {
                WrongAnswers.Add(answer);
                CountOfWrongAnswers = WrongAnswers.Count();
            }
            return WrongAnswers;
           
        }


        public bool Authorize(string login, string password)
        {
            if (login == String.Empty)
            {
               
                return false;
            }

            string hashedPassword = PasswordHelper.GetHash(password);
            User user = context.Users.SingleOrDefault(x => x.Login == login && x.Password == hashedPassword);
            if (user != null)
            {
                _authorizedUser = user;
                return true;
            }

            return false;
        }

        public bool RegisterUser(User user, ref string errMessage)
        {
            if (!UserInfoHelper.CheckUser(user))
            {
                errMessage = "Empty login";
                return false;
            }
            User found = context.Users.SingleOrDefault(x => x.Login == user.Login);
            if (found != null)
            {
                errMessage = "User with same login already exists";
                return false;
            }

            context.Users.Add(user);
            context.SaveChanges();
            return true;
        }
    }
}

