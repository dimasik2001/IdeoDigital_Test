using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using IdeoDigital_TestProject.Models;
using LightInject;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using Newtonsoft.Json.Serialization;
using Umbraco.Web;
using Umbraco.Core.Models.PublishedContent;
using System.Net;

namespace IdeoDigital_TestProject.Controllers
{
    public class LoginController : UmbracoApiController
    {
        [HttpPost]
        [Obsolete]
        public object Login(CredentialsPostModel model)
        {
            if(!string.IsNullOrEmpty(model.Email) 
                && !string.IsNullOrEmpty(model.Password)
                && Members.Login(model.Email, model.Password))
            {
               var memberService = Services.MemberService;
               var member = memberService.GetByEmail(model.Email);
               
                var iconInfo= member.GetValue("icon").ToString();
                var icon = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<IconModel>>(iconInfo);
                var iconId = new Guid(icon.FirstOrDefault().MediaKey);         
                var iconUrl = Umbraco.Media(iconId).Url(mode: UrlMode.Absolute);
                var viewModel = new MemberViewModel
                {
                    FirstName = (string)member.GetValue("firstName"),
                    LastName = (string)member.GetValue("lastName"),
                    Email = member.Email,
                    Phone = member.GetValue("phone"),
                    IconUrl = iconUrl
                };
                return viewModel;
            }
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }
    }
}