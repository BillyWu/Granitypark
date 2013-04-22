using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using System.Reflection;
using System.IO;
using DevExpress.XtraEditors;
using DevExpress.DXperience.Demos;

namespace XtraReportsDemos {
	/// <summary>
	/// Summary description for ModuleControl.
	/// </summary>
	public class ModuleControl: TutorialControlBase {
		protected XtraReport fReport;
		protected string fileName = "";

		public ModuleControl() {
		}
				
		public virtual XtraReport Report { 
			get { return fReport; } 
			set { fReport = value; }
		}	
			
		public virtual void Activate() {
            System.ComponentModel.DXDisplayNameAttribute.UseResourceManager = true;
			Report = CreateReport();	
			File.Delete(fileName);
		}
		protected virtual XtraReport CreateReport() {
			return null;
		}
	}
}
