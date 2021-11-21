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
            var isLogin = Members.Login(model.Email, model.Password);
            if (ModelState.IsValid
                && isLogin)
            {
               var memberService = Services.MemberService;
               var member = memberService.GetByEmail(model.Email);
                var iconUrl = string.Empty;
                
                try
                {
                    var iconInfo= member.GetValue("icon")?.ToString();
                    var iconReferenceModel = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<IconReferenceModel>>(iconInfo);
                    var iconId = new Guid(iconReferenceModel.FirstOrDefault().MediaKey);
                    iconUrl = Umbraco.Media(iconId)?.Url(mode: UrlMode.Absolute);
                }
                catch(ArgumentNullException)
                {
                }

                var viewModel = new MemberViewModel
                {
                    FirstName = (string)member.GetValue("firstName"),
                    LastName = (string)member.GetValue("lastName"),
                    Email = member.Email,
                    Phone = member.GetValue("phone"),
                    IconUrl = !string.IsNullOrEmpty(iconUrl) ? $"{iconUrl}?width=50&height=50" : string.Empty
                };
                return viewModel;
            }
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }
    }
}