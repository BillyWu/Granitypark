using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace XtraReportsDemos
{
	public class MainFeaturesControl : XtraReportsDemos.ModuleControl {
		private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
		private RichTextBox rtbFeatures;
		private System.ComponentModel.IContainer components = null;

		public MainFeaturesControl()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.rtbFeatures = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtraTabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
            this.xtraTabControl1.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            this.xtraTabControl1.Size = new System.Drawing.Size(412, 273);
            this.xtraTabControl1.TabIndex = 0;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage1});
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Controls.Add(this.rtbFeatures);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(403, 264);
            this.xtraTabPage1.Text = "xtraTabPage1";
            // 
            // rtbFeatures
            // 
            this.rtbFeatures.BackColor = System.Drawing.Color.White;
            this.rtbFeatures.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbFeatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbFeatures.ForeColor = System.Drawing.Color.Black;
            this.rtbFeatures.Location = new System.Drawing.Point(0, 0);
            this.rtbFeatures.Name = "rtbFeatures";
            this.rtbFeatures.ReadOnly = true;
            this.rtbFeatures.Size = new System.Drawing.Size(403, 264);
            this.rtbFeatures.TabIndex = 1;
            this.rtbFeatures.Text = "";
            // 
            // MainFeaturesControl
            // 
            this.Controls.Add(this.xtraTabControl1);
            this.Name = "MainFeaturesControl";
            this.Size = new System.Drawing.Size(412, 273);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtraTabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion
		public override void Activate() {
			System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("XtraReportsDemos.BaseForms.ReportsAbout.rtf");
			rtbFeatures.LoadFile(stream,RichTextBoxStreamType.RichText);
		}
	}
}

