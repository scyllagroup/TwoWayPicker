using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoWayPicker.Core.Helpers.Interfaces;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Security;

namespace TwoWayPicker.Core.Helpers.Implementations
{
    public class UmbracoContentServiceWrapper : IUmbracoServiceWrapper
    {
        private IContentService _contentService;
        public UmbracoContentServiceWrapper(IContentService contentService)
        {
            _contentService = contentService;
        }

        public IContentBase GetById(int id)
        {
            return _contentService.GetById(id);
        }

        public void Save(IContentBase item, bool publishIfPublishable, int userId = 0, bool raiseEvents = true)
        {
            try
            {
                IContent content = (IContent)item;
                if(publishIfPublishable && content.Published)
                {
                    _contentService.SaveAndPublishWithStatus((IContent)item, userId, raiseEvents);
                }
                else
                {
                    _contentService.Save((IContent)item, userId, raiseEvents);
                }
               
            }
            catch (Exception e) { }
        }
    }
}
