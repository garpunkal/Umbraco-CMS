﻿using System;
using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Umbraco.Core.Persistence.Repositories
{
    internal interface IMacroRepository : IQueryRepository<int, IMacro>, IReadRepository<Guid, IMacro>
    {

        //IEnumerable<IMacro> GetAll(params string[] aliases);

    }
}