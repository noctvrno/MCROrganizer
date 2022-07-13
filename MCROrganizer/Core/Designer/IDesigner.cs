using MCROrganizer.Core.CustomControls;

namespace MCROrganizer.Core.Designer
{
    public interface IDesigner
    {
        // What to design.
        public DraggableButtonDataContext RunData { get; }

        // Design.
        public void Design(CustomizableRunElements elementToDesign);
    }
}
