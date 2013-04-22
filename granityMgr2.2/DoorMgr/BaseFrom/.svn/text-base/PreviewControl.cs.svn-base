using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrinting.Control;

namespace XtraReportsDemos {
	/// <summary>
	/// Summary description for ModuleControl.
	/// </summary>
	public class PreviewControl: ModuleControl {
		public class DesignForm : DevExpress.XtraReports.UserDesigner.XRDesignFormEx {
			protected override void SaveLayout() { }
			protected override void RestoreLayout() { }
		}
		protected System.Windows.Forms.Panel pnlSettings;
		protected DevExpress.XtraPrinting.Control.PrintControl printControl;
		protected DevExpress.XtraPrinting.PrintingSystem printingSystem;
		private System.ComponentModel.IContainer components;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		protected DevExpress.XtraEditors.SimpleButton smplEdit;
		protected PrintBarManager fPrintBarManager;
		
		public PreviewControl() {
			InitializeComponent();	
			printingSystem.SetCommandVisibility(PrintingSystemCommand.ClosePreview, DevExpress.XtraPrinting.CommandVisibility.None);
			fPrintBarManager = CreatePrintBarManager(printControl);
		}
				
		public override XtraReport Report { 
			get { return fReport; } 
			set {
				if (fReport != value) {
					if (fReport != null)
						fReport.Dispose();
					fReport = value;
					if (fReport == null) 
						return;
					printingSystem.ClearContent();
					Invalidate();
					Update();
					fileName = XtraReportsDemos.Helper.GetReportPath(fReport, "repx");
					fReport.PrintingSystem = printingSystem;

					fReport.CreateDocument(true);
				} 
			}
		}
		
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		/// 
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.smplEdit = new DevExpress.XtraEditors.SimpleButton();
            this.printControl = new DevExpress.XtraPrinting.Control.PrintControl();
            this.printingSystem = new DevExpress.XtraPrinting.PrintingSystem(this.components);
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.pnlSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.printingSystem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.smplEdit);
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSettings.Location = new System.Drawing.Point(0, 0);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(700, 36);
            this.pnlSettings.TabIndex = 0;
            // 
            // smplEdit
            // 
            this.smplEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.smplEdit.Location = new System.Drawing.Point(619, 6);
            this.smplEdit.Name = "smplEdit";
            this.smplEdit.Size = new System.Drawing.Size(75, 24);
            this.smplEdit.TabIndex = 2;
            this.smplEdit.Text = "Edit";
            this.smplEdit.Visible = false;
            this.smplEdit.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // printControl
            // 
            this.printControl.BackColor = System.Drawing.Color.Empty;
            this.printControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.printControl.ForeColor = System.Drawing.Color.Empty;
            this.printControl.IsMetric = false;
            this.printControl.Location = new System.Drawing.Point(2, 2);
            this.printControl.Name = "printControl";
            this.printControl.PrintingSystem = this.printingSystem;
            this.printControl.Size = new System.Drawing.Size(696, 356);
            this.printControl.TabIndex = 1;
            this.printControl.TabStop = false;
            this.printControl.TooltipFont = new System.Drawing.Font("Tahoma", 9F);
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.panelControl1.Controls.Add(this.printControl);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 36);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(700, 360);
            this.panelControl1.TabIndex = 4;
            // 
            // PreviewControl
            // 
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.pnlSettings);
            this.Name = "PreviewControl";
            this.Size = new System.Drawing.Size(700, 396);
            this.pnlSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.printingSystem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		protected PrintBarManager CreatePrintBarManager(PrintControl pc) {
			PrintBarManager printBarManager = new PrintBarManager();
			printBarManager.Form = printControl;
			printBarManager.Initialize(pc);
			printBarManager.MainMenu.Visible = false;
			printBarManager.AllowCustomization = false;
			return printBarManager;
		}
		private void ShowDesignerForm(Form designForm, Form parentForm) {
            designForm.MinimumSize = parentForm.MinimumSize;
            if(parentForm.WindowState == FormWindowState.Normal)
			    designForm.Bounds = parentForm.Bounds;
            designForm.WindowState = parentForm.WindowState;
			parentForm.Visible = false;
			designForm.ShowDialog();
            parentForm.Visible = true;
		}
		protected virtual void InitializeControls() {
		}
		private void simpleButton1_Click(object sender, EventArgs e) {
			string saveFileName = XtraReportsDemos.Helper.GetReportPath(fReport, "sav");
            fReport.PrintingSystem.ExecCommand(PrintingSystemCommand.StopPageBuilding);
            fReport.SaveLayout(saveFileName);
            using(XtraReport newReport = XtraReport.FromFile(saveFileName, true)) {
                XRDesignFormExBase designForm = new CustomDesignForm();
                designForm.OpenReport(newReport);
                designForm.FileName = fileName;
                ShowDesignerForm(designForm, this.FindForm());
                if(designForm.FileName != fileName && File.Exists(designForm.FileName))
                    File.Copy(designForm.FileName, fileName, true);

                designForm.OpenReport((XtraReport)null);
                designForm.Dispose();
            }
            if(File.Exists(fileName)) {
                fReport.LoadLayout(fileName);
                File.Delete(fileName);
                fReport.CreateDocument(true);
            }
            
            ShowParameters();
			File.Delete(saveFileName);
			InitializeControls();
		}

        protected void ShowParameters() {
            this.printingSystem.ExecCommand(DevExpress.XtraPrinting.PrintingSystemCommand.Parameters, new object[] { true });
        }
	}
	public class Helper {
		public const int ICC_USEREX_CLASSES = 0x00000200;

		[StructLayout(LayoutKind.Sequential, Pack=1)]
			public class INITCOMMONCONTROLSEX {
			public int  dwSize = 8; //ndirect.DllLib.sizeOf(this);
			public int  dwICC;
		}
		[DllImport("comctl32.dll")]
		public static extern bool InitCommonControlsEx(INITCOMMONCONTROLSEX icc);

		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string libname);
		
		[DllImport("kernel32.dll")]
		public static extern bool FreeLibrary(IntPtr hModule);
		
		public static string GetReportPath(DevExpress.XtraReports.UI.XtraReport fReport, string ext) {
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
			string repName = fReport.Name;
			if(repName.Length == 0) repName = fReport.GetType().Name;
			string dirName = Path.GetDirectoryName(asm.Location);
			return Path.Combine(dirName, repName + "." + ext);
		}
		public static Image LoadImage(string name) {
			Bitmap bmp = new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("XtraReportsDemos." + name));
			bmp.MakeTransparent(Color.Magenta);
			return bmp;
		}
        public static void PatchConnections(string startupPath) {
            //string ignore = global::XtraReportsDemos.Properties.Settings.Default.nwindConnectionString;
            //ignore = global::XtraReportsDemos.Properties.Settings.Default.countriesDBConnectionString;
            //ignore = global::XtraReportsDemos.Properties.Settings.Default.CarsDBConnectionString;
            //ignore = global::XtraReportsDemos.Properties.Settings.Default.StyleSheetLavenderPath;
            //ignore = global::XtraReportsDemos.Properties.Settings.Default.BiolifeTxtPath;

            //string[] connections = new String[] { "nwindConnectionString", "countriesDBConnectionString", "CarsDBConnectionString" };
            //foreach(string s in connections)
            //    PatchConnection(s, startupPath);
            //string[] pathes = new String[] { "StyleSheetLavenderPath", "BiolifeTxtPath" };
            //foreach(string s in pathes)
            //    PatchPath(s, startupPath);
        }
        static void PatchPath(string propertyName, string startupPath) {
            //string path = (string)global::XtraReportsDemos.Properties.Settings.Default[propertyName];
            //string fileName = System.IO.Path.GetFileName(path);
            //string newPath = DevExpress.Utils.FilesHelper.FindingFileName(startupPath, "Data\\" + fileName, false);
            //if(System.IO.File.Exists(newPath))
            //    global::XtraReportsDemos.Properties.Settings.Default[propertyName] = newPath;
        }
        static void PatchConnection(string propertyName, string startupPath) {
            //string s = (string)global::XtraReportsDemos.Properties.Settings.Default[propertyName];
            //System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(s, @".*Data Source=(?<dataSource>[^;]*)");
            //System.Text.RegularExpressions.Group matchGroup = m.Groups["dataSource"];
            //if(matchGroup.Success) {
            //    string fileName = System.IO.Path.GetFileName(matchGroup.Value);
            //    string path = DevExpress.Utils.FilesHelper.FindingFileName(startupPath, "Data\\" + fileName, false);
            //    if(File.Exists(path))
            //        global::XtraReportsDemos.Properties.Settings.Default[propertyName] = s.Replace(matchGroup.Value, path);
            //}
        }
	}
	public class ReportNames {
		public const string
			PivotGrid = "Pivot Grid",
			MailMerge = "Mail Merge",
			OddEvenStyles = "Products by Categories",
			CrossBands = "Cross Band Controls",
			AnchorVertical = "Vertical Anchoring",
			NorthwindTraders_Products = "Products List",
			NorthwindTraders_Catalog = "Fall Catalog",
			NorthwindTraders_Invoice = "Invoice",
			MasterDetailReport = "Suppliers",
			MultiColumnReport = "Customers",
			IListDatasource = "Fishes",
			CustomDraw = "Population",
			ShrinkGrow = "Employees",
			BarCodes_ProductLabels = "Product Labels",
			BarCodes_BarcodeTypes = "Barcode Types",
			RichText = "Cars",
			Subreports = "Customers List",
			TableReport = "Customer Order",
			TreeView = "Countries",
			Charts = "Prices",
			Shapes = "Shapes",
			SideBySideReports = "Employee Comparison",
			CalculatedFieldsReport = "Calculated Fields",
			FormattingRulesReport = "Conditional Formatting",
		    HugeAmountRecords = "100000 Records";
	}
}
