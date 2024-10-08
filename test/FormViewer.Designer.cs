﻿#if AUTOCAD2015_TO_2024
using Viewer = Sharper.AutoCAD.DrawableViewer.DrawableViewer;
#else
using Viewer = Sharper.GstarCAD.Extensions.DrawableViewer;
#endif

namespace DrawableViewer.Test
{
    public partial class FormViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.viewer1 = new Viewer();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // viewer1
            // 
            this.viewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.viewer1.Location = new System.Drawing.Point(12, 12);
            this.viewer1.Name = "viewer1";
            this.viewer1.Size = new System.Drawing.Size(635, 426);
            this.viewer1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(674, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(114, 43);
            this.button1.TabIndex = 1;
            this.button1.Text = "独立圆";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(674, 65);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(114, 43);
            this.button2.TabIndex = 2;
            this.button2.Text = "当前空间";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // TestWindowsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.viewer1);
            this.Name = "FormViewer";
            this.Text = "FormViewer";
            this.ResumeLayout(false);

        }

        #endregion

        private Viewer viewer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}