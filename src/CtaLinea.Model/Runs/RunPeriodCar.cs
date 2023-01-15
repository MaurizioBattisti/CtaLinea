using CtaLinea.Model.External;
using CtaLinea.QueryModel;

namespace CtaLinea.Model.Runs
{
    public class RunPeriodCar
    {
        private const string CarTYpe_PRimary = "P";
        private const string CarTYpe_Spare = "S";
        private const string CarTYw_Replacement = "R";

        public Guid RunCarId { get; set; }

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
                    case CarTYpe_Spare:
                        result = CarTypeEnum.Spare;
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
                    case CarTypeEnum.Spare:
                        this.CarType = CarTYpe_Spare;
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

        public string SearchText => (CarData?.Description ?? string.Empty) + " " + (AssociateData?.Description ?? string.Empty);

        public RunPeriodCar CreateCopy ()
        {
            return new RunPeriodCar()
            {
                RunCarId = this.RunCarId,
                CarType = this.CarType,
                AssociateId = this.AssociateId,
                AssociateData = this.AssociateData,
                CarId= this.CarId,
                CarData = this.CarData ,

                CarCosts = this.CarCosts ,

                Note= this.Note
            };
        }
    }

    public enum CarTypeEnum
    {
        Primary,
        Spare,
        Replacement
    }
}
