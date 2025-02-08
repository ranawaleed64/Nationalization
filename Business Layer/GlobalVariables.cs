namespace Nationalization
{

    /// <summary>
    /// GGlobally whatever variable do you want declare here 
    /// We can use any class and module from here  
    /// </summary>
    /// <remarks></remarks>
    static class GlobalVariables
    {

        #region " ... Common For SAP ..."
        public static SAPbobsCOM.Company oCompany;
        public static GlobalFunctions oGFun = new GlobalFunctions();
        public static SAPbouiCOM.Form oForm;

        #endregion

        #region " ... Common For Forms ..."
        public static string contractNo = "";

        //Cash In/Out
        public static string NationalizationID = "INT";
        public static string NationalizationXML = "Presentation_Layer.Masters.Inter.xml";
        public static Nationalization oNationalization = new Nationalization();



        #endregion

        #region " ... Gentral Purpose ..."
        public static long v_RetVal;
        public static int v_ErrCode;
        public static string v_ErrMsg = "";
        public static string addonName = "Inter Reconcilation";
        public static string sQuery = "";
        public static string BankFileName = "";
        public static string FileName = "";
        #endregion
    }
}
