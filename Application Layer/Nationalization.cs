using Nationalization.List;
using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
namespace Nationalization
{
    internal class Nationalization
    {
        #region Variables
        SAPbouiCOM.Form frmNationalization;
        SAPbouiCOM.DBDataSource oDBDSHeader;
        SAPbouiCOM.DBDataSource oDBDetail;
        SAPbouiCOM.Matrix oMatrix;
        SAPbouiCOM.ComboBox typeCombo;
        SAPbouiCOM.EditText CodeEdit;
        string GLCode;
        //SAPbouiCOM.LinkedButton BP, GL;
        SAPbouiCOM.LinkedButton BP, GL;
        List<Data> Data = new List<Data>();
        public SAPbobsCOM.Company oTargetComp = new SAPbobsCOM.Company();
        string oCountry = "", oCashAccount = "", oCMAccount = "", oQuery = "", oSeriesInvoice = "", oSeriesCreditMemo = "", oSeriesPayment = "", oSeriesJE = "", oSeriesJOutgoing = "";
        double oLeaveAmount = 0;
        Boolean oPaymentFlag = false;
        #endregion

        #region LoadForm
        public void LoadNationalization()
        {
            try
            {

                GlobalVariables.oGFun.LoadXML(frmNationalization, GlobalVariables.NationalizationID, GlobalVariables.NationalizationXML);
                frmNationalization = EventHandler.oApplication.Forms.Item(GlobalVariables.NationalizationID);
                oDBDSHeader = frmNationalization.DataSources.DBDataSources.Item("@INTR_H");
                oDBDetail = frmNationalization.DataSources.DBDataSources.Item("@INTR_D");
                oMatrix = (Matrix)frmNationalization.Items.Item("24").Specific;
                frmNationalization.EnableMenu("1292", true);
                frmNationalization.EnableMenu("1293", true);
                InternalReconcellation();
                this.InitForm();
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("Load : " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
        }
        public void InitForm()
        {
            try
            {
                frmNationalization.Freeze(true);
                int Doc = GlobalVariables.oGFun.GetCodeGeneration("@INTR_H");
                oDBDSHeader.SetValue("DocNum", 0, Doc.ToString());
                oDBDSHeader.SetValue("CreateDate", 0, GlobalVariables.oGFun.GetServerDate());
                typeCombo = (ComboBox)frmNationalization.Items.Item("1000009").Specific;
                CodeEdit=frmNationalization.Items.Item("t_cc1").Specific;
                BP = frmNationalization.Items.Item("t_bp").Specific;

              
                string oQm = String.Format("SELECT T0.\"BPLId\", T0.\"BPLName\" FROM \"OBPL\" T0");
                GlobalVariables.oGFun.setComboBoxBranches(frmNationalization.Items.Item("16").Specific, oQm);
                frmNationalization.Freeze(false);

            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
                frmNationalization.Freeze(false);
            }
            finally
            {
            }
        }
        #endregion

        #region ItemEvent
        public void ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                switch (pVal.EventType)
                {
                    #region Choose_from_list
                    case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST:
                        try
                        {
                            SAPbouiCOM.DataTable oDataTable = null;
                            SAPbouiCOM.ChooseFromListEvent oCFLE = (SAPbouiCOM.ChooseFromListEvent)pVal;
                            oDataTable = oCFLE.SelectedObjects;
                            if ((oDataTable != null) & pVal.BeforeAction == false)
                            {
                                switch (pVal.ItemUID)
                                {
                                    case "t_cc1":
                                        if (pVal.BeforeAction == false && int.Parse(typeCombo.Selected.Value) == 1)
                                        {
                                           
                                            oDBDSHeader.SetValue("U_GL_BP", 0, (string)oDataTable.GetValue("CardCode", 0));
                                            oDBDSHeader.SetValue("U_GL_B", 0, (string)oDataTable.GetValue("CardCode", 0));
                                          
                                        }
                                        else
                                        {
                                             GLCode = (string)oDataTable.GetValue("FormatCode", 0);
                                            oDBDSHeader.SetValue("U_GL_B", 0, (string)oDataTable.GetValue("AcctCode", 0));
                                            oDBDSHeader.SetValue("U_GL_BP", 0, (string)oDataTable.GetValue("FormatCode", 0));
                                        
                                        }
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            EventHandler.oApplication.StatusBar.SetText("Error: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                        }
                        finally
                        {
                        }

                        break;
                    #endregion
                
                    case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT:
                        try
                        {
                            switch (pVal.ItemUID)
                            {


                                case "1000009":
                                    frmNationalization = EventHandler.oApplication.Forms.Item(GlobalVariables.NationalizationID);
                                    if (pVal.BeforeAction == false)
                                    {
                                        frmNationalization.Freeze(true);
                                        SAPbouiCOM.Item linkButtonItem = frmNationalization.Items.Item("t_bp");
                                        SAPbouiCOM.LinkedButton myLinkButton = (SAPbouiCOM.LinkedButton)linkButtonItem.Specific;
                                        if (int.Parse(typeCombo.Selected.Value) == 1)
                                        {
                                            myLinkButton.LinkedObject = BoLinkedObject.lf_BusinessPartner;
                                            Customer(frmNationalization, typeCombo.Selected.Value, CodeEdit);                                          
                                                                                                                           
                                        }
                                        else if (int.Parse(typeCombo.Selected.Value) == 2)
                                        {
                                            Account(frmNationalization, typeCombo.Selected.Value, CodeEdit);
                                        }
                                        frmNationalization.Freeze(false);
                                    }
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            EventHandler.oApplication.MessageBox(ex.Message);
                            frmNationalization.Freeze(false);
                        }
                        break;
                    case SAPbouiCOM.BoEventTypes.et_LOST_FOCUS:
                        try
                        {
                            switch (pVal.ItemUID)
                            {
                                case "24":
                                    {
                                        if (pVal.BeforeAction == false && (pVal.ColUID == "V_4"))
                                        {
                                            GlobalVariables.oGFun.SetNewLine(oMatrix, oDBDetail, pVal.Row, "V_4");

                                        }
                                    }
                                    break;
                            }
                            break;

                        }
                        catch (Exception ex)
                        {
                            EventHandler.oApplication.MessageBox(ex.Message);
                            frmNationalization.Freeze(false);
                        }
                        finally
                        {
                        }

                        break;


                    case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:
                        try
                        {
                            switch (pVal.ItemUID)
                            {
                                case "b_fetch":
                                    if (pVal.BeforeAction == false)
                                    {
                                        oMatrix.Clear();
                                        oDBDetail.Clear();
                                        Data.Clear();
                                        SAPbobsCOM.Recordset rs = (Recordset)GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                        string oAccCode = oDBDSHeader.GetValue("U_GL_B", 0);
                                        string oTaxDate = oDBDSHeader.GetValue("U_TDate", 0);
                                        string oBranch = oDBDSHeader.GetValue("U_Branch", 0);
                                        string branchCondition = string.IsNullOrEmpty(oBranch) ? "" : "and T0.BPLId = " + oBranch;

                                        string query = "SELECT * FROM (SELECT T5.BPLName, CASE WHEN T0.\"TransType\" = 13 THEN 'A/r Invoice' " +
                                                       "WHEN T0.\"TransType\" = 14 THEN 'A/r Credit Memo' WHEN T0.\"TransType\" = '-2' THEN 'Opening Balance' WHEN T0.\"TransType\" = 30 THEN 'Journal Entry' " +
                                                       "WHEN T0.\"TransType\" = 24 THEN 'IncommingPayment' WHEN T0.\"TransType\" = 46 THEN 'OutgoingPayment' " +
                                                       "WHEN T0.\"TransType\" = 203 THEN 'DownPayment' WHEN T0.\"TransType\" = 18 THEN 'A/p Invoice' " +
                                                       "WHEN T0.\"TransType\" = 19 THEN 'A/p Credit Memo' WHEN T0.\"TransType\" = 46 THEN 'Outgoing Payment' " +
                                                       "WHEN T0.\"TransType\" = 204 THEN 'A/p DownPayment' WHEN T0.\"TransType\" = 30 THEN 'Journal Entry' " +
                                                       "WHEN T0.\"TransType\" = 24 THEN 'IncommingPayment' END as Origin,T0.\"TransType\", CASE " +
                                                       "WHEN T0.\"BalScDeb\" <> 0 THEN 'Debit' WHEN T0.\"BalScCred\" <> 0 THEN  'Credit' ELSE '' END as Type, T0.\"BaseRef\" as OriginNo,DATEPART(month, T0.TaxDate) as TransactionMonth,DATEPART(year, T0.TaxDate) as TransactionYear, T0.\"TaxDate\", CASE WHEN T0.\"BalScDeb\" <> 0 THEN T0.\"BalScDeb\" " +
                                                       "WHEN T0.\"BalScCred\" <> 0 THEN T0.\"BalScCred\" ELSE 0 END as AmountinSC, CASE WHEN T0.BalDueDeb <> 0 THEN T0.BalDueDeb " +
                                                       "WHEN T0.BalDueCred <> 0 THEN T0.BalDueCred ELSE 0 END as AmountinLC, \"LineMemo\", T0.\"TransId\", T0.\"Line_ID\", \"CardCode\" FROM JDT1 T0 LEFT JOIN OCRD T1 ON T1.\"CardCode\"=T0.\"ShortName\" LEFT JOIN OBPL T5 ON T0.\"BPLId\"=T5.\"BPLId\"" +
                                                       "WHERE  T0.\"TaxDate\" <= '" + oTaxDate + "' and (T1.\"CardCode\" = '" + oAccCode + "' or T0.\"Account\" = '" + oAccCode + "') "+ branchCondition +" )TB1 WHERE AmountinLC <> 0 ORDER BY \"TaxDate\"";

                                        rs.DoQuery(query);
                                        if (!rs.EoF)
                                        {
                                            int rowIndex = 0;
                                            while (!rs.EoF)
                                            {
                                                try
                                                {
                                                    var originNo = rs.Fields.Item("OriginNo").Value;
                                                    var origin = rs.Fields.Item("Origin").Value;
                                                    var transType = rs.Fields.Item("TransType").Value;
                                                    var actualAmount = Convert.ToDouble(rs.Fields.Item("AmountinLC").Value);
                                                    var amountSc = Math.Abs(Convert.ToDouble(rs.Fields.Item("AmountinSC").Value));
                                                    var amountLc = Math.Abs(Convert.ToDouble(rs.Fields.Item("AmountinLC").Value));
                                                    var transId = rs.Fields.Item("TransId").Value;
                                                    var transRowId = rs.Fields.Item("Line_ID").Value;
                                                    var type = rs.Fields.Item("Type").Value;
                                                    var month = rs.Fields.Item("TransactionMonth").Value;
                                                    var year = rs.Fields.Item("TransactionYear").Value;
                                                    var Date = rs.Fields.Item("TaxDate").Value;
                                                    var brnch = rs.Fields.Item("BPLName").Value;
                                                    var TransactionType = "";
                                                    if (transType == "14" && actualAmount < 0 || transType == "19" && actualAmount < 0)
                                                    {
                                                        if(type == "Debit")
                                                        {
                                                            TransactionType = "Credit";
                                                        }
                                                        else
                                                        {
                                                            TransactionType = "Debit";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        TransactionType = type;
                                                    }
                                                     
                                                    var exRate = Math.Round(amountLc / amountSc, 2);

                                                    Data.Add(new Data
                                                    {
                                                        OriginNo = originNo,
                                                        Origin = origin,
                                                        TransType = transType,
                                                        AmountSc = amountSc,
                                                        AmountLc = amountLc,
                                                        exRate = exRate,
                                                        TransId = transId,
                                                        TransRowId = transRowId,
                                                        Type = TransactionType,
                                                        ActualAmount = actualAmount,
                                                        Year = year,
                                                        Month = month,
                                                        Date = Date,
                                                        Branch = brnch
                                                    });

                                                    GlobalVariables.oGFun.SetNewLine(oMatrix, oDBDetail);
                                                    oDBDetail.SetValue("U_Branch", rowIndex, brnch.ToString());
                                                    oDBDetail.SetValue("U_PostD", rowIndex, rs.Fields.Item("TaxDate").Value);
                                                    oDBDetail.SetValue("U_transId", rowIndex, transId);
                                                    oDBDetail.SetValue("U_OriginN", rowIndex, originNo);
                                                    oDBDetail.SetValue("U_Origin", rowIndex, origin);
                                                    oDBDetail.SetValue("U_CardCode", rowIndex, rs.Fields.Item("CardCode").Value);
                                                    oDBDetail.SetValue("U_AmountLC", rowIndex, amountLc.ToString().Trim());
                                                    oDBDetail.SetValue("U_AmountSC", rowIndex, amountSc.ToString("F2").Trim());
                                                    oDBDetail.SetValue("U_Detail", rowIndex, rs.Fields.Item("LineMemo").Value);


                                                    rowIndex++;
                                                    rs.MoveNext();
                                                }
                                                catch (Exception ex)
                                                {
                                                    EventHandler.oApplication.MessageBox(ex.Message);
                                                }
                                            }
                                            oMatrix.LoadFromDataSource();
                                        }
                                    }
                                    break;
                                case "29":
                                    if (pVal.BeforeAction == false)
                                    {
                                        EventHandler.oApplication.StatusBar.SetText("Processing Started ...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                        Thread.Sleep(3000);
                                        string oAccountType = oDBDSHeader.GetValue("U_PR", 0);
                                        List<int> Years = Data.Select(payment => payment.Year).Distinct().ToList();
                                        List<string> Branches = Data.Select(x => x.Branch).Distinct().ToList();
                                        List<int> Months = new List<int>();
                                        for (int i = 1; i <= 12; i++)
                                        {
                                            Months.Add(i);
                                        }

                                        var payments = Data.Where(item => item.Type == "Credit")
                                                           .Select(item => new Payment
                                                           {
                                                               line = Convert.ToInt32(item.OriginNo),
                                                               paymentLC = Math.Round(item.AmountLc, 4),
                                                               paymentSC = item.AmountSc,
                                                               exRate = item.exRate,
                                                               TransId = item.TransId,
                                                               TransRowId = item.TransRowId,
                                                               TransType = item.TransType,
                                                               ActualAmount = item.ActualAmount,
                                                               Month = item.Month,
                                                               Year = item.Year,
                                                               Date = item.Date,
                                                               Branch = item.Branch
                                                           })
                                                           .OrderBy(payment => payment.Date)
                                                           .ToList();

                                        var invoices = Data.Where(item => item.Type != "Credit")
                                                           .Select(item => new Invoices
                                                           {
                                                               line = Convert.ToInt32(item.TransId),
                                                               invoiceLC = Math.Round(item.AmountLc, 4),
                                                               invoiceSC = item.AmountSc,
                                                               exRate = item.exRate,
                                                               TransId = item.TransId,
                                                               TransRowId = item.TransRowId,
                                                               TransType = item.TransType,
                                                               ActualAmount = item.ActualAmount,
                                                               Month = item.Month,
                                                               Year = item.Year,
                                                               Date = item.Date,
                                                               Branch = item.Branch
                                                           })
                                                           .OrderBy(invoice => invoice.Date)
                                                           .ToList();




                                       
                                        Task.Run(async () =>
                                        {
                                            int totalIterations = Years.Count * Months.Count;
                                            List<Task> allTasks = new List<Task>();
                                        
                                            int currentProgress = 0;

                                            foreach (var branch in Branches)
                                            {
                                                foreach (var year in Years)
                                                {
                                                    foreach (var month in Months)
                                                    {
                                                        var monthlyPayments = payments.Where(p => p.Year <= year && p.Month <= month && p.Branch == branch && p.paymentLC > 0).ToList();
                                                        var monthlyInvoices = invoices.Where(i => i.Year <= year && i.Month <= month && i.Branch == branch && i.invoiceLC > 0).ToList();
                                                        if (monthlyPayments.Count > 0 && monthlyInvoices.Count > 0)
                                                        {
                                                            doReconcilationProcess(monthlyPayments, monthlyInvoices, oAccountType, allTasks);
                                                        }
                                                        UpdateProgressBar(++currentProgress, totalIterations);
                                                    }
                                                }
                                            }

                                            await Task.WhenAll(allTasks);
                                            EventHandler.oApplication.StatusBar.SetText("Processing complete", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                        });

                                        //foreach (var year in Years)
                                        //{
                                        //    foreach (var month in Months)
                                        //    {
                                        //        var monthlyPayments = payments.Where(p => p.Year <= year && p.Month <= month && p.paymentLC > 0).ToList();
                                        //        var monthlyInvoices = invoices.Where(i => i.Year <= year && i.Month <= month && i.invoiceLC > 0).ToList();
                                        //        if(monthlyPayments.Count > 0 && monthlyInvoices.Count > 0)
                                        //        {
                                        //            doReconcilationProcess(monthlyPayments, monthlyInvoices, oGainAccount, oLossAccount, oAccountType);
                                        //        }


                                        //    }
                                        //}

                                    }
                                    break;
                                case "t_bp":
                                    if  (int.Parse(typeCombo.Selected.Value) == 2)
                                        {
                                        BubbleEvent = false;
                                        SAPbobsCOM.Recordset Record = (Recordset)GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                        string Query = "Select T0.\"AcctCode\",T0.\"FormatCode\" FROM OACT T0 WHERE T0.\"FormatCode\" = '" + GLCode+"'";
                                        Record.DoQuery(Query);
                                        string Account = Record.Fields.Item("AcctCode").Value;
                                        EventHandler.oApplication.OpenForm(BoFormObjectEnum.fo_GLAccounts, "", Account);
                                    }
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            EventHandler.oApplication.MessageBox(ex.Message);
                        }
                        break;
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
        void UpdateProgressBar(int value, int total)
        {
            if (EventHandler.oApplication != null)
            {
                string message = $"Processing {value} of {total}";
                EventHandler.oApplication.StatusBar.SetText(message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_None);
            }
        }

        private void doReconcilationProcess(List<Payment> payments,List<Invoices> invoices, string oAccountType, List<Task> allTasks)
        {
            double Diffierence = 0.0;
            double TotalPayments = 0.0;
            double TotalInvoices = 0.0;
            double TotaExRate = 0.0;
            List<Reconcilation> reconcilation = new List<Reconcilation>();
            foreach (var item in invoices)
            {
                while (item.invoiceLC > 0)
                {
                    var TopPayment = payments.Where(pay => pay.paymentLC > 0)
                                             .OrderBy(pay => pay.line)
                                             .FirstOrDefault();
                    if (TopPayment == null) break;
                    Diffierence = TopPayment.paymentLC - item.invoiceLC;
                    if (Diffierence > 0)
                    {
                        TotalInvoices += item.invoiceSC;
                        if (TopPayment.exRate != 0 && item.invoiceLC != 0)
                        {
                            TotalPayments += Math.Round(item.invoiceLC / TopPayment.exRate, 10);
                        }
                        else
                        {
                            TotalPayments += Math.Round(item.invoiceSC, 10);
                        }

                        UpdateOrAddReconciliationEntry(item.TransId, item.TransRowId, item.invoiceLC, item.TransType, item.ActualAmount, reconcilation);
                        UpdateOrAddReconciliationEntry(TopPayment.TransId, TopPayment.TransRowId, item.invoiceLC, TopPayment.TransType, TopPayment.ActualAmount, reconcilation);

                        if (TopPayment.exRate != 0 && item.invoiceLC != 0)
                        {
                            TopPayment.paymentSC -= Math.Round(item.invoiceLC / TopPayment.exRate, 10);
                        }
                        else
                        {
                            TopPayment.paymentSC -= Math.Round(item.invoiceSC, 10);
                        }

                        item.invoiceLC = 0;
                        TopPayment.paymentLC = Diffierence;

                    }

                    else if (Diffierence == 0)
                    {
                        TotalInvoices += item.invoiceSC;
                        TotalPayments += item.invoiceSC;
                        UpdateOrAddReconciliationEntry(item.TransId, item.TransRowId, item.invoiceLC, item.TransType, item.ActualAmount, reconcilation);
                        UpdateOrAddReconciliationEntry(TopPayment.TransId, TopPayment.TransRowId, TopPayment.paymentLC, TopPayment.TransType, TopPayment.ActualAmount, reconcilation);
                        item.invoiceLC = 0;
                        TopPayment.paymentLC = 0;
                    }
                    else
                    {
                        TotalPayments += TopPayment.paymentSC;
                        if (item.exRate != 0 && TopPayment.paymentLC != 0)
                        {
                            TotalInvoices += Math.Round(TopPayment.paymentLC / item.exRate, 10);
                        }
                        else
                        {
                            TotalInvoices += Math.Round(TopPayment.paymentSC, 10);
                        }
                        UpdateOrAddReconciliationEntry(item.TransId, item.TransRowId, TopPayment.paymentLC, item.TransType, item.ActualAmount, reconcilation);
                        UpdateOrAddReconciliationEntry(TopPayment.TransId, TopPayment.TransRowId, TopPayment.paymentLC, TopPayment.TransType, TopPayment.ActualAmount, reconcilation);
                        if (item.exRate != 0 && TopPayment.paymentLC != 0)
                        {
                            item.invoiceSC -= Math.Round(TopPayment.paymentLC / item.exRate, 10);
                        }
                        else
                        {
                            item.invoiceSC -= Math.Round(TopPayment.paymentSC, 10);
                        }

                        item.invoiceLC = -Diffierence;
                        TopPayment.paymentLC = 0;
                    }
                }
            }
            TotaExRate = Math.Round((TotalPayments - TotalInvoices), 4);
            Task task =  Task.Run(() =>
            {
               bool ar = InternalReconcellation();
          
            });
            allTasks.Add(task);
        }

        private void UpdateOrAddReconciliationEntry(int transId, int transRowId, double invoiceLC, string type, double actualAmount, List<Reconcilation> reconcilation)
        {
            var existingInvoice = reconcilation.FirstOrDefault(r => r.TransId == transId && r.TransRowId == transRowId);
            if (existingInvoice != null)
            {
                existingInvoice.Sum += invoiceLC;
            }
            else
            {
                reconcilation.Add(new Reconcilation { Sum = invoiceLC, TransId = transId, TransRowId = transRowId,Type = type,ActualAmount=actualAmount });
            }
        }
        #endregion

        #region InternalReconcellation
        private bool InternalReconcellation()
        {
            try
            {
                SAPbobsCOM.InternalReconciliationParams oParam;
                SAPbobsCOM.InternalReconciliationsService oReconService;
                SAPbobsCOM.InternalReconciliationOpenTrans oOposting;
                oReconService = (InternalReconciliationsService)GlobalVariables.oCompany.GetCompanyService().GetBusinessService(SAPbobsCOM.ServiceTypes.InternalReconciliationsService);
                oParam = (InternalReconciliationParams)oReconService.GetDataInterface(SAPbobsCOM.InternalReconciliationsServiceDataInterfaces.irsInternalReconciliationParams);
                oOposting = (InternalReconciliationOpenTrans)oReconService.GetDataInterface(SAPbobsCOM.InternalReconciliationsServiceDataInterfaces.irsInternalReconciliationOpenTrans);
                oOposting.CardOrAccount = CardOrAccountEnum.coaCard;
                int row = 0;

                oOposting.InternalReconciliationOpenTransRows.Add();
                oOposting.InternalReconciliationOpenTransRows.Item(row).Selected = SAPbobsCOM.BoYesNoEnum.tYES;
                oOposting.InternalReconciliationOpenTransRows.Item(row).TransId = 46;
                oOposting.InternalReconciliationOpenTransRows.Item(row).TransRowId = 0;
                oOposting.InternalReconciliationOpenTransRows.Item(row).ReconcileAmount = 400;
                oOposting.InternalReconciliationOpenTransRows.Add();
                row++;

                oOposting.InternalReconciliationOpenTransRows.Add();
                oOposting.InternalReconciliationOpenTransRows.Item(row).Selected = SAPbobsCOM.BoYesNoEnum.tYES;
                oOposting.InternalReconciliationOpenTransRows.Item(row).TransId = 47;
                oOposting.InternalReconciliationOpenTransRows.Item(row).TransRowId = 1;
                oOposting.InternalReconciliationOpenTransRows.Item(row).ReconcileAmount = 400;

                oParam = oReconService.Add(oOposting);

                return true;
            } 
            catch (Exception ex)
            {
                string yu = "";
                throw;
            }

           
        }
        #endregion

        #region FieldsMode
        public void FieldsMode(bool oFlag)
        {
            try
            {
                frmNationalization.Items.Item("1000010").Enabled = oFlag;
                frmNationalization.Items.Item("1000011").Enabled = oFlag;
                frmNationalization.Items.Item("49").Enabled = oFlag;
                frmNationalization.Items.Item("19").Enabled = oFlag;

            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
            }
        }
        #endregion

        #region MenuEvent
        public void MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                switch (pVal.MenuUID)
                {
                    case "1281":
                        {
                            FieldsMode(true);
                            break;
                        }
                    case "1282":
                        {
                            InitForm();
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                frmNationalization.Freeze(false);
                EventHandler.oApplication.MessageBox(ex.Message);
            }
        }
        #endregion

        #region cfl
        void Customer(SAPbouiCOM.Form oForm, string type, SAPbouiCOM.EditText Field)
        {
            try
            {
                SAPbouiCOM.ChooseFromList cfl1;
                SAPbouiCOM.Conditions cons;
                SAPbouiCOM.Condition con;
                SAPbouiCOM.Conditions econ = new SAPbouiCOM.Conditions();

                cfl1 = oForm.ChooseFromLists.Item("Card");
                cfl1.SetConditions(econ);
                cons = cfl1.GetConditions();
                Field.ChooseFromListUID = "Card";
                Field.ChooseFromListAlias = "CardCode";
                cfl1.SetConditions(cons);
            }
            catch (Exception ex)
            {
                // Handle the exception
            }
        }

      
        void Account(SAPbouiCOM.Form oForm, string type, SAPbouiCOM.EditText Field)
        {
            try
            {
                SAPbouiCOM.ChooseFromList cfl1;
                SAPbouiCOM.Conditions cons;
                SAPbouiCOM.Conditions econ = new SAPbouiCOM.Conditions();
                cfl1 = oForm.ChooseFromLists.Item("Account");
                cfl1.SetConditions(econ);
                cons = cfl1.GetConditions();
                Field.ChooseFromListUID = "Account";
                Field.ChooseFromListAlias = "AcctCode";
                cfl1.SetConditions(cons);
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

    }
}
