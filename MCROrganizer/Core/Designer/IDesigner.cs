using MCROrganizer.Core.CustomControls;
using System.ComponentModel;

namespace MCROrganizer.Core.Designer
{
    public interface IDesigner : INotifyPropertyChanged
    {
        // What to design.
        public DraggableButtonDataContext RunData { get; }

        public void Design(CustomizableRunElements elementToDesign);
    }
}
