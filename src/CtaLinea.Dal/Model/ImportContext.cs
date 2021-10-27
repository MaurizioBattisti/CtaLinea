using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ZzSoft.CtaLinea.Dal.Model
{
    public class ImportContext
        : IDisposable
    {
        public ImportContext (
            Guid id,
            IDbConnection connection)
        {
            this.Id = id;
            this.Connection = connection;
        }

        public Guid Id { get; private set; }
        public IDbConnection Connection { get; private set; }

        public string Note { get; set; }
        public int ImportedElements { get; set; }
        public int ProcessedElements { get; set; }

        public void Dispose()
        {
            if (this.Connection != null)
            {
                this.Connection.Dispose();
            }
        }
    }
}
