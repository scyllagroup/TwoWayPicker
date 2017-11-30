using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace TwoWayPicker.Core.Helpers.Interfaces
{
    public interface IOldPickedValuesProvider
    {
        IEnumerable<int> GetOldPickedItems(string thisPickerPropertyAlias, IContentBase newContent);
    }
}
