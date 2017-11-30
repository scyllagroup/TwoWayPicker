using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Publishing;

namespace TwoWayPicker.Core.Helpers.Interfaces
{
    public interface IUmbracoServiceWrapper
    {
        IContentBase GetById(int id);
        void Save(IContentBase item, bool publishIfPublishable, int userId = 0, bool raiseEvents = true);
    }
}
