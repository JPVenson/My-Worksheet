using Newtonsoft.Json;

namespace MyWorksheet.Website.Shared.ViewModels
{
    public class StandardOperationResultBase<TObject> : OperationResultBase<TObject, string>
    {
        [JsonConstructor]
        protected StandardOperationResultBase()
        {

        }

        public StandardOperationResultBase(TObject successObject) : base(successObject)
        {
        }

        public StandardOperationResultBase(string error) : base(error)
        {
        }
    }
}