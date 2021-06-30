using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyllabusZip.API
{
    public interface IRestService<TRestModel>
    {
        Task<TRestModel> CreateObject(TRestModel T);

        Task<TRestModel> ReadObject();

        Task<TRestModel> UpdateObject(TRestModel T);

        Task<TRestModel> DeleteObject();
    }
}
