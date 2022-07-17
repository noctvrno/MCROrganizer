using MCROrganizer.Core.CustomControls;
using Newtonsoft.Json;
using System.ComponentModel;

namespace MCROrganizer.Core.Designer
{
    public interface IDesigner : INotifyPropertyChanged
    {
        // What to design.
        [JsonIgnore]
        public DraggableButtonDataContext RunData { get; }

        public void Design(CustomizableRunElements elementToDesign);
    }
}
