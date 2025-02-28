namespace GIR_Capstone.Server.Helper
{
    using Microsoft.EntityFrameworkCore;
    using System.Xml;

    /// <summary>
    /// Defines the <see cref="XmlParserHelper" />
    /// </summary>
    public static class XmlParserHelper
    {
        /// <summary>
        /// The ReadFilingCE
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public static async Task ReadFilingCE(XmlReader reader)
        {
            await reader.ReadAsync(); // skip parent node

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string elementName = reader.Name;
                    
                    if (await reader.ReadAsync() && reader.NodeType == XmlNodeType.Text)
                    {
                        switch (elementName)
                        {
                            case "TIN":
                                string tin = reader.Value;
                                break;
                            case "ResCountryCode":
                                string code = reader.Value;
                                break;
                            case "Name":
                                string name = reader.Value;
                                break;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// The ReadCorporateStructure
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <param name="corporateId">The corporateId<see cref="string"/></param>
        /// <param name="_context">The _context<see cref="ApplicationDbContext"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public static async Task ReadCorporateStructure(XmlReader reader, string corporateId, ApplicationDbContext _context)
        {
            Dictionary<string, EntityOwnership> missingOwnerships = new Dictionary<string, EntityOwnership>();

            await reader.ReadAsync(); // skip parent node

            //Order Independent
            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "UPE":
                            await ReadUPE(reader.ReadSubtree(), corporateId, _context);
                            break;
                        case "CE":
                            await ReadCE(reader.ReadSubtree(), corporateId, missingOwnerships, _context);
                            break;
                        case "ExcludedEntity":
                            break;
                    }
                }
            }

            if (missingOwnerships.Count > 0)
            {
                foreach (var ownership in missingOwnerships)
                {
                    var entity = _context.CorporateEntities.FirstOrDefaultAsync(c => c.Tin == ownership.Key && c.CorporationId.ToString() == corporateId);
                    ownership.Value.OwnerEntityId = entity.Result.Id;
                    _context.EntityOwnerships.Add(ownership.Value);

                    var ownedEntity = _context.CorporateEntities.FirstOrDefault(c => c.Id == ownership.Value.OwnedEntityId);
                    ownedEntity.ParentId = entity.Result.Id; //temp
                    _context.CorporateEntities.Update(ownedEntity); //temp (bad code)

                }
            }
        }

        /// <summary>
        /// The ReadUPE
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <param name="corporateId">The corporateId<see cref="string"/></param>
        /// <param name="_context">The _context<see cref="ApplicationDbContext"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private static async Task ReadUPE(XmlReader reader, string corporateId, ApplicationDbContext _context)
        {
            await reader.ReadAsync(); // skip parent node

            //Order Independent
            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "OtherUPE":
                            await ReadOtherUPE(reader.ReadSubtree(), corporateId, _context);
                            break;
                        case "ExcludedUPE":
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// The ReadOtherUPE
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <param name="corporateId">The corporateId<see cref="string"/></param>
        /// <param name="_context">The _context<see cref="ApplicationDbContext"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private static async Task ReadOtherUPE(XmlReader reader, string corporateId, ApplicationDbContext _context)
        {
            await reader.ReadAsync(); // skip parent node

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "ID")
                        await ReadOtherUPEId(reader.ReadSubtree(), corporateId, _context);
                }
            }
        }

        /// <summary>
        /// The ReadOtherUPEId
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <param name="corporateId">The corporateId<see cref="string"/></param>
        /// <param name="_context">The _context<see cref="ApplicationDbContext"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private static async Task ReadOtherUPEId(XmlReader reader, string corporateId, ApplicationDbContext _context)
        {
            await reader.ReadAsync(); // skip parent node
            CorporateEntity corporateEntity = new CorporateEntity();
            corporateEntity.Id = Guid.NewGuid(); // might need to add check
            corporateEntity.CorporationId = new Guid(corporateId);
            List<EntityStatus> statuses = new List<EntityStatus>();

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string elementName = reader.Name;

                    if (await reader.ReadAsync() && reader.NodeType == XmlNodeType.Text)
                    {
                        switch (elementName)
                        {
                            case "TIN":
                                corporateEntity.Tin = reader.Value;
                                break;
                            case "ResCountryCode":
                                corporateEntity.Jurisdiction = reader.Value;
                                break;
                            case "Name":
                                corporateEntity.Name = reader.Value;
                                break;
                            case "GlobeStatus":
                                EntityStatus entityStatus = new EntityStatus();
                                entityStatus.EntityId = corporateEntity.Id;
                                entityStatus.Status = reader.Value.Substring(3);
                                statuses.Add(entityStatus);
                                break;
                        }
                    }
                }
            }

            _context.CorporateEntities.Add(corporateEntity);
            if (statuses.Count > 0)
                _context.EntityStatuses.AddRange(statuses);
            await _context.SaveChangesAsync();
            Console.WriteLine($"UPE Inserted: {corporateEntity.Id}");
        }

        /// <summary>
        /// The ReadCE
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <param name="corporateId">The corporateId<see cref="string"/></param>
        /// <param name="missingOwnerships">The missingOwnerships<see cref="Dictionary{string, EntityOwnership}"/></param>
        /// <param name="_context">The _context<see cref="ApplicationDbContext"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private static async Task ReadCE(XmlReader reader, string corporateId, Dictionary<string, EntityOwnership> missingOwnerships, ApplicationDbContext _context)
        {
            await reader.ReadAsync(); // skip parent node

            CorporateEntity corporateEntity = new CorporateEntity();
            List<EntityStatus> statuses = new List<EntityStatus>();
            List<EntityOwnership> ownerships = new List<EntityOwnership>();

            corporateEntity.Id = Guid.NewGuid(); // might need to add check
            corporateEntity.CorporationId = new Guid(corporateId);
            //corporateEntity.ParentId = ; //Remove in the future, replace with IsCe true/false

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "ID":
                            await ReadCEId(reader.ReadSubtree(), corporateEntity, statuses);
                            break;
                        case "OwnershipChange":
                            break;
                        case "Ownership":
                            await ReadCEOwnership(reader.ReadSubtree(), ownerships, corporateEntity.Id, corporateId, _context, corporateEntity, missingOwnerships);
                            break;
                        case "QIIR":
                            await ReadCEQiir(reader.ReadSubtree(), corporateEntity);
                            break;
                        case "QUTPR":
                            break;
                    }
                }
            }

            _context.CorporateEntities.Add(corporateEntity);
            await _context.SaveChangesAsync();
            Console.WriteLine($"CE Inserted: {corporateEntity.Id}");

            if (ownerships.Count > 0)
                _context.EntityOwnerships.AddRange(ownerships);
            int ownershipsInserted = await _context.SaveChangesAsync();
            Console.WriteLine($"Ownership Rows Inserted: {ownershipsInserted}");

            if (statuses.Count > 0)
                _context.EntityStatuses.AddRange(statuses);
            int rowsInserted = await _context.SaveChangesAsync();
            Console.WriteLine($"EntityStatuses Inserted: {corporateEntity.Id} , Rows: {rowsInserted} ");
        }

        /// <summary>
        /// The ReadCEId
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <param name="corporateEntity">The corporateEntity<see cref="CorporateEntity"/></param>
        /// <param name="statuses">The statuses<see cref="List{EntityStatus}"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private static async Task ReadCEId(XmlReader reader, CorporateEntity corporateEntity, List<EntityStatus> statuses)
        {
            await reader.ReadAsync(); // skip parent node

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string elementName = reader.Name;

                    if (await reader.ReadAsync() && reader.NodeType == XmlNodeType.Text)
                    {
                        switch (elementName)
                        {
                            case "TIN":
                                corporateEntity.Tin = reader.Value;
                                break;
                            case "ResCountryCode":
                                corporateEntity.Jurisdiction = reader.Value;
                                break;
                            case "Name":
                                corporateEntity.Name = reader.Value;
                                break;
                            case "GlobeStatus":
                                EntityStatus entityStatus = new EntityStatus();
                                entityStatus.EntityId = corporateEntity.Id;
                                entityStatus.Status = reader.Value.Substring(3);
                                statuses.Add(entityStatus);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The ReadCEOwnership
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <param name="ownerships">The ownerships<see cref="List{EntityOwnership}"/></param>
        /// <param name="entityId">The entityId<see cref="Guid"/></param>
        /// <param name="corporateId">The corporateId<see cref="string"/></param>
        /// <param name="_context">The _context<see cref="ApplicationDbContext"/></param>
        /// <param name="corporate">The corporate<see cref="CorporateEntity"/></param>
        /// <param name="missingOwnerships">The missingOwnerships<see cref="Dictionary{string, EntityOwnership}"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private static async Task ReadCEOwnership(XmlReader reader, List<EntityOwnership> ownerships, Guid entityId, string corporateId, ApplicationDbContext _context, CorporateEntity corporate, Dictionary<string, EntityOwnership> missingOwnerships)
        {
            await reader.ReadAsync(); // skip parent node

            EntityOwnership ownership = new EntityOwnership();
            ownership.Id = Guid.NewGuid();
            ownership.OwnedEntityId = entityId;
            string tin = string.Empty;

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string elementName = reader.Name;
                    
                    if (await reader.ReadAsync() && reader.NodeType == XmlNodeType.Text)
                    {
                        switch (elementName)
                        {
                            case "OwnershipType":
                                ownership.OwnershipType = reader.Value.Substring(3);
                                break;
                            case "TIN":
                                var corporateEntity = await _context.CorporateEntities.FirstOrDefaultAsync(c => c.Tin == reader.Value && c.CorporationId.ToString() == corporateId);
                                tin = reader.Value;
                                ownership.OwnerEntityId = corporateEntity.Id;
                                corporate.ParentId = corporateEntity.Id; //temp
                                break;
                            case "OwnershipPercentage":
                                ownership.OwnershipPercentage = Convert.ToDecimal(reader.Value);
                                break;
                        }
                    }
                }
            }

            if (ownership.OwnerEntityId != Guid.Empty)
                ownerships.Add(ownership);
            else
                missingOwnerships.Add(tin, ownership);
        }

        /// <summary>
        /// The ReadCEQiir
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <param name="corporateEntity">The corporateEntity<see cref="CorporateEntity"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private static async Task ReadCEQiir(XmlReader reader, CorporateEntity corporateEntity)
        {
            await reader.ReadAsync(); // skip parent node

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "POPE-IPE":
                            corporateEntity.QIIR_Status = reader.ReadElementContentAsStringAsync().Result.Substring(3); // Might change depending on future requirements
                            break;
                        case "Exception":
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// The GetCorporateStructure
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <returns>The <see cref="Task{List{CorporateEntityDto}}"/></returns>
        public static async Task<List<CorporateEntityDto>> GetCorporateStructure(XmlReader reader)
        {
            List<CorporateEntityDto> entities = new List<CorporateEntityDto>();
            Dictionary<string, List<(Guid, Guid)>> missingOwnerships = new Dictionary<string, List<(Guid,Guid)>>(); 
            //potentially Dictionary<string (Tin), List<string (corporateid) ,string (ownershipid)>
            //Retreive the Tin (1st Loop), Look for Owned (2nd Loop), Look for Ownership to update (3rd Loop)

            await reader.ReadAsync(); // skip parent node

            //Order Independent
            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "UPE":
                            var upe = await GetUPE(reader.ReadSubtree());
                            entities.Add(upe);
                            break;
                        case "CE":
                            var ce = await GetCE(reader.ReadSubtree(), entities, missingOwnerships);
                            entities.Add(ce);
                            break;
                        case "ExcludedEntity":
                            break;
                    }
                }
            }

            /* if (missingOwnerships.Count > 0)
                        {
                           foreach(var owner in entities)
                           {
                                foreach(var ownership in missingOwnerships)
                                {
                                    if(ownership.Key == owner.Tin)
                                    {
                                        foreach(var item in ownership.Value)
                                        {
                                            var owned = entities.Find(e => e.Id == item.Item1);
                                            var ownedOwnership = owned.Ownerships.Find(owner => owner.Id == item.Item2);
                                            ownedOwnership.OwnerName = owner.Name;
                                            ownedOwnership.OwnerEntityId = owner.Id;
                                        }
                                    }
                                }
                           }
                        }*/

            if (missingOwnerships.Count > 0)
            {
                var entityLookup = entities.ToDictionary(e => e.Id);
                var tinLookup = entities.ToDictionary(e => e.Tin); // Fast TIN lookup
                var ownershipLookup = entities
                    .Where(e => e.Ownerships.Any())
                    .SelectMany(e => e.Ownerships, (e, o) => new { e.Id, Ownership = o })
                    .GroupBy(x => x.Id)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.Ownership).ToArray());

                foreach (var ownership in missingOwnerships)
                {
                    // Use `TIN` lookup instead of `FirstOrDefault()`
                    if (tinLookup.TryGetValue(ownership.Key, out var ownerEntity))
                    {
                        foreach (var (ownedEntityId, ownershipId) in ownership.Value)
                        {
                            if (entityLookup.TryGetValue(ownedEntityId, out var ownedEntity) &&
                                ownershipLookup.TryGetValue(ownedEntityId, out var ownedOwnerships))
                            {
                                var ownedOwnership = ownedOwnerships.FirstOrDefault(o => o.Id == ownershipId);
                                if (ownedOwnership != null)
                                {
                                    ownedOwnership.OwnerName = ownerEntity.Name;
                                    ownedOwnership.OwnerEntityId = ownerEntity.Id;
                                }
                            }
                        }
                    }
                }
            }

            return entities;
        }

        /// <summary>
        /// The GetUPE
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <returns>The <see cref="Task{CorporateEntityDto}"/></returns>
        private static async Task<CorporateEntityDto> GetUPE(XmlReader reader)
        {
            await reader.ReadAsync(); // skip parent node

            //Order Independent
            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "OtherUPE":
                            return await GetOtherUPE(reader.ReadSubtree());
                        case "ExcludedUPE":
                            break;
                    }
                }
            }

            return null!;
        }

        /// <summary>
        /// The GetOtherUPE
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <returns>The <see cref="Task{CorporateEntityDto}"/></returns>
        private static async Task<CorporateEntityDto> GetOtherUPE(XmlReader reader)
        {
            await reader.ReadAsync(); // skip parent node

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "ID")
                        return await GetOtherUPEId(reader.ReadSubtree());
                }
            }

            return null!;
        }

        /// <summary>
        /// The GetOtherUPEId
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <returns>The <see cref="Task{CorporateEntityDto}"/></returns>
        private static async Task<CorporateEntityDto> GetOtherUPEId(XmlReader reader)
        {
            await reader.ReadAsync();
            CorporateEntityDto corporateEntity = new CorporateEntityDto();
            corporateEntity.Id = Guid.NewGuid();
            List<string> statuses = new List<string>();

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string elementName = reader.Name;

                    if (await reader.ReadAsync() && reader.NodeType == XmlNodeType.Text)
                    {
                        switch (elementName)
                        {
                            case "TIN":
                                corporateEntity.Tin = reader.Value;
                                break;
                            case "ResCountryCode":
                                corporateEntity.Jurisdiction = reader.Value;
                                break;
                            case "Name":
                                corporateEntity.Name = reader.Value;
                                break;
                            case "GlobeStatus":
                                statuses.Add(reader.Value.Substring(3));
                                break;
                        }
                    }
                }
            }

            corporateEntity.Statuses = statuses;

            return corporateEntity;
        }

        /// <summary>
        /// The GetCE
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <param name="entities">The entities<see cref="List{CorporateEntityDto}"/></param>
        /// <param name="missingOwnerships">The missingOwnerships<see cref="Dictionary{string,OwnershipDto}"/></param>
        /// <returns>The <see cref="Task{CorporateEntityDto}"/></returns>
        private static async Task<CorporateEntityDto> GetCE(XmlReader reader, List<CorporateEntityDto> entities, Dictionary<string, List<(Guid, Guid)>> missingOwnerships)
        {
            await reader.ReadAsync(); // skip parent node

            CorporateEntityDto corporateEntity = new CorporateEntityDto();
            corporateEntity.ParentId = Guid.NewGuid(); //temp
            List<string> statuses = new List<string>();
            List<OwnershipDto> ownerships = new List<OwnershipDto>();

            corporateEntity.Id = Guid.NewGuid(); // might need to add check

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "ID":
                            await GetCEId(reader.ReadSubtree(), corporateEntity, statuses);
                            break;
                        case "OwnershipChange":
                            break;
                        case "Ownership":
                            await GetCEOwnership(reader.ReadSubtree(), corporateEntity, ownerships, entities, missingOwnerships);
                            break;
                        case "QIIR":
                            await GetCEQiir(reader.ReadSubtree(), corporateEntity);
                            break;
                        case "QUTPR":
                            break;
                    }
                }
            }

            corporateEntity.Ownerships = ownerships;
            return corporateEntity;
        }

        /// <summary>
        /// The GetCEId
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <param name="corporateEntity">The corporateEntity<see cref="CorporateEntityDto"/></param>
        /// <param name="statuses">The statuses<see cref="List{string}"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private static async Task GetCEId(XmlReader reader, CorporateEntityDto corporateEntity, List<string> statuses)
        {
            await reader.ReadAsync(); // skip parent node

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string elementName = reader.Name;

                    if (await reader.ReadAsync() && reader.NodeType == XmlNodeType.Text)
                    {
                        switch (elementName)
                        {
                            case "TIN":
                                corporateEntity.Tin = reader.Value;
                                break;
                            case "ResCountryCode":
                                corporateEntity.Jurisdiction = reader.Value;
                                break;
                            case "Name":
                                corporateEntity.Name = reader.Value;
                                break;
                            case "GlobeStatus":
                                statuses.Add(reader.Value.Substring(3));
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The GetCEOwnership
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <param name="corporateEntity">The corporateEntity<see cref="CorporateEntityDto"/></param>
        /// <param name="ownerships">The ownerships<see cref="List{OwnershipDto}"/></param>
        /// <param name="id">The id<see cref="Guid"/></param>
        /// <param name="entities">The entities<see cref="List{CorporateEntityDto}"/></param>
        /// <param name="missingOwnerships">The missingOwnerships<see cref="Dictionary{string, OwnershipDto}"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private static async Task GetCEOwnership(XmlReader reader, CorporateEntityDto corporateEntity, List<OwnershipDto> ownerships, List<CorporateEntityDto> entities, Dictionary<string, List<(Guid, Guid)>> missingOwnerships)
        {
            await reader.ReadAsync(); // skip parent node

            OwnershipDto ownership = new OwnershipDto();
            ownership.Id = Guid.NewGuid();
            ownership.OwnedEntityId = corporateEntity.Id;
            string tin = string.Empty;

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string elementName = reader.Name;

                    if (await reader.ReadAsync() && reader.NodeType == XmlNodeType.Text)
                    {
                        switch (elementName)
                        {
                            case "OwnershipType":
                                ownership.OwnershipType = reader.Value.Substring(3);
                                break;
                            case "TIN":
                                var upe = entities.FirstOrDefault(corporateEntity => corporateEntity.Tin == reader.Value);
                                if (upe != null)
                                {
                                    ownership.OwnerName = upe.Name;
                                    ownership.OwnerEntityId = upe.Id;
                                }
                                else
                                    tin = reader.Value;
                                break;
                            case "OwnershipPercentage":
                                ownership.OwnershipPercentage = Convert.ToDecimal(reader.Value);
                                break;
                        }
                    }
                }
            }

            ownerships.Add(ownership);
            if (ownership.OwnerEntityId == Guid.Empty)
            {
                if (!missingOwnerships.ContainsKey(tin))
                    missingOwnerships[tin] = new List<(Guid,Guid)>();

                missingOwnerships[tin].Add((corporateEntity.Id, ownership.Id));
            }
                
        }

        /// <summary>
        /// The GetCEQiir
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <param name="corporateEntity">The corporateEntity<see cref="CorporateEntityDto"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private static async Task GetCEQiir(XmlReader reader, CorporateEntityDto corporateEntity)
        {
            await reader.ReadAsync(); // skip parent node

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "POPE-IPE":
                            corporateEntity.qiir_Status = reader.ReadElementContentAsStringAsync().Result.Substring(3); // Might change depending on future requirements
                            break;
                        case "Exception":
                            break;
                    }
                }
            }
        }
    }
}
