using QualityWebApi.Filter.DLL;
using QualityWebApi.Filter.Domain;
using QualityWebApi.Filter.ef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QualityWebApi.Filter.BLL
{
    public class AudienceService
    {
        private AudienceRepository _repository;
        public AudienceService(GhDbContext dbcontext)
        {
            _repository = new AudienceRepository(dbcontext);
        }

        public Audience GetByName(string name)
        {
            return _repository.GetByName(name);
        }
        public Permission GetPermissingById(string authId, string userId)
        {
            return _repository.GetPermissingById(authId, userId);
        }
    }
}
