using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMS_Sender
{
    /**************************************
     
     PORT 7 = D => RETAILER GLOBE
     PORT 6 = C => RETAILER SMART
     PORT 5 = B => 
     PORT 4 = A => 
     PORT 3 = D => 
     PORT 2 = C => 
     PORT 1 = B => 
     PORT 0 = A => GATEWAY NUMBER
     
    ***************************************/
    public partial class form_main : Form
    {
        decimal percentLoad = 0;
        bool globeActive = true, smartActive = true;

        List<SerialPort> _serialPort = new List<SerialPort>();
        List<string> coms = new List<string>();

        DatabaseHandler db = new DatabaseHandler();
        DataTable dtCodes = new DataTable();
        DataTable dtCarrier = new DataTable();
        DataTable dtSysNo = new DataTable();
        DataTable dtUSSDCode = new DataTable();
        DataTable dtRoles = new DataTable();
        DataTable dtConfig = new DataTable();

        bool started = false;

        List<string> loadQueue = new List<string>();
        List<string> config = new List<string>();
        List<string[]> msgList1 = new List<string[]>();
        List<string[]> msgList2 = new List<string[]>();
        List<string[]> smartLoad = new List<string[]>();

        string[] regComm = new string[]{ "RET", "DISTRIBUTOR", "DEALER", "MOBILE", "CITY", "PROVINCIAL" };

        List<Thread> recMsgThread = new List<Thread>();
        List<Thread> recMsgUSSDThread = new List<Thread>();
        List<Thread> processMsgThread = new List<Thread>();

        public form_main()
        {
            InitializeComponent();

            //INITIALIZE EVENTS HERE
            this.FormClosing += form_main_FormClosing;
            stripAddPromo.Click += stripAddPromo_Click;
            stripGenerate.Click += stripGenerate_Click;
            stripReply.Click += stripReply_Click;
            dgvHelp.CellMouseDown += dgvHelp_CellMouseDown;
            stripStart.Click += stripStart_Click;
            stripAdjust.Click += stripAdjust_Click;
            stripPorts.Click += stripPorts_Click;
            stripToggleGLOBE.Click += stripToggleGLOBE_Click;
            stripToggleSMART.Click += stripToggleSMART_Click;
        }

        void stripToggleSMART_Click(object sender, EventArgs e)
        {
            string toggle = "1";
            smartActive = !smartActive;

            if (!smartActive)
                toggle = "0";

            lblSmart.Text = smartActive.ToString().ToUpper();
            db.Query("UPDATE tbl_config SET _VALUE = '" + toggle + "' WHERE _CONFIG = 'SMART_ACTIVE'");
        }

        void stripToggleGLOBE_Click(object sender, EventArgs e)
        {
            string toggle = "1";
            globeActive = !globeActive;

            if (!globeActive)
                toggle = "0";

            lblGlobe.Text = globeActive.ToString().ToUpper();
            db.Query("UPDATE tbl_config SET _VALUE = '" + toggle + "' WHERE _CONFIG = 'GLOBE_ACTIVE'");
        }

        void stripPorts_Click(object sender, EventArgs e)
        {
            form_port_config fpc = new form_port_config();
            fpc.ShowDialog();
        }

        void stripAdjust_Click(object sender, EventArgs e)
        {
            form_adjust fa = new form_adjust();
            fa.ShowDialog();
        }

        void stripStart_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (!started)
            {
                started = true;
                lblRunStat.Text = "Running";
                initializeAll();
            }
            else
            {
                started = false;
                lblRunStat.Text = "Stopped";
                StopProcess();
            }
        }

        void dgvHelp_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            var dataGrid = (DataGridView)sender;
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                var row = dataGrid.Rows[e.RowIndex];
                dataGrid.CurrentCell = row.Cells[e.ColumnIndex == -1 ? 1 : e.ColumnIndex];
                row.Selected = true;
                dataGrid.Focus();
            }
        }

        void stripReply_Click(object sender, EventArgs e)
        {
            try
            {
                string message = dgvHelp.ValueString("_CONCERN", dgvHelp.CurrentRow.Index);
                form_help_reply fhr = new form_help_reply(message);
                fhr.FormClosing += delegate
                {
                    if (fhr.txtResponse.Text.Trim().Length > 0)
                        sendMsg(3, "HELP: " + fhr.txtResponse.Text, dgvHelp.ValueString("_SENDER2", dgvHelp.CurrentRow.Index));
                };
                fhr.ShowDialog();
            }
            catch { }
        }

        void stripGenerate_Click(object sender, EventArgs e)
        {
            form_generate fg = new form_generate();
            fg.ShowDialog();
        }

        void initializeAll()
        {
            _serialPort = new List<SerialPort>();
            coms = new List<string>();
            loadHelpDataGrid();
            loadDataGrid();
            initDatabase();
            initConfig();
            initPorts();
            checkActiveSIM();
            //AS AN I.T. REMEMBER PORT NUMBER STARTS AT ZERO
            //WARNING DO NOT PUT SIM PORT TASK TWICE! BLUE-SCREEN MAY OCCUR!
            receiveMsg(0);//GATEWAY GLOBE
            receiveMsg(1);//GATEWAY SMART
            recMsgUSSD(6);//SMART RETAILER
            recMsgUSSD(7);//GLOBE RETAILER
            processMsgs(2);//CONFIRMATION 1
            processMsgs(3);//CONFIRMATION 2
        }

        void stripAddPromo_Click(object sender, EventArgs e)
        {
            form_add_promo fap = new form_add_promo();
            fap.ShowDialog();
        }

        /*********************************EVENTS****************************************/
        void form_main_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopProcess();
        }
        /*********************************EVENTS****************************************/

        /*********************************CUSTOM METHODS****************************************/
        //THIS IS USED FOR GATEWAY SIM
        void receiveMsg(int index)
        {
            recMsgThread.Add(new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    try
                    {
                        _serialPort[index].NewLine = Environment.NewLine;

                        _serialPort[index].Write("AT\r\n");
                        Thread.Sleep(100);
                        _serialPort[index].Write("AT+CMGF=1\r\n");
                        Thread.Sleep(100);
                        _serialPort[index].Write("AT+CMGL=\"ALL\"\r\n");
                        Thread.Sleep(500);

                        this.Invoke((MethodInvoker)delegate
                        {
                            //***********MESSAGE RETRIEVAL AREA*************
                            string existing = _serialPort[index].ReadExisting();

                            #region MESSAGE HANDLING
                            if (existing.Contains("+CMT: "))
                            {
                                string[] _dat = existing.Split(new string[] { "+CMT: " }, StringSplitOptions.None);

                                lblStatus.Text = "DATA:\r\n" + _dat[1];

                                string[] tmpResponse = _dat[1].Split(
                                    new[] { Environment.NewLine },
                                    StringSplitOptions.None
                                );

                                //ALL NEEDED VARIABLES
                                string[] tmpSplit = tmpResponse[0].Replace("\"", "").Split(',');
                                string _senderNbr = tmpSplit[0].Replace("+63", "0");
                                string _dateTime = tmpSplit[2].Split('+')[0].Replace("/", "-");
                                string _message = tmpResponse[1].Trim();
                                string _carrier = "";
                                bool _systemMsg = false;

                                //CHECK IF WHAT CARRIER AND PREFIX. Ex. 0945 is GLOBE
                                try
                                {
                                    var _tempDR = dtCarrier.Select("_PREFIX LIKE '" + _senderNbr.Substring(1, 4) + "%'", "");

                                    if (_tempDR.Count() == 0)
                                        _tempDR = dtCarrier.Select("_PREFIX LIKE '" + _senderNbr.Substring(1, 3) + "%'", "");

                                    foreach (DataRow dr in _tempDR)
                                    {
                                        _carrier = dr[1].ToString();
                                        break;
                                    }
                                }
                                catch { }

                                //IF CARRIER IS NON EXISTENT, TRY SYSTEM MESSAGE
                                if (_carrier == "")
                                {
                                    var _tempDR = dtSysNo.Select("_SYSNO = '" + _senderNbr + "'", "");

                                    foreach (DataRow dr in _tempDR)
                                    {
                                        _carrier = dr[1].ToString();
                                        _systemMsg = true;
                                        break;
                                    }
                                }
                                //****************************RESPONSE HANDLING****************************

                                #region *808 SMART
                                if (_senderNbr.StartsWith("800")) {

                                    char[] prefix = { '8', '0', '0', '1' };

                                    string number = _senderNbr.Trim(prefix);
                                    number = "0" + number;

                                    if (userExists(number)) {

                                        if (hasLoadBalance(number, 15)) {
                                            sendMsg(6, "PASALOAD " + number + " 15", "808");
                                        }
                                        else {
                                            sendMsg(3, "You don't have enough Load Wallet to make this transaction", number);
                                        }
                                    }
                                } 
                                #endregion

                                #region System response
                                if (_systemMsg)
                                {
                                    #region AutoLoadMAX response
                                    if (_senderNbr.EqualsIgnoreCase("AutoLoadMAX"))
                                    {
                                    }
                                    #endregion
                                    #region Just System response
                                    else
                                    {
                                        var _tempDR = dtSysNo.Select("_SYSNO = '" + _senderNbr + "'", "");
                                        insertMessage(_senderNbr, _tempDR[0]["_CARRIER"].ToString(), _message, _dateTime, "");
                                        loadDataGrid();
                                    }
                                    #endregion
                                }
                                #endregion
                                #region Cell No. Text
                                else if (!_systemMsg)
                                {
                                    if (userExists(_senderNbr))
                                    {
                                        #region BALANCE INQUIRY
                                        if (_message.EqualsIgnoreCase("BAL"))
                                        {
                                            var acctINFO = accountInfo(_senderNbr);
                                            sendMsg(3, "Current Balance:" + Environment.NewLine +
                                            "iLOAD: " + acctINFO[0].toFinancial() + Environment.NewLine +
                                            "PINS: " + acctINFO[1] + Environment.NewLine + Environment.NewLine +
                                            DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                        }
                                        #endregion
                                        #region CIGNAL
                                        else if (_message.ToUpper().Contains("CIGNAL "))
                                        {
                                            try
                                            {
                                                DataRow[] _tempCode = new DataRow[0];
                                                string[] _messageSplit = new string[0];
                                                _messageSplit = _message.Split(' ');
                                                string refno = InputHelper.GenRefNo;
                                                string loadAMT = "";
                                                bool loadOK = false;

                                                if (_messageSplit.Length == 3)
                                                {
                                                    _tempCode = dtCodes.Select("_CARRIER = 'CIGNAL'", "");
                                                    foreach (DataRow dr in _tempCode)
                                                        if (dr[2].ToString().EqualsIgnoreCase(_messageSplit[2]))
                                                        {
                                                            loadAMT = dr[3].ToString();
                                                            if (hasLoadBalance(_senderNbr, decimal.Parse(loadAMT)))
                                                            {
                                                                //INSERT MESSAGE AS PENDING FIRST
                                                                insertPending(_senderNbr,
                                                                    dr["_CARRIER"].ToString(),
                                                                    _messageSplit[1],
                                                                    dr[3].ToString(),
                                                                    _message,
                                                                    _dateTime,
                                                                    refno);
                                                                loadOK = true;
                                                                sendMsg(6, string.Format(dr[7].ToString(), _messageSplit[1]), "3443");
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                sendMsg(3, "Load Failed! You don't have enough load balance to make this transaction.", _senderNbr);
                                                                break;
                                                            }
                                                        }

                                                    if (loadOK)
                                                    {
                                                        deductLoad(_senderNbr, loadAMT);
                                                        recordIncome(_senderNbr, (decimal.Parse(loadAMT) * percentLoad), "LOAD", refno, DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
                                                        var acctINFO = accountInfo(_senderNbr);
                                                        Thread.Sleep(1000);
                                                        sendMsg(3, "Load Success! You have issued P" + loadAMT + "(" + (decimal.Parse(loadAMT)*(1-percentLoad)) + ") " +
                                                        "Cust #: " + _messageSplit[1] + Environment.NewLine +
                                                        "Curr bal: " + acctINFO[0].toFinancial() + Environment.NewLine +
                                                        "Ref #: " + refno + Environment.NewLine + Environment.NewLine +
                                                            DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                    }
                                                    else {
                                                        sendMsg(3, "Load Failed. Invalid product code, please use the correct product code.", _senderNbr);
                                                    }
                                                }
                                                else
                                                    sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                            }
                                            catch
                                            {

                                            }
                                        }
                                        #endregion
                                        #region PLDT
                                        else if (_message.ToUpper().Contains("PLDT "))
                                        {
                                            try
                                            {
                                                DataRow[] _tempCode = new DataRow[0];
                                                string[] _messageSplit = new string[0];
                                                _messageSplit = _message.Split(' ');
                                                string refno = InputHelper.GenRefNo;
                                                string loadAMT = "";
                                                bool loadOK = false;

                                                if (_messageSplit.Length == 3)
                                                {
                                                    _tempCode = dtCodes.Select("_CARRIER = 'PLDT'", "");
                                                    foreach (DataRow dr in _tempCode)
                                                        if (dr[2].ToString().EqualsIgnoreCase(_messageSplit[2]))
                                                        {
                                                            loadAMT = dr[3].ToString();
                                                            if (hasLoadBalance(_senderNbr, decimal.Parse(loadAMT)))
                                                            {
                                                                //INSERT MESSAGE AS PENDING FIRST
                                                                insertPending(_senderNbr,
                                                                    dr["_CARRIER"].ToString(),
                                                                    _messageSplit[1],
                                                                    dr[3].ToString(),
                                                                    _message,
                                                                    _dateTime,
                                                                    refno);
                                                                loadOK = true;
                                                                sendMsg(6, string.Format(dr[7].ToString(),_messageSplit[1]), "4122");
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                sendMsg(3, "Load Failed! You don't have enough load balance to make this transaction.", _senderNbr);
                                                                break;
                                                            }
                                                        }

                                                    if (loadOK)
                                                    {
                                                        deductLoad(_senderNbr, loadAMT);
                                                        recordIncome(_senderNbr, (decimal.Parse(loadAMT) * percentLoad), "LOAD", refno, DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
                                                        var acctINFO = accountInfo(_senderNbr);
                                                        Thread.Sleep(1000);
                                                        sendMsg(3, "Load Success! You have issued P" + loadAMT + "(" + (decimal.Parse(loadAMT) * (1 - percentLoad)) + ") " +
                                                        "Cust #: " + _messageSplit[1] + Environment.NewLine +
                                                        "Curr bal: " + acctINFO[0].toFinancial() + Environment.NewLine +
                                                        "Ref #: " + refno + Environment.NewLine + Environment.NewLine +
                                                            DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                    }
                                                    else {
                                                        sendMsg(3, "Load Failed. Invalid product code, please use the correct product code.", _senderNbr);
                                                    }
                                                }
                                                else
                                                    sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                            }
                                            catch
                                            {
                                                sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                            }
                                        }
                                        #endregion
                                        else
                                        {
                                            try
                                            {
                                                string _msgType = "";
                                                string[] _messageSplit = new string[0];
                                                _messageSplit = _message.Split(' ');
                                                _msgType = _messageSplit[0].Trim();
                                                string refno = InputHelper.GenRefNo;

                                                #region NORMAL LOAD
                                                if (Regex.IsMatch(_msgType, @"^[0-9]+$"))
                                                {
                                                    string[] _msgLoad = _message.Split(' ');
                                                    string _loadCarrier = identifyCarrier(_msgLoad[0]);
                                                    bool loadOK = false;
                                                    string loadAMT = "";
                                                    //CHECK IF THERE'S SAME MESSAGE 30 MINS AGO
                                                    if (!has30minsText(_message.Trim(), _senderNbr))
                                                    {

                                                        bool noLoadBalance = false;
                                                        //GET THE LIST OF AVAILABLE CODE DEPENDING ON THE CARRIER!
                                                        DataRow[] _tempCode = new DataRow[0];
                                                        if (_carrier.Contains("/"))
                                                        {
                                                            string[] _tempCarrier = _carrier.Split('/');
                                                            _tempCode = dtCodes.Select("_CARRIER = '" + _tempCarrier[0] + "' OR _CARRIER = '" + _tempCarrier[1] + "' OR _CARRIER = '" + _loadCarrier + "'", "");
                                                        }
                                                        else
                                                        {
                                                            _tempCode = dtCodes.Select("_CARRIER = '" + _loadCarrier + "'", "");
                                                        }

                                                        foreach (DataRow dr in _tempCode)
                                                        {
                                                            if (dr[2].ToString() == _msgLoad[1].Trim())
                                                            {
                                                                loadAMT = dr[3].ToString();
                                                                if (hasLoadBalance(_senderNbr, decimal.Parse(loadAMT)))
                                                                {
                                                                    //INSERT MESSAGE AS PENDING FIRST
                                                                    insertPending(_senderNbr,
                                                                        dr["_CARRIER"].ToString(),
                                                                        _msgLoad[0],
                                                                        dr[3].ToString(),
                                                                        _message,
                                                                        _dateTime,
                                                                        refno);

                                                                    string[] _tempCarrier = new string[0];
                                                                    //TIME TO LOAD
                                                                    #region GLOBE LOAD
                                                                    if (_loadCarrier.Contains("/") || _loadCarrier.Contains("Globe"))
                                                                    {
                                                                        if (globeActive)
                                                                        {
                                                                            _tempCarrier = _loadCarrier.Split('/');
                                                                            for (int i = 0; i < _tempCarrier.Length; i++)
                                                                            {
                                                                                try
                                                                                {
                                                                                    var _tmpUSSDCode = dtUSSDCode.Select("_CARRIER LIKE '%" + _tempCarrier[i] + "' OR _CARRIER LIKE '" + _tempCarrier[i] + "%' OR _CARRIER = '" + _loadCarrier + "'", "");
                                                                                    if (_tmpUSSDCode.Length > 0)
                                                                                    {
                                                                                        loadOK = true;
                                                                                        sendUSSD(7, _tmpUSSDCode[0]["_CODE"].ToString());
                                                                                        loadQueue.Add(dr[6].ToString() + _msgLoad[0]);
                                                                                        break;
                                                                                    }
                                                                                }
                                                                                catch { }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            sendMsg(3, "Sorry, GLOBE transactions are currently unavailable as of this moment. Please try again later.", _senderNbr);
                                                                            break;
                                                                        }
                                                                    }
                                                                    #endregion
                                                                    #region SMART LOAD
                                                                    else if (_loadCarrier.EqualsIgnoreCase("Smart"))
                                                                    {
                                                                        if (smartActive)
                                                                        {
                                                                            loadOK = true;
                                                                            if (dr[2].ToString().Contains('P')) {
                                                                                sendMsg(6, "Load " + dr[2].ToString().Split('P')[1].Trim() + " " + _msgLoad[0], "343");
                                                                            }
                                                                            else {
                                                                                sendMsg(6, "Load " + dr[2].ToString() + " " + _msgLoad[0], "343");
                                                                            }
                                                                            break;
                                                                        }
                                                                        else
                                                                        {
                                                                            sendMsg(3, "Sorry, SMART transactions are currently unavailable as of this moment. Please try again later.", _senderNbr);
                                                                            break;
                                                                        }
                                                                    }
                                                                    #endregion
                                                                    #region TNT LOAD
                                                                    else if (_loadCarrier.EqualsIgnoreCase("TNT"))
                                                                    {
                                                                        if (smartActive)
                                                                        {
                                                                            loadOK = true;
                                                                            

                                                                            if (dr[2].ToString().Contains('P')) {
                                                                                sendMsg(6, "Load " + dr[2].ToString().Split('P')[1].Trim() + " " + _msgLoad[0], "343");
                                                                            }
                                                                            else {
                                                                                sendMsg(6, "Load " + dr[2].ToString() + " " + _msgLoad[0], "4540");
                                                                            }
                                                                            break;
                                                                        }
                                                                        else
                                                                        {
                                                                            sendMsg(3, "Sorry, SMART transactions are currently unavailable as of this moment. Please try again later.", _senderNbr);
                                                                            break;
                                                                        }
                                                                    }
                                                                    #endregion
                                                                }
                                                                else
                                                                {
                                                                    sendMsg(3, "Load Failed! You don't have enough load balance to make this transaction.", _senderNbr);
                                                                    noLoadBalance = true;
                                                                    break;
                                                                }
                                                            }
                                                        }

                                                        if (loadOK)
                                                        {
                                                            deductLoad(_senderNbr, loadAMT);
                                                            recordIncome(_senderNbr, (decimal.Parse(loadAMT) * percentLoad), "LOAD", refno, DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
                                                            var acctINFO = accountInfo(_senderNbr);
                                                            Thread.Sleep(1000);
                                                            sendMsg(3, "Load Success! You have issued P" + loadAMT + "(" + (decimal.Parse(loadAMT) * (1 - percentLoad)) + ") " +
                                                            "Cust #: " + _msgLoad[0] + Environment.NewLine +
                                                            "Curr bal: " + acctINFO[0].toFinancial() + Environment.NewLine +
                                                            "Ref #: " + refno + Environment.NewLine + Environment.NewLine +
                                                                DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                        }
                                                        else {
                                                            if (!noLoadBalance && smartActive && globeActive) {
                                                                sendMsg(3, "Load Failed. Invalid product code, please use the correct product code. (590)", _senderNbr);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        sendMsg(3, "Duplicate Load Detected! To intentionally repeat load place \"REP<SPACE>\" before the number.", _senderNbr);
                                                    }
                                                }
                                                #endregion
                                                #region REPEATING MESSAGE
                                                else if (_msgType.EqualsIgnoreCase("REP"))
                                                {
                                                    List<string> _messageList = _message.Split(' ').ToList();
                                                    _messageList.RemoveAt(0);
                                                    _message = string.Join(" ", _messageList.ToArray());

                                                    if (_message.Trim().Split(' ')[0].EqualsIgnoreCase("TTU")) {
                                                        var position = db.QueryDT("SELECT _ROLE FROM tbl_users WHERE _PHONE = '" + _senderNbr + "'").ValueString("_ROLE", 0);
                                                        string nbr = _message.Trim().Split(' ')[1].Trim();
                                                        string amt = _message.Trim().Split(' ')[2].Trim();

                                                        if (nbr != _senderNbr)
                                                        {
                                                            if (position != "RETAILER") {
                                                                if (userExists(nbr)) {
                                                                    if (isLessEqual(_senderNbr, nbr)) {
                                                                        if (hasPINS(_senderNbr, int.Parse(amt))) {
                                                                            try {
                                                                                if (has30minsPINS(_senderNbr, nbr, amt)) {
                                                                                    transferPins(_senderNbr, nbr, int.Parse(amt));
                                                                                    var acctINFO1 = accountInfo(_senderNbr);
                                                                                    var acctINFO2 = accountInfo(nbr);
                                                                                    string txtRefno = InputHelper.GenRefNo;
                                                                                    sendMsg(3, "You have received " + amt + " PINS from " + _senderNbr + "." + Environment.NewLine + "Available TU Pins: " + int.Parse(acctINFO2[1]).ToString("#,##0") + Environment.NewLine + "Ref No. " + txtRefno + Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), nbr);
                                                                                    Thread.Sleep(500);
                                                                                    sendMsg(3, "You have issued " + amt + " PINS to " + nbr + "." + Environment.NewLine + "New PIN balance " + int.Parse(acctINFO1[1]).ToString("#,##0") + Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                                                    Thread.Sleep(500);
                                                                                    sendMsg(3, "Ref No. " + txtRefno + Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                                                    var name = db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + nbr + "'").ValueString("_FULLNAME", 0);
                                                                                    insertPINHistory(_senderNbr, amt, nbr, name, acctINFO2[1], txtRefno, DateTime.Now.ToString("yyyy-MM-dd HH:mm:tt"));
                                                                                }
                                                                                else
                                                                                    sendMsg(3, "The transaction can only be repeated after 30 minutes.", _senderNbr);
                                                                            }
                                                                            catch {
                                                                                sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                                                            }
                                                                        }
                                                                        else
                                                                            sendMsg(3, "The transaction could not be completed, you have insufficient balance.", _senderNbr);
                                                                    }
                                                                    else
                                                                        sendMsg(3, "You are not allowed to transfer to higher account.", _senderNbr);
                                                                }
                                                                else
                                                                    sendMsg(3, "Receiver is not registered in the system.", _senderNbr);
                                                            }
                                                            else
                                                                sendMsg(3, "You are not allowed to transfer to higher account.", _senderNbr);
                                                        }
                                                        else
                                                        {
                                                            sendMsg(3, "You are not allowed to transfer pins to your own account.", _senderNbr);
                                                        }
                                                    }

                                                    if (_message.Trim().Split(' ')[0].EqualsIgnoreCase("TLC")) {
                                                        string nbr = _message.Split(' ')[1].Trim();
                                                        string amt = _message.Split(' ')[2].Trim();
                                                        decimal decAmt = decimal.Parse(amt);

                                                        string senderRole = db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + _senderNbr + "'").ValueString("_ROLE", 0);
                                                        string receiverRole = db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + nbr + "'").ValueString("_ROLE", 0);

                                                        bool equalLevel = (senderRole == receiverRole);

                                                        if (nbr != _senderNbr)
                                                        {
                                                            if (userExists(nbr)) {
                                                                if (hasLoadBalance(_senderNbr, decimal.Parse(amt))) {
                                                                    if (decAmt > 49) {
                                                                        try {
                                                                            if (isLessEqual(_senderNbr, nbr)) {
                                                                                string txtRefno = InputHelper.GenRefNo;
                                                                                percentLoad = setPercentToBeDeducted(_senderNbr, nbr);
                                                                                transferLoadWallet(_senderNbr, nbr, decAmt);
                                                                                recordIncome(_senderNbr, (decAmt * percentLoad), "TLC", txtRefno, DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
                                                                                var acctINFO1 = accountInfo(_senderNbr);
                                                                                var acctINFO2 = accountInfo(nbr);
                                                                                Thread.Sleep(500);
                                                                                if (equalLevel) {
                                                                                    sendMsg(3, "You have issued P " + decAmt.ToString("#,##0.00") + "(" + decAmt.ToString("#,##0.00") + ")" + " load to " + nbr + ". " + "New load balance P " + decimal.Parse(acctINFO1[0]).ToString("#,##0.00") + ". " + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                                                }
                                                                                else {
                                                                                    sendMsg(3, "You have issued P " + decAmt.ToString("#,##0.00") + " (" + (decAmt - (decAmt * percentLoad)).ToString("#,##0.00") + ") load to " + nbr + ". " + "New load balance P " + decimal.Parse(acctINFO1[0]).ToString("#,##0.00") + ". " + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                                                }
                                                                                Thread.Sleep(500);
                                                                                sendMsg(3, "Ref No. " + txtRefno + ". " + Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                                                Thread.Sleep(500);
                                                                                sendMsg(3, "You have received P " + decAmt.ToString("#,##0.00") + " load from " + _senderNbr + "." + Environment.NewLine + "New load balance P " + decimal.Parse(acctINFO2[0]).ToString("#,##0.00") + " " + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), nbr);
                                                                                Thread.Sleep(500);
                                                                                sendMsg(3, "Ref No. " + txtRefno + ". " + Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), nbr);
                                                                                insertTLCHistory(_senderNbr, acctINFO1[0], nbr, acctINFO2[0], txtRefno, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), amt);
                                                                            }
                                                                            else
                                                                                sendMsg(3, "You are not allowed to transfer to this account.", _senderNbr);
                                                                        }
                                                                        catch (Exception e) {
                                                                            sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                                                        }
                                                                    }
                                                                    else
                                                                        sendMsg(3, "The minimum amount that can be transferred is P 50.00", _senderNbr);
                                                                }
                                                                else
                                                                    sendMsg(3, "The transaction could not be completed, you have insufficient balance.", _senderNbr);
                                                            }
                                                            else
                                                                sendMsg(3, "Receiver is not registered in the system.", _senderNbr);
                                                        }
                                                        else
                                                        {
                                                            sendMsg(3, "You can not send load credits to your own number.", _senderNbr);
                                                        }
                                                        
                                                    }
                                                    
                                                    string[] _msgLoad = _message.Split(' ');
                                                    string _loadCarrier = identifyCarrier(_msgLoad[0]);
                                                    bool loadOK = false;
                                                    string loadAMT = "";

                                                    //GET THE LIST OF AVAILABLE CODE DEPENDING ON THE CARRIER!
                                                    DataRow[] _tempCode = new DataRow[0];
                                                    if (_carrier.Contains("/"))
                                                    {
                                                        string[] _tempCarrier = _carrier.Split('/');
                                                        _tempCode = dtCodes.Select("_CARRIER = '" + _tempCarrier[0] + "' OR _CARRIER = '" + _tempCarrier[1] + "' OR _CARRIER = '" + _loadCarrier + "'", "");
                                                    }
                                                    else
                                                    {
                                                        _tempCode = dtCodes.Select("_CARRIER = '" + _loadCarrier + "'", "");
                                                    }


                                                    foreach (DataRow dr in _tempCode)
                                                    {
                                                        if (dr[2].ToString() == _msgLoad[1].Trim())
                                                        {
                                                            loadAMT = dr[3].ToString();
                                                            if (hasLoadBalance(_senderNbr, decimal.Parse(loadAMT)))
                                                            {
                                                                //INSERT MESSAGE AS PENDING FIRST
                                                                insertPending(_senderNbr,
                                                                    dr["_CARRIER"].ToString(),
                                                                    _msgLoad[0],
                                                                    dr[3].ToString(),
                                                                    _message,
                                                                    _dateTime,
                                                                    refno);

                                                                string[] _tempCarrier = new string[0];
                                                                //TIME TO LOAD
                                                                #region GLOBE LOAD
                                                                if (_loadCarrier.Contains("/") || _loadCarrier.Contains("Globe"))
                                                                {
                                                                    if (globeActive)
                                                                    {
                                                                        _tempCarrier = _loadCarrier.Split('/');
                                                                        for (int i = 0; i < _tempCarrier.Length; i++)
                                                                        {
                                                                            try
                                                                            {
                                                                                var _tmpUSSDCode = dtUSSDCode.Select("_CARRIER LIKE '%" + _tempCarrier[i] + "' OR _CARRIER LIKE '" + _tempCarrier[i] + "%' OR _CARRIER = '" + _loadCarrier + "'", "");
                                                                                if (_tmpUSSDCode.Length > 0)
                                                                                {
                                                                                    loadOK = true;sendUSSD(7, _tmpUSSDCode[0]["_CODE"].ToString());
                                                                                    loadQueue.Add(dr[6].ToString() + _msgLoad[0]);
                                                                                    break;
                                                                                }
                                                                            }
                                                                            catch { }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        sendMsg(3, "Sorry, GLOBE transactions are currently unavailable as of this moment. Please try again later.", _senderNbr);
                                                                        break;
                                                                    }
                                                                }
                                                                #endregion
                                                                #region SMART LOAD
                                                                else if (_loadCarrier.EqualsIgnoreCase("Smart"))
                                                                {
                                                                    if (smartActive)
                                                                    {
                                                                        loadOK = true;
                                                                        if (dr[2].ToString().Contains('P')) {
                                                                            sendMsg(6, "Load " + dr[2].ToString().Split('P')[1].Trim() + " " + _msgLoad[0], "343");
                                                                        }
                                                                        else {
                                                                            sendMsg(6, "Load " + dr[2].ToString() + " " + _msgLoad[0], "343");
                                                                        }
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        sendMsg(3, "Sorry, SMART transactions are currently unavailable as of this moment. Please try again later.", _senderNbr);
                                                                        break;
                                                                    }
                                                                }
                                                                #endregion
                                                                #region TNT LOAD
                                                                else if (_loadCarrier.EqualsIgnoreCase("TNT"))
                                                                {
                                                                    if (smartActive)
                                                                    {
                                                                        loadOK = true;
                                                                        if (dr[2].ToString().Contains('P')) {
                                                                            sendMsg(6, "Load " + dr[2].ToString().Split('P')[1].Trim() + " " + _msgLoad[0], "343");
                                                                        }
                                                                        else {
                                                                            sendMsg(6, "Load " + dr[2].ToString() + " " + _msgLoad[0], "343");
                                                                        }
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        sendMsg(3, "Sorry, SMART transactions are currently unavailable as of this moment. Please try again later.", _senderNbr);
                                                                        break;
                                                                    }
                                                                }
                                                                #endregion
                                                            }
                                                            else
                                                            {
                                                                sendMsg(3, "Load Failed! You don't have enough load balance to make this transaction.", _senderNbr);
                                                                break;
                                                            }
                                                        }
                                                    }

                                                    if (loadOK)
                                                    {
                                                        deductLoad(_senderNbr, loadAMT);
                                                        recordIncome(_senderNbr, (decimal.Parse(loadAMT) * percentLoad), "LOAD", refno, DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
                                                        var acctINFO = accountInfo(_senderNbr);
                                                        Thread.Sleep(1000);
                                                        sendMsg(3, "Load Success! You have issued P" + loadAMT + "(" + (decimal.Parse(loadAMT) * (1 - percentLoad)) + ") " +
                                                        "Cust #: " + _msgLoad[0] + Environment.NewLine +
                                                        "Curr bal: " + acctINFO[0].toFinancial() + Environment.NewLine +
                                                        "Ref #: " + refno + Environment.NewLine + Environment.NewLine +
                                                            DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                    }
                                                    else {
                                                        bool prefixNotLoad = !(_message.Trim().Split(' ')[0].EqualsIgnoreCase("TTU") || _message.Trim().Split(' ')[0].EqualsIgnoreCase("TLC"));
                                                        if (prefixNotLoad) {
                                                            sendMsg(3, "Load Failed. Invalid product code, please use the correct product code. (836)", _senderNbr);
                                                        }
                                                    }
                                                }
                                                #endregion
                                                #region TRANSFER LOAD WALLET
                                                else if (_msgType.EqualsIgnoreCase("TLC"))
                                                {
                                                    _message = _message.Trim();
                                                    string nbr = _message.Split(' ')[1].Trim();
                                                    string amt = _message.Split(' ')[2].Trim();
                                                    decimal decAmt = decimal.Parse(amt);

                                                    string senderRole = db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + _senderNbr + "'").ValueString("_ROLE", 0);
                                                    string receiverRole = db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + nbr + "'").ValueString("_ROLE", 0);

                                                    bool equalLevel = (senderRole == receiverRole);

                                                    if (nbr !=  _senderNbr){
                                                        if (!has30minsTLC(_senderNbr, amt, nbr)) {
                                                            if (userExists(nbr)) {
                                                                if (hasLoadBalance(_senderNbr, decimal.Parse(amt))) {
                                                                    if (decAmt > 49) {
                                                                        try {
                                                                            if (isLessEqual(_senderNbr, nbr)) {
                                                                                string txtRefno = InputHelper.GenRefNo;
                                                                                percentLoad = setPercentToBeDeducted(_senderNbr, nbr);
                                                                                transferLoadWallet(_senderNbr, nbr, decAmt);
                                                                                recordIncome(_senderNbr, (decAmt * percentLoad), "TLC", txtRefno, DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
                                                                                var acctINFO1 = accountInfo(_senderNbr);
                                                                                var acctINFO2 = accountInfo(nbr);
                                                                                Thread.Sleep(500);
                                                                                if (equalLevel) {
                                                                                    sendMsg(3, "You have issued P " + decAmt.ToString("#,##0.00") + "(" + decAmt.ToString("#,##0.00") + ")" + " load to " + nbr + ". " + "New load balance P " + decimal.Parse(acctINFO1[0]).ToString("#,##0.00") + ". " + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                                                }
                                                                                else {
                                                                                    sendMsg(3, "You have issued P " + decAmt.ToString("#,##0.00") + " (" + (decAmt - (decAmt * percentLoad)).ToString("#,##0.00") + ") load to " + nbr + ". " + "New load balance P " + decimal.Parse(acctINFO1[0]).ToString("#,##0.00") + ". " + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                                                }
                                                                                Thread.Sleep(500);
                                                                                sendMsg(3, "Ref No. " + txtRefno + ". " + Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                                                Thread.Sleep(500);
                                                                                sendMsg(3, "You have received P " + decAmt.ToString("#,##0.00") + " load from " + _senderNbr + "." + Environment.NewLine + "New load balance P " + decimal.Parse(acctINFO2[0]).ToString("#,##0.00") + " " + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), nbr);
                                                                                Thread.Sleep(500);
                                                                                sendMsg(3, "Ref No. " + txtRefno + ". " + Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), nbr);
                                                                                insertTLCHistory(_senderNbr, acctINFO1[0], nbr, acctINFO2[0], txtRefno, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), amt);
                                                                            }
                                                                            else
                                                                                sendMsg(3, "You are not allowed to transfer to this account.", _senderNbr);
                                                                        }
                                                                        catch (Exception e) {
                                                                            sendMsg(3, "System Error. Please try again later.", _senderNbr);
                                                                        }
                                                                    }
                                                                    else
                                                                        sendMsg(3, "The minimum amount that can be transferred is P 50.00", _senderNbr);
                                                                }
                                                                else
                                                                    sendMsg(3, "The transaction could not be completed, you have insufficient balance.", _senderNbr);
                                                            }
                                                            else
                                                                sendMsg(3, "Receiver is not registered in the system.", _senderNbr);
                                                        }
                                                        else
                                                            sendMsg(3, "The transaction can only be repeated after 30 minutes.", _senderNbr);
                                                    }
                                                    else{
                                                        sendMsg(3, "You can not send load credits to your own number.", _senderNbr);
                                                    }
                                                    
                                                }
                                                #endregion
                                                #region FORWARD PINS
                                                else if (_msgType.EqualsIgnoreCase("TTU"))
                                                {
                                                    var position = db.QueryDT("SELECT _ROLE FROM tbl_users WHERE _PHONE = '" + _senderNbr + "'").ValueString("_ROLE", 0);
                                                    string nbr = _message.Trim().Split(' ')[1].Trim();
                                                    string amt = _message.Trim().Split(' ')[2].Trim();

                                                    if (nbr != _senderNbr) {
                                                        if (position != "RETAILER") {
                                                            if (userExists(nbr)) {
                                                                if (isLessEqual(_senderNbr, nbr)) {
                                                                    if (hasPINS(_senderNbr, int.Parse(amt))) {
                                                                        try {
                                                                            if (!has30minsPINS(_senderNbr, nbr, amt)) {
                                                                                transferPins(_senderNbr, nbr, int.Parse(amt));
                                                                                var acctINFO1 = accountInfo(_senderNbr);
                                                                                var acctINFO2 = accountInfo(nbr);
                                                                                string txtRefno = InputHelper.GenRefNo;
                                                                                sendMsg(3, "You have received " + amt + " PINS from " + _senderNbr + "." + Environment.NewLine + "Available TU Pins: " + int.Parse(acctINFO2[1]).ToString("#,##0") + Environment.NewLine + "Ref No. " + txtRefno + Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), nbr);
                                                                                Thread.Sleep(500);
                                                                                sendMsg(3, "You have issued " + amt + " PINS to " + nbr + "." + Environment.NewLine + "New PIN balance " + int.Parse(acctINFO1[1]).ToString("#,##0") + Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                                                Thread.Sleep(500);
                                                                                sendMsg(3, "Ref No. " + txtRefno + Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                                                var name = db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + nbr + "'").ValueString("_FULLNAME", 0);
                                                                                insertPINHistory(_senderNbr, amt, nbr, name, acctINFO2[1], txtRefno, DateTime.Now.ToString("yyyy-MM-dd HH:mm:tt"));
                                                                            }
                                                                            else
                                                                                sendMsg(3, "The transaction can only be repeated after 30 minutes.", _senderNbr);
                                                                        }
                                                                        catch {
                                                                            sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                                                        }
                                                                    }
                                                                    else
                                                                        sendMsg(3, "The transaction could not be completed, you have insufficient balance.", _senderNbr);
                                                                }
                                                                else
                                                                    sendMsg(3, "You are not allowed to transfer to higher account.", _senderNbr);
                                                            }
                                                            else
                                                                sendMsg(3, "Receiver is not registered in the system.", _senderNbr);
                                                        }
                                                        else
                                                            sendMsg(3, "You are not allowed to transfer to higher account.", _senderNbr);
                                                    }
                                                    else {
                                                        sendMsg(3, "You are not allowed to transfer pins to your own account.", _senderNbr);
                                                    }

                                                }
                                                #endregion

                                                #region ADDITIONAL ACCOUNT
                                                else if (_msgType.EqualsIgnoreCase("ADD")) {
                                                    string code = _message.Split(' ')[1].Trim().Split('/')[0];
                                                    string sender = _senderNbr;

                                                    string status = db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + sender + "'").ValueString("_ROLE", 0).Split(' ')[0].Trim();
                                                    string oldPin = db.QueryDT("SELECT * FROM tbl_account WHERE _PHONE = '" + sender + "'").ValueString("_PINS", 0);

                                                    bool elligibleCode = db.QueryDT("SELECT * FROM tbl_act_code WHERE _CODE = '" + code + "' AND _USED = 'NO'").RowCount() == 1;
                                                    bool elligibleType = db.QueryDT("SELECT * FROM tbl_act_code WHERE _TYPE = '" + status + "' AND _CODE = '" + code + "'").RowCount() == 1;
                                                    bool elligibleNumber = userExists(sender);

                                                    int pins = 0;
                                                    
                                                    if (userExists(sender)) {
                                                        if (elligibleCode && elligibleType) {

                                                            try {
                                                                switch (status) {
                                                                    case ("RETAILER"):
                                                                        pins = 0;
                                                                        break;

                                                                    case ("DISTRIBUTOR"):
                                                                        pins = 50;
                                                                        break;

                                                                    case ("DEALER"):
                                                                        pins = 70;
                                                                        break;

                                                                    case ("MOBILE"):
                                                                        pins = 1500;
                                                                        break;

                                                                    case ("CITY"):
                                                                        pins = 7000;
                                                                        break;

                                                                    case ("PROVINCIAL"):
                                                                        pins = 20000;
                                                                        break;
                                                                }
                                                                db.Query("UPDATE tbl_account SET _PINS = '" + (int.Parse(oldPin) + pins) + "' WHERE _PHONE = '" + sender + "'");
                                                                db.Query("UPDATE tbl_act_code SET _USED = 'YES' WHERE _CODE = '" + code + "'");

                                                                if ((status == "MOBILE") || (status == "CITY") || (status == "PROVINCIAL")) {
                                                                    sendMsg(3, "Congratulations! " + sender + " has added additional 1 " + status.ToLower() + " stockiest. You now have additional + " + pins.ToString() + " TU pins.", sender);
                                                                }
                                                                else {
                                                                    sendMsg(3, "Congratulations! " + sender + " has added additional 1 " + status.ToLower() + ". You now have additional + " + pins.ToString() + " TU pins.", sender);
                                                                }
                                                            }
                                                            catch {
                                                                sendMsg(3, "System Error. Please try again later.", sender);
                                                            }
                                                        }
                                                        else {
                                                            sendMsg(3, "Additional Account Error. Invalid product code.", sender);
                                                        }
                                                    }
                                                    else {
                                                        sendMsg(3, "Additional Account Error. Number not registered in the system", sender);
                                                    }
                                                } 
                                                #endregion

                                                else if (_msgType.ToUpper().Contains("UP")) {
                                                    string number = _message.Split(' ')[1].Trim().Split('/')[1];
                                                    string code = _message.Split(' ')[1].Trim().Split('/')[0];
                                                    string sender = _senderNbr;

                                                    string upgradeTo = _message.Split(' ')[0].Trim().Replace("UP", "");
                                                    string upgradeFrom = db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + sender + "'").ValueString("_ROLE", 0).Split(' ')[0].Trim();

                                                    int upgradeToLevel = db.QueryDT("SELECT * FROM tbl_role WHERE _ROLE LIKE '" + upgradeTo + "%'").ValueInt("_LEVEL", 0);
                                                    int upgradeFromLevel = db.QueryDT("SELECT * FROM tbl_role WHERE _ROLE LIKE '" + upgradeFrom + "%'").ValueInt("_LEVEL", 0);
                                                    int pins = db.QueryDT("SELECT * FROM tbl_account WHERE _PHONE = '" + number + "'").ValueInt("_PINS", 0);

                                                    bool elligibleLevel = upgradeToLevel > upgradeFromLevel;
                                                    bool elligibleCode = db.QueryDT("SELECT * FROM tbl_act_code WHERE _CODE = '" + code + "' AND _USED = 'NO'").RowCount() == 1;
                                                    bool elligibleType = db.QueryDT("SELECT * FROM tbl_act_code WHERE _TYPE = '" + upgradeTo + "' AND _CODE = '" + code + "'").RowCount() == 1;


                                                    if (userExists(number)) {
                                                        if (elligibleCode) {
                                                            if (elligibleLevel) {
                                                                if (elligibleType) {

                                                                    int additionalPin = 0;

                                                                    try {
                                                                        switch (upgradeTo) {
                                                                            case ("DISTRIBUTOR"):
                                                                                additionalPin = 50;
                                                                                pins = pins + additionalPin;
                                                                                break;

                                                                            case ("DEALER"):
                                                                                additionalPin = 70;
                                                                                pins = pins + additionalPin;
                                                                                break;

                                                                            case ("MOBILE"):
                                                                                additionalPin = 1500;
                                                                                pins = pins + additionalPin;
                                                                                break;

                                                                            case ("CITY"):
                                                                                additionalPin = 7000;
                                                                                pins = pins + additionalPin;
                                                                                break;

                                                                            case ("PROVINCIAL"):
                                                                                additionalPin = 20000;
                                                                                pins = pins + additionalPin;
                                                                                break;
                                                                        }

                                                                        db.Query("UPDATE tbl_users SET _ROLE = '" + upgradeTo + "' WHERE _PHONE = '" + number + "'");
                                                                        db.Query("UPDATE tbl_account SET _PINS = '" + pins.ToString() + "' WHERE _PHONE = '" + number + "'");
                                                                        db.Query("UPDATE tbl_act_code SET _USED = 'YES' WHERE _CODE = '" + code + "'");

                                                                        if ((upgradeTo == "MOBILE") || (upgradeTo == "CITY") || (upgradeTo == "PROVINCIAL")) {
                                                                            sendMsg(3, "Congratulations! You have upgraded your account to " + upgradeTo + " STOCKIEST. You now have " + additionalPin.ToString() + " additional TU pins.", number);
                                                                            sendMsg(3, "Congratulations! " + number + " has been upgraded to " + upgradeTo + " STOCKIEST.", sender);
                                                                        }
                                                                        else {
                                                                            sendMsg(3, "Congratulations! You have upgraded your account to " + upgradeTo + ". You now have " + additionalPin.ToString() + " additional TU pins.", number);
                                                                            sendMsg(3, "Congratulations! " + number + " has been upgraded to " + upgradeTo + ".", sender);
                                                                        }
                                                                    }
                                                                    catch (Exception e) {
                                                                        sendMsg(3, "Upgrade Failed. An error has occurred, please try again later.", sender);
                                                                    }
                                                                }
                                                                else {
                                                                    //can't use code
                                                                    sendMsg(3, "Upgrade Failed. You are inelligible to use this code", sender);
                                                                }
                                                            }
                                                            else {
                                                                //code used or doesnt exist
                                                                sendMsg(3, "Upgrade Failed. You can not downgrade or upgrade to your current level.", sender);
                                                            }
                                                        }
                                                        else {
                                                            //you can not downgrade
                                                            sendMsg(3, "Upgrade Failed. The code you entered is either used or didn't exist.", sender);
                                                        }
                                                    }
                                                    else {
                                                        sendMsg(3, "Upgrade Failed. Number not registered in the system.", sender);
                                                    }
                                                    
                                                }

                                                #region RETAILER REGISTRATION
                                                else if (_msgType.EqualsIgnoreCase("RET"))
                                                {
                                                    if (canRegister(_senderNbr, "RET"))
                                                    {
                                                        if (hasPINS(_senderNbr, 1))
                                                        {
                                                            try
                                                            {
                                                                string nbr = _message.Split('/')[0].Split(' ')[1].Trim();
                                                                if (!userExists(nbr))
                                                                {
                                                                    string password = _message.Split('/')[1];

                                                                    if(password.ToString().Length == 5)
                                                                    {
                                                                        string username = _message.Split('/')[3];

                                                                        if((username.Length <= 8) && (Regex.IsMatch(username, @"^[a-zA-Z0-9]+$")))
                                                                        {
                                                                            string month = _message.Split('/')[4].Split('-')[0];
                                                                            string day = _message.Split('/')[4].Split('-')[1];
                                                                            string year = _message.Split('/')[4].Split('-')[2];

                                                                            bool validDate = (((int.Parse(month) <= 12) && (int.Parse(month) > 0)) && (year.Length == 4)) && ((int.Parse(day) > 0) && (int.Parse(day) <= 31));
                                                                            
                                                                            if (validDate) {
                                                                                regUser(_message, _senderNbr, _carrier, _dateTime, "RETAILER", "0", "0");
                                                                            }
                                                                            else
                                                                            {
                                                                                sendMsg(3, "Activation Failed. Birthday must be mm-dd-yyyy.", _senderNbr);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            sendMsg(3, "Activation Failed. Username must be 8 characters only. Letters and Numbers only.", _senderNbr);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        sendMsg(3, "Activation Failed. Password must be a 5 digit number.", _senderNbr);
                                                                    }
                                                                }
                                                                else
                                                                    sendMsg(3, "Activation Failed. Number already registered.", _senderNbr);
                                                            }
                                                            catch
                                                            {
                                                                sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                                            }
                                                        }
                                                        else
                                                            sendMsg(3, "Activation Failed! You don't have enough PINS.", _senderNbr);
                                                    }
                                                }
                                                #endregion
                                                #region DISTRIBUTOR REGISTRATION
                                                else if (_msgType.EqualsIgnoreCase("DISTRIBUTOR"))
                                                {
                                                    if (canRegister(_senderNbr, "DISTRIBUTOR"))
                                                    {
                                                        if (hasPINS(_senderNbr, 1))
                                                        {
                                                            try
                                                            {
                                                                string nbr = _message.Split('/')[0].Split(' ')[1].Trim();
                                                                if (!userExists(nbr)) 
                                                                {
                                                                    string password = _message.Split('/')[2];

                                                                    if (password.ToString().Length == 5) {
                                                                        string username = _message.Split('/')[4];

                                                                        if ((username.Length <= 8) && (Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"))) {
                                                                            string month = _message.Split('/')[5].Split('-')[0];
                                                                            string day = _message.Split('/')[5].Split('-')[1];
                                                                            string year = _message.Split('/')[5].Split('-')[2];

                                                                            bool validDate = (((int.Parse(month) <= 12) && (int.Parse(month) > 0)) && (year.Length == 4)) && ((int.Parse(day) > 0) && (int.Parse(day) <= 31));

                                                                            if (validDate) {
                                                                                
                                                                                regUser(_message, _senderNbr, _carrier, _dateTime, "DISTRIBUTOR", "0", "50");
                                                                            }
                                                                            else {
                                                                                sendMsg(3, "Activation Failed. Birthday must be mm-dd-yyyy.", _senderNbr);
                                                                            }
                                                                        }
                                                                        else {
                                                                            sendMsg(3, "Activation Failed. Username must be 8 characters only. Letters and Numbers only.", _senderNbr);
                                                                        }
                                                                    }
                                                                    else {
                                                                        sendMsg(3, "Activation Failed. Password must be a 5 digit number.", _senderNbr);
                                                                    }
                                                                }
                                                                else
                                                                    sendMsg(3, "Activation Failed.  Number already registered.", _senderNbr);
                                                            }
                                                            catch
                                                            {
                                                                sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                                            }
                                                        }
                                                        else
                                                            sendMsg(3, "Activation Failed! You don't have enough PINS.", _senderNbr);
                                                    }
                                                    else
                                                        sendMsg(3, "You cannot register a DISTRIBUTOR because your account is level is low.", _senderNbr);
                                                }
                                                #endregion
                                                #region DEALER REGISTRATION
                                                else if (_msgType.EqualsIgnoreCase("DEALER"))
                                                {
                                                    if (canRegister(_senderNbr, "DEALER"))
                                                    {
                                                        if (hasPINS(_senderNbr, 1))
                                                        {
                                                            try
                                                            {
                                                                string nbr = _message.Split('/')[0].Split(' ')[1].Trim();
                                                                if (!userExists(nbr))
                                                                {
                                                                    string password = _message.Split('/')[2];

                                                                    if (password.ToString().Length == 5) {
                                                                        string username = _message.Split('/')[4];

                                                                        if ((username.Length <= 8) && (Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"))) {
                                                                            string month = _message.Split('/')[5].Split('-')[0];
                                                                            string day = _message.Split('/')[5].Split('-')[1];
                                                                            string year = _message.Split('/')[5].Split('-')[2];

                                                                            bool validDate = (((int.Parse(month) <= 12) && (int.Parse(month) > 0)) && (year.Length == 4)) && ((int.Parse(day) > 0) && (int.Parse(day) <= 31));

                                                                            if (validDate) {
                                                                                regUser(_message, _senderNbr, _carrier, _dateTime, "DEALER", "0", "70");
                                                                            }
                                                                            else {
                                                                                sendMsg(3, "Activation Failed. Birthday must be mm-dd-yyyy.", _senderNbr);
                                                                            }
                                                                        }
                                                                        else {
                                                                            sendMsg(3, "Activation Failed. Username must be 8 characters only. Letters and Numbers only.", _senderNbr);
                                                                        }
                                                                    }
                                                                    else {
                                                                        sendMsg(3, "Activation Failed. Password must be a 5 digit number.", _senderNbr);
                                                                    }
                                                                }
                                                                    
                                                                else
                                                                    sendMsg(3, "Activation Failed. Number already registered.", _senderNbr);
                                                            }
                                                            catch
                                                            {
                                                                sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                                            }
                                                        }
                                                        else
                                                            sendMsg(3, "Activation Failed! You don't have enough PINS.", _senderNbr);
                                                    }
                                                    else
                                                        sendMsg(3, "You cannot register a DEALER because your account is level is low.", _senderNbr);
                                                }
                                                #endregion
                                                #region MOBILE STOCKIEST REGISTRATION
                                                else if (_msgType.EqualsIgnoreCase("MOBILE"))
                                                {
                                                    if (canRegister(_senderNbr, "MOBILE"))
                                                    {
                                                        if (hasPINS(_senderNbr, 1))
                                                        {
                                                            try
                                                            {
                                                                string nbr = _message.Split('/')[0].Split(' ')[1].Trim();
                                                                if (!userExists(nbr))
                                                                {
                                                                    string password = _message.Split('/')[2];

                                                                    if (password.ToString().Length == 5) {
                                                                        string username = _message.Split('/')[4];

                                                                        if ((username.Length <= 8) && (Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"))) {
                                                                            string month = _message.Split('/')[5].Split('-')[0];
                                                                            string day = _message.Split('/')[5].Split('-')[1];
                                                                            string year = _message.Split('/')[5].Split('-')[2];

                                                                            bool validDate = (((int.Parse(month) <= 12) && (int.Parse(month) > 0)) && (year.Length == 4)) && ((int.Parse(day) > 0) && (int.Parse(day) <= 31));

                                                                            if (validDate) {
                                                                                regUser(_message, _senderNbr, _carrier, _dateTime, "MOBILE", "0", "1500");
                                                                            }
                                                                            else {
                                                                                sendMsg(3, "Activation Failed. Birthday must be mm-dd-yyyy.", _senderNbr);
                                                                            }
                                                                        }
                                                                        else {
                                                                            sendMsg(3, "Activation Failed. Username must be 8 characters only. Letters and Numbers only.", _senderNbr);
                                                                        }
                                                                    }
                                                                    else {
                                                                        sendMsg(3, "Activation Failed. Password must be a 5 digit number.", _senderNbr);
                                                                    }
                                                                }
                                                                else
                                                                    sendMsg(3, "Activation Failed. Number already registered.", _senderNbr);
                                                            }
                                                            catch
                                                            {
                                                                sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                                            }
                                                        }
                                                        else
                                                            sendMsg(3, "Activation Failed! You don't have enough PINS.", _senderNbr);
                                                    }
                                                    else
                                                        sendMsg(3, "You cannot register a MOBILE STOCKIEST because your account is level is low.", _senderNbr);
                                                }
                                                #endregion
                                                #region CITY STOCKIEST REGISTRATION
                                                else if (_msgType.EqualsIgnoreCase("CITY"))
                                                {
                                                    if (canRegister(_senderNbr, "CITY"))
                                                    {
                                                        if (hasPINS(_senderNbr, 1))
                                                        {
                                                            try
                                                            {
                                                                string nbr = _message.Split('/')[0].Split(' ')[1].Trim();
                                                                if (!userExists(nbr))
                                                                {
                                                                    string password = _message.Split('/')[2];

                                                                    if (password.ToString().Length == 5) {
                                                                        string username = _message.Split('/')[4];

                                                                        if ((username.Length <= 8) && (Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"))) {
                                                                            string month = _message.Split('/')[5].Split('-')[0];
                                                                            string day = _message.Split('/')[5].Split('-')[1];
                                                                            string year = _message.Split('/')[5].Split('-')[2];

                                                                            bool validDate = (((int.Parse(month) <= 12) && (int.Parse(month) > 0)) && (year.Length == 4)) && ((int.Parse(day) > 0) && (int.Parse(day) <= 31));

                                                                            if (validDate) {
                                                                                regUser(_message, _senderNbr, _carrier, _dateTime, "CITY", "0", "7000");
                                                                            }
                                                                            else {
                                                                                sendMsg(3, "Activation Failed. Birthday must be mm-dd-yyyy.", _senderNbr);
                                                                            }
                                                                        }
                                                                        else {
                                                                            sendMsg(3, "Activation Failed. Username must be 8 characters only. Letters and Numbers only.", _senderNbr);
                                                                        }
                                                                    }
                                                                    else {
                                                                        sendMsg(3, "Activation Failed. Password must be a 5 digit number.", _senderNbr);
                                                                    }
                                                                }
                                                                else
                                                                    sendMsg(3, "Activation Failed. Number already registered.", _senderNbr);
                                                            }
                                                            catch
                                                            {
                                                                sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                                            }
                                                        }
                                                        else
                                                            sendMsg(3, "Activation Failed! You don't have enough PINS.", _senderNbr);
                                                    }
                                                    else
                                                        sendMsg(3, "You cannot register a CITY STOCKIEST because your account is level is low.", _senderNbr);
                                                }
                                                #endregion
                                                #region PROVINCIAL STOCKIEST REGISTRATION
                                                else if (_msgType.EqualsIgnoreCase("PROVINCIAL"))
                                                {
                                                    if (canRegister(_senderNbr, "PROVINCIAL"))
                                                    {
                                                        if (hasPINS(_senderNbr, 1))
                                                        {
                                                            try
                                                            {
                                                                string nbr = _message.Split('/')[0].Split(' ')[1].Trim();
                                                                if (!userExists(nbr))
                                                                {
                                                                    string password = _message.Split('/')[2];

                                                                    if (password.ToString().Length == 5) {
                                                                        string username = _message.Split('/')[4];

                                                                        if ((username.Length <= 8) && (Regex.IsMatch(username, @"^[a-zA-Z0-9]+$"))) {
                                                                            string month = _message.Split('/')[5].Split('-')[0];
                                                                            string day = _message.Split('/')[5].Split('-')[1];
                                                                            string year = _message.Split('/')[5].Split('-')[2];

                                                                            bool validDate = (((int.Parse(month) <= 12) && (int.Parse(month) > 0)) && (year.Length == 4)) && ((int.Parse(day) > 0) && (int.Parse(day) <= 31));

                                                                            if (validDate) {
                                                                                regUser(_message, _senderNbr, _carrier, _dateTime, "PROVINCIAL", "0", "20000");
                                                                            }
                                                                            else {
                                                                                sendMsg(3, "Activation Failed. Birthday must be mm-dd-yyyy.", _senderNbr);
                                                                            }
                                                                        }
                                                                        else {
                                                                            sendMsg(3, "Activation Failed. Username must be 8 characters only. Letters and Numbers only.", _senderNbr);
                                                                        }
                                                                    }
                                                                    else {
                                                                        sendMsg(3, "Activation Failed. Password must be a 5 digit number.", _senderNbr);
                                                                    }
                                                                }
                                                                else
                                                                    sendMsg(3, "Activation Failed. Number already registered.", _senderNbr);
                                                            }
                                                            catch
                                                            {
                                                                sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                                            }
                                                        }
                                                        else
                                                            sendMsg(3, "Activation Failed! You don't have enough PINS.", _senderNbr);
                                                    }
                                                    else
                                                        sendMsg(3, "You cannot register a PROVINCIAL STOCKIEST because your account is level is low.", _senderNbr);
                                                }
                                                #endregion
                                                #region HELP
                                                else if (_msgType.EqualsIgnoreCase("HELP"))
                                                {
                                                    try
                                                    {
                                                        string referenceNo = _message.Split(' ')[1].Trim().Split('/')[0].Trim();
                                                        string dateOfIncident = _message.Split(' ')[1].Trim().Split('/')[1].Trim();
                                                        string code = _message.Split(' ')[1].Trim().Split('/')[2].Trim();

                                                        insertHelp(_senderNbr, referenceNo, dateOfIncident, code, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "PENDING");
                                                        loadHelpDataGrid();
                                                        sendMsg(3, "Your concern/inquiry has been received. You will be contacted by one of our representatives.", _senderNbr);
                                                    }
                                                    catch
                                                    {
                                                        sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                                    }
                                                }
                                                #endregion
                                                #region CHANGE PASSWORD
                                                else if (_msgType.EqualsIgnoreCase("CPW"))
                                                {
                                                    try
                                                    {
                                                        string[] _msgCPW = _message.Split(' ');
                                                        string[] tmp = _msgCPW[1].Split('/');
                                                        var user = db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + _senderNbr + "'").ValueString("_USERNAME", 0);
                                                        if (tmp[0].EqualsIgnoreCase(user))
                                                        {
                                                            if (usernameExists(tmp[0]))
                                                            {
                                                                if (passwordCorrect(tmp[0], tmp[1]))
                                                                {
                                                                    if ((tmp[2].Length == 5) && (Regex.IsMatch(tmp[2], @"^[0-9]*$")))
                                                                    {
                                                                        db.Query("UPDATE tbl_users SET _PASSWORD = '" + tmp[2] + "' WHERE _USERNAME = '" + tmp[0] + "' AND _PASSWORD = '" + tmp[1] + "'");
                                                                        sendMsg(3, "Change password successfull. Never share your username and password to anyone.", _senderNbr);
                                                                    }
                                                                    else
                                                                        sendMsg(3, "New password must be exactly 5 numerical digits.", _senderNbr);
                                                                }
                                                                else
                                                                    sendMsg(3, "Invalid old password.", _senderNbr);
                                                            }
                                                            else
                                                                sendMsg(3, "Invalid Username. Please make sure you entered the correct username.", _senderNbr);
                                                        }
                                                        else
                                                            sendMsg(3, "Invalid username or changing password can only be done using the registered number.", _senderNbr);
                                                    }
                                                    catch
                                                    {
                                                        sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                                    }
                                                }
                                                #endregion
                                                else
                                                    sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                            }
                                            catch
                                            {
                                                sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                                            }
                                        }
                                    }
                                    #region IS NOT REGISTERED
                                    else
                                    {
                                        try
                                        {
                                            string[] _messageSplit = new string[0];
                                            _messageSplit = _message.Split(' ');
                                            var _user = _messageSplit[2].Trim();
                                            var _pass = _messageSplit[3].Trim();
                                            string refno = InputHelper.GenRefNo;

                                            if (_messageSplit.Length == 4)
                                            {
                                                if (usernameExists(_user))
                                                {
                                                    int attempts = db.QueryDT("SELECT _ATTEMPTS FROM tbl_users WHERE _USERNAME = '" + _user + "'").ValueInt("_ATTEMPTS", 0);
                                                    if (passwordCorrect(_user, _pass))
                                                    {
                                                        if (attempts >= 4)
                                                            sendMsg(3, "Your account has been blocked from loading via other sim. Please contact us, for password recovery.", _senderNbr);
                                                        else
                                                        {
                                                            var _msgType = _messageSplit[0].Trim();
                                                            #region NORMAL LOAD
                                                            if (Regex.IsMatch(_msgType, @"^[0-9]+$"))
                                                            {
                                                                string[] _msgLoad = _message.Split(' ');
                                                                string _loadCarrier = identifyCarrier(_msgLoad[0]);
                                                                bool loadOK = false;
                                                                string loadAMT = "";
                                                                string accountNo = db.QueryDT("SELECT * FROM tbl_users WHERE _USERNAME = '" + _user + "'").ValueString("_PHONE", 0);

                                                                //CHECK IF THERE'S SAME MESSAGE 30 MINS AGO
                                                                if (!has30minsText(_message.Trim(), _senderNbr))
                                                                {
                                                                    //GET THE LIST OF AVAILABLE CODE DEPENDING ON THE CARRIER!
                                                                    DataRow[] _tempCode = new DataRow[0];
                                                                    if (_carrier.Contains("/"))
                                                                    {
                                                                        string[] _tempCarrier = _carrier.Split('/');
                                                                        _tempCode = dtCodes.Select("_CARRIER = '" + _tempCarrier[0] + "' OR _CARRIER = '" + _tempCarrier[1] + "' OR _CARRIER = '" + _loadCarrier + "'", "");
                                                                    }
                                                                    else
                                                                    {
                                                                        _tempCode = dtCodes.Select("_CARRIER = '" + _loadCarrier + "'", "");
                                                                    }

                                                                    foreach (DataRow dr in _tempCode)
                                                                    {
                                                                        if (dr[2].ToString() == _msgLoad[1].Trim())
                                                                        {
                                                                            loadAMT = dr[3].ToString();
                                                                            if (hasLoadBalance(accountNo, decimal.Parse(loadAMT)))
                                                                            {
                                                                                //INSERT MESSAGE AS PENDING FIRST
                                                                                insertPending(_senderNbr,
                                                                                    dr["_CARRIER"].ToString(),
                                                                                    _msgLoad[0],
                                                                                    dr[3].ToString(),
                                                                                    _message,
                                                                                    _dateTime,
                                                                                    refno);

                                                                                string[] _tempCarrier = new string[0];
                                                                                //TIME TO LOAD
                                                                                #region GLOBE LOAD
                                                                                if (_loadCarrier.Contains("/") || _loadCarrier.Contains("Globe"))
                                                                                {
                                                                                    if (globeActive)
                                                                                    {
                                                                                        _tempCarrier = _loadCarrier.Split('/');
                                                                                        for (int i = 0; i < _tempCarrier.Length; i++)
                                                                                        {
                                                                                            try
                                                                                            {
                                                                                                var _tmpUSSDCode = dtUSSDCode.Select("_CARRIER LIKE '%" + _tempCarrier[i] + "' OR _CARRIER LIKE '" + _tempCarrier[i] + "%' OR _CARRIER = '" + _loadCarrier + "'", "");
                                                                                                if (_tmpUSSDCode.Length > 0)
                                                                                                {
                                                                                                    loadOK = true;
                                                                                                    sendUSSD(7, _tmpUSSDCode[0]["_CODE"].ToString());
                                                                                                    loadQueue.Add(dr[6].ToString() + _msgLoad[0]);
                                                                                                    break;
                                                                                                }
                                                                                            }
                                                                                            catch{ }
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        sendMsg(3, "Sorry, GLOBE transactions are currently unavailable as of this moment. Please try again later.", _senderNbr);
                                                                                        break;
                                                                                    }
                                                                                }
                                                                                #endregion
                                                                                #region SMART LOAD
                                                                                else if (_loadCarrier.EqualsIgnoreCase("Smart"))
                                                                                {
                                                                                    if (smartActive)
                                                                                    {
                                                                                        loadOK = true;
                                                                                        if (dr[2].ToString().Contains('P')) {
                                                                                            sendMsg(6, "Load " + dr[2].ToString().Split('P')[1].Trim() + " " + _msgLoad[0].Trim(), "343");
                                                                                        }
                                                                                        else {
                                                                                            sendMsg(6, "Load " + dr[2].ToString() + " " + _msgLoad[0].Trim(), "343");
                                                                                        }
                                                                                        break;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        sendMsg(3, "Sorry, SMART transactions are currently unavailable as of this moment. Please try again later.", _senderNbr);
                                                                                        break;
                                                                                    }
                                                                                }
                                                                                #endregion
                                                                                #region TNT LOAD
                                                                                else if (_loadCarrier.EqualsIgnoreCase("TNT"))
                                                                                {
                                                                                    if (smartActive)
                                                                                    {
                                                                                        loadOK = true;
                                                                                        if (dr[2].ToString().Contains('P')) {
                                                                                            sendMsg(6, "Load " + dr[2].ToString().Split('P')[1].Trim() + " " + _msgLoad[0], "4540");
                                                                                        }
                                                                                        else {
                                                                                            sendMsg(6, "Load " + dr[2].ToString() + " " + _msgLoad[0], "4540");
                                                                                        }
                                                                                        break;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        sendMsg(3, "Sorry, SMART transactions are currently unavailable as of this moment. Please try again later.", _senderNbr);
                                                                                        break;
                                                                                    }
                                                                                }
                                                                                #endregion
                                                                            }
                                                                            else
                                                                            {
                                                                                sendMsg(3, "Load Failed! You don't have enough load balance to make this transaction.", _senderNbr);
                                                                                break;
                                                                            }
                                                                        }
                                                                    }

                                                                    if (loadOK)
                                                                    {
                                                                        string accountNumber = db.QueryDT("SELECT * FROM tbl_users WHERE _USERNAME = '" + _messageSplit[2] + "'").ValueString("_PHONE", 0);
                                                                        deductLoad(accountNumber, (int.Parse(loadAMT) - (int.Parse(loadAMT) * percentLoad)).ToString());
                                                                        recordIncome(accountNumber, (decimal.Parse(loadAMT) * percentLoad), "LOAD", refno, DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
                                                                        var acctINFO = accountInfo(_messageSplit[0]);
                                                                        Thread.Sleep(500);
                                                                        sendMsg(3, "Load Success!" + loadAMT + "(" + (int.Parse(loadAMT) - (int.Parse(loadAMT) * percentLoad)).ToString() + ") has been loaded to " + _messageSplit[0] + "." + Environment.NewLine + "Curr bal: " + acctINFO[0].toFinancial() + Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                                        Thread.Sleep(500);
                                                                        sendMsg(3, "Ref #: " + refno + Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                                        Thread.Sleep(500);
                                                                        sendMsg(3, loadAMT + "(" + (int.Parse(loadAMT) - (int.Parse(loadAMT) * percentLoad)).ToString() + ") has been loaded to " + _messageSplit[0] + "." + Environment.NewLine + "New load Balance: " + acctINFO[0].toFinancial() + " " + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), accountNumber);
                                                                        Thread.Sleep(500);
                                                                        sendMsg(3, "Ref #: " + refno + " " + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), accountNumber);
                                                                    }
                                                                    else {
                                                                        sendMsg(3, "Load Failed. Invalid product code, please use the correct product code. (1655)", _senderNbr);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    sendMsg(3, "Duplicate Load Detected! To intentionally repeat load place \"REP<SPACE>\" before the number.", _senderNbr);
                                                                }
                                                            }
                                                            #endregion
                                                            #region REPEATING MESSAGE
                                                            else if (_msgType.EqualsIgnoreCase("REP"))
                                                            {
                                                                string[] _msgLoad = _message.Split(' ');
                                                                string _loadCarrier = identifyCarrier(_msgLoad[1]);
                                                                bool loadOK = false;
                                                                string loadAMT = "";

                                                                //GET THE LIST OF AVAILABLE CODE DEPENDING ON THE CARRIER!
                                                                DataRow[] _tempCode = new DataRow[0];
                                                                if (_carrier.Contains("/"))
                                                                {
                                                                    string[] _tempCarrier = _carrier.Split('/');
                                                                    _tempCode = dtCodes.Select("_CARRIER = '" + _tempCarrier[0] + "' OR _CARRIER = '" + _tempCarrier[1] + "' OR _CARRIER = '" + _loadCarrier + "'", "");
                                                                }
                                                                else
                                                                {
                                                                    _tempCode = dtCodes.Select("_CARRIER = '" + _loadCarrier + "'", "");
                                                                }

                                                                foreach (DataRow dr in _tempCode)
                                                                {
                                                                    if (dr[2].ToString() == _msgLoad[2].Trim())
                                                                    {
                                                                        loadAMT = dr[3].ToString();
                                                                        if (hasLoadBalance(_senderNbr, decimal.Parse(loadAMT)))
                                                                        {
                                                                            //INSERT MESSAGE AS PENDING FIRST
                                                                            insertPending(_senderNbr,
                                                                                dr["_CARRIER"].ToString(),
                                                                                _msgLoad[1],
                                                                                dr[3].ToString(),
                                                                                _message,
                                                                                _dateTime,
                                                                                refno);

                                                                            string[] _tempCarrier = new string[0];
                                                                            //TIME TO LOAD
                                                                            #region GLOBE LOAD
                                                                            if (_loadCarrier.Contains("/") || _loadCarrier.Contains("Globe"))
                                                                            {
                                                                                if (globeActive)
                                                                                {
                                                                                    _tempCarrier = _loadCarrier.Split('/');
                                                                                    for (int i = 0; i < _tempCarrier.Length; i++)
                                                                                    {
                                                                                        try
                                                                                        {
                                                                                            var _tmpUSSDCode = dtUSSDCode.Select("_CARRIER LIKE '%" + _tempCarrier[i] + "' OR _CARRIER LIKE '" + _tempCarrier[i] + "%' OR _CARRIER = '" + _loadCarrier + "'", "");
                                                                                            if (_tmpUSSDCode.Length > 0)
                                                                                            {
                                                                                                loadOK = true;
                                                                                                sendUSSD(7, _tmpUSSDCode[0]["_CODE"].ToString());
                                                                                                loadQueue.Add(dr[6].ToString() + _msgLoad[0]);
                                                                                                break;
                                                                                            }
                                                                                        }
                                                                                        catch { }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    sendMsg(3, "Sorry, GLOBE transactions are currently unavailable as of this moment. Please try again later.", _senderNbr);
                                                                                    break;
                                                                                }
                                                                            }
                                                                            #endregion
                                                                            #region SMART LOAD
                                                                            else if (_loadCarrier.EqualsIgnoreCase("Smart"))
                                                                            {
                                                                                if (smartActive)
                                                                                {
                                                                                    loadOK = true;
                                                                                    if (dr[2].ToString().Contains('P')) {
                                                                                        sendMsg(6, "Load " + dr[2].ToString().Split('P')[1].Trim() + " " + _msgLoad[1], "343");
                                                                                    }
                                                                                    else {
                                                                                        sendMsg(6, "Load " + dr[2].ToString() + " " + _msgLoad[1], "343");
                                                                                    }
                                                                                    break;
                                                                                }
                                                                                else
                                                                                {
                                                                                    sendMsg(3, "Sorry, SMART transactions are currently unavailable as of this moment. Please try again later.", _senderNbr);
                                                                                    break;
                                                                                }
                                                                            }
                                                                            #endregion
                                                                            #region TNT LOAD
                                                                            else if (_loadCarrier.EqualsIgnoreCase("TNT"))
                                                                            {
                                                                                if (smartActive)
                                                                                {
                                                                                    loadOK = true;
                                                                                    if (dr[2].ToString().Contains('P')) {
                                                                                        sendMsg(6, "Load " + dr[2].ToString().Split('P')[1].Trim() + " " + _msgLoad[1], "4540");
                                                                                    }
                                                                                    else {
                                                                                        sendMsg(6, "Load " + dr[2].ToString() + " " + _msgLoad[1], "4540");
                                                                                    }
                                                                                    break;
                                                                                }
                                                                                else
                                                                                {
                                                                                    sendMsg(3, "Sorry, SMART transactions are currently unavailable as of this moment. Please try again later.", _senderNbr);
                                                                                    break;
                                                                                }
                                                                            }
                                                                            #endregion
                                                                        }
                                                                        else
                                                                        {
                                                                            sendMsg(3, "Load Failed! You don't have enough load balance to make this transaction.", _senderNbr);
                                                                            break;
                                                                        }
                                                                    }
                                                                }

                                                                if (loadOK)
                                                                {
                                                                    
                                                                    deductLoad(_senderNbr, loadAMT);
                                                                    recordIncome(_senderNbr, (decimal.Parse(loadAMT) * percentLoad), "LOAD", refno, DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
                                                                    var acctINFO = accountInfo(_senderNbr);
                                                                    Thread.Sleep(1000);
                                                                    sendMsg(3, "Load Success! You have issued " + loadAMT + "(" + (decimal.Parse(loadAMT)*(1 - percentLoad)).ToString() + ") Cust #: " + _msgLoad[1] + Environment.NewLine + "Curr bal: " + acctINFO[0].toFinancial() + Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                                    Thread.Sleep(500);
                                                                    sendMsg(3, "Ref #: " + refno + Environment.NewLine + Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"), _senderNbr);
                                                                }
                                                                else {
                                                                    sendMsg(3, "Load Failed. Invalid product code, please use the correct product code. 1783", _senderNbr);
                                                                }
                                                            }
                                                            #endregion
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (attempts < 4 && attempts >= 0){
                                                            db.Query("UPDATE tbl_users SET _ATTEMPTS = _ATTEMPTS + 1 WHERE _USERNAME = '" + _user + "'");
                                                            sendMsg(3, "Invalid Password. You have " + (4 - attempts).ToString() + " attempt(s) remaining. (P50 Password Recovery Fee with valid I.D.)", _senderNbr);
                                                        }
                                                        else if (attempts >= 4)
                                                            sendMsg(3, "Your account has been blocked from loading via other sim. Please contact us, for password recovery.", _senderNbr);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        });
                        Thread.Sleep(1000);
                    } catch { }
                }
            })));
            recMsgThread[recMsgThread.Count - 1].Start();
        }

        //THIS IS USED FOR RETAILER SIM
        void recMsgUSSD(int index)
        {
            recMsgUSSDThread.Add(new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    _serialPort[index].NewLine = Environment.NewLine;

                    _serialPort[index].Write("AT\r\n");
                    Thread.Sleep(100);
                    _serialPort[index].Write("AT+CMGF=1\r\n");
                    Thread.Sleep(100);
                    _serialPort[index].Write("AT+CMGL=\"ALL\"\r\n");
                    Thread.Sleep(1000);

                    string loadmessage = "";
                    string loadnumber = "";

                    if (index == 6)
                    {
                        try
                        {
                            var tmp = smartLoad[0];
                            loadmessage = tmp[0];
                            loadnumber = tmp[1];
                            smartLoad.RemoveAt(0);
                        }
                        catch { }

                        if (loadmessage != "" && loadnumber != "")
                        {
                            Thread.Sleep(5000);

                            _serialPort[index].NewLine = Environment.NewLine;

                            _serialPort[index].Write("AT\r\n");

                            Thread.Sleep(100);

                            _serialPort[index].Write("AT+CMGF=1\r");

                            Thread.Sleep(100);

                            _serialPort[index].Write("AT+CMGS=\"" + loadnumber + "\"\r\n");

                            Thread.Sleep(100);

                            _serialPort[index].Write(loadmessage + "\x1A");

                            Thread.Sleep(1000);
                        }
                    }

                    this.Invoke((MethodInvoker)delegate
                    {
                        //***********MESSAGE RETRIEVAL AREA*************
                        string existing = _serialPort[index].ReadExisting();

                        #region MESSAGE HANDLING
                        if (existing.Contains("+CMT: "))
                        {
                            string[] _dat = existing.Split(new string[] { "+CMT: " }, StringSplitOptions.None);

                            lblStatus.Text = "DATA:\r\n" + _dat[1];

                            string[] tmpResponse = _dat[1].Split(
                                new[] { Environment.NewLine },
                                StringSplitOptions.None
                            );

                            //ALL NEEDED VARIABLES
                            string[] tmpSplit = tmpResponse[0].Replace("\"", "").Split(',');
                            string _senderNbr = tmpSplit[0].Replace("+63", "0");
                            string _dateTime = tmpSplit[2].Split('+')[0].Replace("/", "-");
                            string _message = tmpResponse[1].Trim();
                            string _carrier = "";

                            var _tempDR = dtSysNo.Select("_SYSNO = '" + _senderNbr + "'", "");

                            foreach (DataRow dr in _tempDR)
                                _carrier = dr[1].ToString();

                            //****************************RESPONSE HANDLING****************************

                            #region GLOBE/TM response
                            if (_carrier.EqualsIgnoreCase("Globe/TM"))
                            {
                                #region AutoLoadMAX response
                                if (_senderNbr.EqualsIgnoreCase("AutoLoadMAX"))
                                {
                                    try {
                                        string _loadSender = "";
                                        string _loadNbr = "";
                                        string _trcNo = "";
                                        string[] _tmpMsg = _message.Split(' ');

                                        _loadNbr = _tmpMsg[8].Replace(".", "").Trim();
                                        _trcNo = _tmpMsg[15].Split(':')[1].Trim();

                                        //READ MESSAGE HERE! TO BE CODED
                                        Thread.Sleep(1000);
                                        var _tempPending = db.QueryDT("SELECT * FROM tbl_pending WHERE _PHONE = '" + _loadNbr + "' ORDER BY _DATETIME DESC");
                                        Thread.Sleep(1000);
                                        _loadSender = _tempPending.ValueString("_SENDER", 0);

                                        var tmpDT = dtSysNo.Select("_SYSNO = '" + _senderNbr + "'", "");
                                        insertMessage(_senderNbr, tmpDT[0]["_CARRIER"].ToString(), _message, _dateTime, _tempPending.ValueString("_REFNO", 0));

                                        moveToHistory(
                                            _loadSender,
                                            tmpDT[0]["_CARRIER"].ToString(),
                                            _loadNbr,
                                            _tempPending.ValueString("_CREDIT", 0),
                                            _tempPending.ValueString("_MESSAGE", 0),
                                            _tempPending.ValueString("_REFNO", 0),
                                            _trcNo,
                                            _dateTime);

                                        loadDataGrid();
                                    }
                                    catch {

                                    }
                                }
                                #endregion
                                else
                                {
                                    var tmpDT = dtSysNo.Select("_SYSNO = '" + _senderNbr + "'", "");
                                    insertMessage(_senderNbr, tmpDT[0]["_CARRIER"].ToString(), _message, _dateTime, "");
                                    loadDataGrid();
                                }
                            }
                            #endregion
                            #region SMART/TNT response
                            else
                            {
                                #region SMARTLoad response
                                if (_senderNbr.EqualsIgnoreCase("SMARTLoad"))
                                {
                                    string _loadSender = "";
                                    string _loadNbr = "";
                                    string _trcNo = "";
                                    string[] _tmpMsg = _message.Split(' ');

                                    _loadNbr = "0" + _tmpMsg[6].Replace(".", "").Trim().Remove(0, 2);

                                    try
                                    {
                                        _trcNo = _tmpMsg[10].Split(':')[2].Trim();
                                    }
                                    catch { }

                                    //READ MESSAGE HERE! TO BE CODED
                                    var _tempPending = db.QueryDT("SELECT * FROM tbl_pending WHERE _PHONE = '" + _loadNbr + "' ORDER BY _DATETIME ASC");

                                    
                                    _loadSender = _tempPending.ValueString("_SENDER", 0);

                                    var tmpDT = dtSysNo.Select("_SYSNO = '" + _senderNbr + "'", "");
                                    insertMessage(_senderNbr, tmpDT[0]["_CARRIER"].ToString(), _message, _dateTime, _tempPending.ValueString("_REFNO", 0));
                                   
                                    moveToHistory(
                                        _loadSender,
                                        tmpDT[0]["_CARRIER"].ToString(),
                                        _loadNbr,
                                        _tempPending.ValueString("_CREDIT", 0),
                                        _tempPending.ValueString("_MESSAGE", 0),
                                        _tempPending.ValueString("_REFNO", 0),
                                        _trcNo,
                                        _dateTime);

                                    loadDataGrid();
                                }
                                #endregion
                                else
                                {
                                    var tmpDT = dtSysNo.Select("_SYSNO = '" + _senderNbr + "'", "");
                                    insertMessage(_senderNbr, tmpDT[0]["_CARRIER"].ToString(), _message, _dateTime, "");
                                    loadDataGrid();
                                }
                            }
                            #endregion
                        }
                        #endregion

                        //sendUSSD(7, "*100#");

                        #region USSD REPLY HANDLING
                        if (existing.Contains("+CUSD: "))
                        {
                            string[] _dat = existing.Split(new string[] { "+CUSD: " }, StringSplitOptions.None);

                            if (loadQueue.Count > 0)
                            {
                                string[] tmpArr = loadQueue[0].Split(';');

                                if (_dat[1].Contains("Mobile Number")) {
                                    sendUSSD(index, tmpArr[3]);
                                }
                                else if (_dat[1].Contains("denomination")) {
                                    sendUSSD(index, tmpArr[1]);
                                }
                                else if (_dat[1].Contains("Load " + tmpArr[1] + " to " + tmpArr[3])) {
                                    sendUSSD(index, tmpArr[2]);
                                    loadQueue.RemoveAt(0);
                                }
                                else {
                                    sendUSSD(index, tmpArr[0]);
                                }
                            }
                        }
                        #endregion
                    });
                    Thread.Sleep(1000);
                }
            })));
            recMsgUSSDThread[recMsgUSSDThread.Count - 1].Start();
        }

        //INITIALIZE CODE LIST AND ALL CARRIER
        void initDatabase()
        {
            dtCodes = new DataTable();
            dtCarrier = new DataTable();
            dtSysNo = new DataTable();
            dtUSSDCode = new DataTable();
            dtRoles = new DataTable();
            dtConfig = new DataTable(); 

            dtCodes = db.QueryDT("SELECT * FROM tbl_codes");
            dtCarrier = db.QueryDT("SELECT * FROM tbl_carrier");
            dtSysNo = db.QueryDT("SELECT * FROM tbl_sysno");
            dtUSSDCode = db.QueryDT("SELECT * FROM tbl_ussdcode");
            dtRoles = db.QueryDT("SELECT * FROM tbl_role");
            dtConfig = db.QueryDT("SELECT * FROM tbl_config");

            percentLoad = dtConfig.ValueDecimal("_VALUE", 0);
        }

        //GET ALL MESSAGE LIST
        void loadDataGrid()
        {
            dgvSMS.Rows.Clear();
            using (DataTable dt = db.QueryDT("SELECT * FROM tbl_messages WHERE _DATETIME LIKE '"+ DateTime.Now.ToString("yyyy-MM-dd")+"%'"))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                    dgvSMS.Rows.Add(dt.ValueString(0, i),
                        dt.ValueString(1, i),
                        dt.ValueString(2, i),
                        dt.ValueString(3, i),
                        dt.ValueString(4, i),
                        dt.ValueString(6, i));
            }
            try
            {
                dgvSMS.FirstDisplayedScrollingRowIndex = dgvSMS.RowCount - 1;
            }
            catch { }
        }

        //GET ALL HELP MESSAGES
        void loadHelpDataGrid()
        {
            dgvHelp.Rows.Clear();
            using (DataTable dt = db.QueryDT("SELECT * FROM tbl_help ORDER BY FIELD(_STATUS, 'PENDING', 'RESOLVED', 'FAILED'), _DATETIME DESC"))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                    dgvHelp.Rows.Add(dt.ValueString(0, i),
                        dt.ValueString(1, i),
                        dt.ValueString(2, i),
                        dt.ValueString(3, i),
                        dt.ValueString(4, i));
            }
        }

        //SEND USSD CODE ex: *100#
        void sendUSSD(int index, string code)
        {
            //Replace "COM7"withcorresponding port name

            //_serialPort[index] = new SerialPort(coms[index], 115200);

            Thread.Sleep(100);

            _serialPort[index].Write("AT\r\n");

            Thread.Sleep(100);

            _serialPort[index].Write("AT+CUSD=1,\"" + code + "\",15\r\n");

            Thread.Sleep(100);

            //_serialPort[index].Close();
            
        }

        //SEND MESSAGE
        void sendMsg(int index, string message, string number)
        {
            if (index == 2)
                msgList1.Add(new string[]{ message, number });
            else if (index == 3)
                msgList2.Add(new string[] { message, number });
            else if (index == 6)
                smartLoad.Add(new string[] { message, number });
        }

        //PROCESS MESSAGE SENDING
        void processMsgs(int index)
        {
            processMsgThread.Add(new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    string message = "";
                    string number = "";

                    if (index == 2)
                    {
                        try
                        {
                            var tmp = msgList1[0];
                            message = tmp[0];
                            number = tmp[1];
                            msgList1.RemoveAt(0);
                        }
                        catch { }
                    }
                    else if (index == 3)
                    {
                        try
                        {
                            var tmp = msgList2[0];
                            message = tmp[0];
                            number = tmp[1];
                            msgList2.RemoveAt(0);
                        }
                        catch { }
                    }

                    if (message != "" && number != "")
                    {
                        Thread.Sleep(5000);

                        _serialPort[index].NewLine = Environment.NewLine;

                        _serialPort[index].Write("AT\r\n");

                        Thread.Sleep(100);

                        _serialPort[index].Write("AT+CMGF=1\r");

                        Thread.Sleep(100);

                        _serialPort[index].Write("AT+CMGS=\"" + number + "\"\r\n");

                        Thread.Sleep(100);

                        _serialPort[index].Write(message + "\x1A");

                        Thread.Sleep(1000);
                        //***********MESSAGE RETRIEVAL AREA*************
                        string existing = _serialPort[index].ReadExisting();

                        this.Invoke((MethodInvoker)delegate
                        {
                            lblStatus.Text = "DATA: \r\n\r\n" + existing;
                        });
                    }
                }
            })));
            processMsgThread[processMsgThread.Count - 1].Start();
        }

        //INITALIZE CONFIGURAITON
        void initConfig()
        {
            config = new List<string>();
            DataTable dtTemp = db.QueryDT("SELECT * FROM tbl_config");
            for (int i = 0; i < dtTemp.RowCount(); i++)
                config.Add(dtTemp.ValueString(0, i));
        }

        //IDENTIFY CARRIER
        string identifyCarrier(string _nbr)
        {
            string output = "";
            try
            {
                var _tempDR = dtCarrier.Select("_PREFIX LIKE '" + _nbr.Substring(1, 4) + "%'", "");

                if (_tempDR.Count() == 0)
                    _tempDR = dtCarrier.Select("_PREFIX LIKE '" + _nbr.Substring(1, 3) + "%'", "");

                foreach (DataRow dr in _tempDR)
                {
                    output = dr[1].ToString();
                    break;
                }
            }
            catch { }
            return output;
        }

        bool isValidCode(string code, string type)
        {
            DataTable dt = db.QueryDT("SELECT * FROM tbl_act_code WHERE _CODE = '" + code.ToUpper() + "' AND _TYPE = '" + type + "' AND _USED = 'NO'");
            return dt.RowCount() == 1;
        }

        //CHECK IF THE PHONE NUMBER CAN REGISTER
        bool canRegister(string texterNbr, string msgType)
        {
            bool output = false;
            var user = db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + texterNbr + "'");
            int index = 0;

            for (int i = 0; i < regComm.Length; i++)
                if (regComm[i].EqualsIgnoreCase(msgType))
                {
                    index = i;
                    break;
                }

            if (user.RowCount() == 1)
            {
                var userRole = dtRoles.Select("_ROLE = '" + user.ValueString("_ROLE", 0) + "'", "")[0]["_LEVEL"].ToString();
                output = index <= int.Parse(userRole);
            }
            return output;
        }

        //CHECK IF THE ACCOUNT LEVEL IS LESSER THAN OR EQUAL
        bool isLessEqual(string texterNbr, string trnsfrNbr)
        {
            bool output = false;
            var texter = db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + texterNbr + "'");
            var trnsfr = db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + trnsfrNbr + "'");

            if (texter.RowCount() == 1 && trnsfr.RowCount() == 1)
            {
                var texterRole = dtRoles.Select("_ROLE = '" + texter.ValueString("_ROLE", 0) + "'", "")[0]["_LEVEL"].ToString();
                var trnsfrRole = dtRoles.Select("_ROLE = '" + trnsfr.ValueString("_ROLE", 0) + "'", "")[0]["_LEVEL"].ToString();
                
                output = int.Parse(trnsfrRole) <= int.Parse(texterRole);
            }
            return output;
        }

        //CHECK IF THE NUMBER IS REGISTERED
        bool userExists(string texterNbr)
        {
            return db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + texterNbr + "'").RowCount() == 1;
        }

        //CHECK IF THE NUMBER IS REGISTERED
        bool usernameExists(string user)
        {
            return db.QueryDT("SELECT * FROM tbl_users WHERE _USERNAME = '" + user + "'").RowCount() == 1;
        }

        //CHECK IF THE PASSWORD IS CORRECT
        bool passwordCorrect(string user, string pass)
        {
            return db.QueryDT("SELECT * FROM tbl_users WHERE _USERNAME = '" + user + "' AND _PASSWORD = '" + pass + "'").RowCount() == 1;
        }

        //CHECK IF HAS SAME TEXT MESSAGE 30 MINS AGO
        bool has30minsText(string msg, string sender)
        {
            int count1 = db.QueryDT("SELECT * FROM tbl_history WHERE _DATETIME >= NOW() - INTERVAL 30 MINUTE AND _MESSAGE = '" + msg + "' AND _SENDER = '" + sender + "'").RowCount();
            int count2 = db.QueryDT("SELECT * FROM tbl_pending WHERE _DATETIME >= NOW() - INTERVAL 30 MINUTE AND _MESSAGE = '" + msg + "' AND _SENDER = '" + sender + "'").RowCount();
            return count1 != 0 || count2 != 0;
        }

        //CHECK IF HAS TLC TRANSACTION 30 MINS AGO
        bool has30minsTLC(string sender, string amount, string receiver)
        {
            int count1 = db.QueryDT("SELECT * FROM tbl_tlc WHERE _DATETIME >= NOW() - INTERVAL 30 MINUTE AND _SENDER = '" + sender + "' AND _AMOUNT = '" + amount + "' AND _RECEIVER = '" + receiver + "'").RowCount();
            return count1 != 0;
        }

        //CHECK IF HAS PINS TRANSFER 30 MINS AGO
        bool has30minsPINS(string sender, string receiver, string amount)
        {
            int count1 = db.QueryDT("SELECT * FROM tbl_pin WHERE _DATETIME >= NOW() - INTERVAL 30 MINUTE AND _SENDER = '" + sender + "' AND _RECEIVER = '" + receiver + "' AND _PIN_AMT = '" + amount + "'").RowCount();
            return count1 != 0;
        }

        string[] accountInfo(string nbr)
        {
            var acct = db.QueryDT("SELECT * FROM tbl_account WHERE _PHONE = '" + nbr + "'");
            return new string[] { acct.ValueString("_BALANCE", 0), acct.ValueString("_PINS", 0) };
        }

        //PROCESS LOAD, DEDUCT FROM USER
        void deductLoad(string userNbr, string amount)
        {
            decimal decAMT = decimal.Parse(amount);
            string finalAMT = (decAMT - (decAMT * percentLoad)).ToString();
            db.Query("UPDATE tbl_account SET _BALANCE = _BALANCE - " + finalAMT + " WHERE _PHONE = '" + userNbr + "'");
        }

        //PROCESS PIN, DEDUCT FROM USER
        void deductPINS(string userNbr, string amount)
        {
            db.Query("UPDATE tbl_account SET _PINS = _PINS - '" + amount + "' WHERE _PHONE = '" + userNbr + "'");
        }

        //CHECK LOAD BALANCE LEFT
        bool hasLoadBalance(string userNbr, decimal amount)
        {
            decimal fnlAmt = amount - (amount * percentLoad);
            decimal bal = db.QueryDT("SELECT * FROM tbl_account WHERE _PHONE = '" + userNbr + "'").ValueDecimal("_BALANCE", 0);

            return bal >= fnlAmt;
        }

        //CHECK PINS LEFT
        bool hasPINS(string userNbr, int amount)
        {
            decimal bal = db.QueryDT("SELECT * FROM tbl_account WHERE _PHONE = '" + userNbr + "'").ValueInt("_PINS", 0);
            return bal >= amount;
        }

        //CHECK ACTIVE
        void checkActiveSIM()
        {
            var data = db.QueryDT("SELECT * FROM tbl_config");
            globeActive = data.ValueInt("_VALUE", 2) == 1;
            smartActive = data.ValueInt("_VALUE", 3) == 1;
            lblGlobe.Text = globeActive.ToString().ToUpper();
            lblSmart.Text = smartActive.ToString().ToUpper();
        }

        //USER REGISTRATION
        void regUser(string _message, string _senderNbr, string _carrier, string _dateTime, string _position, string _bal, string _pins)
        {
            string[] _msg = _message.Split('/');
            //RETAILER
            if (_msg.Length == 6) {
                
                try {
                    
                    string _nbr = _msg[0].Split(' ')[1];
                    string _pass = _msg[1];
                    string _fullname = _msg[2];
                    string _username = _msg[3];
                    string _bdate = _msg[4];
                    string _address = _msg[5];

                    if (!_username.All(char.IsLetterOrDigit) || _pass.Length != 5 || _nbr.Length != 11 || _fullname.Length < 4 || _username.Length < 4)
                        throw new Exception();

                    var inserted = insertUser(_nbr, _pass, _fullname, _username, _bdate, _address, _position, _bal, _pins);

                    if (inserted) {
                        if (_position == "RETAILER") {
                            deductPINS(_senderNbr, "1");
                            var acctINFO = accountInfo(_senderNbr);
                            sendMsg(3, _nbr + " has been registered as Techno User. Remaining TU Pins: " + acctINFO[1], _senderNbr);
                            Thread.Sleep(1000);
                            sendMsg(3, "Congratulations! You are now registerd as Techno User. With USERNAME: " + _username + " and PW: " + _pass + ". NEVER share this information to anyone. You may refer to the product guide for transactions.", _nbr);
                        }
                        string referenceNumber = InputHelper.GenRefNo;

                        insertMessage(_senderNbr, _carrier, _message, _dateTime, referenceNumber);
                        loadDataGrid();
                        insertActivatedRetailer(_senderNbr, _username, _nbr, _fullname, "0", referenceNumber);
                    }
                    else
                        sendMsg(3, "Registration failed! Username already exists!", _senderNbr);
                }
                catch (Exception e){
                    sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                }
            }
            //WITH ACTIVATION CODE
            else if (_msg.Length == 7) {
                try {
                    string _nbr = _msg[0].Split(' ')[1];
                    string _code = _msg[1];
                    string _pass = _msg[2];
                    string _fullname = _msg[3];
                    string _username = _msg[4];
                    string _bdate = _msg[5];
                    string _address = _msg[6];

                    if (isValidCode(_code, _position)) {
                        if (!_username.All(char.IsLetterOrDigit) || _pass.Length != 5 || _nbr.Length != 11 || _fullname.Length < 4 || _username.Length < 4)
                            throw new Exception();

                        var inserted = insertUser(_nbr, _pass, _fullname, _username, _bdate, _address, _position, _bal, _pins);
                        if (inserted) {
                            if (_position == "DISTRIBUTOR" || _position == "DEALER") {
                                deductPINS(_senderNbr, "1");
                                useCode(_code, _username);

                                sendMsg(3, "Congratulations! " + _nbr + " has been registered as " + _position + ".", _senderNbr);
                                Thread.Sleep(1000);
                                sendMsg(3, "Congratulations! You are now registerd as " + _position + ". With USERNAME: " + _username + " and PW: " + _pass + ". NEVER share this information to anyone. You now have " + _pins + " TU pins.", _nbr);
                                Thread.Sleep(1000);
                                sendMsg(3, "Final Step: Get your Product code from your sponsor to register your account on website.", _nbr);
                            }
                            else if (_position == "MOBILE" || _position == "CITY" || _position == "PROVINCIAL") {
                                deductPINS(_senderNbr, "1");
                                useCode(_code, _username);

                                sendMsg(3, "Congratulations! " + _nbr + " has been registered as " + _position + ".", _senderNbr);
                                Thread.Sleep(1000);
                                sendMsg(3, "Congratulations! You are now registerd as " + _position + ". With USERNAME: " + _username + " and PW: " + _pass + ". NEVER share this information to anyone. You now have " + _pins + " TU pins.", _nbr);
                                Thread.Sleep(1000);
                                sendMsg(3, "Final Step: Get your Product code from your sponsor to register your account on website.", _nbr);
                            }

                            string referenceNumber = InputHelper.GenRefNo;

                            insertMessage(_senderNbr, _carrier, _message, _dateTime, referenceNumber);
                            loadDataGrid();
                            insertActivatedRetailer(_senderNbr, _username, _nbr, _fullname, _pins, referenceNumber);

                        }
                        else {
                            sendMsg(3, "Registration failed! Username already exists!", _senderNbr);
                        }

                    }
                    else
                        sendMsg(3, "Registration Failed! Invalid Activation Code.", _senderNbr);
                }
                catch (Exception e) {
                    sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
                }
            }
            else
                sendMsg(3, "Invalid command. Please make sure your format is correct and your message does not exceed 160 characters.", _senderNbr);
        }

        //TRANSFER LOAD WALLET TO CERTAIN NUMBER
        void transferLoadWallet(string sender, string receiver, decimal amount)
        {
            db.Query("UPDATE tbl_account set _BALANCE = _BALANCE - " + (amount - (amount * percentLoad)).ToString() + " WHERE _PHONE = '" + sender + "'");
            db.Query("UPDATE tbl_account set _BALANCE = _BALANCE + " + amount + " WHERE _PHONE = '" + receiver + "'");
        }

        //PERCENTAGE DEDUCTIONS WHEN TRANSFERING
        decimal setPercentToBeDeducted(string sender, string receiver) {

            decimal percentLoad = 0.00M;

            string senderRole = db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + sender + "'").ValueString("_ROLE", 0).Split(' ')[0].Trim();
            string receiverRole = db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + receiver + "'").ValueString("_ROLE", 0).Split(' ')[0].Trim();

            int senderLevel = db.QueryDT("SELECT * FROM tbl_role WHERE _ROLE LIKE '%" + senderRole + "'").ValueInt("_LEVEL", 0);
            int receiverLevel = db.QueryDT("SELECT * FROM tbl_role WHERE _ROLE LIKE '%" + receiverRole + "'").ValueInt("_LEVEL", 0);

            if (senderLevel == receiverLevel) {
                percentLoad = 0.00M;
            }
            else {
                if (!userExists(receiver)) {
                    for (int i = 0; i < (senderLevel - receiverLevel); i++) {
                        percentLoad = percentLoad + db.QueryDT("SELECT * FROM tbl_percentage WHERE _LEVEL = '" + (receiverLevel + i).ToString() + "'").ValueDecimal("_PERCENTAGE", 0);
                    }
                }
                else {
                    for (int i = 1; i <= (senderLevel - receiverLevel); i++) {
                        percentLoad = percentLoad + db.QueryDT("SELECT * FROM tbl_percentage WHERE _LEVEL = '" + (receiverLevel + i).ToString() + "'").ValueDecimal("_PERCENTAGE", 0);
                    }
                }
            }
            return percentLoad;
        }

        //CHECK IF CODE IS CORRECT
        bool isCodeElligible(string senderNumber, string activationCode) {
            bool codeCorrect = false;

            string codeRoleType = db.QueryDT("SELECT * FROM tbl_act_code WHERE _CODE = '" + activationCode + "'").ValueString("_TYPE", 0);
            int codeLevel = db.QueryDT("SELECT * FROM tbl_percentage WHERE _LEVEL = '" + codeRoleType + "'").ValueInt("_LEVEL", 0);

            string senderRole = db.QueryDT("SELECT * FROM tbl_users WHERE _PHONE = '" + senderNumber + "'").ValueString("_ROLE", 0);
            int senderLevel = db.QueryDT("SELECT * FROM tbl_percentage WHERE _LEVEL = '" + senderRole + "'").ValueInt("_LEVEL", 0);

            bool levelOk = senderLevel >= codeLevel;
            bool codeUsed = db.QueryDT("SELECT * FROM tbl_act_code WHERE _CODE = '" + activationCode + "'").ValueString("_USED", 0) == "YES";

            if (levelOk) {
                codeCorrect = true;
            }
            else {
                sendMsg(3, "Activation Failed. You can not activate users that are higher than your position.", senderNumber);
            }

            return codeCorrect;
        }

        //TRANSFER LOAD WALLET TO CERTAIN NUMBER
        void transferPins(string sender, string receiver, int amount)
        {
            db.Query("UPDATE tbl_account set _PINS = _PINS - " + amount.ToString() + " WHERE _PHONE = '" + sender + "'");
            db.Query("UPDATE tbl_account set _PINS = _PINS + " + amount + " WHERE _PHONE = '" + receiver + "'");
        }

        void StopProcess()
        {
            for (int i = 0; i < _serialPort.Count; i++)
                _serialPort[i].Dispose();
            for (int i = 0; i < recMsgThread.Count; i++)
                recMsgThread[i].Abort();
            for (int i = 0; i < recMsgUSSDThread.Count; i++)
                recMsgUSSDThread[i].Abort();
            Environment.Exit(0);
        }

        //INITALIZE ALL PORTS
        void initPorts()
        {
            var tempData = db.QueryDT("SELECT * FROM tbl_config LIMIT 4, 8");
            for (int i = 0; i < tempData.RowCount(); i++)
            {
                coms.Add(tempData.ValueString("_VALUE", i));
                _serialPort.Add(new SerialPort(coms[_serialPort.Count], 115200));
                _serialPort[_serialPort.Count - 1].Open();
                _serialPort[_serialPort.Count - 1].Write("AT+CNMI=1,2,0,0,0\r"); // Receive notification of incoming messages
            }
        }
        /*********************************CUSTOM METHODS****************************************/

        #region QUERIES
        //SET CODE AS USED
        void useCode(string code, string user)
        {
            db.Query("UPDATE tbl_act_code SET _USED = 'YES', _USER = '" + user + "' WHERE _CODE = '" + code + "' AND _USED = 'NO'");
        }

        //INSERT PENDING LOAD
        void insertPending(string sender, string carrier, string phone, string credit, string message, string datetime, string refno)
        {
            db.Query("INSERT INTO tbl_pending SET _SENDER = '" + sender +
                "', _CARRIER = '" + carrier +
                "', _PHONE = '" + phone +
                "', _CREDIT = '" + credit +
                "', _MESSAGE = '" + message.Trim() +
                "', _REFNO = '" + refno +
                "', _DATETIME = '" + datetime + "'");
        }

        void recordIncome(string phoneNumber, decimal income, string type, string referenceNo, string dateTime) {
            db.Query("INSERT INTO tbl_income SET _PHONE = '" + phoneNumber +
                "', _INCOME = '" + income +
                "', _TYPE = '" + type +
                "', _REFERENCE_NO = '" + referenceNo +
                "', _DATETIME = '" + dateTime + "'");
        }

        //INSERT HELP
        void insertHelp(string sender, string referenceNo, string dateOfIncident, string code, string datetime, string status)
        {
            db.Query("INSERT INTO tbl_help SET _SENDER = '" + sender +
                "', _REFERENCE_NO = '" + referenceNo +
                "', _DATE_OF_INCIDENT = '" + dateOfIncident +
                "', _CODE = '" + code +
                "', _DATETIME = '" + datetime +
                "', _STATUS = '" + status +"'");
        }

        //MOVE PENDING LOAD TO HISTORY
        void moveToHistory(string sender, string carrier, string phone, string credit, string message, string refno, string trcno, string datetime)
        {
            db.Query("INSERT INTO tbl_history SET _SENDER = '" + sender +
                "', _CARRIER = '" + carrier +
                "', _PHONE = '" + phone +
                "', _CREDIT = '" + credit +
                "', _MESSAGE = '" + message +
                "', _DATETIME = '" + datetime +
                "', _TRCNO = '" + trcno +
                "', _REFNO = '" + refno + "'");

            db.Query("DELETE FROM tbl_pending WHERE _SENDER = '" + sender +
                "' AND _PHONE = '" + phone +
                "' AND _CREDIT = '" + credit +
                "' AND _MESSAGE = '" + message +
                "' AND _REFNO = '" + refno +
                "'");
        }

        //INSERT USER
        bool insertUser(string number, string pass, string fullname, string username, string bdate, string address, string role, string bal, string pins)
        {
            bool output = true;

            if (!usernameExists(username))
            {
                db.Query("INSERT INTO tbl_users SET _USERNAME = '" + username +
                    "', _PASSWORD = '" + pass +
                    "', _PHONE = '" + number +
                    "', _FULLNAME = '" + fullname +
                    "', _BDATE = '" + bdate +
                    "', _ADDRESS = '" + address +
                    "', _ROLE = '" + role + "'");

                db.Query("INSERT INTO tbl_account SET _PHONE = '" + number +
                    "', _BALANCE = '" + bal +
                    "', _PINS = '" + pins + "'");
            }
            else
                output = false;

            return output;
        }

        //INSERT MESSAGE
        void insertMessage(string sender, string carrier, string message, string datetime, string refno)
        {
            db.Query("INSERT INTO tbl_messages SET _SENDER = '" + sender +
                "', _CARRIER = '" + carrier +
                "', _MESSAGE = '" + message +
                "', _DATETIME = '" + datetime +
                "', _REFNO = '" + refno + "'");
        }

        //INSERT ACTIVATED RETAILER
        void insertActivatedRetailer(string activatorNumber, string username, string phoneNumber, string fullName, string pins, string referenceNumber) {
            
            db.Query("INSERT INTO tbl_act_ret SET _ACTIVATOR_NUMBER = '" + activatorNumber +
                "', _USERNAME = '" + username +
                "', _PHONE = '" + phoneNumber +
                "', _NAME = '" + fullName +
                "', _PINS = '" + pins +
                "', _REFNO = '" + referenceNumber + "'");
        }

        //INSERT TLC History
        void insertTLCHistory(string sender, string senderbal, string receiver, string receiverbal, string refno, string datetime, string amount)
        {
            db.Query("INSERT INTO tbl_tlc SET _SENDER = '" + sender +
                "', _SENDER_BAL = '" + senderbal +
                "', _RECEIVER = '" + receiver +
                "', _RECEIVER_BAL = '" + receiverbal +
                "', _REFNO = '" + refno +
                "', _DATETIME = '" + datetime + "', _AMOUNT = '" + amount + "'");
        }

        //INSERT PIN Transfer History
        void insertPINHistory(string sender, string amt, string receiver, string name, string pins, string refno, string datetime)
        {
            db.Query("INSERT INTO tbl_pin SET _SENDER = '" + sender +
                "', _PIN_AMT = '" + amt +
                "', _RECEIVER = '" + receiver +
                "', _NAME = '" + name +
                "', _PINS = '" + pins +
                "', _REFNO = '" + refno +
                "', _DATETIME = '" + datetime + "'");
        }
        #endregion
    }

    public class DatabaseHandler
    {
        MySqlConnection conn = new MySqlConnection("user id=root; password=''; Convert Zero Datetime=True; CharSet=utf8; server='localhost'; database=allload; port=3306;");
        DataTable _DATA = new DataTable();

        public void Query(string query)
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();

            conn.Open();
            using (MySqlCommand comm = new MySqlCommand(query, conn)) //IMPORTANT TO USE "USING" CLAUSE TO PREVENT MEMORY LEAKAGE, IT GIVES GOOD BENEFITS ALWAYS
            {
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(comm))//SIMPLE, NOT TIME CONSUMING
                {
                    adapter.Fill(_DATA);
                }
            }
            conn.Close();
        }

        public DataTable QueryDT(string query)
        {
            DataTable dt = new DataTable();

            if (conn.State == ConnectionState.Open)
                conn.Close();

            conn.Open();
            using (MySqlCommand comm = new MySqlCommand(query, conn))
            {
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(comm))
                {
                    adapter.Fill(dt);
                }
            }
            conn.Close();
            return dt;
        }

        public DataTable DATA
        {
            get { return _DATA; }
        }
    }

    public static class InputHelper
    {
        //OTHER DATAGRID VIEW HELPERS

        // THIS SECTION JUST SIMPLIFIES EX. dgvData["NAME", i].Value.ToString() TO ===>> dgvData.ValueString("NAME", i)
        //    OR SOMETHING LIKE THIS TOO. int.Parse(dt.Rows[i]["AGE"].ToString())  TO ===>> dt.ValueInt("AGE", i)
        // SHORTER, VERBALLY UNDERSTANDABLE, MORE TIME TO CODE

        #region GET DATAGRIDVIEW DATA VALUE AS STRING

        public static string ValueString(this DataGridView dgv, string colName, int row)
        {
            string res = "";
            try
            {
                res = dgv[colName, row].Value.ToString().Trim();
            }
            catch { }
            return res;
        }

        public static string ValueString(this DataGridView dgv, int col, int row)
        {
            string res = "";
            try
            {
                res = dgv[col, row].Value.ToString().Trim();
            }
            catch { }
            return res;
        }

        #endregion

        #region GET DATAGRIDVIEW DATA VALUE AS DECIMAL

        public static decimal ValueDecimal(this DataGridView dgv, string colName, int row)
        {
            decimal res = 0m;
            try
            {
                res = decimal.Parse(dgv[colName, row].Value.ToString().Trim());
            }
            catch { }
            return res;
        }

        public static decimal ValueDecimal(this DataGridView dgv, int col, int row)
        {
            decimal res = 0m;
            try
            {
                res = decimal.Parse(dgv[col, row].Value.ToString().Trim());
            }
            catch { }
            return res;
        }

        #endregion

        #region GET DATAGRIDVIEW DATA VALUE AS INTEGER

        public static int ValueInt(this DataGridView dgv, string colName, int row)
        {
            int res = 0;
            try
            {
                res = int.Parse(dgv[colName, row].Value.ToString().Trim());
            }
            catch { }
            return res;
        }

        public static int ValueInt(this DataGridView dgv, int col, int row)
        {
            int res = 0;
            try
            {
                res = int.Parse(dgv[col, row].Value.ToString().Trim());
            }
            catch { }
            return res;
        }

        #endregion

        #region GET DATAGRIDVIEW DATA VALUE AS BOOL

        public static bool ValueBool(this DataGridView dgv, string colName, int row)
        {
            bool res = false;
            try
            {
                res = dgv[colName, row].Value.ToString().Trim().Equals("True", StringComparison.OrdinalIgnoreCase);
            }
            catch { }
            return res;
        }

        public static bool ValueBool(this DataGridView dgv, int col, int row)
        {
            bool res = false;
            try
            {
                res = dgv[col, row].Value.ToString().Trim().Equals("True", StringComparison.OrdinalIgnoreCase);
            }
            catch { }
            return res;
        }

        #endregion

        #region GET DATAGRIDVIEW DATA AS DATETIME OR STRING DATE

        public static string ValueDateString(this DataGridView dgv, int col, int row, string customFormat)
        {
            string res = "";
            try
            {
                res = DateTime.Parse(dgv[col, row].Value.ToString().Trim()).ToString(customFormat);
            }
            catch { }
            return res;
        }

        public static string ValueDateString(this DataGridView dgv, string colName, int row, string customFormat)
        {
            string res = "";
            try
            {
                res = DateTime.Parse(dgv[colName, row].Value.ToString().Trim()).ToString(customFormat);
            }
            catch { }
            return res;
        }

        public static DateTime ValueDateTime(this DataGridView dgv, int col, int row)
        {
            DateTime res = new DateTime();
            try
            {
                res = DateTime.Parse(dgv[col, row].Value.ToString().Trim());
            }
            catch { }
            return res;
        }

        public static DateTime ValueDateTime(this DataGridView dgv, string colName, int row)
        {
            DateTime res = new DateTime();
            try
            {
                res = DateTime.Parse(dgv[colName, row].Value.ToString().Trim());
            }
            catch { }
            return res;
        }

        #endregion

        public static bool IsDigitsOnly(this string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public static string GenRefNo
        {
            get
            {
                Random rnd = new Random();
                string date = DateTime.Now.ToString("yyyy-MMddhhmm-{0}");
                return string.Format(date, rnd.Next(0, 1000).ToString("0000"));
            }
        }

        public static string toFinancial(this string input)
        {
            string output = "";
            try
            {
                output = decimal.Parse(input).ToString("P #,##0.00");
            }
            catch { }
            return output;
        }

        public static bool EqualsIgnoreCase(this string input, string compare)
        {
            return input.Equals(compare, StringComparison.OrdinalIgnoreCase);
        }

        public static string ValueString(this DataTable dt, string colName, int row)
        {
            string res = "";
            try
            {
                res = dt.Rows[row][colName].ToString().Trim();
            }
            catch { }
            return res;
        }

        public static string ValueString(this DataTable dt, int col, int row)
        {
            string res = "";
            try
            {
                res = dt.Rows[row][col].ToString().Trim();
            }
            catch { }
            return res;
        }

        public static decimal ValueDecimal(this DataTable dt, string colName, int row)
        {
            decimal res = 0m;
            try
            {
                res = decimal.Parse(dt.Rows[row][colName].ToString().Trim());
            }
            catch { }
            return res;
        }

        public static decimal ValueDecimal(this DataTable dt, int col, int row)
        {
            decimal res = 0m;
            try
            {
                res = decimal.Parse(dt.Rows[row][col].ToString().Trim());
            }
            catch { }
            return res;
        }

        public static int ValueInt(this DataTable dt, string colName, int row)
        {
            int res = 0;
            try
            {
                res = int.Parse(dt.Rows[row][colName].ToString().Trim());
            }
            catch { }
            return res;
        }

        public static int ValueInt(this DataTable dt, int col, int row)
        {
            int res = 0;
            try
            {
                res = int.Parse(dt.Rows[row][col].ToString().Trim());
            }
            catch { }
            return res;
        }

        public static bool ValueBool(this DataTable dt, string colName, int row)
        {
            bool res = false;
            try
            {
                res = bool.Parse(dt.Rows[row][colName].ToString().Trim());
            }
            catch { }
            return res;
        }

        public static bool ValueBool(this DataTable dt, int col, int row)
        {
            bool res = false;
            try
            {
                res = bool.Parse(dt.Rows[row][col].ToString().Trim());
            }
            catch { }
            return res;
        }

        public static string ValueDateString(this DataTable dt, int col, int row, string customFormat)
        {
            string res = "";
            try
            {
                res = DateTime.Parse(dt.Rows[row][col].ToString().Trim()).ToString(customFormat);
            }
            catch { }
            return res;
        }

        public static string ValueDateString(this DataTable dt, string colName, int row, string customFormat)
        {
            string res = "";
            try
            {
                res = DateTime.Parse(dt.Rows[row][colName].ToString().Trim()).ToString(customFormat);
            }
            catch { }
            return res;
        }

        public static DateTime ValueDateTime(this DataTable dt, int col, int row)
        {
            DateTime res = new DateTime();
            try
            {
                res = DateTime.Parse(dt.Rows[row][col].ToString().Trim());
            }
            catch { }
            return res;
        }

        public static DateTime ValueDateTime(this DataTable dt, string colName, int row)
        {
            DateTime res = new DateTime();
            try
            {
                res = DateTime.Parse(dt.Rows[row][colName].ToString().Trim());
            }
            catch { }
            return res;
        }

        public static int RowCount(this DataTable dt)
        {
            return dt.Rows.Count;
        }

        public static object[] ToObjArr(this DataTable dt, int index)
        {
            List<object> output = new List<object>();
            for (int i = 0; i < dt.Columns.Count; i++)
                output.Add(dt.ValueString(i, index));
            return output.ToArray();
        }
    }
}
