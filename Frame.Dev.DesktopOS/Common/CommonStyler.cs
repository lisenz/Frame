using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
//-----------------
using DevExpress.Utils;
using DevExpress.XtraTab;
using DevExpress.Tutorials;
using DevExpress.XtraEditors;

namespace Frame.Dev.DesktopOS.Common
{
    /// <summary>
    /// 基础控件通用样式方法类，提供设置基础窗体控件和基础模块控件的控件样式的一系列方法和属性。
    /// </summary>
    internal class CommonStyler
    {
        #region 属性

        /// <summary>
        /// 样式类对象。
        /// </summary>
        internal static CommonStyler Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new CommonStyler();
                }
                return _Instance;
            }
        }

        #endregion

        #region 字段

        /// <summary>
        /// 样式类对象。
        /// </summary>
        private static CommonStyler _Instance = null;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        private CommonStyler()
        {
        }

        #endregion

        #region 方法

        /// <summary>
        /// 设置基础窗体控件或基础模块控件中按钮的样式。
        /// </summary>
        /// <param name="fControls">基窗体或基控件对象。</param>
        internal void InitializeButtonStyle(Control fControls)
        {
            try
            {
                foreach (Control control in fControls.Controls)
                {
                    GroupControl xgb = null;
                    PanelControl xpl = null;
                    XtraTabControl xtc = null;
                    GroupBox gb = null;
                    Panel pl = null;
                    TabControl tc = null;
                    if (control is GroupBox)
                    {
                        gb = control as GroupBox;
                        InitializeButtonStyle(gb);
                    }
                    if (control is Panel)
                    {
                        pl = control as Panel;
                        InitializeButtonStyle(pl);
                    }
                    if (control is TabControl)
                    {
                        tc = control as TabControl;
                        InitializeButtonStyle(tc);
                    }
                    if (control is GroupControl)
                    {
                        xgb = control as GroupControl;
                        InitializeButtonStyle(xgb);
                    }
                    if (control is PanelControl)
                    {
                        xpl = control as PanelControl;
                        InitializeButtonStyle(xpl);
                    }
                    if (control is XtraTabControl)
                    {
                        xtc = control as XtraTabControl;
                        foreach (XtraTabPage tp in xtc.TabPages)
                        {
                            InitializeButtonStyle(tp);
                        }
                    }

                    foreach (Control dbControl in fControls.Controls)
                    {
                        if (dbControl is Button)
                        {
                            Button btn = dbControl as Button;
                            if (btn.Tag != null)
                            {
                                try
                                {
                                    btn.Height = 23;
                                    btn.ImageAlign = ContentAlignment.MiddleLeft;
                                    btn.TextAlign = ContentAlignment.MiddleRight;
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(string.Format("模块内控件[{0}]样式定义不正确，请修改!附:{1}.", btn.Text, ex.Message), ex);
                                }
                            }
                        }
                        if (dbControl is SimpleButton)
                        {
                            SimpleButton btn = dbControl as SimpleButton;
                            if (btn.Tag != null)
                            {
                                try
                                {
                                    btn.Image = (Image)btn.Tag;
                                    btn.Height = 23;
                                    btn.Appearance.Options.UseImage = true;
                                    btn.Appearance.Options.UseTextOptions = true;
                                    btn.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
                                    btn.Appearance.TextOptions.VAlignment = VertAlignment.Center;
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(string.Format("模块内控件[{0}]样式定义不正确，请修改!附:{1}.", btn.Text, ex.Message), ex);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 设置基础窗体控件或基础模块控件中GridView控件行的样式。
        /// </summary>
        /// <param name="fControls">基窗体或基控件对象。</param>
        internal void InitializeGridRowStyle(Control fControls)
        {
        }

        #endregion
    }
}
