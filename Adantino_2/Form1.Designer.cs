namespace Adantino_2
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.undo_button = new System.Windows.Forms.Button();
            this.restart_button = new System.Windows.Forms.Button();
            this.turn_label = new System.Windows.Forms.Label();
            this.move_label = new System.Windows.Forms.Label();
            this.ai_depth_label = new System.Windows.Forms.Label();
            this.render_label = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ai_ct_label = new System.Windows.Forms.Label();
            this.ai_lt_label = new System.Windows.Forms.Label();
            this.kill_ai_button = new System.Windows.Forms.Button();
            this.start_ai_button = new System.Windows.Forms.Button();
            this.blackAI_box = new System.Windows.Forms.CheckBox();
            this.redAI_box = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // undo_button
            // 
            this.undo_button.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.undo_button.Location = new System.Drawing.Point(1157, 12);
            this.undo_button.Name = "undo_button";
            this.undo_button.Size = new System.Drawing.Size(115, 58);
            this.undo_button.TabIndex = 0;
            this.undo_button.Text = "Undo";
            this.undo_button.UseVisualStyleBackColor = true;
            this.undo_button.Click += new System.EventHandler(this.undo_button_Click);
            // 
            // restart_button
            // 
            this.restart_button.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.restart_button.Location = new System.Drawing.Point(1157, 891);
            this.restart_button.Name = "restart_button";
            this.restart_button.Size = new System.Drawing.Size(115, 58);
            this.restart_button.TabIndex = 3;
            this.restart_button.Text = "Restart";
            this.restart_button.UseVisualStyleBackColor = true;
            this.restart_button.Click += new System.EventHandler(this.restart_button_Click);
            // 
            // turn_label
            // 
            this.turn_label.AutoSize = true;
            this.turn_label.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.turn_label.ForeColor = System.Drawing.Color.Red;
            this.turn_label.Location = new System.Drawing.Point(414, 8);
            this.turn_label.Name = "turn_label";
            this.turn_label.Size = new System.Drawing.Size(153, 36);
            this.turn_label.TabIndex = 2;
            this.turn_label.Text = "Reds turn";
            // 
            // move_label
            // 
            this.move_label.AutoSize = true;
            this.move_label.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.move_label.Location = new System.Drawing.Point(901, 8);
            this.move_label.Name = "move_label";
            this.move_label.Size = new System.Drawing.Size(86, 24);
            this.move_label.TabIndex = 6;
            this.move_label.Text = "Move: 0";
            // 
            // ai_depth_label
            // 
            this.ai_depth_label.AutoSize = true;
            this.ai_depth_label.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ai_depth_label.Location = new System.Drawing.Point(48, 56);
            this.ai_depth_label.Name = "ai_depth_label";
            this.ai_depth_label.Size = new System.Drawing.Size(110, 24);
            this.ai_depth_label.TabIndex = 7;
            this.ai_depth_label.Text = "AI depth: 0";
            // 
            // render_label
            // 
            this.render_label.AutoSize = true;
            this.render_label.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.render_label.Location = new System.Drawing.Point(2, 936);
            this.render_label.Name = "render_label";
            this.render_label.Size = new System.Drawing.Size(125, 16);
            this.render_label.TabIndex = 8;
            this.render_label.Text = "Rendertime: 000 ms";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ai_ct_label);
            this.panel1.Controls.Add(this.move_label);
            this.panel1.Controls.Add(this.ai_lt_label);
            this.panel1.Controls.Add(this.ai_depth_label);
            this.panel1.Controls.Add(this.turn_label);
            this.panel1.Location = new System.Drawing.Point(142, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1000, 1000);
            this.panel1.TabIndex = 1;
            // 
            // ai_ct_label
            // 
            this.ai_ct_label.AutoSize = true;
            this.ai_ct_label.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ai_ct_label.Location = new System.Drawing.Point(14, 8);
            this.ai_ct_label.Name = "ai_ct_label";
            this.ai_ct_label.Size = new System.Drawing.Size(161, 24);
            this.ai_ct_label.TabIndex = 10;
            this.ai_ct_label.Text = "current time: 0 s";
            // 
            // ai_lt_label
            // 
            this.ai_lt_label.AutoSize = true;
            this.ai_lt_label.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ai_lt_label.Location = new System.Drawing.Point(14, 32);
            this.ai_lt_label.Name = "ai_lt_label";
            this.ai_lt_label.Size = new System.Drawing.Size(161, 24);
            this.ai_lt_label.TabIndex = 9;
            this.ai_lt_label.Text = "Last AI time: 0 s";
            // 
            // kill_ai_button
            // 
            this.kill_ai_button.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kill_ai_button.Location = new System.Drawing.Point(12, 88);
            this.kill_ai_button.Name = "kill_ai_button";
            this.kill_ai_button.Size = new System.Drawing.Size(115, 58);
            this.kill_ai_button.TabIndex = 9;
            this.kill_ai_button.Text = "Kill AI";
            this.kill_ai_button.UseVisualStyleBackColor = true;
            this.kill_ai_button.Click += new System.EventHandler(this.kill_ai_button_Click);
            // 
            // start_ai_button
            // 
            this.start_ai_button.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.start_ai_button.Location = new System.Drawing.Point(12, 12);
            this.start_ai_button.Name = "start_ai_button";
            this.start_ai_button.Size = new System.Drawing.Size(115, 58);
            this.start_ai_button.TabIndex = 10;
            this.start_ai_button.Text = "Start AI";
            this.start_ai_button.UseVisualStyleBackColor = true;
            this.start_ai_button.Click += new System.EventHandler(this.start_ai_button_Click);
            // 
            // blackAI_box
            // 
            this.blackAI_box.AutoSize = true;
            this.blackAI_box.Checked = true;
            this.blackAI_box.CheckState = System.Windows.Forms.CheckState.Checked;
            this.blackAI_box.Location = new System.Drawing.Point(1157, 88);
            this.blackAI_box.Name = "blackAI_box";
            this.blackAI_box.Size = new System.Drawing.Size(69, 17);
            this.blackAI_box.TabIndex = 11;
            this.blackAI_box.Text = "Black AI ";
            this.blackAI_box.UseVisualStyleBackColor = true;
            this.blackAI_box.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // redAI_box
            // 
            this.redAI_box.AutoSize = true;
            this.redAI_box.Checked = true;
            this.redAI_box.CheckState = System.Windows.Forms.CheckState.Checked;
            this.redAI_box.Location = new System.Drawing.Point(1157, 112);
            this.redAI_box.Name = "redAI_box";
            this.redAI_box.Size = new System.Drawing.Size(59, 17);
            this.redAI_box.TabIndex = 12;
            this.redAI_box.Text = "Red AI";
            this.redAI_box.UseVisualStyleBackColor = true;
            this.redAI_box.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 961);
            this.Controls.Add(this.redAI_box);
            this.Controls.Add(this.blackAI_box);
            this.Controls.Add(this.start_ai_button);
            this.Controls.Add(this.kill_ai_button);
            this.Controls.Add(this.render_label);
            this.Controls.Add(this.restart_button);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.undo_button);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button undo_button;
        private System.Windows.Forms.Button restart_button;
        private System.Windows.Forms.Label turn_label;
        private System.Windows.Forms.Label move_label;
        private System.Windows.Forms.Label ai_depth_label;
        private System.Windows.Forms.Label render_label;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label ai_lt_label;
        private System.Windows.Forms.Label ai_ct_label;
        private System.Windows.Forms.Button kill_ai_button;
        private System.Windows.Forms.Button start_ai_button;
        private System.Windows.Forms.CheckBox blackAI_box;
        private System.Windows.Forms.CheckBox redAI_box;
    }
}

