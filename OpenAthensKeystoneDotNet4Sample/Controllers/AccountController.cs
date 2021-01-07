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
        // GET: Login
        public void Login(string returnUrl = "/")
        {
            if (!base.Request.IsAuthenticated)
            {
                base.HttpContext.GetOwinContext().Authentication.Challenge(Array.Empty<string>());
            }
            else
            {
                string target = "";
                if (base.Request.QueryString["target"] != null)
                {
                    target = base.Request.QueryString["target"].ToString();
                }
                string str2 = "";
                foreach (Claim claim in ClaimsPrincipal.Current.Claims)
                {
                    if (claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                    {
                        str2 = claim.Value;
                    }
                    if (str2.IndexOf("openathens") > 0)
                    {
                        if (claim.Type == "derivedEduPersonScope")
                        {
                            string str3 = ConfigurationManager.AppSettings["anatomytvurl"];
                            string paramvalue = claim.Value;
                            this.redirectPage("scope", paramvalue, "", "", target);
                        }
                    }
                    else if (claim.Type == "realmName")
                    {
                        string str5 = ConfigurationManager.AppSettings["anatomytvurl"];
                        string paramvalue = claim.Value;
                        if (claim.Type == "EduPersonEntitlement")
                        {
                            string str7 = claim.Value;
                            if (!string.IsNullOrEmpty(str7))
                            {
                                this.redirectPage("entityID", paramvalue, "EduPersonEntitlement", str7, "");
                            }
                        }
                        this.redirectPage("entityID", paramvalue, "", "", "");
                    }
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
            string[] textArray1 = new string[] { str, "login?", paramname, "=", paramvalue, "&", paramname2, "=", paramvalue2 };
            base.Response.Write(string.Concat(textArray1));
            if (base.Request.Cookies["copyweblinkparams"] != null)
            {
                if (base.Request.Cookies["copyweblinkparams"].Value.Length > 0)
                {
                    string str3 = base.Request.Cookies["copyweblinkparams"].Value;
                    base.Response.Cookies["copyweblinkparams"].Value = "";
                    str3 = str3.Substring(1);
                    if (paramname2 != "")
                    {
                        string[] textArray2 = new string[] { str, "login?", paramname, "=", paramvalue, "&", paramname2, "=", paramvalue2, "&", str3, "&lastpageurl=", target, "&returnUrl=", str2 };
                        base.Response.Redirect(string.Concat(textArray2));
                    }
                    else
                    {
                        string[] textArray3 = new string[] { str, "login?", paramname, "=", paramvalue, "&", str3, "&lastpageurl=", target, "&returnUrl=", str2 };
                        base.Response.Redirect(string.Concat(textArray3));
                    }
                }
            }
            else if (paramname2 != "")
            {
                string[] textArray4 = new string[] { str, "login?", paramname, "=", paramvalue, "&", paramname2, "=", paramvalue2, "&lastpageurl=", target, "&returnUrl=", str2 };
                base.Response.Redirect(string.Concat(textArray4));
            }
            else
            {
                string[] textArray5 = new string[] { str, "login?", paramname, "=", paramvalue, "&lastpageurl=", target, "&returnUrl=", str2 };
                base.Response.Redirect(string.Concat(textArray5));
            }
        }




        [Authorize]
        public ActionResult Claims()
        {
            return View();
        }
    }
}