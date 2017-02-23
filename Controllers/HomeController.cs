using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LandsDepartment.Models;
using System.Web.Security;

namespace LandsDepartment.Controllers
{
    public class HomeController : Controller
    {
        private LandsDBEntities db = new LandsDBEntities();

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult News()
        {
            List<News> objnews = db.News.ToList();
            return View(objnews);
        }

        public ActionResult Projects()
        {
            List<ProjectInformation> objProjects = db.ProjectInformations.Where(p => p.isDeleted ==false || p.isDeleted == null).ToList();
            return View(objProjects);
        }
        

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult SaveContact(ContactUsViewModel model)
        {
            EmailSender Email = new EmailSender();


            try
            {

                string body = "Dear Concerned,<br>Below are the details and message from the Visitor from Lands department Website<br><br>";
                body += "<h5> Name : </h5>" + model.Name;
                body += "<br><h5> Email : </h5>" + model.Email;
                body += "<br><h5> Phone : </h5>" + model.Phone;
                body += "<br><h5> Subject : </h5>" + model.Subject;
                body += "<br><h5> Message : </h5>" + model.Message;

                body += "<br><br>Regards,";
                body += "<br>Lands Department Website";

                Email.SendEmail(System.Configuration.ConfigurationManager.AppSettings["EmailReciever"].ToString(), "Contact Us Request From Lands Department Website", body);
            }
            catch (Exception)
            {
                JsonResult objres1 = new JsonResult();
                objres1.Data = "Error";
                return Json(objres1);
            }
            
            JsonResult objres = new JsonResult();
            objres.Data = "Success";
            return Json(objres);
        }

        public ActionResult Forms()
        {
            return View();
        }

        public ActionResult ManageUsers()
        {
            List<aspnet_Membership> objUsers = db.aspnet_Membership.ToList();
            return View(objUsers);
        }


        public ActionResult EditProfile()
        {
             aspnet_Membership objUsers = db.aspnet_Membership.SingleOrDefault(m => m.Email == User.Identity.Name);
            return PartialView("_EditProfile",objUsers);
        }

        public ActionResult SaveProfile(aspnet_Membership model)
        {
            aspnet_Membership objUsers = db.aspnet_Membership.SingleOrDefault(m => m.UserId == model.UserId);
            if (objUsers != null)
            {
                objUsers.Name = model.Name;
                objUsers.Phone = model.Phone;
                //objUsers.Phone = model.Phone;

                db.ObjectStateManager.ChangeObjectState(objUsers, System.Data.EntityState.Modified);
                db.SaveChanges();

                return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = "Error" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword()
        {
            return PartialView("_ChangePassword");
        }

        [HttpPost]
        public JsonResult PassChange(LocalPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipUser objUser = Membership.GetUser(User.Identity.Name);
                objUser.ChangePassword(model.OldPassword, model.NewPassword);

                
                return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);

                //return RedirectToAction("PassChangeSucessful", "Account");
            }
            return Json(new { Message = "Error" }, JsonRequestBehavior.AllowGet);
          //  return View(model);
        }

        public ActionResult ChangeUserPassword(string userid)
        {
            LocalPasswordModel objnew = new LocalPasswordModel();
            objnew.UserName = userid;
            return PartialView("_ChangeUserPassword",objnew);
        }


        public ActionResult UpdateUserPassword(LocalPasswordModel model)
        {
            //if (ModelState.IsValid)
            //{
            if (!string.IsNullOrEmpty(model.UserName) && (model.NewPassword == model.ConfirmPassword))
            {
                MembershipUser objUser = Membership.GetUser(model.UserName);

                objUser.ChangePassword(objUser.GetPassword(), model.NewPassword);


                return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
            }
                //return RedirectToAction("PassChangeSucessful", "Account");
            //}
            return Json(new { Message = "Error" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UsersnRoles()
        {
            List<aspnet_Membership> objUsers = db.aspnet_Membership.ToList();
            return PartialView("_ManageUsers", objUsers);
        }


        #region User Module

        [Authorize(Roles = "Admin")]
        public ActionResult UsersList()
        {
            List<aspnet_Membership> membership = db.aspnet_Membership.ToList();
            return View(membership);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult CreateNormalUser(Guid id)
        {
            aspnet_Users objuser = db.aspnet_Users.SingleOrDefault(au => au.UserId == id);

            Roles.RemoveUserFromRole(objuser.UserName, "Admin");
            Roles.AddUserToRole(objuser.UserName, "User");

            return RedirectToAction("UsersList");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult CreateAdmin(Guid id)
        {
            aspnet_Users objuser = db.aspnet_Users.SingleOrDefault(au => au.UserId == id);

            Roles.RemoveUserFromRole(objuser.UserName, "User");
            Roles.AddUserToRole(objuser.UserName, "Admin");

            return RedirectToAction("ManageUsers");
        }

       // [Authorize(Roles = "Admin")]
        public ActionResult UnblockUser(Guid id)
        {
            aspnet_Membership objuser = db.aspnet_Membership.SingleOrDefault(au => au.UserId == id);
            objuser.IsLockedOut = false;
            db.ObjectStateManager.ChangeObjectState(objuser, System.Data.EntityState.Modified);
            db.SaveChanges();

            return RedirectToAction("ManageUsers");
        }

        //[Authorize(Roles = "Admin")]
        public ActionResult BlockUser(Guid id)
        {
            aspnet_Membership objuser = db.aspnet_Membership.SingleOrDefault(au => au.UserId == id);
            objuser.IsLockedOut = true;
            db.ObjectStateManager.ChangeObjectState(objuser, System.Data.EntityState.Modified);
            db.SaveChanges();

            return RedirectToAction("ManageUsers");
        }


        public ActionResult CreateNewUser()
        {
            return PartialView("_CreateNewUser");
        }

        [HttpPost]
        public ActionResult SaveNewUser(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    MembershipCreateStatus createStatus;
                    Membership.CreateUser(model.registermodel.UserName, model.registermodel.Password, model.registermodel.UserName, null, null, true, null, out createStatus);
                    aspnet_Roles SelectedRole = db.aspnet_Roles.Single(r => r.RoleName == "User");
                    if (createStatus == MembershipCreateStatus.Success)
                    {

                        Roles.AddUserToRole(model.registermodel.UserName, SelectedRole.RoleName);

                        aspnet_Membership objUsers = db.aspnet_Membership.SingleOrDefault(m => m.Email == model.registermodel.UserName);
                        if (objUsers != null)
                        {
                            objUsers.Name = model.registermodel.Name;
                            objUsers.Phone = model.registermodel.Phone;

                            db.ObjectStateManager.ChangeObjectState(objUsers, System.Data.EntityState.Modified);
                            db.SaveChanges();

                        }

                    }
                    return PartialView("_CreateNewUser",model);
                    //return View(model);
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
           // return View(model);
            return PartialView("_CreateNewUser");
        }

        public ActionResult ManageMaps()
        {
            List<Map> objmaps = db.Maps.ToList();
            return PartialView("_ManageMaps", objmaps);
        }

        public ActionResult CreateMap(int ID=0)
        {
            Map obj = null;

            if (ID != 0)
                obj = db.Maps.SingleOrDefault(m => m.ID == ID);

            return PartialView("_CreateMaps", obj);
        }


        public ActionResult SaveMaps(Map model)
        {
            
                if (model.ID == 0)
                {
                    db.Maps.AddObject(model);
                    db.SaveChanges();

                    List<Map> objmaps = db.Maps.ToList();
                    return PartialView("_ManageMaps", objmaps);
                    //return Json(new { Message = "Success", Model = objmaps }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                  Map  obj = db.Maps.SingleOrDefault(m => m.ID == model.ID);
                  if (obj != null)
                  {
                      obj.GoogleID = model.GoogleID;
                      obj.LANG = model.LANG;
                      obj.LAT = model.LAT;
                      obj.TITLE = model.TITLE;
                      obj.URL = model.URL;

                      db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                      db.SaveChanges();

                      List<Map> objmaps = db.Maps.ToList();
                      return PartialView("_ManageMaps", objmaps);
                      //return Json(new { Message = "Success", Model = objmaps }, JsonRequestBehavior.AllowGet);
            
            
                  }
                }
                return Json(new { Message = "Error" }, JsonRequestBehavior.AllowGet);
            //return PartialView("_CreateMaps", model);
        }

        public ActionResult DeleteMap(int ID = 0)
        {
            if (ID != 0)
            {
                Map obj = db.Maps.SingleOrDefault(m => m.ID == ID);

                db.DeleteObject(obj);
                db.SaveChanges();
            }

            List<Map> objmaps = db.Maps.ToList();
            return PartialView("_ManageMaps", objmaps);
        }



        #endregion User Module



        public ActionResult ManageNews()
        {
            List<News> objnews = db.News.ToList();
            return PartialView("_ManageNews", objnews);
        }

        public ActionResult CreateNews(int ID=0)
        {
            News obj = null;

            if (ID != 0)
                obj = db.News.SingleOrDefault(m => m.NewsID == ID);

            return PartialView("_CreateNews", obj);
        }

        [HttpPost]
        public ActionResult SaveNews(News model, HttpPostedFileBase imgNews)
        {

            if (model.NewsID == 0)
            {
                model.CreatedDate = System.DateTime.Now;
                model.ModifiedDate = System.DateTime.Now;
                db.News.AddObject(model);
                db.SaveChanges();

                if (imgNews != null)
                {
                    string[] fileExt = imgNews.FileName.Split('.');
                    string fileName = Server.MapPath("~\\Content\\NewsPics\\") + model.NewsID + "." + fileExt[1];

                    imgNews.SaveAs(fileName);

                    model.Image = model.NewsID + "." + fileExt[1];
                    db.ObjectStateManager.ChangeObjectState(model, System.Data.EntityState.Modified);
                    db.SaveChanges();
                }
                return RedirectToAction("ManageUsers");
                //List<News> objmaps = db.News.ToList();
                //return PartialView("_ManageNews", objmaps);
            }
            else
            {
                News obj = db.News.SingleOrDefault(m => m.NewsID == model.NewsID);
                if (obj != null)
                {
                    obj.Description = model.Description;
                    obj.Title = model.Title;
                    obj.ModifiedDate = System.DateTime.Now;
                    // Image updation

                   

                    if (imgNews != null)
                    {
                        string[] fileExt = imgNews.FileName.Split('.');
                        string fileName = Server.MapPath("~\\Content\\NewsPics\\") + model.NewsID + "." + fileExt[1];

                        imgNews.SaveAs(fileName);

                        obj.Image = model.NewsID + "." + fileExt[1];
                       
                    }
                    db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                    db.SaveChanges();


                    //List<News> objmaps = db.News.ToList();
                    //return PartialView("_ManageNews", objmaps);
                    return RedirectToAction("ManageUsers");

                }
            }
                return Json(new { Message = "Error" }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SaveNews1(NewsViewModel model1, HttpPostedFileBase imgNews)
        {
            News model = new News();

            if (model.NewsID == 0)
            {
                model.CreatedDate = System.DateTime.Now;
                model.ModifiedDate = System.DateTime.Now;
                db.News.AddObject(model);
                db.SaveChanges();

                //if (image != null)
                //{
                //    string[] fileExt = image.FileName.Split('.');
                //    string fileName = Server.MapPath("~\\Content\\NewsPics\\") + model.NewsID + "." + fileExt[1];

                //    image.SaveAs(fileName);
                //}

                List<News> objmaps = db.News.ToList();
                return PartialView("_ManageNews", objmaps);
            }
            else
            {
                News obj = db.News.SingleOrDefault(m => m.NewsID == model.NewsID);
                if (obj != null)
                {
                    obj.Description = model.Description;
                    obj.Title = model.Title;
                    obj.ModifiedDate = System.DateTime.Now;
                    // Image updation

                    db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                    db.SaveChanges();

                    //if (image != null)
                    //{
                    //    string[] fileExt = image.FileName.Split('.');
                    //    string fileName = Server.MapPath("~\\Content\\NewsPics\\") + model.NewsID + "." + fileExt[1];

                    //    image.SaveAs(fileName);
                    //}

                    List<News> objmaps = db.News.ToList();
                    return PartialView("_ManageNews", objmaps);
                    //  return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);


                }
            }
            return Json(new { Message = "Error" }, JsonRequestBehavior.AllowGet);
        }

        //

        public ActionResult DeleteNews(int ID = 0)
        {
            if (ID != 0)
            {
                News obj = db.News.SingleOrDefault(m => m.NewsID == ID);

                db.DeleteObject(obj);
                db.SaveChanges();
            }

            List<News> objnews = db.News.ToList();
            return PartialView("_ManageNews", objnews);
        }


        public ActionResult NewsDetails(int newsid = 0)
        {
            News obj = null;

            if (newsid != 0)
                obj = db.News.SingleOrDefault(m => m.NewsID == newsid);

            return View(obj);
        }



        public ActionResult ManageProjects()
        {
            List<ProjectInformation> objnews = db.ProjectInformations.Where(p => p.isDeleted == false || p.isDeleted ==null).ToList();
            return PartialView("_ManageProject", objnews);
        }

        public ActionResult CreateProject(int ID = 0)
        {
            ProjectInformation obj = null;

            if (ID != 0)
                obj = db.ProjectInformations.SingleOrDefault(m => m.ProjectID == ID);

            return PartialView("_CreateProject", obj);
        }


        public ActionResult SaveProject(ProjectInformation model)
        {

            if (model.ProjectID == 0)
            {
                model.CreatedDate = System.DateTime.Now;
                model.ModifiedDate = System.DateTime.Now;
                model.isDeleted = false;
                db.ProjectInformations.AddObject(model);
                db.SaveChanges();

                List<ProjectInformation> objnews = db.ProjectInformations.Where(p => p.isDeleted == false || p.isDeleted == null).ToList();
                return PartialView("_ManageProject", objnews);
            }
            else
            {
                ProjectInformation obj = db.ProjectInformations.SingleOrDefault(m => m.ProjectID == model.ProjectID);
                if (obj != null)
                {
                    obj.shortDescription = model.shortDescription;
                    obj.LongDesccription = model.LongDesccription;
                    obj.Title = model.Title;
                    obj.ModifiedDate = System.DateTime.Now;
                    
                    // Image updation

                    db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                    db.SaveChanges();

                    List<ProjectInformation> objnews = db.ProjectInformations.Where(p => p.isDeleted == false || p.isDeleted == null).ToList();
                    return PartialView("_ManageProject", objnews);
                  //  return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);


                }
            }
            return Json(new { Message = "Error" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteProject(int ID = 0)
        {
            if (ID != 0)
            {
                ProjectInformation obj = db.ProjectInformations.SingleOrDefault(m => m.ProjectID == ID);
                obj.isDeleted = true;

                db.ObjectStateManager.ChangeObjectState(obj, System.Data.EntityState.Modified);
                //db.DeleteObject(obj);
                db.SaveChanges();
            }

            List<ProjectInformation> objnews = db.ProjectInformations.Where(p => p.isDeleted == false  || p.isDeleted == null).ToList();
            return PartialView("_ManageProject", objnews);
        }




        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        
    }
}
