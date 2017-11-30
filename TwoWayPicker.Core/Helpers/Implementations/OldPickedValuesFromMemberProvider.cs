using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoWayPicker.Core.Helpers.Interfaces;
using Umbraco.Core.Models;
using TwoWayPicker.Core.Extensions;
using Umbraco.Web.Security;

namespace TwoWayPicker.Core.Helpers.Implementations
{
    public class OldPickedValuesFromMemberProvider : IOldPickedValuesProvider
    {
        private IMember _member;
        public OldPickedValuesFromMemberProvider(IMember member)
        {
            _member = member;
        }

        public IEnumerable<int> GetOldPickedItems(string thisPickerPropertyAlias, IContentBase newContent)
        {
            return MemberPreviousValueRepo.Instance.GetAndRemoveValue(newContent.Id.ToString(), thisPickerPropertyAlias).ToIntList();
        }
    }
}
