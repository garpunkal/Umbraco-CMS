﻿using System;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Umbraco.Core.Persistence.Repositories
{
    public interface IServerRegistrationRepository : IUnitOfWorkRepository, IQueryRepository<int, IServerRegistration>
    {
        void DeactiveStaleServers(TimeSpan staleTimeout);
    }
}