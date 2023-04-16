namespace CtaLineaApp.Components
{
    public interface IEditableData
    {
        void SetDirty();
		bool IsClean { get; }
    }
}