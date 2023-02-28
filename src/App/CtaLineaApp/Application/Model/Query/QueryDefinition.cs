namespace CtaLinea.Application.Model.Query
{
    public class QueryDefinition
    {
        public QueryDefinition ()
        {
            this.PageSize = -1;
            this.Page = 0;
            this.Skip = 0;
            this.Count = true;
        }

        public string? EndPoint { get; set; }

        public string? FullText { get; set; }
        
        public string? Filter { get; set; }
        public string? Sort { get; set; }

        private int _Page;
        public int Page 
        { 
            get
            {
                if (this.PageSize <= 0) return 0;
                if (this.PageSize == int.MaxValue) return 0;
                if (this._Page < 0) return 0;
                return this._Page;
            }
            set { this._Page = value; }
        }
        public int PageSize { get; set; }
        public int Skip { get; set; }
        private bool _Count;
        public bool Count 
        { 
            get 
            {
                if (this.PageSize <= 0) return false;
                if (this.PageSize == int.MaxValue) return false;
                return this._Count; 
            }
            set { this._Count = value; }
        }
    }
}
