using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Duplicati.GUI
{
    /// <summary>
    /// This class wraps either a MWF NotifyIcon or a gtkSharp StatusIcon
    /// and exposes properties as if it was a MWF NotifyIcon
    /// </summary>
    class TrayIconWrapper
    {
        private NotifyIcon m_notifyIcon;
        private /*Gtk.StatusIcon*/ object m_statusIcon;

        private Control m_owner;

        System.Threading.Thread m_runner;

        private ContextMenuStrip m_menu;
        private Icon m_icon;
        private string m_text;
        private bool m_visible;

        public event MouseEventHandler MouseClick;
        public event MouseEventHandler MouseDoubleClick;

        private System.Reflection.Assembly m_gtkasm;
        private System.Reflection.Assembly m_gdkasm;

        /// <summary>
        /// Internal class used to pass arguments when setting properties on the GTK object
        /// </summary>
        private class SetPropertyEventArgs : EventArgs
        {
            private object m_owner;
            private string m_propertyName;
            private object m_value;

            public SetPropertyEventArgs(object owner, string propertyname, object value)
            {
                m_owner = owner;
                m_propertyName = propertyname;
                m_value = value;
            }

            /// <summary>
            /// Helper method to set a property on the GTK item in the correct thread context
            /// </summary>
            /// <param name="sender">Dummy sender object</param>
            /// <param name="a">The set parameter object</param>
            public void SetProperty(object sender, EventArgs a)
            {
                System.Reflection.PropertyInfo pi = m_owner.GetType().GetProperty(m_propertyName);
                pi.SetValue(m_owner, m_value, null);
            }
        }

        public TrayIconWrapper(Control owner)
        {
            m_owner = owner;
            //if (Duplicati.Library.Core.Utility.IsClientLinux)
            if(true)
            {
                try
                {
                    //These give deprecation warnings, because it can be tricky to do this,
                    // but in this special case, we actually want the newest gtk we can find.
                    m_gtkasm = System.Reflection.Assembly.LoadWithPartialName("gtk-sharp");
                    m_gdkasm = System.Reflection.Assembly.LoadWithPartialName("gdk-sharp");

                    if (m_gtkasm != null && m_gdkasm != null)
                    {
                        if (m_gtkasm.GetName().Version < new Version(2, 0))
                            throw new Exception("Too old gtk: " + m_gtkasm.GetName().Version.ToString());
                        if (m_gdkasm.GetName().Version < new Version(2, 0))
                            throw new Exception("Too old gdk: " + m_gtkasm.GetName().Version.ToString());

                        //Start a new thread so GTK can run its message pump
                        System.Threading.AutoResetEvent ev = new System.Threading.AutoResetEvent(false);
                        m_runner = new System.Threading.Thread(Thread_Run);
                        m_runner.Start(ev);

                        ev.WaitOne();
                        ev.Close();
                    }
                    else
                        throw new Exception("Unable to locate gtk assemblies");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    //We failed to load the gtk libraries, this should be logged
                }
            }

            if (m_statusIcon == null)
            {
                m_notifyIcon = new NotifyIcon();
                m_notifyIcon.MouseClick += new MouseEventHandler(m_notifyIcon_MouseClick);
                m_notifyIcon.MouseDoubleClick += new MouseEventHandler(m_notifyIcon_MouseDoubleClick);
            }
        }

        //Since GTK runs its own message pump, we run it in a separate thread
        void Thread_Run(object item)
        {
            //Gtk.Application.Init();
            Type app = m_gtkasm.GetType("Gtk.Application");
            app.GetMethod("Init", new Type[0]).Invoke(null, null);

            //m_statusIcon = new Gtk.StatusIcon();
            Type icon = m_gtkasm.GetType("Gtk.StatusIcon");
            m_statusIcon = Activator.CreateInstance(icon);

            //m_statusIcon.Visible = false;
            icon.GetProperty("Visible").SetValue(m_statusIcon, false, null);

            //m_statusIcon.Activate += new EventHandler(m_statusIcon_Activate);
            icon.GetEvent("Activate").AddEventHandler(m_statusIcon, new EventHandler(m_statusIcon_Activate));

            //m_statusIcon.PopupMenu += new Gtk.PopupMenuHandler(m_statusIcon_PopupMenu);
            Type popupHandler = icon.GetEvent("PopupMenu").EventHandlerType;
            System.Reflection.MethodInfo mi = this.GetType().GetMethod("m_statusIcon_PopupMenu", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            icon.GetEvent("PopupMenu").AddEventHandler(m_statusIcon, Delegate.CreateDelegate(popupHandler, this, mi));

            ((System.Threading.AutoResetEvent)item).Set();

            //Gtk.Application.Run();
            app.GetMethod("Run", new Type[0]).Invoke(null, null);
        }

        private void InvokeGtk(string propname, object value)
        {
            SetPropertyEventArgs args = new SetPropertyEventArgs(m_statusIcon, propname, value);
            //Gtk.Application.Invoke(this, args, new EventHandler(args.Execute));
            m_gtkasm.GetType("Gtk.Application")
                .GetMethod("Invoke", new Type[] { typeof(object), typeof(EventArgs), typeof(EventHandler) })
                .Invoke(null, new object[] { this, args, new EventHandler(args.SetProperty) });
        }

        private void InvokeQuit(object o, EventArgs a)
        {
            //Gtk.Application.Quit();
            m_gtkasm.GetType("Gtk.Application").GetMethod("Quit", new Type[0]).Invoke(null, null);
        }

        private void m_statusIcon_PopupMenu(object o, object args)
        {
            if (m_owner.InvokeRequired)
                m_owner.Invoke(new EventHandler(m_statusIcon_PopupMenu), o, args);
            else
            {
                //TODO: Remove this once it actually works
                Console.WriteLine("Popup menu is opening");
                if (m_menu != null)
                    m_menu.Show(Cursor.Position);
            }
        }

        void m_statusIcon_Activate(object sender, EventArgs e)
        {
            if (m_owner.InvokeRequired)
                m_owner.Invoke(new EventHandler(m_statusIcon_Activate), sender, e);
            else
            {
                //TODO: Remove this once it actually works
                Console.WriteLine("Icon was clicked");
                if (MouseClick != null)
                    MouseClick(this, new MouseEventArgs(MouseButtons.Left, 1, Cursor.Position.X, Cursor.Position.Y, 0));
            }
        }

        void m_notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (MouseDoubleClick != null)
                MouseDoubleClick(this, e);
        }

        void m_notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (MouseClick != null)
                MouseClick(this, e);
        }

        public Icon Icon
        {
            get
            {
                if (m_notifyIcon != null)
                    return m_notifyIcon.Icon;
                else
                    return m_icon;
            }
            set
            {
                if (m_notifyIcon != null)
                    m_notifyIcon.Icon = value;
                else
                {
                    m_icon = value;

                    using (Bitmap bmp = m_icon.ToBitmap())
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Position = 0;

                        //InvokeGtk("Pixbuf", new Gdk.Pixbuf(ms));
                        InvokeGtk("Pixbuf", Activator.CreateInstance(m_gdkasm.GetType("Gdk.Pixbuf"), ms));
                    }
                }
            }
        }


        public string Text
        {
            get
            {
                if (m_notifyIcon != null)
                    return m_notifyIcon.Text;
                else
                    return m_text;
            }
            set
            {
                if (m_notifyIcon != null)
                    m_notifyIcon.Text = value;
                else
                {
                    m_text = value;
                    
                    InvokeGtk("Tooltip", m_text);
                }
            }
        }

        public bool Visible
        {
            get
            {
                if (m_notifyIcon != null)
                    return m_notifyIcon.Visible;
                else
                    return m_visible;
            }
            set
            {
                if (m_notifyIcon != null)
                    m_notifyIcon.Visible = value;
                else
                {
                    m_visible = value;
                    InvokeGtk("Visible", m_visible);
                }
            }
        }

        public ContextMenuStrip ContextMenuStrip
        {
            get
            {
                if (m_notifyIcon != null)
                    return m_notifyIcon.ContextMenuStrip;
                else
                    return m_menu;
            }
            set
            {
                if (m_notifyIcon != null)
                    m_notifyIcon.ContextMenuStrip = value;
                else
                    m_menu = value;
            }
        }

        public void Exit()
        {
            //Gtk.Application.Invoke(this, null, new EventHandler(InvokeQuit));
            m_gtkasm.GetType("Gtk.Application")
                .GetMethod("Invoke", new Type[] { typeof(object), typeof(EventArgs), typeof(EventHandler) })
                .Invoke(null, new object[] { this, null, new EventHandler(InvokeQuit) });

            m_runner.Join();
        }
    }
}
