using System.Collections.Generic;
﻿using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using AutoMapper;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.Mvc;
using Umbraco.Web.Editors;
using System.Net.Http;
using Archetype.Umbraco.Models;

namespace Archetype.Umbraco.Api
{
    [PluginController("ArchetypeApi")]
    public class ArchetypeDataTypeController : UmbracoAuthorizedJsonController
    {

        public IEnumerable<object> GetAllPropertyEditors()
        {
            return
                global::Umbraco.Core.PropertyEditors.PropertyEditorResolver.Current.PropertyEditors
                    .Select(x => new {defaultPreValues = x.DefaultPreValues, alias = x.Alias, view = x.ValueEditor.View});
        }

        public object GetAll() 
        {
            var dataTypes = Services.DataTypeService.GetAllDataTypeDefinitions();
            return dataTypes.Select(t => new { guid = t.Key, name = t.Name });
        }

        public object GetByGuid(Guid guid)
        {
            var dataType = Services.DataTypeService.GetDataTypeDefinitionById(guid);
            if (dataType == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var dataTypeDisplay = Mapper.Map<IDataTypeDefinition, DataTypeDisplay>(dataType);
            return new { selectedEditor = dataTypeDisplay.SelectedEditor, preValues = dataTypeDisplay.PreValues };
        }

		// TODO: set up VS code formatting rules to match the rest of the solution
		public object GetConfiguration(Guid guid) {
			return GetExistingConfiguration(guid) ?? new ArchetypeConfiguration { Id = Guid.NewGuid() };
		}

		public void PutConfiguration([FromBody]ArchetypeConfiguration configuration) {
			System.Threading.Thread.Sleep(2000);

			if(configuration.Id == Guid.Empty) {
				// invalid
				throw new HttpResponseException(HttpStatusCode.NotFound);
			}
			var existingConfiguration = GetExistingConfiguration(configuration.Id);
			if(existingConfiguration == null) {
				existingConfiguration = new ArchetypeConfiguration {
					Id = configuration.Id,
					Configuration = configuration.Configuration
				};
				UmbracoDatabase.Insert(existingConfiguration);
			}
			else {
				existingConfiguration.Configuration = configuration.Configuration;
				UmbracoDatabase.Update(existingConfiguration);
			}
		}

		private ArchetypeConfiguration GetExistingConfiguration(Guid guid) {
			return UmbracoDatabase.FirstOrDefault<ArchetypeConfiguration>(string.Format("WHERE Id = '{0}'", guid));
		}

		private UmbracoDatabase UmbracoDatabase {
			get {
				return ApplicationContext.DatabaseContext.Database;
			}
		}
	}
}
