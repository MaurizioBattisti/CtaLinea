namespace CtaLineaApp.Components.Utility
{
	public class BeforeActOnItemArgs<T> where T : class
	{
		public BeforeActOnItemArgs(T item)
		{
			this.Item = item;
		}
		public bool Cancelled { get; set; } = false;
		public T Item { get; private set; }
	}
}
