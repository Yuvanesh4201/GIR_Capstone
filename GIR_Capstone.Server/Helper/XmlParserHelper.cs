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
                    await reader.ReadAsync(); // Move inside the element
                    if (reader.NodeType == XmlNodeType.Text)
                    {
                        switch (elementName)
                        {
                            case "TIN":
                                string tin = reader.Value;
                                Console.WriteLine(tin);
                                break;
                            case "ResCountryCode":
                                string code = reader.Value;
                                Console.WriteLine(code);
                                break;
                            case "Name":
                                string name = reader.Value;
                                Console.WriteLine(name);
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
            await reader.ReadAsync(); // skip parent node

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
                            await ReadCE(reader.ReadSubtree(), corporateId, _context);
                            break;
                        case "ExcludedEntity":
                            break;
                    }
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
        public static async Task ReadUPE(XmlReader reader, string corporateId, ApplicationDbContext _context)
        {
            await reader.ReadAsync(); // skip parent node

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
        public static async Task ReadOtherUPE(XmlReader reader, string corporateId, ApplicationDbContext _context)
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
        public static async Task ReadOtherUPEId(XmlReader reader, string corporateId, ApplicationDbContext _context)
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
                    await reader.ReadAsync(); // Move inside the element
                    if (reader.NodeType == XmlNodeType.Text)
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
        /// <param name="_context">The _context<see cref="ApplicationDbContext"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public static async Task ReadCE(XmlReader reader, string corporateId, ApplicationDbContext _context)
        {
            await reader.ReadAsync(); // skip parent node

            CorporateEntity corporateEntity = new CorporateEntity();
            corporateEntity.Id = Guid.NewGuid(); // might need to add check
            corporateEntity.CorporationId = new Guid(corporateId);
            corporateEntity.ParentId = Guid.NewGuid(); //Remove in the future, replace with IsCe true/false
            List<EntityStatus> statuses = new List<EntityStatus>();

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "ID":
                            await ReadCEId(reader, corporateEntity, statuses);
                            break;
                        case "OwnershipChange":
                            break;
                        case "Ownership":
                            await ReadCEOwnership(reader, corporateEntity.Id, corporateId, _context);
                            break;
                        case "QIIR":
                            await ReadCEQiir(reader, corporateEntity);
                            break;
                        case "QUTPR":
                            break;
                    }
                }
            }

            _context.CorporateEntities.Add(corporateEntity);
            if (statuses.Count > 0)
                _context.EntityStatuses.AddRange(statuses);
            await _context.SaveChangesAsync();
            Console.WriteLine($"CE Inserted: {corporateEntity.Id}");
        }

        /// <summary>
        /// The ReadCEOwnership
        /// </summary>
        /// <param name="reader">The reader<see cref="XmlReader"/></param>
        /// <param name="entityId">The entityId<see cref="Guid"/></param>
        /// <param name="corporateId">The corporateId<see cref="string"/></param>
        /// <param name="_context">The _context<see cref="ApplicationDbContext"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private static async Task ReadCEOwnership(XmlReader reader, Guid entityId, string corporateId, ApplicationDbContext _context)
        {
            await reader.ReadAsync(); // skip parent node

            EntityOwnership ownership = new EntityOwnership();
            ownership.Id = Guid.NewGuid();
            ownership.OwnedEntityId = entityId;

            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string elementName = reader.Name;
                    await reader.ReadAsync(); // Move inside the element
                    if (reader.NodeType == XmlNodeType.Text)
                    {
                        switch (elementName)
                        {
                            case "OwnershipType":
                                ownership.OwnershipType = reader.Value.Substring(3);
                                break;
                            case "TIN":
                                var corporateEntity = await _context.CorporateEntities.FirstOrDefaultAsync(c => c.Tin == reader.Value && c.CorporationId.ToString() == corporateId);
                                ownership.OwnerEntityId = corporateEntity.Id;
                                break;
                            case "OwnershipPercentage":
                                ownership.OwnershipPercentage = Convert.ToDecimal(reader.Value);
                                break;
                        }
                    }
                }
            }

            _context.EntityOwnerships.Add(ownership);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Ownership Row Inserted: {ownership.Id}");
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
                            corporateEntity.QIIR_Status = reader.ReadElementContentAsStringAsync().Result; // Might change depending on future requirements
                            break;
                        case "Exception":
                            break;
                    }
                }
            }
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
                    await reader.ReadAsync(); // Move inside the element
                    if (reader.NodeType == XmlNodeType.Text)
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
    }
}
