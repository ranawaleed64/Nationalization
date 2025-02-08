using System;
namespace Nationalization
{
    public class TableCreation
    {
        #region TableCreation
        public TableCreation()
        {
            try
            {
                //if (GlobalVariables.oCompany.UserName == "manager") {
                this.Nationalizations();
                //}

            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("Table Creation Failed: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
        }

        #region ...RFQ Status
        public void Nationalizations()
        {
            try
            {
                this.Nationalization();

                if (!GlobalVariables.oGFun.UDOExists("INT"))
                {
                    string[,] FindField = new string[,] { { "DocNum", "DocNum" }, { "CreateDate", "CreateDate" } };
                    GlobalVariables.oGFun.RegisterUDO("INT", "Internal Header", SAPbobsCOM.BoUDOObjType.boud_Document, FindField, "INTR_H", "INTR_D");
                    FindField = null;
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
            }
            finally
            {
            }
        }
        public void Nationalization()
        {
            try
            {
                #region ... Header

                GlobalVariables.oGFun.CreateTable("INTR_H", "Internal Header", SAPbobsCOM.BoUTBTableType.bott_Document);
                GlobalVariables.oGFun.CreateUserFields("@INTR_H", "PR", "Preious Reconcilation For", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@INTR_H", "GL_BP", "G/L Acct/BP Code From", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@INTR_H", "GL_B", "G/L Acct/BP Code From", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@INTR_H", "Branch", "Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@INTR_H", "TDate", "Till Date", SAPbobsCOM.BoFieldTypes.db_Date);

                #endregion


                #region ... Detail
                GlobalVariables.oGFun.CreateTable("INTR_D", "Internal Detail", SAPbobsCOM.BoUTBTableType.bott_DocumentLines);
                GlobalVariables.oGFun.CreateUserFields("@INTR_D", "Check", "Selected", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@INTR_D", "Origin", "Origin", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@INTR_D", "CardCode", "CardCode", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@INTR_D", "transId", "TransId", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@INTR_D", "transRow", "TransRowId", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@INTR_D", "OriginN", "Origin No", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@INTR_D", "PostD", "Posting Date", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@INTR_D", "Detail", "Detail", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@INTR_D", "AmountLC", "Amount(LC)", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Sum);
                GlobalVariables.oGFun.CreateUserFields("@INTR_D", "AmountSC", "Amount(SC)", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Sum);
                GlobalVariables.oGFun.CreateUserFields("@INTR_D", "AmontR", "Amount To Reconcile", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Sum);
                GlobalVariables.oGFun.CreateUserFields("@INTR_D", "ExRate", "Ex Rat", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Sum);
                GlobalVariables.oGFun.CreateUserFields("@INTR_D", "ExAmt", "Ex Rat Diff", SAPbobsCOM.BoFieldTypes.db_Float, 10, SAPbobsCOM.BoFldSubTypes.st_Sum);
                GlobalVariables.oGFun.CreateUserFields("@INTR_D", "Branch", "Branch", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);

            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
            }
        }

        #endregion


    }

    #endregion
    #endregion
}
