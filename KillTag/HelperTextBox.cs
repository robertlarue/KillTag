using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ThingMagic.URA
{
    class HelperTextBox : System.Windows.Forms.TextBox
    {
	private bool _helperVisible;

        [Description("Description of expected entry values")]
        [Category("Appearance")]
        public string HelperText
	{
	    get { return _helperText; }
	    set
	    {
		_helperText = value;
		if (_helperVisible)
		{
		    Text = HelperText;
		}
	    }
	}
	private string _helperText;
        private string NormalText;

        [Description("Foreground color of HelperText")]
        [Category("Appearance")]
        public System.Drawing.Color HelperBackColor;
        private System.Drawing.Color NormalBackColor;

        [Description("Background color of HelperText")]
        [Category("Appearance")]
        public System.Drawing.Color HelperForeColor;
        private System.Drawing.Color NormalForeColor;

        public HelperTextBox()
        {
            SaveNormals();
            HelperText = "";
            HelperBackColor = NormalBackColor;
            HelperForeColor = System.Drawing.SystemColors.GrayText;
            ShowHelpers();
        }

        private void SaveNormals()
        {
            NormalText = Text;
            NormalBackColor = BackColor;
            NormalForeColor = ForeColor;
        }

        private void ShowHelpers()
        {
            Text = HelperText;
            BackColor = HelperBackColor;
            ForeColor = HelperForeColor;
	    _helperVisible = true;
        }

        private void ShowNormals()
        {
            Text = NormalText;
            BackColor = NormalBackColor;
            ForeColor = NormalForeColor;
	    _helperVisible = false;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            SaveNormals();
            if ("" == NormalText)
            {
                ShowHelpers();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            ShowNormals();
            base.OnGotFocus(e);
        }
    }
}
