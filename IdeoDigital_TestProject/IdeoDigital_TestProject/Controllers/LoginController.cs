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
namespace IdeoDigital_TestProject.Controllers
{
    public class LoginController : UmbracoApiController
    {
        [HttpPost]
        public MemberViewModel Login(CredentialsPostModel model)
        {
            if(Members.Login(model.Email, model.Password))
            {
               var memberService = Services.MemberService;
               var member = memberService.GetByEmail(model.Email);
                var viewModel = new MemberViewModel
                {
                    FirstName = (string)member.GetValue("firstName"),
                    LastName = (string)member.GetValue("lastName"),
                    Email = member.Email,
                    Phone = member.GetValue("phone")
                };
                var iconStr= member.GetValue("icon").ToString();
                var icon = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<IconModel>>(iconStr);
                var mediaService = Services.MediaService;
                var i = mediaService.GetById(new Guid(icon.FirstOrDefault().MediaKey));
                return viewModel;
            }
            return null;
        }
    }
}