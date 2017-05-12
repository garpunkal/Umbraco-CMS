﻿using Umbraco.Core.Models;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Umbraco.Core.Persistence.Repositories
{
    public interface IRelationRepository : IUnitOfWorkRepository, IQueryRepository<int, IRelation>
    {

    }
}