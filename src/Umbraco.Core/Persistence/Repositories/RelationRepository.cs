﻿using System;
using System.Collections.Generic;
using System.Linq;
using LightInject;
using NPoco;
using Umbraco.Core.Cache;
using Umbraco.Core.DI;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.EntityBase;
using Umbraco.Core.Models.Rdbms;

using Umbraco.Core.Persistence.Factories;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Umbraco.Core.Persistence.Repositories
{
    /// <summary>
    /// Represents a repository for doing CRUD operations for <see cref="Relation"/>
    /// </summary>
    internal class RelationRepository : NPocoRepositoryBase<int, IRelation>, IRelationRepository
    {
        private readonly IRelationTypeRepository _relationTypeRepository;

        public RelationRepository(IScopeUnitOfWork work, [Inject(RepositoryCompositionRoot.DisabledCache)] CacheHelper cache, ILogger logger, IRelationTypeRepository relationTypeRepository)
            : base(work, cache, logger)
        {
            _relationTypeRepository = relationTypeRepository;
        }

        #region Overrides of RepositoryBase<int,Relation>

        protected override IRelation PerformGet(int id)
        {
            var sql = GetBaseQuery(false);
            sql.Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<RelationDto>(SqlSyntax.SelectTop(sql, 1)).FirstOrDefault();
            if (dto == null)
                return null;

            var relationType = _relationTypeRepository.Get(dto.RelationType);
            if (relationType == null)
                throw new Exception(string.Format("RelationType with Id: {0} doesn't exist", dto.RelationType));

            var factory = new RelationFactory(relationType);
            return DtoToEntity(dto, factory);
        }

        protected override IEnumerable<IRelation> PerformGetAll(params int[] ids)
        {
            var sql = GetBaseQuery(false);
            if (ids.Length > 0)
                sql.WhereIn<RelationDto>(x => x.Id, ids);
            sql.OrderBy<RelationDto>(x => x.RelationType);
            var dtos = Database.Fetch<RelationDto>(sql);
            return DtosToEntities(dtos);
        }

        protected override IEnumerable<IRelation> PerformGetByQuery(IQuery<IRelation> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IRelation>(sqlClause, query);
            var sql = translator.Translate();
            sql.OrderBy<RelationDto>(x => x.RelationType);
            var dtos = Database.Fetch<RelationDto>(sql);
            return DtosToEntities(dtos);
        }

        private IEnumerable<IRelation> DtosToEntities(IEnumerable<RelationDto> dtos)
        {
            // in most cases, the relation type will be the same for all of them,
            // plus we've ordered the relations by type, so try to allocate as few
            // factories as possible - bearing in mind that relation types are cached
            RelationFactory factory = null;
            var relationTypeId = -1;

            return dtos.Select(x =>
            {
                if (relationTypeId != x.RelationType)
                    factory = new RelationFactory(_relationTypeRepository.Get(relationTypeId = x.RelationType));
                return DtoToEntity(x, factory);
            }).ToList();
        }

        private static IRelation DtoToEntity(RelationDto dto, RelationFactory factory)
        {
            var entity = factory.BuildEntity(dto);

            //on initial construction we don't want to have dirty properties tracked
            // http://issues.umbraco.org/issue/U4-1946
            ((TracksChangesEntityBase)entity).ResetDirtyProperties(false);

            return entity;
        }

        #endregion

        #region Overrides of NPocoRepositoryBase<int,Relation>

        protected override Sql<SqlContext> GetBaseQuery(bool isCount)
        {
            var sql = Sql();

            sql = isCount
                ? sql.SelectCount()
                : sql.Select<RelationDto>();

            sql
               .From<RelationDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "umbracoRelation.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                           {
                               "DELETE FROM umbracoRelation WHERE id = @Id"
                           };
            return list;
        }

        protected override Guid NodeObjectTypeId
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Unit of Work Implementation

        protected override void PersistNewItem(IRelation entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new RelationFactory(entity.RelationType);
            var dto = factory.BuildDto(entity);

            var id = Convert.ToInt32(Database.Insert(dto));
            entity.Id = id;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IRelation entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new RelationFactory(entity.RelationType);
            var dto = factory.BuildDto(entity);
            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        #endregion
    }
}