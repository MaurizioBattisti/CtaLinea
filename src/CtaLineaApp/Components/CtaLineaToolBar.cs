using CtaLineaApp.Application.Model;
using CtaLineaApp.Components.ToolBars;
using CtaLineaApp.Shared;
using Microsoft.AspNetCore.Components;

namespace CtaLineaApp
{
    public class CtaLineaToolBar
		: ComponentBase
    {
        private bool _isBusy = false;
        [Parameter]
        public bool IsBusy 
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                if (this.ListBar != null)
                {
                    this.ListBar.IsBusy = _isBusy;
                }
            }
        }


        private ListToolBar? _ListBar;
        [CascadingParameter]
        public ListToolBar? ListBar 
        {
            get { return _ListBar; }
            set
            {
                if (_ListBar != null)
                {
                    // sgancia gli event hangler
                }
                _ListBar = value;
                if (_ListBar != null)
                {
                    // riaggancia gli event handler

                    // assegna i valori
                    _ListBar.IsBusy = this.IsBusy;
                }
            }
        }
	}
}
