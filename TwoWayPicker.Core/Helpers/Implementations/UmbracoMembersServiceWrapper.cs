using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoWayPicker.Core.Helpers.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using Umbraco.Web.Security;

namespace TwoWayPicker.Core.Helpers.Implementations
{
    public class UmbracoMembersServiceWrapper : IUmbracoServiceWrapper
    {
        private IMemberService _memberService;


        public UmbracoMembersServiceWrapper(IMemberService memberService)
        {
            _memberService = memberService;

        }

        public IContentBase GetById(int id)
        {
            return _memberService.GetById(id);
        }

        public void Save(IContentBase item, bool publishIfPublishable, int userId = 0, bool raiseEvents = true)
        {
            try
            {
                this._memberService.Save((IMember)item, raiseEvents);
            }
            catch(Exception e)
            {

            }
        }
    }
}
