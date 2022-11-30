using CtaLinea.Model.External;

namespace CtaLinea.Model.Runs
{
    public class RunPeriodCar
    {
        private const string CarTYpe_PRimary = "P";
        private const string CarTYpe_Spare_1 = "1";
        private const string CarTYpe_Spare_2 = "2";
        private const string CarTYw_Replacement = "R";

        public Guid RunPeriodCarId { get; set; }

        public string? CarType { get; set; }
        #region car type as enumeration
        public CarTypeEnum RunCarType
        {
            get {
                CarTypeEnum result = CarTypeEnum.Primary;
                switch (this.CarType)
                {
                    case CarTYpe_PRimary:
                        result = CarTypeEnum.Primary;
                        break;
                    case CarTYpe_Spare_1:
                        result = CarTypeEnum.Spare1;
                        break;
                    case CarTYpe_Spare_2:
                        result = CarTypeEnum.Spare2;
                        break;
                    case CarTYw_Replacement:
                        result = CarTypeEnum.Replacement;
                        break;
                }
                return result;
            }
            set
            {
                switch (value )
                {
                    case CarTypeEnum.Primary:
                        this.CarType = CarTYpe_PRimary;
                        break;
                    case CarTypeEnum.Spare1:
                        this.CarType = CarTYpe_Spare_1;
                        break;
                    case CarTypeEnum.Spare2:
                        this.CarType = CarTYpe_Spare_2;
                        break;
                    case CarTypeEnum.Replacement:
                        this.CarType = CarTYw_Replacement;
                        break;
                    default:
                        this.CarType = CarTYpe_PRimary;
                        break;
                }
            }
        }
        #endregion

        public Guid AssociateId { get; set; }   
        public Associate? AssociateData { get; set; }
        public Guid CarId { get; set; }
        public Car? CarData { get; set; }
        public string? Note { get; set; }

        public IList<RunCarCost>? CarCosts { get; set; }
    }

    public enum CarTypeEnum
    {
        Primary,
        Spare1,
        Spare2,
        Replacement
    }
}
