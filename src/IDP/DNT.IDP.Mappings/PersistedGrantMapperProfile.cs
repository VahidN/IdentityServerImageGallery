// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AutoMapper;
using DNT.IDP.DomainClasses.IdentityServer4Entities;

namespace DNT.IDP.Mappings
{
    /// <summary>
    /// Defines entity/model mapping for persisted grants.
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class PersistedGrantMapperProfile:Profile
    {
        /// <summary>
        /// <see cref="PersistedGrantMapperProfile">
        /// </see>
        /// </summary>
        public PersistedGrantMapperProfile()
        {
            // entity to model
            CreateMap<PersistedGrant, IdentityServer4.Models.PersistedGrant>(MemberList.Destination);

            // model to entity
            CreateMap<IdentityServer4.Models.PersistedGrant, PersistedGrant>(MemberList.Source);
        }
    }
}
