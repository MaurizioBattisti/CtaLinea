using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using ZzSoft.CtaLinea.Dal.Context;

namespace CtaLineaWebApi.Utility
{
    public class ZzRequestConstx
        : IZzRequestConstx
    {
        private readonly IHttpContextAccessor _httContext;
        private readonly ILogger _logger;

        public ZzRequestConstx(
            IHttpContextAccessor httpContext,
            ILogger<ZzRequestConstx> logger)
        {
            _httContext = httpContext;
            _logger = logger;
        }

        private DateTime? _PeriodStartDate;
        public DateTime? PeriodStartDate
        {
            get
            {
                if (_read == false) this.GetData();
                return _PeriodStartDate;
            }
        }
        private DateTime? _PeriodEndDate;
        public DateTime? PeriodEndDate
        {
            get
            {
                if (_read == false) this.GetData();
                return _PeriodEndDate;
            }
        }
        private int? _ContractId;
        public int? ContractId
        {
            get
            {
                if (_read == false) this.GetData();
                return _ContractId;
            }
        }
        private string? _UserName;
        public string? UserName
        {
            get
            {
                if (_read == false) this.GetData();
                return _UserName;
            }
        }
        public void Override(
            int? contractId,
            DateTime? startDate,
            DateTime? endDate)
        {
			if (_read == false) this.GetData();
            _ContractId = contractId;
            _PeriodStartDate = startDate;
            _PeriodEndDate = endDate;
		}

		private bool _read = false;
        private void GetData()
        {

            try
            {
                this._UserName = this._httContext.HttpContext.User?.Identity.Name;

                if (this._httContext.HttpContext.Request.Headers.ContainsKey(Constants.RequestHeader_PeriodStartDate))
                {
                    var str = this._httContext.HttpContext.Request.Headers[Constants.RequestHeader_PeriodStartDate].FirstOrDefault();
                    if (string.IsNullOrEmpty(str) == false)
                    {
                        _PeriodStartDate = DateTime.Parse(str);
                    }
                }
                if (this._httContext.HttpContext.Request.Headers.ContainsKey(Constants.RequestHeader_PeriodEndDate))
                {
                    var str = this._httContext.HttpContext.Request.Headers[Constants.RequestHeader_PeriodEndDate].FirstOrDefault();
                    if (string.IsNullOrEmpty(str) == false)
                    {
                        _PeriodEndDate = DateTime.Parse(str);
                    }
                }
                if (this._httContext.HttpContext.Request.Headers.ContainsKey(Constants.RequestHeader_ContractId))
                {
                    var str = this._httContext.HttpContext.Request.Headers[Constants.RequestHeader_ContractId].FirstOrDefault();
                    if (string.IsNullOrEmpty(str) == false)
                    {
                        _ContractId = int.Parse(str);
                    }
                }
            }
            catch (Exception ex)
            {
                // non fa nulla ma non prende i valori
                _logger.LogWarning("Headernon recuperabili :" + ex.Message);
            }

            _read = true;
        }
    }
}
