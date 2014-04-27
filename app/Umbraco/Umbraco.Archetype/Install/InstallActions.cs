using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umbraco.interfaces;
using Umbraco.Web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using Archetype.Umbraco.Models;
using System.Xml.Linq;
using System.Xml;

// TODO: set up VS code formatting rules to match the rest of the solution
namespace Archetype.Umbraco.Install {
	public class InstallActions : IPackageAction {
		public string Alias() {
			return "ArchetypeInstall";
		}

		public bool Execute(string packageName, System.Xml.XmlNode xmlData) {
			var db = UmbracoContext.Current.Application.DatabaseContext.Database;
			if(db.TableExist("Archetype") == false) {
				db.CreateTable<ArchetypeConfiguration>();
			}
			return true;
		}

		public System.Xml.XmlNode SampleXml() {
			var element = XElement.Parse(string.Format(@"<Action runat=""install"" undo=""true"" alias=""{0}"" />", Alias()));
			using(var xmlReader = element.CreateReader()) {
				var xmlDoc = new XmlDocument();
				xmlDoc.Load(xmlReader);
				return xmlDoc;
			}
		}

		public bool Undo(string packageName, System.Xml.XmlNode xmlData) {
			var db = UmbracoContext.Current.Application.DatabaseContext.Database;
			if(db.TableExist("Archetype")) {
				db.DropTable<ArchetypeConfiguration>();
			}
			return true;
		}
	}
}
