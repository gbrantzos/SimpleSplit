using SimpleSplit.Application.Base;
using SimpleSplit.Domain.Features.Common;

namespace SimpleSplit.Application.Features.Common
{
    public class GetImage : Request<Image>
    {
        public string EntityType { get; set; }
        public long EntityID { get; set; }
    }
}