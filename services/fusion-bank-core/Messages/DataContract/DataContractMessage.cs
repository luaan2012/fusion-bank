using fusion.bank.core.Model;

namespace fusion.bank.core.Messages.DataContract
{
    public class DataContractMessage<TData>
    {
        public TData Data { get; set; }
        public ErrorMessage Error { get; set; }
        public bool Success { get; set; }

        public DataContractMessage() { }

        public DataContractMessage(TData data, bool success)
        {
            Data = data;
            Success = success;
        }

        public DataContractMessage<TData> HandleSuccess(TData data)
        {
            return new DataContractMessage<TData>
            {
                Data = data,
                Success = true
            };
        }

        public DataContractMessage<TData> HandleError(ErrorMessage data)
        {
            return new DataContractMessage<TData>
            {
                Error = data,
                Success = false
            };
        }
    }
}
