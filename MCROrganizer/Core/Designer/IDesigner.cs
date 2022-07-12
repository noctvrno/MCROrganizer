namespace MCROrganizer.Core.Designer
{
    public interface IDesigner
    {
        // What to design.
        public ViewModel.ControlLogic ControlLogic { get; }

        // How to design.
        public CustomControls.RunState RunState { get; }

        // Design.
        public void Design();
    }
}
