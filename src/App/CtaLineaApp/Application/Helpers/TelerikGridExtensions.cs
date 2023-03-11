using Telerik.Blazor.Components;

namespace CtaLineaApp.Application.Helpers
{
    public static class TelerikGridExtensions
    {
        public static async Task ClearFilterAsync<TEntity> (
            this TelerikGrid<TEntity> grid)
        {
            GridState<TEntity> state = new GridState<TEntity>()
            {
                SearchFilter = null,
                FilterDescriptors = null
            };

            await grid.SetStateAsync(state);
        }
    }
}
