using System;
using System.Windows.Forms;
namespace Nationalization
{


    /// <summary>
    /// This Application Sarted from here
    /// This is Used for to connect the SAP and dot net console
    /// And create menu
    /// 1)SetApplication
    ///  It is used to get connection to SAP.
    ///  Here we are using SAPbouiCOM.SboGuiApi, it is part of the SAP Business One Software Development Kit (SDK), and exposes user 
    ///  interface elements of the SAP Business One front end
    /// 2)SetFilter
    ///   Sets an EventFilter object that filters in events on specific forms
    /// 3)CookieConnect
    ///  It is represent the one of the Company Data base
    ///  It is enable to connect the company and Create the Business Object to use the company data base
    /// 4)ConnectionContext
    ///   It is used for to connect the company
    /// 5)TableCreation
    ///   It is used for to craete user tables and user define objects
    /// 6)SetEventFilter
    ///   User to filder the events for particilar forms
    ///   it is used to high performance 
    /// 7)AddXML
    ///   It is used to add memu XML 
    /// </summary>
    /// <remarks></remarks>
    static class MainM
    {

        #region "... Main ..."
        static void Main(string[] args)
        {
            try
            {
                GlobalVariables.oGFun.SetApplication();
                //1)
                string s = EventHandler.oApplication.Company.InstallationId.ToString();
                //if (s == "0020545074")
                //if (s == "0090461245")
                //{
                if (!(GlobalVariables.oGFun.CookieConnect() == 0))
                {
                    //3)
                    EventHandler.oApplication.MessageBox("DI Api Conection Failed");
                    System.Environment.Exit(0);
                }
                //4)
                if (!(GlobalVariables.oGFun.ConnectionContext() == 0))
                {
                    EventHandler.oApplication.MessageBox("Failed to Connect Company");

                    System.Environment.Exit(0);
                }
                //}
                //else {
                //    System.Windows.Forms.MessageBox.Show("Kindly Contact Vendor License of this Add-on...");
                //    System.Windows.Forms.Application.ExitThread();
                //    System.Environment.Exit(0);
                //}
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Application Not Found", "Inter reconcilation Add-on" + ex.Message);
                System.Windows.Forms.Application.ExitThread();
            }
            finally
            {
            }
            try
            {

                try
                {
                    TableCreation oTableCreation = new TableCreation();
                    //5)              
                    EventHandler.SetEventFilter();
                    //6)

                    GlobalVariables.oGFun.AddXML("Presentation_Layer.Menu.xml");
                    //7)
                    SAPbouiCOM.MenuItem MenuItem = EventHandler.oApplication.Menus.Item("Nationalization");
                    if (MenuItem.Enabled == true)
                    {
                        MenuItem.Enabled = false;
                    }
                    //MenuItem.Image = Application.StartupPath + "\\Chrisma-Addon.bmp";
                    MenuItem.Checked = true;
                    MenuItem.Enabled = true;

                }
                catch (Exception ex)
                {
                   // System.Windows.Forms.MessageBox.Show(ex.Message);
                    System.Windows.Forms.Application.ExitThread();
                }
                finally
                {
                }

                EventHandler.oApplication.StatusBar.SetText("Connected.......", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                System.Windows.Forms.Application.Run();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(GlobalVariables.addonName + " Main Method Failed : " + ex.Message);
                System.Windows.Forms.Application.ExitThread();

            }
            finally
            {
            }
        }
        #endregion
    }
}
