﻿using AutoMapper;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Mapping;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Models.Mapping
{
    internal class TemplateModelMapper : ModelMapperConfiguration
    {
        public override void ConfigureMappings(IMapperConfiguration config)
        {
            config.CreateMap<ITemplate, TemplateDisplay>()
                .ForMember(x => x.Notifications, exp => exp.Ignore());

            config.CreateMap<TemplateDisplay, Template>()
                .ForMember(x => x.Key, exp => exp.Ignore())
                .ForMember(x => x.Path, exp => exp.Ignore())
                .ForMember(x => x.CreateDate, exp => exp.Ignore())
                .ForMember(x => x.UpdateDate, exp => exp.Ignore())
                .ForMember(x => x.VirtualPath, exp => exp.Ignore())
                .ForMember(x => x.Path, exp => exp.Ignore())
                .ForMember(x => x.MasterTemplateId, exp => exp.Ignore()) // ok, assigned when creating the template
                .ForMember(x => x.IsMasterTemplate, exp => exp.Ignore())
                .ForMember(x => x.HasIdentity, exp => exp.Ignore());
        }
    }
}
