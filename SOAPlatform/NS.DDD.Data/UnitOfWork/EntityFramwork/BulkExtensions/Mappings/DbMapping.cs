using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySql.Data.MySqlClient.X.XDevAPI.Common;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data.Metadata.Edm;
using System.Linq;

namespace NS.DDD.Data.BulkExtensions
{
   
    public class DbMapping
    {
        private readonly DbContext _context;
        //private readonly Model model;

        private readonly Dictionary<string, TableMapping> _tableMappings = new Dictionary<string, TableMapping>();
        private readonly Dictionary<string, List<Property>> _tableColumnEdmMembers = new Dictionary<string, List<Property>>();
        private readonly Dictionary<string, List<string>> _primaryKeysMapping = new Dictionary<string, List<string>>();

        public DbMapping(DbContext dbContext)
        {
            this._context = dbContext;
            MapDb();
        }
        public TableMapping this[Type tableType]
        {
            get { return this[tableType.FullName]; }
        }

        public TableMapping this[string tableTypeFullName]
        {
            get { return _tableMappings[tableTypeFullName]; }
        }
        private void MapDb()
        {
            ExtractTableColumnEdmMembers();

            foreach (EntityType table in _context.Model.GetEntityTypes())
            {
                MapTable(table);
            }
        }

        private void ExtractTableColumnEdmMembers()
        {
            //IsAutoIncrement
            foreach (EntityType type in _context.Model.GetEntityTypes())
            {
                string identity = type.Name;

                List<Property> properties = new List<Property>(type.GetProperties());
                //properties.AddRange(type.NavigationProperties);
                _tableColumnEdmMembers[identity] = properties;
            }
        }

        private void MapTable(EntityType tableEdmType)
        {
            string identity = tableEdmType.Name;
           
            EntityType baseEdmType = tableEdmType.BaseType;

            var tableMapping = new TableMapping(identity, identity, identity);
            _tableMappings.Add(identity, tableMapping);
            //tableEdmType.GetPrivateFieldValue()
          

            var obj=(tableEdmType.GetKeys().ToList<Key>()).Select(key=> key.Properties).ToList<IReadOnlyList<Property>>();
            List<string> strs = new List<string>();
            foreach(var p in obj)
            {
               foreach(var m in p)
                {
                    strs.Add(m.Name);
                }
            }
            _primaryKeysMapping.Add(identity,strs);

           
            foreach (var prop in tableEdmType.GetProperties())
            {
                MapColumn(identity, _tableMappings[identity], prop);
            }
        }

        private void MapColumn(string identity, TableMapping tableMapping, Property property)
        {
            var columnName = property.Name;
            Property edmMember = _tableColumnEdmMembers[identity].Single(c => c.Name == columnName);
            string propertyName = edmMember.Name;

            ColumnMapping columnMapping = tableMapping.AddColumn(propertyName, columnName);
            BuildColumnMapping(identity, property, propertyName, columnMapping);
        }
        private void BuildColumnMapping(string identity, Property property, string propertyName, ColumnMapping columnMapping)
        {
            if (_primaryKeysMapping[identity].Contains(propertyName))
            {
                columnMapping.IsPk = true;
            }

            columnMapping.Nullable = property.IsNullable;
            //var columnType = property.Relational().ColumnType;
          
            columnMapping.DefaultValue = property.Relational().DefaultValue;
            if (property.GetMaxLength() != null)
            columnMapping.MaxLength =(int)property.GetMaxLength();

            //if(property.RequiresValueGenerator())
            //{
                columnMapping.IsIdentity = property.ValueGenerated==ValueGenerated.OnAdd;
                columnMapping.Computed = property.ValueGenerated==ValueGenerated.OnAddOrUpdate;
            //}



        }
    }
}
    //internal class DbMapping
    //{
    //    private readonly DbContext _context;
    //    //private readonly MetadataWorkspace _metadataWorkspace;
    //    //private readonly EntityContainer _codeFirstEntityContainer;
    //    private readonly Dictionary<string, TableMapping> _tableMappings = new Dictionary<string, TableMapping>();
    //    //private readonly Dictionary<string, List<EdmMember>> _tableColumnEdmMembers = new Dictionary<string, List<EdmMember>>();
    //    private readonly Dictionary<string, List<string>> _primaryKeysMapping = new Dictionary<string, List<string>>();



    //    //public void test(DbContext context)
    //    //{
    //    //    //var entitytypes= context.Model.GetEntityTypes();
    //    //    //var annotations = context.Model.GetAnnotations();

    //    //    foreach (var entityType in context.Model.GetEntityTypes())
    //    //    {
    //    //        var tableName = entityType.Relational().TableName;

    //    //        foreach (var propertyType in entityType.GetProperties())
    //    //        {
    //    //            var columnName = propertyType.Relational().ColumnName;
    //    //        }
    //    //    }

    //    //}

    //    public DbMapping(DbContext context)
    //    {


    //        throw new Exception(".net standard 暂不支持DbMapping");
    //        //_context = context;

    //        //var objectContext = ((IObjectContextAdapter)context).ObjectContext;
    //        //_metadataWorkspace = objectContext.MetadataWorkspace;

    //        //_codeFirstEntityContainer = _metadataWorkspace.GetEntityContainer("CodeFirstDatabase", DataSpace.SSpace);

    //        //MapDb();
    //    }

    //    public TableMapping this[Type tableType]
    //    {
    //        get { return this[tableType.FullName]; }
    //    }

    //    public TableMapping this[string tableTypeFullName]
    //    {
    //        get { return _tableMappings[tableTypeFullName]; }
    //    }

    //    //private void MapDb()
    //    //{
    //    //    ExtractTableColumnEdmMembers();

    //    //    List<EntityType> tables =
    //    //      _metadataWorkspace
    //    //        .GetItems(DataSpace.OCSpace)
    //    //        .Select(x => x.GetPrivateFieldValue("EdmItem") as EntityType)
    //    //        .Where(x => x != null)
    //    //        .ToList();

    //    //    foreach (var table in tables)
    //    //    {
    //    //        MapTable(table);
    //    //    }
    //    //}

    //    //private void ExtractTableColumnEdmMembers()
    //    //{
    //    //    IEnumerable<object> entitySetMaps =
    //    //      (IEnumerable<object>)_metadataWorkspace
    //    //        .GetItemCollection(DataSpace.CSSpace)[0]
    //    //        .GetPrivateFieldValue("EntitySetMaps");

    //    //    foreach (var entitySetMap in entitySetMaps)
    //    //    {
    //    //        IEnumerable<object> typeMappings = (IEnumerable<object>)entitySetMap.GetPrivateFieldValue("TypeMappings");
    //    //        foreach (var typeMapping in typeMappings)
    //    //        {
    //    //            IEnumerable<EdmType> types = (IEnumerable<EdmType>)typeMapping.GetPrivateFieldValue("Types");
    //    //            foreach (EntityType type in types)
    //    //            {
    //    //                string identity = type.FullName;

    //    //                List<EdmMember> properties = new List<EdmMember>(type.Properties);
    //    //                properties.AddRange(type.NavigationProperties);

    //    //                _tableColumnEdmMembers[identity] = properties;
    //    //            }
    //    //        }
    //    //    }
    //    //}

    //    //private void MapTable(EntityType tableEdmType)
    //    //{
    //    //    string identity = tableEdmType.FullName;
    //    //    EdmType baseEdmType = tableEdmType;
    //    //    EntitySet storageEntitySet = null;

    //    //    while (!_codeFirstEntityContainer.TryGetEntitySetByName(baseEdmType.Name, false, out storageEntitySet))
    //    //    {
    //    //        if (baseEdmType.BaseType == null) break;
    //    //        baseEdmType = baseEdmType.BaseType;
    //    //    }
    //    //    if (storageEntitySet == null) return;

    //    //    var tableName = (string)storageEntitySet.MetadataProperties["Table"].Value;
    //    //    var schemaName = (string)storageEntitySet.MetadataProperties["Schema"].Value;

    //    //    var tableMapping = new TableMapping(identity, schemaName, tableName);
    //    //    _tableMappings.Add(identity, tableMapping);
    //    //    _primaryKeysMapping.Add(identity, storageEntitySet.ElementType.KeyMembers.Select(x => x.Name).ToList());

    //    //    foreach (var prop in storageEntitySet.ElementType.Properties)
    //    //    {
    //    //        MapColumn(identity, _tableMappings[identity], prop);
    //    //    }
    //    //}

    //    //private void MapColumn(string identity, TableMapping tableMapping, EdmProperty property)
    //    //{
    //    //    var columnName = property.Name;
    //    //    EdmMember edmMember = _tableColumnEdmMembers[identity].Single(c => c.Name == columnName);
    //    //    string propertyName = edmMember.Name;

    //    //    ColumnMapping columnMapping = tableMapping.AddColumn(propertyName, columnName);
    //    //    BuildColumnMapping(identity, property, propertyName, columnMapping);
    //    //}

    //    //private void BuildColumnMapping(string identity, EdmProperty property, string propertyName, ColumnMapping columnMapping)
    //    //{
    //    //    if (_primaryKeysMapping[identity].Contains(propertyName))
    //    //    {
    //    //        columnMapping.IsPk = true;
    //    //    }

    //    //    foreach (var facet in property.TypeUsage.Facets)
    //    //    {
    //    //        switch (facet.Name)
    //    //        {
    //    //            case "Nullable":
    //    //                columnMapping.Nullable = (bool)facet.Value;
    //    //                break;
    //    //            case "DefaultValue":
    //    //                columnMapping.DefaultValue = facet.Value;
    //    //                break;
    //    //            case "StoreGeneratedPattern":
    //    //                columnMapping.IsIdentity = (StoreGeneratedPattern)facet.Value == StoreGeneratedPattern.Identity;
    //    //                columnMapping.Computed = (StoreGeneratedPattern)facet.Value == StoreGeneratedPattern.Computed;
    //    //                break;
    //    //            case "MaxLength":
    //    //                columnMapping.MaxLength = (int)facet.Value;
    //    //                break;
    //    //        }
    //    //    }
    //    //}
    //}
//}
