using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.IO;
using System.Data.SqlClient;
using System.Xml;

namespace QRPublish {
    public partial class FMain : Form {
        public PrintDocument printDocument;
        public StringReader lineReader;

        public string sqlConnectionString = "";
        public SqlConnection sqlCon = null;
        public SqlCommand sqlCmd;
        public SqlDataReader sqlResult = null;

        public TamPubWin1.LogFile log = new TamPubWin1.LogFile();

        public ThoughtWorks.QRCode.Codec.QRCodeEncoder encoder;
        public PrintSetting p = new PrintSetting();

        public FMain() {
            InitializeComponent();
            printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(this.printDocument_PrintPage);
            printDocument.BeginPrint += new PrintEventHandler(printDocument_BeginPrint);
            printDocument.QueryPageSettings += new QueryPageSettingsEventHandler(printDocument_QueryPageSettings);
            log.richTextBox = richTextBox1;
            encoder = new ThoughtWorks.QRCode.Codec.QRCodeEncoder();
            encoder.QRCodeEncodeMode = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ENCODE_MODE.BYTE;//二维码编码方式
            encoder.QRCodeScale = 10; //每个小方格的宽度
            encoder.QRCodeVersion = 5; //二维码版本号
            encoder.QRCodeErrorCorrect = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.M;//纠错码等级

            sqlCon = new SqlConnection();
            sqlCon.ConnectionString = "server=.;user id=sa;pwd=1qazxsw2;database=QRTomahawk";
            try {
                sqlCon.Open();
            } catch (Exception ee) {
                log.writeLogError("无法连接数据库,errorString=" + ee.Message);
                return;
            }
            sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlCon;
            log.writeLogCommon("数据库连接成功");

        }

        void printDocument_QueryPageSettings(object sender, QueryPageSettingsEventArgs e) {
            //e.PageSettings.PaperSize.
        }

        void printDocument_BeginPrint(object sender, PrintEventArgs e) {
            if (!initPrint()) {
                e.Cancel = true;
                return;
            }
        }

        private bool initPrint() {
            if (!p.valid) {
                log.writeLogWarning("printSetting数据无效");
                return false;
            }

            //载入打印范围
            try {
                p.printPageStart = Convert.ToInt32(textBox7.Text);
                p.printPageCount = Convert.ToInt32(textBox8.Text);
            } catch {
                log.writeLogWarning("输入的页码范围错误");
                return false;
            }
            if ((p.printPageStart < 1) || (p.printPageStart > p.batchPageCount)) {
                log.writeLogWarning("需要打印的页码超出范围");
                return false;
            }
            if ((p.printPageCount < 1) || ((p.printPageStart + p.printPageCount - 1) > p.batchPageCount)) {
                log.writeLogWarning("需要打印的页数超出范围");
                return false;
            }
            p.currentPageNumber = p.printPageStart;


            //查询基本信息
            if (sqlResult != null) sqlResult.Close();
            sqlCmd.CommandText = "select * from manufacturerInfo where accountName='" + TamPub1.Etc.sqlEncode(p.accountName) + "'";
            try {
                sqlResult = sqlCmd.ExecuteReader();
            } catch (Exception ee) {
                log.writeLogError("启动打印过程中查询数据库出错，errorString=" + ee.Message);
                return false;
            }
            if (!sqlResult.Read()) {
                log.writeLogWarning("用户账号不存在，无法开始打印：" + textBox1.Text);
                return false;
            }
            p.accountName = textBox1.Text;
            p.accountCnName = Convert.ToString(sqlResult["cnName"]);
            p.accountWebSite = Convert.ToString(sqlResult["website"]);
            p.accountContact = Convert.ToString(sqlResult["contactName"]);
            p.accountTel = Convert.ToString(sqlResult["contactTel"]);

            //查询二维码信息
            if (sqlResult != null) sqlResult.Close();
            sqlCmd.CommandText =
                "select * from qrMain where accountName='" + TamPub1.Etc.sqlEncode(p.accountName) + "'" +
                " and batchSn=" + p.batchId +
                " and qrSn>=" + p.currentPageStartSn +
                " and qrSn<" + (p.currentPageStartSn + p.printQrCount) +
                " order by qrSn";
            try {
                sqlResult = sqlCmd.ExecuteReader();
            } catch (Exception ee) {
                log.writeLogError("启动打印过程中查询数据库出错，errorString=" + ee.Message);
                return false;
            }
            return true;
        }

        private void printDocument_PrintPage(object sender, PrintPageEventArgs e) {
            double dpmm = e.PageBounds.Width / p.paperXSizeMM;
            int fontSize = Convert.ToInt32(p.titleSizeMM / 3 * dpmm);

            //打印题头信息
            int textX, textY;
            textX = Convert.ToInt32((p.xBorderSizeMM + 11f) * dpmm);
            textY = Convert.ToInt32((p.yBorderSizeMM + 0.3f) * dpmm);
            e.Graphics.DrawString("批号：" + p.batchId +
                "，本页序号：" + p.currentPageStartSn +
                "-" + (p.currentPageStartSn + p.pageSize - 1) +
                "，批次序号：" + p.batchSnStart + "-" + (p.batchSnStart + p.batchQrCount - 1),
                new Font("黑体", fontSize * 3 / 4), new SolidBrush(Color.Black), new PointF(textX, textY));
            textY += fontSize * 3 / 2;
            e.Graphics.DrawString("第" + p.currentPageNumber + "页 共" + p.batchPageCount + "页" +
                "，本页套数：" + p.pageSize + "，批次套数：" + p.batchQrCount,
                new Font("黑体", fontSize * 3 / 4), new SolidBrush(Color.Black), new PointF(textX, textY));

            textX = Convert.ToInt32(e.PageBounds.Width * 0.45);
            textY = Convert.ToInt32((p.yBorderSizeMM + 0.3f) * dpmm);
            e.Graphics.DrawString("账户名：" + p.accountName +
                "，中文名：" + p.accountCnName,
                new Font("黑体", fontSize * 3 / 4), new SolidBrush(Color.Black), new PointF(textX, textY));
            textY += fontSize * 3 / 2;
            e.Graphics.DrawString("联系人：" + p.accountContact +
                ",联系电话：" + p.accountTel,
                new Font("黑体", fontSize * 3 / 4), new SolidBrush(Color.Black), new PointF(textX, textY));

            textY += fontSize * 3 / 2;

            //题头横线
            //e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, textY, e.PageBounds.Width, fontSize / 8));

            //查询数据库打印二维码
            Bitmap bitmap;
            Bitmap bg = new Bitmap(p.img);
            int dx, dy;
            int sfontSize = Convert.ToInt32((p.qrYSizeMM - p.qrXSizeMM) / 2 * dpmm);
            for (int y = 0; y < p.yCount; y++) {
                for (int x = 0; x < p.xCount; x++) {
                    if (!sqlResult.Read()) {
                        e.HasMorePages = false;
                        return;
                    }
                    dx = Convert.ToInt32((p.xBorderSizeMM + x * p.imgWidthMM) * dpmm);
                    dy = Convert.ToInt32((p.yBorderSizeMM + p.titleSizeMM + y * p.imgHeightMM) * dpmm);
                    if (p.printCutter) {
                        e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(dx, dy, Convert.ToInt32(p.qrXSizeMM * dpmm), Convert.ToInt32(p.qrYSizeMM * dpmm)));
                        continue;
                    }
                    //背景图片
                    e.Graphics.DrawImage(bg, dx, dy, Convert.ToInt32(p.imgWidthMM * dpmm), Convert.ToInt32(p.imgHeightMM * dpmm));
                    //二维码
                    bitmap = encoder.Encode("http://www.ma315.com/q/q.htm?an=" + p.accountName + "&c2=" + Convert.ToString(sqlResult["qr2"]));
                    e.Graphics.DrawImage(bitmap,
                        new Rectangle(
                            Convert.ToInt32(dx + p.qrPos.X * dpmm),
                            Convert.ToInt32(dy + p.qrPos.Y * dpmm),
                            Convert.ToInt32(p.qrPos.Width * dpmm),
                            Convert.ToInt32(p.qrPos.Height * dpmm)
                        )
                    );
                    bitmap.Dispose();
                    //序列号
                    System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
                    drawFormat.Alignment = System.Drawing.StringAlignment.Center;
                    drawFormat.LineAlignment = System.Drawing.StringAlignment.Center;
                    string s = Convert.ToString(sqlResult["qrBarcode"]);
                    s =
                        TamPub1.StringFunc.copy(s, 0, 3) + " " +
                        TamPub1.StringFunc.copy(s, 3, 3) + " " +
                        TamPub1.StringFunc.copy(s, 6, 3) + " " +
                        TamPub1.StringFunc.copy(s, 9, 30);
                    e.Graphics.DrawString(
                        s,
                        new System.Drawing.Font("Arial", Convert.ToSingle(p.snPos.Height / 1.5 * dpmm * 3 / 4), FontStyle.Bold),
                        new System.Drawing.SolidBrush(Color.Black),
                        new System.Drawing.Rectangle(
                            Convert.ToInt32(dx + p.snPos.X * dpmm),
                            Convert.ToInt32(dy + p.snPos.Y * dpmm + p.snPos.Height * 0.05 * dpmm),
                            Convert.ToInt32(p.snPos.Width * dpmm),
                            Convert.ToInt32(p.snPos.Height * dpmm)
                        ),
                        drawFormat
                    );
                    /*
                    bitmap = encoder.Encode("http://www.zhongshuo.cn/int.aspx?aaa=" + Convert.ToString(sqlResult["qr2"]));
                    e.Graphics.DrawImage(bitmap, new Rectangle(x * xSize + borderWidth, y * ySize + borderWidth + titleSize + xSize + xSize * 3 / 24, xSize * 5 / 6, xSize * 5 / 6));
                    bitmap.Dispose();
                    */
                }
            }

            //对位标
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(
                0, 0, Convert.ToInt32(10 * dpmm), Convert.ToInt32(1 * dpmm)
            ));
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(
                0, 0, Convert.ToInt32(1 * dpmm), Convert.ToInt32((p.titleSizeMM - 1) * dpmm)
            ));
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(
                Convert.ToInt32((p.pageXSizeMM - 10) * dpmm), 0, Convert.ToInt32(10 * dpmm), Convert.ToInt32(1 * dpmm)
            ));
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(
                Convert.ToInt32((p.pageXSizeMM - 1) * dpmm), 0, Convert.ToInt32(1 * dpmm), Convert.ToInt32((p.titleSizeMM - 1) * dpmm)
            ));
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(
                0, Convert.ToInt32((p.pageYSizeMM - 1) * dpmm), Convert.ToInt32(10 * dpmm), Convert.ToInt32(1 * dpmm)
            ));
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(
                0, Convert.ToInt32((p.pageYSizeMM - 2) * dpmm), Convert.ToInt32(1 * dpmm), Convert.ToInt32(2 * dpmm)
            ));
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(
                Convert.ToInt32((p.pageXSizeMM - 10) * dpmm), Convert.ToInt32((p.pageYSizeMM - 1) * dpmm), Convert.ToInt32(10 * dpmm), Convert.ToInt32(1 * dpmm)
            ));
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(
                Convert.ToInt32((p.pageXSizeMM - 1) * dpmm), Convert.ToInt32((p.pageYSizeMM - 2) * dpmm), Convert.ToInt32(1 * dpmm), Convert.ToInt32(2 * dpmm)
            ));
            //外框
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(
                0, 0, Convert.ToInt32(p.pageXSizeMM * dpmm), 1
            ));
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(
                0, 0, 1, Convert.ToInt32(p.pageYSizeMM * dpmm)
            ));
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(
                Convert.ToInt32(p.pageXSizeMM * dpmm) - 1, 0, 1, Convert.ToInt32(p.pageYSizeMM * dpmm)
            ));
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(
                0, Convert.ToInt32(p.pageYSizeMM * dpmm) - 1, Convert.ToInt32(p.pageXSizeMM * dpmm), 1
            ));

            p.currentPageNumber++;
            if (p.currentPageNumber >= p.printPageStart + p.printPageCount) {
                e.HasMorePages = false;
            } else {
                e.HasMorePages = true;
            }
        }

        private void loadConfig() {
            string filename = TamPub1.FileOperation.currentFilePath + "config.xml";
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try {
                doc.Load(filename);
            } catch (Exception) {
                return;
            }
            System.Xml.XmlNode node;
            Rectangle rect = Screen.GetWorkingArea(this);
            string section = "desktop" + rect.Width + "x" + rect.Height;
            //database connection string
            node = doc.SelectSingleNode("/config/databaseConnectionString");
            if (node != null) {
                sqlConnectionString = TamPub1.ConfigFileXml.readString(node, "server=.;user id=sa;pwd=1qazxsw2;database=QRTomahawk");
            }
            //窗口尺寸
            node = doc.SelectSingleNode("/config/" + section + "/mainWidth");
            if (node != null) {
                Width = TamPub1.ConfigFileXml.readInt32(node, 100);
            }
            node = doc.SelectSingleNode("/config/" + section + "/mainHeight");
            if (node != null) {
                Height = TamPub1.ConfigFileXml.readInt32(node, 100);
            }
            node = doc.SelectSingleNode("/config/" + section + "/mainLeft");
            if (node != null) {
                Left = TamPub1.ConfigFileXml.readInt32(node, 100);
            }
            node = doc.SelectSingleNode("/config/" + section + "/mainTop");
            if (node != null) {
                Top = TamPub1.ConfigFileXml.readInt32(node, 100);
            }
            //窗口状态
            node = doc.SelectSingleNode("/config/" + section + "/mainState");
            if (node != null) {
                string state = TamPub1.ConfigFileXml.readString(node, "Normal");
                if (state.Equals("Normal")) {
                    WindowState = FormWindowState.Normal;
                } else if (state.Equals("Maximized")) {
                    WindowState = FormWindowState.Maximized;
                }
            }
            //textbox框
            node = doc.SelectSingleNode("/config/" + section + "/textBox1");
            if (node != null) {
                textBox1.Text = TamPub1.ConfigFileXml.readString(node, "");
            }
            node = doc.SelectSingleNode("/config/" + section + "/textBox2");
            if (node != null) {
                textBox2.Text = TamPub1.ConfigFileXml.readString(node, "");
            }
            node = doc.SelectSingleNode("/config/" + section + "/textBox5");
            if (node != null) {
                textBox5.Text = TamPub1.ConfigFileXml.readString(node, "");
            }
        }

        private void saveConfig() {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            System.Xml.XmlNode root, sectionNode, node;
            root = doc.AppendChild(doc.CreateElement("config"));
            Rectangle rect = Screen.GetWorkingArea(this);
            string section = "desktop" + rect.Width + "x" + rect.Height;
            sectionNode = root.AppendChild(doc.CreateElement(section));
            //窗口状态
            node = sectionNode.AppendChild(doc.CreateElement("mainState"));
            if (WindowState == FormWindowState.Maximized) {
                node.InnerXml = "Maximized";
                WindowState = FormWindowState.Normal;
            } else {
                if (WindowState == FormWindowState.Minimized) {
                    WindowState = FormWindowState.Normal;
                }
                node.InnerXml = "Normal";
            }
            //窗口尺寸
            node = sectionNode.AppendChild(doc.CreateElement("mainWidth"));
            node.InnerXml = Width.ToString();
            node = sectionNode.AppendChild(doc.CreateElement("mainHeight"));
            node.InnerXml = Height.ToString();
            node = sectionNode.AppendChild(doc.CreateElement("mainLeft"));
            node.InnerXml = Left.ToString();
            node = sectionNode.AppendChild(doc.CreateElement("mainTop"));
            node.InnerXml = Top.ToString();
            //textBox框
            node = sectionNode.AppendChild(doc.CreateElement("textBox1"));
            node.InnerXml = textBox1.Text;
            node = sectionNode.AppendChild(doc.CreateElement("textBox2"));
            node.InnerXml = textBox2.Text;
            node = sectionNode.AppendChild(doc.CreateElement("textBox5"));
            node.InnerXml = textBox5.Text;

            //存盘
            string filename = TamPub1.FileOperation.currentFilePath + "config.xml";
            if (System.IO.File.Exists(filename)) {
                System.IO.File.Delete(filename);
            }
            doc.Save(filename);
        }

        private bool loadModule(string filename) {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try {
                doc.Load(filename);
            } catch (Exception) {
                return false;
            }
            System.Xml.XmlNode node;

            node = doc.SelectSingleNode("/module/imgName");
            if (node == null) return false;
            p.img = TamPub1.FileOperation.extractFilePath(filename) + TamPub1.ConfigFileXml.readString(node, "");
            node = doc.SelectSingleNode("/module/imgWidthMM");
            if (node == null) return false;
            try {
                p.imgWidthMM = Convert.ToDouble(TamPub1.ConfigFileXml.readString(node, ""));
            } catch {
                return false;
            }
            node = doc.SelectSingleNode("/module/imgHeightMM");
            if (node == null) return false;
            try {
                p.imgHeightMM = Convert.ToDouble(TamPub1.ConfigFileXml.readString(node, ""));
            } catch {
                return false;
            }
            //qrPos
            node = doc.SelectSingleNode("/module/qrPos/left");
            if (node == null) return false;
            try {
                p.qrPos.X = Convert.ToSingle(TamPub1.ConfigFileXml.readString(node, ""));
            } catch {
                return false;
            }
            node = doc.SelectSingleNode("/module/qrPos/top");
            if (node == null) return false;
            try {
                p.qrPos.Y = Convert.ToSingle(TamPub1.ConfigFileXml.readString(node, ""));
            } catch {
                return false;
            }
            node = doc.SelectSingleNode("/module/qrPos/width");
            if (node == null) return false;
            try {
                p.qrPos.Width = Convert.ToSingle(TamPub1.ConfigFileXml.readString(node, ""));
            } catch {
                return false;
            }
            node = doc.SelectSingleNode("/module/qrPos/height");
            if (node == null) return false;
            try {
                p.qrPos.Height = Convert.ToSingle(TamPub1.ConfigFileXml.readString(node, ""));
            } catch {
                return false;
            }
            //snPos
            node = doc.SelectSingleNode("/module/snPos/left");
            if (node == null) return false;
            try {
                p.snPos.X = Convert.ToSingle(TamPub1.ConfigFileXml.readString(node, ""));
            } catch {
                return false;
            }
            node = doc.SelectSingleNode("/module/snPos/top");
            if (node == null) return false;
            try {
                p.snPos.Y = Convert.ToSingle(TamPub1.ConfigFileXml.readString(node, ""));
            } catch {
                return false;
            }
            node = doc.SelectSingleNode("/module/snPos/width");
            if (node == null) return false;
            try {
                p.snPos.Width = Convert.ToSingle(TamPub1.ConfigFileXml.readString(node, ""));
            } catch {
                return false;
            }
            node = doc.SelectSingleNode("/module/snPos/height");
            if (node == null) return false;
            try {
                p.snPos.Height = Convert.ToSingle(TamPub1.ConfigFileXml.readString(node, ""));
            } catch {
                return false;
            }
            return true;
        }

        private void button2_Click(object sender, EventArgs e) {
            PageSetupDialog pageSetupDialog = new PageSetupDialog();
            pageSetupDialog.Document = printDocument;
            pageSetupDialog.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e) {
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.Document = printDocument;
            try {
                printPreviewDialog.ShowDialog();
            } catch (Exception excep) {
                MessageBox.Show(excep.Message, "打印出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            log.writeLogCommon("启动打印预览，" + p.batchInfoEx);
        }



        private void button4_Click(object sender, EventArgs e) {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument;
            if (printDialog.ShowDialog() == DialogResult.OK) {
                try {
                    printDocument.Print();
                } catch (Exception excep) {
                    MessageBox.Show(excep.Message, "打印出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    printDocument.PrintController.OnEndPrint(printDocument, new PrintEventArgs());
                }
            }
        }

        private void button5_Click(object sender, EventArgs e) {
            textBox1.Text = TamPub1.EncryptEx.encrypt40bit(0, 5).ToString().PadLeft(13, '0');
        }

        private void button7_Click(object sender, EventArgs e) {
            //检测用户名
            if (sqlResult != null) sqlResult.Close();
            sqlCmd.CommandText = "select * from manufacturerInfo where accountName = '" + TamPub1.Etc.sqlEncode(textBox1.Text) + "'";
            try {
                sqlResult = sqlCmd.ExecuteReader();
                if (!sqlResult.Read()) {
                    log.writeLogWarning("用户不存在");
                    return;
                }

            } catch (Exception ee) {
                log.writeLogError("数据库执行错误,errorString=" + ee.Message);
                return;
            }
            //获取费率和其他信息并计算费率
            double balance = Convert.ToDouble(sqlResult["balance"]);
            double rate = Convert.ToDouble(sqlResult["rate"]);
            Int64 qrSn = Convert.ToInt64(sqlResult["qrSn"]);
            UInt32 qrKey = Convert.ToUInt32(sqlResult["qrKey"]);
            int batchSn = Convert.ToInt32(sqlResult["batchSn"]);
            int count;
            try {
                count = Convert.ToInt32(textBox2.Text);
            } catch {
                log.writeLogWarning("产生二维码的数量是一个无效的整数");
                return;
            }
            if (rate * count > balance) {
                log.writeLogWarning("账户余额不足,balance=" + balance.ToString() + ",rate=" + rate.ToString());
                return;
            }
            if (MessageBox.Show("产生" + count + "组二维码费用" + rate * count + "元,当前账户余额" + balance + "元，消费后余额将变为" + (balance - rate * count) + "元，" +
                "您是否继续？", "消费确认", MessageBoxButtons.OKCancel) != DialogResult.OK) {
                log.writeLogWarning("用户取消了产生二维码的操作");
                return;
            }

            //产生二维码
            log.writeLogCommon("开始产生二维码,SN=" + qrSn + ",Key=" + qrKey + ",count=" + count);
            StringBuilder s = new StringBuilder();
            Int64 code1, code2;
            for (int i = 0; i < count; i++) {
                code1 = TamPub1.EncryptEx.encrypt40bit(qrSn, qrKey);
                code2 = TamPub1.EncryptEx.encrypt40bit(qrSn, qrKey ^ 0x55555555);
                s.Append("insert into qrMain (accountName, qrSn, qrBarcode, qr1, qr2, batchSn) values (" +
                    "'" + TamPub1.Etc.sqlEncode(textBox1.Text) + "'," +
                    qrSn + "," +
                    "'" + code1.ToString().PadLeft(13, '0') + "'," +
                    "'" + qrToString(code1) + "'," +
                    "'" + qrToString(code2) + "'," +
                    batchSn + "); ");
                qrSn++;
                if ((i == count - 1) || (i % 500) == 499) {
                    if (sqlResult != null) sqlResult.Close();
                    sqlCmd.CommandText = s.ToString();
                    s.Clear();
                    try {
                        sqlCmd.ExecuteNonQuery();
                        log.writeLogCommon("已产生" + (i + 1) + "/" + count + "组二维码");
                    } catch (Exception ee) {
                        log.writeLogError("产生二维码过程异常终止，i=" + i + ",error=" + ee.Message);
                        return;
                    }
                }
            }
            log.writeLogCommon("产生二维码完成");

            //扣费、写入日志、写入批次
            if (sqlResult != null) sqlResult.Close();
            sqlCmd.CommandText =
                //扣费,更新序号和批号
                "update manufacturerInfo set balance = balance - " + (rate * count) +
                ", qrSn=qrSn+" + count +
                ", batchSn=batchSn+1" +
                " where accountName = '" + TamPub1.Etc.sqlEncode(textBox1.Text) + "'; " +
                //日志
                "insert into qrLog (logType,operator,accountName,param1,param2,param3,note) values (" +
                "'消费'," +
                "'后台打印'," +
                "'" + TamPub1.Etc.sqlEncode(textBox1.Text) + "'," +
                "'" + (rate * count) + "'," + //消费金额
                "'" + (balance - rate * count) + "'," + //消费后余额
                "'" + batchSn + "'," + //批号
                "'" + "产生" + count + "组二维码,批号:" + batchSn + ",费率" + rate + "元，费用" + rate * count + "元，当前账户余额" + balance + "元，消费后余额" + (balance - rate * count) + "元'); " +
                //批次信息
                "insert into qrBatchInfo (accountName,batchId,qrSnStart,qrSnCount,qrKey) values (" +
                "'" + TamPub1.Etc.sqlEncode(textBox1.Text) + "'," +
                batchSn + "," +
                (qrSn - count) + "," +
                count + "," +
                qrKey + "); ";
            try {
                sqlCmd.ExecuteNonQuery();
            } catch (Exception ee) {
                log.writeLogError("扣费并写入日志错误，errorString=" + ee.Message);
                return;
            }
            log.writeLogCommon("账户：" + textBox1.Text + "扣费完成，产生" + count + "组二维码，起始序号：" +
                (qrSn - count) + "，批号:" + batchSn + ",费率" + rate + "元，费用" + rate * count +
                "元，当前账户余额" + balance + "元，消费后余额" + (balance - rate * count) + "元");
        }

        private string qrToString(Int64 code) {
            string result = "";
            result = (code & 0xFF).ToString("X2") +
                ((code >> 8) & 0xFF).ToString("X2") +
                ((code >> 16) & 0xFF).ToString("X2") +
                ((code >> 24) & 0xFF).ToString("X2") +
                ((code >> 32) & 0xFF).ToString("X2");
            return result;
        }

        private void FMain_Load(object sender, EventArgs e) {
            loadConfig();
        }

        private void FMain_FormClosing(object sender, FormClosingEventArgs e) {
            saveConfig();
            e.Cancel = false;
        }

        private void button6_Click(object sender, EventArgs e) {
            p.valid = false;
            p.accountName = textBox1.Text;
            try {
                p.batchId = Convert.ToInt32(textBox5.Text);
            } catch {
                log.writeLogWarning("批号输入非法");
                return;
            }

            //查询批号信息
            if (sqlResult != null) sqlResult.Close();
            sqlCmd.CommandText = "select * from [qrBatchInfo] where " +
                "accountName = '" + TamPub1.Etc.sqlEncode(p.accountName) + "' " +
                "and batchId = " + p.batchId;
            try {
                sqlResult = sqlCmd.ExecuteReader();
                if (!sqlResult.Read()) {
                    log.writeLogWarning("批号不存在");
                    return;
                }
            } catch (Exception ee) {
                log.writeLogError("数据库执行错误,errorString=" + ee.Message);
                return;
            }
            p.batchSnStart = Convert.ToInt64(sqlResult["qrSnStart"]);
            p.batchQrCount = Convert.ToInt32(sqlResult["qrSnCount"]);
            log.writeLogCommon("批号查询完成, " + p.batchInfoEx);
            p.valid = true;
            refreshPrintSetting();
        }

        private void refreshPrintSetting() {
            label3.Text = p.batchInfo;
            if (checkBox1.Checked != p.printInnerOnly) {
                checkBox1.Checked = p.printInnerOnly;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            p.printInnerOnly = checkBox1.Checked;
            refreshPrintSetting();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            p.printCutter = checkBox2.Checked;
        }

        private void button1_Click(object sender, EventArgs e) {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "二维码模块文件(*.qrm)|*.qrm";

            if (d.ShowDialog() != DialogResult.OK) {
                return;
            }
            if (!loadModule(d.FileName)) {
                log.writeLogWarning("载入模板失败, filename=" + d.FileName);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e) {

        }

    }

    public class PrintSetting {
        public string accountName = ""; //用户账号
        public string accountCnName = ""; //用户中文名
        public string accountWebSite = ""; //用户网站
        public string accountContact = ""; //用户联系人
        public string accountTel = ""; //用户联系电话
        public int batchId = 0; //批号
        public int currentPageNumber = 1; //当前页码1开始
        public int printPageStart = 1; //需要打印的起始页码
        public int printPageCount = 1; //需要打印的页数
        public Int64 batchSnStart = 1; //本批序号起始值
        public int batchQrCount = 1; //本批二维码总数
        //public int xCount = 8; //横向二维码数量
        public double pageXSizeMM = 203; //页面宽度毫米(可打印区域)
        public double pageYSizeMM = 289; //页面高度毫米(可打印区域)
        public double paperXSizeMM = 210; //纸张实际宽度
        public double xBorderSizeMM = 0; //左右边界宽度毫米
        public double yBorderSizeMM = 0; //上下边界占页面高毫米
        public double qrBorderRatio = 5.0 / 20; //二维码切割后边距与二维码宽度的比例值
        public double qrBottomRatio = 4.5 / 21; //二维码底部序列号文字区域高度和二维码大小的比例
        public double titleSizeMM = 8; //标题高度毫米
        public bool printInnerOnly = false; //是否只打印内标签
        public bool printCutter = false; //是否输出切模
        public bool valid = false;
        public string img = ""; //图片背景
        public double imgWidthMM = 40; //图片宽度和高度
        public double imgHeightMM = 19.5;
        public RectangleF qrPos = new RectangleF(3.9f, 3.5f, 12.9f, 12.9f); //二维码位置和大小
        public RectangleF snPos = new RectangleF(19.1f, 15.0f, 18.4f, 2.1f); //序列号位置和大小



        /// <summary>横向二维码数量</summary>
        public int xCount {
            get {
                return Convert.ToInt32(Math.Floor((pageXSizeMM - xBorderSizeMM * 2) / imgWidthMM));
            }
        }

        /// <summary>纵向二维码的数量</summary>
        public int yCount {
            get {
                return Convert.ToInt32(Math.Floor((pageYSizeMM - yBorderSizeMM * 2 - titleSizeMM) / imgHeightMM));
            }
        }

        /// <summary>每个二维码切割边距</summary>
        public double qrBorderMM {
            get {
                return qrBorderRatio * qrXSizeMM;
            }
        }

        /// <summary>当前页序号起始值</summary>
        public Int64 currentPageStartSn {
            get {
                return (currentPageNumber - 1) * pageSize + batchSnStart;
            }
        }

        /// <summary>本批次总页数</summary>
        public int batchPageCount {
            get {
                return batchQrCount / pageSize + 1;
            }
        }

        /// <summary>二维码x尺寸毫米</summary>
        public double qrXSizeMM {
            get {
                return (pageXSizeMM - xBorderSizeMM * 2) / (xCount + qrBorderRatio * (xCount - 1) * 2);
            }
        }
        /// <summary>二维码y尺寸毫米</summary>
        public double qrYSizeMM {
            get {
                return qrXSizeMM + qrXSizeMM * qrBottomRatio;
            }
        }

        /// <summary>每页二维码套数</summary>
        public int pageSize {
            get {
                return xCount * yCount;
            }
        }

        /// <summary>需要打印的二维码数量</summary>
        public int printQrCount {
            get {
                int r = (printPageStart + printPageCount - 1) * pageSize;
                r = Math.Min(r, batchQrCount);
                return r - (printPageStart - 1) * pageSize;
            }
        }

        /// <summary>本批次信息</summary>
        public string batchInfo {
            get {
                return "起始序号：" + batchSnStart + "，批次数量：" + batchQrCount +
                    "，总页数：" + batchPageCount + "，每页数量：" + pageSize;
            }
        }

        public string batchInfoEx {
            get {
                return "账户名：" + accountName + "，账户中文名：" + accountCnName +
                "，批号：" + batchId + "，" + batchInfo;
            }
        }
    }
}
