﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

class TransparentTabControl : TabControl
{
    private List<Panel> pages = new List<Panel>();

    public void MakeTransparent()
    {
        if (TabCount == 0) throw new InvalidOperationException();
        var height = GetTabRect(0).Bottom;
        // Move controls to panels
        for (int tab = 0; tab < TabCount; ++tab)
        {
            var page = new Panel
            {
                Left = this.Left,
                Top = this.Top + height,
                Width = this.Width,
                Height = this.Height - height,
                BackColor = Color.Transparent,
                Visible = tab == this.SelectedIndex
            };
            for (int ix = TabPages[tab].Controls.Count - 1; ix >= 0; --ix)
            {
                TabPages[tab].Controls[ix].Parent = page;
            }
            pages.Add(page);
            this.Parent.Controls.Add(page);
        }
        this.Height = height /* + 1 */;
    }

    // Bug in this code, maybe related to binding to combobox that they do not like. 
    protected override void OnSelectedIndexChanged(EventArgs e)
    {
        try
        {
            base.OnSelectedIndexChanged(e);
            for (int tab = 0; tab < pages.Count; ++tab)
            {
                pages[tab].Visible = tab == SelectedIndex;
            }
        }
        catch //###Fix Bug later. 
        {
           
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing) foreach (var page in pages) page.Dispose();
        base.Dispose(disposing);
    }
}