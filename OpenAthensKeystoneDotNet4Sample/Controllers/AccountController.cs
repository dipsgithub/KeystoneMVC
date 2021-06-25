using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace OpenAthensKeystoneDotNet4Sample.Controllers
{   
    public class AccountController : Controller
    {
        private string email;
        private string firstname;
        private string lastname;
        private string logintype;
        private string paramvalue;
        private string EduPersonEntitlement;
        // GET: Login
        public void Login(string returnUrl = "/")
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(Array.Empty<string>());
            }
            else
            {
                string target = "";
                if (Request.QueryString["target"] != null)
                {
                    target = Request.QueryString["target"].ToString();
                }
                string str2 = "";
                foreach (Claim claim in ClaimsPrincipal.Current.Claims)
                {
                    if (claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                    {
                        str2 = claim.Value;
                    }
                    if (claim.Type == "email")
                    {
                        email = claim.Value;
                    }
                    if (claim.Type == "name")
                    {
                        firstname = claim.Value;
                    }
                    if (claim.Type == "lastname")
                    {
                        lastname = claim.Value;
                    }
                    if (str2.IndexOf("openathens") > 0)
                    {
                        if (claim.Type == "derivedEduPersonScope")
                        {
                            logintype = "athense";
                            string str3 = ConfigurationManager.AppSettings["anatomytvurl"];
                            paramvalue = claim.Value;
                        }
                    }
                    else if (claim.Type == "realmName")
                    {
                        string str5 = ConfigurationManager.AppSettings["anatomytvurl"];
                        paramvalue = claim.Value;
                        if (claim.Type == "EduPersonEntitlement")
                        {
                            logintype = "shibboleth";
                            EduPersonEntitlement = claim.Value;
                        }
                    }                    
                }
                if (logintype == "athense")
                {
                    this.redirectPage("scope", paramvalue, "", "", target);
                }
                else
                {
                    if (!string.IsNullOrEmpty(EduPersonEntitlement))
                    {
                        this.redirectPage("entityID", paramvalue, "EduPersonEntitlement", EduPersonEntitlement, "");
                    }
                    else
                        this.redirectPage("entityID", paramvalue, "", "", "");
                }
            }
        }

        public void LogOff()
        {
            if (Request.IsAuthenticated)
            {
                var authTypes = HttpContext.GetOwinContext().Authentication.GetAuthenticationTypes();
                HttpContext.GetOwinContext().Authentication.SignOut(authTypes.Select(t => t.AuthenticationType).ToArray());
            }

            Response.Redirect("/");
        }
        protected void redirectPage(string paramname, string paramvalue, string paramname2, string paramvalue2, string target)
        {
            string str = ConfigurationManager.AppSettings["anatomytvurl"];
            string str2 = ConfigurationManager.AppSettings["returnurlforlogout"];
            string textArray1 = str+ "login?"+ paramname+ "="+ paramvalue+ "&"+ paramname2+ "="+ paramvalue2;
            Response.Write(string.Concat(textArray1));
            string personalinfo = "&email="+email+"&firstname="+firstname+"&lastname="+lastname;
            if (Request.Cookies["copyweblinkparams"] != null)
            {
                if (Request.Cookies["copyweblinkparams"].Value.Length > 0)
                {
                    string str3 = Request.Cookies["copyweblinkparams"].Value;
                    Response.Cookies["copyweblinkparams"].Value = "";
                    str3 = str3.Substring(1);
                    if (paramname2 != "")
                    {
                        string textArray2 = textArray1+ "&"+ str3+ "&lastpageurl="+ target+ "&returnUrl="+ str2;
                        Response.Redirect(string.Concat(textArray2, personalinfo));
                    }
                    else
                    {
                        string textArray3 = str+ "login?"+ paramname+ "="+ paramvalue+ "&"+ str3+ "&lastpageurl="+ target+ "&returnUrl="+ str2;
                        Response.Redirect(string.Concat(textArray3, personalinfo));
                    }
                }
            }
            else if (paramname2 != "")
            {
                string textArray4 = str+ "login?"+ paramname+ "="+paramvalue+ "&"+ paramname2+ "="+ paramvalue2+ "&lastpageurl="+ target+ "&returnUrl="+ str2;
                Response.Redirect(string.Concat(textArray4, personalinfo));
            }
            else
            {
                string textArray5 = str+ "login?"+ paramname+ "="+ paramvalue+ "&lastpageurl="+ target+ "&returnUrl="+ str2;
                Response.Redirect(string.Concat(textArray5, personalinfo));
            }
        }




        [Authorize]
        public ActionResult Claims()
        {
            return View();
        }
    }
}