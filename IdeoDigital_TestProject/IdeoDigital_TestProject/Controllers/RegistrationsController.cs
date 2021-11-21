using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;
using IdeoDigital_TestProject.Models;
using LightInject;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
namespace IdeoDigital_TestProject.Controllers 
{
    public class RegistrationsController : UmbracoApiController
    {

        [HttpPost]
        public void Register(RegisterPostModel model)
        {
            if(!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            var memberService = Services.MemberService;

            if (memberService.GetByEmail(model.Email) != null)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            var fullName = $"{model.FirstName} {model.LastName}";
            var member = memberService.CreateMember(model.Email, model.Email, fullName, "myMember");

            member.SetValue("firstName", model.FirstName);
            member.SetValue("lastName", model.LastName);
            member.SetValue("phone", model.Phone);

            memberService.Save(member); 
            memberService.SavePassword(member, model.Password);

            if(!string.IsNullOrEmpty(model.IconEncoded))
            {
                var bytes = Convert.FromBase64String(model.IconEncoded);
                CreateIconFromBytes(member, bytes);
            }
        }
        private void CreateIconFromBytes(IMember member, byte[] bytes)
        {   
            var mediaService = Services.MediaService;
            using (var stream = new MemoryStream(bytes))
            {
                var fileName = GenerateIconName(member);
                var media = mediaService.CreateMedia(fileName, Constants.System.Root, "Image");

                media.SetValue(Services.ContentTypeBaseServices, "umbracoFile", fileName, stream);
                mediaService.Save(media);

                var refs = new List<IconReferenceModel>();
                refs.Add(new IconReferenceModel
                {
                    Key = Guid.NewGuid().ToString(),
                    MediaKey = media.Key.ToString()
                }
                );

                member.SetValue("icon", Newtonsoft.Json.JsonConvert.SerializeObject(refs));
                Services.MemberService.Save(member);
            }
        }

        private string GenerateIconName(IMember member)
        {
            return $"{member.Id}_{member.Name}_icon.jpg";
        }
    }
}